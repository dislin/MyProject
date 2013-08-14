using System;
using System.Data;
using System.Data.SqlClient;

namespace CommandLib.DB
{
    public class DBManagerFactory
    {
        private DBManagerFactory() { }

        public static IDbConnection GetConnection(DataProvider providerType)
        {
            IDbConnection iDbConnection = null;

            switch (providerType)
            {
                case DataProvider.SqlServer:
                    iDbConnection = new SqlConnection();
                    break;
                default:
                    return null;
            }

            return iDbConnection;
        }

        public static IDbCommand GetCommand(DataProvider providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlCommand();
                default:
                    return null;
            }
        }

        public static IDbDataAdapter GetDataAdapter(DataProvider providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlDataAdapter();
                default:
                    return null;
            }
        }

        public static IDbDataParameter GetParameter(DataProvider providerType)
        {
            IDbDataParameter iDataParameter = null;

            switch (providerType)
            {
                case DataProvider.SqlServer:
                    iDataParameter = new SqlParameter();
                    break;
            }

            return iDataParameter;
        }

        public static IDbDataParameter[] GetParameters(DataProvider providerType, int paramsCount)
        {
            IDbDataParameter[] idbParams = new IDbDataParameter[paramsCount];

            switch (providerType)
            {
                case DataProvider.SqlServer:
                    for (int i = 0; i < paramsCount; ++i)
                    {
                        idbParams[i] = new SqlParameter();
                    }
                    break;
                default:
                    idbParams = null;
                    break;
            }

            return idbParams;
        }

        public static IDbConnection BuilConnection(DataProvider providerType, string connectionString)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlConnection(connectionString);
                default:
                    return null;
            }
        }

        public static IDbCommand BuildCommand(DataProvider providerType, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText)
        {
            IDbCommand idbCommand = null;

            switch (providerType)
            {
                case DataProvider.SqlServer:
                    idbCommand = new SqlCommand();
                    break;
                default:
                    return null;
            }

            idbCommand.Connection = connection;
            idbCommand.CommandText = commandText;
            idbCommand.CommandType = commandType;

            if (transaction != null)
            {
                idbCommand.Transaction = transaction;
            }

            return idbCommand;
        }

        public static IDbDataParameter BuildParameter(DataProvider providerType, string paramName, object paramValue)
        {
            IDbDataParameter idbParameter = null;

            switch (providerType)
            {
                case DataProvider.SqlServer:
                    idbParameter = new SqlParameter();
                    break;
            }

            idbParameter.ParameterName = paramName;
            idbParameter.Value = paramValue;

            if ((idbParameter.Direction == ParameterDirection.InputOutput) && (idbParameter.Value == null))
            {
                idbParameter.Value = DBNull.Value;
            }

            return idbParameter;
        }
    }
}
