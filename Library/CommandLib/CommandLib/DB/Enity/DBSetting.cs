using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CommandLib.DB.Enity
{
    public class DBSetting
    {
        public DBSetting() {
            SqlParameterList = new List<SqlParameter>();
            DBCommandType = CommandType.StoredProcedure;
        }
        public string ConnectionKey { set; get; }
        public string StoredProcedure { set; get; }
        public List<SqlParameter> SqlParameterList { set; get; }
        public CommandType DBCommandType { set; get; }
    }
}
