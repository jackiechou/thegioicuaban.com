using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;

namespace CommonLibrary.Providers.DataProviders
{
    public sealed class SqlHelperParameterCache
    {
        #region private methods, variables, and constructors
        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new SqlHelperParameterCache()"
        private SqlHelperParameterCache() { }

        private static System.Collections.Hashtable paramCache;

        static SqlHelperParameterCache()
        {
            SqlHelperParameterCache.paramCache = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        }
        #endregion private methods, variables, and constructors


        #region caching functions
        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters to be cached</param>
        public static void CacheParameterSet(string connectionString, string commandText, System.Data.SqlClient.SqlParameter[] commandParameters)
        {
            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((commandText == null) || (commandText.Length == 0))
                throw new System.ArgumentNullException("commandText");
            string s = connectionString + ":" + commandText;
            SqlHelperParameterCache.paramCache[s] = commandParameters;
        }

        /// <summary>
        /// Deep copy of cached SqlParameter array
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An array of SqlParamters</returns>
        public static System.Data.SqlClient.SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            System.Data.SqlClient.SqlParameter[] sqlParameterArr1;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            if ((commandText == null) || (commandText.Length == 0))
                throw new System.ArgumentNullException("commandText");
            string s = connectionString + ":" + commandText;
            sqlParameterArr1 = (System.Data.SqlClient.SqlParameter[])SqlHelperParameterCache.paramCache[s];
            if (sqlParameterArr1 == null)
            {
                return null;
            }
            else
            {
                return SqlHelperParameterCache.CloneParameters(sqlParameterArr1);
            }
        }
        #endregion caching functions

        #region Parameter Discovery Functions
        /// <summary>
        /// Resolve at run time the appropriate set of SqlParameters for a stored procedure
        /// </summary>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
        /// <returns>The parameter array discovered.</returns>
        private static System.Data.SqlClient.SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter, object[] parameterValues)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(spName, connection);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            connection.Open();
            System.Data.SqlClient.SqlCommandBuilder.DeriveParameters(sqlCommand);
            connection.Close();
            if (!includeReturnValueParameter)
                sqlCommand.Parameters.RemoveAt(0);
            System.Data.SqlClient.SqlParameter[] sqlParameterArr1 = new System.Data.SqlClient.SqlParameter[checked(checked(sqlCommand.Parameters.Count - 1) + 1)];
            sqlCommand.Parameters.CopyTo(sqlParameterArr1, 0);
            System.Data.SqlClient.SqlParameter[] sqlParameterArr3 = sqlParameterArr1;
            for (int i = 0; i < sqlParameterArr3.Length; i = checked(i + 1))
            {
                System.Data.SqlClient.SqlParameter sqlParameter = sqlParameterArr3[i];
                sqlParameter.Value = System.DBNull.Value;
            }
            return sqlParameterArr1;
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        public static System.Data.SqlClient.SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            return SqlHelperParameterCache.GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        public static System.Data.SqlClient.SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            SqlConnection sqlConnection =null;
            System.Data.SqlClient.SqlParameter[] sqlParameterArr;

            if (connection == null)
                throw new System.ArgumentNullException("connection");
            try
            {
                sqlConnection = (SqlConnection)((ICloneable)connection).Clone();
                sqlParameterArr = SqlHelperParameterCache.GetSpParameterSetInternal(sqlConnection, spName, includeReturnValueParameter);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
            return sqlParameterArr;
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        public static System.Data.SqlClient.SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return SqlHelperParameterCache.GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        public static System.Data.SqlClient.SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            SqlConnection sqlConnection = null;
            System.Data.SqlClient.SqlParameter[] sqlParameterArr;

            if ((connectionString == null) || (connectionString.Length == 0))
                throw new System.ArgumentNullException("connectionString");
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlParameterArr = SqlHelperParameterCache.GetSpParameterSetInternal(sqlConnection, spName, includeReturnValueParameter);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
            return sqlParameterArr;
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        private static System.Data.SqlClient.SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
                throw new System.ArgumentNullException("connection");
            if ((spName == null) || (spName.Length == 0))
                throw new System.ArgumentNullException("spName");


            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            SqlParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                SqlParameter[] spParameters = SqlHelperParameterCache.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter, new object[0]);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }
            return CloneParameters(cachedParameters);
        }
        #endregion Parameter Discovery Functions
    }
}