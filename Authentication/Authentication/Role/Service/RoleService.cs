using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EzNet.Library.Common;
using Authentication.Role.Entity;
using EzNet.Library.Config.Entity;
using EzNet.Library.Config.Enum;
using EzNet.Library.DB.Entity;
using EzNet.Library.Config.Service;
using System.Data.SqlClient;
using System.Data;
using Authentication.Role.Enum;
using EzNet.Library.DB.Service;

namespace Authentication.Role.Service
{
    public class RoleService : Singleton<RoleService>
    {
        private RoleService() { }
        public RoleEntity GetRoleByID(int id)
        {
            ConfigSetting dbconfig = new ConfigSetting(ConfigEnum.ConfigType.Database);
            DBSetting dbSetting = ConfigService.Instance.GetObject(dbconfig, new DBSetting()).FirstOrDefault();
            dbSetting.StoredProcedure = "role_GetRoleDataByID";

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@idnum";
            param.SqlDbType = SqlDbType.Int;
            param.Direction = ParameterDirection.Input;
            param.Value = id;

            dbSetting.SqlParameterList.Add(param);
            RoleEntity oRole = new RoleEntity();
            Action<IDataReader> fnDR = (IDataReader oDr) =>
            {
                if (oDr.Read())
                {
                    oRole = new RoleEntity()
                    {
                        idnum = oDr["idnum"].ToString().ToInt(),
                        name = oDr["name"].ToString(),
                        permission = oDr["permission"].ToString(),
                        status = (RoleEnum.RoleStatusEnum)oDr["status"].ToInt(),
                        isdelete = (RoleEnum.RoleDeleteEnum)oDr["isdelete"].ToInt(),
                        creater = oDr["creater"].ToString(),
                        createdate = oDr["createdate"].ToString(),
                        modifier = oDr["modifier"].ToString(),
                        modifydate = oDr["modifydate"].ToString()
                    };
                }
                oDr.Close();
            };

            DBService.Instance.SqlExecuteReader(dbSetting, fnDR);
            return oRole;
        }
    }
}
