using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Xml;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.FileSystem;
using CommonLibrary.Services.Localization;
using CommonLibrary.Entities.Host;
using CommonLibrary.Framework.Providers;
using CommonLibrary.Data;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Security.Roles;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Application;
using CommonLibrary.Entities.Users;
using CommonLibrary.UI.Utilities;
using CommonLibrary.Entities.Modules.Actions;

namespace CommonLibrary.Common
{
	public static class Globals
	{
        

        public enum PerformanceSettings
        {
            NoCaching = 0,
            LightCaching = 1,
            ModerateCaching = 3,
            HeavyCaching = 6
        }
        public enum UpgradeStatus
        {
            Upgrade,
            Install,
            None,
            Error
        }
        public enum PortalRegistrationType
        {
            NoRegistration = 0,
            PrivateRegistration = 1,
            PublicRegistration = 2,
            VerifiedRegistration = 3
        }

       
        public const string glbRoleAllUsers = "-1";
        public const string glbRoleSuperUser = "-2";
        public const string glbRoleUnauthUser = "-3";
        public const string glbRoleNothing = "-4";
        public const string glbRoleAllUsersName = "All Users";
        public const string glbRoleSuperUserName = "Superuser";
        public const string glbRoleUnauthUserName = "Unauthenticated Users";
        public const string glbDefaultPage = "Default.aspx";
        public const string glbHostSkinFolder = "_default";
        public const string glbDefaultControlPanel = "Admin/ControlPanel/IconBar.ascx";
        public const string glbDefaultPane = "ContentPane";
        public const string glbImageFileTypes = "jpg,jpeg,jpe,gif,bmp,png,swf";
        public const string glbConfigFolder = "\\Config\\";
        public const string glbAboutPage = "about.htm";
        public const string glbConfig = "5eagles.config";
        public const int glbSuperUserAppName = -1;
        public const string glbProtectedExtension = ".resources";
        public const string glbEmailRegEx = "\\b[a-zA-Z0-9._%\\-+']+@[a-zA-Z0-9.\\-]+\\.[a-zA-Z]{2,4}\\b";
        public const string glbScriptFormat = "<script type=\"text/javascript\" src=\"{0}\" ></script>";
        private static string _ApplicationPath;
        private static string _ApplicationMapPath;
        private static System.Version _DataBaseVersion;
        private static string _HostMapPath;
        private static string _HostPath;
        private static string _InstallMapPath;
        private static string _InstallPath;
        private static string _IISAppName;
        private static bool _IsInstalled;
        private static string _ServerName;
        private static System.Version _OperatingSystemVersion;
        private static System.Version _NETFrameworkVersion;
        private static System.Version _DatabaseEngineVersion;
        private static UpgradeStatus _Status = UpgradeStatus.None;
        private static Hashtable _HostSettings;
        private static bool _WebFarmEnabled = Null.NullBoolean;
        private static bool _WebFarmEnabledSet = Null.NullBoolean;
        private static PerformanceSettings _PerformanceSetting;
        public static string ApplicationPath
        {
            get { return _ApplicationPath; }
            set { _ApplicationPath = value; }
        }
        public static string ApplicationMapPath
        {
            get { return _ApplicationMapPath; }
            set { _ApplicationMapPath = value; }
        }
        public static System.Version DataBaseVersion
        {
            get { return _DataBaseVersion; }
        }
        public static string HostMapPath
        {
            get { return _HostMapPath; }
            set { _HostMapPath = value; }
        }
        public static string HostPath
        {
            get { return _HostPath; }
            set { _HostPath = value; }
        }
        public static string InstallMapPath
        {
            get { return _InstallMapPath; }
            set { _InstallMapPath = value; }
        }
        public static string InstallPath
        {
            get { return _InstallPath; }
            set { _InstallPath = value; }
        }
        public static string IISAppName
        {
            get { return _IISAppName; }
            set { _IISAppName = value; }
        }
        public static string ServerName
        {
            get { return _ServerName; }
            set { _ServerName = value; }
        }
        public static System.Version OperatingSystemVersion
        {
            get { return _OperatingSystemVersion; }
            set { _OperatingSystemVersion = value; }
        }
        public static System.Version NETFrameworkVersion
        {
            get { return _NETFrameworkVersion; }
            set { _NETFrameworkVersion = value; }
        }
        public static System.Version DatabaseEngineVersion
        {
            get { return _DatabaseEngineVersion; }
            set { _DatabaseEngineVersion = value; }
        }
        public static UpgradeStatus Status
        {
            get { return _Status; }
        }
        private static bool IsInstalled()
        {
            const int c_PassingScore = 4;
            int installationdatefactor = Convert.ToInt32(HasInstallationDate() ? 1 : 0);
            int dataproviderfactor = Convert.ToInt32(HasDataProviderLogFiles() ? 3 : 0);
            int htmlmodulefactor = Convert.ToInt32(ModuleDirectoryExists("html") ? 2 : 0);
            int portaldirectoryfactor = Convert.ToInt32(HasNonDefaultPortalDirectory() ? 2 : 0);
            int localexecutionfactor = Convert.ToInt32(HttpContext.Current.Request.IsLocal ? c_PassingScore - 1 : 0);
            return (!IsInstallationURL()) && ((installationdatefactor + dataproviderfactor + htmlmodulefactor + portaldirectoryfactor + localexecutionfactor) >= c_PassingScore);
        }
        private static bool HasDataProviderLogFiles()
        {
            Provider currentdataprovider = Config.GetDefaultProvider("data");
            string providerpath = currentdataprovider.Attributes["providerPath"];
            if (!string.IsNullOrEmpty(providerpath))
            {
                providerpath = HttpContext.Current.Server.MapPath(providerpath);
                if (Directory.Exists(providerpath))
                {
                    return Directory.GetFiles(providerpath, "*.log.resources").Length > 0;
                }
            }
            return false;
        }
        private static bool HasInstallationDate()
        {
            return Config.GetSetting("InstallationDate") == null ? false : true;
        }
        private static bool ModuleDirectoryExists(string ModuleName)
        {
            string dir = Globals.ApplicationMapPath + "\\desktopmodules\\" + ModuleName;
            return Directory.Exists(dir);
        }
        private static bool HasNonDefaultPortalDirectory()
        {
            string dir = Globals.ApplicationMapPath + "\\portals";
            if (Directory.Exists(dir))
            {
                return Directory.GetDirectories(dir).Length > 1;
            }
            return false;
        }
        private static bool IsInstallationURL()
        {
            string requestURL = HttpContext.Current.Request.RawUrl.ToLowerInvariant();
            return requestURL.Contains("\\install.aspx") || requestURL.Contains("\\installwizard.aspx");
        }
        public static DataSet BuildCrossTabDataSet(string DataSetName, IDataReader result, string FixedColumns, string VariableColumns, string KeyColumn, string FieldColumn, string FieldTypeColumn, string StringValueColumn, string NumericValueColumn)
        {
            return BuildCrossTabDataSet(DataSetName, result, FixedColumns, VariableColumns, KeyColumn, FieldColumn, FieldTypeColumn, StringValueColumn, NumericValueColumn, System.Globalization.CultureInfo.CurrentCulture);
        }
        public static DataSet BuildCrossTabDataSet(string DataSetName, IDataReader result, string FixedColumns, string VariableColumns, string KeyColumn, string FieldColumn, string FieldTypeColumn, string StringValueColumn, string NumericValueColumn, System.Globalization.CultureInfo Culture)
        {
            string[] arrFixedColumns = null;
            string[] arrVariableColumns = null;
            string[] arrField;
            string FieldType;
            int intColumn;
            int intKeyColumn;
            DataSet crosstab = new DataSet(DataSetName);
            crosstab.Namespace = "NetFrameWork";
            DataTable tab = new DataTable(DataSetName);
            arrFixedColumns = FixedColumns.Split(',');
            for (intColumn = 0; intColumn < arrFixedColumns.Length; intColumn++)
            {
                arrField = arrFixedColumns[intColumn].Split('|');
                DataColumn col = new DataColumn(arrField[0], System.Type.GetType("System." + arrField[1]));
                tab.Columns.Add(col);
            }
            if (!String.IsNullOrEmpty(VariableColumns))
            {
                arrVariableColumns = VariableColumns.Split(',');
                for (intColumn = 0; intColumn < arrVariableColumns.Length; intColumn++)
                {
                    arrField = arrVariableColumns[intColumn].Split('|');
                    DataColumn col = new DataColumn(arrField[0], System.Type.GetType("System." + arrField[1]));
                    col.AllowDBNull = true;
                    tab.Columns.Add(col);
                }
            }
            crosstab.Tables.Add(tab);
            intKeyColumn = -1;
            DataRow row = null;
            while (result.Read())
            {
                if (Convert.ToInt32(result[KeyColumn]) != intKeyColumn)
                {
                    if (intKeyColumn != -1)
                    {
                        tab.Rows.Add(row);
                    }
                    row = tab.NewRow();
                    for (intColumn = 0; intColumn < arrFixedColumns.Length; intColumn++)
                    {
                        arrField = arrFixedColumns[intColumn].Split('|');
                        row[arrField[0]] = result[arrField[0]];
                    }
                    if (!String.IsNullOrEmpty(VariableColumns))
                    {
                        for (intColumn = 0; intColumn < arrVariableColumns.Length; intColumn++)
                        {
                            arrField = arrVariableColumns[intColumn].Split('|');
                            switch (arrField[1])
                            {
                                case "Decimal":
                                    row[arrField[0]] = 0;
                                    break;
                                case "String":
                                    row[arrField[0]] = "";
                                    break;
                            }
                        }
                    }
                    intKeyColumn = Convert.ToInt32(result[KeyColumn]);
                }
                if (!String.IsNullOrEmpty(FieldTypeColumn))
                {
                    FieldType = result[FieldTypeColumn].ToString();
                }
                else
                {
                    FieldType = "String";
                }
                switch (FieldType)
                {
                    case "Decimal":
                        row[Convert.ToInt32(result[FieldColumn])] = result[NumericValueColumn];
                        break;
                    case "String":
                        if (object.ReferenceEquals(Culture, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            row[result[FieldColumn].ToString()] = result[StringValueColumn];
                        }
                        else
                        {
                            switch (tab.Columns[result[FieldColumn].ToString()].DataType.ToString())
                            {
                                case "System.Decimal":
                                case "System.Currency":
                                    row[result[FieldColumn].ToString()] = decimal.Parse(result[StringValueColumn].ToString(), Culture);
                                    break;
                                case "System.Int32":
                                    row[result[FieldColumn].ToString()] = Int32.Parse(result[StringValueColumn].ToString(), Culture);
                                    break;
                                default:
                                    row[result[FieldColumn].ToString()] = result[StringValueColumn];
                                    break;
                            }
                        }
                        break;
                }
            }
            result.Close();
            if (intKeyColumn != -1)
            {
                tab.Rows.Add(row);
            }
            crosstab.AcceptChanges();
            return crosstab;
        }
        public static DataSet ConvertDataReaderToDataSet(IDataReader reader)
        {
            DataSet objDataSet = new DataSet();
            objDataSet.Tables.Add(ConvertDataReaderToDataTable(reader));
            return objDataSet;
        }
        public static DataTable ConvertDataReaderToDataTable(IDataReader reader)
        {
            DataTable objDataTable = new DataTable();
            int intFieldCount = reader.FieldCount;
            int intCounter;
            for (intCounter = 0; intCounter <= intFieldCount - 1; intCounter++)
            {
                objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter));
            }
            objDataTable.BeginLoadData();
            object[] objValues = new object[intFieldCount];
            while (reader.Read())
            {
                reader.GetValues(objValues);
                objDataTable.LoadDataRow(objValues, true);
            }
            reader.Close();
            objDataTable.EndLoadData();
            return objDataTable;
        }
        public static string GetAbsoluteServerPath(HttpRequest Request)
        {
            string strServerPath;
            strServerPath = Request.MapPath(Request.ApplicationPath);
            if (!strServerPath.EndsWith("\\"))
            {
                strServerPath += "\\";
            }
            return strServerPath;
        }

        public static string ResolveVirtual(string PhysicalPath)
        {
            string appPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            string url = PhysicalPath.Substring(appPath.Length).Replace('\\', '/').Insert(0, "~/");
            return (url);
        }


        public static string GetApplicationName()
        {
            string appName;
            if (HttpContext.Current.Items["ApplicationName"] == null
                || String.IsNullOrEmpty(HttpContext.Current.Items["ApplicationName"].ToString()))
            {
                PortalSettings _PortalSettings = PortalController.GetCurrentPortalSettings();
                if (_PortalSettings == null)
                {
                    appName = "/";
                }
                else
                {
                    appName = GetApplicationName(_PortalSettings.PortalId);
                }
            }
            else
            {
                appName = Convert.ToString(HttpContext.Current.Items["ApplicationName"]);
            }
            return appName;
        }
        public static string GetApplicationName(int PortalID)
        {
            string appName;
            ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration("data");
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];
            string _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (!String.IsNullOrEmpty(_objectQualifier) && _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }
            appName = _objectQualifier + Convert.ToString(PortalID);
            return appName;
        }
        public static bool FindDatabaseVersion(int Major, int Minor, int Build)
        {
            bool version = false;
            IDataReader dr = null;
            try
            {
                dr = DataProvider.Instance().FindDatabaseVersion(Major, Minor, Build);
                if (dr.Read())
                {
                    version = true;
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return version;
        }
        public static void UpdateDataBaseVersion(System.Version version)
        {
            DataProvider.Instance().UpdateDatabaseVersion(version.Major, version.Minor, version.Build, AppContext.Current.Application.Name);
            _DataBaseVersion = version;
        }
        public static string GetDomainName(HttpRequest Request)
        {
            return GetDomainName(Request, false);
        }
        public static string GetDomainName(HttpRequest Request, bool ParsePortNumber)
        {
            StringBuilder DomainName = new StringBuilder();
            string[] URL;
            string URI;
            int intURL = 0;
            URI = Request.Url.ToString();
            string hostHeader = Config.GetSetting("HostHeader");
            if (!String.IsNullOrEmpty(hostHeader))
            {
                URI = URI.ToLower().Replace(hostHeader.ToLower(), "");
            }
            intURL = URI.IndexOf("?");
            if (intURL > -1)
            {
                URI = URI.Substring(0, intURL);
            }
            URL = URI.Split('/');
            for (intURL = 2; intURL <= URL.GetUpperBound(0); intURL++)
            {
                bool needExit = false;
                switch (URL[intURL].ToLower())
                {
                    case "admin":
                    case "controls":
                    case "desktopmodules":
                    case "mobilemodules":
                    case "premiummodules":
                    case "providers":
                        needExit = true;
                        break;
                    default:
                        if ((URL[intURL].Length >= ".aspx".Length))
                        {
                            if (URL[intURL].ToLower().LastIndexOf(".aspx") == (URL[intURL].Length - (".aspx".Length))
                                || URL[intURL].ToLower().LastIndexOf(".axd") == (URL[intURL].Length - (".axd".Length))
                                || URL[intURL].ToLower().LastIndexOf(".ashx") == (URL[intURL].Length - (".ashx".Length)))
                            {
                                break;
                            }
                        }
                        DomainName.Append((!String.IsNullOrEmpty(DomainName.ToString()) ? "/" : "") + URL[intURL]);
                        break;
                }
                if (needExit) break;
            }
            if (ParsePortNumber)
            {
                if (DomainName.ToString().IndexOf(":") != -1)
                {
                    bool usePortNumber = (!Request.IsLocal);
                    if (Utilities.Config.GetSetting("UsePortNumber") != null)
                    {
                        usePortNumber = bool.Parse(Utilities.Config.GetSetting("UsePortNumber"));
                    }
                    if (usePortNumber == false)
                    {
                        DomainName = DomainName.Replace(":" + Request.Url.Port.ToString(), "");
                    }
                }
            }
            return DomainName.ToString();
        }
        public static ArrayList GetFileList()
        {
            return GetFileList(-1, "", true, "", false);
        }
        public static ArrayList GetFileList(int PortalId)
        {
            return GetFileList(PortalId, "", true, "", false);
        }
        public static ArrayList GetFileList(int PortalId, string strExtensions)
        {
            return GetFileList(PortalId, strExtensions, true, "", false);
        }
        public static ArrayList GetFileList(int PortalId, string strExtensions, bool NoneSpecified)
        {
            return GetFileList(PortalId, strExtensions, NoneSpecified, "", false);
        }
        public static ArrayList GetFileList(int PortalId, string strExtensions, bool NoneSpecified, string Folder)
        {
            return GetFileList(PortalId, strExtensions, NoneSpecified, Folder, false);
        }
        public static ArrayList GetFileList(int PortalId, string strExtensions, bool NoneSpecified, string Folder, bool includeHidden)
        {
            ArrayList arrFileList = new ArrayList();
            if (NoneSpecified)
            {
                arrFileList.Add(new FileItem("", "<" + Localization.GetString("None_Specified") + ">"));
            }
            string portalRoot;
            if (PortalId == Null.NullInteger)
            {
                portalRoot = HostMapPath;
            }
            else
            {
                PortalController objPortals = new PortalController();
                PortalInfo objPortal = objPortals.GetPortal(PortalId);
                portalRoot = objPortal.HomeDirectoryMapPath;
            }
            FolderInfo objFolder = FileSystemUtils.GetFolder(PortalId, Folder);
            if (objFolder != null)
            {
                FileController objFiles = new FileController();
                IDataReader dr = null;
                try
                {
                    dr = objFiles.GetFiles(PortalId, objFolder.FolderID);
                    while (dr.Read())
                    {
                        if (FilenameMatchesExtensions(dr["FileName"].ToString(), strExtensions))
                        {
                            string filePath = (portalRoot + dr["Folder"].ToString() + dr["fileName"].ToString()).Replace("/", "\\");
                            int StorageLocation = 0;
                            if (dr["StorageLocation"] != null)
                            {
                                StorageLocation = Convert.ToInt32(dr["StorageLocation"]);
                                switch (StorageLocation)
                                {
                                    case 1:
                                        filePath = filePath + glbProtectedExtension;
                                        break;
                                    case 2:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (StorageLocation != 2)
                            {
                                if (File.Exists(filePath))
                                {
                                    if (includeHidden)
                                    {
                                        arrFileList.Add(new FileItem(dr["FileID"].ToString(), dr["FileName"].ToString()));
                                    }
                                    else
                                    {
                                        System.IO.FileAttributes attributes = File.GetAttributes(filePath);
                                        if ((attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                                        {
                                            arrFileList.Add(new FileItem(dr["FileID"].ToString(), dr["FileName"].ToString()));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                arrFileList.Add(new FileItem(dr["FileID"].ToString(), dr["FileName"].ToString()));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
            }
            return arrFileList;
        }
        public static PortalSettings GetHostPortalSettings()
        {
            int TabId = -1;
            int PortalId = -1;
            PortalAliasInfo objPortalAliasInfo = null;
            if (Host.HostPortalID > Null.NullInteger)
            {
                PortalId = Host.HostPortalID;
                objPortalAliasInfo = new PortalAliasInfo();
                objPortalAliasInfo.PortalID = PortalId;
            }
            return new PortalSettings(TabId, objPortalAliasInfo);
        }
        public static string GetPortalDomainName(string strPortalAlias, HttpRequest Request, bool blnAddHTTP)
        {
            string strDomainName = "";
            string strURL = "";
            string[] arrPortalAlias;
            int intAlias;
            if (Request != null)
            {
                strURL = GetDomainName(Request);
            }
            arrPortalAlias = strPortalAlias.Split(',');
            for (intAlias = 0; intAlias <= arrPortalAlias.Length - 1; intAlias++)
            {
                if (arrPortalAlias[intAlias] == strURL)
                {
                    strDomainName = arrPortalAlias[intAlias];
                }
            }
            if (String.IsNullOrEmpty(strDomainName))
            {
                strDomainName = arrPortalAlias[0];
            }
            if (blnAddHTTP)
            {
                strDomainName = AddHTTP(strDomainName);
            }
            return strDomainName;
        }
        public static PortalSettings GetPortalSettings()
        {
            PortalSettings portalSettings = null;
            if (HttpContext.Current != null)
            {
                portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            }
            if (portalSettings == null)
            {
                portalSettings = GetHostPortalSettings();
            }
            return portalSettings;
        }
        public static string GetSubFolderPath(string strFileNamePath, int portalId)
        {
            string ParentFolderName;
            if (portalId == Null.NullInteger)
            {
                ParentFolderName = Globals.HostMapPath.Replace("/", "\\");
            }
            else
            {
                PortalController objPortals = new PortalController();
                PortalInfo objPortal = objPortals.GetPortal(portalId);
                ParentFolderName = objPortal.HomeDirectoryMapPath.Replace("/", "\\");
            }
            string strFolderpath = strFileNamePath.Substring(0, strFileNamePath.LastIndexOf("\\") + 1);
            return strFolderpath.Substring(ParentFolderName.Length).Replace("\\", "/");
        }
        public static int GetTotalRecords(ref IDataReader dr)
        {
            int total = 0;
            if (dr.Read())
            {
                try
                {
                    total = Convert.ToInt32(dr["TotalRecords"]);
                }
                catch (Exception ex)
                {
                    total = -1;
                }
            }
            return total;
        }
        public static void GetStatus()
        {
            string strMessage = "";
            if (DataBaseVersion != null)
            {
                return;
            }
            _Status = UpgradeStatus.None;
            strMessage = DataProvider.Instance().GetProviderPath();
            if (!strMessage.StartsWith("ERROR:"))
            {
                try
                {
                    _DataBaseVersion = DataProvider.Instance().GetVersion();
                }
                catch (Exception ex)
                {
                    strMessage = "ERROR:" + ex.Message;
                }
            }
            if (strMessage.StartsWith("ERROR"))
            {
                if (IsInstalled() && !HttpContext.Current.Request.IsLocal)
                {
                    _Status = UpgradeStatus.Error;
                }
                else
                {
                    _Status = UpgradeStatus.Install;
                }
            }
            else if (DataBaseVersion == null)
            {
                _Status = UpgradeStatus.Install;
            }
            else
            {
                if (AppContext.Current.Application.Version.Major > DataBaseVersion.Major)
                {
                    _Status = UpgradeStatus.Upgrade;
                }
                else if (AppContext.Current.Application.Version.Major == DataBaseVersion.Major && AppContext.Current.Application.Version.Minor > DataBaseVersion.Minor)
                {
                    _Status = UpgradeStatus.Upgrade;
                }
                else if (AppContext.Current.Application.Version.Major == DataBaseVersion.Major && AppContext.Current.Application.Version.Minor == DataBaseVersion.Minor && AppContext.Current.Application.Version.Build > DataBaseVersion.Build)
                {
                    _Status = UpgradeStatus.Upgrade;
                }
            }
        }

        public static void SetStatus(UpgradeStatus status)
        {
            _Status = status;
        }

        public static string ImportFile(int PortalId, string url)
        {
            string strUrl = url;
            if (GetURLType(url) == TabType.File)
            {
                FileController objFileController = new FileController();
                int fileId = objFileController.ConvertFilePathToFileId(url, PortalId);
                if (fileId >= 0)
                {
                    strUrl = "FileID=" + fileId.ToString();
                }
            }
            return strUrl;
        }
        public static string HTTPPOSTEncode(string strPost)
        {
            strPost = strPost.Replace("\\", "");
            strPost = System.Web.HttpUtility.UrlEncode(strPost);
            strPost = strPost.Replace("%2f", "/");
            return strPost;
        }
        public static void SetApplicationName(int PortalID)
        {
            HttpContext.Current.Items["ApplicationName"] = GetApplicationName(PortalID);
        }
        public static void SetApplicationName(string ApplicationName)
        {
            HttpContext.Current.Items["ApplicationName"] = ApplicationName;
        }
        public static string FormatAddress(object Unit, object Street, object City, object Region, object Country, object PostalCode)
        {
            string strAddress = "";
            if (Unit != null)
            {
                if (!String.IsNullOrEmpty(Unit.ToString().Trim()))
                {
                    strAddress += ", " + Unit.ToString();
                }
            }
            if (Street != null)
            {
                if (!String.IsNullOrEmpty(Street.ToString().Trim()))
                {
                    strAddress += ", " + Street.ToString();
                }
            }
            if (City != null)
            {
                if (!String.IsNullOrEmpty(City.ToString().Trim()))
                {
                    strAddress += ", " + City.ToString();
                }
            }
            if (Region != null)
            {
                if (!String.IsNullOrEmpty(Region.ToString().Trim()))
                {
                    strAddress += ", " + Region.ToString();
                }
            }
            if (Country != null)
            {
                if (!String.IsNullOrEmpty(Country.ToString().Trim()))
                {
                    strAddress += ", " + Country.ToString();
                }
            }
            if (PostalCode != null)
            {
                if (!String.IsNullOrEmpty(PostalCode.ToString().Trim()))
                {
                    strAddress += ", " + PostalCode.ToString();
                }
            }
            if (!String.IsNullOrEmpty(strAddress.Trim()))
            {
                strAddress = strAddress.Substring(3);
            }
            return strAddress;
        }
        public static string FormatVersion(System.Version version)
        {
            return FormatVersion(version, false);
        }
        public static string FormatVersion(System.Version version, bool includeBuild)
        {
            string strVersion = version.Major.ToString("00") + "." + version.Minor.ToString("00") + "." + version.Build.ToString("00");
            if (includeBuild)
            {
                strVersion += " (" + version.Revision.ToString() + ")";
            }
            return strVersion;
        }
        public static string FormatVersion(System.Version version, string fieldFormat, int fieldCount, string delimiterCharacter)
        {
            string strVersion = "";
            int intZero = 0;
            if (version != null)
            {
                if (fieldCount > 0)
                {
                    if (version.Major >= 0)
                    {
                        strVersion += version.Major.ToString(fieldFormat);
                    }
                    else
                    {
                        strVersion += intZero.ToString(fieldFormat);
                    }
                }
                if (fieldCount > 1)
                {
                    strVersion += delimiterCharacter;
                    if (version.Minor >= 0)
                    {
                        strVersion += version.Minor.ToString(fieldFormat);
                    }
                    else
                    {
                        strVersion += intZero.ToString(fieldFormat);
                    }
                }
                if (fieldCount > 2)
                {
                    strVersion += delimiterCharacter;
                    if (version.Build >= 0)
                    {
                        strVersion += version.Build.ToString(fieldFormat);
                    }
                    else
                    {
                        strVersion += intZero.ToString(fieldFormat);
                    }
                }
                if (fieldCount > 3)
                {
                    strVersion += delimiterCharacter;
                    if (version.Revision >= 0)
                    {
                        strVersion += version.Revision.ToString(fieldFormat);
                    }
                    else
                    {
                        strVersion += intZero.ToString(fieldFormat);
                    }
                }
            }
            return strVersion;
        }
        public static string CloakText(string PersonalInfo)
        {
            if (PersonalInfo != null)
            {
                StringBuilder sb = new StringBuilder();
                char[] chars = PersonalInfo.ToCharArray();
                foreach (char chr in chars)
                {
                    sb.Append(((int)chr).ToString());
                }
                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                StringBuilder sbScript = new StringBuilder();
                sbScript.Append(Environment.NewLine + "<script type=\"text/javascript\">" + Environment.NewLine);
                sbScript.Append("//<![CDATA[" + Environment.NewLine);
                sbScript.Append("   document.write(String.fromCharCode(" + sb.ToString() + "))" + Environment.NewLine);
                sbScript.Append("//]]>" + Environment.NewLine);
                sbScript.Append("</script>" + Environment.NewLine);
                return sbScript.ToString();
            }
            else
            {
                return Null.NullString;
            }
        }
        public static string GetMediumDate(string strDate)
        {
            if (!String.IsNullOrEmpty(strDate))
            {
                System.DateTime datDate = Convert.ToDateTime(strDate);
                string strYear = datDate.Year.ToString();
                string strMonth = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(datDate.Month);
                string strDay = datDate.Day.ToString();
                strDate = strDay + "-" + strMonth + "-" + strYear;
            }
            return strDate;
        }
        public static string GetShortDate(string strDate)
        {
            if (!String.IsNullOrEmpty(strDate))
            {
                System.DateTime datDate = Convert.ToDateTime(strDate);
                string strYear = datDate.Year.ToString();
                string strMonth = datDate.Month.ToString();
                string strDay = datDate.Day.ToString();
                strDate = strMonth + "/" + strDay + "/" + strYear;
            }
            return strDate;
        }
        public static bool IsAdminControl()
        {
            if (HttpContext.Current == null)
            {
                return false;
            }
            return (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["mid"])) || (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ctl"]));
        }
        public static bool IsAdminSkin()
        {
            bool _IsAdminSkin = Null.NullBoolean;
            if (HttpContext.Current != null)
            {
                string AdminKeys = "tab,module,importmodule,exportmodule,help";
                string ControlKey = "";
                if (HttpContext.Current.Request.QueryString["ctl"] != null)
                {
                    ControlKey = HttpContext.Current.Request.QueryString["ctl"].ToLower();
                }
                int ModuleID = -1;
                if (HttpContext.Current.Request.QueryString["mid"] != null)
                {
                    Int32.TryParse(HttpContext.Current.Request.QueryString["mid"], out ModuleID);
                }
                _IsAdminSkin = (!String.IsNullOrEmpty(ControlKey) && ControlKey != "view" && ModuleID != -1) || (!String.IsNullOrEmpty(ControlKey) && AdminKeys.IndexOf(ControlKey) != -1 && ModuleID == -1);
            }
            return _IsAdminSkin;
        }
        public static bool IsEditMode()
        {
            return (TabPermissionController.CanAddContentToPage() && PortalController.GetCurrentPortalSettings().UserMode == PortalSettings.Mode.Edit);
        }
        public static bool IsLayoutMode()
        {
            return (TabPermissionController.CanAddContentToPage() && PortalController.GetCurrentPortalSettings().UserMode == PortalSettings.Mode.Layout);
        }
        public static void CreateRSS(IDataReader dr, string TitleField, string URLField, string CreatedDateField, string SyndicateField, string DomainName, string FileName)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string strRSS = "";
            string strRelativePath = DomainName + FileName.Substring(FileName.IndexOf("\\Portals")).Replace("\\", "/");
            strRelativePath = strRelativePath.Substring(0, strRelativePath.LastIndexOf("/"));
            try
            {
                while (dr.Read())
                {
                    if (Convert.ToInt32(dr[SyndicateField]) > 0)
                    {
                        strRSS += "      <item>" + Environment.NewLine;
                        strRSS += "         <title>" + dr[TitleField].ToString() + "</title>" + Environment.NewLine;
                        if (dr["URL"].ToString().IndexOf("://") == -1)
                        {
                            if (Regex.IsMatch(dr["URL"].ToString(), "^\\d+$"))
                            {
                                strRSS += "         <link>" + DomainName + "/" + glbDefaultPage + "?tabid=" + dr[URLField].ToString() + "</link>" + Environment.NewLine;
                            }
                            else
                            {
                                strRSS += "         <link>" + strRelativePath + dr[URLField].ToString() + "</link>" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            strRSS += "         <link>" + dr[URLField].ToString() + "</link>" + Environment.NewLine;
                        }
                        strRSS += "         <description>" + _portalSettings.PortalName + " " + GetMediumDate(dr[CreatedDateField].ToString()) + "</description>" + Environment.NewLine;
                        strRSS += "     </item>" + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            if (!String.IsNullOrEmpty(strRSS))
            {
                strRSS = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" + Environment.NewLine + "<rss version=\"0.91\">" + Environment.NewLine + "  <channel>" + Environment.NewLine + "     <title>" + _portalSettings.PortalName + "</title>" + Environment.NewLine + "     <link>" + DomainName + "</link>" + Environment.NewLine + "     <description>" + _portalSettings.PortalName + "</description>" + Environment.NewLine + "     <language>en-us</language>" + Environment.NewLine + "     <copyright>" + _portalSettings.FooterText + "</copyright>" + Environment.NewLine + "     <webMaster>" + _portalSettings.Email + "</webMaster>" + Environment.NewLine + strRSS + "   </channel>" + Environment.NewLine + "</rss>";
                StreamWriter objStream;
                objStream = File.CreateText(FileName);
                objStream.WriteLine(strRSS);
                objStream.Close();
            }
            else
            {
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                }
            }
        }
        public static string ManageUploadDirectory(string strHTML, string strUploadDirectory)
        {
            strHTML = ManageTokenUploadDirectory(strHTML, strUploadDirectory, "src");
            return ManageTokenUploadDirectory(strHTML, strUploadDirectory, "background");
        }
        public static string ManageTokenUploadDirectory(string strHTML, string strUploadDirectory, string strToken)
        {
            int P;
            int R;
            int S = 0;
            int tLen;
            string strURL;
            StringBuilder sbBuff = new StringBuilder("");
            if (!String.IsNullOrEmpty(strHTML))
            {
                tLen = strToken.Length + 2;
                string _UploadDirectory = strUploadDirectory.ToLower();
                P = strHTML.IndexOf(strToken + "=\"", StringComparison.InvariantCultureIgnoreCase);
                while (P != -1)
                {
                    sbBuff.Append(strHTML.Substring(S, P - S + tLen));
                    S = P + tLen;
                    R = strHTML.IndexOf("\"", S);
                    if (R >= 0)
                    {
                        strURL = strHTML.Substring(S, R - S).ToLower();
                    }
                    else
                    {
                        strURL = strHTML.Substring(S).ToLower();
                    }
                    if (!strURL.Contains("://") && !strURL.StartsWith("/") && !strURL.StartsWith(_UploadDirectory))
                    {
                        sbBuff.Append(strUploadDirectory);
                    }
                    P = strHTML.IndexOf(strToken + "=\"", S + strURL.Length + 2, StringComparison.InvariantCultureIgnoreCase);
                }
                if (S > -1)
                    sbBuff.Append(strHTML.Substring(S));
            }
            return sbBuff.ToString();
        }
        public static Control FindControlRecursive(Control objControl, string strControlName)
        {
            if (objControl.Parent == null)
            {
                return null;
            }
            else
            {
                if (objControl.Parent.FindControl(strControlName) != null)
                {
                    return objControl.Parent.FindControl(strControlName);
                }
                else
                {
                    return FindControlRecursive(objControl.Parent, strControlName);
                }
            }
        }
        public static Control FindControlRecursiveDown(Control objParent, string strControlName)
        {
            Control objCtl;
            objCtl = objParent.FindControl(strControlName);
            if (objCtl == null)
            {
                foreach (Control objChild in objParent.Controls)
                {
                    objCtl = FindControlRecursiveDown(objChild, strControlName);
                    if (objCtl != null)
                        break;
                }
            }
            return objCtl;
        }
        public static void SetFormFocus(Control control)
        {
            //if (control.Page != null && control.Visible)
            //{
            //    if (control.Page.Request.Browser.EcmaScriptVersion.Major >= 1)
            //    {
            //        if (ClientAPI.ClientAPIDisabled() == false)
            //        {
            //            ClientAPI.RegisterClientReference(control.Page, ClientNamespaceReferences.dnn);
            //            ClientAPI.AddBodyOnloadEventHandler(control.Page, "__dnn_SetInitialFocus('" + control.ClientID + "');");
            //        }
            //        else
            //        {
            //            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //            sb.Append("<script type=\"text/javascript\">");
            //            sb.Append("<!--");
            //            sb.Append(Environment.NewLine);
            //            sb.Append("function SetInitialFocus() {");
            //            sb.Append(Environment.NewLine);
            //            sb.Append(" document.");
            //            Control objParent = control.Parent;
            //            while (!(objParent is System.Web.UI.HtmlControls.HtmlForm))
            //            {
            //                objParent = objParent.Parent;
            //            }
            //            sb.Append(objParent.ClientID);
            //            sb.Append("['");
            //            sb.Append(control.UniqueID);
            //            sb.Append("'].focus(); }");
            //            sb.Append("window.onload = SetInitialFocus;");
            //            sb.Append(Environment.NewLine);
            //            sb.Append("// -->");
            //            sb.Append(Environment.NewLine);
            //            sb.Append("</script>");
            //            ClientAPI.RegisterClientScriptBlock(control.Page, "InitialFocus", sb.ToString());
            //        }
            //    }
            //}
        }
        public static HttpWebRequest GetExternalRequest(string Address)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(Address);
            objRequest.Timeout = Host.WebRequestTimeout;
            objRequest.UserAgent = "DotNetNuke";
            if (!string.IsNullOrEmpty(Host.ProxyServer))
            {
                WebProxy Proxy;
                NetworkCredential ProxyCredentials;
                Proxy = new WebProxy(Host.ProxyServer, Host.ProxyPort);
                if (!string.IsNullOrEmpty(Host.ProxyUsername))
                {
                    ProxyCredentials = new NetworkCredential(Host.ProxyUsername, Host.ProxyPassword);
                    Proxy.Credentials = ProxyCredentials;
                }
                objRequest.Proxy = Proxy;
            }
            return objRequest;
        }
        public static HttpWebRequest GetExternalRequest(string Address, NetworkCredential Credentials)
        {
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(Address);
            objRequest.Timeout = Host.WebRequestTimeout;
            objRequest.UserAgent = "DotNetNuke";
            if (Credentials.UserName != null)
            {
                objRequest.Credentials = Credentials;
            }
            if (!string.IsNullOrEmpty(Host.ProxyServer))
            {
                WebProxy Proxy;
                NetworkCredential ProxyCredentials;
                Proxy = new WebProxy(Host.ProxyServer, Host.ProxyPort);
                if (!string.IsNullOrEmpty(Host.ProxyUsername))
                {
                    ProxyCredentials = new NetworkCredential(Host.ProxyUsername, Host.ProxyPassword);
                    Proxy.Credentials = ProxyCredentials;
                }
                objRequest.Proxy = Proxy;
            }
            return objRequest;
        }
        public static void DeleteFolderRecursive(string strRoot)
        {
            if (!String.IsNullOrEmpty(strRoot))
            {
                if (Directory.Exists(strRoot))
                {
                    foreach (string strFolder in Directory.GetDirectories(strRoot))
                    {
                        DeleteFolderRecursive(strFolder);
                    }
                    foreach (string strFile in Directory.GetFiles(strRoot))
                    {
                        try
                        {
                            FileSystemUtils.DeleteFile(strFile);
                        }
                        catch
                        {
                        }
                    }
                    try
                    {
                        Directory.Delete(strRoot);
                    }
                    catch
                    {
                    }
                }
            }
        }
        public static void DeleteFilesRecursive(string strRoot, string filter)
        {
            if (!String.IsNullOrEmpty(strRoot))
            {
                if (Directory.Exists(strRoot))
                {
                    foreach (string strFolder in Directory.GetDirectories(strRoot))
                    {
                        DirectoryInfo directory = new DirectoryInfo(strFolder);
                        if ((directory.Attributes & FileAttributes.Hidden) == 0 && (directory.Attributes & FileAttributes.System) == 0)
                        {
                            DeleteFilesRecursive(strFolder, filter);
                        }
                    }
                    foreach (string strFile in Directory.GetFiles(strRoot, "*" + filter))
                    {
                        try
                        {
                            FileSystemUtils.DeleteFile(strFile);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        private static object ValidIDLookupCallback(CacheItemArgs cacheItemArgs)
        {
            return new Dictionary<string, string>();
        }
        public static string CreateValidID(string inputValue)
        {
            string returnValue;
            Dictionary<string, string> validIDLookup = CBO.GetCachedObject<Dictionary<string, string>>(new CacheItemArgs("ValidIDLookup", 200, System.Web.Caching.CacheItemPriority.NotRemovable), ValidIDLookupCallback);
            if (validIDLookup.ContainsKey(inputValue))
            {
                returnValue = validIDLookup[inputValue];
            }
            else
            {
                Regex invalidCharacters = new Regex("[^A-Z0-9_:]", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                returnValue = invalidCharacters.Replace(inputValue, "_");
                Regex invalidInitialCharacters = new Regex("^[^A-Z]", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                returnValue = invalidInitialCharacters.Replace(returnValue, "A");
                validIDLookup[inputValue] = returnValue;
            }
            return returnValue;
        }
        public static string CleanFileName(string FileName)
        {
            return CleanFileName(FileName, "", "");
        }
        public static string CleanFileName(string FileName, string BadChars)
        {
            return CleanFileName(FileName, BadChars, "");
        }
        public static string CleanFileName(string FileName, string BadChars, string ReplaceChar)
        {
            string strFileName = FileName;
            if (String.IsNullOrEmpty(BadChars))
            {
                BadChars = ":/\\?*|" + ((char)34) + ((char)39) + ((char)9);
            }
            if (String.IsNullOrEmpty(ReplaceChar))
            {
                ReplaceChar = "_";
            }
            int intCounter;
            for (intCounter = 0; intCounter <= BadChars.Length - 1; intCounter++)
            {
                strFileName = strFileName.Replace(BadChars.Substring(intCounter, 1), ReplaceChar);
            }
            return strFileName;
        }
        public static string CleanName(string Name)
        {
            string strName = Name;
            string strBadChars = ". ~`!@#$%^&*()-_+={[}]|\\:;<,>?/" + ((char)34) + ((char)39);
            int intCounter;
            for (intCounter = 0; intCounter <= strBadChars.Length - 1; intCounter++)
            {
                strName = strName.Replace(strBadChars.Substring(intCounter, 1), "");
            }
            return strName;
        }
        public static string AccessDeniedURL()
        {
            return AccessDeniedURL("");
        }
        public static string AccessDeniedURL(string Message)
        {
            string strURL = "";
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                if (String.IsNullOrEmpty(Message))
                {
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Access Denied");
                }
                else
                {
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Access Denied", "message=" + HttpUtility.UrlEncode(Message));
                }
            }
            else
            {
                strURL = LoginURL(HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl), false);
            }
            return strURL;
        }
        public static string AddHTTP(string strURL)
        {
            if (!String.IsNullOrEmpty(strURL))
            {
                if (strURL.IndexOf("mailto:") == -1 && strURL.IndexOf("://") == -1 && strURL.IndexOf("~") == -1 && strURL.IndexOf("\\\\") == -1)
                {
                    if (HttpContext.Current != null && HttpContext.Current.Request.IsSecureConnection)
                    {
                        strURL = "https://" + strURL;
                    }
                    else
                    {
                        strURL = "http://" + strURL;
                    }
                }
            }
            return strURL;
        }
        public static string ApplicationURL()
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            if (_portalSettings != null)
            {
                return (ApplicationURL(_portalSettings.ActiveTab.TabID));
            }
            else
            {
                return (ApplicationURL(-1));
            }
        }
        public static string ApplicationURL(int TabID)
        {
            string strURL = "~/" + glbDefaultPage;
            if (TabID != -1)
            {
                strURL += "?tabid=" + TabID.ToString();
            }
            return strURL;
        }
        public static string FormatHelpUrl(string HelpUrl, PortalSettings objPortalSettings, string Name)
        {
            return FormatHelpUrl(HelpUrl, objPortalSettings, Name, "");
        }
        public static string FormatHelpUrl(string HelpUrl, PortalSettings objPortalSettings, string Name, string Version)
        {
            string strURL = HelpUrl;
            if (strURL.IndexOf("?") != -1)
            {
                strURL += "&helpculture=";
            }
            else
            {
                strURL += "?helpculture=";
            }
            if (!String.IsNullOrEmpty(Thread.CurrentThread.CurrentCulture.ToString().ToLower()))
            {
                strURL += Thread.CurrentThread.CurrentCulture.ToString().ToLower();
            }
            else
            {
                strURL += objPortalSettings.DefaultLanguage.ToLower();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                strURL += "&helpmodule=" + System.Web.HttpUtility.UrlEncode(Name);
            }
            if (!String.IsNullOrEmpty(Version))
            {
                strURL += "&helpversion=" + System.Web.HttpUtility.UrlEncode(Version);
            }
            return AddHTTP(strURL);
        }
        public static string FriendlyUrl(TabInfo tab, string path)
        {
            return FriendlyUrl(tab, path, glbDefaultPage);
        }
        public static string FriendlyUrl(TabInfo tab, string path, string pageName)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return FriendlyUrl(tab, path, pageName, _portalSettings);
        }
        public static string FriendlyUrl(TabInfo tab, string path, PortalSettings settings)
        {
            return FriendlyUrl(tab, path, glbDefaultPage, settings);
        }
        public static string FriendlyUrl(TabInfo tab, string path, string pageName, PortalSettings settings)
        {
            return CommonLibrary.Services.Url.FriendlyUrl.FriendlyUrlProvider.Instance().FriendlyUrl(tab, path, pageName, settings);
        }
        public static string FriendlyUrl(TabInfo tab, string path, string pageName, string portalAlias)
        {
            return CommonLibrary.Services.Url.FriendlyUrl.FriendlyUrlProvider.Instance().FriendlyUrl(tab, path, pageName, portalAlias);
        }
        public static TabType GetURLType(string URL)
        {
            if (String.IsNullOrEmpty(URL))
            {
                return TabType.Normal;
            }
            else
            {
                if (URL.ToLower().StartsWith("mailto:") == false && URL.IndexOf("://") == -1 && URL.StartsWith("~") == false && URL.StartsWith("\\\\") == false && URL.StartsWith("/") == false)
                {
                    if (Regex.IsMatch(URL, "^\\d+$"))
                    {
                        return TabType.Tab;
                    }
                    else
                    {
                        if (URL.ToLower().StartsWith("userid="))
                        {
                            return TabType.Member;
                        }
                        else
                        {
                            return TabType.File;
                        }
                    }
                }
                else
                {
                    return TabType.Url;
                }
            }
        }
        public static string ImportUrl(int ModuleId, string url)
        {
            string strUrl = url;
            TabType urlType = GetURLType(url);
            int intId = -1;
            PortalSettings portalSettings = GetPortalSettings();
            switch (urlType)
            {
                case TabType.File:
                    if (Int32.TryParse(url.Replace("FileID=", ""), out intId))
                    {
                        FileController objFileController = new FileController();
                        CommonLibrary.Services.FileSystem.FileInfo objFile = objFileController.GetFileById(intId, portalSettings.PortalId);
                        if (objFile == null)
                        {
                            strUrl = "";
                        }
                    }
                    else
                    {
                        strUrl = "";
                    }
                    break;
                case TabType.Member:
                    if (Int32.TryParse(url.Replace("UserID=", ""), out intId))
                    {
                        if (UserController.GetUserById(portalSettings.PortalId, intId) == null)
                        {
                            strUrl = "";
                        }
                    }
                    else
                    {
                        strUrl = "";
                    }
                    break;
                case TabType.Tab:
                    if (Int32.TryParse(url, out intId))
                    {
                        TabController objTabController = new TabController();
                        if (objTabController.GetTab(intId, portalSettings.PortalId, false) == null)
                        {
                            strUrl = "";
                        }
                    }
                    else
                    {
                        strUrl = "";
                    }
                    break;
            }
            return strUrl;
        }
        public static string LoginURL(string returnURL, bool @override)
        {
            string strURL = "";
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            returnURL = String.Format("returnurl={0}", returnURL);
            if (_portalSettings.LoginTabId != -1 && !@override)
            {
                if (ValidateLoginTabID(_portalSettings.LoginTabId))
                {
                    if (string.IsNullOrEmpty(returnURL))
                    {
                        strURL = NavigateURL(_portalSettings.LoginTabId, "");
                    }
                    else
                    {
                        strURL = NavigateURL(_portalSettings.LoginTabId, "", returnURL);
                    }
                }
                else
                {
                    string strMessage = String.Format("error={0}", Localization.GetString("NoLoginControl", Localization.GlobalResourceFile));
                    if (string.IsNullOrEmpty(returnURL))
                    {
                        strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login", strMessage);
                    }
                    else
                    {
                        strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login", returnURL, strMessage);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(returnURL))
                {
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login");
                }
                else
                {
                    strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Login", returnURL);
                }
            }
            return strURL;
        }
        public static string UserProfileURL(int userId)
        {
            string strURL = "";
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();

            strURL = NavigateURL(_portalSettings.UserTabId, "", string.Format("userId={0}", userId));

            return strURL;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL()
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return NavigateURL(_portalSettings.ActiveTab.TabID, Null.NullString);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID)
        {
            return NavigateURL(TabID, Null.NullString);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, bool IsSuperTab)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return NavigateURL(TabID, IsSuperTab, _portalSettings, Null.NullString, "", null);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(string ControlKey)
        {
            if (ControlKey == "Access Denied")
            {
                return AccessDeniedURL();
            }
            else
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                return NavigateURL(_portalSettings.ActiveTab.TabID, ControlKey);
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(string ControlKey, params string[] AdditionalParameters)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return NavigateURL(_portalSettings.ActiveTab.TabID, ControlKey, AdditionalParameters);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, string ControlKey)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return NavigateURL(TabID, _portalSettings, ControlKey, null);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, string ControlKey, params string[] AdditionalParameters)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return NavigateURL(TabID, _portalSettings, ControlKey, AdditionalParameters);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, PortalSettings settings, string ControlKey, params string[] AdditionalParameters)
        {
            bool isSuperTab = false;
            if (settings != null)
            {
                if (settings.ActiveTab.IsSuperTab)
                {
                    isSuperTab = true;
                }
            }
            return NavigateURL(TabID, isSuperTab, settings, ControlKey, AdditionalParameters);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string NavigateURL(int TabID, bool IsSuperTab, PortalSettings settings, string ControlKey, params object[] AdditionalParameters)
        {
            return NavigateURL(TabID, IsSuperTab, settings, ControlKey, Thread.CurrentThread.CurrentCulture.Name, AdditionalParameters);
        }
        public static string NavigateURL(int TabID, bool IsSuperTab, PortalSettings settings, string ControlKey, string Language, params object[] AdditionalParameters)
        {
            string strURL;
            if (TabID == Null.NullInteger)
            {
                strURL = ApplicationURL();
            }
            else
            {
                strURL = ApplicationURL(TabID);
            }
            if (!String.IsNullOrEmpty(ControlKey))
            {
                strURL += "&ctl=" + ControlKey;
            }
            if (AdditionalParameters != null)
            {
                foreach (string parameter in AdditionalParameters)
                {
                    if (!string.IsNullOrEmpty(parameter))
                    {
                        strURL += "&" + parameter.ToString();
                    }
                }
            }
            if (IsSuperTab)
            {
                strURL += "&portalid=" + settings.PortalId.ToString();
            }
            if (settings != null && Localization.GetLocales(settings.PortalId).Count > 1 && settings.EnableUrlLanguage)
            {
                if (String.IsNullOrEmpty(Language))
                {
                    strURL += "&language=" + Thread.CurrentThread.CurrentCulture.Name;
                }
                else
                {
                    strURL += "&language=" + Language;
                }
            }
            if (Host.UseFriendlyUrls)
            {
                TabInfo objTab = null;
                if (new TabController().GetTabsByPortal(settings.PortalId).TryGetValue(TabID, out objTab))
                {
                    return FriendlyUrl(objTab, strURL, settings);
                }
                return FriendlyUrl(null, strURL, settings);
            }
            else
            {
                return ResolveUrl(strURL);
            }
        }
        public static string QueryStringEncode(string QueryString)
        {
            QueryString = HttpUtility.UrlEncode(QueryString);
            return QueryString;
        }
        public static string QueryStringDecode(string QueryString)
        {
            QueryString = HttpUtility.UrlDecode(QueryString);
            string fullPath;
            try
            {
                fullPath = HttpContext.Current.Request.MapPath(QueryString, HttpContext.Current.Request.ApplicationPath, false);
            }
            catch (HttpException ex)
            {
                throw new HttpException(404, "Not Found" + ex.ToString());
            }
            string strDoubleDecodeURL = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Server.UrlDecode(QueryString));
            if (QueryString.IndexOf("..") != -1 || strDoubleDecodeURL.IndexOf("..") != -1)
            {
                throw new HttpException(404, "Not Found");
            }
            return QueryString;
        }
        public static string RegisterURL(string returnURL, string originalURL)
        {
            string strURL = "";
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string extraParams = string.Concat("returnurl=", returnURL);
            if (!string.IsNullOrEmpty(originalURL))
            {
                extraParams += string.Concat("&orignalurl=", originalURL);
            }
            if (_portalSettings.RegisterTabId != -1)
            {
                //user defined tab
                strURL = NavigateURL(_portalSettings.RegisterTabId, "", extraParams);
            }
            else
            {
                strURL = NavigateURL(_portalSettings.ActiveTab.TabID, "Register", extraParams);
            }
            return strURL;
        }
        public static string ResolveUrl(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return url;
            }
            if ((url.StartsWith("~") == false))
            {
                return url;
            }
            if ((url.Length == 1))
            {
                return Common.Globals.ApplicationPath;
            }
            if ((url.ToCharArray()[1] == '/' || url.ToCharArray()[1] == '\\'))
            {
                if (!string.IsNullOrEmpty(Common.Globals.ApplicationPath) && Common.Globals.ApplicationPath.Length > 1)
                {
                    return Common.Globals.ApplicationPath + "/" + url.Substring(2);
                }
                else
                {
                    return "/" + url.Substring(2);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Common.Globals.ApplicationPath) && Common.Globals.ApplicationPath.Length > 1)
                {
                    return Common.Globals.ApplicationPath + "/" + url.Substring(1);
                }
                else
                {
                    return Common.Globals.ApplicationPath + url.Substring(1);
                }
            }
        }
        public static string EncodeReservedCharacters(string QueryString)
        {
            QueryString = QueryString.Replace("$", "%24");
            QueryString = QueryString.Replace("&", "%26");
            QueryString = QueryString.Replace("+", "%2B");
            QueryString = QueryString.Replace(",", "%2C");
            QueryString = QueryString.Replace("/", "%2F");
            QueryString = QueryString.Replace(":", "%3A");
            QueryString = QueryString.Replace(";", "%3B");
            QueryString = QueryString.Replace("=", "%3D");
            QueryString = QueryString.Replace("?", "%3F");
            QueryString = QueryString.Replace("@", "%40");
            return QueryString;
        }
        public static string DateToString(DateTime DateValue)
        {
            try
            {
                if (!Null.IsNull(DateValue))
                {
                    return DateValue.ToString("s");
                }
                else
                {
                    return Null.NullString;
                }
            }
            catch (Exception ex)
            {
                return Null.NullString;
            }
        }
        public static string GetHashValue(object HashObject, string DefaultValue)
        {
            if (HashObject != null)
            {
                if (!String.IsNullOrEmpty(HashObject.ToString()))
                {
                    return Convert.ToString(HashObject);
                }
                else
                {
                    return DefaultValue;
                }
            }
            else
            {
                return DefaultValue;
            }
        }
        public static string LinkClick(string Link, int TabID, int ModuleID)
        {
            return LinkClick(Link, TabID, ModuleID, true, "");
        }
        public static string LinkClick(string Link, int TabID, int ModuleID, bool TrackClicks)
        {
            return LinkClick(Link, TabID, ModuleID, TrackClicks, "");
        }
        public static string LinkClick(string Link, int TabID, int ModuleID, bool TrackClicks, string ContentType)
        {
            return LinkClick(Link, TabID, ModuleID, TrackClicks, !String.IsNullOrEmpty(ContentType));
        }
        public static string LinkClick(string Link, int TabID, int ModuleID, bool TrackClicks, bool ForceDownload)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return LinkClick(Link, TabID, ModuleID, TrackClicks, ForceDownload, _portalSettings.PortalId,
                             _portalSettings.EnableUrlLanguage,
                             _portalSettings.GUID.ToString());
        }

        public static string LinkClick(string Link, int TabID, int ModuleID, bool TrackClicks, bool ForceDownload, int PortalId, bool EnableUrlLanguage, string portalGuid)
        {
            string strLink = "";
            TabType UrlType = GetURLType(Link);
            if (UrlType == TabType.Member)
            {
                strLink = UserProfileURL(Convert.ToInt32(UrlUtils.GetParameterValue(Link)));
            }
            else if (TrackClicks == true || ForceDownload == true || UrlType == TabType.File)
            {
                if (Link.ToLowerInvariant().StartsWith("fileid="))
                {
                    strLink = Common.Globals.ApplicationPath + "/LinkClick.aspx?fileticket=" + UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(Link), portalGuid);
                }
                if (String.IsNullOrEmpty(strLink))
                {
                    strLink = Common.Globals.ApplicationPath + "/LinkClick.aspx?link=" + HttpUtility.UrlEncode(Link);
                }
                if (TabID != Null.NullInteger)
                {
                    strLink += "&tabid=" + TabID.ToString();
                }
                if (ModuleID != -1)
                {
                    strLink += "&mid=" + ModuleID.ToString();
                }
                if (Localization.GetLocales(PortalId).Count > 1 && EnableUrlLanguage)
                {
                    strLink += "&language=" + Thread.CurrentThread.CurrentCulture.Name;
                }
                if (ForceDownload)
                {
                    strLink += "&forcedownload=true";
                }
            }
            else
            {
                switch (UrlType)
                {
                    case TabType.Tab:
                        strLink = NavigateURL(int.Parse(Link));
                        break;
                    default:
                        strLink = Link;
                        break;
                }
            }
            return strLink;
        }
        public static string GetRoleName(int RoleID)
        {
            if (Convert.ToString(RoleID) == glbRoleAllUsers)
            {
                return "All Users";
            }
            else if (Convert.ToString(RoleID) == glbRoleUnauthUser)
            {
                return "Unauthenticated Users";
            }
            Hashtable htRoles = null;
            if (Host.PerformanceSetting != Common.Globals.PerformanceSettings.NoCaching)
            {
                htRoles = (Hashtable)DataCache.GetCache("GetRoles");
            }
            if (htRoles == null)
            {
                RoleController objRoleController = new RoleController();
                ArrayList arrRoles;
                arrRoles = objRoleController.GetRoles();
                htRoles = new Hashtable();
                int i;
                for (i = 0; i <= arrRoles.Count - 1; i++)
                {
                    RoleInfo objRole;
                    objRole = (RoleInfo)arrRoles[i];
                    htRoles.Add(objRole.RoleID, objRole.RoleName);
                }
                if (Host.PerformanceSetting != Common.Globals.PerformanceSettings.NoCaching)
                {
                    DataCache.SetCache("GetRoles", htRoles);
                }
            }
            return Convert.ToString(htRoles[RoleID]);
        }
        public static XmlNode GetContent(string Content, string ContentType)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Content);
            if (String.IsNullOrEmpty(ContentType))
            {
                return xmlDoc.DocumentElement;
            }
            else
            {
                return xmlDoc.SelectSingleNode(ContentType);
            }
        }
        public static string GenerateTabPath(int ParentId, string TabName)
        {
            string strTabPath = "";
            TabController objTabs = new TabController();
            TabInfo objTab;
            string strTabName;
            objTab = objTabs.GetTab(ParentId, Null.NullInteger, false);
            while (objTab != null)
            {
                strTabName = HtmlUtils.StripNonWord(objTab.TabName, false);
                strTabPath = "//" + strTabName + strTabPath;
                if (Null.IsNull(objTab.ParentId))
                {
                    objTab = null;
                }
                else
                {
                    objTab = objTabs.GetTab(objTab.ParentId, objTab.PortalID, false);
                }
            }
            strTabPath = strTabPath + "//" + HtmlUtils.StripNonWord(TabName, false);
            return strTabPath;
        }
        public static string GetHelpText(int moduleControlId)
        {
            string helpText = Null.NullString;
            ModuleControlInfo objModuleControl = ModuleControlController.GetModuleControl(moduleControlId);
            if (objModuleControl != null)
            {
                string FileName = Path.GetFileName(objModuleControl.ControlSrc);
                string LocalResourceFile = objModuleControl.ControlSrc.Replace(FileName, Services.Localization.Localization.LocalResourceDirectory + "/" + FileName);
                if (!String.IsNullOrEmpty(Services.Localization.Localization.GetString(ModuleActionType.HelpText, LocalResourceFile)))
                {
                    helpText = Services.Localization.Localization.GetString(ModuleActionType.HelpText, LocalResourceFile);
                }
            }
            return helpText;
        }
        public static string GetOnLineHelp(string HelpUrl, ModuleInfo moduleConfig)
        {
            bool isAdminModule = moduleConfig.DesktopModule.IsAdmin;
            string ctlString = Convert.ToString(HttpContext.Current.Request.QueryString["ctl"]);
            if (((Host.EnableModuleOnLineHelp && !isAdminModule) || (isAdminModule)))
            {
                if ((isAdminModule) || (IsAdminControl() && ctlString == "Module") || (IsAdminControl() && ctlString == "Tab"))
                {
                    HelpUrl = Host.HelpURL;
                }
            }
            else
            {
                HelpUrl = "";
            }
            return HelpUrl;
        }
        public static bool ValidateLoginTabID(int tabId)
        {
            bool hasAccountModule = Null.NullBoolean;
            foreach (ModuleInfo objModule in new ModuleController().GetTabModules(tabId).Values)
            {
                if (objModule.ModuleDefinition.FriendlyName == "Account Login")
                {
                    TabInfo tab = new TabController().GetTab(tabId, objModule.PortalID, false);
                    if (TabPermissionController.CanViewPage(tab))
                    {
                        hasAccountModule = true;
                        break;
                    }
                }
            }
            return hasAccountModule;
        }
        private static bool FilenameMatchesExtensions(string filename, string strExtensions)
        {
            bool result = (strExtensions == string.Empty);
            if (!result)
            {
                filename = filename.ToUpper();
                strExtensions = strExtensions.ToUpper();
                foreach (string extension in strExtensions.Split(','))
                {
                    string ext = extension.Trim();
                    if (!ext.StartsWith("."))
                        ext = "." + extension;
                    result = filename.EndsWith(extension);
                    if (result)
                        break;
                }
            }
            return result;
        }
        public static Hashtable DeserializeHashTableBase64(string Source)
        {
            Hashtable objHashTable;
            if (!String.IsNullOrEmpty(Source))
            {
                byte[] bits = Convert.FromBase64String(Source);
                MemoryStream mem = new MemoryStream(bits);
                BinaryFormatter bin = new BinaryFormatter();
                try
                {
                    objHashTable = (Hashtable)bin.Deserialize(mem);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    objHashTable = new Hashtable();
                }
                mem.Close();
            }
            else
            {
                objHashTable = new Hashtable();
            }
            return objHashTable;
        }
        public static Hashtable DeserializeHashTableXml(string Source)
        {
            return XmlUtils.DeSerializeHashtable(Source, "profile");
        }
        public static string SerializeHashTableBase64(Hashtable Source)
        {
            string strString;
            if (Source.Count != 0)
            {
                BinaryFormatter bin = new BinaryFormatter();
                MemoryStream mem = new MemoryStream();
                try
                {
                    bin.Serialize(mem, Source);
                    strString = Convert.ToBase64String(mem.GetBuffer(), 0, Convert.ToInt32(mem.Length));
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    strString = "";
                }
                finally
                {
                    mem.Close();
                }
            }
            else
            {
                strString = "";
            }
            return strString;
        }
        public static string SerializeHashTableXml(Hashtable Source)
        {
            return XmlUtils.SerializeDictionary(Source, "profile");
        }

        /// <summary>
        /// Add:
        /// </summary>
        /// <returns></returns>
        public static bool IsDebugMode()
        {
            string mode = Config.GetSetting("DebugMode");
            if (!String.IsNullOrEmpty(mode) && mode.ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       
       
	}
}