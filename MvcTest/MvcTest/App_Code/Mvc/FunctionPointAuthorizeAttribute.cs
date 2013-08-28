using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace NumbersGame.Mark6.Web.BO.WebSite.AppCode.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class FunctionPointAuthorizeAttribute : BasicAuthorizeAttribute
    {
        public string[] Function { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                if (this.Function == null || this.Function.Length == 0)
                {
                    return true;
                }

                //return BackOfficeManager.ValidateUserFunctionPermission(this.user.Functions, this.Function);

                if (this.user == null)
                {
                    return false;
                }

                return ValidateUserFunctionPermission(this.user.Functions, this.Function);
            }

            return false;
        }

        private bool ValidateUserFunctionPermission(IList<string> userAccessableFunction, IList<string> requiredFunctionList)
        {
            foreach (string item in requiredFunctionList)
            {
                if (!userAccessableFunction.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }
    }
}