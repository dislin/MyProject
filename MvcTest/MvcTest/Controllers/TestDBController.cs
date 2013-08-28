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
            SqlCommand oCmd = new SqlCommand("role_GetAllRoleData", oSqlCn);
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

    }
}
