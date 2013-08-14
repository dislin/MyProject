using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Reflection;


namespace CommandLib.Utilities
{
    public static class Extensions
    {
        public static SelectList ToSelectList<TEnum>(this TEnum tenum)
        {
            var lists = from TEnum e in Enum.GetValues(typeof(TEnum))
                        select new { Id = e.GetHashCode(), Name = e.ToString() };

            return new SelectList(lists, "Id", "Name");

        }
        public static SelectList ToSelectList<TEnum>(this TEnum tenum,bool IsSelected)
        {
            var lists = from TEnum e in Enum.GetValues(typeof(TEnum))
                        select new { Id = e.GetHashCode(), Name = e.ToString() };
            if (IsSelected)
            {
                return new SelectList(lists, "Id", "Name", tenum);
            }
            else {
                return new SelectList(lists, "Id", "Name");
            }
        }

        public static SelectList ToSelectList<TEnum>(this TEnum tenum, bool IsSelected, int rejectValue)
        {
            var lists = from TEnum e in Enum.GetValues(typeof(TEnum))
                        where e.GetHashCode() != rejectValue
                        select new { Id = e.GetHashCode(), Name = e.ToString() };
            if (IsSelected)
            {
                return new SelectList(lists, "Id", "Name", tenum);
            }
            else
            {
                return new SelectList(lists, "Id", "Name");
            }
        }

        public static string ToDescription(this object obj)
        {
            return ToDescription(obj, false);
        }

        public static string ToDescription(this object obj, bool isTop)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            try
            {
                Type _enumType = obj.GetType();
                DescriptionAttribute dna = null;
                if (isTop)
                {
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(_enumType, typeof(DescriptionAttribute));
                }
                else
                {
                    FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, obj));
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(
                       fi, typeof(DescriptionAttribute));
                }
                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                    return dna.Description;
            }
            catch
            {
            }
            return obj.ToString();
        }

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
