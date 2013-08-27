using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.App_Code;
using CommandLib.Helper;
using System.Data.SqlClient;
using System.Data;


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
            oSqlCn.Open();
            SqlDataReader oDr = oCmd.ExecuteReader(CommandBehavior.CloseConnection);
            oCmd.Dispose();
            oSqlCn.Dispose();
            //ViewBag.configValue = config.GetValue();
            return View(ViewBag);
        }

    }
}
