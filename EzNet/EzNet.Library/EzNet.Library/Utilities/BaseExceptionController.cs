using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EzNet.Library.Utilities;

namespace EzNet.Library.Utilities
{
    public class BaseExceptionController : Controller
    {

        /// <summary>
        /// Handle all exceptions with log output, etc.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            HttpException httpEx = filterContext.Exception as HttpException;

            if (httpEx != null)
            {
                if (httpEx.GetHttpCode() != 401)
                {
                    SimpleLogger.Exception(httpEx);
                }
                else
                {
                    // Handle Unauthorized Exception, do not log exception since it is not at all an exception
                }
            }
            else
            {
                System.Exception ex = filterContext.Exception;
                SimpleLogger.Exception(ex); 

            }

            filterContext.ExceptionHandled = true; // Comment this line to see yellow screen with exception message
        }
    }
}
