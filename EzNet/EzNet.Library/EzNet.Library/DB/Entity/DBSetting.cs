using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EzNet.Library.DB.Entity
{
    public class DBSetting
    {
        public DBSetting()
        {
            SqlParameterList = new List<SqlParameter>();
            DBCommandType = CommandType.StoredProcedure;
        }
        public string ConnectionKey { set; get; }
        public string StoredProcedure { set; get; }
        public List<SqlParameter> SqlParameterList { set; get; }
        public CommandType DBCommandType { set; get; }
    }
}
