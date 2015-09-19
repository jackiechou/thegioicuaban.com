using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.OutputCache;
using CommonLibrary.Services.Cache;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Services.Log.EventLog;
using CommonLibrary.Entities.Host;
using System.Web.Caching;

namespace CommonLibrary.Common.Utilities
{
    public enum CoreCacheType
	{
		Host = 1,
		Portal = 2,
		Tab = 3
	}
    public class DataCache
    {
        private static string _CachePersistenceEnabled = "";
        private static Dictionary<string, object> dictionaryCache = new Dictionary<string, object>();
        private static ReaderWriterLock dictionaryLock = new ReaderWriterLock();
        private static Dictionary<string, object> lockDictionary = new Dictionary<string, object>();
        public const string SecureHostSettingsCacheKey = "SecureHostSettings";
        public const string HostSettingsCacheKey = "HostSettings";
        public const CacheItemPriority HostSettingsCachePriority = CacheItemPriority.NotRemovable;
        public const int HostSettingsCacheTimeOut = 20;
        public const string PortalAliasCacheKey = "PortalAlias";
        public const CacheItemPriority PortalAliasCachePriority = CacheItemPriority.NotRemovable;
        public const int PortalAliasCacheTimeOut = 200;
        public const string PortalSettingsCacheKey = "PortalSettings{0}";
        public const CacheItemPriority PortalSettingsCachePriority = CacheItemPriority.NotRemovable;
        public const int PortalSettingsCacheTimeOut = 20;
        public const string PortalDictionaryCacheKey = "PortalDictionary";
        public const CacheItemPriority PortalDictionaryCachePriority = CacheItemPriority.High;
        public const int PortalDictionaryTimeOut = 20;
        public const string PortalCacheKey = "Portal{0}";
        public const CacheItemPriority PortalCachePriority = CacheItemPriority.High;
        public const int PortalCacheTimeOut = 20;
        public const string TabCacheKey = "Tab_Tabs{0}";
        public const CacheItemPriority TabCachePriority = CacheItemPriority.High;
        public const int TabCacheTimeOut = 20;
        public const string TabPathCacheKey = "Tab_TabPathDictionary{0}";
        public const CacheItemPriority TabPathCachePriority = CacheItemPriority.High;
        public const int TabPathCacheTimeOut = 20;
        public const string TabPermissionCacheKey = "Tab_TabPermissions{0}";
        public const CacheItemPriority TabPermissionCachePriority = CacheItemPriority.High;
        public const int TabPermissionCacheTimeOut = 20;
        public const string AuthenticationServicesCacheKey = "AuthenticationServices";
        public const CacheItemPriority AuthenticationServicesCachePriority = CacheItemPriority.NotRemovable;
        public const int AuthenticationServicesCacheTimeOut = 20;
        public const string DesktopModulePermissionCacheKey = "DesktopModulePermissions";
        public const CacheItemPriority DesktopModulePermissionCachePriority = CacheItemPriority.High;
        public const int DesktopModulePermissionCacheTimeOut = 20;
        public const string DesktopModuleCacheKey = "DesktopModulesByPortal{0}";
        public const CacheItemPriority DesktopModuleCachePriority = CacheItemPriority.High;
        public const int DesktopModuleCacheTimeOut = 20;
        public const string PortalDesktopModuleCacheKey = "PortalDesktopModules{0}";
        public const CacheItemPriority PortalDesktopModuleCachePriority = CacheItemPriority.AboveNormal;
        public const int PortalDesktopModuleCacheTimeOut = 20;
        public const string ModuleDefinitionCacheKey = "ModuleDefinitions";
        public const CacheItemPriority ModuleDefinitionCachePriority = CacheItemPriority.High;
        public const int ModuleDefinitionCacheTimeOut = 20;
        public const string ModuleControlsCacheKey = "ModuleControls";
        public const CacheItemPriority ModuleControlsCachePriority = CacheItemPriority.High;
        public const int ModuleControlsCacheTimeOut = 20;
        public const string TabModuleCacheKey = "TabModules{0}";
        public const CacheItemPriority TabModuleCachePriority = CacheItemPriority.AboveNormal;
        public const int TabModuleCacheTimeOut = 20;
        public const string ModulePermissionCacheKey = "ModulePermissions{0}";
        public const CacheItemPriority ModulePermissionCachePriority = CacheItemPriority.AboveNormal;
        public const int ModulePermissionCacheTimeOut = 20;
        public const string ModuleCacheKey = "Modules{0}";
        public const int ModuleCacheTimeOut = 20;
        public const string FolderCacheKey = "Folders{0}";
        public const int FolderCacheTimeOut = 20;
        public const CacheItemPriority FolderCachePriority = CacheItemPriority.Normal;
        public const string FolderPermissionCacheKey = "FolderPermissions{0}";
        public const CacheItemPriority FolderPermissionCachePriority = CacheItemPriority.Normal;
        public const int FolderPermissionCacheTimeOut = 20;
        public const string ListsCacheKey = "Lists{0}";
        public const CacheItemPriority ListsCachePriority = CacheItemPriority.Normal;
        public const int ListsCacheTimeOut = 20;
        public const string ProfileDefinitionsCacheKey = "ProfileDefinitions{0}";
        public const int ProfileDefinitionsCacheTimeOut = 20;
        public const string UserCacheKey = "UserInfo|{0}|{1}";
        public const int UserCacheTimeOut = 1;
        public const CacheItemPriority UserCachePriority = CacheItemPriority.Normal;
        public const string LocalesCacheKey = "Locales{0}";
        public const CacheItemPriority LocalesCachePriority = CacheItemPriority.Normal;
        public const int LocalesCacheTimeOut = 20;
        public const string SkinDefaultsCacheKey = "SkinDefaults_{0}";
        public const CacheItemPriority SkinDefaultsCachePriority = CacheItemPriority.Normal;
        public const int SkinDefaultsCacheTimeOut = 20;
        public const CacheItemPriority ResourceFilesCachePriority = CacheItemPriority.Normal;
        public const int ResourceFilesCacheTimeOut = 20;
        public const string ResourceFileLookupDictionaryCacheKey = "ResourceFileLookupDictionary";
        public const CacheItemPriority ResourceFileLookupDictionaryCachePriority = CacheItemPriority.NotRemovable;
        public const int ResourceFileLookupDictionaryTimeOut = 200;
        public const string SkinsCacheKey = "GetSkins{0}";
        public const string BannersCacheKey = "Banners:{0}:{1}:{2}";
        public const CacheItemPriority BannersCachePriority = CacheItemPriority.Normal;
        public const int BannersCacheTimeOut = 20;
        public static bool CachePersistenceEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_CachePersistenceEnabled))
                {
                    if (Config.GetSetting("EnableCachePersistence") == null)
                    {
                        _CachePersistenceEnabled = "false";
                    }
                    else
                    {
                        _CachePersistenceEnabled = Config.GetSetting("EnableCachePersistence");
                    }
                }
                return bool.Parse(_CachePersistenceEnabled);
            }
        }
        private static string GetCacheKey(string CacheKey)
        {
            return CachingProvider.GetCacheKey(CacheKey);
        }
        private static string CleanCacheKey(string CacheKey)
        {
            return CachingProvider.CleanCacheKey(CacheKey);
        }
        static internal void ItemRemovedCallback(string key, object value, CacheItemRemovedReason removedReason)
        {
            try
            {
                if (Globals.Status == Globals.UpgradeStatus.None)
                {
                    LogInfo objEventLogInfo = new LogInfo();
                    switch (removedReason)
                    {
                        case CacheItemRemovedReason.Removed:
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_REMOVED.ToString();
                            break;
                        case CacheItemRemovedReason.Expired:
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_EXPIRED.ToString();
                            break;
                        case CacheItemRemovedReason.Underused:
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_UNDERUSED.ToString();
                            break;
                        case CacheItemRemovedReason.DependencyChanged:
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_DEPENDENCYCHANGED.ToString();
                            break;
                    }
                    objEventLogInfo.LogProperties.Add(new LogDetailInfo(key, removedReason.ToString()));
                    EventLogController objEventLog = new EventLogController();
                    objEventLog.AddLog(objEventLogInfo);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static void ClearCache()
        {
            CachingProvider.Instance().Clear("Prefix", "5EAGLES_");
            dictionaryCache.Clear();
            LogInfo objEventLogInfo = new LogInfo();
            objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.CACHE_REFRESH.ToString();
            objEventLogInfo.LogProperties.Add(new LogDetailInfo("*", "Refresh"));
            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog(objEventLogInfo);
        }
        public static void ClearCache(string cachePrefix)
        {
            CachingProvider.Instance().Clear("Prefix", GetCacheKey(cachePrefix));
        }
        public static void ClearFolderCache(int PortalId)
        {
            CachingProvider.Instance().Clear("Folder", PortalId.ToString());
        }
        public static void ClearHostCache(bool Cascade)
        {
            if (Cascade)
            {
                ClearCache();
            }
            else
            {
                CachingProvider.Instance().Clear("Host", "");
            }
        }
        public static void ClearModuleCache(int TabId)
        {
            CachingProvider.Instance().Clear("Module", TabId.ToString());
            Dictionary<int, int> portals = PortalController.GetPortalDictionary();
            if (portals.ContainsKey(TabId))
            {
                OutputCachingProvider.RemoveItemFromAllProviders(TabId);
            }
        }
        public static void ClearModulePermissionsCachesByPortal(int PortalId)
        {
            CachingProvider.Instance().Clear("ModulePermissionsByPortal", PortalId.ToString());
        }
        public static void ClearPortalCache(int PortalId, bool Cascade)
        {
            if (Cascade)
            {
                CachingProvider.Instance().Clear("PortalCascade", PortalId.ToString());
            }
            else
            {
                CachingProvider.Instance().Clear("Portal", PortalId.ToString());
            }
        }
        public static void ClearTabsCache(int PortalId)
        {
            CachingProvider.Instance().Clear("Tab", PortalId.ToString());
        }
        public static void ClearDefinitionsCache(int PortalId)
        {
            RemoveCache(string.Format(ProfileDefinitionsCacheKey, PortalId));
        }
        public static void ClearDesktopModulePermissionsCache()
        {
            RemoveCache(DesktopModulePermissionCacheKey);
        }
        public static void ClearFolderPermissionsCache(int PortalId)
        {
            RemoveCache(string.Format(FolderPermissionCacheKey, PortalId));
        }
        public static void ClearListsCache(int PortalId)
        {
            RemoveCache(string.Format(ListsCacheKey, PortalId));
        }
        public static void ClearModulePermissionsCache(int TabId)
        {
            RemoveCache(string.Format(ModulePermissionCacheKey, TabId));
        }
        public static void ClearTabPermissionsCache(int PortalId)
        {
            RemoveCache(string.Format(TabPermissionCacheKey, PortalId));
        }
        public static void ClearUserCache(int PortalId, string username)
        {
            RemoveCache(string.Format(UserCacheKey, PortalId, username));
        }
        public static TObject GetCachedData<TObject>(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired)
        {
            return GetCachedData<TObject>(cacheItemArgs, cacheItemExpired, false);
        }

        internal static TObject GetCachedData<TObject>(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired, bool storeInDictionary)
        {
            //declare local object and try and retrieve item from the cache

            object objObject = null;

            if (!storeInDictionary)
            {
                objObject = GetCache(cacheItemArgs.CacheKey);
            }
            else if (dictionaryCache.ContainsKey(cacheItemArgs.CacheKey))
            {
                objObject = dictionaryCache[cacheItemArgs.CacheKey];
            }
            if (objObject == null)
            {
                object @lock = GetUniqueLockObject(cacheItemArgs.CacheKey);
                lock (@lock)
                {
                    if (!storeInDictionary)
                    {
                        objObject = GetCache(cacheItemArgs.CacheKey);
                    }
                    else if (dictionaryCache.ContainsKey(cacheItemArgs.CacheKey))
                    {
                        objObject = dictionaryCache[cacheItemArgs.CacheKey];
                    }
                    if (objObject == null)
                    {
                        try
                        {
                            objObject = cacheItemExpired(cacheItemArgs);
                        }
                        catch (Exception ex)
                        {
                            objObject = null;
                            Exceptions.LogException(ex);
                        }
                        if (storeInDictionary)
                        {
                            dictionaryCache[cacheItemArgs.CacheKey] = objObject;
                        }
                        else
                        {
                            // set cache timeout
                            int timeOut = cacheItemArgs.CacheTimeOut * Convert.ToInt32(Host.PerformanceSetting);
                            if (objObject != null && timeOut > 0)
                            {
                                //DataCache.SetCache(cacheItemArgs.CacheKey, objObject, cacheItemArgs.CacheDependency, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(timeOut), cacheItemArgs.CachePriority, cacheItemArgs.CacheCallback);
                                //if (DataCache.GetCache(cacheItemArgs.CacheKey) == null)
                                //{
                                //    LogInfo objEventLogInfo = new LogInfo();
                                //    objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.CACHE_OVERFLOW.ToString();
                                //    objEventLogInfo.LogProperties.Add(new LogDetailInfo(cacheItemArgs.CacheKey, "Overflow - Item Not Cached"));
                                //    EventLogController objEventLog = new EventLogController();
                                //    objEventLog.AddLog(objEventLogInfo);
                                //}
                            }
                        }
                        RemoveUniqueLockObject(cacheItemArgs.CacheKey);
                    }
                }
            }
            if (objObject == null)
            {
                return default(TObject);
            }
            else
            {
                return (TObject)objObject;
            }
        }
        private static object GetUniqueLockObject(string key)
        {
            object @lock = null;
            dictionaryLock.AcquireReaderLock(new TimeSpan(0, 0, 5));
            try
            {
                if (lockDictionary.ContainsKey(key))
                {
                    @lock = lockDictionary[key];
                }
            }
            finally
            {
                dictionaryLock.ReleaseReaderLock();
            }
            if (@lock == null)
            {
                dictionaryLock.AcquireWriterLock(new TimeSpan(0, 0, 5));
                try
                {
                    if (!lockDictionary.ContainsKey(key))
                    {
                        lockDictionary[key] = new object();
                    }
                    @lock = lockDictionary[key];
                }
                finally
                {
                    dictionaryLock.ReleaseWriterLock();
                }
            }
            return @lock;
        }
        private static void RemoveUniqueLockObject(string key)
        {
            dictionaryLock.AcquireWriterLock(new TimeSpan(0, 0, 5));
            try
            {
                if (lockDictionary.ContainsKey(key))
                {
                    lockDictionary.Remove(key);
                }
            }
            finally
            {
                dictionaryLock.ReleaseWriterLock();
            }
        }
        public static TObject GetCache<TObject>(string CacheKey)
        {
            object objObject = GetCache(CacheKey);
            if (objObject == null)
            {
                return default(TObject);
            }
            return (TObject)objObject;
        }
        public static object GetCache(string CacheKey)
        {
            return CachingProvider.Instance().GetItem(GetCacheKey(CacheKey));
        }
        public static void RemoveCache(string CacheKey)
        {
            CachingProvider.Instance().Remove(GetCacheKey(CacheKey));
        }
        public static void RemoveFromPrivateDictionary(string DnnCacheKey)
        {
            dictionaryCache.Remove(CleanCacheKey(DnnCacheKey));
        }
        public static void SetCache(string CacheKey, object objObject)
        {
            CommonLibrary.Services.Cache.CacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }
        public static void SetCache(string CacheKey, object objObject, CommonLibrary.Services.Cache.CacheDependency objDependency)
        {
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }
        public static void SetCache(string CacheKey, object objObject, System.DateTime AbsoluteExpiration)
        {
            CommonLibrary.Services.Cache.CacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, AbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }
        public static void SetCache(string CacheKey, object objObject, TimeSpan SlidingExpiration)
        {
            CommonLibrary.Services.Cache.CacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
        }
        public static void SetCache(string CacheKey, object objObject, CommonLibrary.Services.Cache.CacheDependency objDependency, System.DateTime AbsoluteExpiration, System.TimeSpan SlidingExpiration)
        {
            SetCache(CacheKey, objObject, objDependency, AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
        }
        public static void SetCache(string CacheKey, object objObject, CommonLibrary.Services.Cache.CacheDependency objDependency, System.DateTime AbsoluteExpiration, System.TimeSpan SlidingExpiration, CacheItemPriority Priority, CacheItemRemovedCallback OnRemoveCallback)
        {
            if (objObject != null)
            {
                if (OnRemoveCallback == null)
                {
                    OnRemoveCallback = ItemRemovedCallback;
                }
                CachingProvider.Instance().Insert(GetCacheKey(CacheKey), objObject, objDependency, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback);
            }
        }
    }
}
