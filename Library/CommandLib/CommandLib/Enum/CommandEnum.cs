using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


    public enum EnableEnum : int
    {
        Enable  = 1,
        Disable = 0,
        NULL    = -1
    }

    public enum StatusEnum : int
    {
        Normal  = 0,
        Delete  = 1,
        NULL    = -1
    }
    public enum DataProvider:int
    {
        Oracle      = 0,
        SqlServer   = 1,
        OleDb       = 2,
        Odbc        = 3
    }

    public enum DBSource
    {
        MainDBConstr
    }

    public enum DBError
    {
        /// <summary>800000001 - Error On execute Stored Proc</summary>
        DATA_ACCESS_ERROR = 800000001,
        /// <summary>800000002 - Connection Not Created</summary>
        NO_CONNECTION = 800000002,
        /// <summary>800000003 - Not Supported Data Provider</summary>
        NOT_SUPPORTED_DATA_PROVIDER = 800000003,
        /// <summary>800000004 - Reference data not found</summary>
        DATA_REFERENCE_NOT_FOUND = 800000004,
        /// <summary>800000005 - Data duplicate</summary>
        DATA_DUPLICATED = 800000005,
    }

    public enum CacheEnum : int
    {
        Unknow          = 0,
        StatusDataCache = 1,
        PersonDataCache = 2
    }

    public enum DateTimeFormatEnum
    {
        [Description("yyyy/MM/dd HH:ss")]
        Normal      = 0,
        [Description("yyyy/MM/dd")]
        ShortDate   = 1
    }
