using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CommandLib.Entity;

namespace CommandLib.DB
{
    public class DBManager2
    {
        private static DBManager2 instance = new DBManager2();

        public static DBManager2 Instance
        {
            get { return instance; }
        }

        public int ExecuteNonQuery(DBSettingEntity dbSetting)
        {
            try
            {
                int result;

                using (IDbConnection con = DBManagerFactory.BuilConnection(dbSetting.ProviderType, dbSetting.ConnectionString))
                {
                    con.Open();
                    IDbTransaction idbTransaction = null;

                    if (dbSetting.IsTransaction)
                    {
                        idbTransaction = con.BeginTransaction();
                    }

                    using (IDbCommand cmd = DBManagerFactory.BuildCommand(dbSetting.ProviderType, con, idbTransaction, dbSetting.CommandType, dbSetting.CommandText))
                    {
                        this.AttachParameters(cmd, dbSetting.Parameters, dbSetting.ProviderType);

                        result = cmd.ExecuteNonQuery();

                        if (idbTransaction != null)
                        {
                            idbTransaction.Commit();
                        }
                    }
                }

                return result;
            }
            catch (SqlException ex)
            {
                throw this.TranslateException(ex);
            }
        }

        public void ExecuteReader(DBSettingEntity dbSetting, Func<IDataReader, bool> funcProccessDataReader)
        {
            try
            {
                using (IDbConnection con = DBManagerFactory.BuilConnection(dbSetting.ProviderType, dbSetting.ConnectionString))
                {
                    con.Open();
                    IDbTransaction idbTransaction = null;

                    if (dbSetting.IsTransaction)
                    {
                        idbTransaction = con.BeginTransaction();
                    }

                    using (IDbCommand cmd = DBManagerFactory.BuildCommand(dbSetting.ProviderType, con, idbTransaction, dbSetting.CommandType, dbSetting.CommandText))
                    {
                        this.AttachParameters(cmd, dbSetting.Parameters, dbSetting.ProviderType);

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            funcProccessDataReader.Invoke(dr);
                        }

                        if (idbTransaction != null)
                        {
                            idbTransaction.Commit();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw this.TranslateException(ex);
            }
        }

        public object ExecuteScalar(DBSettingEntity dbSetting)
        {
            try
            {
                object result;

                using (IDbConnection con = DBManagerFactory.BuilConnection(dbSetting.ProviderType, dbSetting.ConnectionString))
                {
                    con.Open();
                    IDbTransaction idbTransaction = null;

                    if (dbSetting.IsTransaction)
                    {
                        idbTransaction = con.BeginTransaction();
                    }

                    using (IDbCommand cmd = DBManagerFactory.BuildCommand(dbSetting.ProviderType, con, idbTransaction, dbSetting.CommandType, dbSetting.CommandText))
                    {
                        this.AttachParameters(cmd, dbSetting.Parameters, dbSetting.ProviderType);

                        result = cmd.ExecuteScalar();

                        if (idbTransaction != null)
                        {
                            idbTransaction.Commit();
                        }
                    }
                }

                return result;
            }
            catch (SqlException ex)
            {
                throw this.TranslateException(ex);
            }
        }

        public void ExecuteDataSet(DBSettingEntity dbSetting, Func<DataSet, bool> funcProccessDataSet)
        {
            try
            {
                using (IDbConnection con = DBManagerFactory.BuilConnection(dbSetting.ProviderType, dbSetting.ConnectionString))
                {
                    con.Open();
                    IDbTransaction idbTransaction = null;

                    if (dbSetting.IsTransaction)
                    {
                        idbTransaction = con.BeginTransaction();
                    }

                    using (IDbCommand cmd = DBManagerFactory.BuildCommand(dbSetting.ProviderType, con, idbTransaction, dbSetting.CommandType, dbSetting.CommandText))
                    {
                        this.AttachParameters(cmd, dbSetting.Parameters, dbSetting.ProviderType);

                        IDbDataAdapter dataAdapter = DBManagerFactory.GetDataAdapter(dbSetting.ProviderType);
                        dataAdapter.SelectCommand = cmd;
                        DataSet dataSet = new DataSet();
                        dataAdapter.Fill(dataSet);
                        funcProccessDataSet.Invoke(dataSet);

                        if (idbTransaction != null)
                        {
                            idbTransaction.Commit();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw this.TranslateException(ex);
            }
        }

        #region Private Method
        public DBException TranslateException(SqlException ex)
        {
            switch (ex.Number)
            {
                case 547:
                    // ForeignKey Violation 
                    return new DBException(ex.Message, ex, DBError.DATA_REFERENCE_NOT_FOUND);
                case 2627:
                case 2601:
                    // Unique Index/Constriant Violation 
                    return new DBException(ex.Message, ex, DBError.DATA_DUPLICATED);
                default:
                    // throw a general DAL Exception 
                    return new DBException(ex.Message, ex, DBError.DATA_ACCESS_ERROR);
            }
        }

        private void AttachParameters(IDbCommand command, List<DBParameterEntity> lstParams, DataProvider providerType)
        {
            foreach (DBParameterEntity item in lstParams)
            {
                command.Parameters.Add(DBManagerFactory.BuildParameter(providerType, item.ParameterName, item.ParameterValue));
            }
        }
        #endregion
    }
}
