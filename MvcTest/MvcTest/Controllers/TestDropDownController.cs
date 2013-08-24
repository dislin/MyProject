﻿using System;
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

        public ActionResult TestDropDown(TestDropDownModel model)
        {
            List<TestMember> memberList = new List<TestMember>();
            memberList.Add(new TestMember("Leo", 1, 30, "Male"));
            memberList.Add(new TestMember("YH", 2, 28, "Female"));

            var oList = from m in memberList
                        select new
                        {
                            key = m.Id,
                            name = m.Name
                        };

            model.selMemberList = new SelectList(oList, "key", "name");
            model.oMemberList = memberList.Where(x => x.Id == model.intMemberID).ToList();

            //YangLogger.SimpleLogger.Debug(model.intMemberID + " is selected");

            //Request["intMemberID"] == null ? "YH" : selectedMember.Name);

            //List<SelectListItem> items = new List<SelectListItem>();
            //items.Add(new SelectListItem { Text = "LEO", Value = "LEO"Y });
            //items.Add(new SelectListItem { Text = "Yang", Value = "Yang" });
            //var dropdownList = new SelectList(items, "Text", "Value",
            //    Request["cofoundersList"] == null ? "Yang" : Request["cofoundersList"]);
            //this.ViewData["cofoundersList"] = dropdownList;

            //YangLogger.SimpleLogger.Debug(dropdownList.SelectedValue + " is selected");

            return View(model);
        }

        public ActionResult TestDropDownJS()
        {
            TestDropDownModel model = new TestDropDownModel();
            List<TestMember> memberList = new List<TestMember>();
            memberList.Add(new TestMember("Leo", 1, 30, "Male"));
            memberList.Add(new TestMember("YH", 2, 28, "Female"));

            var oList = from m in memberList
                        select new
                        {
                            key = m.Id,
                            name = m.Name
                        };

            model.selMemberList = new SelectList(oList, "key", "name");
            return View(model);
        }
    }

    public class TestMember
    {
        public TestMember(string name, int id, int age, string sex)
        {
            this.Name = name;
            this.Id = id;
            this.Age = age;
            this.Sex = sex;
        }
        public string Name { get; set; }
        public int Id { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
    }
}
