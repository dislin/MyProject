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

        public bool CreateRole(RoleEntity role)
        {
            bool isSuccess = false;
            ConfigSetting dbconfig = new ConfigSetting(ConfigEnum.ConfigType.Database);
            DBSetting dbSetting = ConfigService.Instance.GetObject(dbconfig, new DBSetting()).FirstOrDefault();
            dbSetting.StoredProcedure = "role_CreateRoleData";

            if (role == null)
            {
                return false;
            }

            for (int intNum = 0; intNum < 4; intNum += 1)
            {
                SqlParameter param = new SqlParameter();
                switch (intNum)
                {
                    case 0 :
                        param.ParameterName = "@name";
                        param.SqlDbType = SqlDbType.NVarChar;
                        param.Value = role.name;
                        break;
                    case 1:
                        param.ParameterName = "@status";
                        param.SqlDbType = SqlDbType.Int;
                        param.Value = (int)role.status;
                        break;
                    case 2:
                        param.ParameterName = "@permission";
                        param.SqlDbType = SqlDbType.NVarChar;
                        param.Value = role.permission;
                        break;
                    case 3:
                        param.ParameterName = "@creater";
                        param.SqlDbType = SqlDbType.NVarChar;
                        param.Value = role.creater;
                        break;
                }
                param.Direction = ParameterDirection.Input;
                dbSetting.SqlParameterList.Add(param);
            }

            Action<IDataReader> fnDR = (IDataReader oDr) =>
            {
                if (oDr.Read())
                {
                    isSuccess = (oDr["StatusValue"].ToInt() == 1);
                }
                oDr.Close();
            };

            DBService.Instance.SqlExecuteReader(dbSetting, fnDR);
            return isSuccess;
        }

        public bool UpdateRole(RoleEntity role)
        {
            bool isSuccess = false;
            ConfigSetting dbconfig = new ConfigSetting(ConfigEnum.ConfigType.Database);
            DBSetting dbSetting = ConfigService.Instance.GetObject(dbconfig, new DBSetting()).FirstOrDefault();
            dbSetting.StoredProcedure = "role_UpdateRoleData";

            if (role == null)
            {
                return false;
            }

            for (int intNum = 0; intNum < 5; intNum += 1)
            {
                SqlParameter param = new SqlParameter();
                switch (intNum)
                {
                    case 0:
                        param.ParameterName = "@name";
                        param.SqlDbType = SqlDbType.NVarChar;
                        param.Value = role.name;
                        break;
                    case 1:
                        param.ParameterName = "@status";
                        param.SqlDbType = SqlDbType.Int;
                        param.Value = (int)role.status;
                        break;
                    case 2:
                        param.ParameterName = "@permission";
                        param.SqlDbType = SqlDbType.NVarChar;
                        param.Value = role.permission;
                        break;
                    case 3:
                        param.ParameterName = "@modifier";
                        param.SqlDbType = SqlDbType.NVarChar;
                        param.Value = role.modifier;
                        break;
                    case 4:
                        param.ParameterName = "@idnum";
                        param.SqlDbType = SqlDbType.Int;
                        param.Value = (int)role.idnum;
                        break;
                }
                param.Direction = ParameterDirection.Input;
                dbSetting.SqlParameterList.Add(param);
            }

            Action<IDataReader> fnDR = (IDataReader oDr) =>
            {
                if (oDr.Read())
                {
                    isSuccess = (oDr["StatusValue"].ToInt() == 1);
                }
                oDr.Close();
            };

            DBService.Instance.SqlExecuteReader(dbSetting, fnDR);
            return isSuccess;
        }
    }
}
