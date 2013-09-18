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
        public void SqlExecuteReader(DBSetting dbSetting, Action<IDataReader> actDataReaderDelegate)
        {
            SqlConnection oSqlCn = new SqlConnection();
            oSqlCn.ConnectionString = dbSetting.ConnectionString;
            SqlCommand oCmd = new SqlCommand(dbSetting.StoredProcedure, oSqlCn);
            oCmd.CommandType = CommandType.StoredProcedure;

            if (dbSetting.SqlParameterList.Count > 0)
            {
                dbSetting.SqlParameterList.ForEach(x => oCmd.Parameters.Add(x));
            }

            oSqlCn.Open();
            using (SqlDataReader oDr = oCmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                actDataReaderDelegate.Invoke(oDr);
            }
            oCmd.Dispose();
            oSqlCn.Dispose();
        }
    }
}
