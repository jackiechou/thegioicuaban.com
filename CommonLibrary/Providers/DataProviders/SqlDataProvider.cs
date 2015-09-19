using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.ComponentModel;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Web;
using System.IO;

namespace CommonLibrary.Providers.DataProviders
{
    public class SqlDataProvider 
    {
        private string _connectionString;
        private string _coreConnectionString;
        private string _databaseOwner;
        private string _objectQualifier;
        private string _providerName;
        private string _providerPath;
        private string _scriptDelimiterRegex = "(?<=(?:[^\\w]+|^))GO(?=(?: |\\t)*?(?:\\r?\\n|$))";
        private string _upgradeConnectionString;
        private bool _isConnectionValid;
        public SqlDataProvider()
            : this(true)
        {
        }
        public SqlDataProvider(bool useConfig)
        {
            _providerName = Settings["providerName"];
            _providerPath = Settings["providerPath"];
            if (useConfig)
            {
                _connectionString = Config.GetConnectionString();
            }
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = Settings["connectionString"];
            }
            _objectQualifier = Settings["objectQualifier"];
            if (!string.IsNullOrEmpty(_objectQualifier) && _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }
            _databaseOwner = Settings["databaseOwner"];
            if (!string.IsNullOrEmpty(_databaseOwner) && _databaseOwner.EndsWith(".") == false)
            {
                _databaseOwner += ".";
            }
            _coreConnectionString = _connectionString;
            if (!_coreConnectionString.EndsWith(";"))
            {
                _coreConnectionString += ";";
            }
            _coreConnectionString += "Application Name=DNNCore;";
            if (!String.IsNullOrEmpty(Settings["upgradeConnectionString"]))
            {
                _upgradeConnectionString = Settings["upgradeConnectionString"];
            }
            else
            {
                _upgradeConnectionString = _coreConnectionString;
            }
            _isConnectionValid = CanConnect(ConnectionString, DatabaseOwner, ObjectQualifier);
        }

        public string ConnectionString
        {
            get { return _coreConnectionString; }
        }
        public string DatabaseOwner
        {
            get { return _databaseOwner; }
        }
        public string ObjectQualifier
        {
            get { return _objectQualifier; }
        }
        public string ProviderName
        {
            get { return _providerName; }
        }
        public Dictionary<string, string> Settings
        {
            get { return ComponentFactory.GetComponentSettings<SqlDataProvider>() as Dictionary<string, string>; }
        }
        public string ProviderPath
        {
            get { return _providerPath; }
        }
        public string UpgradeConnectionString
        {
            get { return _upgradeConnectionString; }
        }
        public bool IsConnectionValid
        {
            get
            {
                return _isConnectionValid;
            }
        }

        protected Regex SqlDelimiterRegex
        {
            get
            {
                Regex objRegex = (Regex)DataCache.GetCache("SQLDelimiterRegex");
                if (objRegex == null)
                {
                    objRegex = new Regex(_scriptDelimiterRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    DataCache.SetCache("SQLDelimiterRegex", objRegex);
                }
                return objRegex;
            }
        }
        private void ExecuteADOScript(SqlTransaction trans, string SQL)
        {
            SqlConnection connection = trans.Connection;
            SqlCommand command = new SqlCommand(SQL, trans.Connection);
            command.Transaction = trans;
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();
        }
        private void ExecuteADOScript(string SQL)
        {
            SqlConnection connection = new SqlConnection(UpgradeConnectionString);
            SqlCommand command = new SqlCommand(SQL, connection);
            command.CommandTimeout = 0;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        private object GetRoleNull(int RoleID)
        {
            if (RoleID.ToString() == Common.Globals.glbRoleNothing)
            {
                return DBNull.Value;
            }
            else
            {
                return RoleID;
            }
        }
        private string GetConnectionStringUserID()
        {
            string DBUser = "public";
            string[] ConnSettings;
            string[] ConnSetting;
            ConnSettings = ConnectionString.Split(';');
            if (ConnectionString.ToUpper().Contains("USER ID") || ConnectionString.ToUpper().Contains("UID") || ConnectionString.ToUpper().Contains("USER"))
            {
                ConnSettings = ConnectionString.Split(';');
                foreach (string s in ConnSettings)
                {
                    if (s != string.Empty)
                    {
                        ConnSetting = s.Split('=');
                        if ("USER ID|UID|USER".Contains(ConnSetting[0].Trim().ToUpper()))
                        {
                            DBUser = ConnSetting[1].Trim();
                        }
                    }
                }
            }
            return DBUser;
        }
        private string GrantStoredProceduresPermission(string Permission, string LoginOrRole)
        {
            string SQL = string.Empty;
            string Exceptions = string.Empty;

            try
            {
                SQL += "if exists (select * from dbo.sysusers where name='" + LoginOrRole + "')";
                SQL += "  begin";
                SQL += "    declare @exec nvarchar(2000) ";
                SQL += "    declare @name varchar(150) ";
                SQL += "    declare sp_cursor cursor for select o.name as name ";
                SQL += "    from dbo.sysobjects o ";
                SQL += "    where ( OBJECTPROPERTY(o.id, N'IsProcedure') = 1 or OBJECTPROPERTY(o.id, N'IsExtendedProc') = 1 or OBJECTPROPERTY(o.id, N'IsReplProc') = 1 ) ";
                SQL += "    and OBJECTPROPERTY(o.id, N'IsMSShipped') = 0 ";
                SQL += "    and o.name not like N'#%%' ";
                SQL += "    and (left(o.name,len('" + ObjectQualifier + "')) = '" + ObjectQualifier + "' or left(o.name,7) = 'aspnet_') ";
                SQL += "    open sp_cursor ";
                SQL += "    fetch sp_cursor into @name ";
                SQL += "    while @@fetch_status >= 0 ";
                SQL += "      begin";
                SQL += "        select @exec = 'grant " + Permission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
                SQL += "        execute (@exec)";
                SQL += "        fetch sp_cursor into @name ";
                SQL += "      end ";
                SQL += "    deallocate sp_cursor";
                SQL += "  end ";
                SqlHelper.ExecuteNonQuery(UpgradeConnectionString, CommandType.Text, SQL);
            }
            catch (SqlException objException)
            {
                Exceptions += objException.ToString() + Environment.NewLine + Environment.NewLine + SQL + Environment.NewLine + Environment.NewLine;
            }
            return Exceptions;
        }
        private string GrantUserDefinedFunctionsPermission(string ScalarPermission, string TablePermission, string LoginOrRole)
        {
            string SQL = string.Empty;
            string Exceptions = string.Empty;
            try
            {
                SQL += "if exists (select * from dbo.sysusers where name='" + LoginOrRole + "')";
                SQL += "  begin";
                SQL += "    declare @exec nvarchar(2000) ";
                SQL += "    declare @name varchar(150) ";
                SQL += "    declare @isscalarfunction int ";
                SQL += "    declare @istablefunction int ";
                SQL += "    declare sp_cursor cursor for select o.name as name, OBJECTPROPERTY(o.id, N'IsScalarFunction') as IsScalarFunction ";
                SQL += "    from dbo.sysobjects o ";
                SQL += "    where ( OBJECTPROPERTY(o.id, N'IsScalarFunction') = 1 OR OBJECTPROPERTY(o.id, N'IsTableFunction') = 1 ) ";
                SQL += "      and OBJECTPROPERTY(o.id, N'IsMSShipped') = 0 ";
                SQL += "      and o.name not like N'#%%' ";
                SQL += "      and (left(o.name,len('" + ObjectQualifier + "')) = '" + ObjectQualifier + "' or left(o.name,7) = 'aspnet_') ";
                SQL += "    open sp_cursor ";
                SQL += "    fetch sp_cursor into @name, @isscalarfunction ";
                SQL += "    while @@fetch_status >= 0 ";
                SQL += "      begin ";
                SQL += "        if @IsScalarFunction = 1 ";
                SQL += "          begin";
                SQL += "            select @exec = 'grant " + ScalarPermission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
                SQL += "            execute (@exec)";
                SQL += "            fetch sp_cursor into @name, @isscalarfunction  ";
                SQL += "          end ";
                SQL += "        else ";
                SQL += "          begin";
                SQL += "            select @exec = 'grant " + TablePermission + " on [' +  @name  + '] to [" + LoginOrRole + "]'";
                SQL += "            execute (@exec)";
                SQL += "            fetch sp_cursor into @name, @isscalarfunction  ";
                SQL += "          end ";
                SQL += "      end ";
                SQL += "    deallocate sp_cursor";
                SQL += "  end ";
                SqlHelper.ExecuteNonQuery(UpgradeConnectionString, CommandType.Text, SQL);
            }
            catch (SqlException objException)
            {
                Exceptions += objException.ToString() + Environment.NewLine + Environment.NewLine + SQL + Environment.NewLine + Environment.NewLine;
            }
            return Exceptions;
        }
        //Private Overloads Function TestDatabaseConnection(ByVal ConnectionString As String, ByVal Owner As String, ByVal Qualifier As String) As Boolean
        // Dim result As Boolean
        // Try
        // SqlHelper.ExecuteReader(ConnectionString, Owner & Qualifier & "GetDatabaseVersion")
        // result = True
        // Catch ex As SqlException
        // End Try
        // Return result
        //End Function
        private bool CanConnect(string ConnectionString, string Owner, string Qualifier)
        {

            bool connectionValid = true;

            try
            {
                SqlHelper.ExecuteReader(ConnectionString, Owner + Qualifier + "GetDatabaseVersion",null);
            }
            catch (SqlException ex)
            {

                foreach (SqlError c in ex.Errors)
                {
                    if (!(c.Number == 2812 & c.Class == 16))
                    {
                        connectionValid = false;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            return connectionValid;
        }
        public void ExecuteNonQuery(string ProcedureName, params object[] commandParameters)
        {
            SqlHelper.ExecuteNonQuery(_connectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters);
        }
        public IDataReader ExecuteReader(string ProcedureName, params object[] commandParameters)
        {
            return SqlHelper.ExecuteReader(_connectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters);
        }
        public object ExecuteScalar(string ProcedureName, params object[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(_connectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters);
        }
        public T ExecuteScalar<T>(string ProcedureName, params object[] commandParameters)
        {
            object retObject = ExecuteScalar(ProcedureName, commandParameters);
            T retValue = default(T);
            if (retObject != null)
            {
                retValue = (T)Convert.ChangeType(retObject, typeof(T));
            }
            return retValue;
        }
        public DataSet ExecuteDataSet(string ProcedureName, params object[] commandParameters)
        {
            return SqlHelper.ExecuteDataset(_connectionString, DatabaseOwner + ObjectQualifier + ProcedureName, commandParameters);
        }
        public IDataReader ExecuteSQL(string SQL)
        {
            return ExecuteSQL(_connectionString, SQL, (IDataParameter)null);
        }

        public IDataReader ExecuteSQL(string SQL, params IDataParameter[] commandParameters)
        {
            return ExecuteSQL(_coreConnectionString, SQL, commandParameters);
        }

        public IDataReader ExecuteSQL(string ConnectionString, string SQL, params IDataParameter[] commandParameters)
        {
            SqlParameter[] sqlCommandParameters = null;
            if (commandParameters != null)
            {
                sqlCommandParameters = new SqlParameter[commandParameters.Length];
                for (int intIndex = 0; intIndex <= commandParameters.Length - 1; intIndex++)
                {
                    sqlCommandParameters[intIndex] = (SqlParameter)commandParameters[intIndex];
                }
            }
            SQL = SQL.Replace("{databaseOwner}", DatabaseOwner);
            SQL = SQL.Replace("{objectQualifier}", ObjectQualifier);
            try
            {
                return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, SQL, sqlCommandParameters);
            }
            catch
            {
                return null;
            }
        }
        public DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new SqlConnectionStringBuilder();
        }
        public object GetNull(object Field)
        {
            return Null.GetNull(Field, DBNull.Value);
        }
        public void CommitTransaction(DbTransaction transaction)
        {
            try
            {
                transaction.Commit();
            }
            finally
            {
                if (transaction != null && transaction.Connection != null)
                {
                    transaction.Connection.Close();
                }
            }
        }
        public string ExecuteScript(string Script, DbTransaction transaction)
        {
            string Exceptions = "";
            string[] arrSQL = SqlDelimiterRegex.Split(Script);
            bool IgnoreErrors;
            foreach (string SQL in arrSQL)
            {
                string s = SQL;
                if (!String.IsNullOrEmpty(s.Trim()))
                {
                    s = s.Replace("{databaseOwner}", DatabaseOwner);
                    s = s.Replace("{objectQualifier}", ObjectQualifier);
                    IgnoreErrors = false;
                    if (s.Trim().StartsWith("{IgnoreError}"))
                    {
                        IgnoreErrors = true;
                        s = s.Replace("{IgnoreError}", "");
                    }
                    try
                    {
                        ExecuteADOScript((SqlTransaction)transaction, s);
                    }
                    catch (SqlException objException)
                    {
                        if (!IgnoreErrors)
                        {
                            Exceptions += objException.ToString() + Environment.NewLine + Environment.NewLine + s + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }
            }
            return Exceptions;
        }
        public DbTransaction GetTransaction()
        {
            SqlConnection Conn = new SqlConnection(UpgradeConnectionString);
            Conn.Open();
            SqlTransaction transaction = Conn.BeginTransaction();
            return transaction;
        }
        public void RollbackTransaction(DbTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            finally
            {
                if (transaction != null && transaction.Connection != null)
                {
                    transaction.Connection.Close();
                }
            }
        }
        public string ExecuteScript(string Script)
        {
            return ExecuteScript(Script, false);
        }
        /// <summary>
        /// This is a temporary overload until proper support for named instances is added to the core.
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="Script"></param>
        /// <returns></returns>
        /// <remarks></remarks>


        public string ExecuteScript(string Script, bool UseTransactions)
        {
            string Exceptions = "";
            if (UseTransactions)
            {
                DbTransaction transaction = GetTransaction();
                try
                {
                    Exceptions += ExecuteScript(Script, transaction);
                    if (String.IsNullOrEmpty(Exceptions))
                    {
                        CommitTransaction(transaction);
                    }
                    else
                    {
                        RollbackTransaction(transaction);
                        Exceptions += "SQL Execution failed.  Database was rolled back" + Environment.NewLine + Environment.NewLine + Script + Environment.NewLine + Environment.NewLine;
                    }
                }
                finally
                {
                    if (transaction != null && transaction.Connection != null)
                    {
                        transaction.Connection.Close();
                    }
                }
            }
            else
            {
                string[] arrSQL = SqlDelimiterRegex.Split(Script);
                foreach (string SQL in arrSQL)
                {
                    string s = SQL;
                    if (!String.IsNullOrEmpty(s.Trim()))
                    {
                        s = s.Replace("{databaseOwner}", DatabaseOwner);
                        s = s.Replace("{objectQualifier}", ObjectQualifier);
                        try
                        {
                            ExecuteADOScript(s);
                        }
                        catch (SqlException objException)
                        {
                            Exceptions += objException.ToString() + Environment.NewLine + Environment.NewLine + s + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }
            }
            if (UpgradeConnectionString != ConnectionString || DatabaseOwner.Trim().ToLower() != "dbo.")
            {
                try
                {
                    Exceptions += GrantStoredProceduresPermission("EXECUTE", GetConnectionStringUserID());
                }
                catch (SqlException objException)
                {
                    Exceptions += objException.ToString() + Environment.NewLine + Environment.NewLine + Script + Environment.NewLine + Environment.NewLine;
                }
                try
                {
                    Exceptions += GrantUserDefinedFunctionsPermission("EXECUTE", "SELECT", GetConnectionStringUserID());
                }
                catch (SqlException objException)
                {
                    Exceptions += objException.ToString() + Environment.NewLine + Environment.NewLine + Script + Environment.NewLine + Environment.NewLine;
                }
            }
            return Exceptions;
        }
        public IDataReader GetDatabaseServer()
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDatabaseServer",null);
        }
        public System.Version GetDatabaseEngineVersion()
        {
            string version = "0.0";
            IDataReader dr = null;
            try
            {
                dr = GetDatabaseServer();
                if (dr.Read())
                {
                    version = dr["Version"].ToString();
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return new System.Version(version);
        }
        //public IDataReader FindDatabaseVersion(int Major, int Minor, int Build)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "FindDatabaseVersion", Major, Minor, Build);
        //}
        //public IDataReader GetDatabaseVersion()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDatabaseVersion");
        ////}
        //public System.Version GetVersion()
        //{
        //    System.Version version = null;
        //    try
        //    {
        //        IDataReader dr = GetDatabaseVersion();
        //        if (dr.Read())
        //        {
        //            version = new System.Version(Convert.ToInt32(dr["Major"]), Convert.ToInt32(dr["Minor"]), Convert.ToInt32(dr["Build"]));
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        for (int i = 0; i <= ex.Errors.Count - 1; i++)
        //        {
        //            SqlError sqlError = ex.Errors[i];
        //            if (sqlError.Number == 2812 && sqlError.Class == 16)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //    return version;
        //}
        public string GetProviderPath()
        {
            HttpContext objHttpContext = HttpContext.Current;
            string path = ProviderPath;
            if (!String.IsNullOrEmpty(path))
            {
                path = objHttpContext.Server.MapPath(path);
                if (Directory.Exists(path))
                {
                    if (!IsConnectionValid)
                    {
                        path = "ERROR: Could not connect to database specified in connectionString for SqlDataProvider";
                    }
                }
                else
                {
                    path = "ERROR: providerPath folder " + path + " specified for SqlDataProvider does not exist on web server";
                }
            }
            else
            {
                path = "ERROR: providerPath folder value not specified in web.config for SqlDataProvider";
            }
            return path;
        }
        //public string TestDatabaseConnection(DbConnectionStringBuilder builder, string Owner, string Qualifier)
        //{
        //    SqlConnectionStringBuilder sqlBuilder = builder as SqlConnectionStringBuilder;
        //    string connectionString = Null.NullString;
        //    if (sqlBuilder != null)
        //    {
        //        connectionString = sqlBuilder.ToString();
        //        IDataReader dr = null;
        //        try
        //        {
        //            dr = SqlHelper.ExecuteReader(connectionString, Owner + Qualifier + "GetDatabaseVersion");
        //        }
        //        catch (SqlException ex)
        //        {
        //            string message = "ERROR:";
        //            bool bError = true;
        //            int i;
        //            StringBuilder errorMessages = new StringBuilder();
        //            for (i = 0; i <= ex.Errors.Count - 1; i++)
        //            {
        //                SqlError sqlError = ex.Errors[i];
        //                if (sqlError.Number == 2812 && sqlError.Class == 16)
        //                {
        //                    bError = false;
        //                    break;
        //                }
        //                else
        //                {
        //                    string filteredMessage = String.Empty;
        //                    switch (sqlError.Number)
        //                    {
        //                        case 17:
        //                            filteredMessage = "Sql server does not exist or access denied";
        //                            break;
        //                        case 4060:
        //                            filteredMessage = "Invalid Database";
        //                            break;
        //                        case 18456:
        //                            filteredMessage = "Sql login failed";
        //                            break;
        //                        case 1205:
        //                            filteredMessage = "Sql deadlock victim";
        //                            break;
        //                    }
        //                    errorMessages.Append("<b>Index #:</b> " + i.ToString() + "<br/>" + "<b>Source:</b> " + sqlError.Source + "<br/>" + "<b>Class:</b> " + sqlError.Class + "<br/>" + "<b>Number:</b> " + sqlError.Number + "<br/>" + "<b>Message:</b> " + filteredMessage + "<br/><br/>");
        //                }
        //            }
        //            if (bError)
        //            {
        //                connectionString = message + errorMessages.ToString();
        //            }
        //        }
        //        finally
        //        {
        //            CBO.CloseDataReader(dr, true);
        //        }
        //    }
        //    else
        //    {
        //    }
        //    return connectionString;
        //}
        public void UpgradeDatabaseSchema(int Major, int Minor, int Build)
        {
        }
        //public void UpdateDatabaseVersion(int Major, int Minor, int Build, string Name)
        //{
        //    if ((Major >= 5 || (Major == 4 && Minor == 9 && Build > 0)))
        //    {
        //        SqlHelper.ExecuteNonQuery(UpgradeConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDatabaseVersionAndName", Major, Minor, Build, Name);
        //    }
        //    else
        //    {
        //        SqlHelper.ExecuteNonQuery(UpgradeConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDatabaseVersion", Major, Minor, Build);
        //    }
        //}
        //public IDataReader GetHostSettings()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetHostSettings");
        //}
        //public IDataReader GetHostSetting(string SettingName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetHostSetting", SettingName);
        //}
        //public void AddHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, int CreatedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddHostSetting", SettingName, SettingValue, SettingIsSecure, CreatedByUserID);
        //}
        //public void UpdateHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateHostSetting", SettingName, SettingValue, SettingIsSecure, LastModifiedByUserID);
        //}
        //public IDataReader GetServers()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetServers");
        //}
        //public IDataReader GetServerConfiguration()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetServerConfiguration");
        //}
        //public void UpdateServer(int ServerId, string Url, bool Enabled)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateServer", ServerId, Url, Enabled);
        //}
        //public void DeleteServer(int ServerId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteServer", ServerId);
        //}
        //public void UpdateServerActivity(string ServerName, string IISAppName, DateTime CreatedDate, DateTime LastActivityDate)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateServerActivity", ServerName, IISAppName, CreatedDate, LastActivityDate);
        //}
        //public int AddPortalInfo(string portalname, string currency, string firstname, string lastname, string username, string password, string email, System.DateTime expirydate, double hostfee, double hostspace,
        //int pagequota, int userquota, int siteloghistory, string homedirectory, int createdbyuserid)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "addportalinfo", portalname, currency, GetNull(expirydate), hostfee, hostspace, pagequota, userquota, GetNull(siteloghistory),
        //    homedirectory, createdbyuserid));
        //}
        //public int CreatePortal(string PortalName, string Currency, System.DateTime ExpiryDate, double HostFee, double HostSpace, int PageQuota, int UserQuota, int SiteLogHistory, string HomeDirectory, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalInfo", PortalName, Currency, GetNull(ExpiryDate), HostFee, HostSpace, PageQuota, UserQuota, GetNull(SiteLogHistory),
        //    HomeDirectory, CreatedByUserID));
        //}
        //public void DeletePortalInfo(int PortalId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalInfo", PortalId);
        //}
        //public void DeletePortalSetting(int PortalId, string SettingName, string CultureCode)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalSetting", PortalId, SettingName, CultureCode);
        //}
        //public void DeletePortalSettings(int PortalId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalSettings", PortalId);
        //}
        //public IDataReader GetExpiredPortals()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetExpiredPortals");
        //}
        //public IDataReader GetPortal(int PortalId, string CultureCode)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortal", PortalId, CultureCode);
        //}
        //public IDataReader GetPortalByAlias(string PortalAlias)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByAlias", PortalAlias);
        //}
        //public IDataReader GetPortalByTab(int TabId, string PortalAlias)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByTab", TabId, PortalAlias);
        //}
        //public int GetPortalCount()
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalCount"));
        //}
        //public IDataReader GetPortals(string CultureCode)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortals", CultureCode);
        //}
        //public IDataReader GetPortals()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortals");
        //}
        //public IDataReader GetPortalsByName(string nameToMatch, int pageIndex, int pageSize)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalsByName", nameToMatch, pageIndex, pageSize);
        //}
        //public IDataReader GetPortalSettings(int PortalId, string CultureCode)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalSettings", PortalId, CultureCode);
        //}
        //public IDataReader GetPortalSpaceUsed(int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalSpaceUsed", GetNull(PortalId));
        //}
        //public void UpdatePortalInfo(int PortalId, string PortalName, string LogoFile, string FooterText, System.DateTime ExpiryDate, int UserRegistration, int BannerAdvertising, string Currency, int AdministratorId, double HostFee,
        //double HostSpace, int PageQuota, int UserQuota, string PaymentProcessor, string ProcessorUserId, string ProcessorPassword, string Description, string KeyWords, string BackgroundFile, int SiteLogHistory,
        //int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId, int UserTabId, string DefaultLanguage, int TimeZoneOffset, string HomeDirectory, int lastModifiedByUserID, string CultureCode)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalInfo", PortalId, PortalName, GetNull(LogoFile), GetNull(FooterText), GetNull(ExpiryDate), UserRegistration, BannerAdvertising, Currency,
        //    GetNull(AdministratorId), HostFee, HostSpace, PageQuota, UserQuota, GetNull(PaymentProcessor), GetNull(ProcessorUserId), GetNull(ProcessorPassword), GetNull(Description), GetNull(KeyWords),
        //    GetNull(BackgroundFile), GetNull(SiteLogHistory), GetNull(SplashTabId), GetNull(HomeTabId), GetNull(LoginTabId), GetNull(RegisterTabId), GetNull(UserTabId), GetNull(DefaultLanguage), GetNull(TimeZoneOffset), HomeDirectory, lastModifiedByUserID,
        //    CultureCode);
        //}
        //public void UpdatePortalSetting(int PortalId, string SettingName, string SettingValue, int UserID, string CultureCode)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalSetting", PortalId, SettingName, SettingValue, UserID, CultureCode);
        //}
        //public void UpdatePortalSetup(int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId, int UserTabId, int AdminTabId, string CultureCode)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalSetup", PortalId, AdministratorId, AdministratorRoleId, RegisteredRoleId, SplashTabId, HomeTabId, LoginTabId, RegisterTabId, UserTabId,
        //    AdminTabId, CultureCode);
        //}
        //public IDataReader VerifyPortal(int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "VerifyPortal", PortalId);
        //}
        //public IDataReader VerifyPortalTab(int PortalId, int TabId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "VerifyPortalTab", PortalId, TabId);
        //}
        //public int AddTab(int ContentItemId, int PortalId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string IconFileLarge, string Title, string Description, string KeyWords,
        //string Url, string SkinSrc, string ContainerSrc, string TabPath, System.DateTime StartDate, System.DateTime EndDate, int RefreshInterval, string PageHeadText, bool IsSecure, bool PermanentRedirect,
        //float SiteMapPriority, int CreatedByUserID, string CultureCode)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddTab", ContentItemId, GetNull(PortalId), TabName, IsVisible, DisableLink, GetNull(ParentId), IconFile, IconFileLarge, Title,
        //    Description, KeyWords, Url, GetNull(SkinSrc), GetNull(ContainerSrc), TabPath, GetNull(StartDate), GetNull(EndDate), GetNull(RefreshInterval), GetNull(PageHeadText),
        //    IsSecure, PermanentRedirect, SiteMapPriority, CreatedByUserID, CultureCode));
        //}

        //public void UpdateTab(int TabId, int ContentItemId, int PortalId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string IconFileLarge, string Title, string Description,
        //string KeyWords, bool IsDeleted, string Url, string SkinSrc, string ContainerSrc, string TabPath, System.DateTime StartDate, System.DateTime EndDate, int RefreshInterval, string PageHeadText,
        //bool IsSecure, bool PermanentRedirect, float SiteMapPriority, int LastModifiedByUserID, string CultureCode)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTab", TabId, ContentItemId, GetNull(PortalId), TabName, IsVisible, DisableLink, GetNull(ParentId), IconFile, IconFileLarge,
        //    Title, Description, KeyWords, IsDeleted, Url, GetNull(SkinSrc), GetNull(ContainerSrc), TabPath, GetNull(StartDate), GetNull(EndDate),
        //    GetNull(RefreshInterval), GetNull(PageHeadText), IsSecure, PermanentRedirect, SiteMapPriority, LastModifiedByUserID, CultureCode);
        //}
        //public void UpdateTabOrder(int TabId, int TabOrder, int Level, int ParentId, string TabPath, int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabOrder", TabId, TabOrder, Level, GetNull(ParentId), TabPath, LastModifiedByUserID);
        //}
        //public void DeleteTab(int TabId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTab", TabId);
        //}
        //public IDataReader GetTabs(int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabs", GetNull(PortalId));
        //}
        //public IDataReader GetAllTabs()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllTabs");
        //}
        //public IDataReader GetTabPaths(int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPaths", GetNull(PortalId));
        //}
        //public IDataReader GetTab(int TabId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTab", TabId);
        //}
        //public IDataReader GetTabByName(string TabName, int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabByName", TabName, GetNull(PortalId));
        //}
        //public int GetTabCount(int PortalId)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabCount", PortalId));
        //}
        //public IDataReader GetTabsByParentId(int ParentId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabsByParentId", ParentId);
        //}
        //public IDataReader GetTabsByModuleID(int moduleID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabsByModuleID", moduleID);
        //}
        //public IDataReader GetTabsByPackageID(int portalID, int packageID, bool forHost)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabsByPackageID", GetNull(portalID), packageID, forHost);
        //}
        //public IDataReader GetTabPanes(int TabId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPanes", TabId);
        //}
        //public IDataReader GetPortalTabModules(int PortalId, int TabId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModules", TabId);
        //}
        //public IDataReader GetTabModules(int TabId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModules", TabId);
        //}
        //public IDataReader GetAllModules()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllModules");
        //}
        //public IDataReader GetModules(int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModules", PortalId);
        //}
        //public IDataReader GetAllTabsModules(int PortalId, bool AllTabs)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllTabsModules", PortalId, AllTabs);
        //}
        //public IDataReader GetModule(int ModuleId, int TabId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModule", ModuleId, GetNull(TabId));
        //}
        //public IDataReader GetModuleByDefinition(int PortalId, string FriendlyName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleByDefinition", GetNull(PortalId), FriendlyName);
        //}
        //public int AddModule(int ContentItemId, int PortalID, int ModuleDefID, string ModuleTitle, bool AllTabs, string Header, string Footer, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted,
        //int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddModule", ContentItemId, GetNull(PortalID), ModuleDefID, ModuleTitle, AllTabs, GetNull(Header), GetNull(Footer), GetNull(StartDate), GetNull(EndDate),
        //    InheritViewPermissions, IsDeleted, createdByUserID));
        //}
        //public void UpdateModule(int ModuleId, int ContentItemId, string ModuleTitle, bool AllTabs, string Header, string Footer, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModule", ModuleId, ContentItemId, ModuleTitle, AllTabs, GetNull(Header), GetNull(Footer), GetNull(StartDate), GetNull(EndDate), InheritViewPermissions,
        //    IsDeleted, lastModifiedByUserID);
        //}
        //public void DeleteModule(int ModuleId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModule", ModuleId);
        //}
        //public IDataReader GetTabModuleOrder(int TabId, string PaneName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleOrder", TabId, PaneName);
        //}
        //public void UpdateModuleOrder(int TabId, int ModuleId, int ModuleOrder, string PaneName)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleOrder", TabId, ModuleId, ModuleOrder, PaneName);
        //}
        //public void AddTabModule(int TabId, int ModuleId, int ModuleOrder, string PaneName, int CacheTime, string CacheMethod, string Alignment, string Color, string Border, string IconFile,
        //int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint, bool DisplaySyndicate, bool IsWebSlice, string WebSliceTitle, DateTime WebSliceExpiryDate, int WebSliceTTL, int createdByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabModule", TabId, ModuleId, ModuleOrder, PaneName, CacheTime, GetNull(CacheMethod), GetNull(Alignment), GetNull(Color),
        //    GetNull(Border), GetNull(IconFile), Visibility, GetNull(ContainerSrc), DisplayTitle, DisplayPrint, DisplaySyndicate, IsWebSlice, WebSliceTitle, GetNull(WebSliceExpiryDate),
        //    WebSliceTTL, createdByUserID);
        //}
        //public void DeleteTabModule(int TabId, int ModuleId, bool softDelete)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModule", TabId, ModuleId, softDelete);
        //}
        //public void MoveTabModule(int fromTabId, int moduleId, int toTabId, string toPaneName, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "MoveTabModule", fromTabId, moduleId, toTabId, toPaneName, lastModifiedByUserID);
        //}
        //public void RestoreTabModule(int TabId, int ModuleId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "RestoreTabModule", TabId, ModuleId);
        //}
        //public void UpdateTabModule(int TabId, int ModuleId, int ModuleOrder, string PaneName, int CacheTime, string CacheMethod, string Alignment, string Color, string Border, string IconFile,
        //int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint, bool DisplaySyndicate, bool IsWebSlice, string WebSliceTitle, DateTime WebSliceExpiryDate, int WebSliceTTL, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabModule", TabId, ModuleId, ModuleOrder, PaneName, CacheTime, GetNull(CacheMethod), GetNull(Alignment), GetNull(Color),
        //    GetNull(Border), GetNull(IconFile), Visibility, GetNull(ContainerSrc), DisplayTitle, DisplayPrint, DisplaySyndicate, IsWebSlice, WebSliceTitle, GetNull(WebSliceExpiryDate),
        //    WebSliceTTL, lastModifiedByUserID);
        //}
        //public IDataReader GetSearchModules(int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchModules", PortalId);
        //}
        //public IDataReader GetModuleSettings(int ModuleId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleSettings", ModuleId);
        //}
        //public IDataReader GetModuleSetting(int ModuleId, string SettingName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleSetting", ModuleId, SettingName);
        //}
        //public void AddModuleSetting(int ModuleId, string SettingName, string SettingValue, int createdByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddModuleSetting", ModuleId, SettingName, SettingValue, createdByUserID);
        //}
        //public void UpdateModuleSetting(int ModuleId, string SettingName, string SettingValue, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleSetting", ModuleId, SettingName, SettingValue, lastModifiedByUserID);
        //}
        //public void DeleteModuleSetting(int ModuleId, string SettingName)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleSetting", ModuleId, SettingName);
        //}
        //public void DeleteModuleSettings(int ModuleId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleSettings", ModuleId);
        //}
        //public IDataReader GetTabSettings(int TabID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabSettings", TabID);
        //}
        //public IDataReader GetTabSetting(int TabID, string SettingName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabSetting", TabID, SettingName);
        //}
        //public void UpdateTabSetting(int TabId, string SettingName, string SettingValue, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabSetting", TabId, SettingName, SettingValue, lastModifiedByUserID);
        //}
        //public void AddTabSetting(int TabId, string SettingName, string SettingValue, int createdByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabSetting", TabId, SettingName, SettingValue, createdByUserID);
        //}
        //public void DeleteTabSetting(int TabId, string SettingName)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabSetting", TabId, SettingName);
        //}
        //public void DeleteTabSettings(int TabId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabSettings", TabId);
        //}
        //public IDataReader GetTabModuleSettings(int TabModuleId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleSettings", TabModuleId);
        //}
        //public IDataReader GetTabModuleSetting(int TabModuleId, string SettingName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabModuleSetting", TabModuleId, SettingName);
        //}
        //public void AddTabModuleSetting(int TabModuleId, string SettingName, string SettingValue, int createdByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabModuleSetting", TabModuleId, SettingName, SettingValue, createdByUserID);
        //}
        //public void UpdateTabModuleSetting(int TabModuleId, string SettingName, string SettingValue, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabModuleSetting", TabModuleId, SettingName, SettingValue, lastModifiedByUserID);
        //}
        //public void DeleteTabModuleSetting(int TabModuleId, string SettingName)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModuleSetting", TabModuleId, SettingName);
        //}
        //public void DeleteTabModuleSettings(int TabModuleId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabModuleSettings", TabModuleId);
        //}
        //public IDataReader GetDesktopModule(int DesktopModuleId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModule", DesktopModuleId);
        //}
        //public IDataReader GetDesktopModuleByFriendlyName(string FriendlyName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModuleByFriendlyName", FriendlyName);
        //}
        //public IDataReader GetDesktopModuleByModuleName(string ModuleName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModuleByModuleName", ModuleName);
        //}
        //public IDataReader GetDesktopModuleByPackageID(int packageID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModuleByPackageID", packageID);
        //}
        //public IDataReader GetDesktopModules()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModules");
        //}
        //public IDataReader GetDesktopModulesByPortal(int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulesByPortal", PortalId);
        //}
        //public int AddDesktopModule(int packageID, string ModuleName, string FolderName, string FriendlyName, string Description, string Version, bool IsPremium, bool IsAdmin, string BusinessControllerClass, int SupportedFeatures,
        //string CompatibleVersions, string Dependencies, string Permissions, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddDesktopModule", packageID, ModuleName, FolderName, FriendlyName, GetNull(Description), GetNull(Version), IsPremium, IsAdmin,
        //    BusinessControllerClass, SupportedFeatures, GetNull(CompatibleVersions), GetNull(Dependencies), GetNull(Permissions), createdByUserID));
        //}
        //public void UpdateDesktopModule(int DesktopModuleId, int packageID, string ModuleName, string FolderName, string FriendlyName, string Description, string Version, bool IsPremium, bool IsAdmin, string BusinessControllerClass,
        //int SupportedFeatures, string CompatibleVersions, string Dependencies, string Permissions, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDesktopModule", DesktopModuleId, packageID, ModuleName, FolderName, FriendlyName, GetNull(Description), GetNull(Version), IsPremium,
        //    IsAdmin, BusinessControllerClass, SupportedFeatures, GetNull(CompatibleVersions), GetNull(Dependencies), GetNull(Permissions), lastModifiedByUserID);
        //}
        //public void DeleteDesktopModule(int DesktopModuleId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModule", DesktopModuleId);
        //}
        //public IDataReader GetPortalDesktopModules(int PortalId, int DesktopModuleId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalDesktopModules", GetNull(PortalId), GetNull(DesktopModuleId));
        //}
        //public int AddPortalDesktopModule(int PortalId, int DesktopModuleId, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalDesktopModule", PortalId, DesktopModuleId, createdByUserID));
        //}
        //public void DeletePortalDesktopModules(int PortalId, int DesktopModuleId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalDesktopModules", GetNull(PortalId), GetNull(DesktopModuleId));
        //}
        //public IDataReader GetModuleDefinitions()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleDefinitions");
        //}
        //public IDataReader GetModuleDefinition(int ModuleDefId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleDefinition", ModuleDefId);
        //}
        //public IDataReader GetModuleDefinitionByName(int DesktopModuleId, string FriendlyName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleDefinitionByName", DesktopModuleId, FriendlyName);
        //}
        //public int AddModuleDefinition(int DesktopModuleId, string FriendlyName, int DefaultCacheTime, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddModuleDefinition", DesktopModuleId, FriendlyName, DefaultCacheTime, createdByUserID));
        //}
        //public void DeleteModuleDefinition(int ModuleDefId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleDefinition", ModuleDefId);
        //}
        //public void UpdateModuleDefinition(int ModuleDefId, string FriendlyName, int DefaultCacheTime, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleDefinition", ModuleDefId, FriendlyName, DefaultCacheTime, lastModifiedByUserID);
        //}
        //public IDataReader GetModuleControls()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControls");
        //}
        //public IDataReader GetModuleControl(int ModuleControlId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControl", ModuleControlId);
        //}
        //public IDataReader GetModuleControlsByKey(string ControlKey, int ModuleDefId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControlsByKey", GetNull(ControlKey), GetNull(ModuleDefId));
        //}
        //public IDataReader GetModuleControlByKeyAndSrc(int ModuleDefID, string ControlKey, string ControlSrc)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModuleControlByKeyAndSrc", GetNull(ModuleDefID), GetNull(ControlKey), GetNull(ControlSrc));
        //}
        //public int AddModuleControl(int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl, bool SupportsPartialRendering, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddModuleControl", GetNull(ModuleDefId), GetNull(ControlKey), GetNull(ControlTitle), ControlSrc, GetNull(IconFile), ControlType, GetNull(ViewOrder), GetNull(HelpUrl),
        //    SupportsPartialRendering, createdByUserID));
        //}
        //public void UpdateModuleControl(int ModuleControlId, int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl, bool SupportsPartialRendering,
        //int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModuleControl", ModuleControlId, GetNull(ModuleDefId), GetNull(ControlKey), GetNull(ControlTitle), ControlSrc, GetNull(IconFile), ControlType, GetNull(ViewOrder),
        //    GetNull(HelpUrl), SupportsPartialRendering, lastModifiedByUserID);
        //}
        //public void DeleteModuleControl(int ModuleControlId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModuleControl", ModuleControlId);
        //}
        //public int AddSkinControl(int packageID, string ControlKey, string ControlSrc, bool SupportsPartialRendering, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSkinControl", GetNull(packageID), GetNull(ControlKey), ControlSrc, SupportsPartialRendering, CreatedByUserID));
        //}
        //public void DeleteSkinControl(int skinControlID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSkinControl", skinControlID);
        //}
        //public IDataReader GetSkinControls()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinControls");
        //}
        //public IDataReader GetSkinControl(int skinControlID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinControl", skinControlID);
        //}
        //public IDataReader GetSkinControlByKey(string controlKey)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinControlByKey", controlKey);
        //}
        //public IDataReader GetSkinControlByPackageID(int packageID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinControlByPackageID", packageID);
        //}
        //public void UpdateSkinControl(int skinControlID, int packageID, string ControlKey, string ControlSrc, bool SupportsPartialRendering, int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateSkinControl", skinControlID, GetNull(packageID), GetNull(ControlKey), ControlSrc, SupportsPartialRendering, LastModifiedByUserID);
        //}
        //public IDataReader GetFiles(int PortalId, int FolderID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFiles", GetNull(PortalId), FolderID);
        //}
        //public IDataReader GetFile(string FileName, int PortalId, int FolderID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFile", FileName, GetNull(PortalId), FolderID);
        //}
        //public IDataReader GetFileById(int FileId, int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFileById", FileId, GetNull(PortalId));
        //}
        //public void DeleteFile(int PortalId, string FileName, int FolderID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFile", GetNull(PortalId), FileName, FolderID);
        //}
        //public void DeleteFiles(int PortalId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFiles", GetNull(PortalId));
        //}
        //public int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddFile", GetNull(PortalId), FileName, Extension, Size, GetNull(Width), GetNull(Height), ContentType, Folder,
        //    FolderID));
        //}
        //public void UpdateFile(int FileId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFile", FileId, FileName, Extension, Size, GetNull(Width), GetNull(Height), ContentType, Folder,
        //    FolderID);
        //}
        //public DataTable GetAllFiles()
        //{
        //    return SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllFiles").Tables[0];
        //}
        //public IDataReader GetFileContent(int FileId, int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFileContent", FileId, GetNull(PortalId));
        //}
        //public void UpdateFileContent(int FileId, byte[] Content)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFileContent", FileId, GetNull(Content));
        //}
        //public void AddSiteLog(System.DateTime DateTime, int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId, int AffiliateId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSiteLog", DateTime, PortalId, GetNull(UserId), GetNull(Referrer), GetNull(URL), GetNull(UserAgent), GetNull(UserHostAddress), GetNull(UserHostName),
        //    GetNull(TabId), GetNull(AffiliateId));
        //}
        //public IDataReader GetSiteLog(int PortalId, string PortalAlias, string ReportName, System.DateTime StartDate, System.DateTime EndDate)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ReportName, PortalId, PortalAlias, StartDate, EndDate);
        //}
        //public IDataReader GetSiteLogReports()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSiteLogReports");
        //}
        //public void DeleteSiteLog(System.DateTime DateTime, int PortalId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSiteLog", DateTime, PortalId);
        //}
        //public IDataReader GetTables()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTables");
        //}
        //public IDataReader GetFields(string TableName)
        //{
        //    string SQL = "SELECT * FROM {objectQualifier}" + TableName + " WHERE 1 = 0";
        //    return ExecuteSQL(SQL);
        //}
        //public IDataReader GetVendors(int PortalId, bool UnAuthorized, int PageIndex, int PageSize)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendors", GetNull(PortalId), UnAuthorized, GetNull(PageSize), GetNull(PageIndex));
        //}
        //public IDataReader GetVendorsByEmail(string Filter, int PortalId, int PageIndex, int PageSize)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorsByEmail", Filter, GetNull(PortalId), GetNull(PageSize), GetNull(PageIndex));
        //}
        //public IDataReader GetVendorsByName(string Filter, int PortalId, int PageIndex, int PageSize)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorsByName", Filter, GetNull(PortalId), GetNull(PageSize), GetNull(PageIndex));
        //}
        //public IDataReader GetVendor(int VendorId, int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendor", VendorId, GetNull(PortalId));
        //}
        //public void DeleteVendor(int VendorId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteVendor", VendorId);
        //}
        //public int AddVendor(int PortalId, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax,
        //string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddVendor", GetNull(PortalId), VendorName, Unit, Street, City, Region, Country, PostalCode,
        //    Telephone, Fax, Cell, Email, Website, FirstName, LastName, UserName, LogoFile, KeyWords,
        //    bool.Parse(Authorized)));
        //}
        //public void UpdateVendor(int VendorId, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax,
        //string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateVendor", VendorId, VendorName, Unit, Street, City, Region, Country, PostalCode,
        //    Telephone, Fax, Cell, Email, Website, FirstName, LastName, UserName, LogoFile, KeyWords,
        //    bool.Parse(Authorized));
        //}
        //public IDataReader GetVendorClassifications(int VendorId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetVendorClassifications", GetNull(VendorId));
        //}
        //public void DeleteVendorClassifications(int VendorId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteVendorClassifications", VendorId);
        //}
        //public int AddVendorClassification(int VendorId, int ClassificationId)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddVendorClassification", VendorId, ClassificationId));
        //}
        //public IDataReader GetBanners(int VendorId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetBanners", VendorId);
        //}
        //public IDataReader GetBanner(int BannerId, int VendorId, int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetBanner", BannerId, VendorId, GetNull(PortalId));
        //}
        //public DataTable GetBannerGroups(int PortalId)
        //{
        //    return SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner + ObjectQualifier + "GetBannerGroups", GetNull(PortalId)).Tables[0];
        //}
        //public void DeleteBanner(int BannerId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteBanner", BannerId);
        //}
        //public int AddBanner(string BannerName, int VendorId, string ImageFile, string URL, int Impressions, double CPM, System.DateTime StartDate, System.DateTime EndDate, string UserName, int BannerTypeId,
        //string Description, string GroupName, int Criteria, int Width, int Height)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddBanner", BannerName, VendorId, GetNull(ImageFile), GetNull(URL), Impressions, CPM, GetNull(StartDate), GetNull(EndDate),
        //    UserName, BannerTypeId, GetNull(Description), GetNull(GroupName), Criteria, Width, Height));
        //}
        //public void UpdateBanner(int BannerId, string BannerName, string ImageFile, string URL, int Impressions, double CPM, System.DateTime StartDate, System.DateTime EndDate, string UserName, int BannerTypeId,
        //string Description, string GroupName, int Criteria, int Width, int Height)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateBanner", BannerId, BannerName, GetNull(ImageFile), GetNull(URL), Impressions, CPM, GetNull(StartDate), GetNull(EndDate),
        //    UserName, BannerTypeId, GetNull(Description), GetNull(GroupName), Criteria, Width, Height);
        //}
        //public IDataReader FindBanners(int PortalId, int BannerTypeId, string GroupName)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "FindBanners", GetNull(PortalId), GetNull(BannerTypeId), GetNull(GroupName));
        //}
        //public void UpdateBannerViews(int BannerId, System.DateTime StartDate, System.DateTime EndDate)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateBannerViews", BannerId, GetNull(StartDate), GetNull(EndDate));
        //}
        //public void UpdateBannerClickThrough(int BannerId, int VendorId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateBannerClickThrough", BannerId, VendorId);
        //}
        //public IDataReader GetAffiliates(int VendorId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAffiliates", VendorId);
        //}
        //public IDataReader GetAffiliate(int AffiliateId, int VendorId, int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAffiliate", AffiliateId, VendorId, GetNull(PortalId));
        //}
        //public void DeleteAffiliate(int AffiliateId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteAffiliate", AffiliateId);
        //}
        //public int AddAffiliate(int VendorId, System.DateTime StartDate, System.DateTime EndDate, double CPC, double CPA)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddAffiliate", VendorId, GetNull(StartDate), GetNull(EndDate), CPC, CPA));
        //}
        //public void UpdateAffiliate(int AffiliateId, System.DateTime StartDate, System.DateTime EndDate, double CPC, double CPA)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAffiliate", AffiliateId, GetNull(StartDate), GetNull(EndDate), CPC, CPA);
        //}
        //public void UpdateAffiliateStats(int AffiliateId, int Clicks, int Acquisitions)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAffiliateStats", AffiliateId, Clicks, Acquisitions);
        //}
        //public bool CanDeleteSkin(string SkinType, string SkinFoldername)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "CanDeleteSkin", SkinType, SkinFoldername)) == 1;
        //}
        //public int AddSkin(int skinPackageID, string skinSrc)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSkin", skinPackageID, skinSrc));
        //}
        //public int AddSkinPackage(int packageID, int portalID, string skinName, string skinType, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSkinPackage", packageID, GetNull(portalID), skinName, skinType, CreatedByUserID));
        //}
        //public void DeleteSkin(int skinID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSkin", skinID);
        //}
        //public void DeleteSkinPackage(int skinPackageID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSkinPackage", skinPackageID);
        //}
        //public IDataReader GetSkinByPackageID(int packageID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinPackageByPackageID", packageID);
        //}
        //public IDataReader GetSkinPackage(int portalID, string skinName, string skinType)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSkinPackage", GetNull(portalID), skinName, skinType);
        //}
        //public void UpdateSkin(int skinID, string skinSrc)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateSkin", skinID, skinSrc);
        //}
        //public void UpdateSkinPackage(int skinPackageID, int packageID, int portalID, string skinName, string skinType, int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateSkinPackage", skinPackageID, packageID, GetNull(portalID), skinName, skinType, LastModifiedByUserID);
        //}
        //public IDataReader GetAllProfiles()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAllProfiles");
        //}
        //public IDataReader GetProfile(int UserId, int PortalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetProfile", UserId, PortalId);
        //}
        //public void AddProfile(int UserId, int PortalId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddProfile", UserId, PortalId);
        //}
        //public void UpdateProfile(int UserId, int PortalId, string ProfileData)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateProfile", UserId, PortalId, ProfileData);
        //}
        //public int AddPropertyDefinition(int PortalId, int ModuleDefId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required, string ValidationExpression, int ViewOrder, bool Visible,
        //int Length, int CreatedByUserID)
        //{
        //    int retValue = Null.NullInteger;
        //    try
        //    {
        //        retValue = Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPropertyDefinition", GetNull(PortalId), ModuleDefId, DataType, DefaultValue, PropertyCategory, PropertyName, Required, ValidationExpression,
        //        ViewOrder, Visible, Length, CreatedByUserID));
        //    }
        //    catch (SqlException ex)
        //    {
        //        retValue = -ex.Number;
        //        if (ex.Number != 2601)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return retValue;
        //}
        //public void DeletePropertyDefinition(int definitionId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePropertyDefinition", definitionId);
        //}
        //public IDataReader GetPropertyDefinition(int definitionId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinition", definitionId);
        //}
        //public IDataReader GetPropertyDefinitionByName(int portalId, string name)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinitionByName", GetNull(portalId), name);
        //}
        //public IDataReader GetPropertyDefinitionsByPortal(int portalId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPropertyDefinitionsByPortal", GetNull(portalId));
        //}
        //public void UpdatePropertyDefinition(int PropertyDefinitionId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required, string ValidationExpression, int ViewOrder, bool Visible, int Length,
        //int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePropertyDefinition", PropertyDefinitionId, DataType, DefaultValue, PropertyCategory, PropertyName, Required, ValidationExpression, ViewOrder,
        //    Visible, Length, LastModifiedByUserID);
        //}
        //public IDataReader GetUrls(int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrls", PortalID);
        //}
        //public IDataReader GetUrl(int PortalID, string Url)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrl", PortalID, Url);
        //}
        //public void AddUrl(int PortalID, string Url)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrl", PortalID, Url);
        //}
        //public void DeleteUrl(int PortalID, string Url)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUrl", PortalID, Url);
        //}
        //public IDataReader GetUrlTracking(int PortalID, string Url, int ModuleID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrlTracking", PortalID, Url, GetNull(ModuleID));
        //}
        //public void AddUrlTracking(int PortalID, string Url, string UrlType, bool LogActivity, bool TrackClicks, int ModuleID, bool NewWindow)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrlTracking", PortalID, Url, UrlType, LogActivity, TrackClicks, GetNull(ModuleID), NewWindow);
        //}
        //public void UpdateUrlTracking(int PortalID, string Url, bool LogActivity, bool TrackClicks, int ModuleID, bool NewWindow)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateUrlTracking", PortalID, Url, LogActivity, TrackClicks, GetNull(ModuleID), NewWindow);
        //}
        //public void DeleteUrlTracking(int PortalID, string Url, int ModuleID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUrlTracking", PortalID, Url, GetNull(ModuleID));
        //}
        //public void UpdateUrlTrackingStats(int PortalID, string Url, int ModuleID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateUrlTrackingStats", PortalID, Url, GetNull(ModuleID));
        //}
        //public IDataReader GetUrlLog(int UrlTrackingID, System.DateTime StartDate, System.DateTime EndDate)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetUrlLog", UrlTrackingID, GetNull(StartDate), GetNull(EndDate));
        //}
        //public void AddUrlLog(int UrlTrackingID, int UserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddUrlLog", UrlTrackingID, GetNull(UserID));
        //}
        //public IDataReader GetPermissionsByModuleDefID(int ModuleDefID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByModuleDefID", ModuleDefID);
        //}
        //public IDataReader GetPermissionsByModuleID(int ModuleID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByModuleID", ModuleID);
        //}
        //public IDataReader GetPermissionsByPortalDesktopModule()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByPortalDesktopModule");
        //}
        //public IDataReader GetPermissionsByFolder()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByFolder");
        //}
        //public IDataReader GetPermissionByCodeAndKey(string PermissionCode, string PermissionKey)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionByCodeAndKey", GetNull(PermissionCode), GetNull(PermissionKey));
        //}
        //public IDataReader GetPermissionsByTab()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByTab");
        //}
        //public IDataReader GetPermission(int permissionID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermission", permissionID);
        //}
        //public void DeletePermission(int permissionID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePermission", permissionID);
        //}
        //public int AddPermission(string permissionCode, int moduleDefID, string permissionKey, string permissionName, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPermission", moduleDefID, permissionCode, permissionKey, permissionName, createdByUserID));
        //}
        //public void UpdatePermission(int permissionID, string permissionCode, int moduleDefID, string permissionKey, string permissionName, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePermission", permissionID, permissionCode, moduleDefID, permissionKey, permissionName, lastModifiedByUserID);
        //}
        //public IDataReader GetModulePermission(int modulePermissionID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermission", modulePermissionID);
        //}
        //public IDataReader GetModulePermissionsByModuleID(int moduleID, int PermissionID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByModuleID", moduleID, PermissionID);
        //}
        //public IDataReader GetModulePermissionsByPortal(int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByPortal", PortalID);
        //}
        //public IDataReader GetModulePermissionsByTabID(int TabID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePermissionsByTabID", TabID);
        //}
        //public void DeleteModulePermissionsByModuleID(int ModuleID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModulePermissionsByModuleID", ModuleID);
        //}
        //public void DeleteModulePermissionsByUserID(int PortalID, int UserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModulePermissionsByUserID", PortalID, UserID);
        //}
        //public void DeleteModulePermission(int modulePermissionID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteModulePermission", modulePermissionID);
        //}
        //public int AddModulePermission(int moduleID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddModulePermission", moduleID, PermissionID, GetRoleNull(roleID), AllowAccess, GetNull(UserID), createdByUserID));
        //}
        //public void UpdateModulePermission(int modulePermissionID, int moduleID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateModulePermission", modulePermissionID, moduleID, PermissionID, GetRoleNull(roleID), AllowAccess, GetNull(UserID), lastModifiedByUserID);
        //}
        //public IDataReader GetTabPermissionsByPortal(int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPermissionsByPortal", GetNull(PortalID));
        //}
        //public IDataReader GetTabPermissionsByTabID(int TabID, int PermissionID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetTabPermissionsByTabID", TabID, PermissionID);
        //}
        //public void DeleteTabPermissionsByTabID(int TabID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabPermissionsByTabID", TabID);
        //}
        //public void DeleteTabPermissionsByUserID(int PortalID, int UserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabPermissionsByUserID", PortalID, UserID);
        //}
        //public void DeleteTabPermission(int TabPermissionID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteTabPermission", TabPermissionID);
        //}
        //public int AddTabPermission(int TabID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddTabPermission", TabID, PermissionID, GetRoleNull(roleID), AllowAccess, GetNull(UserID), createdByUserID));
        //}
        //public void UpdateTabPermission(int TabPermissionID, int TabID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateTabPermission", TabPermissionID, TabID, PermissionID, GetRoleNull(roleID), AllowAccess, GetNull(UserID), lastModifiedByUserID);
        //}
        //public IDataReader GetDesktopModulePermission(int desktopModulePermissionID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulePermission", desktopModulePermissionID);
        //}
        //public IDataReader GetDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulePermissionsByPortalDesktopModuleID", portalDesktopModuleID);
        //}
        //public IDataReader GetDesktopModulePermissions()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDesktopModulePermissions");
        //}
        //public void DeleteDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModulePermissionsByPortalDesktopModuleID", portalDesktopModuleID);
        //}
        //public void DeleteDesktopModulePermissionsByUserID(int userID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModulePermissionsByUserID", userID);
        //}
        //public void DeleteDesktopModulePermission(int desktopModulePermissionID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteDesktopModulePermission", desktopModulePermissionID);
        //}
        //public int AddDesktopModulePermission(int portalDesktopModuleID, int permissionID, int roleID, bool allowAccess, int userID, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddDesktopModulePermission", portalDesktopModuleID, permissionID, GetRoleNull(roleID), allowAccess, GetNull(userID), createdByUserID));
        //}
        //public void UpdateDesktopModulePermission(int desktopModulePermissionID, int portalDesktopModuleID, int permissionID, int roleID, bool allowAccess, int userID, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateDesktopModulePermission", desktopModulePermissionID, portalDesktopModuleID, permissionID, GetRoleNull(roleID), allowAccess, GetNull(userID), lastModifiedByUserID);
        //}
        //public IDataReader GetFoldersByPortal(int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolders", GetNull(PortalID), -1, "");
        //}
        //public IDataReader GetFolder(int PortalID, int FolderID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderByFolderID", GetNull(PortalID), FolderID);
        //}
        //public IDataReader GetFolder(int PortalID, string FolderPath)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderByFolderPath", GetNull(PortalID), FolderPath);
        //}
        //public int AddFolder(int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, System.DateTime LastUpdated, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddFolder", GetNull(PortalID), FolderPath, StorageLocation, IsProtected, IsCached, GetNull(LastUpdated), createdByUserID));
        //}
        //public void UpdateFolder(int PortalID, int FolderID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, System.DateTime LastUpdated, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFolder", GetNull(PortalID), FolderID, FolderPath, StorageLocation, IsProtected, IsCached, GetNull(LastUpdated), lastModifiedByUserID);
        //}
        //public void DeleteFolder(int PortalID, string FolderPath)
        //{
        //    SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolder", GetNull(PortalID), FolderPath);
        //}
        //public IDataReader GetFolderPermission(int FolderPermissionID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderPermission", FolderPermissionID);
        //}
        //public IDataReader GetFolderPermissionsByPortal(int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderPermissionsByPortal", GetNull(PortalID));
        //}
        //public IDataReader GetFolderPermissionsByFolderPath(int PortalID, string FolderPath, int PermissionID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetFolderPermissionsByFolderPath", GetNull(PortalID), FolderPath, PermissionID);
        //}
        //public void DeleteFolderPermissionsByFolderPath(int PortalID, string FolderPath)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderPermissionsByFolderPath", GetNull(PortalID), FolderPath);
        //}
        //public void DeleteFolderPermissionsByUserID(int PortalID, int UserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderPermissionsByUserID", PortalID, UserID);
        //}
        //public void DeleteFolderPermission(int FolderPermissionID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteFolderPermission", FolderPermissionID);
        //}
        //public int AddFolderPermission(int FolderID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddFolderPermission", FolderID, PermissionID, GetRoleNull(roleID), AllowAccess, GetNull(UserID), createdByUserID));
        //}
        //public void UpdateFolderPermission(int FolderPermissionID, int FolderID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateFolderPermission", FolderPermissionID, FolderID, PermissionID, GetRoleNull(roleID), AllowAccess, GetNull(UserID), lastModifiedByUserID);
        //}
        //public System.Data.IDataReader GetSearchIndexers()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchIndexers");
        //}
        //public System.Data.IDataReader GetSearchResultModules(int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchResultModules", PortalID);
        //}
        //public void DeleteSearchItems(int ModuleID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItems", ModuleID);
        //}
        //public void DeleteSearchItem(int SearchItemId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItem", SearchItemId);
        //}
        //public void DeleteSearchItemWords(int SearchItemId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSearchItemWords", SearchItemId);
        //}
        //public int AddSearchItem(string Title, string Description, int Author, System.DateTime PubDate, int ModuleId, string Key, string Guid, int ImageFileId)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchItem", Title, Description, GetNull(Author), GetNull(PubDate), ModuleId, Key, Guid, ImageFileId));
        //}
        //public System.Data.IDataReader GetSearchCommonWordsByLocale(string Locale)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchCommonWordsByLocale", Locale);
        //}
        //public System.Data.IDataReader GetDefaultLanguageByModule(string ModuleList)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetDefaultLanguageByModule", ModuleList);
        //}
        //public IDataReader GetSearchSettings(int ModuleId)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchSettings", ModuleId);
        //}
        //public System.Data.IDataReader GetSearchWords()
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchWords");
        //}
        //public int AddSearchWord(string Word)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchWord", Word));
        //}
        //public int AddSearchItemWord(int SearchItemId, int SearchWordsID, int Occurrences)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchItemWord", SearchItemId, SearchWordsID, Occurrences));
        //}
        //public void AddSearchItemWordPosition(int SearchItemWordID, string ContentPositions)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddSearchItemWordPosition", SearchItemWordID, ContentPositions);
        //}
        //public IDataReader GetSearchResults(int PortalID, string Word)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchResultsByWord", PortalID, Word);
        //}
        //public IDataReader GetSearchItems(int PortalID, int TabID, int ModuleID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchItems", GetNull(PortalID), GetNull(TabID), GetNull(ModuleID));
        //}
        //public IDataReader GetSearchResults(int PortalID, int TabID, int ModuleID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchResults", GetNull(PortalID), GetNull(TabID), GetNull(ModuleID));
        //}
        //public IDataReader GetSearchItem(int ModuleID, string SearchKey)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSearchItem", GetNull(ModuleID), SearchKey);
        //}
        //public void UpdateSearchItem(int SearchItemId, string Title, string Description, int Author, System.DateTime PubDate, int ModuleId, string Key, string Guid, int HitCount, int ImageFileId)
        //{
        //    SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateSearchItem", SearchItemId, Title, Description, GetNull(Author), GetNull(PubDate), ModuleId, Key, Guid,
        //    HitCount, ImageFileId);
        //}
        //public IDataReader GetLists(int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetLists", PortalID);
        //}
        //public IDataReader GetList(string ListName, string ParentKey, int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetList", ListName, ParentKey, PortalID);
        //}
        //public IDataReader GetListEntry(string ListName, string Value)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetListEntry", ListName, Value, -1);
        //}
        //public IDataReader GetListEntry(int EntryID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetListEntry", "", "", EntryID);
        //}
        //public IDataReader GetListEntriesByListName(string ListName, string ParentKey, int PortalID)
        //{
        //    return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetListEntries", ListName, ParentKey, GetNull(PortalID));
        //}
        //public int AddListEntry(string ListName, string Value, string Text, int ParentID, int Level, bool EnableSortOrder, int DefinitionID, string Description, int PortalID, bool SystemList, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddListEntry", ListName, Value, Text, ParentID, Level, EnableSortOrder, DefinitionID, Description, PortalID, SystemList, CreatedByUserID));
        //}
        //public void UpdateListEntry(int EntryID, string Value, string Text, string Description, int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateListEntry", EntryID, Value, Text, Description, LastModifiedByUserID);
        //}
        //public void DeleteListEntryByID(int EntryID, bool DeleteChild)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteListEntryByID", EntryID, DeleteChild);
        //}
        //public void DeleteList(string ListName, string ParentKey)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteList", ListName, ParentKey);
        //}
        //public void DeleteListEntryByListName(string ListName, string Value, bool DeleteChild)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteListEntryByListName", ListName, Value, DeleteChild);
        //}
        //public void UpdateListSortOrder(int EntryID, bool MoveUp)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateListSortOrder", EntryID, MoveUp);
        //}
        //public IDataReader GetPortalAlias(string PortalAlias, int PortalID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAlias", PortalAlias, PortalID);
        //}
        //public IDataReader GetPortalByPortalAliasID(int PortalAliasId)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalByPortalAliasID", PortalAliasId);
        //}
        //public void UpdatePortalAlias(string PortalAlias, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalAliasOnInstall", PortalAlias, lastModifiedByUserID);
        //}
        //public void UpdatePortalAliasInfo(int PortalAliasID, int PortalID, string HTTPAlias, int lastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalAlias", PortalAliasID, PortalID, HTTPAlias, lastModifiedByUserID);
        //}
        //public int AddPortalAlias(int PortalID, string HTTPAlias, int createdByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalAlias", PortalID, HTTPAlias, createdByUserID));
        //}
        //public void DeletePortalAlias(int PortalAliasID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalAlias", PortalAliasID);
        //}
        //public IDataReader GetPortalAliasByPortalID(int PortalID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAliasByPortalID", PortalID);
        //}
        //public IDataReader GetPortalAliasByPortalAliasID(int PortalAliasID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalAliasByPortalAliasID", PortalAliasID);
        //}
        //public int AddEventMessage(string eventName, int priority, string processorType, string processorCommand, string body, string sender, string subscriberId, string authorizedRoles, string exceptionMessage, System.DateTime sentDate,
        //System.DateTime expirationDate, string attributes)
        //{
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddEventMessage", eventName, priority, processorType, processorCommand, body, sender, subscriberId, authorizedRoles,
        //    exceptionMessage, sentDate, expirationDate, attributes);
        //}
        //public IDataReader GetEventMessages(string eventName)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventMessages", eventName);
        //}
        //public IDataReader GetEventMessagesBySubscriber(string eventName, string subscriberId)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventMessagesBySubscriber", eventName, subscriberId);
        //}
        //public void SetEventMessageComplete(int eventMessageId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "SetEventMessageComplete", eventMessageId);
        //}
        //public int AddAuthentication(int packageID, string authenticationType, bool isEnabled, string settingsControlSrc, string loginControlSrc, string logoffControlSrc, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddAuthentication", packageID, authenticationType, isEnabled, settingsControlSrc, loginControlSrc, logoffControlSrc, CreatedByUserID));
        //}
        //public int AddUserAuthentication(int userID, string authenticationType, string authenticationToken, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddUserAuthentication", userID, authenticationType, authenticationToken, CreatedByUserID));
        //}
        //public void DeleteAuthentication(int authenticationID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteAuthentication", authenticationID);
        //}
        //public IDataReader GetAuthenticationService(int authenticationID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAuthenticationService", authenticationID);
        //}
        //public IDataReader GetAuthenticationServiceByPackageID(int packageID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAuthenticationServiceByPackageID", packageID);
        //}
        //public IDataReader GetAuthenticationServiceByType(string authenticationType)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAuthenticationServiceByType", authenticationType);
        //}
        //public IDataReader GetAuthenticationServices()
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetAuthenticationServices");
        //}
        //public IDataReader GetEnabledAuthenticationServices()
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEnabledAuthenticationServices");
        //}
        //public void UpdateAuthentication(int authenticationID, int packageID, string authenticationType, bool isEnabled, string settingsControlSrc, string loginControlSrc, string logoffControlSrc, int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAuthentication", authenticationID, packageID, authenticationType, isEnabled, settingsControlSrc, loginControlSrc, logoffControlSrc, LastModifiedByUserID);
        //}
        //public int AddPackage(int portalID, string name, string friendlyName, string description, string type, string version, string license, string manifest, string owner, string organization,
        //string url, string email, string releaseNotes, bool isSystemPackage, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPackage", GetNull(portalID), name, friendlyName, description, type, version, license, manifest,
        //    owner, organization, url, email, releaseNotes, isSystemPackage, CreatedByUserID));
        //}
        //public void DeletePackage(int packageID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePackage", packageID);
        //}
        //public IDataReader GetPackage(int packageID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackage", packageID);
        //}
        //public IDataReader GetPackageByName(int portalID, string name)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackageByName", GetNull(portalID), name);
        //}
        //public IDataReader GetPackages(int portalID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackages", GetNull(portalID));
        //}
        //public IDataReader GetPackagesByType(int portalID, string type)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackagesByType", GetNull(portalID), type);
        //}
        //public IDataReader GetPackageType(string type)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackageType", type);
        //}
        //public IDataReader GetPackageTypes()
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPackageTypes");
        //}
        //public IDataReader GetModulePackagesInUse(int portalID, bool forHost)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetModulePackagesInUse", portalID, forHost);
        //}
        //public int RegisterAssembly(int packageID, string assemblyName, string version)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "RegisterAssembly", packageID, assemblyName, version));
        //}
        //public bool UnRegisterAssembly(int packageID, string assemblyName)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "UnRegisterAssembly", packageID, assemblyName)) == 1;
        //}
        //public void UpdatePackage(int portalID, string name, string friendlyName, string description, string type, string version, string license, string manifest, string owner, string organization,
        //string url, string email, string releaseNotes, bool isSystemPackage, int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePackage", GetNull(portalID), name, friendlyName, description, type, version, license, manifest,
        //    owner, organization, url, email, releaseNotes, isSystemPackage, LastModifiedByUserID);
        //}
        //public int AddLanguage(string cultureCode, string cultureName, string fallbackCulture, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddLanguage", cultureCode, cultureName, fallbackCulture, CreatedByUserID));
        //}
        //public void DeleteLanguage(int languageID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteLanguage", languageID);
        //}
        //public IDataReader GetLanguages()
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetLanguages");
        //}
        //public void UpdateLanguage(int languageID, string cultureCode, string cultureName, string fallbackCulture, int LastModifiedByUserID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateLanguage", languageID, cultureCode, cultureName, fallbackCulture, LastModifiedByUserID);
        //}
        //public int AddPortalLanguage(int portalID, int languageID, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddPortalLanguage", portalID, languageID, CreatedByUserID));
        //}
        //public void DeletePortalLanguages(int portalID, int languageID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeletePortalLanguages", GetNull(portalID), GetNull(languageID));
        //}
        //public IDataReader GetLanguagesByPortal(int portalID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetLanguagesByPortal", portalID);
        //}
        //public int AddLanguagePack(int packageID, int languageID, int dependentPackageID, int CreatedByUserID)
        //{
        //    return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddLanguagePack", packageID, languageID, dependentPackageID, CreatedByUserID));
        //}
        //public void DeleteLanguagePack(int languagePackID)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteLanguagePack", languagePackID);
        //}
        //public IDataReader GetLanguagePackByPackage(int packageID)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetLanguagePackByPackage", packageID);
        //}
        //public int UpdateLanguagePack(int languagePackID, int packageID, int languageID, int dependentPackageID, int LastModifiedByUserID)
        //{
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateLanguagePack", languagePackID, packageID, languageID, dependentPackageID, LastModifiedByUserID);
        //}
        ////localisation
        //public string GetPortalDefaultLanguage(int portalID)
        //{
        //    return Convert.ToString(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPortalDefaultLanguage", portalID));
        //}
        //public void UpdatePortalDefaultLanguage(int portalID, string CultureCode)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdatePortalDefaultLanguage", portalID, CultureCode);
        //}
        //public void EnsureLocalizationExists(int portalID, string CultureCode)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "EnsureLocalizationExists", portalID, CultureCode);
        //}
    }
}
