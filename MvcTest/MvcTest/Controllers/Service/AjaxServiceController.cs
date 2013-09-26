using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Models;

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

        public ActionResult GetProduct()
        {
            List<ProductModel> oList = new List<ProductModel>();
            foreach (ProductModel oModel in RenderProduct())
            {
                oList.Add(oModel);
            }
            return Json(oList);
        }

        internal IEnumerable<ProductModel> RenderProduct()
        {
            Random oRnd = new Random();
            yield return new ProductModel()
            {
                ProductId = 1,
                ProductName = "Mobile",
                Price = oRnd.Next(10000, 20000),
                Quantity = oRnd.Next(1,100)
            };

            yield return new ProductModel()
            {
                ProductId = 2,
                ProductName = "Fruit",
                Price = oRnd.Next(100, 500),
                Quantity = oRnd.Next(10, 500)
            };

            yield return new ProductModel()
            {
                ProductId = 3,
                ProductName = "PS3",
                Price = oRnd.Next(10000, 15000),
                Quantity = oRnd.Next(5, 30)
            };
        }
    }
}
