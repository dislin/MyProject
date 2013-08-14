using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.App_Code;

namespace MvcTest.Controllers
{
    public class TestDBController : Controller
    {
        //
        // GET: /TestDB/

        public ActionResult TestDB1()
        {
            ConfigHelper config = new ConfigHelper(ConfigEnum.Database, "MainDB");
            ViewBag.configValue = config.GetValue();
            return View(ViewBag);
        }

    }
}
