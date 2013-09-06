using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EzNet.Library.Config.Enum
{
    public class ConfigEnum
    {
        public enum ConfigType : int
        {
            None = 0,
            Database = 1,
            StoredProcedure = 2,
            LogSetting = 3,
        }
    }
}
