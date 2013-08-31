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

namespace MvcTest.Controllers
{
    public class TestDBController : Controller
    {
        //
        // GET: /TestDB/

        public ActionResult TestDB1()
        {
            ConfigHelper config = new ConfigHelper(ConfigEnum.Database, "MainDB");
            SqlConnection oSqlCn = new SqlConnection();
            oSqlCn.ConnectionString = config.GetValue();
            SqlCommand oCmd = new SqlCommand("role_GetRoleDataByID", oSqlCn);
            
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@idnum";
            param.SqlDbType = SqlDbType.Int;
            param.Direction = ParameterDirection.Input;
            param.Value = 1;
            oCmd.Parameters.Add(param);


            oCmd.CommandType = CommandType.StoredProcedure;
            List<TestRole> oRoleList = new List<TestRole>();
            oSqlCn.Open();
            SqlDataReader oDr = oCmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (oDr.HasRows)
            {
                while (oDr.Read())
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
            }
            oCmd.Dispose();
            oSqlCn.Dispose();

            return View(oRoleList);
        }

        public ActionResult TestDB2()
        {
            ConfigHelper config = new ConfigHelper(ConfigEnum.Database, "MainDB");
            DataClasses1DataContext testBO = new DataClasses1DataContext(config.GetValue());
            ISingleResult<role_GetAllRoleDataResult> results = testBO.role_GetAllRoleData();
            var oRoleList = new List<role_GetAllRoleDataResult>();
            foreach (var aRole in results)
            {
                oRoleList.Add(aRole);
            }

            return View(oRoleList);
        }

    }
}
