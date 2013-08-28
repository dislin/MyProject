using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using NumbersGame.Mark6.Web.Common;

namespace NumbersGame.Mark6.Web.BO.WebSite.AppCode.Mvc
{
    public class ControllerFactory : IControllerFactory
    {
        private static readonly ConcurrentDictionary<string, Type> controllerTypeCache = new ConcurrentDictionary<string, Type>();
        private static readonly string controllerFullNameFormat1 = "NumbersGame.Mark6.Web.BO.WebSite.Controllers.{0}Controller";
        private static readonly string controllerFullNameFormat2 = "NumbersGame.Mark6.Web.BO.WebSite.Controllers.Services.{0}Controller";

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("Controller Name");
            }

            Type controllerType = controllerTypeCache.GetOrAdd(
                controllerName,
                name =>
                {
                    Type type =
                        Type.GetType(string.Format(controllerFullNameFormat1, name), false, true) ??
                        Type.GetType(string.Format(controllerFullNameFormat2, name), false, true);

                    if (type == null)
                    {
                        throw new Exception(string.Format(
                            "Can't find controller [{0}] in the followings:\r\n {1}\r\n {2}",
                            controllerName,
                            string.Format(controllerFullNameFormat1, controllerName),
                            string.Format(controllerFullNameFormat2, controllerName)));
                    }

                    return type;
                });

            return CreateController(requestContext, controllerType);
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        public void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

        }

        private static IController CreateController(RequestContext context, Type controllerType)
        {
            try
            {
                string culture = context.RouteData.Values["culture"] as string;

                if (!string.IsNullOrEmpty(culture))
                {
                    var language = AppConfiguration.Instance.LanguageInfos.FirstOrDefault(l => l.CultureName.Equals(culture, StringComparison.InvariantCultureIgnoreCase));
                    Thread currentThread = Thread.CurrentThread;
                    currentThread.CurrentCulture = language.Culture;
                    currentThread.CurrentUICulture = language.Culture;

                    return (IController)Activator.CreateInstance(controllerType, language);
                }

                return (IController)Activator.CreateInstance(controllerType);
            }
            catch (ArgumentNullException)
            {
                throw new HttpException(404, "File not found");
            }
        }

    }
}
