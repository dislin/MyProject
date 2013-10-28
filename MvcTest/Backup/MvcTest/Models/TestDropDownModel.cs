using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Controllers;

namespace MvcTest.Models
{
    public class TestDropDownModel
    {
        public TestDropDownModel()
        {
            this.intMemberID = 1;
            this.oMemberList = new List<TestMember>();
        }
        public int intMemberID { get; set; }
        public SelectList selMemberList { get; set; }
        public List<TestMember> oMemberList { get; set; }
    }
}