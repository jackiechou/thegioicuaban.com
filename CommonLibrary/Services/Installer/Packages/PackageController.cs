using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Packages;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Services.Localization;
using System.Xml.XPath;
using CommonLibrary.UI.Skins;
using System.Xml;
using System.IO;
using CommonLibrary.Data;
using CommonLibrary.Services.Authentication;

namespace CommonLibrary.Services.Installer.Packages
{
    public class PackageController
    {
        private static DataProvider provider = DataProvider.Instance();
        public static int AddPackage(PackageInfo package, bool includeDetail)
        {
            int packageID = provider.AddPackage(package.PortalID, package.Name, package.FriendlyName, package.Description, package.PackageType, package.Version.ToString(3), package.License, package.Manifest, package.Owner, package.Organization,
            package.Url, package.Email, package.ReleaseNotes, package.IsSystemPackage, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(package, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.PACKAGE_CREATED);
            if (includeDetail)
            {
                Locale locale;
                LanguagePackInfo languagePack;
                switch (package.PackageType)
                {
                    case "Auth_System":
                        AuthenticationInfo authSystem = new AuthenticationInfo();
                        authSystem.AuthenticationType = package.Name;
                        authSystem.IsEnabled = Null.NullBoolean;
                        authSystem.PackageID = packageID;
                        AuthenticationController.AddAuthentication(authSystem);
                        break;
                    case "Container":
                    case "Skin":
                        SkinPackageInfo skinPackage = new SkinPackageInfo();
                        skinPackage.SkinName = package.Name;
                        skinPackage.PackageID = packageID;
                        skinPackage.SkinType = package.PackageType;
                        SkinController.AddSkinPackage(skinPackage);
                        break;
                    case "CoreLanguagePack":
                        locale = Localization.Localization.GetLocale(PortalController.GetCurrentPortalSettings().DefaultLanguage);
                        languagePack = new LanguagePackInfo();
                        languagePack.PackageID = packageID;
                        languagePack.LanguageID = locale.LanguageID;
                        languagePack.DependentPackageID = -2;
                        LanguagePackController.SaveLanguagePack(languagePack);
                        break;
                    case "ExtensionLanguagePack":
                        locale = Localization.Localization.GetLocale(PortalController.GetCurrentPortalSettings().DefaultLanguage);
                        languagePack = new LanguagePackInfo();
                        languagePack.PackageID = packageID;
                        languagePack.LanguageID = locale.LanguageID;
                        languagePack.DependentPackageID = Null.NullInteger;
                        LanguagePackController.SaveLanguagePack(languagePack);
                        break;
                    case "Module":
                        DesktopModuleInfo desktopModule = new DesktopModuleInfo();
                        desktopModule.PackageID = packageID;
                        desktopModule.ModuleName = package.Name;
                        desktopModule.FriendlyName = package.FriendlyName;
                        desktopModule.FolderName = package.Name;
                        desktopModule.Description = package.Description;
                        desktopModule.Version = package.Version.ToString(3);
                        desktopModule.SupportedFeatures = 0;
                        int desktopModuleId = DesktopModuleController.SaveDesktopModule(desktopModule, false, true);
                        if (desktopModuleId > Null.NullInteger)
                        {
                            DesktopModuleController.AddDesktopModuleToPortals(desktopModuleId);
                        }
                        break;
                    case "SkinObject":
                        SkinControlInfo skinControl = new SkinControlInfo();
                        skinControl.PackageID = packageID;
                        skinControl.ControlKey = package.Name;
                        SkinControlController.SaveSkinControl(skinControl);
                        break;
                }
            }
            return packageID;
        }
        public static bool CanDeletePackage(PackageInfo package, PortalSettings portalSettings)
        {
            bool bCanDelete = true;
            switch (package.PackageType)
            {
                case "Skin":
                case "Container":
                    string strFolderPath = string.Empty;
                    string strRootSkin = package.PackageType == "Skin" ? SkinController.RootSkin : SkinController.RootContainer;
                    SkinPackageInfo _SkinPackageInfo = SkinController.GetSkinByPackageID(package.PackageID);
                    foreach (System.Collections.Generic.KeyValuePair<int, string> kvp in _SkinPackageInfo.Skins)
                    {
                        if (kvp.Value.Contains(Common.Globals.HostMapPath))
                        {
                            strFolderPath = Path.Combine(Path.Combine(Common.Globals.HostMapPath, strRootSkin), _SkinPackageInfo.SkinName);
                        }
                        else
                        {
                            strFolderPath = Path.Combine(Path.Combine(portalSettings.HomeDirectoryMapPath, strRootSkin), _SkinPackageInfo.SkinName);
                        }
                        break;
                    }

                    bCanDelete = SkinController.CanDeleteSkin(strFolderPath, portalSettings.HomeDirectoryMapPath);
                    break;
                case "Provider":
                    XmlDocument configDoc = Config.Load();
                    string providerName = package.Name;
                    if (providerName.IndexOf(".") > Null.NullInteger)
                    {
                        providerName = providerName.Substring(providerName.IndexOf(".") + 1);
                    }
                    switch (providerName)
                    {
                        case "SchedulingProvider":
                            providerName = "DNNScheduler";
                            break;
                        case "SearchIndexProvider":
                            providerName = "ModuleIndexProvider";
                            break;
                        case "SearchProvider":
                            providerName = "SearchDataStoreProvider";
                            break;
                    }
                    XPathNavigator providerNavigator = configDoc.CreateNavigator().SelectSingleNode("/configuration/dotnetnuke/*[@defaultProvider='" + providerName + "']");
                    bCanDelete = (providerNavigator == null);
                    break;
            }
            return bCanDelete;
        }
        public static void DeletePackage(PackageInfo package)
        {
            switch (package.PackageType)
            {
                case "Auth_System":
                    Authentication.AuthenticationInfo authSystem = Authentication.AuthenticationController.GetAuthenticationServiceByPackageID(package.PackageID);
                    if (authSystem != null)
                    {
                        Authentication.AuthenticationController.DeleteAuthentication(authSystem);
                    }
                    break;
                case "CoreLanguagePack":
                    LanguagePackInfo languagePack = Localization.LanguagePackController.GetLanguagePackByPackage(package.PackageID);
                    if (languagePack != null)
                    {
                        LanguagePackController.DeleteLanguagePack(languagePack);
                    }
                    break;
                case "Module":
                    DesktopModuleController controller = new DesktopModuleController();
                    DesktopModuleInfo desktopModule = DesktopModuleController.GetDesktopModuleByPackageID(package.PackageID);
                    if (desktopModule != null)
                    {
                        controller.DeleteDesktopModule(desktopModule);
                    }
                    break;
                case "SkinObject":
                    SkinControlInfo skinControl = SkinControlController.GetSkinControlByPackageID(package.PackageID);
                    if (skinControl != null)
                    {
                        SkinControlController.DeleteSkinControl(skinControl);
                    }
                    break;
            }
            DeletePackage(package.PackageID);
        }
        public static void DeletePackage(int packageID)
        {
            provider.DeletePackage(packageID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("packageID", packageID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Services.Log.EventLog.EventLogController.EventLogType.PACKAGE_DELETED);
        }
        public static PackageInfo GetPackage(int packageID)
        {
            return CBO.FillObject<PackageInfo>(provider.GetPackage(packageID));
        }
        public static PackageInfo GetPackageByName(string name)
        {
            return GetPackageByName(Null.NullInteger, name);
        }
        public static PackageInfo GetPackageByName(int portalId, string name)
        {
            return CBO.FillObject<PackageInfo>(provider.GetPackageByName(portalId, name));
        }
        public static List<PackageInfo> GetPackages()
        {
            return GetPackages(Null.NullInteger);
        }
        public static List<PackageInfo> GetPackages(int portalID)
        {
            return CBO.FillCollection<PackageInfo>(provider.GetPackages(portalID));
        }
        public static List<PackageInfo> GetPackagesByType(string type)
        {
            return GetPackagesByType(Null.NullInteger, type);
        }
        public static IDictionary<int, PackageInfo> GetModulePackagesInUse(int portalID, bool forHost)
        {
            return CBO.FillDictionary<int, PackageInfo>("PackageID", provider.GetModulePackagesInUse(portalID, forHost));
        }
        public static List<PackageInfo> GetPackagesByType(int portalID, string type)
        {
            return CBO.FillCollection<PackageInfo>(provider.GetPackagesByType(portalID, type));
        }
        public static PackageType GetPackageType(string type)
        {
            return CBO.FillObject<PackageType>(provider.GetPackageType(type));
        }
        public static List<PackageType> GetPackageTypes()
        {
            return CBO.FillCollection<PackageType>(provider.GetPackageTypes());
        }
        public static void SavePackage(PackageInfo package)
        {
            if (package.PackageID == Null.NullInteger)
            {
                package.PackageID = AddPackage(package, false);
            }
            else
            {
                UpdatePackage(package);
            }
        }
        public static void UpdatePackage(PackageInfo package)
        {
            provider.UpdatePackage(package.PortalID, package.Name, package.FriendlyName, package.Description, package.PackageType, package.Version.ToString(3), package.License, package.Manifest, package.Owner, package.Organization,
            package.Url, package.Email, package.ReleaseNotes, package.IsSystemPackage, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(package, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.PACKAGE_UPDATED);
        }
    }
}
