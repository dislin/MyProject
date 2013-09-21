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
using EzNets.Library.Log.Entity;
using EzNets.Library.Log.Enum;

namespace EzNets.Library.Log
{
    public class LogService : Singleton<LogService>
    {
        private log4net.ILog m_Logger;
        //private log4net.ILog m_FatalLogger = LogManager.GetLogger(LogEnum.LogType.FatalLogger.ToString());
        //private log4net.ILog m_DebugLogger = LogManager.GetLogger(LogEnum.LogType.DebugLogger.ToString());
        //private log4net.ILog m_InfoLogger = LogManager.GetLogger(LogEnum.LogType.InfoLogger.ToString());

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

            string fileName = frame0.GetFileName();
            if (!string.IsNullOrEmpty(fileName))
            {
                string[] fileNamePaths = frame0.GetFileName().Split('\\');

                sb.AppendFormat("Message:{0}; In {1}.{2}; At {3}:{4} ",
                    ex.Message,
                    ex.Source,
                    ex.TargetSite.Name,
                    fileNamePaths[fileNamePaths.Length - 1],
                    frame0.GetFileLineNumber());
            }
            else
            {
                sb.AppendFormat("Message:{0}; In {1}.{2} ",
                ex.Message,
                ex.Source,
                ex.TargetSite.Name);
            }

            this.Fatal(sb.ToString());
        }

        public void Fatal(object message)
        {
            m_Logger = LogManager.GetLogger(LogEnum.LogType.FatalLogger.ToString());
            m_Logger.Fatal(message);
        }

        public void Debug(object message)
        {
            m_Logger = LogManager.GetLogger(LogEnum.LogType.DebugLogger.ToString());
            m_Logger.Debug(message);
        }

        public void Info(object message)
        {
            m_Logger = LogManager.GetLogger(LogEnum.LogType.InfoLogger.ToString());
            m_Logger.Debug(message);
        }
    }
}
