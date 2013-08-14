using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.SqlClient;
using System.Data;

namespace CommandLib.DB
{
    public class DBSetting
    {
        private static string _dbconstrValue = string.Empty;
        private static string _dbConnectionString = string.Empty;
        private static string _DBConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + "Configuration\\db.config";
        private static string _SPConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + "Configuration\\sp.config";

        public static string DBConfigPath
        {
            set 
            {
                DBConfigPath = _DBConfigPath;
            }
            get
            {
                return _DBConfigPath;
            }
        }

        public static string SPConfigPath
        {
            set
            {
                SPConfigPath = _SPConfigPath;
            }
            get
            {
                return _SPConfigPath;
            }
        }
        public static string GetDBConnection(DBSource dbconstrID)
        {
            if (File.Exists(DBConfigPath))
            {
                XmlTextReader reader = new XmlTextReader(DBConfigPath);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                reader.Close();
                XmlNodeList dbconstrs = doc.SelectSingleNode("/root/dbconstrs").ChildNodes;
                XmlNodeList dbs = doc.SelectSingleNode("/root/dbs").ChildNodes;
                if (GetXMLNodeValue(dbconstrs, "dbconstr", dbconstrID.ToString(), out _dbconstrValue))
                {
                    GetXMLNodeValue(dbs, "db", _dbconstrValue, out _dbConnectionString);
                }
                return _dbConnectionString;
            }
            return null;
        }
        public static string GetSPValue(string spID) 
        {
            if (File.Exists(SPConfigPath))
            {
                XmlTextReader reader = new XmlTextReader(SPConfigPath);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                reader.Close();
                XmlNodeList sps = doc.SelectSingleNode("/root").ChildNodes;
                string spValue = string.Empty;
                if (GetXMLNodeValue(sps, "sp", spID, out spValue))
                {
                    return spValue;
                }                
            }
            return null;
        }
        private static bool GetXMLNodeValue(XmlNodeList nodes, string nodeName, string value, out string returnValue)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Name.ToLower() == nodeName.ToLower() && node.Attributes["id"].Value.ToLower() == value.ToLower())
                {
                    returnValue = node.Attributes["value"].Value.ToString();
                    return true;
                }
            }
            returnValue = string.Empty;
            return false;
        }

        public static DataTable GetDataTable(DBSource dbconstrID, string spID)
        {
            string cnstr = GetDBConnection(dbconstrID) ?? string.Empty;
            string sp = GetSPValue(spID) ?? string.Empty;
            if (!string.IsNullOrEmpty(cnstr) && !string.IsNullOrEmpty(sp)) 
            {
                SqlConnection cn = new SqlConnection(cnstr);
                SqlCommand cmd = new SqlCommand(sp, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                return dt;
            }
            return null;
        }

        public static IDataReader GetDataReader(DBSource dbconstrID, string spID)
        {
            string cnstr = GetDBConnection(dbconstrID) ?? string.Empty;
            string sp = GetSPValue(spID) ?? string.Empty;
            if (!string.IsNullOrEmpty(cnstr) && !string.IsNullOrEmpty(sp))
            {
                SqlConnection cn = new SqlConnection(cnstr);
                SqlCommand cmd = new SqlCommand(sp, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader(); 
                return dr;
            }
            return null;
        }

    }
}
