using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommandLib.Helper;
using YangLogger;
using System.Xml;
using MvcTest.Models;
using System.Reflection;
using MvcTest.App_Code.Enum;
using System.Xml.Serialization;
using System.IO;
using System.Text;

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

        public ActionResult ObjectConfig()
        {
            List<TestRole> roles = new List<TestRole>();
            PropertyInfo[] pis = (new TestRole()).GetType().GetProperties();

            var oPIs = from pi in pis
                       select new {
                           name = pi.Name,
                           type = pi.PropertyType
                       };

            string txtFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "Configuration\\TestRole.config";
            XmlDocument doc = new XmlDocument();

            using (XmlTextReader reader = new XmlTextReader(txtFilePath))
            {
                doc.Load(reader);
            }

            XmlNodeList nodes = doc.SelectSingleNode("/roots").ChildNodes;
            List<string> oNodes = new List<string>();

            foreach (XmlNode node in nodes)
            {
                TestRole role = new TestRole();
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (oPIs.Select(x => x.name.ToString()).ToList().Contains(n.Name))
                    {
                        PropertyInfo pinfo = role.GetType().GetProperty(n.Name);
                        if (!pinfo.PropertyType.IsEnum)
                        {
                            pinfo.SetValue(role, Convert.ChangeType(n.InnerText, pinfo.PropertyType), null);
                        }
                        else
                        {
                            pinfo.SetValue(role, Convert.ToInt16(n.InnerText), null);
                        }
                    }
                }
                roles.Add(role);
                ViewBag.content += role.name + ", ";
            }


            return View(ViewBag);
        }


    }
}
