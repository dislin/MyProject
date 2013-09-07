using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EzNet.Library.Config.Entity
{
    public class DBConfig: IConfig
    {
        public DBConfig()
        {
            this.FilePath = System.AppDomain.CurrentDomain.BaseDirectory + @"Configuration\";
            this.FileName = "db.config";
            this.NodePath = "/roots";
        }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string NodePath { get; set; }
    }
}
