using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EzNets.Library.Config.Entity
{
    public class LogSettingConfig : IConfig
    {
        public LogSettingConfig()
        {
            this.FilePath = System.AppDomain.CurrentDomain.BaseDirectory + @"Configuration\";
            this.FileName = "LogSetting.config";
            this.NodePath = "/roots";
        }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string NodePath { get; set; }
    }
}
