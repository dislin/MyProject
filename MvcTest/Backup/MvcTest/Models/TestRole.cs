using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTest.Controllers;
using MvcTest.App_Code.Enum;
using System.Xml.Serialization;

namespace MvcTest.Models
{
    public class TestRole
    {
        [XmlAttribute("idnum")]
        public int idnum {get; set;}

        [XmlAttribute("name")]
        public string name  {get; set;}

        [XmlAttribute("status")]
        public CommonEnum.RoleStatusEnum status { get; set; }

        [XmlAttribute("isdelete")]
        public CommonEnum.RoleDeleteEnum isdelete { get; set; }

        [XmlAttribute("permission")]
        public string permission  {get; set;}

        [XmlIgnore]
        public string creater  {get; set;}

        [XmlIgnore]
        public string createdate  {get; set;}

        [XmlIgnore]
        public string modifier  {get; set;}

        [XmlIgnore]
        public string modifydate  {get; set;}
    }
}