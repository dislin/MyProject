using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EzNets.Library.Utilities;
using EzNets.Library.Config.Entity;
using EzNets.Library.Config.Service;
using EzNets.Library.Log;

namespace EzNets.Library.Utilities
{
    public class EzNetBaseExceptionController : Controller
    {
        protected LogService logService = LogService.Instance;

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
                    logService.Exception(httpEx);
                }
                else
                {
                    // Handle Unauthorized Exception, do not log exception since it is not at all an exception
                }
            }
            else
            {
                System.Exception ex = filterContext.Exception;
                logService.Exception(ex); 

            }

            filterContext.ExceptionHandled = true; // Comment this line to see yellow screen with exception message

            GeneralConfig exceptionConfig = new GeneralConfig("Exception.config");
            ConfigSetting exceptionConfigSetting = new ConfigSetting(exceptionConfig);
            List<ExceptionSettingEntity> entities = ConfigService.Instance.GetObject(exceptionConfigSetting, new ExceptionSettingEntity());
            ExceptionSettingEntity entity = entities.FirstOrDefault();
            if (entity != null)
            {
                filterContext.Result = this.View(entity.ErrorView);
            }
            else
            {
                Response.Write("Sorry, an error occurred while processing your request.");
            }
            
        }
    }

    public class ExceptionSettingEntity
    {
        public string ErrorView { get; set; }
    }
}
