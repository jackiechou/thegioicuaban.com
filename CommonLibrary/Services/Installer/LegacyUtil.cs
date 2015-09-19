using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLibrary.UI.Skins;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Entities.Portal;
using System.Xml.XPath;
using CommonLibrary.Entities.Host;
using System.Xml;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Common;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Installer.Packages;
using CommonLibrary.Services.Localization;


namespace CommonLibrary.Services.Installer
{
    public class LegacyUtil
    {
        private static string AdminModules = "Adsense, MarketShare, Authentication, Banners, FeedExplorer, FileManager, HostSettings, Lists, LogViewer, Newsletters, PortalAliases, Portals, RecycleBin, Scheduler, SearchAdmin, SearchInput, SearchResults, Security, SiteLog, SiteWizard, SkinDesigner, Solutions, SQL, Tabs, Vendors,";
        private static string CoreModules = "DNN_Announcements, Blog, DNN_Documents, DNN_Events, DNN_FAQs, DNN_Feedback, DNN_Forum, Help, DNN_HTML, DNN_IFrame, DNN_Links, DNN_Media, DNN_NewsFeeds, DNN_Reports, Repository, Repository Dashboard, Store Admin, Store Account, Store Catalog, Store Mini Cart, Store Menu, DNN_Survey, DNN_UserDefinedTable, DNN_UsersOnline, Wiki, DNN_XML,";
        private static string KnownSkinObjects = "ACTIONBUTTON, ACTIONS, BANNER, BREADCRUMB, COPYRIGHT, CURRENTDATE, CommonLibrary, DROPDOWNACTIONS, HELP, HOSTNAME, ICON, LANGUAGE, LINKACTIONS, LINKS, LOGIN, LOGO, MENU, NAV, PRINTMODULE, PRIVACY, SEARCH, SIGNIN, SOLPARTACTIONS, SOLPARTMENU, STYLES, TERMS, TEXT, TITLE, TREEVIEW, USER, VISIBILITY,";
        private static string KnownSkins = "DNN-Blue, DNN-Gray, MinimalExtropy,";
        private static PackageInfo CreateSkinPackage(SkinPackageInfo skin)
        {
            PackageInfo package = new PackageInfo(new InstallerInfo());
            package.Name = skin.SkinName;
            package.FriendlyName = skin.SkinName;
            package.Description = Null.NullString;
            package.Version = new Version(1, 0, 0);
            package.PackageType = skin.SkinType;
            package.License = Util.PACKAGE_NoLicense;
            ParsePackageName(package);
            return package;
        }
        private static void CreateSkinManifest(XmlWriter writer, string skinFolder, string skinType, string tempInstallFolder, string subFolder)
        {
            string skinName = Path.GetFileNameWithoutExtension(skinFolder);
            SkinPackageInfo skin = new SkinPackageInfo();
            skin.SkinName = skinName;
            skin.SkinType = skinType;
            PackageInfo package = CreateSkinPackage(skin);
            SkinPackageWriter skinWriter = new SkinPackageWriter(skin, package, tempInstallFolder, subFolder);
            skinWriter.GetFiles(false);
            skinWriter.SetBasePath();
            skinWriter.WriteManifest(writer, true);
        }
        private static void ProcessLegacySkin(string skinFolder, string skinType)
        {
            string skinName = Path.GetFileName(skinFolder);
            if (skinName != "_default")
            {
                SkinPackageInfo skin = new SkinPackageInfo();
                skin.SkinName = skinName;
                skin.SkinType = skinType;
                PackageInfo package = CreateSkinPackage(skin);
                SkinPackageWriter skinWriter = new SkinPackageWriter(skin, package);
                skinWriter.GetFiles(false);
                package.Manifest = skinWriter.WriteManifest(true);
                CommonLibrary.Services.Installer.Packages.PackageController.SavePackage(package);
                skin.PackageID = package.PackageID;
                skin.SkinPackageID = SkinController.AddSkinPackage(skin);
                foreach (InstallFile skinFile in skinWriter.Files.Values)
                {
                    if (skinFile.Type == InstallFileType.Ascx)
                    {
                        if (skinType == "Skin")
                        {
                            SkinController.AddSkin(skin.SkinPackageID, Path.Combine("[G]" + SkinController.RootSkin, Path.Combine(skin.SkinName, skinFile.FullName)));
                        }
                        else
                        {
                            SkinController.AddSkin(skin.SkinPackageID, Path.Combine("[G]" + SkinController.RootContainer, Path.Combine(skin.SkinName, skinFile.FullName)));
                        }
                    }
                }
            }
        }
        private static void ParsePackageName(PackageInfo package, string separator)
        {
            int ownerIndex = package.Name.IndexOf(separator);
            if (ownerIndex > 0)
            {
                package.Owner = package.Name.Substring(0, ownerIndex);
            }
        }
        public static string CreateSkinManifest(string skinFolder, string skinType, string tempInstallFolder)
        {
            bool isCombi = false;
            DirectoryInfo installFolder = new DirectoryInfo(tempInstallFolder);
            DirectoryInfo[] subFolders = installFolder.GetDirectories();
            if (subFolders.Length > 0)
            {
                if ((subFolders[0].Name.ToLowerInvariant() == "containers" || subFolders[0].Name.ToLowerInvariant() == "skins"))
                {
                    isCombi = true;
                }
            }
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment));
            PackageWriterBase.WriteManifestStartElement(writer);
            if (isCombi)
            {
                if (Directory.Exists(Path.Combine(tempInstallFolder, "Skins")))
                {
                    CreateSkinManifest(writer, skinFolder, "Skin", tempInstallFolder.Replace(Globals.ApplicationMapPath + "\\", ""), "Skins");
                }
                if (Directory.Exists(Path.Combine(tempInstallFolder, "Containers")))
                {
                    CreateSkinManifest(writer, skinFolder, "Container", tempInstallFolder.Replace(Globals.ApplicationMapPath + "\\", ""), "Containers");
                }
            }
            else
            {
                CreateSkinManifest(writer, skinFolder, skinType, tempInstallFolder.Replace(Globals.ApplicationMapPath + "\\", ""), "");
            }
            PackageWriterBase.WriteManifestEndElement(writer);
            writer.Close();
            return sb.ToString();
        }
        public static void ParsePackageName(PackageInfo package)
        {
            ParsePackageName(package, ".");
            if (string.IsNullOrEmpty(package.Owner))
            {
                ParsePackageName(package, "\\");
            }
            if (string.IsNullOrEmpty(package.Owner))
            {
                ParsePackageName(package, "_");
            }
            if (package.PackageType == "Module" && AdminModules.Contains(package.Name + ",") || package.PackageType == "Module" && CoreModules.Contains(package.Name + ",") || (package.PackageType == "Container" || package.PackageType == "Skin") && KnownSkins.Contains(package.Name + ",") || package.PackageType == "SkinObject" && KnownSkinObjects.Contains(package.Name + ","))
            {
                if (string.IsNullOrEmpty(package.Owner))
                {
                    package.Owner = "CommonLibrary";
                    package.Name = "CommonLibrary." + package.Name;
                    switch (package.PackageType)
                    {
                        case "Skin":
                            package.Name += ".Skin";
                            package.FriendlyName += " Skin";
                            break;
                        case "Container":
                            package.Name += ".Container";
                            package.FriendlyName += " Container";
                            break;
                        case "SkinObject":
                            package.Name += "SkinObject";
                            package.FriendlyName += " SkinObject";
                            break;
                    }
                }
            }
            if (package.Owner == "CommonLibrary")
            {
                package.License = Localization.Localization.GetString("License", Localization.Localization.GlobalResourceFile);
                package.Organization = "CommonLibrary Corporation";
                package.Url = "www.CommonLibrary.com";
                package.Email = "support@CommonLibrary.com";
                package.ReleaseNotes = "There are no release notes for this version.";
            }
            else
            {
                package.License = Util.PACKAGE_NoLicense;
            }
        }
        public static void ProcessLegacyLanguages()
        {
            string filePath = Common.Globals.ApplicationMapPath + Localization.Localization.SupportedLocalesFile.Substring(1).Replace("/", "\\");
            if (File.Exists(filePath))
            {
                XPathDocument doc = new XPathDocument(filePath);
                HostSettingsController controller = new HostSettingsController();
                XPathNavigator browserNav = doc.CreateNavigator().SelectSingleNode("root/browserDetection");
                if (browserNav != null)
                {
                    controller.UpdateHostSetting("EnableBrowserLanguage", Util.ReadAttribute(browserNav, "enabled", false, null, Null.NullString, "true"));
                }
                XPathNavigator urlNav = doc.CreateNavigator().SelectSingleNode("root/languageInUrl");
                if (urlNav != null)
                {
                    controller.UpdateHostSetting("EnableUrlLanguage", Util.ReadAttribute(urlNav, "enabled", false, null, Null.NullString, "true"));
                }
                foreach (XPathNavigator nav in doc.CreateNavigator().Select("root/language"))
                {
                    if (nav.NodeType != XPathNodeType.Comment)
                    {
                        Locale language = new Locale();
                        language.Text = Util.ReadAttribute(nav, "name");
                        language.Code = Util.ReadAttribute(nav, "key");
                        language.Fallback = Util.ReadAttribute(nav, "fallback");
                        if (language.Code != Localization.Localization.SystemLocale)
                        {
                            PackageInfo package = new PackageInfo(new InstallerInfo());
                            package.Name = language.Text;
                            package.FriendlyName = language.Text;
                            package.Description = Null.NullString;
                            package.Version = new Version(1, 0, 0);
                            package.PackageType = "CoreLanguagePack";
                            package.License = Util.PACKAGE_NoLicense;
                            LanguagePackWriter packageWriter = new LanguagePackWriter(language, package);
                            package.Manifest = packageWriter.WriteManifest(true);
                            PackageController.SavePackage(package);
                            Localization.Localization.SaveLanguage(language);
                            LanguagePackInfo languagePack = new LanguagePackInfo();
                            languagePack.LanguageID = language.LanguageID;
                            languagePack.PackageID = package.PackageID;
                            languagePack.DependentPackageID = -2;
                            LanguagePackController.SaveLanguagePack(languagePack);
                        }
                    }
                }
            }
            foreach (PortalInfo portal in new PortalController().GetPortals())
            {
                int portalID = portal.PortalID;
                filePath = string.Format(Common.Globals.ApplicationMapPath + Localization.Localization.ApplicationResourceDirectory.Substring(1).Replace("/", "\\") + "\\Locales.Portal-{0}.xml", portalID.ToString());
                if (File.Exists(filePath))
                {
                    XPathDocument doc = new XPathDocument(filePath);
                    XPathNavigator browserNav = doc.CreateNavigator().SelectSingleNode("locales/browserDetection");
                    if (browserNav != null)
                    {
                        PortalController.UpdatePortalSetting(portalID, "EnableBrowserLanguage", Util.ReadAttribute(browserNav, "enabled", false, null, Null.NullString, "true"));
                    }
                    XPathNavigator urlNav = doc.CreateNavigator().SelectSingleNode("locales/languageInUrl");
                    if (urlNav != null)
                    {
                        PortalController.UpdatePortalSetting(portalID, "EnableUrlLanguage", Util.ReadAttribute(urlNav, "enabled", false, null, Null.NullString, "true"));
                    }
                    foreach (Locale installedLanguage in Localization.Localization.GetLocales(Null.NullInteger).Values)
                    {
                        string code = installedLanguage.Code;
                        bool bFound = false;
                        foreach (XPathNavigator inactiveNav in doc.CreateNavigator().Select("locales/inactive/locale"))
                        {
                            if (inactiveNav.Value == code)
                            {
                                bFound = true;
                                break;
                            }
                        }
                        if (!bFound)
                        {
                            Localization.Localization.AddLanguageToPortal(portalID, installedLanguage.LanguageID, false);
                        }
                    }
                }
                else
                {
                    foreach (Locale installedLanguage in Localization.Localization.GetLocales(Null.NullInteger).Values)
                    {
                        Localization.Localization.AddLanguageToPortal(portalID, installedLanguage.LanguageID, false);
                    }
                }
            }
        }
        public static void ProcessLegacyModules()
        {
            foreach (DesktopModuleInfo desktopModule in DesktopModuleController.GetDesktopModules(Null.NullInteger).Values)
            {
                if (desktopModule.PackageID == Null.NullInteger)
                {
                    string moduleFolder = Path.Combine(CommonLibrary.Common.Globals.ApplicationMapPath, Path.Combine("DesktopModules", desktopModule.FolderName));
                    XPathNavigator rootNav = null;
                    try
                    {
                        string hostModules = "Portals, SQL, HostSettings, Scheduler, SearchAdmin, Lists, SkinDesigner, Extensions";
                        string[] files = Directory.GetFiles(moduleFolder, "*.dnn.config");
                        if (files.Length > 0)
                        {
                            XPathDocument doc = new XPathDocument(new FileStream(files[0], FileMode.Open, FileAccess.Read));
                            rootNav = doc.CreateNavigator().SelectSingleNode("CommonLibrary");
                        }
                        PackageInfo package = new PackageInfo(new InstallerInfo());
                        package.Name = desktopModule.ModuleName;
                        package.FriendlyName = desktopModule.FriendlyName;
                        package.Description = desktopModule.Description;
                        package.Version = new Version(1, 0, 0);
                        if (!string.IsNullOrEmpty(desktopModule.Version))
                        {
                            package.Version = new Version(desktopModule.Version);
                        }
                        if (hostModules.Contains(desktopModule.ModuleName))
                        {
                            package.IsSystemPackage = true;
                            desktopModule.IsAdmin = true;
                        }
                        else
                        {
                            desktopModule.IsAdmin = false;
                        }
                        package.PackageType = "Module";
                        ParsePackageName(package);
                        if (files.Length > 0)
                        {
                            ModulePackageWriter modulewriter = new ModulePackageWriter(desktopModule, rootNav, package);
                            package.Manifest = modulewriter.WriteManifest(true);
                        }
                        else
                        {
                            package.Manifest = "";
                        }
                        PackageController.SavePackage(package);
                        desktopModule.PackageID = package.PackageID;
                        DesktopModuleController.SaveDesktopModule(desktopModule, false, false);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
        }
        public static void ProcessLegacySkinControls()
        {
            foreach (SkinControlInfo skinControl in SkinControlController.GetSkinControls().Values)
            {
                if (skinControl.PackageID == Null.NullInteger)
                {
                    try
                    {
                        PackageInfo package = new PackageInfo(new InstallerInfo());
                        package.Name = skinControl.ControlKey;
                        package.FriendlyName = skinControl.ControlKey;
                        package.Description = Null.NullString;
                        package.Version = new Version(1, 0, 0);
                        package.PackageType = "SkinObject";
                        ParsePackageName(package);
                        SkinControlPackageWriter skinControlWriter = new SkinControlPackageWriter(skinControl, package);
                        package.Manifest = skinControlWriter.WriteManifest(true);
                        PackageController.SavePackage(package);
                        skinControl.PackageID = package.PackageID;
                        SkinControlController.SaveSkinControl(skinControl);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
        }
        public static void ProcessLegacySkins()
        {
            string skinRootPath = Path.Combine(Common.Globals.HostMapPath, SkinController.RootSkin);
            foreach (string skinFolder in Directory.GetDirectories(skinRootPath))
            {
                ProcessLegacySkin(skinFolder, "Skin");
            }
            skinRootPath = Path.Combine(Common.Globals.HostMapPath, SkinController.RootContainer);
            foreach (string skinFolder in Directory.GetDirectories(skinRootPath))
            {
                ProcessLegacySkin(skinFolder, "Container");
            }
        }
    }
}
