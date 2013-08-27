using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Controllers;
using MvcTest.App_Code.Enum;

namespace MvcTest.Models
{
    public class TestRole
    {
        public int idnum {get; set;}
        public string name  {get; set;}
        public CommonEnum.RoleStatusEnum status { get; set; }
        public CommonEnum.RoleDeleteEnum isdelete { get; set; }
        public string permission  {get; set;}
        public string creater  {get; set;}
        public string createdate  {get; set;}
        public string modifier  {get; set;}
        public string modifydate  {get; set;}
    }
}