using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTest.Controllers
{
    public class TestDropDownController : Controller
    {
        //
        // GET: /TestDropDown/

        public ActionResult TestDropDown()
        {

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "LEO", Value = "LEO" });
            items.Add(new SelectListItem { Text = "Yang", Value = "Yang" });
            var dropdownList = new SelectList(items, "Text", "Value",
                Request["cofoundersList"] == null ? "Yang" : Request["cofoundersList"]);
            this.ViewData["cofoundersList"] = dropdownList;

            YangLogger.SimpleLogger.Debug(dropdownList.SelectedValue + " is selected");

            return View();
        }

    }
}
