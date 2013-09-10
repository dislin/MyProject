using System;
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
using EzNet.Library.Helpers;
using EzNet.Library.Utilities;
using EzNet.Library.Config.Entity;
using EzNet.Library.Config.Service;

namespace MvcTest.Controllers
{
    public class UploadController : EzNetBaseExceptionController
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
            processor.UploadFileFoundCallBackFunc = new UploadProcessor.UploadFileFoundCallBack(x =>
            {
                string newFile = x + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".EzNet";
                if (System.IO.File.Exists(newFile))
                {

                }

                ///TODO:记载DB，LOG等等

                return newFile;
            }
            );

            processor.StreamToDisk(context, encoding);

            return View("UploadSuccess");
        }

    }
    
}
