using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommandLib.Helper;
using YangLogger;

namespace MvcTest.Controllers
{
    public class TestConfigController : Controller
    {
        //
        // GET: /TestConfig/

        public ActionResult LogConfigTest()
        {
            SimpleLogger.Fatal("log output by yanghang");
            SimpleLogger.Debug("log output by yanghang");
            SimpleLogger.Info("log output by yanghang");

            
            //ConfigHelper configLogSetting = new ConfigHelper(ConfigEnum.LogSetting, "Log4NetSetting");
            ViewBag.limit = "";// configLogSetting.GetValue("limit");
            ViewBag.path = "";// configLogSetting.GetValue("path");
            return View(ViewBag);
        }

    }
}
