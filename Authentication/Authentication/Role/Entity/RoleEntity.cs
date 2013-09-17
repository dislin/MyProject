using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Authentication.Role.Enum;

namespace Authentication.Role.Entity
{
    public class RoleEntity
    {
        public int idnum { get; set; }

        public string name { get; set; }

        public RoleEnum.RoleStatusEnum status { get; set; }

        public RoleEnum.RoleDeleteEnum isdelete { get; set; }

        public string permission { get; set; }

        public string creater { get; set; }

        public string createdate { get; set; }

        public string modifier { get; set; }

        public string modifydate { get; set; }
    }
}
