using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Lists;
using CommonLibrary.Framework;
using System.Xml.XPath;
using CommonLibrary.Services.Installer.Packages;
using CommonLibrary.Services.Localization;

namespace CommonLibrary.Services.Installer.Installers
{
    public class InstallerFactory
    {
        public static ComponentInstallerBase GetInstaller(string installerType)
        {
            ComponentInstallerBase installer = null;
            switch (installerType)
            {
                case "File":
                    installer = new FileInstaller();
                    break;
                case "Assembly":
                    installer = new AssemblyInstaller();
                    break;
                case "ResourceFile":
                    installer = new ResourceFileInstaller();
                    break;
                case "AuthenticationSystem":
                case "Auth_System":
                    installer = new AuthenticationInstaller();
                    break;
                case "DashboardControl":
                    installer = new DashboardInstaller();
                    break;
                case "Script":
                    installer = new ScriptInstaller();
                    break;
                case "Config":
                    installer = new ConfigInstaller();
                    break;
                case "Cleanup":
                    installer = new CleanupInstaller();
                    break;
                case "Skin":
                    installer = new SkinInstaller();
                    break;
                case "Container":
                    installer = new ContainerInstaller();
                    break;
                case "Module":
                    installer = new ModuleInstaller();
                    break;
                case "CoreLanguage":
                    installer = new LanguageInstaller(LanguagePackType.Core);
                    break;
                case "ExtensionLanguage":
                    installer = new LanguageInstaller(LanguagePackType.Extension);
                    break;
                case "Provider":
                    installer = new ProviderInstaller();
                    break;
                case "SkinObject":
                    installer = new SkinControlInstaller();
                    break;
                case "Widget":
                    installer = new WidgetInstaller();
                    break;
                default:
                    ListController listController = new ListController();
                    ListEntryInfo entry = listController.GetListEntryInfo("Installer", installerType);
                    if (entry != null && !string.IsNullOrEmpty(entry.Text))
                    {
                        installer = (ComponentInstallerBase)Reflection.CreateObject(entry.Text, "Installer_" + entry.Value);
                    }
                    break;
            }
            return installer;
        }
        public static ComponentInstallerBase GetInstaller(XPathNavigator manifestNav, PackageInfo package)
        {
            string installerType = Util.ReadAttribute(manifestNav, "type");
            string componentVersion = Util.ReadAttribute(manifestNav, "version");
            ComponentInstallerBase installer = GetInstaller(installerType);
            if (installer != null)
            {
                installer.Package = package;
                installer.Type = installerType;
                if (!string.IsNullOrEmpty(componentVersion))
                {
                    installer.Version = new Version(componentVersion);
                }
                else
                {
                    installer.Version = package.Version;
                }
                if (package.InstallerInfo.InstallMode != InstallMode.ManifestOnly || installer.SupportsManifestOnlyInstall)
                {
                    installer.ReadManifest(manifestNav);
                }
            }
            return installer;
        }
    }
}
