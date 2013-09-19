using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using log4net;
using System.Runtime.CompilerServices;
using EzNets.Library.Common;
using EzNets.Library.Config.Entity;
using EzNets.Library.Config.Service;

namespace EzNets.Library.Log
{
    public class LogService : Singleton<LogService>
    {
        private log4net.ILog m_FatalLogger = LogManager.GetLogger("FatalLogger");
        private log4net.ILog m_DebugLogger=LogManager.GetLogger("DebugLogger");
        private log4net.ILog m_InfoLogger = LogManager.GetLogger("InfoLogger");

        //Singleton只会呼叫一次
        private LogService()
        {
            GeneralConfig config = new GeneralConfig("LogSetting.config");
            ConfigSetting setting = new ConfigSetting(config);
            List<LogSettingEntity> entities = ConfigService.Instance.GetObject(setting, new LogSettingEntity());
            LogSettingEntity settingEntity = entities.FirstOrDefault();
            InitConfig(System.AppDomain.CurrentDomain.BaseDirectory+settingEntity.Log4NetConfigFileName);
        }

        public static void InitConfig(string configFileName)
        {   
            System.IO.FileInfo configFileInfo = new System.IO.FileInfo(configFileName);
             log4net.Config.XmlConfigurator.Configure(configFileInfo);
        }

        public void Exception(Exception ex)
        {
            System.Web.HttpException httpEx = ex as HttpException;
            StringBuilder sb = new StringBuilder();
            if (httpEx != null)
            {
                sb.AppendFormat("Http Error Code:{0}; ",httpEx.ErrorCode);
                sb.AppendFormat("Http Web Event Code:{0}; ",httpEx.WebEventCode);
            }

            StackTrace st = new StackTrace(ex, true);
            StackFrame frame0 = st.GetFrame(0);

            string[] fileNamePaths = frame0.GetFileName().Split('\\');

            sb.AppendFormat("Message:{0}; In {1}.{2}; At {3}:{4} ",
                ex.Message,
                ex.Source,
                ex.TargetSite.Name,
                fileNamePaths[fileNamePaths.Length - 1],
                frame0.GetFileLineNumber());

            m_FatalLogger.Fatal(sb.ToString());
        }

        public void Fatal(object message)
        {
            m_FatalLogger.Fatal(message);
        }

        public void Debug(object message)
        {
            m_DebugLogger.Debug(message);
        }

        public void Info(object message)
        {
            m_InfoLogger.Debug(message);
        }
    }

    public class LogSettingEntity
    {
        public string Log4NetConfigFileName { get; set; }
    }
}
