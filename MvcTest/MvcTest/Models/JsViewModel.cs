using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTest.Models
{
    public class JsViewModel
    {
        public IHtmlString[] BundleCollection { get; set; }
        public IHtmlString[] JavasciptCollection { get; set; }
    }
}