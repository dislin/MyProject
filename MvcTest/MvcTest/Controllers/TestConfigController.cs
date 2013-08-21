using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommandLib.Helper;

namespace MvcTest.Controllers
{
    public class TestConfigController : Controller
    {
        //
        // GET: /TestConfig/

        public ActionResult LogConfigTest()
        {
            ConfigHelper configLogSetting = new ConfigHelper(ConfigEnum.LogSetting, "Log4NetSetting");
            ViewBag.limit = configLogSetting.GetValue("limit");
            ViewBag.path = configLogSetting.GetValue("path");
            return View(ViewBag);
        }

    }
}
