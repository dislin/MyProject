using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Models;

namespace MvcTest.Controllers
{
    public class TestDropDownController : Controller
    {
        //
        // GET: /TestDropDown/

        public ActionResult TestDropDown()
        {
            TestDropDownModel model = new TestDropDownModel();
            List<TestMember> MemberList = new List<TestMember>();
            MemberList.Add(new TestMember("Leo", 1));
            MemberList.Add(new TestMember("YH", 2));

            var oList = from m in MemberList
                        select new
                        {
                            key = m.Id,
                            name = m.Name
                        };
            model.selMemberList = new SelectList(oList, "key", "name");

            //List<SelectListItem> items = new List<SelectListItem>();
            //items.Add(new SelectListItem { Text = "LEO", Value = "LEO" });
            //items.Add(new SelectListItem { Text = "Yang", Value = "Yang" });
            //var dropdownList = new SelectList(items, "Text", "Value",
            //    Request["cofoundersList"] == null ? "Yang" : Request["cofoundersList"]);
            //this.ViewData["cofoundersList"] = dropdownList;

            //YangLogger.SimpleLogger.Debug(dropdownList.SelectedValue + " is selected");

            return View(model);
        }

    }

    public class TestMember
    {
        public TestMember(string name, int id)
        {
            this.Name = name;
            this.Id = id;
        }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
