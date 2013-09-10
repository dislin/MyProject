using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EzNet.Library.Config.Entity;
using EzNet.Library.Config.Service;
using MvcTest.Models;

namespace MvcTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            GeneralConfig roleConfig = new GeneralConfig("SystemCatalog.config");
            ConfigSetting roleConfigSetting = new ConfigSetting(roleConfig);
            List<SystemCatalog> model = new ConfigService().GetObject(roleConfigSetting, new SystemCatalog());
            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
