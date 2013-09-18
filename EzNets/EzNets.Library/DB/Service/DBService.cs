using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using EzNets.Library.DB.Entity;
using EzNets.Library.Common;

namespace EzNets.Library.DB.Service
{
    public class DBService : Singleton <DBService>
    {
        private DBService() { }
        public void ExecuteReader(DBSetting dbSetting, Action<IDataReader> actDataReaderDelegate)
        {
            using (SqlConnection oSqlCn = new SqlConnection())
            {
                oSqlCn.ConnectionString = dbSetting.ConnectionString;
                using (SqlCommand oCmd = new SqlCommand(dbSetting.StoredProcedure, oSqlCn))
                {
                    oCmd.CommandType = dbSetting.DBCommandType;

                    if (dbSetting.SqlParameterList.Count > 0)
                    {
                        dbSetting.SqlParameterList.ForEach(x => oCmd.Parameters.Add(x));
                    }

                    oSqlCn.Open();
                    using (SqlDataReader oDr = oCmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        actDataReaderDelegate.Invoke(oDr);
                    }
                }
            }
        }

        public void ExecuteDataSet(DBSetting dbSetting, Action<DataSet> actDataSetDelegate)
        {
            using (SqlConnection oSqlCn = new SqlConnection())
            {
                oSqlCn.ConnectionString = dbSetting.ConnectionString;
                using (SqlCommand oCmd = new SqlCommand(dbSetting.StoredProcedure, oSqlCn))
                {
                    oCmd.CommandType = dbSetting.DBCommandType;

                    if (dbSetting.SqlParameterList.Count > 0)
                    {
                        dbSetting.SqlParameterList.ForEach(x => oCmd.Parameters.Add(x));
                    }
                    
                    oSqlCn.Open();
                    using (SqlDataAdapter oDa = new SqlDataAdapter())
                    {
                        oDa.SelectCommand = oCmd;
                        DataSet oDs = new DataSet();
                        oDa.Fill(oDs);
                        actDataSetDelegate.Invoke(oDs);
                    }
                }
            }
        }

        public DataTable ExecuteDataTable(DBSetting dbSetting)
        {
            DataTable oDt = new DataTable();
            using (SqlConnection oSqlCn = new SqlConnection())
            {
                oSqlCn.ConnectionString = dbSetting.ConnectionString;
                using (SqlCommand oCmd = new SqlCommand(dbSetting.StoredProcedure, oSqlCn))
                {
                    oCmd.CommandType = dbSetting.DBCommandType;

                    if (dbSetting.SqlParameterList.Count > 0)
                    {
                        dbSetting.SqlParameterList.ForEach(x => oCmd.Parameters.Add(x));
                    }

                    oSqlCn.Open();
                    using (SqlDataAdapter oDa = new SqlDataAdapter())
                    {
                        oDa.SelectCommand = oCmd;
                        oDa.Fill(oDt);
                    }
                }
            }
            return oDt;
        }

        public int ExecuteNonQuery(DBSetting dbSetting)
        {
            SqlConnection oSqlCn = new SqlConnection();
            oSqlCn.ConnectionString = dbSetting.ConnectionString;
            SqlCommand oCmd = new SqlCommand(dbSetting.StoredProcedure, oSqlCn);
            oCmd.CommandType = dbSetting.DBCommandType;

            if (dbSetting.SqlParameterList.Count > 0)
            {
                dbSetting.SqlParameterList.ForEach(x => oCmd.Parameters.Add(x));
            }

            oSqlCn.Open();
            int intReturnNum = oCmd.ExecuteNonQuery();

            oCmd.Dispose();
            oSqlCn.Dispose();
            return intReturnNum;
        }

        public object ExecuteScalar(DBSetting dbSetting)
        {
            object oResult = null;
            using (SqlConnection oSqlCn = new SqlConnection())
            {
                oSqlCn.ConnectionString = dbSetting.ConnectionString;
                using (SqlCommand oCmd = new SqlCommand(dbSetting.StoredProcedure, oSqlCn))
                {
                    oCmd.CommandType = dbSetting.DBCommandType;

                    if (dbSetting.SqlParameterList.Count > 0)
                    {
                        dbSetting.SqlParameterList.ForEach(x => oCmd.Parameters.Add(x));
                    }

                    oSqlCn.Open();
                    oResult = oCmd.ExecuteScalar();
                }
            }
            return oResult;
        }
    }
}
