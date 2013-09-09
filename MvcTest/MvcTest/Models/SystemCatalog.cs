using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTest.Models
{
    public class SystemCatalog
    {
        public SystemCatalog()
        {
            Item = new List<SystemCatalogItem>();
        }
        public string GroupName { set; get; }
        public List<SystemCatalogItem> Item { set; get; }
    }

    public class SystemCatalogItem
    {
        public string Name { set; get; }
        public string Url { set; get; }
        public string Target { set; get; }
    }
}