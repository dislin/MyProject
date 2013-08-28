using System;
using System.Web;
using System.Web.Mvc;
//using NumbersGame.Infrastructure.SsoManagement;
using NumbersGame.Mark6.Web.BO.WebSite.App_GlobalResources;
using NumbersGame.Mark6.Web.Common.Enums;

namespace NumbersGame.Mark6.Web.BO.WebSite.AppCode.Mvc
{
    /// <summary>
    /// BasicAuthorize includes FormAuthorize, SessionTimeOut and Kick-Out
    /// please use it instead of Authorize (check FormAuthorize only)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthorizeAttribute : AuthorizeAttribute
    {
        protected UserInfo user;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (!AuthorizeCore(filterContext.HttpContext))
            {
                // please make sure that never go here  
                throw new HttpException(403, Message.Error_NoPermission, (int)UnauthorizedCode.NoPermission);
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                SessionData session = new SessionData();
                this.user = session.UserInfo;

                // identify session timeout or kick out
                if (this.user == null)
                {
                    throw new HttpException(401, Message.Error_SessionTimeOut, (int)UnauthorizedCode.SessionTimeout);
                }

                return true;
            }
            else
            {
                throw new HttpException(401, Message.Error_NotAuthorized, (int)UnauthorizedCode.None);
            }
        }

    }
}