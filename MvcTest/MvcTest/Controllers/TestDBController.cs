using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.App_Code;
using CommandLib.Helper;
using System.Data.SqlClient;
using System.Data;
using MvcTest.App_Code.Common;
using MvcTest.Models;
using MvcTest.App_Code.Enum;
using System.Data.Linq;
using CommandLib.DB.Enity;
using CommandLib.DB.Service;

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
            DBSetting dbSetting = new DBSetting()
            {
                ConnectionKey = "MainDB",
                StoredProcedure = "role_GetRoleDataByID"
            };
            DBService dbservice = new DBService();

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@idnum";
            param.SqlDbType = SqlDbType.Int;
            param.Direction = ParameterDirection.Input;
            param.Value = 1;

            dbSetting.SqlParameterList.Add(param);
            Func<IDataReader, bool> fnDR = (IDataReader oDr) =>
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
                    return true;
            };

            dbservice.SqlExecuteReader(dbSetting, fnDR);
            return View(oRoleList);
        }

    }
}
