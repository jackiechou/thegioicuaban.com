using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CommonLibrary.ComponentModel;
using System.Collections;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Localization;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Entities.Host;
using System.Web.Caching;

namespace CommonLibrary.Services.Cache
{
    public abstract class CachingProvider
    {
        private static System.Web.Caching.Cache _objCache;
        private static string _cachePrefix = "5EAGLES_";
        protected static System.Web.Caching.Cache Cache
        {
            get
            {
                if (_objCache == null)
                {
                    _objCache = HttpRuntime.Cache;
                }
                return _objCache;
            }
        }

        public static string CleanCacheKey(string CacheKey)
        {
            if (String.IsNullOrEmpty(CacheKey))
            {
                throw new ArgumentException("Argument cannot be null or an empty string", "CacheKey");
            }
            return CacheKey.Substring(_cachePrefix.Length);
        }
        public static string GetCacheKey(string CacheKey)
        {
            if (string.IsNullOrEmpty(CacheKey))
            {
                throw new ArgumentException("Argument cannot be null or an empty string", "CacheKey");
            }
            return _cachePrefix + CacheKey;
        }
        public static CachingProvider Instance()
        {
            return ComponentFactory.GetComponent<CachingProvider>();
        }
        private void ClearCacheInternal(string prefix, bool clearRuntime)
        {
            foreach (DictionaryEntry objDictionaryEntry in HttpRuntime.Cache)
            {
                if (Convert.ToString(objDictionaryEntry.Key).StartsWith(prefix))
                {
                    if (clearRuntime)
                    {
                        RemoveInternal(Convert.ToString(objDictionaryEntry.Key));
                    }
                    else
                    {
                        Remove(Convert.ToString(objDictionaryEntry.Key));
                    }
                }
            }
        }
        private void ClearCacheKeysByPortalInternal(int portalId, bool clearRuntime)
        {
            RemoveFormattedCacheKey(DataCache.LocalesCacheKey, clearRuntime, portalId);
            RemoveFormattedCacheKey(DataCache.ProfileDefinitionsCacheKey, clearRuntime, portalId);
            RemoveFormattedCacheKey(DataCache.ListsCacheKey, clearRuntime, portalId);
            RemoveFormattedCacheKey(DataCache.SkinsCacheKey, clearRuntime, portalId);
        }
        private void ClearDesktopModuleCacheInternal(int portalId, bool clearRuntime)
        {
            RemoveFormattedCacheKey(DataCache.DesktopModuleCacheKey, clearRuntime, portalId);
            RemoveFormattedCacheKey(DataCache.PortalDesktopModuleCacheKey, clearRuntime, portalId);
            RemoveCacheKey(DataCache.ModuleDefinitionCacheKey, clearRuntime);
            RemoveCacheKey(DataCache.ModuleControlsCacheKey, clearRuntime);
        }
        private void ClearFolderCacheInternal(int portalId, bool clearRuntime)
        {
            RemoveFormattedCacheKey(DataCache.FolderCacheKey, clearRuntime, portalId);
            RemoveFormattedCacheKey(DataCache.FolderPermissionCacheKey, clearRuntime, portalId);
        }
        private void ClearHostCacheInternal(bool clearRuntime)
        {
            RemoveCacheKey(DataCache.HostSettingsCacheKey, clearRuntime);
            RemoveCacheKey(DataCache.SecureHostSettingsCacheKey, clearRuntime);
            RemoveCacheKey(DataCache.PortalAliasCacheKey, clearRuntime);
            RemoveCacheKey("CSS", clearRuntime);
            RemoveCacheKey(DataCache.DesktopModulePermissionCacheKey, clearRuntime);
            RemoveCacheKey("GetRoles", clearRuntime);
            RemoveCacheKey("CompressionConfig", clearRuntime);
            ClearFolderCacheInternal(-1, clearRuntime);
            ClearDesktopModuleCacheInternal(-1, clearRuntime);
            ClearCacheKeysByPortalInternal(-1, clearRuntime);
        }
        private void ClearModuleCacheInternal(int tabId, bool clearRuntime)
        {
            RemoveFormattedCacheKey(DataCache.TabModuleCacheKey, clearRuntime, tabId);
            RemoveFormattedCacheKey(DataCache.ModulePermissionCacheKey, clearRuntime, tabId);
        }
        private void ClearModulePermissionsCachesByPortalInternal(int portalId, bool clearRuntime)
        {
            TabController objTabs = new TabController();
            foreach (KeyValuePair<int, CommonLibrary.Entities.Tabs.TabInfo> tabPair in objTabs.GetTabsByPortal(portalId))
            {
                RemoveFormattedCacheKey(DataCache.ModulePermissionCacheKey, clearRuntime, tabPair.Value.TabID);
            }
        }
        private void ClearPortalCacheInternal(int portalId, bool cascade, bool clearRuntime)
        {
            RemoveFormattedCacheKey(DataCache.PortalSettingsCacheKey, clearRuntime, portalId);
            Dictionary<string, Locale> locales = CommonLibrary.Services.Localization.Localization.GetLocales(portalId);
            if (locales.Count == 0)
            {
                //At least attempt to remove default locale
                string defaultLocale = PortalController.GetPortalDefaultLanguage(portalId);
                RemoveCacheKey(String.Format(DataCache.PortalCacheKey, portalId.ToString() + "-" + defaultLocale), clearRuntime);
            }
            else
            {
                foreach (Locale portalLocale in Localization.Localization.GetLocales(portalId).Values)
                {
                    RemoveCacheKey(String.Format(DataCache.PortalCacheKey, portalId.ToString() + "-" + portalLocale.Code), clearRuntime);
                }
            }
            if (cascade)
            {
                TabController objTabs = new TabController();
                foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(portalId))
                {
                    ClearModuleCacheInternal(tabPair.Value.TabID, clearRuntime);
                }
                ModuleController moduleController = new ModuleController();
                foreach (ModuleInfo moduleInfo in moduleController.GetModules(portalId))
                {
                    RemoveCacheKey("GetModuleSettings" + moduleInfo.ModuleID.ToString(), clearRuntime);
                }
            }
            ClearFolderCacheInternal(portalId, clearRuntime);
            ClearCacheKeysByPortalInternal(portalId, clearRuntime);
            ClearDesktopModuleCacheInternal(portalId, clearRuntime);
            ClearTabCacheInternal(portalId, clearRuntime);
        }
        private void ClearTabCacheInternal(int portalId, bool clearRuntime)
        {
            RemoveFormattedCacheKey(DataCache.TabCacheKey, clearRuntime, portalId);
            RemoveFormattedCacheKey(DataCache.TabPathCacheKey, clearRuntime, portalId);
            RemoveFormattedCacheKey(DataCache.TabPermissionCacheKey, clearRuntime, portalId);
        }
        private void RemoveCacheKey(string CacheKey, bool clearRuntime)
        {
            if (clearRuntime)
            {
                RemoveInternal(GetCacheKey(CacheKey));
            }
            else
            {
                Remove(GetCacheKey(CacheKey));
            }
        }
        private void RemoveFormattedCacheKey(string CacheKeyBase, bool clearRuntime, params object[] parameters)
        {
            if (clearRuntime)
            {
                RemoveInternal(string.Format(GetCacheKey(CacheKeyBase), parameters));
            }
            else
            {
                Remove(string.Format(GetCacheKey(CacheKeyBase), parameters));
            }
        }
        protected void ClearCacheInternal(string cacheType, string data, bool clearRuntime)
        {
            switch (cacheType)
            {
                case "Prefix":
                    ClearCacheInternal(data, clearRuntime);
                    break;
                case "Host":
                    ClearHostCacheInternal(clearRuntime);
                    break;
                case "Folder":
                    ClearFolderCacheInternal(int.Parse(data), clearRuntime);
                    break;
                case "Module":
                    ClearModuleCacheInternal(int.Parse(data), clearRuntime);
                    break;
                case "ModulePermissionsByPortal":
                    ClearModulePermissionsCachesByPortalInternal(int.Parse(data), clearRuntime);
                    break;
                case "Portal":
                    ClearPortalCacheInternal(int.Parse(data), false, clearRuntime);
                    break;
                case "PortalCascade":
                    ClearPortalCacheInternal(int.Parse(data), true, clearRuntime);
                    break;
                case "Tab":
                    ClearTabCacheInternal(int.Parse(data), clearRuntime);
                    break;
            }
        }
        protected void RemoveInternal(string CacheKey)
        {
            DataCache.RemoveFromPrivateDictionary(CacheKey);
            if (Cache[CacheKey] != null)
            {
                Cache.Remove(CacheKey);
            }
        }
        public virtual void Clear(string type, string data)
        {
            ClearCacheInternal(type, data, false);
        }
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            return Cache.GetEnumerator();
        }
        public virtual object GetItem(string CacheKey)
        {
            return Cache[CacheKey];
        }
        public virtual void Insert(string CacheKey, object objObject)
        {
            CacheDependency objDependency = null;
            Insert(CacheKey, objObject, objDependency, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }
        public virtual void Insert(string CacheKey, object objObject, CacheDependency objDependency)
        {
            Insert(CacheKey, objObject, objDependency, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }
        public virtual void Insert(string CacheKey, object objObject, CacheDependency objDependency, System.DateTime AbsoluteExpiration, System.TimeSpan SlidingExpiration)
        {
            Insert(CacheKey, objObject, objDependency, AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Default, null);
        }
        public virtual void Insert(string CacheKey, object Value, CacheDependency objDependency, System.DateTime AbsoluteExpiration, System.TimeSpan SlidingExpiration, CacheItemPriority Priority, CacheItemRemovedCallback OnRemoveCallback)
        {
            if (objDependency == null)
            {
                Cache.Insert(CacheKey, Value, null, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback);
            }
            else
            {
                Cache.Insert(CacheKey, Value, objDependency.SystemCacheDependency, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback);
            }
        }
        public virtual bool IsWebFarm()
        {
            return (ServerController.GetEnabledServers().Count > 1);
        }
        public virtual string PurgeCache()
        {
            return Localization.Localization.GetString("PurgeCacheUnsupported.Text", Localization.Localization.GlobalResourceFile);
        }
        public virtual void Remove(string CacheKey)
        {
            RemoveInternal(CacheKey);
        }
    }
}
