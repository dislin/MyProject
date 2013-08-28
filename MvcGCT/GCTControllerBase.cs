using System;
using System.Diagnostics;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using NumbersGame.Common.Exception;
using NumbersGame.Common.Utilities;
using NumbersGame.Mark6.OPA.DomainIPService.Service;
using NumbersGame.Mark6.Web.BO.WebSite.App_GlobalResources;
using NumbersGame.Mark6.Web.Common;
using NumbersGame.Mark6.Web.Common.Enums;
using NumbersGame.Mark6.Web.Common.Exceptions;
using NumbersGame.Mark6.Web.Common.Models.Configuration;
using NumbersGame.Mark6.Web.Common.Mvc.ActionResults;

namespace NumbersGame.Mark6.Web.BO.WebSite.AppCode.Mvc
{
    public class GCTControllerBase : Controller
    {
        private static readonly ResourceManager MessageResourceManager = new ResourceManager(typeof(Message));
        protected SessionData SessionData = new SessionData();
        protected LanguageInfo LanguageInfo { get; private set; }

        protected GCTControllerBase(LanguageInfo languageInfo)
        {
            if (languageInfo == null)
            {
                throw new ArgumentNullException("Language Info");
            }
            this.LanguageInfo = languageInfo;
        }

        #region Override Method
        protected override void OnException(ExceptionContext filterContext)
        {
            HttpException httpEx = filterContext.Exception as HttpException;

            if (httpEx != null)
            {
                if (httpEx.GetHttpCode() == 401)
                {
                    #region Handle Unauthorized Exception, do not log exception since it is not at all an exception
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        this.AjaxError(401, httpEx.Message);
                    }
                    else
                    {
                        switch ((UnauthorizedCode)httpEx.ErrorCode)
                        {
                            case UnauthorizedCode.SessionTimeout:
                            case UnauthorizedCode.KickOut:
                                httpEx.HelpLink = string.Format(Message.Error_ClickToLogin_Format, this.Url.Action("LogOff", "Authentication"));
                                filterContext.Result = this.View("~/Views/Error/_401.cshtml", httpEx);
                                break;

                            case UnauthorizedCode.NoPermission:
                                filterContext.Result = this.View("~/Views/Error/_401.cshtml", httpEx);
                                break;

                            default:
                                filterContext.Result = new HttpUnauthorizedResult();
                                break;
                        }
                    }
                    #endregion
                }
                else
                {
                    LogHelper.Exception(httpEx);
                    throw httpEx;
                }
            }
            else
            {
                BaseException baseEx = filterContext.Exception as BaseException;

                if (baseEx == null)
                {
                    LogHelper.Exception(filterContext.Exception);
                    this.HandleException(filterContext, null);
                }
                else
                {
                    if (baseEx.GetType().Name == "DomainException")
                    {
                        LogHelper.Server.Warn(baseEx.Message);
                    }
                    else
                    {
                        LogHelper.Server.Error(baseEx);
                    }

                    string errorMessage = Message.Error_ServerInternal; // default
                    string localizedErrorMessage = GetLocalizedErrorMessage(baseEx.ModuleCode, baseEx.ErrorCode, baseEx.Message);

                    if (string.IsNullOrEmpty(localizedErrorMessage)) // cannot find from resource file
                    {
                        if (baseEx.GetType().Name == "DomainException")
                        {
                            if (!string.IsNullOrEmpty(baseEx.Message))
                            {
                                errorMessage = baseEx.Message; // take default english message from domain
                            }
                        }
                    }
                    else
                    {
                        errorMessage = localizedErrorMessage; // localized message found from resource file
                    }

                    HandleException(filterContext, errorMessage);

                    //if (AppConfiguration.Instance.SystemSetting.ShowFriendlyErrorMessage)
                    //{
                    //    HandleException(filterContext, Message.Error_ServerInternal);
                    //}
                    //else
                    //{
                    //    HandleException(filterContext, GetLocalizedErrorMessage(baseEx.ModuleCode, baseEx.ErrorCode) ?? baseEx.Message);
                    //}
                }
            }

            filterContext.ExceptionHandled = true; // Comment this line to see yellow screen with exception message
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string ip = CommonHelper.GetClientIP();

            DateTime checkUserDomainIPDateTime = SessionData.CheckUserDomainIPDateTime;

            //Set Default BlackList Country to Session
            if (SessionData.BlackListCountryInfo == null)
            {
                string country = WhiteListIPService.Instance.GetCountryCodeByIP(ip);

                BlackListCountry blackListCountryInfo = DomainIPManager.GetBlackListedCountryInfo(country);

                BlackListCountryInfo blackListinfo = new BlackListCountryInfo();

                if (blackListCountryInfo != null)
                {
                    blackListinfo.CountryCode = blackListCountryInfo.CountryCode;
                    blackListinfo.CountryName = blackListCountryInfo.CountryName;
                    blackListinfo.RedirectURL = blackListCountryInfo.RedirectURL;
                    blackListinfo.CheckUserDomainURLMinuteTimer = blackListCountryInfo.CheckUserDomainURLMinuteTimer;
                }
                
                SessionData.BlackListCountryInfo = blackListinfo;
            }

            //Checking Whitelist
            if (NumbersGame.Mark6.Web.Common.AppConfiguration.Instance.SystemSetting.IsBackOffice)
            {
                if (DateTime.Now > checkUserDomainIPDateTime.AddMinutes(SessionData.BlackListCountryInfo.CheckUserDomainURLMinuteTimer))
                {
                    SessionData.CheckUserDomainIPDateTime = DateTime.Now;

                    if (WhiteListIPService.Instance.GetDomainIPManagementFlag())
                    {
                        if (!DomainIPManager.IsWhiteListedIP(ip))
                        {
                            SessionData.UserDomainIPBlocked = true;
                        }
                        else
                        {
                            SessionData.UserDomainIPBlocked = false;
                        }
                    }
                    else
                    {
                        SessionData.UserDomainIPBlocked = false;
                    }
                }
            }
            else
            {
                if (DateTime.Now > checkUserDomainIPDateTime.AddMinutes(SessionData.BlackListCountryInfo.CheckUserDomainURLMinuteTimer) && SessionData.BlackListCountryInfo.CountryCode != null)
                {
                    SessionData.CheckUserDomainIPDateTime = DateTime.Now;

                    if (!DomainIPManager.IsWhiteListedIP(ip))
                    {
                        SessionData.UserDomainIPBlocked = true;
                    }
                    else
                    {
                        SessionData.UserDomainIPBlocked = false;
                    }
                }
            }

            if (SessionData.UserDomainIPBlocked)
            {
                string controller = filterContext.RouteData.Values["controller"] as string;
                if (string.IsNullOrEmpty(controller) || !controller.Equals("Error", StringComparison.OrdinalIgnoreCase))
                {
                    filterContext.Result = RedirectToAction("Forbidden", "Error");
                    return;
                }
            }
            else
            {
                if (!this.ModelState.IsValid)
                {
                    #region build invalid parameter exception

                    StringBuilder errors = new StringBuilder();
                    foreach (var item in this.ModelState)
                    {
                        if (item.Value.Errors.Count > 0)
                        {
                            errors.AppendLine(item.Key);
                            foreach (var error in item.Value.Errors)
                            {
                                errors.AppendLine(error.ErrorMessage);
                            }
                        }
                    }

                    throw new InvalidParameterException(errors.ToString());

                    #endregion
                }

                UserInfo user = SessionData.UserInfo;

                if (user != null)
                {
                    Thread.SetData(Thread.GetNamedDataSlot("LoginName"), user.LoginName);
                }

                filterContext.HttpContext.Response.Cache.SetOmitVaryStar(true);
                filterContext.Controller.ViewBag.culture = this.LanguageInfo.CultureName; // all View pages can access this ViewBag
                filterContext.Controller.ViewBag.SessionData = this.SessionData; // all View pages can access this ViewBag

                // capture page rendered time - START
                var controller = filterContext.Controller;

                if (controller != null)
                {
                    var timer = new Stopwatch();
                    controller.ViewData["_ActionTimer"] = timer;
                    timer.Start();
                }
                // capture page rendered time - END
            }

            base.OnActionExecuting(filterContext);

        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // capture page rendered time - START
            var controller = filterContext.Controller;

            if (controller != null)
            {
                var timer = (Stopwatch)controller.ViewData["_ActionTimer"];

                if (timer != null)
                {
                    timer.Stop();
                    //controller.ViewData["_ElapsedTime"] = timer.ElapsedMilliseconds;
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        if (filterContext.ActionDescriptor.ActionName != "GetServerDateTime") // skip this because too many requests every 5s
                        {
                            LogHelper.RequestPerformance(timer.Elapsed, "[Service]" + controller.ControllerContext.HttpContext.Request.RawUrl);
                        }
                    }
                    else
                    {
                        LogHelper.RequestPerformance(timer.Elapsed, "[Page]" + controller.ControllerContext.HttpContext.Request.RawUrl);
                    }
                }
            }
            // capture page rendered time - END
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonDataContractResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
        #endregion

        #region Private Methods
        private void HandleException(ExceptionContext filterContext, string message)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (AppConfiguration.Instance.SystemSetting.ShowStackTraceInDialog)
                {
                    this.AjaxError(500, (message ?? Message.Error_ServerInternal) + "<br/><br/><u>StackTrace:</u><br/>" + filterContext.Exception.StackTrace);
                }
                else
                {
                    this.AjaxError(500, message ?? Message.Error_ServerInternal);
                }
            }
            else
            {
                filterContext.Result = this.View("~/Views/Error/_500.cshtml", message);
            }
        }

        private void AjaxError(int httpStatusCode, string message)
        {
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = httpStatusCode;
            Response.ContentType = "text/plain";
            Response.Write(message);
            Response.End();
        }

        private string GetLocalizedErrorMessage(string moduleCode, string errorCode, string domainMessage)
        {
            string message = MessageResourceManager.GetString("EnumError_" + moduleCode + "_" + errorCode); // try to find from EnumError_xxxx_xxxx first
            if (string.IsNullOrEmpty(message))
            {
                message = MessageResourceManager.GetString(errorCode);
            }

            message = ExtendLocalizedErrorMessage(moduleCode, errorCode, message, domainMessage);
            return message;
        }

        private string ExtendLocalizedErrorMessage(string moduleCode, string errorCode, string message, string domainMessage)
        {
            int first = domainMessage.IndexOf("<Date>");
            int last = domainMessage.LastIndexOf("<Date>");
            if (first > 0 && last > 0)
            {
                string dateValue = domainMessage.Substring(first + 6, last - (first + 6));
                message = message.Replace("<Date>", Convert.ToDateTime(dateValue).ToString("yyyy-MM-dd"));
            }
            return message;

        }
        #endregion
    }
}
