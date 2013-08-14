using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;

namespace MvcTest.App_Code
{
    public enum ConfigEnum : int
    {
        Database = 0,
        StoredProcedure = 1
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
        internal virtual string GetValue()
        {
            string val = string.Empty;
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
        internal ConfigStrategyDB(string filepath, string nodePath, string nodeName, string nodeAttributes, string nodeID)
        {
            this.FilePath = filepath;
            this.NodePath = nodePath;
            this.NodeName = nodeName;
            this.NodeAttributes = nodeAttributes;
            this.NodeID = nodeID;
        }
    }

    internal class ConfigStrategySP : ConfigStrategy
    {
        internal ConfigStrategySP(string filepath, string nodePath, string nodeName, string nodeAttributes, string nodeID)
        {
            this.FilePath = filepath;
            this.NodePath = nodePath;
            this.NodeName = nodeName;
            this.NodeAttributes = nodeAttributes;
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
                    _config = new ConfigStrategyDB(filepath, "/root/dbs", "db", "value", nodeID);
                    break;
                case ConfigEnum.StoredProcedure:
                    filepath = System.AppDomain.CurrentDomain.BaseDirectory + "Configuration\\sp.config";
                    _config = new ConfigStrategySP(filepath, "/root", "sp", "value", nodeID);
                    break;
            }
        }

        public string GetValue()
        {
            return _config.GetValue();
        }
    }
}