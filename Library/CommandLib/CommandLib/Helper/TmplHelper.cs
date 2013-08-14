using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace CommandLib.Helper
{
    public class TmplHelper
    {
        /// <summary>
        /// use "_?_" instead ">"
        /// </summary>
        public static string GetTmpl(string tmplFileName,string node)
        {
            string tmplPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\CDN\\tmpl\\" + tmplFileName;
            if (File.Exists(tmplPath))
            {
                XmlTextReader reader = new XmlTextReader(tmplPath);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                reader.Close();
                string targetNode = "/tmpl/" + node;
                XmlNodeList nodelist = doc.SelectSingleNode(targetNode).ChildNodes;
                string tmplList = string.Empty;
                for(int i = 0; i < nodelist.Count; i++){
                    tmplList += nodelist.Item(i).OuterXml;
                }
                return tmplList.Replace("_?_",">").ToString();
            }
            return null;
        }
    }
}
