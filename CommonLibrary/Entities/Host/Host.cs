using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using CommonLibrary.Common.Utilities;
using System.Data;
using CommonLibrary.Data;
using CommonLibrary.Common;
using System.Web;
using CommonLibrary.Framework;
using CommonLibrary.Services.Scheduling;
using CommonLibrary.UI.Skins;

namespace CommonLibrary.Entities.Host
{
    public class Host : BaseEntityInfo
    {
        private static bool GetHostSettingAsBoolean(string key, bool defaultValue)
        {
            bool retValue = false;
            try
            {
                string setting = GetHostSetting(key);
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
        private static double GetHostSettingAsDouble(string key, double defaultValue)
        {
            double retValue = 0;
            try
            {
                string setting = GetHostSetting(key);
                if (string.IsNullOrEmpty(setting))
                {
                    retValue = defaultValue;
                }
                else
                {
                    retValue = Convert.ToDouble(setting);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return retValue;
        }
        private static int GetHostSettingAsInteger(string key)
        {
            return GetHostSettingAsInteger(key, Null.NullInteger);
        }
        private static int GetHostSettingAsInteger(string key, int defaultValue)
        {
            int retValue = 0;
            try
            {
                string setting = GetHostSetting(key);
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
        private static string GetHostSettingAsString(string key, string defaultValue)
        {
            string retValue = defaultValue;
            try
            {
                string setting = GetHostSetting(key);
                if (!string.IsNullOrEmpty(setting))
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
        private static object GetSecureHostSettingsDictionaryCallBack(CacheItemArgs cacheItemArgs)
        {
            Dictionary<string, string> dicSettings = new Dictionary<string, string>();
            IDataReader dr = DataProvider.Instance().GetHostSettings();
            try
            {
                while (dr.Read())
                {
                    if (Convert.ToInt32(dr[2]) < 1)
                    {
                        string settingName = dr.GetString(0);
                        if (settingName.ToLower().IndexOf("password") == -1)
                        {
                            if (!dr.IsDBNull(1))
                            {
                                dicSettings.Add(settingName, dr.GetString(1));
                            }
                            else
                            {
                                dicSettings.Add(settingName, "");
                            }
                        }
                    }
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return dicSettings;
        }
        public static int AutoAccountUnlockDuration
        {
            get { return GetHostSettingAsInteger("AutoAccountUnlockDuration", 10); }
        }
        public static string AuthenticatedCacheability
        {
            get { return GetHostSettingAsString("AuthenticatedCacheability", "4"); }
        }
        public static bool CheckUpgrade
        {
            get { return GetHostSettingAsBoolean("CheckUpgrade", true); }
        }
        public static string ControlPanel
        {
            get
            {
                string setting = GetHostSetting("ControlPanel");
                if (string.IsNullOrEmpty(setting))
                {
                    setting = Globals.glbDefaultControlPanel;
                }
                return setting;
            }
        }
        public static string DefaultAdminContainer
        {
            get
            {
                string setting = GetHostSetting("DefaultAdminContainer");
                if (string.IsNullOrEmpty(setting))
                {
                    setting = SkinController.GetDefaultAdminContainer();
                }
                return setting;
            }
        }
        public static string DefaultAdminSkin
        {
            get
            {
                string setting = GetHostSetting("DefaultAdminSkin");
                if (string.IsNullOrEmpty(setting))
                {
                    setting = SkinController.GetDefaultAdminSkin();
                }
                return setting;
            }
        }
        public static string DefaultDocType
        {
            get
            {
                string doctype = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
                string setting = GetHostSetting("DefaultDocType");
                if (!string.IsNullOrEmpty(setting))
                {
                    switch (setting)
                    {
                        case "0":
                            doctype = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
                            break;
                        case "1":
                            doctype = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
                            break;
                        case "2":
                            doctype = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">";
                            break;
                    }
                }
                return doctype;
            }
        }
        public static string DefaultPortalContainer
        {
            get
            {
                string setting = GetHostSetting("DefaultPortalContainer");
                if (string.IsNullOrEmpty(setting))
                {
                    setting = SkinController.GetDefaultPortalContainer();
                }
                return setting;
            }
        }
        public static string DefaultPortalSkin
        {
            get
            {
                string setting = GetHostSetting("DefaultPortalSkin");
                if (string.IsNullOrEmpty(setting))
                {
                    setting = SkinController.GetDefaultPortalSkin();
                }
                return setting;
            }
        }
        public static int DemoPeriod
        {
            get { return GetHostSettingAsInteger("DemoPeriod", Null.NullInteger); }
        }
        public static bool DemoSignup
        {
            get { return GetHostSettingAsBoolean("DemoSignup", false); }
        }
        public static bool DisplayBetaNotice
        {
            get { return GetHostSettingAsBoolean("DisplayBetaNotice", true); }
        }
        public static bool DisplayCopyright
        {
            get { return GetHostSettingAsBoolean("Copyright", true); }
        }
        [Obsolete("MS AJax is now required for DotNetNuke 5.0 and above")]
        public static bool EnableAJAX
        {
            get { return GetHostSettingAsBoolean("EnableAJAX", true); }
        }
        public static bool EnableFileAutoSync
        {
            get { return GetHostSettingAsBoolean("EnableFileAutoSync", false); }
        }
        public static bool ContentLocalization
        {
            get { return GetHostSettingAsBoolean("ContentLocalization", false); }
        }
        [Obsolete("property obsoleted in 5.4.0 - code updated to use portalcontroller method")]
        public static string ContentLocale
        {
            get { return "en-us"; }
        }
        public static bool EnableBrowserLanguage
        {
            get { return GetHostSettingAsBoolean("EnableBrowserLanguage", true); }
        }
        public static bool EnableModuleOnLineHelp
        {
            get { return GetHostSettingAsBoolean("EnableModuleOnLineHelp", false); }
        }
        public static bool EnableRequestFilters
        {
            get { return GetHostSettingAsBoolean("EnableRequestFilters", false); }
        }
        public static bool EnableUrlLanguage
        {
            get { return GetHostSettingAsBoolean("EnableUrlLanguage", true); }
        }
        public static bool EnableUsersOnline
        {
            get { return !GetHostSettingAsBoolean("DisableUsersOnline", true); }
        }
        public static bool EnableSMTPSSL
        {
            get { return GetHostSettingAsBoolean("SMTPEnableSSL", false); }
        }
        public static bool EventLogBuffer
        {
            get { return GetHostSettingAsBoolean("EventLogBuffer", false); }
        }
        public static string FileExtensions
        {
            get { return GetHostSetting("FileExtensions"); }
        }
        public static string GUID
        {
            get { return GetHostSetting("GUID"); }
        }
        public static string HelpURL
        {
            get { return GetHostSetting("HelpURL"); }
        }
        public static string HostCurrency
        {
            get
            {
                string setting = GetHostSetting("HostCurrency");
                if (string.IsNullOrEmpty(setting))
                {
                    setting = "USD";
                }
                return setting;
            }
        }
        public static string HostEmail
        {
            get { return GetHostSetting("HostEmail"); }
        }
        public static double HostFee
        {
            get { return GetHostSettingAsDouble("HostFee", 0); }
        }
        public static int HostPortalID
        {
            get { return GetHostSettingAsInteger("HostPortalId"); }
        }
        public static double HostSpace
        {
            get { return GetHostSettingAsDouble("HostSpace", 0); }
        }
        public static string HostTitle
        {
            get { return GetHostSetting("HostTitle"); }
        }
        public static string HostURL
        {
            get { return GetHostSetting("HostURL"); }
        }
        public static int HttpCompressionAlgorithm
        {
            get { return GetHostSettingAsInteger("HttpCompression"); }
        }
        public static string ModuleCachingMethod
        {
            get { return GetHostSetting("ModuleCaching"); }
        }
        public static string PageCachingMethod
        {
            get { return GetHostSetting("PageCaching"); }
        }
        public static int PageQuota
        {
            get { return GetHostSettingAsInteger("PageQuota", 0); }
        }
        public static string PageStatePersister
        {
            get
            {
                string setting = GetHostSetting("PageStatePersister");
                if (string.IsNullOrEmpty(setting))
                {
                    setting = "P";
                }
                return setting;
            }
        }
        public static int PasswordExpiry
        {
            get { return GetHostSettingAsInteger("PasswordExpiry", 0); }
        }
        public static int PasswordExpiryReminder
        {
            get { return GetHostSettingAsInteger("PasswordExpiryReminder", 7); }
        }
        public static string PaymentProcessor
        {
            get { return GetHostSetting("PaymentProcessor"); }
        }
        public static Globals.PerformanceSettings PerformanceSetting
        {
            get
            {
                Globals.PerformanceSettings setting = Globals.PerformanceSettings.ModerateCaching;
                string s = GetHostSetting("PerformanceSetting");
                if (!string.IsNullOrEmpty(s))
                {
                    setting = (Globals.PerformanceSettings)Enum.Parse(typeof(Globals.PerformanceSettings), s);
                }
                return setting;
            }
        }
        public static string ProcessorPassword
        {
            get { return GetHostSetting("ProcessorPassword"); }
        }
        public static string ProcessorUserId
        {
            get { return GetHostSetting("ProcessorUserId"); }
        }
        public static string ProxyPassword
        {
            get { return GetHostSetting("ProxyPassword"); }
        }
        public static int ProxyPort
        {
            get { return GetHostSettingAsInteger("ProxyPort"); }
        }
        public static string ProxyServer
        {
            get { return GetHostSetting("ProxyServer"); }
        }
        public static string ProxyUsername
        {
            get { return GetHostSetting("ProxyUsername"); }
        }
        public static bool RememberCheckbox
        {
            get { return GetHostSettingAsBoolean("RememberCheckbox", true); }
        }
        public static SchedulerMode SchedulerMode
        {
            get
            {
                SchedulerMode setting = SchedulerMode.TIMER_METHOD;
                string s = GetHostSetting("SchedulerMode");
                if (!string.IsNullOrEmpty(s))
                {
                    setting = (Services.Scheduling.SchedulerMode)Enum.Parse(typeof(Services.Scheduling.SchedulerMode), s);
                }
                return setting;
            }
        }
        public static bool SearchIncludeCommon
        {
            get { return GetHostSettingAsBoolean("SearchIncludeCommon", false); }
        }
        public static bool SearchIncludeNumeric
        {
            get { return GetHostSettingAsBoolean("SearchIncludeNumeric", true); }
        }
        public static int SearchMaxWordlLength
        {
            get { return GetHostSettingAsInteger("MaxSearchWordLength", 50); }
        }
        public static int SearchMinWordlLength
        {
            get { return GetHostSettingAsInteger("MinSearchWordLength", 4); }
        }
        public static int SiteLogBuffer
        {
            get { return GetHostSettingAsInteger("SiteLogBuffer", 1); }
        }
        public static int SiteLogHistory
        {
            get { return GetHostSettingAsInteger("SiteLogHistory", Null.NullInteger); }
        }
        public static string SiteLogStorage
        {
            get
            {
                string setting = GetHostSetting("SiteLogStorage");
                if (string.IsNullOrEmpty(setting))
                {
                    setting = "D";
                }
                return setting;
            }
        }
        public static string SMTPAuthentication
        {
            get { return GetHostSetting("SMTPAuthentication"); }
        }
        public static string SMTPPassword
        {
            get { return GetHostSetting("SMTPPassword"); }
        }
        public static string SMTPServer
        {
            get { return GetHostSetting("SMTPServer"); }
        }
        public static string SMTPUsername
        {
            get { return GetHostSetting("SMTPUsername"); }
        }
        public static bool ThrowCBOExceptions
        {
            get { return GetHostSettingAsBoolean("ThrowCBOExceptions", false); }
        }
        public static bool UseFriendlyUrls
        {
            get { return GetHostSettingAsBoolean("UseFriendlyUrls", false); }
        }
        public static bool UseCustomErrorMessages
        {
            get { return GetHostSettingAsBoolean("UseCustomErrorMessages", false); }
        }
        public static int UserQuota
        {
            get { return GetHostSettingAsInteger("UserQuota", 0); }
        }
        public static int UsersOnlineTimeWindow
        {
            get { return GetHostSettingAsInteger("UsersOnlineTime", 15); }
        }
        public static int WebRequestTimeout
        {
            get { return GetHostSettingAsInteger("WebRequestTimeout", 10000); }
        }
        public static bool WhitespaceFilter
        {
            get { return GetHostSettingAsBoolean("WhitespaceFilter", false); }
        }
        public static bool jQueryDebug
        {
            get { return GetHostSettingAsBoolean("jQueryDebug", false); }
        }
        public static bool jQueryHosted
        {
            get { return GetHostSettingAsBoolean("jQueryHosted", false); }
        }
        public static string jQueryUrl
        {
            get
            {
                if (HttpContext.Current.Request.IsSecureConnection)
                {
                    return GetHostSettingAsString("jQueryUrl", jQuery.DefaultHostedUrl).Replace("http://", "https://");
                }
                else
                {
                    return GetHostSettingAsString("jQueryUrl", jQuery.DefaultHostedUrl);
                }
            }
        }
        private static string GetHostSetting(string key)
        {
            string setting = Null.NullString;
            if (Globals.DataBaseVersion != null)
            {
                GetHostSettingsDictionary().TryGetValue(key, out setting);
            }
            return setting;
        }
        public static Dictionary<string, string> GetHostSettingsDictionary()
        {
            Dictionary<string, string> dicSettings = DataCache.GetCache<Dictionary<string, string>>(DataCache.HostSettingsCacheKey);
            if (dicSettings == null)
            {
                dicSettings = new Dictionary<string, string>();
                IDataReader dr = DataProvider.Instance().GetHostSettings();
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
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
                CacheDependency objDependency = null;
                //DataCache.SetCache(DataCache.HostSettingsCacheKey, dicSettings, objDependency, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(DataCache.HostSettingsCacheTimeOut), DataCache.HostSettingsCachePriority, null);
            }
            return dicSettings;
        }
        public static string GetSecureHostSetting(string key)
        {
            string setting = Null.NullString;
            GetSecureHostSettingsDictionary().TryGetValue(key, out setting);
            return setting;
        }
        public static Dictionary<string, string> GetSecureHostSettingsDictionary()
        {
            return CBO.GetCachedObject<Dictionary<string, string>>(new CacheItemArgs(DataCache.SecureHostSettingsCacheKey, DataCache.HostSettingsCacheTimeOut, DataCache.HostSettingsCachePriority), GetSecureHostSettingsDictionaryCallBack);
        }
    }
}
