using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel;
using System.Reflection;


namespace MvcTest.AppCode.Common
{
    public static class HtmlExtensions
    {
        public static int ToInt(this object obj) 
        {
            int num = 0;
            int.TryParse(obj.ToString(), out num);
            return num;
        }

        public static int ToInt(this object obj, int defaultValue)
        {
            int num = defaultValue;
            int.TryParse(obj.ToString(), out num);
            return num;
        }

        public static IHtmlString RenderScript(this HtmlHelper htmlHelper, string scriptUrl)
        {
            StringBuilder content = new StringBuilder();
            content.AppendFormat("<script src=\"/CDN/Js/{0}\" type=\"text/javascript\"></script>", scriptUrl);
            return htmlHelper.Raw(content.ToString());
        }
    }
}
