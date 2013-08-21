using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace YangLogger
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
