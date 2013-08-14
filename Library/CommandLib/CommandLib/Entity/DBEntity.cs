using System;
using System.Collections.Generic;
using System.Data;

namespace CommandLib.Entity
{
    public class DBEntity
    {
        #region DB Properties
        public IDbConnection Connection { get; set; }
        public IDataReader DataReader { get; set; }
        public DataProvider ProviderType { get; set; }
        public string ConnectionString { get; set; }
        public IDbCommand Command { get; set; }
        public IDbTransaction Transaction { get; set; }
        public IDbDataParameter[] Parameters { get; set; }

        public List<IDbDataParameter> Par { get; set; }
        #endregion
    }

    public class DBParameterEntity
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
    }

    public class DBSettingEntity
    {
        public DBSettingEntity(DataProvider dataProviderType, string conString, CommandType cmdType, string cmdText, bool isTrans)
        {
            ProviderType = dataProviderType;
            ConnectionString = conString;
            CommandType = cmdType;
            CommandText = cmdText;
            IsTransaction = isTrans;
            Parameters = new List<DBParameterEntity>();
        }

        private DBSettingEntity() { }

        public DataProvider ProviderType { get; set; }
        public string ConnectionString { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public List<DBParameterEntity> Parameters { get; set; }
        public bool IsTransaction { get; set; }

        public void AddParameters(string paramName, object paramValue, ParameterDirection direction)
        {
            Parameters.Add(new DBParameterEntity() { ParameterName = paramName, ParameterValue = paramValue });
        }
    }
}
