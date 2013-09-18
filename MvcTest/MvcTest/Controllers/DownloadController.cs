using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcTest.Models;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using EzNets.Library.Helpers;
using EzNets.Library.Utilities;
using EzNets.Library.Config.Entity;
using EzNets.Library.Config.Service;

namespace MvcTest.Controllers
{
    public class DownloadController : EzNetBaseExceptionController
    {
        //
        // GET: /Download
        public ActionResult Index()
        {
            GeneralConfig config = new GeneralConfig("Upload.config");
            ConfigSetting setting = new ConfigSetting(config);
            List<UploadSettingEntity> entities = ConfigService.Instance.GetObject(setting, new UploadSettingEntity());
            UploadSettingEntity settingEntity = entities.FirstOrDefault();
            string path = AppDomain.CurrentDomain.BaseDirectory + settingEntity.RootPath;
            DirectoryInfo dir = new DirectoryInfo(path);
            IEnumerable<FileLink> fileLinks = from m in dir.EnumerateFiles()
                                       select new FileLink
                                       {
                                           Name = m.Name,
                                           Url = Request.Url+ "/GetFile?fileName=" + m.Name,
                                           Target = "_blank"
                                       };

            return View("Download", fileLinks);
        }

        //
        // GET: /Download/GetFile/{}
        public FileStreamResult GetFile(string fileName)
        {
            GeneralConfig config = new GeneralConfig("Upload.config");
            ConfigSetting setting = new ConfigSetting(config);
            List<UploadSettingEntity> entities = ConfigService.Instance.GetObject(setting, new UploadSettingEntity());
            UploadSettingEntity settingEntity = entities.FirstOrDefault();
            string path = AppDomain.CurrentDomain.BaseDirectory  + settingEntity.RootPath +"\\";
            return File( new EzNetDeCryptFileStream(path + fileName,Encoding.ASCII.GetBytes("yanghang")), "text/plain", fileName);
        }

    }
    
}
