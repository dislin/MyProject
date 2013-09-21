using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EzNets.Library.Log.Enum
{
    public class LogEnum
    {
        public enum LogType:int
        {
            None = 0,
            InfoLogger = 1,
            FatalLogger = 2,
            DebugLogger = 3
        }
    }
}
