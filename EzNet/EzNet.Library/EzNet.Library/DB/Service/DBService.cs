using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using EzNet.Library.DB.Entity;

namespace EzNet.Library.DB.Service
{
    public class DBService
    {
        public void SqlExecuteReader(DBSetting dbSetting, Func<IDataReader, bool> funcDataReaderDelegate)
        {
            ConfigHelper config = new ConfigHelper(ConfigEnum.Database, dbSetting.ConnectionKey);
            SqlConnection oSqlCn = new SqlConnection();
            oSqlCn.ConnectionString = config.GetValue();
            SqlCommand oCmd = new SqlCommand(dbSetting.StoredProcedure, oSqlCn);
            oCmd.CommandType = CommandType.StoredProcedure;

            if (dbSetting.SqlParameterList.Count > 0)
            {
                dbSetting.SqlParameterList.ForEach(x => oCmd.Parameters.Add(x));
            }

            oSqlCn.Open();
            using (SqlDataReader oDr = oCmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                funcDataReaderDelegate.Invoke(oDr);
            }
            oCmd.Dispose();
            oSqlCn.Dispose();
        }
    }
}
