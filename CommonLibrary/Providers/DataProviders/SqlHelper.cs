using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Data.Common;


namespace CommonLibrary.Providers.DataProviders
{
    public sealed class SqlHelper
    {
        private enum SqlConnectionOwnership
        {
            Internal,
            External
        }

        private SqlHelper(){}

        #region Set Parameter
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) && (parameterValues == null))
            {
                return;
            }
            else
            {
                if (commandParameters.Length != parameterValues.Length)
                    throw new System.ArgumentException("Parameter count does not match Parameter Value count.");
                int i2 = checked(commandParameters.Length - 1);
                int i3 = i2;
                for (int i1 = 0; i1 <= i3; i1 = checked(i1 + 1))
                {
                    if ((parameterValues[i1] is System.Data.IDbDataParameter))
                    {
                        System.Data.IDbDataParameter idbDataParameter = (System.Data.IDbDataParameter)parameterValues[i1];
                        if (idbDataParameter.Value == null)
                            commandParameters[i1].Value = System.DBNull.Value;
                        else
                            commandParameters[i1].Value = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(idbDataParameter.Value);
                    }
                    else if (parameterValues[i1] == null)
                    {
                        commandParameters[i1].Value = System.DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i1].Value = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(parameterValues[i1]);
                    }
                }
            }
        }

        private static void AssignParameterValues(SqlParameter[] commandParameters, System.Data.DataRow dataRow)
        {
            int i1 = 0;

            if ((commandParameters == null) || (dataRow == null))
            {
                return;
            }
            else
            {
                SqlParameter[] sqlParameterArr = commandParameters;
                for (int i2 = 0; i2 < sqlParameterArr.Length; i2 = checked(i2 + 1))
                {
                    SqlParameter sqlParameter = sqlParameterArr[i2];
                    if ((sqlParameter.ParameterName == null) || (sqlParameter.ParameterName.Length <= 1))
                        throw new System.Exception(System.String.Format("Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: ' {1}' .", i1, sqlParameter.ParameterName));
                    if (dataRow.Table.Columns.IndexOf(sqlParameter.ParameterName.Substring(1)) != -1)
                        sqlParameter.Value = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(dataRow[sqlParameter.ParameterName.Substring(1)]);
                    i1 = checked(i1 + 1);
                }
            }
        }

        private static void AttachParameters(System.Data.SqlClient.SqlCommand command, params SqlParameter[] commandParameters)
        {
            if (command == null)
                throw new System.ArgumentNullException("command");
            if (commandParameters != null)
            {
                SqlParameter[] sqlParameterArr = commandParameters;
                for (int i = 0; i < sqlParameterArr.Length; i = checked(i + 1))
                {
                    SqlParameter sqlParameter = sqlParameterArr[i];
                    if (sqlParameter != null)
                    {
                        if (((sqlParameter.Direction == System.Data.ParameterDirection.InputOutput) || (sqlParameter.Direction == System.Data.ParameterDirection.Input)) && (sqlParameter.Value == null))
                            sqlParameter.Value = System.DBNull.Value;
                        command.Parameters.Add(sqlParameter);
                    }
                }
            }
        }

        private static void PrepareCommand(System.Data.SqlClient.SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, ref bool mustCloseConnection)
        {
            if (command == null)
                throw new System.ArgumentNullException("command");
            if ((commandText == null) || (commandText.Length == 0))
                throw new System.ArgumentNullException("commandText");
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
                mustCloseConnection = true;
            }
            else
            {
                mustCloseConnection = false;
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                if (transaction.Connection == null)
                    throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
                AttachParameters(command, commandParameters);
        }      
        #endregion Set Parameter

        #region ExecuteDataset
        public static System.Data.DataSet ExecuteDataset(string connectionString, string spName, object[] parameterValues)
        {
            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static System.Data.DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, null);
        }

        public static System.Data.DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, null);
        }

        public static System.Data.DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            System.Data.SqlClient.SqlConnection sqlConnection = null;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            try
            {
                sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
                sqlConnection.Open();
                return ExecuteDataset(sqlConnection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
        }

        public static System.Data.DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            System.Data.SqlClient.SqlDataAdapter sqlDataAdapter = null;

            if (connection == null)
                throw new System.ArgumentNullException("connection");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand();
            System.Data.DataSet dataSet1 = new System.Data.DataSet();
            bool flag = false;
            PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, ref flag);
            try
            {
                sqlDataAdapter = new System.Data.SqlClient.SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataSet1);
                sqlCommand.Parameters.Clear();
            }
            finally
            {
                if (sqlDataAdapter != null)
                    sqlDataAdapter.Dispose();
            }
            if (flag)
                connection.Close();
            return dataSet1;
        }

        public static System.Data.DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            System.Data.SqlClient.SqlDataAdapter sqlDataAdapter = null;

            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand();
            System.Data.DataSet dataSet1 = new System.Data.DataSet();
            bool flag = false;
            PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, ref flag);
            try
            {
                sqlDataAdapter = new System.Data.SqlClient.SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataSet1);
                sqlCommand.Parameters.Clear();
            }
            finally
            {
                if (sqlDataAdapter != null)
                    sqlDataAdapter.Dispose();
            }
            return dataSet1;
        }

        public static System.Data.DataSet ExecuteDataset(SqlTransaction transaction, string spName, object[] parameterValues)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        public static System.Data.DataSet ExecuteDataset(SqlConnection connection, string spName, object[] parameterValues)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        public static System.Data.DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, null);
        }
        #endregion ExecuteDataset              

        #region ExecuteNonQuery
        public static int ExecuteNonQuery(string connectionString, string spName, object[] parameterValues)
        {
            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            System.Data.SqlClient.SqlConnection sqlConnection = null;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            try
            {
                sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
                sqlConnection.Open();
                return ExecuteNonQuery(sqlConnection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, null);
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, object[] parameterValues)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand();
            bool flag = false;
            PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, ref flag);
            int i2 = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            return i2;
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, null);
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, null);
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand();
            bool flag = false;
            PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, ref flag);
            int i2 = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();
            if (flag)
                connection.Close();
            return i2;
        }

        public static int ExecuteNonQuery(SqlConnection connection, string spName, object[] parameterValues)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }
        #endregion ExecuteNonQuery        

        #region ExecuteDatasetTypedParams
        public static System.Data.DataSet ExecuteDatasetTypedParams(string connectionString, string spName, System.Data.DataRow dataRow)
        {
            System.Data.DataSet dataSet;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                dataSet = ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                dataSet = ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
            return dataSet;
        }

        public static System.Data.DataSet ExecuteDatasetTypedParams(SqlConnection connection, string spName, System.Data.DataRow dataRow)
        {
            System.Data.DataSet dataSet;

            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                dataSet = ExecuteDataset(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                dataSet = ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
            return dataSet;
        }

        public static System.Data.DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, string spName, System.Data.DataRow dataRow)
        {
            System.Data.DataSet dataSet;

            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                dataSet = ExecuteDataset(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                dataSet = ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
            return dataSet;
        }
        #endregion ExecuteDatasetTypedParams

        #region ExecuteNonQueryTypedParams
        public static int ExecuteNonQueryTypedParams(string connectionString, string spName, System.Data.DataRow dataRow)
        {
            int i;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                i = ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                i = ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
            return i;
        }

        public static int ExecuteNonQueryTypedParams(SqlConnection connection, string spName, System.Data.DataRow dataRow)
        {
            int i;

            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                i = ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                i = ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
            return i;
        }

        public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, string spName, System.Data.DataRow dataRow)
        {
            int i;

            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                i = ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                i = ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
            return i;
        }
        #endregion ExecuteNonQueryTypedParams                            

        #region ExecuteReader
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, object[] parameterValues)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        public static SqlDataReader ExecuteReader(string connectionString, string spName, object[] parameterValues)
        {
            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, null);
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(connectionString, commandType, commandText, commandParameters);
        }

        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            // ExecuteReader #1
            if (connection == null) throw new ArgumentNullException("connection");
            bool mustCloseConnection = false;
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);
                SqlDataReader dataReader;
                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                // HACK: There is a problem here, the output parameter values are fletched
                // when the reader is closed, so if the parameters are detached from the command
                // then the SqlReader can set its values.
                // When this happen, the parameters can磘 be used again in other command.
                bool canClear = true;
                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }
                return dataReader;
            }

            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        /// <remarks>
        /// SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>        
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, null);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, null);
        }
        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, object[] parameterValues)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteReader(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }
        ///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

       
        #endregion ExecuteReader

        #region ExecuteReaderTypedParams (DataRow)
        public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, string spName, System.Data.DataRow dataRow)
        {
            SqlDataReader sqlDataReader;

            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                sqlDataReader = ExecuteReader(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                sqlDataReader = ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
            return sqlDataReader;
        }

        public static SqlDataReader ExecuteReaderTypedParams(string connectionString, string spName, System.Data.DataRow dataRow)
        {
            SqlDataReader sqlDataReader;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                sqlDataReader = ExecuteReader(connectionString, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                sqlDataReader = ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
            return sqlDataReader;
        }

        public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, string spName, System.Data.DataRow dataRow)
        {
            SqlDataReader sqlDataReader;

            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                sqlDataReader = ExecuteReader(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                sqlDataReader = ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
            return sqlDataReader;
        }
        #endregion ExecuteReaderTypedParams

        #region ExecuteScalar
        public static object ExecuteScalar(string connectionString, string spName, object[] parameterValues)
        {
            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, null);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, null);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            System.Data.SqlClient.SqlConnection sqlConnection = null;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            try
            {
                sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
                sqlConnection.Open();
                return ExecuteScalar(sqlConnection, commandType, commandText, commandParameters);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand();
            bool flag = false;
            PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, ref flag);
            object obj2 = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(sqlCommand.ExecuteScalar());
            sqlCommand.Parameters.Clear();
            if (flag)
                connection.Close();
            return obj2;
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand();
            bool flag = false;
            PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, ref flag);
            object obj2 = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(sqlCommand.ExecuteScalar());
            sqlCommand.Parameters.Clear();
            return obj2;
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, null);
        }

        public static object ExecuteScalar(SqlConnection connection, string spName, object[] parameterValues)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        public static object ExecuteScalar(SqlTransaction transaction, string spName, object[] parameterValues)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }

       
        #endregion ExecuteScalar

        #region ExecuteScalarTypedParams (DataRow)
        public static object ExecuteScalarTypedParams(string connectionString, string spName, System.Data.DataRow dataRow)
        {
            object obj;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                obj = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, sqlParameterArr));
            }
            else
            {
                obj = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(ExecuteScalar(connectionString, CommandType.StoredProcedure, spName));
            }
            return obj;
        }

        public static object ExecuteScalarTypedParams(SqlTransaction transaction, string spName, System.Data.DataRow dataRow)
        {
            object obj;

            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                obj = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(ExecuteScalar(transaction, CommandType.StoredProcedure, spName, sqlParameterArr));
            }
            else
            {
                obj = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(ExecuteScalar(transaction, CommandType.StoredProcedure, spName));
            }
            return obj;
        }

        public static object ExecuteScalarTypedParams(SqlConnection connection, string spName, System.Data.DataRow dataRow)
        {
            object obj;

            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                obj = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(ExecuteScalar(connection, CommandType.StoredProcedure, spName, sqlParameterArr));
            }
            else
            {
                obj = System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(ExecuteScalar(connection, CommandType.StoredProcedure, spName));
            }
            return obj;
        }

        #endregion ExecuteScalarTypedParams (DataRow)

        #region ExecuteXmlReader
        public static System.Xml.XmlReader ExecuteXmlReader(SqlConnection connection, string spName, object[] parameterValues)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        public static System.Xml.XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(transaction, commandType, commandText, null);
        }

        public static System.Xml.XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(connection, commandType, commandText, null);
        }

        public static System.Xml.XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {            
            return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static System.Xml.XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, object[] parameterValues)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        public static System.Xml.XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand();
            bool flag = false;
            PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, ref flag);
            System.Xml.XmlReader xmlReader2 = sqlCommand.ExecuteXmlReader();
            sqlCommand.Parameters.Clear();
            return xmlReader2;
        }

       
        #endregion ExecuteXmlReader

        #region ExecuteXmlReaderTypedParams (DataRow)
        public static System.Xml.XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, string spName, System.Data.DataRow dataRow)
        {
            System.Xml.XmlReader xmlReader;

            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                xmlReader = ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                xmlReader = ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
            return xmlReader;
        }

        public static System.Xml.XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, string spName, System.Data.DataRow dataRow)
        {
            System.Xml.XmlReader xmlReader;

            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, dataRow);
                xmlReader = ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, sqlParameterArr);
            }
            else
            {
                xmlReader = ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
            return xmlReader;
        }
        #endregion ExecuteXmlReaderTypedParams (DataRow)

        #region FillDataset
        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        public static void FillDataset(string connectionString, string spName, System.Data.DataSet dataSet, string[] tableNames, object[] parameterValues)
        {
            System.Data.SqlClient.SqlConnection sqlConnection = null;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if (dataSet == null)
                throw new System.ArgumentNullException("dataSet");
            try
            {
                sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
                sqlConnection.Open();
                FillDataset(sqlConnection, spName, dataSet, tableNames, parameterValues);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
        }

        public static void FillDataset(string connectionString, CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames)
        {
            System.Data.SqlClient.SqlConnection sqlConnection = null;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if (dataSet == null)
                throw new System.ArgumentNullException("dataSet");
            try
            {
                sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
                sqlConnection.Open();
                FillDataset(sqlConnection, commandType, commandText, dataSet, tableNames);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
        }

        public static void FillDataset(string connectionString, CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            System.Data.SqlClient.SqlConnection sqlConnection = null;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if (dataSet == null)
                throw new System.ArgumentNullException("dataSet");
            try
            {
                sqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);
                sqlConnection.Open();
                FillDataset(sqlConnection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
        }

        public static void FillDataset(SqlConnection connection, string spName, System.Data.DataSet dataSet, string[] tableNames, object[] parameterValues)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if (dataSet == null)
                throw new System.ArgumentNullException("dataSet");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, sqlParameterArr);
            }
            else
            {
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        public static void FillDataset(SqlTransaction transaction, string spName, System.Data.DataSet dataSet, string[] tableNames, object[] parameterValues)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (dataSet == null)
                throw new System.ArgumentNullException("dataSet");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(sqlParameterArr, parameterValues);
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, sqlParameterArr);
            }
            else
            {
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        private static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if (dataSet == null)
                throw new System.ArgumentNullException("dataSet");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand();
            bool flag = false;
            PrepareCommand(sqlCommand, connection, transaction, commandType, commandText, commandParameters, ref flag);
            using (System.Data.SqlClient.SqlDataAdapter sqlDataAdapter = new System.Data.SqlClient.SqlDataAdapter(sqlCommand))
            {
                if ((tableNames != null) && (tableNames.Length > 0))
                {
                    string s = "Table";
                    int i3 = checked(tableNames.Length - 1);
                    for (int i1 = 0; i1 <= i3; i1 = checked(i1 + 1))
                    {
                        if ((tableNames[i1] == null) || (tableNames[i1].Length == 0))
                            throw new System.ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        sqlDataAdapter.TableMappings.Add(s, tableNames[i1]);
                        int i2 = checked(i1 + 1);
                        s += i2.ToString();
                    }
                }
                sqlDataAdapter.Fill(dataSet);
                sqlCommand.Parameters.Clear();
            }
            if (flag)
                connection.Close();
        }

        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new System.ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null))
                throw new System.ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, System.Data.DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }
        #endregion FillDataset

        #region UpdateDataset
        public static void UpdateDataset(System.Data.SqlClient.SqlCommand insertCommand, System.Data.SqlClient.SqlCommand deleteCommand, System.Data.SqlClient.SqlCommand updateCommand, System.Data.DataSet dataSet, string tableName)
        {
            if (insertCommand == null)
                throw new System.ArgumentNullException("insertCommand");
            if (deleteCommand == null)
                throw new System.ArgumentNullException("deleteCommand");
            if (updateCommand == null)
                throw new System.ArgumentNullException("updateCommand");
            if (dataSet == null)
                throw new System.ArgumentNullException("dataSet");
            if ((tableName == null) || (tableName.Length == 0))
                throw new System.ArgumentNullException("tableName");
            using (System.Data.SqlClient.SqlDataAdapter sqlDataAdapter = new System.Data.SqlClient.SqlDataAdapter())
            {
                sqlDataAdapter.UpdateCommand = updateCommand;
                sqlDataAdapter.InsertCommand = insertCommand;
                sqlDataAdapter.DeleteCommand = deleteCommand;
                sqlDataAdapter.Update(dataSet, tableName);
                dataSet.AcceptChanges();
            }
        }
        #endregion UpdateDataset

        #region CreateCommand
        public static System.Data.SqlClient.SqlCommand CreateCommand(SqlConnection connection, string spName, string[] sourceColumns)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            System.Data.SqlClient.SqlCommand sqlCommand1 = new System.Data.SqlClient.SqlCommand(spName, connection);
            sqlCommand1.CommandType = CommandType.StoredProcedure;
            if ((sourceColumns != null) && (sourceColumns.Length > 0))
            {
                SqlParameter[] sqlParameterArr = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                int i2 = checked(sourceColumns.Length - 1);
                for (int i1 = 0; i1 <= i2; i1 = checked(i1 + 1))
                {
                    sqlParameterArr[i1].SourceColumn = sourceColumns[i1];
                }
                AttachParameters(sqlCommand1, sqlParameterArr);
            }
            return sqlCommand1;
        }
        #endregion CreateCommand

       

    } 
}
