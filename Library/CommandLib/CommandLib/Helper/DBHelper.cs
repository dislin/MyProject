using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using CommandLib.Entity;
using CommandLib.Object;
using CommandLib.DB;

namespace CommandLib.Helper
{
    public class DBHelper
    {
        #region String Function
        public static string GetStringValue(DataRow dr, string dataRowName)
        {
            return dr[dataRowName].ToString() ?? string.Empty;
        }

        public static string GetStringValue(DataRow dr, string dataRowName, string defaultValue)
        {
            return dr[dataRowName].ToString() ?? defaultValue;
        }

        public static string GetStringValue(IDataReader dr, string dataReaderName)
        {
            return dr[dataReaderName].ToString() ?? string.Empty;
        }

        public static string GetStringValue(IDataReader dr, string dataReaderName, string defaultValue)
        {
            return dr[dataReaderName].ToString() ?? defaultValue;
        } 
        #endregion

        #region Int Function
        public static int GetIntValue(DataRow dr, string dataRowName)
        {
            int intValue = 0;
            int.TryParse(dr[dataRowName].ToString(), out intValue);
            return intValue;
        }

        public static int GetIntValue(DataRow dr, string dataRowName, int defaultValue)
        {
            int intValue = defaultValue;
            int.TryParse(dr[dataRowName].ToString(), out intValue);
            return intValue;
        }

        public static int GetIntValue(IDataReader dr, string dataReaderName)
        {
            int intValue = 0;
            int.TryParse(dr[dataReaderName].ToString(), out intValue);
            return intValue;
        }

        public static int GetIntValue(IDataReader dr, string dataReaderName, int defaultValue)
        {
            int intValue = defaultValue;
            int.TryParse(dr[dataReaderName].ToString(), out intValue);
            return intValue;
        } 
        #endregion

        #region Decimal Function
        public static decimal GetDecimalValue(DataRow dr, string dataRowName)
        {
            decimal decimalValue = 0m;
            decimal.TryParse(dr[dataRowName].ToString(), out decimalValue);
            return decimalValue;
        }

        public static decimal GetDecimalValue(DataRow dr, string dataRowName, decimal defaultValue)
        {
            decimal decimalValue = defaultValue;
            decimal.TryParse(dr[dataRowName].ToString(), out decimalValue);
            return decimalValue;
        }

        public static decimal GetDecimalValue(IDataReader dr, string dataReaderName)
        {
            decimal decimalValue = 0m;
            decimal.TryParse(dr[dataReaderName].ToString(), out decimalValue);
            return decimalValue;
        }

        public static decimal GetDecimalValue(IDataReader dr, string dataReaderName, decimal defaultValue)
        {
            decimal decimalValue = defaultValue;
            decimal.TryParse(dr[dataReaderName].ToString(), out decimalValue);
            return decimalValue;
        } 
        
        #endregion

        #region DateTime function
        public static DateTime GetDateTimeValue(DataRow dr, string dataRowName)
        {
            DateTime dateTimeValue = new DateTime();
            DateTime.TryParse(dr[dataRowName].ToString(), out dateTimeValue);
            return dateTimeValue;
        }

        public static DateTime GetDateTimeValue(DataRow dr, string dataRowName, DateTime defaultValue)
        {
            DateTime dateTimeValue = defaultValue;
            DateTime.TryParse(dr[dataRowName].ToString(), out dateTimeValue);
            return dateTimeValue;
        }

        public static DateTime? GetNullDateTimeValue(DataRow dr, string dataRowName)
        {
            DateTime? dateTimeValue = new DateTime?();
            if (dr[dataRowName] != DBNull.Value)
            {
                dateTimeValue = DateTime.Parse(dr[dataRowName].ToString());
            }
            else
            {
                dateTimeValue = null;
            }
            return dateTimeValue;
        }

        public static DateTime GetDateTimeValue(IDataReader dr, string dataReaderName)
        {
            DateTime dateTimeValue = new DateTime();
            DateTime.TryParse(dr[dataReaderName].ToString(), out dateTimeValue);
            return dateTimeValue;
        }

        public static DateTime GetDateTimeValue(IDataReader dr, string dataReaderName, DateTime defaultValue)
        {
            DateTime dateTimeValue = defaultValue;
            DateTime.TryParse(dr[dataReaderName].ToString(), out dateTimeValue);
            return dateTimeValue;
        }

        public static DateTime? GetNullDateTimeValue(IDataReader dr, string dataReaderName)
        {
            DateTime? dateTimeValue = new DateTime?();
            if (dr[dataReaderName] != DBNull.Value)
            {
                dateTimeValue = DateTime.Parse(dr[dataReaderName].ToString());
            }
            else
            {
                dateTimeValue = null;
            }
            return dateTimeValue;
        } 
        #endregion

        #region DataMapping

        public static object DataReaderMapping(IDataReader dr, object obj)
        {
            PropertyInfo[] ps = obj.GetType().GetProperties();
            foreach (PropertyInfo p in ps)
            {
                p.SetValue(obj, ReturnValue(dr[p.Name]), null);
            }
            return obj;
        }

        public static object DataReaderFilterMapping(IDataReader dr, object obj,ref List<String> drNameList)
        {
            PropertyInfo[] ps = obj.GetType().GetProperties();

            foreach (PropertyInfo p in ps)
            {
                if (drNameList.Contains(p.Name.ToLower()))
                {
                    p.SetValue(obj, ReturnValue(dr[p.Name]), null);
                }
            }
            return obj;
        }

        public static void InputToDB(object obj, string SPName) 
        {
            DBSettingEntity dbSetting = new DBSettingEntity(DataProvider.SqlServer, DBSetting.GetDBConnection(DBSource.MainDBConstr), CommandType.StoredProcedure, DBSetting.GetSPValue(SPName), false);
            List<SPParameterEntity> sp = SPParameterObject.GetParameter(SPName);
            PropertyInfo[] ps = obj.GetType().GetProperties();
            foreach (PropertyInfo p in ps) 
            {
                string parametername = string.Empty;
                if (sp.Exists(delegate(SPParameterEntity s) { parametername = s.name; return s.name.ToLower() == "@" + p.Name.ToLower(); })) 
                {
                    dbSetting.AddParameters(parametername, p.GetValue(obj, null) == null ? DBNull.Value : p.GetValue(obj, null), ParameterDirection.Input);
                }
            }
            DBManager2.Instance.ExecuteNonQuery(dbSetting);
        }

        public static bool ReturnDBInputStatus(object obj, string SPName)
        {
            DBSettingEntity dbSetting = new DBSettingEntity(DataProvider.SqlServer, DBSetting.GetDBConnection(DBSource.MainDBConstr), CommandType.StoredProcedure, DBSetting.GetSPValue(SPName), false);
            List<SPParameterEntity> sp = SPParameterObject.GetParameter(SPName);
            PropertyInfo[] ps = obj.GetType().GetProperties();
            bool statusVal = false;
            foreach (PropertyInfo p in ps)
            {
                string parametername = string.Empty;
                if (sp.Exists(delegate(SPParameterEntity s) { parametername = s.name; return s.name.ToLower() == "@" + p.Name.ToLower(); }))
                {
                    dbSetting.AddParameters(parametername, p.GetValue(obj, null) == null ? DBNull.Value : p.GetValue(obj, null), ParameterDirection.Input);
                }
            }
            Func<IDataReader, bool> func = delegate(IDataReader dr)
            {
                while (dr.Read())
                {
                    if (dr["StatusValue"].ToString() == "1")
                    {
                        statusVal = true;
                    }
                    else {
                        statusVal = false;
                    }
                }
                return true;
            };
            DBManager2.Instance.ExecuteReader(dbSetting,func);
            return statusVal;
        }

        public static List<string> DataReaderNameList(IDataReader dr)
        {
                List<string> list = new List<string>(dr.FieldCount);
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    list.Add(dr.GetName(i).ToLower());
                }
                return list;
        }

        private static object ReturnValue(object data)
        {
            string typename = data.GetType().Name.ToLower();
            if (typename.Contains("string"))
            {
                return data.ToString();
            }
            if (typename.Contains("int"))
            {
                return int.Parse(data.ToString());
            }
            if (typename.Contains("datetime"))
            {
                DateTime dateTimeValue = new DateTime();
                DateTime.TryParse(data.ToString(), out dateTimeValue);
                return dateTimeValue;
            }
            if (typename.Contains("decimal"))
            {
                return decimal.Parse(data.ToString());
            }
            return null;
        } 

        #endregion
    }
}
