using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Models;
using EzNet.Library.Utilities;


namespace MvcTest.Controllers
{
    public class TestDropDownController : BaseExceptionController
    {
        //
        // GET: /TestDropDown/

        #region TestDropDown
        public ActionResult TestDropDown(TestDropDownModel model)
        {
            m_logger.Debug(model.intMemberID + " is selected");

            List<TestMember> memberList = new List<TestMember>();
            memberList.Add(new TestMember("Leo", 1, 30, "Male"));
            memberList.Add(new TestMember("YH", 2, 28, "Female"));
            model.selMemberList = GetMemberSelectList(memberList);
            model.oMemberList = memberList.Where(x => x.Id == model.intMemberID).ToList();
            
            return View(model);
        } 
        #endregion

        #region TestDropDownJS
        public ActionResult TestDropDownJS()
        {
            TestDropDownModel model = new TestDropDownModel();
            List<TestMember> memberList = new List<TestMember>();
            memberList.Add(new TestMember("Leo", 1, 30, "Male"));
            memberList.Add(new TestMember("YH", 2, 28, "Female"));
            model.selMemberList = GetMemberSelectList(memberList);
            return View(model);
        } 
        #endregion

        public ActionResult TestException(TestDropDownModel model)
        {
            Request["ex"].ToString();
            return View(model);
        } 

        #region Internal Function

        #region GetMemberSelectList
        internal SelectList GetMemberSelectList(List<TestMember> memberList)
        {
            var oList = from m in memberList
                        select new
                        {
                            key = m.Id,
                            name = m.Name
                        };

            return (new SelectList(oList, "key", "name"));
        }  
        #endregion

        #endregion
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
