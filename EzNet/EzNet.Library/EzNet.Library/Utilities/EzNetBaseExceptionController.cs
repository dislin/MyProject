using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EzNet.Library.Utilities;
using EzNet.Library.Config.Entity;
using EzNet.Library.Config.Service;

namespace EzNet.Library.Utilities
{
    public class EzNetBaseExceptionController : Controller
    {
        protected SimpleLogger m_logger = SimpleLogger.GetInstance(); 

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
                    m_logger.Exception(httpEx);
                }
                else
                {
                    // Handle Unauthorized Exception, do not log exception since it is not at all an exception
                }
            }
            else
            {
                System.Exception ex = filterContext.Exception;
                m_logger.Exception(ex); 

            }

            filterContext.ExceptionHandled = true; // Comment this line to see yellow screen with exception message

            GeneralConfig exceptionConfig = new GeneralConfig("Exception.config");
            ConfigSetting exceptionConfigSetting = new ConfigSetting(exceptionConfig);
            List<ExceptionSettingEntity> entities = new ConfigService().GetObject(exceptionConfigSetting, new ExceptionSettingEntity());
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

    internal class ExceptionSettingEntity
    {
        public string ErrorView { get; set; }
    }
}
