using System;
using System.Data;

namespace CommandLib.DB
{
    public interface IDBManager : IDisposable
    {
        void Open();
        void CreateAndOpenConnection();

        void BeginTransaction();
        void CommitTransaction();

        void CloseReader();
        void Close();

        void CreateParameters(int paramsCount);
        void AddParameters(int index, string paramName, object objValue);

        IDataReader ExecuteReader(CommandType commandType, string commandText);
        DataSet ExecuteDataSet(CommandType commandType, string commandText);
        object ExecuteScalar(CommandType commandType, string commandText);
        int ExecuteNonQuery(CommandType commandType, string commandText);
    }
}
