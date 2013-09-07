using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EzNet.Library.Config.Entity
{
    public class GeneralConfig: IConfig
    {
        public GeneralConfig(string fileName)
        {
            this.FilePath = System.AppDomain.CurrentDomain.BaseDirectory + @"Configuration\";
            this.FileName = fileName;
            this.NodePath = "/roots";
        }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string NodePath { get; set; }
    }
}
