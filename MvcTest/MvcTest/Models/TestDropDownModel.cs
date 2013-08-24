using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTest.Models
{
    public class TestDropDownModel
    {
        public int intMemberID { get; set; }
        public SelectList selMemberList { get; set; }
    }
}