using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTest.App_Code.Enum
{
    public class CommonEnum
    {
        public enum RoleStatusEnum
        {
            InActive = 0,
            Active = 1
        }

        public enum RoleDeleteEnum
        {
            None = 0,
            Delete = 1
        }
    }
}