using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using EzNets.Library.Config.Entity;
using EzNets.Library.Config.Service;

namespace EzNets.Library.Helpers
{

    public class UploadProcessor
    {
        //在外部如有必要可自行重命名文件，重定位文件，将外部真实文件名与DB建立映射等等
        public delegate string UploadFileFoundCallBack(string oriFileFullName);

        public UploadFileFoundCallBack UploadFileFoundCallBackFunc
        {
            get;
            set;
        }

        private UploadSettingEntity m_settingEntity;

        /// TODO:以后把密码传进来
        private byte[] m_password=Encoding.ASCII.GetBytes("yanghang");

        private byte[] _buffer;
        private byte[] _boundaryBytes;
        private byte[] _endHeaderBytes;
        private byte[] _endHTTPBytes;
        private byte[] _lineBreakBytes;
        private const string _lineBreak = "\r\n";
        private readonly Regex _filename =
            new Regex(@"Content-Disposition:\s*form-data\s*;\s*name\s*=\s*""file""\s*;\s*filename\s*=\s*""(.*)""",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly HttpWorkerRequest _workerRequest;

        private object _lock = new object();
        private int _recived = 0;
        /// <summary>
        /// In case that Received > 0 ;  Received / Total => real time uploading progress 
        /// </summary>
        public int ReceivedSize
        {
            get
            {
                lock (_lock)
                {
                    return _recived;
                }
            }
            set
            {
                lock (_lock)
                {
                    _recived = value;
                }
            }
        }

        private int _total = 0;
        public int TotalSize
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
            }
        }


        public UploadProcessor(HttpWorkerRequest workerRequest)
        {
            _workerRequest = workerRequest;

            GeneralConfig config = new GeneralConfig("Upload.config");
            ConfigSetting setting = new ConfigSetting(config);
            List<UploadSettingEntity> entities = ConfigService.Instance.GetObject(setting, new UploadSettingEntity());
            m_settingEntity = entities.FirstOrDefault();
            m_settingEntity.RootPath = System.AppDomain.CurrentDomain.BaseDirectory + m_settingEntity.RootPath;

            UploadFileFoundCallBackFunc = new UploadFileFoundCallBack(x=>x+".EzNet");
        }

        public UploadProcessor(HttpWorkerRequest workerRequest,UploadSettingEntity settingEntity)
        {
            _workerRequest = workerRequest;
            m_settingEntity = settingEntity;
            UploadFileFoundCallBackFunc = new UploadFileFoundCallBack(x => x + ".EzNet");
        }

        public void StreamToDisk(IServiceProvider provider, Encoding encoding)
        {
            string rootPath = m_settingEntity.RootPath;
            var buffer = new byte[8192];
            if (!_workerRequest.HasEntityBody())
            {
                return;
            }
            TotalSize = _workerRequest.GetTotalEntityBodyLength();
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
            
            //need to make sure client sends the same encording content
            var text = encoding.GetString(body);
            //var text = System.Text.Encoding.Default.GetString(body);


            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var fileName = _filename.Matches(text)[0].Groups[1].Value;
            fileName = Path.GetFileName(fileName); // IE captures full user path; chop it 
            
            var path = Path.Combine(rootPath, fileName);


            EzNetEnCryptFileStream stream = null;
            var files = new List<String>();

            if (!string.IsNullOrEmpty(fileName))
            {
                files.Add(fileName);
                path = this.UploadFileFoundCallBackFunc.Invoke(path);
                stream = new EzNetEnCryptFileStream(path, m_password, m_settingEntity.UploadFileSecurity.FileContentEncrypt);
            }
            
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
                ReceivedSize = preloaded;
                while (TotalSize - ReceivedSize >= loaded && _workerRequest.IsClientConnected())
                {
                    loaded = _workerRequest.ReadEntityBody(buffer, buffer.Length);
                    stream = ProcessHeaders(buffer, stream, encoding, loaded, files, rootPath);
                    ReceivedSize += loaded;
                }
                var remaining = TotalSize - ReceivedSize;
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
            _endHTTPBytes = encoding.GetBytes(string.Concat(_lineBreak, boundary, "--", _lineBreak));
            _lineBreakBytes = encoding.GetBytes(string.Concat(_lineBreak + boundary + _lineBreak));
        }

        private EzNetEnCryptFileStream ProcessHeaders(byte[] buffer, EzNetEnCryptFileStream stream, Encoding encoding, int count, ICollection<string> files, string rootPath)
        {
            buffer = AppendBuffer(buffer, ref count);

            int resolvedCount = 0;

            //HTTP上传文件流结尾标识
            var endHttpIndex = -1;

            if (count >= _endHTTPBytes.Length)
            {
                endHttpIndex = IndexOf(buffer, _endHTTPBytes, count - _endHTTPBytes.Length);
            }

            while (resolvedCount < count)
            {
                //文件起始标识
                var fileStartIndex = -1;

                //两个文件分隔标识
                var splitIndex = IndexOf(buffer, _lineBreakBytes, resolvedCount);

                //判断是不是第一个文件开头
                fileStartIndex = IndexOf(buffer, _boundaryBytes, resolvedCount);
                if (fileStartIndex != 0)
                {
                    fileStartIndex = splitIndex;
                }

                //两个换行：文件头的结尾标识，在它之后是文件真正的内容
                var endHeaderIndex = IndexOf(buffer, _endHeaderBytes, fileStartIndex > resolvedCount ? fileStartIndex : resolvedCount);

                if (fileStartIndex != -1)
                {//发现了一个文件的开头

                    if (endHeaderIndex != -1)
                    {//发现了文件header

                        endHeaderIndex += _endHeaderBytes.Length;

                         //保存上一个文件结尾
                        int writeCount = fileStartIndex - resolvedCount;
                        if (writeCount > 0)
                        {
                            stream.Write(buffer, resolvedCount, writeCount);
                            resolvedCount += writeCount;
                        }
                        else if (fileStartIndex == 0)
                        {//当前是第一个文件
                            resolvedCount = endHeaderIndex;
                            continue;
                        }

                        var text = encoding.GetString(buffer, resolvedCount, count - resolvedCount);
                        var match = _filename.Match(text);
                        var fileName = match != null ? match.Groups[1].Value : null;
                        fileName = Path.GetFileName(fileName); // IE captures full user path; chop it    
                   
                        
                        if (!string.IsNullOrEmpty(fileName) && !files.Contains(fileName))
                        {
                            //上一个文件完成上传
                            if (stream != null)
                            {
                                stream.Flush();
                                stream.Close();
                                stream.Dispose();
                            }

                            files.Add(fileName);
                            var filePath = Path.Combine(rootPath, fileName);

                            //建立新文件
                            filePath = this.UploadFileFoundCallBackFunc.Invoke(filePath);
                            stream = new EzNetEnCryptFileStream(filePath, this.m_password,this.m_settingEntity.UploadFileSecurity.FileContentEncrypt);

                        }
                        
                        resolvedCount = endHeaderIndex;

                        continue;

                    }
                    else
                    {//找不到文件header，数据转到下一块一起处理

                        //保存上一个文件结尾
                        int writeCount = fileStartIndex - resolvedCount;
                        if (writeCount > 0)
                        {
                            stream.Write(buffer, resolvedCount, writeCount);
                            resolvedCount += writeCount;
                        }

                        int remainCount = count - resolvedCount;
                        _buffer = new byte[remainCount];
                        Buffer.BlockCopy(buffer, resolvedCount, _buffer, 0, _buffer.Length);
                        return stream;
                    }

                }
                else
                {
                    if(endHttpIndex < 0)
                    {//没发现文件尾也没发现文件头
                        SaveFileContent(stream, buffer, resolvedCount, count - resolvedCount);
                        return stream;
                    }
                    else
                    {//已经到了HTTP数据流的末尾

                        if (resolvedCount < endHttpIndex)
                        {
                            int writeCount = endHttpIndex - resolvedCount;
                            stream.Write(buffer, resolvedCount, writeCount);
                        }

                        return stream;
                    }
                }
            }
            return stream;
        }

        private static EzNetEnCryptFileStream ProcessNextFile(EzNetEnCryptFileStream stream, UploadFileFoundCallBack uploadFileFoundCallBackFunc, byte[] buffer, int count, int startIndex, int endIndex, string filePath)
        {
            var fullCount = count;
            var endOfFile = SkipInput(buffer, startIndex, count, ref count);
            stream.Write(endOfFile, 0, count);
            stream.Flush();
            stream.Close();
            stream.Dispose();

            filePath = uploadFileFoundCallBackFunc.Invoke(filePath);
            stream = new EzNetEnCryptFileStream(filePath, stream.Password, stream.IsCrypt);
            
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

        private byte[] AppendBuffer(byte[] buffer, ref int count)
        {
            var input = new byte[_buffer == null ? buffer.Length : _buffer.Length + count];
            if (_buffer != null)
            {
                Buffer.BlockCopy(_buffer, 0, input, 0, _buffer.Length);
            }

            Buffer.BlockCopy(buffer, 0, input, _buffer == null ? 0 : _buffer.Length, count);
            count += _buffer == null ? 0 : _buffer.Length;

            _buffer = null;
            return input;
        }

        private void SaveFileContent(EzNetEnCryptFileStream stream, byte[] buffer,int startIndex,int count)
        {
            //防止文件尾标识处于当前数据块和下一块之间
            //选用_endHTTPBytes.Length是因为它是各标识中最长的
            if (count > _endHTTPBytes.Length)
            {
                stream.Write(buffer, startIndex, count - _endHTTPBytes.Length);
                _buffer = new byte[_endHTTPBytes.Length];
                Buffer.BlockCopy(buffer, count - _endHTTPBytes.Length, _buffer, 0, _buffer.Length);
            }
            else
            {
                _buffer = buffer;
            }

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
            _request.EndOfRequest();
        }

        public override void FlushResponse(bool finalFlush)
        {
            _request.FlushResponse(finalFlush);
        }

        public override string GetHttpVerbName()
        {
            return _request.GetHttpVerbName();
        }

        public override string GetHttpVersion()
        {
            return _request.GetHttpVersion();
        }

        public override string GetLocalAddress()
        {
            return _request.GetLocalAddress();
        }

        public override int GetLocalPort()
        {
            return _request.GetLocalPort();
        }

        public override string GetQueryString()
        {
            return _request.GetQueryString();
        }

        public override string GetRawUrl()
        {
            return _request.GetRawUrl();
        }

        public override string GetRemoteAddress()
        {
            return _request.GetRemoteAddress();
        }

        public override int GetRemotePort()
        {
            return _request.GetRemotePort();
        }

        public override string GetUriPath()
        {
            return _request.GetUriPath();
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
            _request.SendKnownResponseHeader(index, value);
        }

        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            _request.SendResponseFromFile(handle, offset, length);
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            _request.SendResponseFromFile(filename, offset, length);
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            _request.SendResponseFromMemory(data, length);
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            _request.SendStatus(statusCode, statusDescription);
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
            _request.SendUnknownResponseHeader(name, value);
        }
    }

    public class UploadSettingEntity
    {
        public string RootPath { get; set; }
        public class UploadFileSecurityEntity
        {
            public string Owner { get; set; }
            public bool FileNameEncrypt { get; set; }
            public bool FileContentEncrypt { get; set; }
        }
        public UploadFileSecurityEntity UploadFileSecurity { get; set; }
    }
}
