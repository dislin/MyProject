using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTest.Controllers.Service
{
    public class AjaxServiceController : Controller
    {
        //
        // GET: /AjaxService/

        #region GetTestDropDownJSData
        public ActionResult GetTestDropDownJSData(int MemberID)
        {
            List<TestMember> memberList = new List<TestMember>();
            memberList.Add(new TestMember("Leo", 1, 30, "Male"));
            memberList.Add(new TestMember("YH", 2, 28, "Female"));
            return Json(memberList.Where(x => x.Id == MemberID).ToList());
        } 
        #endregion

    }
}
