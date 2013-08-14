using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLib.Entity;
using CommandLib.DB;
using System.Data;
using CommandLib.Helper;

namespace CommandLib.Object
{
    public class SPParameterObject
    {
        public static List<SPParameterEntity> GetParameter(string SPName)
        {
            List<SPParameterEntity> list = new List<SPParameterEntity>();
            DBSettingEntity dbSetting = new DBSettingEntity(DataProvider.SqlServer, DBSetting.GetDBConnection(DBSource.MainDBConstr), CommandType.StoredProcedure, DBSetting.GetSPValue("GetSPParameters"), false);
            dbSetting.AddParameters("@SPName", DBSetting.GetSPValue(SPName), ParameterDirection.Input);
            Func<IDataReader, bool> func = delegate(IDataReader dr)
            {
                List<string> drlist = new List<string>();

                while (dr.Read())
                {
                    list.Add((SPParameterEntity)DBHelper.DataReaderMapping(dr, new SPParameterEntity()));
                }
                return true;
            };
            DBManager2.Instance.ExecuteReader(dbSetting, func);
            return list;
        }
    }
}
