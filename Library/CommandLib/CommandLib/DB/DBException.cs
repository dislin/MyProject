using System;
using CommandLib.Exception;

namespace CommandLib.DB
{
    [Serializable]
    public class DBException : BaseException
    {
        public DBException(object error, DBError errorCode) : base(Convert.ToInt64(error).ToString(), errorCode.ToString()) { }
        public DBException(object error, System.Exception innerEx, DBError errorCode) : base(Convert.ToInt64(error).ToString(), innerEx, errorCode.ToString()) { }

        public DBException(string error, DBError errorCode) : base(error, errorCode.ToString()) { }
        public DBException(string error, System.Exception innerEx, DBError errorCode) : base(error, innerEx, errorCode.ToString()) { }
    }
}
