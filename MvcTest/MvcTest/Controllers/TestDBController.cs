using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.App_Code;
using System.Data.SqlClient;
using System.Data;
using MvcTest.App_Code.Common;
using MvcTest.Models;
using MvcTest.App_Code.Enum;
using System.Data.Linq;
using EzNet.Library.DB.Service;
using EzNet.Library.DB.Entity;
using EzNet.Library.Config.Service;
using EzNet.Library.Config.Entity;
using EzNet.Library.Config.Enum;


namespace MvcTest.Controllers
{
    public class TestDBController : Controller
    {
        //
        // GET: /TestDB/

        public ActionResult TestDB1()
        {
            List<TestRole> oRoleList = new List<TestRole>();
            #region old code comment
            //ConfigHelper config = new ConfigHelper(ConfigEnum.Database, "MainDB");
            //SqlConnection oSqlCn = new SqlConnection();
            //oSqlCn.ConnectionString = config.GetValue();
            //SqlCommand oCmd = new SqlCommand("role_GetRoleDataByID", oSqlCn);

            //SqlParameter param = new SqlParameter();
            //param.ParameterName = "@idnum";
            //param.SqlDbType = SqlDbType.Int;
            //param.Direction = ParameterDirection.Input;
            //param.Value = 1;
            //oCmd.Parameters.Add(param);


            //oCmd.CommandType = CommandType.StoredProcedure;
            //oSqlCn.Open();
            //SqlDataReader oDr = oCmd.ExecuteReader(CommandBehavior.CloseConnection);

            //if (oDr.HasRows)
            //{
            //    while (oDr.Read())
            //    {
            //        TestRole oRole = new TestRole()
            //        {
            //            idnum = oDr["idnum"].ToString().ToInt(),
            //            name = oDr["name"].ToString(),
            //            permission = oDr["permission"].ToString(),
            //            status = (CommonEnum.RoleStatusEnum)oDr["status"].ToInt(),
            //            isdelete = (CommonEnum.RoleDeleteEnum)oDr["isdelete"].ToInt()
            //        };

            //        oRoleList.Add(oRole);
            //    }
            //    oDr.Close();
            //}
            //oCmd.Dispose();
            //oSqlCn.Dispose(); 
            #endregion
            ConfigSetting dbconfig = new ConfigSetting(ConfigEnum.ConfigType.Database);
            DBSetting dbSetting = ConfigService.Instance.GetObject(dbconfig, new DBSetting()).FirstOrDefault();
            dbSetting.StoredProcedure = "role_GetRoleDataByID";

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@idnum";
            param.SqlDbType = SqlDbType.Int;
            param.Direction = ParameterDirection.Input;
            param.Value = 2;

            dbSetting.SqlParameterList.Add(param);
            Action<IDataReader> fnDR = (IDataReader oDr) =>
            {
                if (oDr.Read())
                {
                        TestRole oRole = new TestRole()
                        {
                            idnum = oDr["idnum"].ToString().ToInt(),
                            name = oDr["name"].ToString(),
                            permission = oDr["permission"].ToString(),
                            status = (CommonEnum.RoleStatusEnum)oDr["status"].ToInt(),
                            isdelete = (CommonEnum.RoleDeleteEnum)oDr["isdelete"].ToInt()
                        };
                        oRoleList.Add(oRole);
                }
                    oDr.Close();
            };

            DBService.Instance.SqlExecuteReader(dbSetting, fnDR);
            return View(oRoleList);
        }

    }
}
