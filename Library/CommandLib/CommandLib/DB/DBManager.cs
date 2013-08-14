using System;
using System.Data;
using CommandLib.Entity;

namespace CommandLib.DB
{
    public class DBManager : IDBManager
    {
        private DBEntity dbEntity;

        #region Constructor
        public DBManager(DataProvider providerType, string connectionString)
        {
            dbEntity = new DBEntity { ProviderType = providerType, ConnectionString = connectionString };

        }
        #endregion

        public void Open()
        {
            if (dbEntity.Connection.State != ConnectionState.Open)
            {
                dbEntity.Connection.Open();
            }
        }

        public void CreateAndOpenConnection()
        {
            dbEntity.Connection = DBManagerFactory.GetConnection(dbEntity.ProviderType);
            dbEntity.Connection.ConnectionString = dbEntity.ConnectionString;

            if (dbEntity.Connection.State != ConnectionState.Open)
            {
                dbEntity.Connection.Open();
            }
        }

        public void Close()
        {
            if (dbEntity.Connection.State != ConnectionState.Closed)
            {
                dbEntity.Connection.Close();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Close();
            dbEntity = null;
        }

        public void CreateParameters(int paramsCount)
        {
            dbEntity.Parameters = new IDbDataParameter[paramsCount];
            dbEntity.Parameters = DBManagerFactory.GetParameters(dbEntity.ProviderType, paramsCount);
        }

        public void AddParameters(int index, string paramName, object objValue)
        {
            if (index < dbEntity.Parameters.Length)
            {
                dbEntity.Parameters[index].ParameterName = paramName;
                dbEntity.Parameters[index].Value = objValue;
            }
        }

        public void BeginTransaction()
        {
            if (dbEntity.Connection != null)
            {
                dbEntity.Transaction = dbEntity.Connection.BeginTransaction();
            }
            else
            {
                throw new DBException(DBError.NO_CONNECTION, DBError.NO_CONNECTION);
            }
        }

        public void CommitTransaction()
        {
            if (dbEntity.Transaction != null)
            {
                dbEntity.Transaction.Commit();
            }
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            dbEntity.Command = DBManagerFactory.GetCommand(dbEntity.ProviderType);
            this.PrepareCommand(dbEntity.Command, dbEntity.Connection, dbEntity.Transaction, commandType, commandText, dbEntity.Parameters);
            int returnValue = dbEntity.Command.ExecuteNonQuery();
            dbEntity.Command.Parameters.Clear();

            return returnValue;
        }

        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            dbEntity.Command = DBManagerFactory.GetCommand(dbEntity.ProviderType);
            this.PrepareCommand(dbEntity.Command, dbEntity.Connection, dbEntity.Transaction, commandType, commandText, dbEntity.Parameters);
            object returnValue = dbEntity.Command.ExecuteScalar();
            dbEntity.Command.Parameters.Clear();

            return returnValue;
        }

        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            dbEntity.Command = DBManagerFactory.GetCommand(dbEntity.ProviderType);
            this.PrepareCommand(dbEntity.Command, dbEntity.Connection, dbEntity.Transaction, commandType, commandText, dbEntity.Parameters);
            IDbDataAdapter dataAdapter = DBManagerFactory.GetDataAdapter(dbEntity.ProviderType);
            dataAdapter.SelectCommand = dbEntity.Command;
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dbEntity.Command.Parameters.Clear();

            return dataSet;
        }

        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            dbEntity.Command = DBManagerFactory.GetCommand(dbEntity.ProviderType);
            dbEntity.Command.Connection = dbEntity.Connection;
            PrepareCommand(dbEntity.Command, dbEntity.Connection, dbEntity.Transaction, commandType, commandText, dbEntity.Parameters);
            dbEntity.DataReader = dbEntity.Command.ExecuteReader();
            dbEntity.Command.Parameters.Clear();

            return dbEntity.DataReader;
        }

        public void CloseReader()
        {
            if (dbEntity.DataReader != null)
            {
                dbEntity.DataReader.Close();
            }
        }

        #region Private Method
        private void AttachParameters(IDbCommand command, IDbDataParameter[] commandParameters)
        {
            foreach (IDbDataParameter idbParameter in commandParameters)
            {
                if ((idbParameter.Direction == ParameterDirection.InputOutput) && (idbParameter.Value == null))
                {
                    idbParameter.Value = DBNull.Value;
                }

                command.Parameters.Add(idbParameter);
            }
        }

        private void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDbDataParameter[] commandParameters)
        {
            command.Connection = connection;
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            if (commandParameters != null)
            {
                this.AttachParameters(command, commandParameters);
            }
        }
        #endregion
    }
}