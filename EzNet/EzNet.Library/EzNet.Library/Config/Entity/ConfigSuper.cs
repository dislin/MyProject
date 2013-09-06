using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EzNet.Library.Config.Entity
{
    internal class ConfigSuper
    {
        internal string FilePath { get; set; }
        internal string NodePath { get; set; }
        internal string NodeName { get; set; }
        internal string NodeAttributesKey { get; set; }
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
}
