using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EzNet.Library.Config.Enum;

namespace EzNet.Library.Config.Entity
{
    public class ConfigSetting: IConfig
    {
        public ConfigSetting(){}
        public ConfigSetting(IConfig config)
        {
            this.FilePath = config.FilePath;
            this.FileName = config.FileName;
            this.NodePath = config.NodePath;
        }
        public ConfigSetting(ConfigEnum.ConfigType configType)
        {
            IConfig oConfig;
            switch (configType)
            {
                case ConfigEnum.ConfigType.Database:
                    oConfig = new DBConfig();
                    break;
                case ConfigEnum.ConfigType.StoredProcedure:
                    oConfig = new SPConfig();
                    break;
                case ConfigEnum.ConfigType.LogSetting:
                    oConfig = new LogSettingConfig();
                    break;
                default:
                    oConfig = null;
                    break;
            }
            setting(oConfig);
        }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string NodePath { get; set; }
        
        private void setting(IConfig config) 
        {
            if (config != null)
            {
                this.FilePath = config.FilePath;
                this.FileName = config.FileName;
                this.NodePath = config.NodePath;
            }
        }
    }
}
