using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Reflection;


namespace MvcTest.App_Code.Common
{
    public static class Extensions
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
    }
}
