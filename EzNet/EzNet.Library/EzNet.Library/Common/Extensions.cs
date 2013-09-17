using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EzNet.Library.Common
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
