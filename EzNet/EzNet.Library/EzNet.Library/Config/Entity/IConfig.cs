using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EzNet.Library.Config.Entity
{
    public interface IConfig
    {
        string FilePath { get; set; }
        string FileName { get; set; }
        string NodePath { get; set; }
    }
}
