using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using log4net;

namespace EzNet.Library.Utilities
{
    public class SimpleLogger
    {
        private static log4net.ILog m_FatalLogger = LogManager.GetLogger("FatalLogger");
        private static log4net.ILog m_DebugLogger=LogManager.GetLogger("DebugLogger");
        private static log4net.ILog m_InfoLogger = LogManager.GetLogger("InfoLogger");

        public static void Init(FileInfo configFileInfo)
        {   
            log4net.Config.XmlConfigurator.Configure(configFileInfo);
        }

        public static void Exception(Exception ex)
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

        public static void Fatal(object message)
        {
            m_FatalLogger.Fatal(message);
        }

        public static void Debug(object message)
        {
            m_DebugLogger.Debug(message);
        }

        public static void Info(object message)
        {
            m_InfoLogger.Debug(message);
        }

        private SimpleLogger()
        {
        }
    }
}
