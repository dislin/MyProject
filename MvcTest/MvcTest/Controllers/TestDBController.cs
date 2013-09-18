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
using EzNets.Library.DB.Service;
using EzNets.Library.DB.Entity;
using EzNets.Library.Config.Service;
using EzNets.Library.Config.Entity;
using EzNets.Library.Config.Enum;
using Authentication.Role.Entity;
using Authentication.Role.Service;


namespace MvcTest.Controllers
{
    public class TestDBController : Controller
    {
        //
        // GET: /TestDB/

        public ActionResult TestDB1()
        {
            List<TestRole> oRoleList = new List<TestRole>();
            RoleEntity role = RoleService.Instance.GetRoleByID(1);
            TestRole oRole = new TestRole()
                        {
                            idnum = role.idnum,
                            name = role.name,
                            permission = role.permission,
                            status = (CommonEnum.RoleStatusEnum)role.status.ToInt(),
                            isdelete = (CommonEnum.RoleDeleteEnum)role.isdelete.ToInt()
                        };
            oRoleList.Add(oRole);
            return View(oRoleList);
        }

    }
}
