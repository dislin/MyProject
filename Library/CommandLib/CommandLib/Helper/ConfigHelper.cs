using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;

namespace CommandLib.Helper
{
    public enum ConfigEnum : int
    {
        Database = 0,
        StoredProcedure = 1,
        LogSetting = 2,
    }
    //ex: C:\\...\...\db.config
    //<root>
    //    <dbs>
    //        <db id="MainDB" value="DBPath" />
    //    </dbs>
    //</root>
    // FilePath: "C:\\...\...\db.config"
    // NodePath:"/root/dbs"
    // NodeName:"db"
    // NodeID: "MainDB"
    // NodeAttributes: "value"

    internal abstract class ConfigStrategy
    {
        internal string FilePath { get; set; }
        internal string NodePath { get; set; }
        internal string NodeName { get; set; }
        internal string NodeAttributes { get; set; }
        internal string NodeID { get; set; }
        internal virtual string GetValue(string nodeAttributes)
        {
            string val = string.Empty;
            this.NodeAttributes = nodeAttributes;
            XmlTextReader reader = new XmlTextReader(this.FilePath);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            reader.Close();
            XmlNodeList nodes = doc.SelectSingleNode(this.NodePath).ChildNodes;
            foreach (XmlNode node in nodes)
            {
                if (node.Name.ToLower() == this.NodeName.ToLower() && node.Attributes["id"].Value.ToString() == this.NodeID)
                {
                    val = node.Attributes[this.NodeAttributes].Value.ToString();
                }
            }
            return val;
        }
    }

    internal class ConfigStrategyDB : ConfigStrategy
    {
        internal ConfigStrategyDB(string filepath, string nodePath, string nodeName, string nodeID)
        {
            this.FilePath = filepath;
            this.NodePath = nodePath;
            this.NodeName = nodeName;
            this.NodeID = nodeID;
        }
    }

    internal class ConfigStrategySP : ConfigStrategy
    {
        internal ConfigStrategySP(string filepath, string nodePath, string nodeName, string nodeID)
        {
            this.FilePath = filepath;
            this.NodePath = nodePath;
            this.NodeName = nodeName;
            this.NodeID = nodeID;
        }
    }

    internal class ConfigStrategyLog : ConfigStrategy
    {
        internal ConfigStrategyLog(string filepath, string nodePath, string nodeName, string nodeID)
        {
            this.FilePath = filepath;
            this.NodePath = nodePath;
            this.NodeName = nodeName;
            this.NodeID = nodeID;
        }
    }

    public class ConfigHelper
    {
        private ConfigStrategy _config = null;
        public ConfigHelper(ConfigEnum configEnum, string nodeID)
        {
            string filepath = string.Empty;
            switch (configEnum)
            {
                case ConfigEnum.Database:
                    filepath = System.AppDomain.CurrentDomain.BaseDirectory + "Configuration\\db.config";
                    _config = new ConfigStrategyDB(filepath, "/root/dbs", "db", nodeID);
                    break;
                case ConfigEnum.StoredProcedure:
                    filepath = System.AppDomain.CurrentDomain.BaseDirectory + "Configuration\\sp.config";
                    _config = new ConfigStrategySP(filepath, "/root", "sp", nodeID);
                    break;
                case ConfigEnum.LogSetting:
                    filepath = System.AppDomain.CurrentDomain.BaseDirectory + "Configuration\\LogSetting.config";
                    _config = new ConfigStrategySP(filepath, "/root", "log", nodeID);
                    break;
            }
        }

        public string GetValue(string nodeAttributes = "value")
        {
            return _config.GetValue(nodeAttributes);
        }
    }
}