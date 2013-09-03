﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcTest.Models;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;

namespace MvcTest.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload
        public ActionResult Index()
        {
            return View("Upload");
        }

        //
        // GET: /Upload/Submit
        public ActionResult Submit()
        {
            var path = Server.MapPath("~/Uploads");
            var context = ControllerContext.HttpContext;
            var provider = (IServiceProvider)context;
            var workerRequest = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
            var verb = workerRequest.GetHttpVerbName();
            if(!verb.Equals("POST"))
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.SuppressContent = true;
                return View("Upload");
            }

            /*
            if(!context.User.Identity.IsAuthenticated)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                Response.SuppressContent = true;
                return View("Upload");
            }*/
            
            var encoding = context.Request.ContentEncoding;
            var processor = new UploadProcessor(workerRequest);
            processor.StreamToDisk(context, encoding, path);
            return View("Upload");
        }

    }

    internal class UploadProcessor
    {
        private byte[] _buffer;
        private byte[] _boundaryBytes;
        private byte[] _endHeaderBytes;
        private byte[] _endFileBytes;
        private byte[] _lineBreakBytes;
        private const string _lineBreak = "\r\n";
        private readonly Regex _filename =
            new Regex(@"Content-Disposition:\s*form-data\s*;\s*name\s*=\s*""file""\s*;\s*filename\s*=\s*""(.*)""",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly HttpWorkerRequest _workerRequest;

        public UploadProcessor(HttpWorkerRequest workerRequest)
        {
            _workerRequest = workerRequest;
        }

        public void StreamToDisk(IServiceProvider provider, Encoding encoding, string rootPath)
        {
            var buffer = new byte[8192];
            if (!_workerRequest.HasEntityBody())
            {
                return;
            }
            var total = _workerRequest.GetTotalEntityBodyLength();
            var preloaded = _workerRequest.GetPreloadedEntityBodyLength();
            var loaded = preloaded;
            SetByteMarkers(_workerRequest, encoding);
            var body = _workerRequest.GetPreloadedEntityBody();
            if (body == null) // IE normally does not preload          
            {
                body = new byte[8192];
                preloaded = _workerRequest.ReadEntityBody(body, body.Length);
                loaded = preloaded;
            }
            var text = encoding.GetString(body);
            var fileName = _filename.Matches(text)[0].Groups[1].Value;
            fileName = Path.GetFileName(fileName); // IE captures full user path; chop it           
            var path = Path.Combine(rootPath, fileName);
            var files = new List<String> { fileName };
            var stream = new FileStream(path, FileMode.Create);
            if (preloaded > 0)
            {
                stream = ProcessHeaders(body, stream, encoding, preloaded, files, rootPath);
            }

            // Used to force further processing (i.e. redirects) to avoid buffering the files again          
            var workerRequest = new StaticWorkerRequest(_workerRequest, body);
            var field = HttpContext.Current.Request.GetType().GetField("_wr", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(HttpContext.Current.Request, workerRequest);
            if (!_workerRequest.IsEntireEntityBodyIsPreloaded())
            {
                var received = preloaded;
                while (total - received >= loaded && _workerRequest.IsClientConnected())
                {
                    loaded = _workerRequest.ReadEntityBody(buffer, buffer.Length);
                    stream = ProcessHeaders(buffer, stream, encoding, loaded, files, rootPath);
                    received += loaded;
                }
                var remaining = total - received;
                buffer = new byte[remaining];
                loaded = _workerRequest.ReadEntityBody(buffer, remaining);
                stream = ProcessHeaders(buffer, stream, encoding, loaded, files, rootPath);
            }
            stream.Flush();
            stream.Close();
            stream.Dispose();
        }

        private void SetByteMarkers(HttpWorkerRequest workerRequest, Encoding encoding)
        {
            var contentType = workerRequest.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);
            var bufferIndex = contentType.IndexOf("boundary=") + "boundary=".Length;
            var boundary = String.Concat("--", contentType.Substring(bufferIndex));
            _boundaryBytes = encoding.GetBytes(string.Concat(boundary, _lineBreak));
            _endHeaderBytes = encoding.GetBytes(string.Concat(_lineBreak, _lineBreak));
            _endFileBytes = encoding.GetBytes(string.Concat(_lineBreak, boundary, "--", _lineBreak));
            _lineBreakBytes = encoding.GetBytes(string.Concat(_lineBreak + boundary + _lineBreak));
        }

        private FileStream ProcessHeaders(byte[] buffer, FileStream stream, Encoding encoding, int count, ICollection<string> files, string rootPath)
        {
            buffer = AppendBuffer(buffer, count);
            var startIndex = IndexOf(buffer, _boundaryBytes, 0);
            if (startIndex != -1)
            {
                var endFileIndex = IndexOf(buffer, _endFileBytes, 0);
                if (endFileIndex != -1)
                {
                    var precedingBreakIndex = IndexOf(buffer, _lineBreakBytes, 0);
                    if (precedingBreakIndex > -1)
                    {
                        startIndex = precedingBreakIndex;
                    }
                    endFileIndex += _endFileBytes.Length;
                    var modified = SkipInput(buffer, startIndex, endFileIndex, ref count);
                    stream.Write(modified, 0, count);
                }
                else
                {
                    var endHeaderIndex = IndexOf(buffer, _endHeaderBytes, 0);
                    if (endHeaderIndex != -1)
                    {
                        endHeaderIndex += _endHeaderBytes.Length;
                        var text = encoding.GetString(buffer);
                        var match = _filename.Match(text);
                        var fileName = match != null ? match.Groups[1].Value : null;
                        fileName = Path.GetFileName(fileName); // IE captures full user path; chop it                       
                        if (!string.IsNullOrEmpty(fileName) && !files.Contains(fileName))
                        {
                            files.Add(fileName);
                            var filePath = Path.Combine(rootPath, fileName);
                            stream = ProcessNextFile(stream, buffer, count, startIndex, endHeaderIndex, filePath);
                        }
                        else
                        {
                            var modified = SkipInput(buffer, startIndex, endHeaderIndex, ref count);
                            stream.Write(modified, 0, count);
                        }
                    }
                    else
                    {
                        _buffer = buffer;
                    }
                }
            }
            else
            {
                stream.Write(buffer, 0, count);
            }
            return stream;
        }

        private static FileStream ProcessNextFile(FileStream stream, byte[] buffer, int count, int startIndex, int endIndex, string filePath)
        {
            var fullCount = count;
            var endOfFile = SkipInput(buffer, startIndex, count, ref count);
            stream.Write(endOfFile, 0, count);
            stream.Flush();
            stream.Close();
            stream.Dispose();
            stream = new FileStream(filePath, FileMode.Create);
            var startOfFile = SkipInput(buffer, 0, endIndex, ref fullCount);
            stream.Write(startOfFile, 0, fullCount);
            return stream;
        }

        private static int IndexOf(byte[] array, IList<byte> value, int startIndex)
        {
            var index = 0;
            var start = Array.IndexOf(array, value[0], startIndex);
            if (start == -1)
            {
                return -1;
            }
            while ((start + index) < array.Length)
            {
                if (array[start + index] == value[index])
                {
                    index++;
                    if (index == value.Count)
                    {
                        return start;
                    }
                }
                else
                {
                    start = Array.IndexOf(array, value[0], start + index);
                    if (start != -1)
                    {
                        index = 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            return -1;
        }

        private static byte[] SkipInput(byte[] input, int startIndex, int endIndex, ref int count)
        {
            var range = endIndex - startIndex;
            var size = count - range;
            var modified = new byte[size];
            var modifiedCount = 0;
            for (var i = 0; i < input.Length; i++)
            {
                if (i >= startIndex && i < endIndex)
                {
                    continue;
                }
                if (modifiedCount >= size)
                {
                    break;
                }
                modified[modifiedCount] = input[i];
                modifiedCount++;
            }
            input = modified;
            count = modified.Length;
            return input;
        }

        private byte[] AppendBuffer(byte[] buffer, int count)
        {
            var input = new byte[_buffer == null ? buffer.Length : _buffer.Length + count];
            if (_buffer != null)
            {
                Buffer.BlockCopy(_buffer, 0, input, 0, _buffer.Length);
            }

            Buffer.BlockCopy(buffer, 0, input, _buffer == null ? 0 : _buffer.Length, count);
            _buffer = null;
            return input;
        }
    }

    internal class StaticWorkerRequest :  HttpWorkerRequest
    {
        readonly HttpWorkerRequest _request;
        private readonly byte[] _buffer;

        public StaticWorkerRequest(HttpWorkerRequest request, byte[] buffer)
        {
            _request = request;
            _buffer = buffer;
        }

        public override int ReadEntityBody(byte[] buffer, int size)
        {
            return 0;
        }

        public override int ReadEntityBody(byte[] buffer, int offset, int size)
        {
            return 0;
        }

        public override byte[] GetPreloadedEntityBody()
        {
            return _buffer;
        }

        public override int GetPreloadedEntityBody(byte[] buffer, int offset)
        {
            Buffer.BlockCopy(_buffer, 0, buffer, offset, _buffer.Length);
            return _buffer.Length;
        }

        public override int GetPreloadedEntityBodyLength()
        {
            return _buffer.Length;
        }

        public override int GetTotalEntityBodyLength()
        {
            return _buffer.Length;
        }

        public override string GetKnownRequestHeader(int index)
        {
            return index == HeaderContentLength
                ? "0"
                : _request.GetKnownRequestHeader(index);
        }       // All other methods elided, they're just passthrough  

        public override void EndOfRequest()
        {
            throw new NotImplementedException();
        }

        public override void FlushResponse(bool finalFlush)
        {
            throw new NotImplementedException();
        }

        public override string GetHttpVerbName()
        {
            throw new NotImplementedException();
        }

        public override string GetHttpVersion()
        {
            throw new NotImplementedException();
        }

        public override string GetLocalAddress()
        {
            throw new NotImplementedException();
        }

        public override int GetLocalPort()
        {
            throw new NotImplementedException();
        }

        public override string GetQueryString()
        {
            throw new NotImplementedException();
        }

        public override string GetRawUrl()
        {
            throw new NotImplementedException();
        }

        public override string GetRemoteAddress()
        {
            throw new NotImplementedException();
        }

        public override int GetRemotePort()
        {
            throw new NotImplementedException();
        }

        public override string GetUriPath()
        {
            throw new NotImplementedException();
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
            throw new NotImplementedException();
        }

        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            throw new NotImplementedException();
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            throw new NotImplementedException();
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            throw new NotImplementedException();
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            throw new NotImplementedException();
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
            throw new NotImplementedException();
        }
    }
}
