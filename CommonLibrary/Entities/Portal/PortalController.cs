using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Localization;
using System.Data;
using CommonLibrary.Common;
using CommonLibrary.Common.Utilities;
using System.Web;
using System.Collections;
using CommonLibrary.Data;
using System.Xml.XPath;
using CommonLibrary.Entities.Users;
using System.Threading;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Security.Roles;
using CommonLibrary.Services.FileSystem;
using System.Xml;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Profile;
using CommonLibrary.Common.Lists;
using CommonLibrary.Security.Permissions;
using System.IO;
using CommonLibrary.Security.Membership;
using ICSharpCode.SharpZipLib.Zip;
using System.Data.SqlClient;
using CommonLibrary.Modules.Dashboard.Components;

namespace CommonLibrary.Entities.Portal
{
    public class PortalController
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        private static object GetPortalCallback(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            string cultureCode = (string)cacheItemArgs.ParamList[1];
            object objPortal = null;
            if (Localization.ActiveLanguagesByPortalID(portalID) == 1)
            {
                //only 1 language active, no need for fallback check
                return CBO.FillObject<PortalInfo>(DataProvider.Instance().GetPortal(portalID, cultureCode));
            }
            else
            {
                System.Data.IDataReader dr = default(System.Data.IDataReader);
                dr = DataProvider.Instance().GetPortal(portalID, cultureCode);
                objPortal = CBO.FillObject<PortalInfo>(dr);
                if (objPortal == null)
                {
                    //Get Fallback language
                    string fallbackLanguage = string.Empty;
                    Locale userLocale = Localization.GetLocale(cultureCode);
                    if (userLocale != null && !string.IsNullOrEmpty(userLocale.Fallback))
                    {
                        fallbackLanguage = userLocale.Fallback;
                    }
                    dr = DataProvider.Instance().GetPortal(portalID, fallbackLanguage);
                    objPortal = CBO.FillObject<PortalInfo>(dr);
                    if (objPortal == null)
                    {
                        objPortal = CBO.FillObject<PortalInfo>(DataProvider.Instance().GetPortal(portalID, PortalController.GetActivePortalLanguage(portalID)));
                    }
                    //if we cannot find any fallback, it mean's it's a non portal default langauge
                    DataProvider.Instance().EnsureLocalizationExists(portalID, PortalController.GetActivePortalLanguage(portalID));
                    objPortal = CBO.FillObject<PortalInfo>(DataProvider.Instance().GetPortal(portalID, PortalController.GetActivePortalLanguage(portalID)));
                    dr.Close();
                    dr.Dispose();
                }
            }
            return objPortal;
        }
        private static object GetPortalDictionaryCallback(CacheItemArgs cacheItemArgs)
        {
            Dictionary<int, int> portalDic = new Dictionary<int, int>();
            if (Host.Host.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
            {
                int intField = 0;
                IDataReader dr = DataProvider.Instance().GetTabPaths(Null.NullInteger);
                try
                {
                    while (dr.Read())
                    {
                        portalDic[Convert.ToInt32(Null.SetNull(dr["TabID"], intField))] = Convert.ToInt32(Null.SetNull(dr["PortalID"], intField));
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.LogException(exc);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
            }
            return portalDic;
        }
        private static object GetPortalSettingsDictionaryCallback(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            Dictionary<string, string> dicSettings = new Dictionary<string, string>();
            IDataReader dr = DataProvider.Instance().GetPortalSettings(portalID, PortalController.GetActivePortalLanguage(portalID));
            try
            {
                while (dr.Read())
                {
                    if (!dr.IsDBNull(1))
                    {
                        dicSettings.Add(dr.GetString(0), dr.GetString(1));
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return dicSettings;
        }
        public static void AddPortalDictionary(int portalId, int tabId)
        {
            Dictionary<int, int> portalDic = GetPortalDictionary();
            portalDic[tabId] = portalId;
            DataCache.SetCache(DataCache.PortalDictionaryCacheKey, portalDic);
        }
        public static void DeleteExpiredPortals(string serverPath)
        {
            foreach (PortalInfo portal in GetExpiredPortals())
            {
                DeletePortal(portal, serverPath);
            }
        }
        public static bool IsChildPortal(PortalInfo portal, string serverPath)
        {
            bool isChild = Null.NullBoolean;
            string portalName;
            PortalAliasController aliasController = new PortalAliasController();
            ArrayList arr = aliasController.GetPortalAliasArrayByPortalID(portal.PortalID);
            if (arr.Count > 0)
            {
                PortalAliasInfo portalAlias = (PortalAliasInfo)arr[0];
                portalName = Globals.GetPortalDomainName(portalAlias.HTTPAlias, null, true);
                if (portalAlias.HTTPAlias.IndexOf("/") > -1)
                {
                    portalName = portalAlias.HTTPAlias.Substring(portalAlias.HTTPAlias.LastIndexOf("/") + 1);
                }
                if (!String.IsNullOrEmpty(portalName) && System.IO.Directory.Exists(serverPath + portalName))
                {
                    isChild = true;
                }
            }
            return isChild;
        }
        public static string DeletePortal(PortalInfo portal, string serverPath)
        {
            string strPortalName;
            string strMessage = string.Empty;
            int portalCount = DataProvider.Instance().GetPortalCount();
            if (portalCount > 1)
            {
                if (portal != null)
                {
                    Globals.DeleteFilesRecursive(serverPath, ".Portal-" + portal.PortalID.ToString() + ".resx");
                    PortalAliasController objPortalAliasController = new PortalAliasController();
                    ArrayList arr = objPortalAliasController.GetPortalAliasArrayByPortalID(portal.PortalID);
                    if (arr.Count > 0)
                    {
                        PortalAliasInfo objPortalAliasInfo = (PortalAliasInfo)arr[0];
                        strPortalName = Globals.GetPortalDomainName(objPortalAliasInfo.HTTPAlias, null, true);
                        if (objPortalAliasInfo.HTTPAlias.IndexOf("/") > -1)
                        {
                            strPortalName = objPortalAliasInfo.HTTPAlias.Substring(objPortalAliasInfo.HTTPAlias.LastIndexOf("/") + 1);
                        }
                        if (!String.IsNullOrEmpty(strPortalName) && System.IO.Directory.Exists(serverPath + strPortalName))
                        {
                            Globals.DeleteFolderRecursive(serverPath + strPortalName);
                        }
                    }
                    Globals.DeleteFolderRecursive(serverPath + "Portals\\" + portal.PortalID.ToString());
                    if (!string.IsNullOrEmpty(portal.HomeDirectory))
                    {
                        string HomeDirectory = portal.HomeDirectoryMapPath;
                        if (System.IO.Directory.Exists(HomeDirectory))
                        {
                            Globals.DeleteFolderRecursive(HomeDirectory);
                        }
                    }
                    PortalController objPortalController = new PortalController();
                    objPortalController.DeletePortalInfo(portal.PortalID);
                }
            }
            else
            {
                strMessage = Localization.GetString("LastPortal");
            }
            return strMessage;
        }
        public static Dictionary<int, int> GetPortalDictionary()
        {
            string cacheKey = string.Format(DataCache.PortalDictionaryCacheKey);
            return CBO.GetCachedObject<Dictionary<int, int>>(new CacheItemArgs(cacheKey, DataCache.PortalDictionaryTimeOut, DataCache.PortalDictionaryCachePriority), GetPortalDictionaryCallback);
        }
        public static ArrayList GetPortalsByName(string nameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            if (pageIndex == -1)
            {
                pageIndex = 0;
                pageSize = int.MaxValue;
            }
            Type type = typeof(PortalInfo);
            return CBO.FillCollection(DataProvider.Instance().GetPortalsByName(nameToMatch, pageIndex, pageSize), ref type, ref totalRecords);
        }
        public static PortalSettings GetCurrentPortalSettings()
        {
            PortalSettings objPortalSettings = null;
            if (HttpContext.Current != null)
            {
                objPortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            }
            return objPortalSettings;
        }
        public static ArrayList GetExpiredPortals()
        {
            return CBO.FillCollection(DataProvider.Instance().GetExpiredPortals(), typeof(PortalInfo));
        }
        public static void DeletePortalSetting(int portalID, string settingName)
        {
            DeletePortalSetting(portalID, settingName, PortalController.GetActivePortalLanguage(portalID));
        }
        public static void DeletePortalSetting(int portalID, string settingName, string CultureCode)
        {
            DataProvider.Instance().DeletePortalSetting(portalID, settingName, CultureCode.ToString().ToLower());
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("SettingName", settingName.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_DELETED);
            DataCache.ClearPortalCache(portalID, false);
        }
        public static void DeletePortalSettings(int portalID)
        {
            DataProvider.Instance().DeletePortalSettings(portalID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("PortalID", portalID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_DELETED);
            DataCache.ClearPortalCache(portalID, false);
        }
        public static Dictionary<string, string> GetPortalSettingsDictionary(int portalID)
        {
            string cacheKey = string.Format(DataCache.PortalSettingsCacheKey, portalID.ToString());
            return CBO.GetCachedObject<Dictionary<string, string>>(new CacheItemArgs(cacheKey, DataCache.PortalSettingsCacheTimeOut, DataCache.PortalSettingsCachePriority, portalID), GetPortalSettingsDictionaryCallback);
        }
        public static string GetPortalSetting(string settingName, int portalID, string defaultValue)
        {
            string retValue = Null.NullString;
            try
            {
                string setting = Null.NullString;
                PortalController.GetPortalSettingsDictionary(portalID).TryGetValue(settingName, out setting);
                if (string.IsNullOrEmpty(setting))
                {
                    retValue = defaultValue;
                }
                else
                {
                    retValue = setting;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return retValue;
        }
        public static bool GetPortalSettingAsBoolean(string key, int portalID, bool defaultValue)
        {
            bool retValue = Null.NullBoolean;
            try
            {
                string setting = Null.NullString;
                PortalController.GetPortalSettingsDictionary(portalID).TryGetValue(key, out setting);
                if (string.IsNullOrEmpty(setting))
                {
                    retValue = defaultValue;
                }
                else
                {
                    retValue = (setting.ToUpperInvariant().StartsWith("Y") || setting.ToUpperInvariant() == "TRUE");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return retValue;
        }
        public static int GetPortalSettingAsInteger(string key, int portalID, int defaultValue)
        {
            int retValue = Null.NullInteger;
            try
            {
                string setting = Null.NullString;
                PortalController.GetPortalSettingsDictionary(portalID).TryGetValue(key, out setting);
                if (string.IsNullOrEmpty(setting))
                {
                    retValue = defaultValue;
                }
                else
                {
                    retValue = Convert.ToInt32(setting);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return retValue;
        }
        public static void UpdatePortalSetting(int portalID, string settingName, string settingValue)
        {
            UpdatePortalSetting(portalID, settingName, settingValue, true);
        }
        public static void UpdatePortalSetting(int portalID, string settingName, string settingValue, bool clearCache)
        {
            string culture = Thread.CurrentThread.CurrentCulture.ToString().ToLower();
            if ((string.IsNullOrEmpty(culture)))
            {
                culture = GetPortalSetting("DefaultLanguage", portalID, "".ToLower());
            }
            if ((string.IsNullOrEmpty(culture)))
            {
                culture = Localization.SystemLocale.ToLower();
            }
            UpdatePortalSetting(portalID, settingName, settingValue, clearCache, culture);
        }

        public static void UpdatePortalSetting(int portalID, string settingName, string settingValue, bool clearCache, string culturecode)
        {
            DataProvider.Instance().UpdatePortalSetting(portalID, settingName, settingValue, UserController.GetCurrentUserInfo().UserID, culturecode);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(settingName.ToString(), settingValue.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
            if (clearCache)
            {
                DataCache.ClearPortalCache(portalID, false);
            }
        }
        public static string CheckDesktopModulesInstalled(XPathNavigator nav)
        {
            string friendlyName = Null.NullString;
            DesktopModuleInfo desktopModule = null;
            StringBuilder modulesNotInstalled = new StringBuilder();

            foreach (XPathNavigator desktopModuleNav in nav.Select("portalDesktopModule"))
            {
                friendlyName = XmlUtils.GetNodeValue(desktopModuleNav, "friendlyname");

                if (!string.IsNullOrEmpty(friendlyName))
                {
                    desktopModule = DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName);
                    if (desktopModule == null)
                    {
                        modulesNotInstalled.Append(friendlyName);
                        modulesNotInstalled.Append("<br/>");
                    }
                }
            }
            return modulesNotInstalled.ToString();
        }

        ///// <summary>
        ///// function provides the language for portalinfo requests
        ///// in case where language has not been installed yet, will return the core install default of en-us
        ///// </summary>
        ///// <param name="portalID"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        public static string GetActivePortalLanguage(int portalID)
        {
            // get Language
            string Language = "en-US";
            string tmpLanguage = GetPortalDefaultLanguage(portalID);
            if (!String.IsNullOrEmpty(tmpLanguage))
            {
                Language = tmpLanguage;
            }
            //handles case where portalcontroller methods invoked before a language is installed
            if (Globals.Status == Globals.UpgradeStatus.None && Localization.ActiveLanguagesByPortalID(portalID) == 1)
            {
                return GetPortalDefaultLanguage(portalID);
            }
            if (HttpContext.Current != null && Globals.Status == Globals.UpgradeStatus.None)
            {
                if ((HttpContext.Current.Request.QueryString["language"] != null))
                {
                    Language = HttpContext.Current.Request.QueryString["language"];
                }
                else
                {
                    if ((HttpContext.Current.Request.Cookies["language"] != null))
                    {
                        Language = HttpContext.Current.Request.Cookies["language"].Value;
                    }
                }
            }
            return Language;
        }
        ///// <summary>
        ///// return the current DefaultLanguage value from the Portals table for the requested Portalid
        ///// </summary>
        ///// <param name="portalID"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        public static string GetPortalDefaultLanguage(int portalID)
        {
            return DataProvider.Instance().GetPortalDefaultLanguage(portalID);
        }
        ///// <summary>
        ///// set the required DefaultLanguage in the Portals table for a particular portal
        ///// saves having to update an entire PortalInfo object
        ///// </summary>
        ///// <param name="portalID"></param>
        ///// <param name="CultureCode"></param>
        ///// <remarks></remarks>
        public static void UpdatePortalDefaultLanguage(int portalID, string CultureCode)
        {
            DataProvider.Instance().UpdatePortalDefaultLanguage(portalID, CultureCode);
            //ensure localization record exists as new portal default language may be relying on fallback chain
            //of which it is now the final part
            DataProvider.Instance().EnsureLocalizationExists(portalID, CultureCode);

        }

        private void CreateDefaultPortalRoles(int portalId, int administratorId, int administratorRoleId, int registeredRoleId, int subscriberRoleId)
        {
            RoleController controller = new RoleController();
            if (administratorRoleId == -1)
            {
                administratorRoleId = CreateRole(portalId, "Administrators", "Portal Administrators", 0, 0, "M", 0, 0, "N", false,
                false);
            }
            if (registeredRoleId == -1)
            {
                registeredRoleId = CreateRole(portalId, "Registered Users", "Registered Users", 0, 0, "M", 0, 0, "N", false,
                true);
            }
            if (subscriberRoleId == -1)
            {
                subscriberRoleId = CreateRole(portalId, "Subscribers", "A public role for portal subscriptions", 0, 0, "M", 0, 0, "N", true,
                true);
            }
            controller.AddUserRole(portalId, administratorId, administratorRoleId, Null.NullDate, Null.NullDate);
            controller.AddUserRole(portalId, administratorId, registeredRoleId, Null.NullDate, Null.NullDate);
            controller.AddUserRole(portalId, administratorId, subscriberRoleId, Null.NullDate, Null.NullDate);
        }
        private int CreateRole(RoleInfo role)
        {
            RoleInfo objRoleInfo;
            RoleController objRoleController = new RoleController();
            int roleId = Null.NullInteger;
            objRoleInfo = objRoleController.GetRoleByName(role.PortalID, role.RoleName);
            if (objRoleInfo == null)
            {
                roleId = objRoleController.AddRole(role);
            }
            else
            {
                roleId = objRoleInfo.RoleID;
            }
            return roleId;
        }
        private int CreateRole(int portalId, string roleName, string description, float serviceFee, int billingPeriod, string billingFrequency, float trialFee, int trialPeriod, string trialFrequency, bool isPublic,
        bool isAuto)
        {
            RoleInfo objRoleInfo = new RoleInfo();
            objRoleInfo.PortalID = portalId;
            objRoleInfo.RoleName = roleName;
            objRoleInfo.RoleGroupID = Null.NullInteger;
            objRoleInfo.Description = description;
            objRoleInfo.ServiceFee = Convert.ToSingle(serviceFee < 0 ? 0 : serviceFee);
            objRoleInfo.BillingPeriod = billingPeriod;
            objRoleInfo.BillingFrequency = billingFrequency;
            objRoleInfo.TrialFee = Convert.ToSingle(trialFee < 0 ? 0 : trialFee);
            objRoleInfo.TrialPeriod = trialPeriod;
            objRoleInfo.TrialFrequency = trialFrequency;
            objRoleInfo.IsPublic = isPublic;
            objRoleInfo.AutoAssignment = isAuto;
            return CreateRole(objRoleInfo);
        }
        private void CreateRoleGroup(RoleGroupInfo roleGroup)
        {
            RoleGroupInfo objRoleGroupInfo;
            RoleController objRoleController = new RoleController();
            int roleGroupId = Null.NullInteger;
            objRoleGroupInfo = RoleController.GetRoleGroupByName(roleGroup.PortalID, roleGroup.RoleGroupName);
            if (objRoleGroupInfo == null)
            {
                roleGroup.RoleGroupID = RoleController.AddRoleGroup(roleGroup);
            }
            else
            {
                roleGroup.RoleGroupID = objRoleGroupInfo.RoleGroupID;
            }
        }
        private void ParseRoles(XPathNavigator nav, int portalID, int administratorId)
        {
            int administratorRoleId = -1;
            int registeredRoleId = -1;
            int subscriberRoleId = -1;
            RoleController controller = new RoleController();
            foreach (XPathNavigator roleNav in nav.Select("role"))
            {
                RoleInfo role = CBO.DeserializeObject<RoleInfo>(new StringReader(roleNav.OuterXml));
                role.PortalID = portalID;
                role.RoleGroupID = Null.NullInteger;
                switch (role.RoleType)
                {
                    case RoleType.Administrator:
                        administratorRoleId = CreateRole(role);
                        break;
                    case RoleType.RegisteredUser:
                        registeredRoleId = CreateRole(role);
                        break;
                    case RoleType.Subscriber:
                        subscriberRoleId = CreateRole(role);
                        break;
                    case RoleType.None:
                        CreateRole(role);
                        break;
                }
            }
            CreateDefaultPortalRoles(portalID, administratorId, administratorRoleId, registeredRoleId, subscriberRoleId);
            PortalInfo objportal;
            objportal = GetPortal(portalID);
            UpdatePortalSetup(portalID, administratorId, administratorRoleId, registeredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(portalID));
        }
        private void ParseRoleGroups(XPathNavigator nav, int portalID, int administratorId)
        {
            int administratorRoleId = -1;
            int registeredRoleId = -1;
            int subscriberRoleId = -1;
            RoleController controller = new RoleController();
            foreach (XPathNavigator roleGroupNav in nav.Select("rolegroup"))
            {
                RoleGroupInfo roleGroup = CBO.DeserializeObject<RoleGroupInfo>(new StringReader(roleGroupNav.OuterXml));
                if (roleGroup.RoleGroupName != "GlobalRoles")
                {
                    roleGroup.PortalID = portalID;
                    CreateRoleGroup(roleGroup);
                }
                foreach (RoleInfo role in roleGroup.Roles.Values)
                {
                    role.PortalID = portalID;
                    role.RoleGroupID = roleGroup.RoleGroupID;
                    switch (role.RoleType)
                    {
                        case RoleType.Administrator:
                            administratorRoleId = CreateRole(role);
                            break;
                        case RoleType.RegisteredUser:
                            registeredRoleId = CreateRole(role);
                            break;
                        case RoleType.Subscriber:
                            subscriberRoleId = CreateRole(role);
                            break;
                        case RoleType.None:
                            CreateRole(role);
                            break;
                    }
                }
            }
            CreateDefaultPortalRoles(portalID, administratorId, administratorRoleId, registeredRoleId, subscriberRoleId);
            PortalInfo objportal;
            objportal = GetPortal(portalID);
            UpdatePortalSetup(portalID, administratorId, administratorRoleId, registeredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(portalID));
        }
        private void AddFolderPermissions(int PortalId, int folderId)
        {
            PortalInfo objPortal = GetPortal(PortalId);
            FolderController objController = new FolderController();
            FolderPermissionInfo objFolderPermission;
            FolderInfo folder = objController.GetFolderInfo(PortalId, folderId);
            PermissionController objPermissionController = new PermissionController();
            foreach (PermissionInfo objpermission in objPermissionController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", ""))
            {
                objFolderPermission = new FolderPermissionInfo(objpermission);
                objFolderPermission.FolderID = folder.FolderID;
                objFolderPermission.RoleID = objPortal.AdministratorRoleId;
                folder.FolderPermissions.Add(objFolderPermission);
                if (objpermission.PermissionKey == "READ")
                {
                    objFolderPermission = new FolderPermissionInfo(objpermission);
                    objFolderPermission.FolderID = folder.FolderID;
                    objFolderPermission.RoleID = int.Parse(Globals.glbRoleAllUsers);
                    folder.FolderPermissions.Add(objFolderPermission);
                }
            }
            FolderPermissionController.SaveFolderPermissions(folder);
        }
        private string CreateProfileDefinitions(int PortalId, string TemplatePath, string TemplateFile)
        {
            string strMessage = Null.NullString;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode node;
                try
                {
                    xmlDoc.Load(TemplatePath + TemplateFile);
                }
                catch
                {
                }
                node = xmlDoc.SelectSingleNode("//portal/profiledefinitions");
                if (node != null)
                {
                    ParseProfileDefinitions(node, PortalId);
                }
                else
                {
                    ProfileController.AddDefaultDefinitions(PortalId);
                }
            }
            catch (Exception ex)
            {
                strMessage = Localization.GetString("CreateProfileDefinitions.Error") + ex.ToString();                 
            }
            return strMessage;
        }
        private int CreatePortal(string PortalName, string HomeDirectory)
        {
            int PortalId = -1;
            try
            {
                System.DateTime datExpiryDate;
                if (Host.Host.DemoPeriod > Null.NullInteger)
                {
                    datExpiryDate = Convert.ToDateTime(Globals.GetMediumDate(DateTime.Now.AddDays(Host.Host.DemoPeriod).ToString()));
                }
                else
                {
                    datExpiryDate = Null.NullDate;
                }
                PortalId = DataProvider.Instance().CreatePortal(PortalName, Host.Host.HostCurrency, datExpiryDate, Host.Host.HostFee, Host.Host.HostSpace, Host.Host.PageQuota, Host.Host.UserQuota, Host.Host.SiteLogHistory, HomeDirectory, UserController.GetCurrentUserInfo().UserID);
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog("PortalName", PortalName.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_CREATED);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            return PortalId;
        }
        private void ParseFiles(XmlNodeList nodeFiles, int PortalId, FolderInfo objFolder)
        {
            int FileId;
            FileController objController = new FileController();
            CommonLibrary.Services.FileSystem.FileInfo objInfo;
            string fileName;
            foreach (XmlNode node in nodeFiles)
            {
                fileName = XmlUtils.GetNodeValue(node.CreateNavigator(), "filename");
                objInfo = objController.GetFile(fileName, PortalId, objFolder.FolderID);
                if (objInfo == null)
                {
                    objInfo = new CommonLibrary.Services.FileSystem.FileInfo();
                    objInfo.PortalId = PortalId;
                    objInfo.FileName = fileName;
                    objInfo.Extension = XmlUtils.GetNodeValue(node.CreateNavigator(), "extension");
                    objInfo.Size = XmlUtils.GetNodeValueInt(node, "size");
                    objInfo.Width = XmlUtils.GetNodeValueInt(node, "width");
                    objInfo.Height = XmlUtils.GetNodeValueInt(node, "height");
                    objInfo.ContentType = XmlUtils.GetNodeValue(node.CreateNavigator(), "contenttype");
                    objInfo.FolderId = objFolder.FolderID;
                    objInfo.Folder = objFolder.FolderPath;
                    FileId = objController.AddFile(objInfo);
                }
                else
                {
                    FileId = objInfo.FileId;
                }
            }
        }
        private void ParseFolders(XmlNode nodeFolders, int PortalId)
        {

            int FolderId;
            FolderController objController = new FolderController();
            FolderInfo objInfo;
            string folderPath;
            int storageLocation;
            bool isProtected = false;
            foreach (XmlNode node in nodeFolders.SelectNodes("//folder"))
            {
                folderPath = XmlUtils.GetNodeValue(node.CreateNavigator(), "folderpath");
                objInfo = objController.GetFolder(PortalId, folderPath, false);
                if (objInfo == null)
                {
                    isProtected = FileSystemUtils.DefaultProtectedFolders(folderPath);
                    if (isProtected == true)
                    {
                        storageLocation = (int)FolderController.StorageLocationTypes.InsecureFileSystem;
                    }
                    else
                    {
                        storageLocation = Convert.ToInt32(XmlUtils.GetNodeValue(node, "storagelocation", "0"));
                        isProtected = XmlUtils.GetNodeValueBoolean(node, "isprotected");
                    }
                    FolderId = objController.AddFolder(PortalId, folderPath, storageLocation, isProtected, false);
                    objInfo = objController.GetFolder(PortalId, folderPath, false);
                }
                else
                {
                    FolderId = objInfo.FolderID;
                }
                XmlNodeList nodeFolderPermissions = node.SelectNodes("folderpermissions/permission");
                ParseFolderPermissions(nodeFolderPermissions, PortalId, objInfo);
                XmlNodeList nodeFiles = node.SelectNodes("files/file");
                if (!String.IsNullOrEmpty(folderPath))
                {
                    folderPath += "/";
                }
                ParseFiles(nodeFiles, PortalId, objInfo);
            }
        }
        private void ParseFolderPermissions(XmlNodeList nodeFolderPermissions, int PortalId, FolderInfo folder)
        {
            Security.Permissions.PermissionController objPermissionController = new Security.Permissions.PermissionController();
            Security.Permissions.FolderPermissionController objFolderPermissionController = new Security.Permissions.FolderPermissionController();
            RoleController objRoleController = new RoleController();
            int PermissionID = 0;
            folder.FolderPermissions.Clear();
            foreach (XmlNode xmlFolderPermission in nodeFolderPermissions)
            {
                string PermissionKey = XmlUtils.GetNodeValue(xmlFolderPermission.CreateNavigator(), "permissionkey");
                string PermissionCode = XmlUtils.GetNodeValue(xmlFolderPermission.CreateNavigator(), "permissioncode");
                string RoleName = XmlUtils.GetNodeValue(xmlFolderPermission.CreateNavigator(), "rolename");
                bool AllowAccess = XmlUtils.GetNodeValueBoolean(xmlFolderPermission, "allowaccess");
                foreach (PermissionInfo objPermission in objPermissionController.GetPermissionByCodeAndKey(PermissionCode, PermissionKey))
                {
                    PermissionID = objPermission.PermissionID;
                }
                int RoleID = int.MinValue;
                switch (RoleName)
                {
                    case Globals.glbRoleAllUsersName:
                        RoleID = Convert.ToInt32(Globals.glbRoleAllUsers);
                        break;
                    case Common.Globals.glbRoleUnauthUserName:
                        RoleID = Convert.ToInt32(Globals.glbRoleUnauthUser);
                        break;
                    default:
                        RoleInfo objRole = objRoleController.GetRoleByName(PortalId, RoleName);
                        if (objRole != null)
                        {
                            RoleID = objRole.RoleID;
                        }
                        break;
                }
                if (RoleID != int.MinValue)
                {
                    FolderPermissionInfo objFolderPermission = new FolderPermissionInfo();
                    objFolderPermission.FolderID = folder.FolderID;
                    objFolderPermission.PermissionID = PermissionID;
                    objFolderPermission.RoleID = RoleID;
                    objFolderPermission.AllowAccess = AllowAccess;
                    folder.FolderPermissions.Add(objFolderPermission);
                }
            }
            FolderPermissionController.SaveFolderPermissions(folder);
        }
        private void ParsePortalSettings(XmlNode nodeSettings, int PortalId)
        {
            PortalInfo objPortal;
            objPortal = GetPortal(PortalId);
            objPortal.LogoFile = Globals.ImportFile(PortalId, XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "logofile"));
            objPortal.FooterText = XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "footertext");
            if (nodeSettings.SelectSingleNode("expirydate") != null)
            {
                objPortal.ExpiryDate = XmlUtils.GetNodeValueDate(nodeSettings, "expirydate", Null.NullDate);
            }
            objPortal.UserRegistration = XmlUtils.GetNodeValueInt(nodeSettings, "userregistration");
            objPortal.BannerAdvertising = XmlUtils.GetNodeValueInt(nodeSettings, "banneradvertising");
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "currency")))
            {
                objPortal.Currency = XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "currency");
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "hostfee")))
            {
                objPortal.HostFee = XmlUtils.GetNodeValueSingle(nodeSettings, "hostfee");
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "hostspace")))
            {
                objPortal.HostSpace = XmlUtils.GetNodeValueInt(nodeSettings, "hostspace");
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "pagequota")))
            {
                objPortal.PageQuota = XmlUtils.GetNodeValueInt(nodeSettings, "pagequota");
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "userquota")))
            {
                objPortal.UserQuota = XmlUtils.GetNodeValueInt(nodeSettings, "userquota");
            }
            objPortal.BackgroundFile = XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "backgroundfile");
            objPortal.PaymentProcessor = XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "paymentprocessor");
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings.CreateNavigator(), "siteloghistory")))
            {
                objPortal.SiteLogHistory = XmlUtils.GetNodeValueInt(nodeSettings, "siteloghistory");
            }
            objPortal.DefaultLanguage = XmlUtils.GetNodeValue(nodeSettings, "defaultlanguage", "en-US");
            objPortal.TimeZoneOffset = XmlUtils.GetNodeValueInt(nodeSettings, "timezoneoffset", -8);
            UpdatePortalInfo(objPortal.PortalID, objPortal.PortalName, objPortal.LogoFile, objPortal.FooterText, objPortal.ExpiryDate, objPortal.UserRegistration, objPortal.BannerAdvertising, objPortal.Currency, objPortal.AdministratorId, objPortal.HostFee,
            objPortal.HostSpace, objPortal.PageQuota, objPortal.UserQuota, objPortal.PaymentProcessor, objPortal.ProcessorUserId, objPortal.ProcessorPassword, objPortal.Description, objPortal.KeyWords, objPortal.BackgroundFile, objPortal.SiteLogHistory,
            objPortal.SplashTabId, objPortal.HomeTabId, objPortal.LoginTabId, objPortal.RegisterTabId, objPortal.UserTabId, objPortal.DefaultLanguage, objPortal.TimeZoneOffset, objPortal.HomeDirectory);
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "skinsrc", "")))
            {
                UpdatePortalSetting(PortalId, "DefaultPortalSkin", XmlUtils.GetNodeValue(nodeSettings, "skinsrc", ""));
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "skinsrcadmin", "")))
            {
                UpdatePortalSetting(PortalId, "DefaultAdminSkin", XmlUtils.GetNodeValue(nodeSettings, "skinsrcadmin", ""));
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "containersrc", "")))
            {
                UpdatePortalSetting(PortalId, "DefaultPortalContainer", XmlUtils.GetNodeValue(nodeSettings, "containersrc", ""));
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "containersrcadmin", "")))
            {
                UpdatePortalSetting(PortalId, "DefaultAdminContainer", XmlUtils.GetNodeValue(nodeSettings, "containersrcadmin", ""));
            }
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeSettings, "enableskinwidgets", "")))
            {
                UpdatePortalSetting(PortalId, "EnableSkinWidgets", XmlUtils.GetNodeValue(nodeSettings, "enableskinwidgets", ""));
            }
        }
        private void ParsePortalDesktopModules(XPathNavigator nav, int portalID)
        {
            string friendlyName = Null.NullString;
            DesktopModuleInfo desktopModule = null;
            foreach (XPathNavigator desktopModuleNav in nav.Select("portalDesktopModule"))
            {
                friendlyName = XmlUtils.GetNodeValue(desktopModuleNav, "friendlyname");
                if (!string.IsNullOrEmpty(friendlyName))
                {
                    desktopModule = DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName);
                    if (desktopModule != null)
                    {
                        DesktopModulePermissionCollection permissions = new DesktopModulePermissionCollection();
                        foreach (XPathNavigator permissionNav in desktopModuleNav.Select("portalDesktopModulePermissions/portalDesktopModulePermission"))
                        {
                            string code = XmlUtils.GetNodeValue(permissionNav, "permissioncode");
                            string key = XmlUtils.GetNodeValue(permissionNav, "permissionkey");
                            DesktopModulePermissionInfo desktopModulePermission = null;
                            ArrayList arrPermissions = new PermissionController().GetPermissionByCodeAndKey(code, key);
                            if (arrPermissions.Count > 0)
                            {
                                PermissionInfo permission = arrPermissions[0] as PermissionInfo;
                                if (permission != null)
                                {
                                    desktopModulePermission = new DesktopModulePermissionInfo(permission);
                                }
                            }
                            desktopModulePermission.AllowAccess = bool.Parse(XmlUtils.GetNodeValue(permissionNav, "allowaccess"));
                            string rolename = XmlUtils.GetNodeValue(permissionNav, "rolename");
                            if (!string.IsNullOrEmpty(rolename))
                            {
                                RoleInfo role = new RoleController().GetRoleByName(portalID, rolename);
                                if (role != null)
                                {
                                    desktopModulePermission.RoleID = role.RoleID;
                                }
                            }
                            permissions.Add(desktopModulePermission);
                        }
                        DesktopModuleController.AddDesktopModuleToPortal(portalID, desktopModule, permissions, false);
                    }
                }
            }
        }
        private void ParseProfileDefinitions(XmlNode nodeProfileDefinitions, int PortalId)
        {
            ListController objListController = new ListController();
            ListEntryInfoCollection colDataTypes = objListController.GetListEntryInfoCollection("DataType");
            int OrderCounter = -1;
            ProfilePropertyDefinition objProfileDefinition;
            foreach (XmlNode node in nodeProfileDefinitions.SelectNodes("//profiledefinition"))
            {
                OrderCounter += 2;
                ListEntryInfo typeInfo = colDataTypes["DataType:" + XmlUtils.GetNodeValue(node.CreateNavigator(), "datatype")];
                if (typeInfo == null)
                {
                    typeInfo = colDataTypes["DataType:Unknown"];
                }
                objProfileDefinition = new ProfilePropertyDefinition(PortalId);
                objProfileDefinition.DataType = typeInfo.EntryID;
                objProfileDefinition.DefaultValue = "";
                objProfileDefinition.ModuleDefId = Null.NullInteger;
                objProfileDefinition.PropertyCategory = XmlUtils.GetNodeValue(node.CreateNavigator(), "propertycategory");
                objProfileDefinition.PropertyName = XmlUtils.GetNodeValue(node.CreateNavigator(), "propertyname");
                objProfileDefinition.Required = false;
                objProfileDefinition.Visible = true;
                objProfileDefinition.ViewOrder = OrderCounter;
                objProfileDefinition.Length = XmlUtils.GetNodeValueInt(node, "length");
                ProfileController.AddPropertyDefinition(objProfileDefinition);
            }
        }
        private void ParseTabs(XmlNode nodeTabs, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs, bool IsNewPortal)
        {
            Hashtable hModules = new Hashtable();
            Hashtable hTabs = new Hashtable();
            string tabname;
            if (!IsNewPortal)
            {
                Hashtable hTabNames = new Hashtable();
                TabController objTabs = new TabController();
                foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(PortalId))
                {
                    TabInfo objTab = tabPair.Value;
                    if (!objTab.IsDeleted)
                    {
                        tabname = objTab.TabName;
                        if (!Null.IsNull(objTab.ParentId))
                        {
                            tabname = Convert.ToString(hTabNames[objTab.ParentId]) + "/" + objTab.TabName;
                        }
                        hTabNames.Add(objTab.TabID, tabname);
                    }
                }
                foreach (int i in hTabNames.Keys)
                {
                    if (hTabs[hTabNames[i]] == null)
                    {
                        hTabs.Add(hTabNames[i], i);
                    }
                }
                hTabNames = null;
            }
            foreach (XmlNode nodeTab in nodeTabs.SelectNodes("//tab"))
            {
                ParseTab(nodeTab, PortalId, IsAdminTemplate, mergeTabs, ref hModules, ref hTabs, IsNewPortal);
            }
            foreach (XmlNode nodeTab in nodeTabs.SelectNodes("//tab[url/@type = 'Tab']"))
            {
                int tabId = XmlUtils.GetNodeValueInt(nodeTab, "tabid", Null.NullInteger);
                string tabPath = XmlUtils.GetNodeValue(nodeTab, "url", Null.NullString);
                if (tabId > Null.NullInteger)
                {
                    TabController controller = new TabController();
                    TabInfo objTab = controller.GetTab(tabId, PortalId, false);
                    objTab.Url = TabController.GetTabByTabPath(PortalId, tabPath).ToString();
                    controller.UpdateTab(objTab);
                }
            }
            foreach (XmlNode nodeTab in nodeTabs.SelectNodes("//tab[url/@type = 'File']"))
            {
                int tabId = XmlUtils.GetNodeValueInt(nodeTab, "tabid", Null.NullInteger);
                string filePath = XmlUtils.GetNodeValue(nodeTab, "url", Null.NullString);
                if (tabId > Null.NullInteger)
                {
                    TabController controller = new TabController();
                    TabInfo objTab = controller.GetTab(tabId, PortalId, false);
                    objTab.Url = "FileID=" + new FileController().ConvertFilePathToFileId(filePath, PortalId).ToString();
                    controller.UpdateTab(objTab);
                }
            }
        }
        private int ParseTab(XmlNode nodeTab, int PortalId, bool IsAdminTemplate, PortalTemplateModuleAction mergeTabs, ref Hashtable hModules, ref Hashtable hTabs, bool IsNewPortal)
        {
            TabInfo objTab = null;
            TabController objTabs = new TabController();
            string strName = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "name");
            PortalInfo objportal = GetPortal(PortalId);
            if (!String.IsNullOrEmpty(strName))
            {
                if (!IsNewPortal)
                {
                    string parenttabname = "";
                    if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent")))
                    {
                        parenttabname = XmlUtils.GetNodeValue(nodeTab.CreateNavigator(), "parent") + "/";
                    }
                    if (hTabs[parenttabname + strName] != null)
                    {
                        objTab = objTabs.GetTab(Convert.ToInt32(hTabs[parenttabname + strName]), PortalId, false);
                    }
                }
                if (objTab == null || IsNewPortal)
                {
                    objTab = TabController.DeserializeTab(nodeTab, null, hTabs, PortalId, IsAdminTemplate, mergeTabs, hModules);
                }
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                if (objTab.TabName == "Admin")
                {
                    objportal.AdminTabId = objTab.TabID;
                    UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId));
                    objEventLog.AddLog("AdminTab", objTab.TabID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                }
                switch (XmlUtils.GetNodeValue(nodeTab, "tabtype", ""))
                {
                    case "splashtab":
                        objportal.SplashTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("SplashTab", objTab.TabID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID,
                            CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                    case "hometab":
                        objportal.HomeTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("HomeTab", objTab.TabID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                    case "logintab":
                        objportal.LoginTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("LoginTab", objTab.TabID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                    case "usertab":
                        objportal.UserTabId = objTab.TabID;
                        UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId));
                        objEventLog.AddLog("UserTab", objTab.TabID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED);
                        break;
                }
            }
            return 0;
        }
        private void UpdatePortalSetup(int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId, int UserTabId, int AdminTabId, string CultureCode)
        {
            DataProvider.Instance().UpdatePortalSetup(PortalId, AdministratorId, AdministratorRoleId, RegisteredRoleId, SplashTabId, HomeTabId, LoginTabId, RegisterTabId, UserTabId, AdminTabId, CultureCode);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("PortalId", PortalId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALINFO_UPDATED);
            DataCache.ClearPortalCache(PortalId, true);
        }
        public void AddPortalAlias(int PortalId, string PortalAlias)
        {
            PortalAliasController objPortalAliasController = new PortalAliasController();
            PortalAliasInfo objPortalAliasInfo = objPortalAliasController.GetPortalAlias(PortalAlias, PortalId);
            if (objPortalAliasInfo == null)
            {
                objPortalAliasInfo = new PortalAliasInfo();
                objPortalAliasInfo.PortalID = PortalId;
                objPortalAliasInfo.HTTPAlias = PortalAlias;
                objPortalAliasController.AddPortalAlias(objPortalAliasInfo);
            }
        }
        public int CreatePortal(string PortalName, string FirstName, string LastName, string Username, string Password, string Email, string Description, string KeyWords, string TemplatePath, string TemplateFile,
        string HomeDirectory, string PortalAlias, string ServerPath, string ChildPath, bool IsChildPortal)
        {
            UserInfo objAdminUser = new UserInfo();
            objAdminUser.FirstName = FirstName;
            objAdminUser.LastName = LastName;
            objAdminUser.Username = Username;
            objAdminUser.DisplayName = FirstName + " " + LastName;
            objAdminUser.Membership.Password = Password;
            objAdminUser.Email = Email;
            objAdminUser.IsSuperUser = false;
            objAdminUser.Membership.Approved = true;
            objAdminUser.Profile.FirstName = FirstName;
            objAdminUser.Profile.LastName = LastName;
            return CreatePortal(PortalName, objAdminUser, Description, KeyWords, TemplatePath, TemplateFile, HomeDirectory, PortalAlias, ServerPath, ChildPath,
            IsChildPortal);
        }
        public void CopyPageTemplate(string templateFile, string MappedHomeDirectory)
        {
            string strHostTemplateFile = string.Format("{0}Templates\\{1}", Globals.HostMapPath, templateFile);
            if (File.Exists(strHostTemplateFile))
            {
                string strPortalTemplateFolder = string.Format("{0}Templates\\", MappedHomeDirectory);
                if (!Directory.Exists(strPortalTemplateFolder))
                {
                    //Create Portal Templates folder
                    Directory.CreateDirectory(strPortalTemplateFolder);
                }
                string strPortalTemplateFile = strPortalTemplateFolder + templateFile;
                if (!File.Exists(strPortalTemplateFile))
                {
                    File.Copy(strHostTemplateFile, strPortalTemplateFile);
                }
            }
        }
        public int CreatePortal(string PortalName, UserInfo objAdminUser, string Description, string KeyWords, string TemplatePath, string TemplateFile, string HomeDirectory, string PortalAlias, string ServerPath, string ChildPath, bool IsChildPortal)
        {
            Services.FileSystem.FolderController objFolderController = new Services.FileSystem.FolderController();
            string strMessage = Null.NullString;
            int AdministratorId = Null.NullInteger;
            int intPortalId = CreatePortal(PortalName, HomeDirectory);
            if (intPortalId != -1)
            {
                if (String.IsNullOrEmpty(HomeDirectory))
                {
                    HomeDirectory = "Portals/" + intPortalId.ToString();
                }
                string MappedHomeDirectory = objFolderController.GetMappedDirectory(Common.Globals.ApplicationPath + "/" + HomeDirectory + "/");
                strMessage += CreateProfileDefinitions(intPortalId, TemplatePath, TemplateFile);
                if (strMessage == Null.NullString)
                {
                    try
                    {
                        objAdminUser.PortalID = intPortalId;
                        UserCreateStatus createStatus = UserController.CreateUser(ref objAdminUser);
                        if (createStatus == UserCreateStatus.Success)
                        {
                            AdministratorId = objAdminUser.UserID;
                        }
                        else
                        {
                            strMessage += UserController.GetUserCreateStatus(createStatus);
                        }
                    }
                    catch (Exception Exc)
                    {
                        strMessage += Localization.GetString("CreateAdminUser.Error") + Exc.Message + Exc.StackTrace;
                    }
                }
                else
                {
                    throw new Exception(strMessage);
                }
                if (String.IsNullOrEmpty(strMessage) && AdministratorId > 0)
                {
                    try
                    {
                        if (Directory.Exists(MappedHomeDirectory))
                        {
                            Globals.DeleteFolderRecursive(MappedHomeDirectory);
                        }
                    }
                    catch (Exception Exc)
                    {
                        strMessage += Localization.GetString("DeleteUploadFolder.Error") + Exc.Message + Exc.StackTrace;
                    }
                    if (strMessage == Null.NullString)
                    {
                        try
                        {
                            if (IsChildPortal)
                            {
                                if (!Directory.Exists(ChildPath))
                                {
                                    System.IO.Directory.CreateDirectory(ChildPath);
                                }
                                if (!File.Exists(ChildPath + "\\" + Globals.glbDefaultPage))
                                {
                                    System.IO.File.Copy(Common.Globals.HostMapPath + "subhost.aspx", ChildPath + "\\" + Globals.glbDefaultPage);
                                }
                            }
                        }
                        catch (Exception Exc)
                        {
                            strMessage += Localization.GetString("ChildPortal.Error") + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception(strMessage);
                    }
                    if (strMessage == Null.NullString)
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(MappedHomeDirectory);
                            //ensure that the Templates folder exists
                            string templateFolder = String.Format("{0}Templates", MappedHomeDirectory);
                            if (!Directory.Exists(templateFolder))
                            {
                                System.IO.Directory.CreateDirectory(templateFolder);
                            }

                            //ensure that the Users folder exists
                            string usersFolder = String.Format("{0}Users", MappedHomeDirectory);
                            if (!Directory.Exists(usersFolder))
                            {
                                System.IO.Directory.CreateDirectory(usersFolder);
                            }

                            //copy the default page template
                            CopyPageTemplate("Default.page.template", MappedHomeDirectory);
                            CopyPageTemplate("UserProfile.page.template", MappedHomeDirectory);

                            // process zip resource file if present
                            ProcessResourceFile(MappedHomeDirectory, TemplatePath + TemplateFile);
                        }
                        catch (Exception Exc)
                        {
                            strMessage += Localization.GetString("ChildPortal.Error") + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception(strMessage);
                    }
                    if (strMessage == Null.NullString)
                    {
                        try
                        {
                            ParseTemplate(intPortalId, TemplatePath, TemplateFile, AdministratorId, PortalTemplateModuleAction.Replace, true);
                        }
                        catch (Exception Exc)
                        {
                            strMessage += Localization.GetString("PortalTemplate.Error") + Exc.Message + Exc.StackTrace;
                        }
                    }
                    else
                    {
                        throw new Exception(strMessage);
                    }
                    if (strMessage == Null.NullString)
                    {
                        PortalInfo objportal = GetPortal(intPortalId);
                        objportal.Description = Description;
                        objportal.KeyWords = KeyWords;
                        objportal.UserTabId = TabController.GetTabByTabPath(objportal.PortalID, "//UserProfile");
                        UpdatePortalInfo(objportal.PortalID, objportal.PortalName, objportal.LogoFile, objportal.FooterText, objportal.ExpiryDate, objportal.UserRegistration, objportal.BannerAdvertising, objportal.Currency, objportal.AdministratorId, objportal.HostFee,
                        objportal.HostSpace, objportal.PageQuota, objportal.UserQuota, objportal.PaymentProcessor, objportal.ProcessorUserId, objportal.ProcessorPassword, objportal.Description, objportal.KeyWords, objportal.BackgroundFile, objportal.SiteLogHistory,
                        objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.DefaultLanguage, objportal.TimeZoneOffset, objportal.HomeDirectory);
                        objAdminUser.Profile.PreferredLocale = objportal.DefaultLanguage;
                        objAdminUser.Profile.TimeZone = objportal.TimeZoneOffset;
                        UserController.UpdateUser(objportal.PortalID, objAdminUser);
                        DesktopModuleController.AddDesktopModulesToPortal(intPortalId);
                        Localization.AddLanguagesToPortal(intPortalId);
                        AddPortalAlias(intPortalId, PortalAlias);
                        try
                        {
                            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                            objEventLogInfo.BypassBuffering = true;
                            objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString();
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("Install Portal:", PortalName));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("FirstName:", objAdminUser.FirstName));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("LastName:", objAdminUser.LastName));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("Username:", objAdminUser.Username));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("Email:", objAdminUser.Email));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("Description:", Description));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("Keywords:", KeyWords));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("TemplatePath:", TemplatePath));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("TemplateFile:", TemplateFile));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("HomeDirectory:", HomeDirectory));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("PortalAlias:", PortalAlias));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("ServerPath:", ServerPath));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("ChildPath:", ChildPath));
                            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("IsChildPortal:", IsChildPortal.ToString()));
                            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                            objEventLog.AddLog(objEventLogInfo);
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                    else
                    {
                        throw new Exception(strMessage);
                    }
                }
                else
                {
                    DeletePortalInfo(intPortalId);
                    intPortalId = -1;
                    throw new Exception(strMessage);
                }
            }
            else
            {
                strMessage += Localization.GetString("CreatePortal.Error");
                throw new Exception(strMessage);
            }
            return intPortalId;
        }
        public void DeletePortalInfo(int PortalId)
        {
            UserController.DeleteUsers(PortalId, false, true);
            DataProvider.Instance().DeletePortalInfo(PortalId);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("PortalId", PortalId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALINFO_DELETED);
            DataCache.ClearHostCache(true);
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets information of a portal
        /// </summary>
        /// <param name="PortalId">Id of the portal</param>
        /// <returns>PortalInfo object with portal definition</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public PortalInfo GetPortal(int PortalId)
        {
            string defaultLanguage = PortalController.GetActivePortalLanguage(PortalId);
            return GetPortal(PortalId, defaultLanguage);
        }
        public PortalInfo GetPortal(int PortalId, string CultureCode)
        {
            string cacheKey = string.Format(DataCache.PortalCacheKey, PortalId.ToString() + "-" + CultureCode);
            return CBO.GetCachedObject<PortalInfo>(new CacheItemArgs(cacheKey, DataCache.PortalCacheTimeOut, DataCache.PortalCachePriority, PortalId, CultureCode), GetPortalCallback);
        }

        public ArrayList GetPortals()
        {
            return CBO.FillCollection(DataProvider.Instance().GetPortals(), typeof(PortalInfo));
        }
        public ArrayList GetPortals(string CultureCode)
        {
           // return CBO.FillCollection(DataProvider.Instance().GetPortals(CultureCode), typeof(PortalInfo));
            return null;
        }
        public long GetPortalSpaceUsedBytes()
        {
            return GetPortalSpaceUsedBytes(-1);
        }
        public long GetPortalSpaceUsedBytes(int portalId)
        {
            long size = 0;
            IDataReader dr = null;
            dr = DataProvider.Instance().GetPortalSpaceUsed(portalId);
            try
            {
                if (dr.Read())
                {
                    if (dr["SpaceUsed"] != DBNull.Value)
                    {
                        size = Convert.ToInt64(dr["SpaceUsed"]);
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
            return size;
        }
        public bool HasSpaceAvailable(int portalId, long fileSizeBytes)
        {
            int hostSpace;
            if (portalId == -1)
            {
                hostSpace = 0;
            }
            else
            {
                PortalSettings ps = GetCurrentPortalSettings();
                if (ps != null && ps.PortalId == portalId)
                {
                    hostSpace = ps.HostSpace;
                }
                else
                {
                    PortalInfo portal = GetPortal(portalId);
                    hostSpace = portal.HostSpace;
                }
            }
            return (((GetPortalSpaceUsedBytes(portalId) + fileSizeBytes) / Math.Pow(1024, 2)) <= hostSpace) || hostSpace == 0;
        }
        public void ParseTemplate(int PortalId, string TemplatePath, string TemplateFile, int AdministratorId, PortalTemplateModuleAction mergeTabs, bool IsNewPortal)
        {
            XmlDocument xmlPortal = new XmlDocument();
            XmlNode node;
            try
            {
                xmlPortal.Load(TemplatePath + TemplateFile);
            }
            catch
            {
            }
            node = xmlPortal.SelectSingleNode("//portal/settings");
            if (node != null && IsNewPortal)
            {
                ParsePortalSettings(node, PortalId);
            }
            node = xmlPortal.SelectSingleNode("//portal/rolegroups");
            if (node != null)
            {
                ParseRoleGroups(node.CreateNavigator(), PortalId, AdministratorId);
            }
            node = xmlPortal.SelectSingleNode("//portal/roles");
            if (node != null)
            {
                ParseRoles(node.CreateNavigator(), PortalId, AdministratorId);
            }
            node = xmlPortal.SelectSingleNode("//portal/portalDesktopModules");
            if (node != null)
            {
                ParsePortalDesktopModules(node.CreateNavigator(), PortalId);
            }
            node = xmlPortal.SelectSingleNode("//portal/folders");
            if (node != null)
            {
                ParseFolders(node, PortalId);
            }
            var objController = new FolderController();
            if (objController.GetFolder(PortalId, "", false) == null)
            {
                int folderid = objController.AddFolder(PortalId, "", (int)Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem, true, false);
                AddFolderPermissions(PortalId, folderid);
            }
            if (objController.GetFolder(PortalId, "Templates/", false) == null)
            {
                int folderid = objController.AddFolder(PortalId, "Templates/", (int)Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem, true, false);
                AddFolderPermissions(PortalId, folderid);
            }
            // force creation of templates folder if not present on template
            if (objController.GetFolder(PortalId, "Users/", false) == null)
            {
                int folderid = objController.AddFolder(PortalId, "Users/", (int)Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem, true, false);
                AddFolderPermissions(PortalId, folderid);
            }
            if (mergeTabs == PortalTemplateModuleAction.Replace)
            {
                var objTabs = new TabController();
                TabInfo objTab;
                foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(PortalId))
                {
                    objTab = tabPair.Value;
                    objTab.TabName = objTab.TabName + "_old";
                    objTab.TabPath = Common.Globals.GenerateTabPath(objTab.ParentId, objTab.TabName);
                    objTab.IsDeleted = true;
                    objTabs.UpdateTab(objTab);
                    var objModules = new ModuleController();
                    ModuleInfo objModule;
                    foreach (KeyValuePair<int, ModuleInfo> modulePair in objModules.GetTabModules(objTab.TabID))
                    {
                        objModule = modulePair.Value;
                        objModules.DeleteTabModule(objModule.TabID, objModule.ModuleID, false);
                    }
                }
            }
            node = xmlPortal.SelectSingleNode("//portal/tabs");
            if (node != null)
            {
                string version = xmlPortal.DocumentElement.GetAttribute("version");
                if (version != "5.0")
                {
                    var xmlAdmin = new XmlDocument();
                    try
                    {
                        xmlAdmin.Load(TemplatePath + "admin.template");
                    }
                    catch
                    {
                    }
                    XmlNode adminNode = xmlAdmin.SelectSingleNode("//portal/tabs");
                    foreach (XmlNode adminTabNode in adminNode.ChildNodes)
                    {
                        node.AppendChild(xmlPortal.ImportNode(adminTabNode, true));
                    }
                }
                ParseTabs(node, PortalId, false, mergeTabs, IsNewPortal);
            }
        }
        public void ProcessResourceFile(string portalPath, string TemplateFile)
        {
            ZipInputStream objZipInputStream;
            try
            {
                objZipInputStream = new ZipInputStream(new FileStream(TemplateFile + ".resources", FileMode.Open, FileAccess.Read));
                FileSystemUtils.UnzipResources(objZipInputStream, portalPath);
            }
            catch (Exception exc)
            {
                exc.ToString();
            }
        }
        public void UpdatePortalExpiry(int PortalId)
        {
            UpdatePortalExpiry(PortalId, PortalController.GetActivePortalLanguage(PortalId));
        }
        public void UpdatePortalExpiry(int PortalId, string CultureCode)
        {
            IDataReader dr = null;
            try
            {
                dr = DataProvider.Instance().GetPortal(PortalId, CultureCode);
                if (dr.Read())
                {
                    DateTime ExpiryDate;
                    if (dr["ExpiryDate"] != DBNull.Value)
                    {
                        ExpiryDate = Convert.ToDateTime(dr["ExpiryDate"]);
                    }
                    else
                    {
                        ExpiryDate = DateTime.Now;
                    }
                    DataProvider.Instance().UpdatePortalInfo(PortalId, Convert.ToString(dr["PortalName"]), Convert.ToString(dr["LogoFile"]), Convert.ToString(dr["FooterText"]), ExpiryDate.AddMonths(1), Convert.ToInt32(dr["UserRegistration"]), Convert.ToInt32(dr["BannerAdvertising"]), Convert.ToString(dr["Currency"]), Convert.ToInt32(dr["AdministratorId"]), Convert.ToDouble(dr["HostFee"]),
                    Convert.ToDouble(dr["HostSpace"]), Convert.ToInt32(dr["PageQuota"]), Convert.ToInt32(dr["UserQuota"]), Convert.ToString(dr["PaymentProcessor"]), Convert.ToString(dr["ProcessorUserId"]), Convert.ToString(dr["ProcessorPassword"]), Convert.ToString(dr["Description"]), Convert.ToString(dr["KeyWords"]), Convert.ToString(dr["BackgroundFile"]), Convert.ToInt32(dr["SiteLogHistory"]),
                    Convert.ToInt32(dr["SplashTabId"]), Convert.ToInt32(dr["HomeTabId"]), Convert.ToInt32(dr["LoginTabId"]), Convert.ToInt32(dr["RegisterTabId"]), Convert.ToInt32(dr["UserTabId"]), Convert.ToString(dr["DefaultLanguage"]), Convert.ToInt32(dr["TimeZoneOffset"]), Convert.ToString(dr["HomeDirectory"]), UserController.GetCurrentUserInfo().UserID, CultureCode);
                    var objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog("ExpiryDate", ExpiryDate.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALINFO_UPDATED);
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
        public void UpdatePortalInfo(PortalInfo Portal)
        {
            UpdatePortalInfo(Portal.PortalID, Portal.PortalName, Portal.LogoFile, Portal.FooterText, Portal.ExpiryDate, Portal.UserRegistration, Portal.BannerAdvertising, Portal.Currency, Portal.AdministratorId, Portal.HostFee,
            Portal.HostSpace, Portal.PageQuota, Portal.UserQuota, Portal.PaymentProcessor, Portal.ProcessorUserId, Portal.ProcessorPassword, Portal.Description, Portal.KeyWords, Portal.BackgroundFile, Portal.SiteLogHistory,
            Portal.SplashTabId, Portal.HomeTabId, Portal.LoginTabId, Portal.RegisterTabId, Portal.UserTabId, Portal.DefaultLanguage, Portal.TimeZoneOffset, Portal.HomeDirectory, PortalController.GetActivePortalLanguage(Portal.PortalID));
        }
        public void UpdatePortalInfo(int PortalId, string PortalName, string LogoFile, string FooterText, System.DateTime ExpiryDate, int UserRegistration, int BannerAdvertising, string Currency, int AdministratorId, double HostFee,
        double HostSpace, int PageQuota, int UserQuota, string PaymentProcessor, string ProcessorUserId, string ProcessorPassword, string Description, string KeyWords, string BackgroundFile, int SiteLogHistory,
        int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId, int UserTabId, string DefaultLanguage, int TimeZoneOffset, string HomeDirectory)
        {
            UpdatePortalInfo(PortalId, PortalName, LogoFile, FooterText, ExpiryDate, UserRegistration, BannerAdvertising, Currency, AdministratorId, HostFee,
            HostSpace, PageQuota, UserQuota, PaymentProcessor, ProcessorUserId, ProcessorPassword, Description, KeyWords, BackgroundFile, SiteLogHistory,
            SplashTabId, HomeTabId, LoginTabId, RegisterTabId, UserTabId, DefaultLanguage, TimeZoneOffset, HomeDirectory, Entities.Host.Host.ContentLocale.ToString());
        }
        public void UpdatePortalInfo(int PortalId, string PortalName, string LogoFile, string FooterText, System.DateTime ExpiryDate, int UserRegistration, int BannerAdvertising, string Currency, int AdministratorId, double HostFee,
        double HostSpace, int PageQuota, int UserQuota, string PaymentProcessor, string ProcessorUserId, string ProcessorPassword, string Description, string KeyWords, string BackgroundFile, int SiteLogHistory,
        int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId, int UserTabId, string DefaultLanguage, int TimeZoneOffset, string HomeDirectory, string CultureCode)
        {
            DataProvider.Instance().UpdatePortalInfo(PortalId, PortalName, LogoFile, FooterText, ExpiryDate, UserRegistration, BannerAdvertising, Currency, AdministratorId, HostFee,
            HostSpace, PageQuota, UserQuota, PaymentProcessor, ProcessorUserId, ProcessorPassword, Description, KeyWords, BackgroundFile, SiteLogHistory,
            SplashTabId, HomeTabId, LoginTabId, RegisterTabId, UserTabId, DefaultLanguage, TimeZoneOffset, HomeDirectory, UserController.GetCurrentUserInfo().UserID, CultureCode);
            var objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("PortalId", PortalId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALINFO_UPDATED);
            //ensure a localization item exists (in case a new default language has been set)
            DataProvider.Instance().EnsureLocalizationExists(PortalId, DefaultLanguage);
            //clear portal cache
            DataCache.ClearPortalCache(PortalId, false);
        }

        #region Methods ================================================================================
        public DataTable GetList()
        {
            SqlCommand cmd = new SqlCommand("aspnet_Portals_GetList", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;

        }

        public DataTable GetListByApplicationId(string ApplicationId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Portals_GetListByApplicationId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int PortalId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Portals_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string ApplicationId, string PortalName, string ExpiryDate, string Currency, string HostFee, int HostSpace,
            string DefaultLanguage, string HomeDirectory, string Url, string LogoFile, string BackgroundFile,
            string KeyWords, string FooterText, string Description, string CreatedByUserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Portals_Add", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@PortalName", PortalName);
            cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
            cmd.Parameters.AddWithValue("@Currency", Currency);
            cmd.Parameters.AddWithValue("@HostFee", HostFee);
            cmd.Parameters.AddWithValue("@HostSpace", HostSpace);
            cmd.Parameters.AddWithValue("@DefaultLanguage", DefaultLanguage);
            cmd.Parameters.AddWithValue("@HomeDirectory", HomeDirectory);
            cmd.Parameters.AddWithValue("@Url", Url);
            cmd.Parameters.AddWithValue("@LogoFile", LogoFile);
            cmd.Parameters.AddWithValue("@BackgroundFile", BackgroundFile);
            cmd.Parameters.AddWithValue("@KeyWords", KeyWords);
            cmd.Parameters.AddWithValue("@FooterText", FooterText);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@CreatedByUserId", CreatedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();

            return retunvalue;
        }

        public int Update(int PortalId, string ApplicationId, string PortalName, string ExpiryDate, string Currency, string HostFee, int HostSpace, string DefaultLanguage,
            string HomeDirectory, string Url, string LogoFile, string BackgroundFile, string KeyWords, string FooterText, string Description, string LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Portals_Edit", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@PortalName", PortalName);
            cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
            cmd.Parameters.AddWithValue("@Currency", Currency);
            cmd.Parameters.AddWithValue("@HostFee", HostFee);
            cmd.Parameters.AddWithValue("@HostSpace", HostSpace);
            cmd.Parameters.AddWithValue("@DefaultLanguage", DefaultLanguage);
            cmd.Parameters.AddWithValue("@HomeDirectory", HomeDirectory);
            cmd.Parameters.AddWithValue("@Url", Url);
            cmd.Parameters.AddWithValue("@LogoFile", LogoFile);
            cmd.Parameters.AddWithValue("@BackgroundFile", BackgroundFile);
            cmd.Parameters.AddWithValue("@KeyWords", KeyWords);
            cmd.Parameters.AddWithValue("@FooterText", FooterText);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();

            return retunvalue;
        }
        #endregion =====================================================================================

    }    
}
