using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using CommonLibrary.Common.Utilities;
using System.IO;
using System.Xml.XPath;
using System.Web;
using CommonLibrary.Common;
using System.Web.UI;
using CommonLibrary.UI.Modules;
using CommonLibrary.Entities.Portal;
using System.Collections.Specialized;
using System.Web.Caching;
using System.Xml;
using CommonLibrary.Entities.Users;
using System.Collections;
using CommonLibrary.Services.Tokens;
using System.Globalization;
using CommonLibrary.Data;
using System.Threading;
using CommonLibrary.Entities.Host;

namespace CommonLibrary.Services.Localization
{
    [Serializable()]
    public enum CultureDropDownTypes
    {
        DisplayName,
        EnglishName,
        Lcid,
        Name,
        NativeName,
        TwoLetterIsoCode,
        ThreeLetterIsoCode
    }

    public class Localization
    {
        private static object objLock = new object();
        private static string _defaultKeyName = "resourcekey";
        private static ListItem[] _timeZoneListItems;
        private static string strShowMissingKeys = "";
        private static string strUseBrowserLanguageDefault = "";
        private static string strUseLanguageInUrlDefault = "";
        public static string ApplicationResourceDirectory
        {
            get { return "~/App_GlobalResources"; }
        }
        public static string ExceptionsResourceFile
        {
            get
            {
                return ApplicationResourceDirectory + "/Exceptions.resx";
            }
        }
        public static string GlobalResourceFile
        {
            get { return ApplicationResourceDirectory + "/GlobalResources.resx"; }
        }
        public static string LocalResourceDirectory
        {
            get { return "App_LocalResources"; }
        }
        public static string LocalSharedResourceFile
        {
            get { return "SharedResources.resx"; }
        }
        public static string SharedResourceFile
        {
            get { return ApplicationResourceDirectory + "/SharedResources.resx"; }
        }
        public static string SupportedLocalesFile
        {
            get { return ApplicationResourceDirectory + "/Locales.xml"; }
        }
        public static string SystemLocale
        {
            get { return "en-US"; }
        }
        public static int SystemTimeZoneOffset
        {
            get { return -480; }
        }
        public static string TimezonesFile
        {
            get { return ApplicationResourceDirectory + "/TimeZones.xml"; }
        }
        private enum CustomizedLocale
        {
            None = 0,
            Portal = 1,
            Host = 2
        }
        public string CurrentCulture
        {
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.ToString(); }
        }
        public static string KeyName
        {
            get { return _defaultKeyName; }
            set
            {
                _defaultKeyName = value;
                if (_defaultKeyName == null || _defaultKeyName == string.Empty)
                {
                    _defaultKeyName = "resourcekey";
                }
            }
        }
        public static bool ShowMissingKeys
        {
            get
            {
                if (string.IsNullOrEmpty(strShowMissingKeys))
                {
                    if (Config.GetSetting("ShowMissingKeys") == null)
                    {
                        strShowMissingKeys = "false";
                    }
                    else
                    {
                        strShowMissingKeys = Config.GetSetting("ShowMissingKeys".ToLower());
                    }
                }
                return bool.Parse(strShowMissingKeys);
            }
        }
        private static object GetLocalesCallBack(CacheItemArgs cacheItemArgs)
        {
            try
            {
                int portalID = (int)cacheItemArgs.ParamList[0];
                Dictionary<string, Locale> locales;
                if (portalID > Null.NullInteger)
                {
                    locales = CBO.FillDictionary<string, Locale>("CultureCode", DataProvider.Instance().GetLanguagesByPortal(portalID), new Dictionary<string, Locale>());
                }
                else
                {
                    locales = CBO.FillDictionary<string, Locale>("CultureCode", DataProvider.Instance().GetLanguages(), new Dictionary<string, Locale>());
                }
                return locales;
            }
            catch
            {
                return null;
            }
        }
        private static object GetResourceFileLookupDictionary(CacheItemArgs cacheItemArgs)
        {
            return new Dictionary<string, bool>();
        }
        private static Dictionary<string, bool> GetResourceFileLookupDictionary()
        {
            return CBO.GetCachedObject<Dictionary<string, bool>>(new CacheItemArgs(DataCache.ResourceFileLookupDictionaryCacheKey, DataCache.ResourceFileLookupDictionaryTimeOut, DataCache.ResourceFileLookupDictionaryCachePriority), GetResourceFileLookupDictionary, true);
        }
        private static object GetResourceFileCallBack(CacheItemArgs cacheItemArgs)
        {
            string cacheKey = cacheItemArgs.CacheKey;
            Dictionary<string, string> resources = null;
            Dictionary<string, bool> resourceFileExistsLookup = GetResourceFileLookupDictionary();
            if ((!resourceFileExistsLookup.ContainsKey(cacheKey)) || resourceFileExistsLookup[cacheKey])
            {
                string filePath = null;
                if (cacheKey.Contains(":\\") && Path.IsPathRooted(cacheKey))
                {
                    if (File.Exists(cacheKey))
                    {
                        filePath = cacheKey;
                    }
                }
                if (filePath == null)
                {
                    filePath = System.Web.Hosting.HostingEnvironment.MapPath(Globals.ApplicationPath + cacheKey);
                }
                if (File.Exists(filePath))
                {
                    XPathDocument doc = null;
                    doc = new XPathDocument(filePath);
                    resources = new Dictionary<string, string>();
                    foreach (XPathNavigator nav in doc.CreateNavigator().Select("root/data"))
                    {
                        if (nav.NodeType != XPathNodeType.Comment)
                        {
                            resources[nav.GetAttribute("name", string.Empty)] = nav.SelectSingleNode("value").Value;
                        }
                    }
                    cacheItemArgs.CacheDependency = new CacheDependency(filePath);
                    resourceFileExistsLookup[cacheKey] = true;
                }
                else
                {
                    resourceFileExistsLookup[cacheKey] = false;
                }
            }
            return resources;
        }
        private static Dictionary<string, string> GetResourceFile(string resourceFile)
        {
            return CBO.GetCachedObject<Dictionary<string, string>>(new CacheItemArgs(resourceFile, DataCache.ResourceFilesCacheTimeOut, DataCache.ResourceFilesCachePriority), GetResourceFileCallBack, true);
        }
        private static string GetResourceFileName(string ResourceFileRoot, string language)
        {
            string ResourceFile;
            language = language.ToLower();
            if (ResourceFileRoot != null)
            {
                if (language == SystemLocale.ToLower() || String.IsNullOrEmpty(language))
                {
                    switch (ResourceFileRoot.Substring(ResourceFileRoot.Length - 5, 5).ToLower())
                    {
                        case ".resx":
                            ResourceFile = ResourceFileRoot;
                            break;
                        case ".ascx":
                            ResourceFile = ResourceFileRoot + ".resx";
                            break;
                        case ".aspx":
                            ResourceFile = ResourceFileRoot + ".resx";
                            break;
                        default:
                            ResourceFile = ResourceFileRoot + ".ascx.resx";
                            break;
                    }
                }
                else
                {
                    switch (ResourceFileRoot.Substring(ResourceFileRoot.Length - 5, 5).ToLower())
                    {
                        case ".resx":
                            ResourceFile = ResourceFileRoot.Replace(".resx", "." + language + ".resx");
                            break;
                        case ".ascx":
                            ResourceFile = ResourceFileRoot.Replace(".ascx", ".ascx." + language + ".resx");
                            break;
                        case ".aspx":
                            ResourceFile = ResourceFileRoot.Replace(".aspx", ".aspx." + language + ".resx");
                            break;
                        default:
                            ResourceFile = ResourceFileRoot + ".ascx." + language + ".resx";
                            break;
                    }
                }
            }
            else
            {
                if (language == SystemLocale.ToLower() || String.IsNullOrEmpty(language))
                {
                    ResourceFile = SharedResourceFile;
                }
                else
                {
                    ResourceFile = SharedResourceFile.Replace(".resx", "." + language + ".resx");
                }
            }
            return ResourceFile;
        }
        private static string GetStringInternal(string key, string userLanguage, string resourceFileRoot, PortalSettings objPortalSettings, bool disableShowMissngKeys)
        {
            if (key.IndexOf(".") < 1)
            {
                key += ".Text";
            }
            string resourceValue = Null.NullString;
            bool bFound = TryGetStringInternal(key, userLanguage, resourceFileRoot, objPortalSettings, ref resourceValue);
            if (ShowMissingKeys && !disableShowMissngKeys)
            {
                if (bFound)
                {
                    resourceValue = "[L]" + resourceValue;
                }
                else
                {
                    resourceValue = "RESX:" + key;
                }
            }
            return resourceValue;
        }
        private static Dictionary<string, string> LoadResourceFileCallback(CacheItemArgs cacheItemArgs)
        {
            string fileName = (string)cacheItemArgs.ParamList[0];
            string filePath = HttpContext.Current.Server.MapPath(fileName);
            Dictionary<string, string> dicResources = new Dictionary<string, string>();
            if (File.Exists(filePath))
            {
                XPathDocument doc = null;
                try
                {
                    doc = new XPathDocument(filePath);
                    foreach (XPathNavigator nav in doc.CreateNavigator().Select("root/data"))
                    {
                        if (nav.NodeType != XPathNodeType.Comment)
                        {
                            dicResources[nav.GetAttribute("name", string.Empty)] = nav.SelectSingleNode("value").Value;
                        }
                    }
                }
                catch
                {
                }
            }
            return dicResources;
        }
        private static void LocalizeDataControlField(DataControlField controlField, string resourceFile)
        {
            string localizedText;
            if (!string.IsNullOrEmpty(controlField.HeaderText))
            {
                localizedText = GetString((controlField.HeaderText + ".Header"), resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    controlField.HeaderText = localizedText;
                    controlField.AccessibleHeaderText = controlField.HeaderText;
                }
            }
            if (controlField is TemplateField)
            {
            }
            else if (controlField is ButtonField)
            {
                ButtonField button = (ButtonField)controlField;
                localizedText = GetString(button.Text, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    button.Text = localizedText;
            }
            else if (controlField is CheckBoxField)
            {
                CheckBoxField checkbox = (CheckBoxField)controlField;
                localizedText = GetString(checkbox.Text, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    checkbox.Text = localizedText;
            }
            else if (controlField is CommandField)
            {
                CommandField commands = (CommandField)controlField;
                localizedText = GetString(commands.CancelText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    commands.CancelText = localizedText;
                localizedText = GetString(commands.DeleteText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    commands.DeleteText = localizedText;
                localizedText = GetString(commands.EditText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    commands.EditText = localizedText;
                localizedText = GetString(commands.InsertText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    commands.InsertText = localizedText;
                localizedText = GetString(commands.NewText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    commands.NewText = localizedText;
                localizedText = GetString(commands.SelectText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    commands.SelectText = localizedText;
                localizedText = GetString(commands.UpdateText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    commands.UpdateText = localizedText;
            }
            else if (controlField is HyperLinkField)
            {
                HyperLinkField link = (HyperLinkField)controlField;
                localizedText = GetString(link.Text, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    link.Text = localizedText;
            }
            else if (controlField is ImageField)
            {
                ImageField image = (ImageField)controlField;
                localizedText = GetString(image.AlternateText, resourceFile);
                if (!string.IsNullOrEmpty(localizedText))
                    image.AlternateText = localizedText;
            }
        }
        private string TimeZoneFile(string filename, string language)
        {
            if (language == Services.Localization.Localization.SystemLocale)
            {
                return filename;
            }
            else
            {
                return filename.Substring(0, filename.Length - 4) + "." + language + ".xml";
            }
        }
        private static bool TryGetFromResourceFile(string key, string resourceFile, string userLanguage, string fallbackLanguage, string defaultLanguage, int portalID, ref string resourceValue)
        {
            bool bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, userLanguage), portalID, ref resourceValue);
            if (!bFound && fallbackLanguage != userLanguage)
            {
                bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, fallbackLanguage), portalID, ref resourceValue);
            }
            if (!bFound && !(defaultLanguage == userLanguage || defaultLanguage == fallbackLanguage))
            {
                bFound = TryGetFromResourceFile(key, GetResourceFileName(resourceFile, defaultLanguage), portalID, ref resourceValue);
            }
            return bFound;
        }
        private static bool TryGetFromResourceFile(string key, string resourceFile, int portalID, ref string resourceValue)
        {
            bool bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.Portal, ref resourceValue);
            if (!bFound)
            {
                bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.Host, ref resourceValue);
            }
            if (!bFound)
            {
                bFound = TryGetFromResourceFile(key, resourceFile, portalID, CustomizedLocale.None, ref resourceValue);
            }
            return bFound;
        }
        private static bool TryGetFromResourceFile(string key, string resourceFile, int portalID, CustomizedLocale resourceType, ref string resourceValue)
        {
            Dictionary<string, string> dicResources = null;
            bool bFound = Null.NullBoolean;
            string resourceFileName = resourceFile;
            switch (resourceType)
            {
                case CustomizedLocale.Host:
                    resourceFileName = resourceFile.Replace(".resx", ".Host.resx");
                    break;
                case CustomizedLocale.Portal:
                    resourceFileName = resourceFile.Replace(".resx", ".Portal-" + portalID.ToString() + ".resx");
                    break;
            }
            if (resourceFileName.StartsWith("desktopmodules", StringComparison.InvariantCultureIgnoreCase) || resourceFileName.StartsWith("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                resourceFileName = "~/" + resourceFileName;
            }
            string cacheKey = resourceFileName.Replace("~/", "/").ToLowerInvariant();
            if (!string.IsNullOrEmpty(Globals.ApplicationPath))
            {
                if (Globals.ApplicationPath != "/portals")
                {
                    if (cacheKey.StartsWith(Globals.ApplicationPath))
                    {
                        cacheKey = cacheKey.Substring(Globals.ApplicationPath.Length);
                    }
                }
                else
                {
                    cacheKey = "~" + cacheKey;
                    if (cacheKey.StartsWith("~" + Globals.ApplicationPath))
                    {
                        cacheKey = cacheKey.Substring(Globals.ApplicationPath.Length + 1);
                    }
                }
            }
            Dictionary<string, bool> resourceFileExistsLookup = GetResourceFileLookupDictionary();
            if ((!resourceFileExistsLookup.ContainsKey(cacheKey)) || resourceFileExistsLookup[cacheKey])
            {
                dicResources = GetResourceFile(cacheKey);
                if (dicResources != null)
                {
                    bFound = dicResources.TryGetValue(key, out resourceValue);
                }
            }
            return bFound;
        }
        private static bool TryGetStringInternal(string key, string userLanguage, string resourceFile, PortalSettings objPortalSettings, ref string resourceValue)
        {
            string defaultLanguage = Null.NullString;
            string fallbackLanguage = SystemLocale;
            int portalId = Null.NullInteger;
            if (objPortalSettings != null)
            {
                defaultLanguage = objPortalSettings.DefaultLanguage;
                portalId = objPortalSettings.PortalId;
            }
            if (string.IsNullOrEmpty(userLanguage))
            {
                userLanguage = Thread.CurrentThread.CurrentCulture.ToString();
            }
            if (string.IsNullOrEmpty(userLanguage))
            {
                userLanguage = defaultLanguage;
            }
            Locale userLocale = null;
            try
            {
                userLocale = GetLocale(userLanguage);
            }
            catch
            {
            }

            if (userLocale != null && !string.IsNullOrEmpty(userLocale.Fallback))
            {
                fallbackLanguage = userLocale.Fallback;
            }
            if (string.IsNullOrEmpty(resourceFile))
            {
                resourceFile = SharedResourceFile;
            }
            bool bFound = TryGetFromResourceFile(key, resourceFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, ref resourceValue);
            if (!bFound)
            {
                if (!(SharedResourceFile.ToLowerInvariant() == resourceFile.ToLowerInvariant()))
                {
                    string localSharedFile = resourceFile.Substring(0, resourceFile.LastIndexOf("/") + 1) + Localization.LocalSharedResourceFile;
                    if (!(localSharedFile.ToLowerInvariant() == resourceFile.ToLowerInvariant()))
                    {
                        bFound = TryGetFromResourceFile(key, localSharedFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, ref resourceValue);
                    }
                }
            }
            if (!bFound)
            {
                if (!(SharedResourceFile.ToLowerInvariant() == resourceFile.ToLowerInvariant()))
                {
                    bFound = TryGetFromResourceFile(key, SharedResourceFile, userLanguage, fallbackLanguage, defaultLanguage, portalId, ref resourceValue);
                }
            }
            return bFound;
        }
        public string GetFixedCurrency(decimal Expression, string Culture, int NumDigitsAfterDecimal)
        {
            string oldCurrentCulture = CurrentCulture;
            System.Globalization.CultureInfo newCulture = new System.Globalization.CultureInfo(Culture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            string currencyStr = Expression.ToString(newCulture.NumberFormat.CurrencySymbol);
            System.Globalization.CultureInfo oldCulture = new System.Globalization.CultureInfo(oldCurrentCulture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = oldCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture;
            return currencyStr;
        }
        public string GetFixedDate(System.DateTime Expression, string Culture)
        {
            string oldCurrentCulture = CurrentCulture;
            System.Globalization.CultureInfo newCulture = new System.Globalization.CultureInfo(Culture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            string dateStr = Expression.ToString(newCulture.DateTimeFormat.FullDateTimePattern);
            System.Globalization.CultureInfo oldCulture = new System.Globalization.CultureInfo(oldCurrentCulture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = oldCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture;
            return dateStr;
        }
        public static int ActiveLanguagesByPortalID(int portalID)
        {
            //return count of cached object
            return Localization.GetLocales(portalID).Count;
        }
        public static void AddLanguageToPortal(int portalID, int languageID, bool clearCache)
        {
            DataProvider.Instance().AddPortalLanguage(portalID, languageID, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("portalID/languageID", portalID.ToString() + "/" + languageID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.LANGUAGETOPORTAL_CREATED);
            if (clearCache)
            {
                DataCache.ClearPortalCache(portalID, false);
            }
        }
        public static void AddLanguagesToPortal(int portalID)
        {
            foreach (Locale language in GetLocales(Null.NullInteger).Values)
            {
                AddLanguageToPortal(portalID, language.LanguageID, false);
            }
            DataCache.ClearPortalCache(portalID, true);
        }
        public static void AddLanguageToPortals(int languageID)
        {
            PortalController controller = new PortalController();
            foreach (PortalInfo portal in controller.GetPortals())
            {
                AddLanguageToPortal(portal.PortalID, languageID, false);
            }
            DataCache.ClearHostCache(true);
        }
        public static void DeleteLanguage(Locale language)
        {
            DataProvider.Instance().DeleteLanguage(language.LanguageID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(language, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGE_DELETED);
            DataCache.ClearHostCache(true);
        }

        public static string GetExceptionMessage(string key, string defaultValue)
        {
            if (HttpContext.Current == null)
            {
                return defaultValue;
            }
            else
            {
                return GetString(key, ExceptionsResourceFile);
            }
        }

        public static string GetExceptionMessage(string key, string defaultValue, params object[] @params)
        {
            if (HttpContext.Current == null)
            {
                return string.Format(defaultValue, @params);
            }
            else
            {
                return string.Format(GetString(key, ExceptionsResourceFile), @params);
            }
        }

        public static Locale GetLocale(string code)
        {
            Dictionary<string, Locale> dicLocales = GetLocales(Null.NullInteger);
            Locale language = null;
            if (dicLocales != null)
            {
                dicLocales.TryGetValue(code, out language);
            }
            return language;
        }
        public static Locale GetLocaleByID(int languageID)
        {
            Dictionary<string, Locale> dicLocales = GetLocales(Null.NullInteger);
            Locale language = null;
            foreach (KeyValuePair<string, Locale> kvp in dicLocales)
            {
                if (kvp.Value.LanguageID == languageID)
                {
                    language = kvp.Value;
                    break;
                }
            }
            return language;
        }
        public static Dictionary<string, Locale> GetLocales(int portalID)
        {
            Dictionary<string, Locale> locals = new Dictionary<string, Locale>();
            if (Globals.Status != Globals.UpgradeStatus.Install)
            {
                string cacheKey = string.Format(DataCache.LocalesCacheKey, portalID.ToString());
                locals = CBO.GetCachedObject<Dictionary<string, Locale>>(new CacheItemArgs(cacheKey, DataCache.LocalesCacheTimeOut, DataCache.LocalesCachePriority, portalID), GetLocalesCallBack, true);
            }
            return locals;
        }
        public static string GetResourceFileName(string resourceFileName, string language, string mode, int portalId)
        {
            if (!resourceFileName.EndsWith(".resx"))
            {
                resourceFileName += ".resx";
            }
            if (language != Localization.SystemLocale)
            {
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + language + ".resx";
            }
            if (mode == "Host")
            {
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + "Host.resx";
            }
            else if (mode == "Portal")
            {
                resourceFileName = resourceFileName.Substring(0, resourceFileName.Length - 5) + "." + "Portal-" + portalId.ToString() + ".resx";
            }
            return resourceFileName;
        }
        public static string GetResourceFile(Control Ctrl, string FileName)
        {
            return Ctrl.TemplateSourceDirectory + "/" + Services.Localization.Localization.LocalResourceDirectory + "/" + FileName;
        }
        public static CultureInfo GetPageLocale(PortalSettings portalSettings)
        {
            CultureInfo pageCulture = null;
            Dictionary<string, Locale> enabledLocales = null;
            if (portalSettings != null)
            {
                enabledLocales = Localization.GetLocales(portalSettings.PortalId);
            }
            string preferredLocale = "";
            string preferredLanguage = "";
            if (HttpContext.Current != null)
            {
                try
                {
                    preferredLocale = HttpContext.Current.Request["language"];
                    if (!String.IsNullOrEmpty(preferredLocale))
                    {
                        if (Services.Localization.Localization.LocaleIsEnabled(preferredLocale))
                        {
                            pageCulture = new CultureInfo(preferredLocale);
                        }
                        else
                        {
                            preferredLanguage = preferredLocale.Split('-')[0];
                        }
                    }
                }
                catch
                {
                }
            }
            if (pageCulture == null)
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                if (objUserInfo.UserID != -1)
                {
                    if (!String.IsNullOrEmpty(objUserInfo.Profile.PreferredLocale))
                    {
                        if (Localization.LocaleIsEnabled(preferredLocale))
                        {
                            pageCulture = new CultureInfo(objUserInfo.Profile.PreferredLocale);
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(preferredLanguage))
                            {
                                preferredLanguage = objUserInfo.Profile.PreferredLocale.Split('-')[0];
                            }
                        }
                    }
                }
            }
            if (pageCulture == null && portalSettings.EnableBrowserLanguage)
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Request.UserLanguages != null)
                    {
                        try
                        {
                            foreach (string userLang in HttpContext.Current.Request.UserLanguages)
                            {
                                string userlanguage = userLang.Split(';')[0];
                                if (Localization.LocaleIsEnabled(userlanguage))
                                {
                                    pageCulture = new CultureInfo(userlanguage);
                                }
                                else if (userLang.Split(';')[0].IndexOf("-") != -1)
                                {
                                    string templang = userLang.Split(';')[0];
                                    foreach (string _localeCode in enabledLocales.Keys)
                                    {
                                        if (_localeCode.Split('-')[0] == templang.Split('-')[0])
                                        {
                                            pageCulture = new CultureInfo(_localeCode);
                                            break;
                                        }
                                    }
                                }
                                if (pageCulture != null)
                                {
                                    break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            if (pageCulture == null && !String.IsNullOrEmpty(preferredLanguage))
            {
                foreach (string _localeCode in enabledLocales.Keys)
                {
                    if (_localeCode.Split('-')[0] == preferredLanguage)
                    {
                        pageCulture = new CultureInfo(_localeCode);
                        break;
                    }
                }
            }
            if (pageCulture == null)
            {
                if (String.IsNullOrEmpty(portalSettings.DefaultLanguage))
                {
                    if (enabledLocales.Count > 0)
                    {
                        foreach (string _localeCode in enabledLocales.Keys)
                        {
                            pageCulture = new CultureInfo(_localeCode);
                            break;
                        }
                    }
                    else
                    {
                        pageCulture = new CultureInfo(Services.Localization.Localization.SystemLocale);
                    }
                }
                else
                {
                    pageCulture = new CultureInfo(portalSettings.DefaultLanguage);
                }
            }
            if (pageCulture == null)
            {
                pageCulture = new CultureInfo(Services.Localization.Localization.SystemLocale);
            }
            Localization.SetLanguage(pageCulture.Name);
            return pageCulture;
        }
        public static string GetString(string key, Control ctrl)
        {
            Control parentControl = ctrl.Parent;
            string localizedText;
            IModuleControl moduleControl = parentControl as IModuleControl;
            if (moduleControl == null)
            {
                System.Reflection.PropertyInfo pi = parentControl.GetType().GetProperty("LocalResourceFile");
                if (pi != null)
                {
                    localizedText = GetString(key, pi.GetValue(parentControl, null).ToString());
                }
                else
                {
                    localizedText = GetString(key, parentControl);
                }
            }
            else
            {
                localizedText = GetString(key, moduleControl.LocalResourceFile);
            }
            return localizedText;
        }
        public static string GetString(string name)
        {
            return GetString(name, null, PortalController.GetCurrentPortalSettings(), null, false);
        }
        public static string GetString(string name, PortalSettings objPortalSettings)
        {
            return GetString(name, null, objPortalSettings, null, false);
        }
        public static string GetString(string name, string ResourceFileRoot, bool disableShowMissingKeys)
        {
            return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), null, disableShowMissingKeys);
        }
        public static string GetString(string name, string ResourceFileRoot)
        {
            return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), null, false);
        }
        public static string GetString(string name, string ResourceFileRoot, string strlanguage)
        {
            return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), strlanguage, false);
        }
        public static string GetString(string name, string ResourceFileRoot, PortalSettings objPortalSettings, string strLanguage)
        {
            return GetString(name, ResourceFileRoot, objPortalSettings, strLanguage, false);
        }
        public static string GetString(string key, string resourceFileRoot, PortalSettings objPortalSettings, string userLanguage, bool disableShowMissingKeys)
        {
            return GetStringInternal(key, userLanguage, resourceFileRoot, objPortalSettings, disableShowMissingKeys);
        }
        public static string GetStringUrl(string name, string ResourceFileRoot)
        {
            return GetString(name, ResourceFileRoot, PortalController.GetCurrentPortalSettings(), null, true);
        }
        public static string GetSystemMessage(PortalSettings objPortal, string MessageName)
        {
            return GetSystemMessage(null, objPortal, MessageName, null, GlobalResourceFile, null);
        }
        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, UserInfo objUser)
        {
            return GetSystemMessage(null, objPortal, MessageName, objUser, GlobalResourceFile, null);
        }
        public static string GetSystemMessage(string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser)
        {
            return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, GlobalResourceFile, null);
        }
        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, string ResourceFile)
        {
            return GetSystemMessage(null, objPortal, MessageName, null, ResourceFile, null);
        }
        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile)
        {
            return GetSystemMessage(null, objPortal, MessageName, objUser, ResourceFile, null);
        }
        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, string ResourceFile, ArrayList Custom)
        {
            return GetSystemMessage(null, objPortal, MessageName, null, ResourceFile, Custom);
        }
        public static string GetSystemMessage(PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList Custom)
        {
            return GetSystemMessage(null, objPortal, MessageName, objUser, ResourceFile, Custom);
        }
        public static string GetSystemMessage(string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList Custom)
        {
            return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, ResourceFile, Custom, null, "", -1);
        }
        public static string GetSystemMessage(string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList Custom, string CustomCaption, int AccessingUserID)
        {
            return GetSystemMessage(strLanguage, objPortal, MessageName, objUser, ResourceFile, Custom, null, CustomCaption, AccessingUserID);
        }
        public static string GetSystemMessage(string strLanguage, PortalSettings objPortal, string MessageName, UserInfo objUser, string ResourceFile, ArrayList CustomArray, IDictionary CustomDictionary, string CustomCaption, int AccessingUserID)
        {
            string strMessageValue;
            strMessageValue = GetString(MessageName, ResourceFile, objPortal, strLanguage);
            if (!String.IsNullOrEmpty(strMessageValue))
            {
                if (String.IsNullOrEmpty(CustomCaption))
                {
                    CustomCaption = "Custom";
                }
                Services.Tokens.TokenReplace objTokenReplace = new Services.Tokens.TokenReplace(Scope.SystemMessages, strLanguage, objPortal, objUser);
                if ((AccessingUserID != -1) && (objUser != null))
                {
                    if (objUser.UserID != AccessingUserID)
                    {
                        objTokenReplace.AccessingUser = new UserController().GetUser(objPortal.PortalId, AccessingUserID);
                    }
                }
                if (CustomArray != null)
                {
                    strMessageValue = objTokenReplace.ReplaceEnvironmentTokens(strMessageValue, CustomArray, CustomCaption);
                }
                else
                {
                    strMessageValue = objTokenReplace.ReplaceEnvironmentTokens(strMessageValue, CustomDictionary, CustomCaption);
                }
            }
            return strMessageValue;
        }
        public static NameValueCollection GetTimeZones(string language)
        {
            language = language.ToLower();
            string cacheKey = "dotnetnuke-" + language + "-timezones";
            string TranslationFile;
            if (language == Services.Localization.Localization.SystemLocale.ToLower())
            {
                TranslationFile = Services.Localization.Localization.TimezonesFile;
            }
            else
            {
                TranslationFile = Services.Localization.Localization.TimezonesFile.Replace(".xml", "." + language + ".xml");
            }
            NameValueCollection timeZones = (NameValueCollection)DataCache.GetCache(cacheKey);
            if (timeZones == null)
            {
                string filePath = HttpContext.Current.Server.MapPath(TranslationFile);
                timeZones = new NameValueCollection();
                if (File.Exists(filePath) == false)
                {
                    return timeZones;
                }
                CacheDependency dp = new CacheDependency(filePath);
                try
                {
                    XmlDocument d = new XmlDocument();
                    d.Load(filePath);
                    foreach (XmlNode n in d.SelectSingleNode("root").ChildNodes)
                    {
                        if (n.NodeType != XmlNodeType.Comment)
                        {
                            timeZones.Add(n.Attributes["name"].Value, n.Attributes["key"].Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                //if (Host.PerformanceSetting != Common.Globals.PerformanceSettings.NoCaching)
                //{
                //    DataCache.SetCache(cacheKey, timeZones, dp);
                //}
            }
            return timeZones;
        }
        /// <summary>
        /// <para>LoadCultureDropDownList loads a DropDownList with the list of supported cultures
        /// based on the languages defined in the supported locales file, for the current portal</para>
        /// </summary>
        /// <param name="list">DropDownList to load</param>
        /// <param name="displayType">Format of the culture to display. Must be one the CultureDropDownTypes values.
        /// <see cref="CultureDropDownTypes"/> for list of allowable values</param>
        /// <param name="selectedValue">Name of the default culture to select</param>
        public static void LoadCultureDropDownList(DropDownList list, CultureDropDownTypes displayType, string selectedValue)
        {
            LoadCultureDropDownList(list, displayType, selectedValue, "", false);
        }
        /// <summary>
        /// <para>LoadCultureDropDownList loads a DropDownList with the list of supported cultures
        /// based on the languages defined in the supported locales file. </para>
        /// <para>This overload allows us to display all installed languages. To do so, pass the value True to the Host parameter</para>
        /// </summary>
        /// <param name="list">DropDownList to load</param>
        /// <param name="displayType">Format of the culture to display. Must be one the CultureDropDownTypes values.
        /// <see cref="CultureDropDownTypes"/> for list of allowable values</param>
        /// <param name="selectedValue">Name of the default culture to select</param>
        /// <param name="Host">Boolean that defines wether or not to load host (ie. all available) locales</param>
        public static void LoadCultureDropDownList(DropDownList list, CultureDropDownTypes displayType, string selectedValue, bool Host)
        {
            LoadCultureDropDownList(list, displayType, selectedValue, "", Host);
        }
        /// <summary>
        /// <para>LoadCultureDropDownList loads a DropDownList with the list of supported cultures
        /// based on the languages defined in the supported locales file</para>
        /// <para>This overload allows us to filter a language from the dropdown. To do so pass a language code to the Filter parameter</para>
        /// <para>This overload allows us to display all installed languages. To do so, pass the value True to the Host parameter</para>
        /// </summary>
        /// <param name="list">DropDownList to load</param>
        /// <param name="displayType">Format of the culture to display. Must be one the CultureDropDownTypes values.
        /// <see cref="CultureDropDownTypes"/> for list of allowable values</param>
        /// <param name="selectedValue">Name of the default culture to select</param>
        /// <param name="Filter">Stringvalue that allows for filtering out a specifiec language</param>
        /// <param name="Host">Boolean that defines wether or not to load host (ie. all available) locales</param>
        public static void LoadCultureDropDownList(DropDownList list, CultureDropDownTypes displayType, string selectedValue, string Filter, bool Host)
        {
            PortalSettings objPortalSettings = PortalController.GetCurrentPortalSettings();
            Dictionary<string, Locale> enabledLanguages;
            if (Host)
            {
                enabledLanguages = Localization.GetLocales(Null.NullInteger);
            }
            else
            {
                enabledLanguages = Localization.GetLocales(objPortalSettings.PortalId);
            }
            ListItem[] _cultureListItems = new ListItem[enabledLanguages.Count];
            CultureDropDownTypes _cultureListItemsType = displayType;
            int intAdded = 0;
            foreach (KeyValuePair<string, Locale> kvp in enabledLanguages)
            {
                if (kvp.Value.Code != Filter)
                {
                    CultureInfo info = CultureInfo.CreateSpecificCulture(kvp.Value.Code);
                    ListItem item = new ListItem();
                    item.Value = kvp.Value.Code;
                    switch (displayType)
                    {
                        case CultureDropDownTypes.EnglishName:
                            item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.EnglishName);
                            break;
                        case CultureDropDownTypes.Lcid:
                            item.Text = info.LCID.ToString();
                            break;
                        case CultureDropDownTypes.Name:
                            item.Text = info.Name;
                            break;
                        case CultureDropDownTypes.NativeName:
                            item.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.NativeName);
                            break;
                        case CultureDropDownTypes.TwoLetterIsoCode:
                            item.Text = info.TwoLetterISOLanguageName;
                            break;
                        case CultureDropDownTypes.ThreeLetterIsoCode:
                            item.Text = info.ThreeLetterISOLanguageName;
                            break;
                        default:
                            item.Text = info.DisplayName;
                            break;
                    }
                    _cultureListItems[intAdded] = item;
                    intAdded += 1;
                }
            }
            if (list.Items.Count > 0)
            {
                list.Items.Clear();
            }
            Array.Resize(ref _cultureListItems, intAdded);
            list.Items.AddRange(_cultureListItems);
            if (selectedValue != null)
            {
                ListItem item = list.Items.FindByValue(selectedValue);
                if (item != null)
                {
                    list.SelectedIndex = -1;
                    item.Selected = true;
                }
            }
        }
        public static void LoadTimeZoneDropDownList(DropDownList list, string language, string selectedValue)
        {
            NameValueCollection timeZones = GetTimeZones(language);
            if (timeZones.Count == 0)
            {
                timeZones = GetTimeZones(Services.Localization.Localization.SystemLocale.ToLower());
            }
            int i;
            for (i = 0; i <= timeZones.Keys.Count - 1; i++)
            {
                list.Items.Add(new ListItem(timeZones.GetKey(i).ToString(), timeZones.Get(i).ToString()));
            }
            if (selectedValue != null)
            {
                ListItem item = list.Items.FindByValue(selectedValue);
                if (item == null)
                {
                    item = list.Items.FindByValue(SystemTimeZoneOffset.ToString());
                }
                if (item != null)
                {
                    list.SelectedIndex = -1;
                    item.Selected = true;
                }
            }
        }
        public static bool LocaleIsEnabled(Locale locale)
        {
            return LocaleIsEnabled(locale.Code);
        }
        public static bool LocaleIsEnabled(string localeCode)
        {
            try
            {
                bool isEnabled = false;
                PortalSettings _Settings = PortalController.GetCurrentPortalSettings();
                Dictionary<string, Locale> dicLocales = GetLocales(_Settings.PortalId);
                if (!dicLocales.ContainsKey(localeCode))
                {
                    isEnabled = false;
                }
                else if (dicLocales[localeCode] == null)
                {
                    if (localeCode.IndexOf("-") == -1)
                    {
                        foreach (string strLocale in dicLocales.Keys)
                        {
                            if (strLocale.Split('-')[0] == localeCode)
                            {
                                localeCode = strLocale;
                                isEnabled = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    isEnabled = true;
                }
                return isEnabled;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }
        public static string LocalizeControlTitle(IModuleControl moduleControl)
        {
            string controlTitle = moduleControl.ModuleContext.Configuration.ModuleTitle;
            string controlKey = moduleControl.ModuleContext.Configuration.ModuleControl.ControlKey.ToLower();
            if (string.IsNullOrEmpty(controlTitle) && !string.IsNullOrEmpty(controlKey))
            {
                string reskey;
                reskey = "ControlTitle_" + moduleControl.ModuleContext.Configuration.ModuleControl.ControlKey.ToLower() + ".Text";
                string localizedvalue = Services.Localization.Localization.GetString(reskey, moduleControl.LocalResourceFile);
                if (localizedvalue != null)
                {
                    controlTitle = localizedvalue;
                }
            }
            return controlTitle;
        }
        public static void LocalizeDataGrid(ref DataGrid grid, string ResourceFile)
        {
            string localizedText;
            foreach (DataGridColumn col in grid.Columns)
            {
                if (!string.IsNullOrEmpty(col.HeaderText))
                {
                    localizedText = GetString(col.HeaderText + ".Header", ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                    {
                        col.HeaderText = localizedText;
                    }
                }
                if (col is EditCommandColumn)
                {
                    EditCommandColumn editCol = (EditCommandColumn)col;
                    localizedText = GetString(editCol.EditText + ".EditText", ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                        editCol.EditText = localizedText;
                    localizedText = GetString(editCol.EditText, ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                        editCol.EditText = localizedText;
                    localizedText = GetString(editCol.CancelText, ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                        editCol.CancelText = localizedText;
                    localizedText = GetString(editCol.UpdateText, ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                        editCol.UpdateText = localizedText;
                }
                else if (col is ButtonColumn)
                {
                    ButtonColumn buttonCol = (ButtonColumn)col;
                    localizedText = GetString(buttonCol.Text, ResourceFile);
                    if (!String.IsNullOrEmpty(localizedText))
                        buttonCol.Text = localizedText;
                }
            }
        }
        public static void LocalizeDetailsView(ref DetailsView detailsView, string resourceFile)
        {
            foreach (DataControlField field in detailsView.Fields)
            {
                LocalizeDataControlField(field, resourceFile);
            }
        }
        public static void LocalizeGridView(ref GridView gridView, string resourceFile)
        {
            foreach (DataControlField column in gridView.Columns)
            {
                LocalizeDataControlField(column, resourceFile);
            }
        }
        public static string LocalizeRole(string role)
        {
            string localRole;
            switch (role)
            {
                case Globals.glbRoleAllUsersName:
                case Globals.glbRoleSuperUserName:
                case Globals.glbRoleUnauthUserName:
                    string roleKey = role.Replace(" ", "");
                    localRole = GetString(roleKey);
                    break;
                default:
                    localRole = role;
                    break;
            }
            return localRole;
        }
        public static void RemoveLanguageFromPortal(int portalID, int languageID)
        {
            DataProvider.Instance().DeletePortalLanguages(portalID, languageID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("portalID/languageID", portalID.ToString() + "/" + languageID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.LANGUAGETOPORTAL_DELETED);
            DataCache.ClearPortalCache(portalID, false);
        }
        public static void RemoveLanguageFromPortals(int languageID)
        {
            DataProvider.Instance().DeletePortalLanguages(Null.NullInteger, languageID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("languageID", languageID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.LANGUAGETOPORTAL_DELETED);
            DataCache.ClearHostCache(true);
        }
        public static void RemoveLanguagesFromPortal(int portalID)
        {
            DataProvider.Instance().DeletePortalLanguages(portalID, Null.NullInteger);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("portalID", portalID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.LANGUAGETOPORTAL_DELETED);
            DataCache.ClearPortalCache(portalID, false);
        }
        public static void SaveLanguage(Locale locale)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            if (locale.LanguageID == Null.NullInteger)
            {
                locale.LanguageID = DataProvider.Instance().AddLanguage(locale.Code, locale.Text, locale.Fallback, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(locale, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGE_CREATED);
            }
            else
            {
                DataProvider.Instance().UpdateLanguage(locale.LanguageID, locale.Code, locale.Text, locale.Fallback, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(locale, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGE_UPDATED);
            }
            DataCache.ClearHostCache(true);
        }
        public static void SetLanguage(string value)
        {
            try
            {
                HttpResponse Response = HttpContext.Current.Response;
                if (Response == null)
                {
                    return;
                }
                System.Web.HttpCookie cookie = null;
                cookie = Response.Cookies.Get("language");
                if ((cookie == null))
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        cookie = new System.Web.HttpCookie("language", value);
                        Response.Cookies.Add(cookie);
                    }
                }
                else
                {
                    cookie.Value = value;
                    if (!String.IsNullOrEmpty(value))
                    {
                        Response.Cookies.Set(cookie);
                    }
                    else
                    {
                        Response.Cookies.Remove("language");
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
