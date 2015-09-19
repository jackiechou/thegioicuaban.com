using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLibrary.ComponentModel;
using System.Data.Common;

namespace CommonLibrary.Data
{
    public abstract class DataProvider
    {
        public static DataProvider Instance()
        {
            return ComponentFactory.GetComponent<DataProvider>();
        }
        public string DefaultProviderName
        {
            get { return Instance().ProviderName; }
        }
        public abstract string ConnectionString { get; }
        public abstract string DatabaseOwner { get; }
        public abstract string ObjectQualifier { get; }
        public abstract string ProviderName { get; }
        public abstract Dictionary<string, string> Settings { get; }
        public abstract void ExecuteNonQuery(string ProcedureName, params object[] commandParameters);
        public abstract IDataReader ExecuteReader(string ProcedureName, params object[] commandParameters);
        public abstract object ExecuteScalar(string ProcedureName, params object[] commandParameters);
        public abstract T ExecuteScalar<T>(string ProcedureName, params object[] commandParameters);
        public abstract DataSet ExecuteDataSet(string ProcedureName, params object[] commandParameters);
        public abstract IDataReader ExecuteSQL(string SQL);
        public abstract IDataReader ExecuteSQL(string SQL, params IDataParameter[] commandParameters);
        public abstract IDataReader ExecuteSQLTemp(string ConnectionString, string SQL);
        public abstract DbConnectionStringBuilder GetConnectionStringBuilder();
        public abstract object GetNull(object Field);
        public abstract void CommitTransaction(DbTransaction transaction);
        public abstract string ExecuteScript(string Script, DbTransaction transaction);
        public abstract DbTransaction GetTransaction();
        public abstract void RollbackTransaction(DbTransaction transaction);
        public abstract string GetProviderPath();
        public abstract string ExecuteScript(string SQL);
        public abstract string ExecuteScript(string SQL, bool UseTransactions);
        public abstract string ExecuteScript(string ConnectionString, string SQL);
        public abstract System.Version GetDatabaseEngineVersion();
        public abstract IDataReader GetDatabaseServer();
        public abstract IDataReader GetDatabaseVersion();
        public abstract System.Version GetVersion();
        public abstract string TestDatabaseConnection(DbConnectionStringBuilder builder, string Owner, string Qualifier);
        public abstract void UpdateDatabaseVersion(int Major, int Minor, int Build, string Name);
        public abstract IDataReader FindDatabaseVersion(int Major, int Minor, int Build);
        public abstract void UpgradeDatabaseSchema(int Major, int Minor, int Build);
        public abstract void AddHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, int createdByUserID);
        public abstract IDataReader GetHostSettings();
        public abstract IDataReader GetHostSetting(string SettingName);
        public abstract void UpdateHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, int lastModifiedByUserID);
        public abstract IDataReader GetServers();
        public abstract IDataReader GetServerConfiguration();
        public abstract void UpdateServer(int ServerId, string Url, bool Enabled);
        public abstract void DeleteServer(int ServerId);
        public abstract void UpdateServerActivity(string ServerName, string IISAppName, DateTime CreatedDate, DateTime LastActivityDate);
        public abstract int AddPortalInfo(string PortalName, string Currency, string FirstName, string LastName, string Username, string Password, string Email, System.DateTime ExpiryDate, double HostFee, double HostSpace,
        int PageQuota, int UserQuota, int SiteLogHistory, string HomeDirectory, int createdByUserID);
        public abstract int CreatePortal(string PortalName, string Currency, System.DateTime ExpiryDate, double HostFee, double HostSpace, int PageQuota, int UserQuota, int SiteLogHistory, string HomeDirectory, int CreatedByUserID);
        public abstract void DeletePortalInfo(int PortalId);
        public abstract void DeletePortalSetting(int PortalId, string SettingName, string CultureCode);
        public abstract void DeletePortalSettings(int PortalId);
        public abstract IDataReader GetExpiredPortals();
        public abstract IDataReader GetPortal(int PortalId, string CultureCode);
        public abstract IDataReader GetPortalByAlias(string PortalAlias);
        public abstract IDataReader GetPortalByTab(int TabId, string PortalAlias);
        public abstract int GetPortalCount();
        public abstract IDataReader GetPortals(string CultureCode);
        public abstract IDataReader GetPortals();
        public abstract IDataReader GetPortalsByName(string nameToMatch, int pageIndex, int pageSize);
        public abstract IDataReader GetPortalSettings(int PortalId, string CultureCode);
        public abstract IDataReader GetPortalSpaceUsed(int PortalId);
        public abstract void UpdatePortalInfo(int PortalId, string PortalName, string LogoFile, string FooterText, System.DateTime ExpiryDate, int UserRegistration, int BannerAdvertising, string Currency, int AdministratorId, double HostFee,
        double HostSpace, int PageQuota, int UserQuota, string PaymentProcessor, string ProcessorUserId, string ProcessorPassword, string Description, string KeyWords, string BackgroundFile, int SiteLogHistory,
        int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId, int UserTabId, string DefaultLanguage, int TimeZoneOffset, string HomeDirectory, int lastModifiedByUserID, string CultureCode);
        public abstract void UpdatePortalSetting(int PortalId, string SettingName, string SettingValue, int UserID, string CultureCode);
        public abstract void UpdatePortalSetup(int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId, int UserTabId, int AdminTabId, string CultureCode);
        public abstract IDataReader VerifyPortalTab(int PortalId, int TabId);
        public abstract IDataReader VerifyPortal(int PortalId);
        public abstract int AddTab(int ContentItemId, int PortalId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string IconFileLarge, string Title, string Description, string KeyWords,
        string Url, string SkinSrc, string ContainerSrc, string TabPath, System.DateTime StartDate, System.DateTime EndDate, int RefreshInterval, string PageHeadText, bool IsSecure, bool PermanentRedirect,
        float SiteMapPriority, int CreatedByUserID, string CultureCode);
        public abstract void UpdateTab(int TabId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string Title, string Description, string KeyWords, bool IsDeleted,
        string Url, string SkinSrc, string ContainerSrc, string TabPath, System.DateTime StartDate, System.DateTime EndDate, string CultureCode);
        public abstract void UpdateTab(int TabId, int ContentItemId, int PortalId, string TabName, bool IsVisible, bool DisableLink, int ParentId, string IconFile, string IconFileLarge, string Title, string Description,
        string KeyWords, bool IsDeleted, string Url, string SkinSrc, string ContainerSrc, string TabPath, System.DateTime StartDate, System.DateTime EndDate, int RefreshInterval, string PageHeadText,
        bool IsSecure, bool PermanentRedirect, float SiteMapPriority, int LastModifiedByuserID, string CultureCode);
        public abstract void UpdateTabOrder(int TabId, int TabOrder, int Level, int ParentId, string TabPath, int LastModifiedByUserID);
        public abstract void DeleteTab(int TabId);
        public abstract IDataReader GetTabs(int PortalId);
        public abstract IDataReader GetAllTabs();
        public abstract IDataReader GetTabPaths(int PortalId);
        public abstract IDataReader GetTab(int TabId);
        public abstract IDataReader GetTabByName(string TabName, int PortalId);
        public abstract IDataReader GetTabsByParentId(int ParentId);
        public abstract IDataReader GetTabsByModuleID(int moduleID);
        public abstract IDataReader GetTabsByPackageID(int portalID, int packageID, bool forHost);
        public abstract int GetTabCount(int PortalId);
        public abstract IDataReader GetPortalTabModules(int PortalId, int TabId);
        public abstract IDataReader GetTabModules(int TabId);
        public abstract IDataReader GetTabPanes(int TabId);
        public abstract IDataReader GetAllModules();
        public abstract IDataReader GetModules(int PortalId);
        public abstract IDataReader GetAllTabsModules(int PortalId, bool AllTabs);
        public abstract IDataReader GetModule(int ModuleId, int TabId);
        public abstract IDataReader GetModuleByDefinition(int PortalId, string FriendlyName);
        public abstract IDataReader GetSearchModules(int PortalId);
        public abstract int AddModule(int ContentItemId, int PortalID, int ModuleDefID, string ModuleTitle, bool AllTabs, string Header, string Footer, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted,
        int createdByUserID);
        public abstract void UpdateModule(int ModuleId, int ContentItemId, string ModuleTitle, bool AllTabs, string Header, string Footer, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted, int lastModifiedByUserID);
        public abstract void DeleteModule(int ModuleId);
        public abstract IDataReader GetTabModuleOrder(int TabId, string PaneName);
        public abstract void UpdateModuleOrder(int TabId, int ModuleId, int ModuleOrder, string PaneName);
        public abstract void AddTabModule(int TabId, int ModuleId, int ModuleOrder, string PaneName, int CacheTime, string CacheMethod, string Alignment, string Color, string Border, string IconFile,
        int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint, bool DisplaySyndicate, bool IsWebSlice, string WebSliceTitle, DateTime WebSliceExpiryDate, int WebSliceTTL, int createdByUserID);
        public abstract void DeleteTabModule(int TabId, int ModuleId, bool softDelete);
        public abstract void MoveTabModule(int fromTabId, int moduleId, int toTabId, string toPaneName, int lastModifiedByUserID);
        public abstract void RestoreTabModule(int TabId, int ModuleId);
        public abstract void UpdateTabModule(int TabId, int ModuleId, int ModuleOrder, string PaneName, int CacheTime, string CacheMethod, string Alignment, string Color, string Border, string IconFile,
        int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint, bool DisplaySyndicate, bool IsWebSlice, string WebSliceTitle, DateTime WebSliceExpiryDate, int WebSliceTTL, int lastModifiedByUserID);
        public abstract IDataReader GetModuleSettings(int ModuleId);
        public abstract IDataReader GetModuleSetting(int ModuleId, string SettingName);
        public abstract void AddModuleSetting(int ModuleId, string SettingName, string SettingValue, int createdByUserID);
        public abstract void UpdateModuleSetting(int ModuleId, string SettingName, string SettingValue, int lastModifiedByUserID);
        public abstract void DeleteModuleSetting(int ModuleId, string SettingName);
        public abstract void DeleteModuleSettings(int ModuleId);
        public abstract IDataReader GetTabSettings(int TabId);
        public abstract IDataReader GetTabSetting(int TabId, string SettingName);
        public abstract void AddTabSetting(int TabId, string SettingName, string SettingValue, int createdByUserID);
        public abstract void UpdateTabSetting(int TabId, string SettingName, string SettingValue, int lastModifiedByUserID);
        public abstract void DeleteTabSetting(int TabId, string SettingName);
        public abstract void DeleteTabSettings(int TabId);
        public abstract IDataReader GetTabModuleSettings(int TabModuleId);
        public abstract IDataReader GetTabModuleSetting(int TabModuleId, string SettingName);
        public abstract void AddTabModuleSetting(int TabModuleId, string SettingName, string SettingValue, int createdByUserID);
        public abstract void UpdateTabModuleSetting(int TabModuleId, string SettingName, string SettingValue, int lastModifiedByUserID);
        public abstract void DeleteTabModuleSetting(int TabModuleId, string SettingName);
        public abstract void DeleteTabModuleSettings(int TabModuleId);
        public abstract IDataReader GetDesktopModule(int DesktopModuleId);
        public abstract IDataReader GetDesktopModuleByFriendlyName(string FriendlyName);
        public abstract IDataReader GetDesktopModuleByModuleName(string ModuleName);
        public abstract IDataReader GetDesktopModuleByPackageID(int packageID);
        public abstract IDataReader GetDesktopModules();
        public abstract IDataReader GetDesktopModulesByPortal(int PortalID);
        public abstract int AddDesktopModule(int packageID, string ModuleName, string FolderName, string FriendlyName, string Description, string Version, bool IsPremium, bool IsAdmin, string BusinessControllerClass, int SupportedFeatures,
        string CompatibleVersions, string Dependencies, string Permissions, int createdByUserID);
        public abstract void UpdateDesktopModule(int DesktopModuleId, int packageID, string ModuleName, string FolderName, string FriendlyName, string Description, string Version, bool IsPremium, bool IsAdmin, string BusinessControllerClass,
        int SupportedFeatures, string CompatibleVersions, string Dependencies, string Permissions, int lastModifiedByUserID);
        public abstract void DeleteDesktopModule(int DesktopModuleId);
        public abstract IDataReader GetPortalDesktopModules(int PortalID, int DesktopModuleID);
        public abstract int AddPortalDesktopModule(int PortalID, int DesktopModuleID, int createdByUserID);
        public abstract void DeletePortalDesktopModules(int PortalID, int DesktopModuleID);
        public abstract IDataReader GetModuleDefinitions();
        public abstract IDataReader GetModuleDefinition(int ModuleDefId);
        public abstract IDataReader GetModuleDefinitionByName(int DesktopModuleId, string FriendlyName);
        public abstract int AddModuleDefinition(int DesktopModuleId, string FriendlyName, int DefaultCacheTime, int createdByUserID);
        public abstract void DeleteModuleDefinition(int ModuleDefId);
        public abstract void UpdateModuleDefinition(int ModuleDefId, string FriendlyName, int DefaultCacheTime, int lastModifiedByUserID);
        public abstract IDataReader GetModuleControls();
        public abstract IDataReader GetModuleControl(int ModuleControlId);
        public abstract IDataReader GetModuleControlsByKey(string ControlKey, int ModuleDefId);
        public abstract IDataReader GetModuleControlByKeyAndSrc(int ModuleDefID, string ControlKey, string ControlSrc);
        public abstract int AddModuleControl(int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl, bool SupportsPartialRendering, int createdByUserID);
        public abstract void UpdateModuleControl(int ModuleControlId, int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl, bool SupportsPartialRendering,
        int lastModifiedByUserID);
        public abstract void DeleteModuleControl(int ModuleControlId);
        public abstract int AddSkinControl(int packageID, string ControlKey, string ControlSrc, bool SupportsPartialRendering, int CreatedByUserID);
        public abstract void DeleteSkinControl(int skinControlID);
        public abstract IDataReader GetSkinControls();
        public abstract IDataReader GetSkinControl(int skinControlID);
        public abstract IDataReader GetSkinControlByKey(string controlKey);
        public abstract IDataReader GetSkinControlByPackageID(int packageID);
        public abstract void UpdateSkinControl(int skinControlID, int packageID, string ControlKey, string ControlSrc, bool SupportsPartialRendering, int LastModifiedByUserID);
        public abstract IDataReader GetFiles(int PortalId, int FolderID);
        public abstract IDataReader GetFile(string FileName, int PortalId, int FolderID);
        public abstract IDataReader GetFileById(int FileId, int PortalId);
        public abstract void DeleteFile(int PortalId, string FileName, int FolderID);
        public abstract void DeleteFiles(int PortalId);
        public abstract int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID);
        public abstract void UpdateFile(int FileId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID);
        public abstract DataTable GetAllFiles();
        public abstract IDataReader GetFileContent(int FileId, int PortalId);
        public abstract void UpdateFileContent(int FileId, byte[] StreamFile);
        public abstract void AddSiteLog(System.DateTime DateTime, int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId, int AffiliateId);
        public abstract IDataReader GetSiteLogReports();
        public abstract IDataReader GetSiteLog(int PortalId, string PortalAlias, string ReportName, System.DateTime StartDate, System.DateTime EndDate);
        public abstract void DeleteSiteLog(System.DateTime DateTime, int PortalId);
        public abstract IDataReader GetTables();
        public abstract IDataReader GetFields(string TableName);
        public abstract IDataReader GetVendors(int PortalId, bool UnAuthorized, int PageIndex, int PageSize);
        public abstract IDataReader GetVendorsByEmail(string Filter, int PortalId, int PageIndex, int PageSize);
        public abstract IDataReader GetVendorsByName(string Filter, int PortalId, int PageIndex, int PageSize);
        public abstract IDataReader GetVendor(int VendorID, int PortalID);
        public abstract void DeleteVendor(int VendorID);
        public abstract int AddVendor(int PortalID, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax,
        string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized);
        public abstract void UpdateVendor(int VendorID, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax,
        string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized);
        public abstract IDataReader GetVendorClassifications(int VendorId);
        public abstract void DeleteVendorClassifications(int VendorId);
        public abstract int AddVendorClassification(int VendorId, int ClassificationId);
        public abstract IDataReader GetBanners(int VendorId);
        public abstract IDataReader GetBanner(int BannerId, int VendorId, int PortalID);
        public abstract DataTable GetBannerGroups(int PortalId);
        public abstract void DeleteBanner(int BannerId);
        public abstract int AddBanner(string BannerName, int VendorId, string ImageFile, string URL, int Impressions, double CPM, System.DateTime StartDate, System.DateTime EndDate, string UserName, int BannerTypeId,
        string Description, string GroupName, int Criteria, int Width, int Height);
        public abstract void UpdateBanner(int BannerId, string BannerName, string ImageFile, string URL, int Impressions, double CPM, System.DateTime StartDate, System.DateTime EndDate, string UserName, int BannerTypeId,
        string Description, string GroupName, int Criteria, int Width, int Height);
        public abstract IDataReader FindBanners(int PortalId, int BannerTypeId, string GroupName);
        public abstract void UpdateBannerViews(int BannerId, System.DateTime StartDate, System.DateTime EndDate);
        public abstract void UpdateBannerClickThrough(int BannerId, int VendorId);
        public abstract IDataReader GetAffiliates(int VendorId);
        public abstract IDataReader GetAffiliate(int AffiliateId, int VendorId, int PortalID);
        public abstract void DeleteAffiliate(int AffiliateId);
        public abstract int AddAffiliate(int VendorId, System.DateTime StartDate, System.DateTime EndDate, double CPC, double CPA);
        public abstract void UpdateAffiliate(int AffiliateId, System.DateTime StartDate, System.DateTime EndDate, double CPC, double CPA);
        public abstract void UpdateAffiliateStats(int AffiliateId, int Clicks, int Acquisitions);
        public abstract bool CanDeleteSkin(string SkinType, string SkinFoldername);
        public abstract int AddSkin(int skinPackageID, string skinSrc);
        public abstract int AddSkinPackage(int packageID, int portalID, string skinName, string skinType, int CreatedByUserID);
        public abstract void DeleteSkin(int skinID);
        public abstract void DeleteSkinPackage(int skinPackageID);
        public abstract IDataReader GetSkinByPackageID(int packageID);
        public abstract IDataReader GetSkinPackage(int portalID, string skinName, string skinType);
        public abstract void UpdateSkin(int skinID, string skinSrc);
        public abstract void UpdateSkinPackage(int skinPackageID, int packageID, int portalID, string skinName, string skinType, int LastModifiedByUserID);
        public abstract IDataReader GetAllProfiles();
        public abstract IDataReader GetProfile(int UserId, int PortalId);
        public abstract void AddProfile(int UserId, int PortalId);
        public abstract void UpdateProfile(int UserId, int PortalId, string ProfileData);
        public abstract int AddPropertyDefinition(int PortalId, int ModuleDefId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required, string ValidationExpression, int ViewOrder, bool Visible,
        int Length, int CreatedByUserID);
        public abstract void DeletePropertyDefinition(int definitionId);
        public abstract IDataReader GetPropertyDefinition(int definitionId);
        public abstract IDataReader GetPropertyDefinitionByName(int portalId, string name);
        public abstract IDataReader GetPropertyDefinitionsByPortal(int portalId);
        public abstract void UpdatePropertyDefinition(int PropertyDefinitionId, int DataType, string DefaultValue, string PropertyCategory, string PropertyName, bool Required, string ValidationExpression, int ViewOrder, bool Visible, int Length,
        int LastModifiedByUserID);
        public abstract IDataReader GetUrls(int PortalID);
        public abstract IDataReader GetUrl(int PortalID, string Url);
        public abstract void AddUrl(int PortalID, string Url);
        public abstract void DeleteUrl(int PortalID, string Url);
        public abstract IDataReader GetUrlTracking(int PortalID, string Url, int ModuleId);
        public abstract void AddUrlTracking(int PortalID, string Url, string UrlType, bool LogActivity, bool TrackClicks, int ModuleId, bool NewWindow);
        public abstract void UpdateUrlTracking(int PortalID, string Url, bool LogActivity, bool TrackClicks, int ModuleId, bool NewWindow);
        public abstract void DeleteUrlTracking(int PortalID, string Url, int ModuleId);
        public abstract void UpdateUrlTrackingStats(int PortalID, string Url, int ModuleId);
        public abstract IDataReader GetUrlLog(int UrlTrackingID, System.DateTime StartDate, System.DateTime EndDate);
        public abstract void AddUrlLog(int UrlTrackingID, int UserID);
        public abstract IDataReader GetFoldersByPortal(int PortalID);
        public abstract IDataReader GetFolder(int PortalID, int FolderID);
        public abstract IDataReader GetFolder(int PortalID, string FolderPath);
        public abstract int AddFolder(int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, System.DateTime LastUpdated, int createdByUserID);
        public abstract void UpdateFolder(int PortalID, int FolderID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, System.DateTime LastUpdated, int lastModifiedByUserID);
        public abstract void DeleteFolder(int PortalID, string FolderPath);
        public abstract IDataReader GetPermission(int permissionID);
        public abstract IDataReader GetPermissionsByModuleDefID(int ModuleDefID);
        public abstract IDataReader GetPermissionsByModuleID(int ModuleID);
        public abstract IDataReader GetPermissionsByPortalDesktopModule();
        public abstract IDataReader GetPermissionsByFolder();
        public abstract IDataReader GetPermissionByCodeAndKey(string PermissionCode, string PermissionKey);
        public abstract IDataReader GetPermissionsByTab();
        public abstract void DeletePermission(int permissionID);
        public abstract int AddPermission(string permissionCode, int moduleDefID, string permissionKey, string permissionName, int createdByUserID);
        public abstract void UpdatePermission(int permissionID, string permissionCode, int moduleDefID, string permissionKey, string permissionName, int lastModifiedByUserID);
        public abstract IDataReader GetModulePermission(int modulePermissionID);
        public abstract IDataReader GetModulePermissionsByModuleID(int moduleID, int PermissionID);
        public abstract IDataReader GetModulePermissionsByPortal(int PortalID);
        public abstract IDataReader GetModulePermissionsByTabID(int TabID);
        public abstract void DeleteModulePermissionsByModuleID(int ModuleID);
        public abstract void DeleteModulePermissionsByUserID(int PortalID, int UserID);
        public abstract void DeleteModulePermission(int modulePermissionID);
        public abstract int AddModulePermission(int moduleID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID);
        public abstract void UpdateModulePermission(int modulePermissionID, int moduleID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID);
        public abstract IDataReader GetTabPermissionsByPortal(int PortalID);
        public abstract IDataReader GetTabPermissionsByTabID(int TabID, int PermissionID);
        public abstract void DeleteTabPermissionsByTabID(int TabID);
        public abstract void DeleteTabPermissionsByUserID(int PortalID, int UserID);
        public abstract void DeleteTabPermission(int TabPermissionID);
        public abstract int AddTabPermission(int TabID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID);
        public abstract void UpdateTabPermission(int TabPermissionID, int TabID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID);
        public abstract IDataReader GetFolderPermission(int FolderPermissionID);
        public abstract IDataReader GetFolderPermissionsByPortal(int PortalID);
        public abstract IDataReader GetFolderPermissionsByFolderPath(int PortalID, string FolderPath, int PermissionID);
        public abstract void DeleteFolderPermissionsByFolderPath(int PortalID, string FolderPath);
        public abstract void DeleteFolderPermissionsByUserID(int PortalID, int UserID);
        public abstract void DeleteFolderPermission(int FolderPermissionID);
        public abstract int AddFolderPermission(int FolderID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID);
        public abstract void UpdateFolderPermission(int FolderPermissionID, int FolderID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID);
        public abstract IDataReader GetDesktopModulePermission(int desktopModulePermissionID);
        public abstract IDataReader GetDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID);
        public abstract IDataReader GetDesktopModulePermissions();
        public abstract void DeleteDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID);
        public abstract void DeleteDesktopModulePermissionsByUserID(int userID);
        public abstract void DeleteDesktopModulePermission(int desktopModulePermissionID);
        public abstract int AddDesktopModulePermission(int portalDesktopModuleID, int permissionID, int roleID, bool allowAccess, int userID, int createdByUserID);
        public abstract void UpdateDesktopModulePermission(int desktopModulePermissionID, int portalDesktopModuleID, int permissionID, int roleID, bool allowAccess, int userID, int lastModifiedByUserID);
        public abstract IDataReader GetSearchIndexers();
        public abstract System.Data.IDataReader GetSearchResultModules(int PortalID);
        public abstract void DeleteSearchItems(int ModuleID);
        public abstract void DeleteSearchItem(int SearchItemId);
        public abstract void DeleteSearchItemWords(int SearchItemId);
        public abstract int AddSearchItem(string Title, string Description, int Author, System.DateTime PubDate, int ModuleId, string Key, string Guid, int ImageFileId);
        public abstract IDataReader GetSearchCommonWordsByLocale(string Locale);
        public abstract IDataReader GetDefaultLanguageByModule(string ModuleList);
        public abstract IDataReader GetSearchSettings(int ModuleId);
        public abstract IDataReader GetSearchWords();
        public abstract int AddSearchWord(string Word);
        public abstract int AddSearchItemWord(int SearchItemId, int SearchWordsID, int Occurrences);
        public abstract void AddSearchItemWordPosition(int SearchItemWordID, string ContentPositions);
        public abstract IDataReader GetSearchResults(int PortalID, string Word);
        public abstract IDataReader GetSearchItems(int PortalID, int TabID, int ModuleID);
        public abstract IDataReader GetSearchResults(int PortalID, int TabID, int ModuleID);
        public abstract IDataReader GetSearchItem(int ModuleID, string SearchKey);
        public abstract void UpdateSearchItem(int SearchItemId, string Title, string Description, int Author, System.DateTime PubDate, int ModuleId, string Key, string Guid, int HitCount, int ImageFileId);
        public abstract IDataReader GetLists(int PortalID);
        public abstract IDataReader GetList(string ListName, string ParentKey, int PortalID);
        public abstract IDataReader GetListEntry(int EntryID);
        public abstract IDataReader GetListEntry(string ListName, string Value);
        public abstract IDataReader GetListEntriesByListName(string ListName, string ParentKey, int PortalID);
        public abstract int AddListEntry(string ListName, string Value, string Text, int ParentID, int Level, bool EnableSortOrder, int DefinitionID, string Description, int PortalID, bool SystemList, int CreatedByUserID);
        public abstract void UpdateListEntry(int EntryID, string Value, string Text, string Description, int LastModifiedByUserID);
        public abstract void DeleteListEntryByID(int EntryID, bool DeleteChild);
        public abstract void DeleteList(string ListName, string ParentKey);
        public abstract void DeleteListEntryByListName(string ListName, string Value, bool DeleteChild);
        public abstract void UpdateListSortOrder(int EntryID, bool MoveUp);
        public abstract IDataReader GetPortalAlias(string PortalAlias, int PortalID);
        public abstract IDataReader GetPortalAliasByPortalID(int PortalID);
        public abstract IDataReader GetPortalAliasByPortalAliasID(int PortalAliasID);
        public abstract IDataReader GetPortalByPortalAliasID(int PortalAliasId);
        public abstract void UpdatePortalAlias(string PortalAlias, int lastModifiedByUserID);
        public abstract void UpdatePortalAliasInfo(int PortalAliasID, int PortalID, string HTTPAlias, int lastModifiedByUserID);
        public abstract int AddPortalAlias(int PortalID, string HTTPAlias, int createdByUserID);
        public abstract void DeletePortalAlias(int PortalAliasID);
        public abstract int AddEventMessage(string eventName, int priority, string processorType, string processorCommand, string body, string sender, string subscriberId, string authorizedRoles, string exceptionMessage, System.DateTime sentDate,
        System.DateTime expirationDate, string attributes);
        public abstract IDataReader GetEventMessages(string eventName);
        public abstract IDataReader GetEventMessagesBySubscriber(string eventName, string subscriberId);
        public abstract void SetEventMessageComplete(int eventMessageId);
        public abstract int AddAuthentication(int packageID, string authenticationType, bool isEnabled, string settingsControlSrc, string loginControlSrc, string logoffControlSrc, int CreatedByUserID);
        public abstract int AddUserAuthentication(int userID, string authenticationType, string authenticationToken, int CreatedByUserID);
        public abstract void DeleteAuthentication(int authenticationID);
        public abstract IDataReader GetAuthenticationService(int authenticationID);
        public abstract IDataReader GetAuthenticationServiceByPackageID(int packageID);
        public abstract IDataReader GetAuthenticationServiceByType(string authenticationType);
        public abstract IDataReader GetAuthenticationServices();
        public abstract IDataReader GetEnabledAuthenticationServices();
        public abstract void UpdateAuthentication(int authenticationID, int packageID, string authenticationType, bool isEnabled, string settingsControlSrc, string loginControlSrc, string logoffControlSrc, int LastModifiedByUserID);
        public abstract int AddPackage(int portalID, string name, string friendlyName, string description, string type, string version, string license, string manifest, string owner, string organization,
        string url, string email, string releaseNotes, bool isSystemPackage, int CreatedByUserID);
        public abstract void DeletePackage(int packageID);
        public abstract IDataReader GetPackage(int packageID);
        public abstract IDataReader GetPackageByName(int portalID, string name);
        public abstract IDataReader GetPackages(int portalID);
        public abstract IDataReader GetPackagesByType(int portalID, string type);
        public abstract IDataReader GetPackageType(string type);
        public abstract IDataReader GetPackageTypes();
        public abstract IDataReader GetModulePackagesInUse(int portalID, bool forHost);
        public abstract int RegisterAssembly(int packageID, string assemblyName, string version);
        public abstract bool UnRegisterAssembly(int packageID, string assemblyName);
        public abstract void UpdatePackage(int portalID, string name, string friendlyName, string description, string type, string version, string license, string manifest, string owner, string organization,
        string url, string email, string releaseNotes, bool isSystemPackage, int LastModifiedByUserID);
        public abstract int AddLanguage(string cultureCode, string cultureName, string fallbackCulture, int CreatedByUserID);
        public abstract void DeleteLanguage(int languageID);
        public abstract IDataReader GetLanguages();
        public abstract void UpdateLanguage(int languageID, string cultureCode, string cultureName, string fallbackCulture, int LastModifiedByUserID);
        public abstract int AddPortalLanguage(int portalID, int languageID, int CreatedByUserID);
        public abstract void DeletePortalLanguages(int portalID, int languageID);
        public abstract IDataReader GetLanguagesByPortal(int portalID);
        public abstract int AddLanguagePack(int packageID, int languageID, int dependentPackageID, int CreatedByUserID);
        public abstract void DeleteLanguagePack(int languagePackID);
        public abstract IDataReader GetLanguagePackByPackage(int packageID);
        public abstract int UpdateLanguagePack(int languagePackID, int packageID, int languageID, int dependentPackageID, int LastModifiedByUserID);
        //localisation
        public abstract string GetPortalDefaultLanguage(int portalID);
        public abstract void UpdatePortalDefaultLanguage(int portalID, string CultureCode);
        public abstract void EnsureLocalizationExists(int portalID, string CultureCode);
    }
}
