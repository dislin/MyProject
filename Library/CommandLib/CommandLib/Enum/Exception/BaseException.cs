using System;

namespace CommandLib.Exception
{
    [Serializable]
    public abstract class BaseException : System.Exception
    {
        public BaseException(object error, string errorCode) : base(Convert.ToInt64(error).ToString()) { ErrorCode = errorCode; }
        public BaseException(object error, System.Exception innerEx, string errorCode) : base(Convert.ToInt64(error).ToString(), innerEx) { ErrorCode = errorCode; }

        public BaseException(string error, string errorCode) : base(error) { ErrorCode = errorCode; }
        public BaseException(string error, System.Exception innerEx, string errorCode) : base(error, innerEx) { ErrorCode = errorCode; }

        public string ErrorCode { get; set; }
    }
}
