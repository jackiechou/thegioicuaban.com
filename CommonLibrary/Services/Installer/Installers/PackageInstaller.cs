using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Packages;
using CommonLibrary.Services.Installer.Dependencies;
using System.Xml.XPath;
using CommonLibrary.Common.Utilities;
using System.IO;

namespace CommonLibrary.Services.Installer.Installers
{
    public class PackageInstaller : ComponentInstallerBase
    {
        private PackageInfo InstalledPackage;
        private SortedList<int, ComponentInstallerBase> ComponentInstallers = new SortedList<int, ComponentInstallerBase>();
        private bool _DeleteFiles = Null.NullBoolean;
        private bool _IsValid = true;
        public PackageInstaller(PackageInfo package)
        {
            this.Package = package;
            if (!string.IsNullOrEmpty(package.Manifest))
            {
                XPathDocument doc = new XPathDocument(new StringReader(package.Manifest));
                XPathNavigator nav = doc.CreateNavigator().SelectSingleNode("package");
                ReadComponents(nav);
            }
            else
            {
                ComponentInstallerBase installer = InstallerFactory.GetInstaller(package.PackageType);
                if (installer != null)
                {
                    installer.Package = package;
                    installer.Type = package.PackageType;
                    ComponentInstallers.Add(0, installer);
                }
            }
        }
        public PackageInstaller(string packageManifest, InstallerInfo info)
        {
            Package = new PackageInfo(info);
            Package.Manifest = packageManifest;
            if (!string.IsNullOrEmpty(packageManifest))
            {
                XPathDocument doc = new XPathDocument(new StringReader(packageManifest));
                XPathNavigator nav = doc.CreateNavigator().SelectSingleNode("package");
                ReadManifest(nav);
            }
        }
        public bool DeleteFiles
        {
            get { return _DeleteFiles; }
            set { _DeleteFiles = value; }
        }
        public bool IsValid
        {
            get { return _IsValid; }
        }
        private void CheckSecurity()
        {
            PackageType type = PackageController.GetPackageType(Package.PackageType);
            if (type == null)
            {
                Log.Logs.Clear();
                Log.AddFailure(Util.SECURITY_NotRegistered + " - " + Package.PackageType);
                _IsValid = false;
            }
            else
            {
                if (type.SecurityAccessLevel > Package.InstallerInfo.SecurityAccessLevel)
                {
                    Log.Logs.Clear();
                    Log.AddFailure(Util.SECURITY_Installer);
                    _IsValid = false;
                }
            }
        }
        private void ReadComponents(XPathNavigator manifestNav)
        {
            foreach (XPathNavigator componentNav in manifestNav.CreateNavigator().Select("components/component"))
            {
                int order = ComponentInstallers.Count;
                string type = componentNav.GetAttribute("type", "");
                if (InstallMode == InstallMode.Install)
                {
                    string installOrder = componentNav.GetAttribute("installOrder", "");
                    if (!string.IsNullOrEmpty(installOrder))
                    {
                        order = int.Parse(installOrder);
                    }
                }
                else
                {
                    string unInstallOrder = componentNav.GetAttribute("unInstallOrder", "");
                    if (!string.IsNullOrEmpty(unInstallOrder))
                    {
                        order = int.Parse(unInstallOrder);
                    }
                }
                if (Package.InstallerInfo != null)
                {
                    Log.AddInfo(Util.DNN_ReadingComponent + " - " + type);
                }
                ComponentInstallerBase installer = InstallerFactory.GetInstaller(componentNav, Package);
                if (installer == null)
                {
                    Log.AddFailure(Util.EXCEPTION_InstallerCreate);
                }
                else
                {
                    ComponentInstallers.Add(order, installer);
                    this.Package.InstallerInfo.AllowableFiles += ", " + installer.AllowableFiles;
                }
            }
        }
        private string ReadTextFromFile(string source)
        {
            string strText = Null.NullString;
            if (Package.InstallerInfo.InstallMode != Services.Installer.InstallMode.ManifestOnly)
            {
                strText = FileSystemUtils.ReadFile(Package.InstallerInfo.TempInstallFolder + "\\" + source);
            }
            return strText;
        }
        private void ValidateVersion(string strVersion)
        {
            if (string.IsNullOrEmpty(strVersion))
            {
                _IsValid = false;
                return;
            }
            Package.Version = new System.Version(strVersion);
            if (InstalledPackage != null)
            {
                Package.InstalledVersion = InstalledPackage.Version;
                if (Package.InstalledVersion > Package.Version)
                {
                    Log.AddFailure(Util.INSTALL_Version + " - " + Package.InstalledVersion.ToString(3));
                    _IsValid = false;
                }
                else if (Package.InstalledVersion == Package.Version)
                {
                    Package.InstallerInfo.Installed = true;
                    Package.InstallerInfo.PortalID = InstalledPackage.PortalID;
                }
            }
        }
        public override void Commit()
        {
            for (int index = 0; index <= ComponentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = ComponentInstallers.Values[index];
                if (compInstaller.Version >= Package.InstalledVersion && compInstaller.Completed)
                {
                    compInstaller.Commit();
                }
            }
            if (Log.Valid)
            {
                Log.AddInfo(Util.INSTALL_Committed);
            }
            else
            {
                Log.AddFailure(Util.INSTALL_Aborted);
            }
            Package.InstallerInfo.PackageID = Package.PackageID;
        }
        public override void Install()
        {
            bool isCompleted = true;
            try
            {
                if (InstalledPackage != null)
                {
                    Package.PackageID = InstalledPackage.PackageID;
                }
                PackageController.SavePackage(Package);
                for (int index = 0; index <= ComponentInstallers.Count - 1; index++)
                {
                    ComponentInstallerBase compInstaller = ComponentInstallers.Values[index];
                    if ((InstalledPackage == null) || (compInstaller.Version > Package.InstalledVersion) || (Package.InstallerInfo.RepairInstall))
                    {
                        Log.AddInfo(Util.INSTALL_Start + " - " + compInstaller.Type);
                        compInstaller.Install();
                        if (compInstaller.Completed)
                        {
                            Log.AddInfo(Util.COMPONENT_Installed + " - " + compInstaller.Type);
                        }
                        else
                        {
                            Log.AddFailure(Util.INSTALL_Failed + " - " + compInstaller.Type);
                            isCompleted = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.INSTALL_Aborted + " - " + Package.Name);
                ex.ToString();
            }
            if (isCompleted)
            {
                Commit();
            }
            else
            {
                Rollback();
            }
        }
        public override void ReadManifest(XPathNavigator manifestNav)
        {
            Package.Name = Util.ReadAttribute(manifestNav, "name", Log, Util.EXCEPTION_NameMissing);
            Package.PackageType = Util.ReadAttribute(manifestNav, "type", Log, Util.EXCEPTION_TypeMissing);
            if (Package.PackageType == "Skin" || Package.PackageType == "Container")
            {
                Package.PortalID = Package.InstallerInfo.PortalID;
            }
            CheckSecurity();
            if (!IsValid)
            {
                return;
            }
            InstalledPackage = PackageController.GetPackageByName(Package.PortalID, Package.Name);
            Package.IsSystemPackage = bool.Parse(Util.ReadAttribute(manifestNav, "isSystem", false, Log, "", bool.FalseString));
            string strVersion = Util.ReadAttribute(manifestNav, "version", Log, Util.EXCEPTION_VersionMissing);
            ValidateVersion(strVersion);
            if (!IsValid)
            {
                return;
            }
            Log.AddInfo(Util.DNN_ReadingPackage + " - " + Package.PackageType + " - " + Package.Name);
            Package.FriendlyName = Util.ReadElement(manifestNav, "friendlyName", Package.Name);
            Package.Description = Util.ReadElement(manifestNav, "description");
            XPathNavigator authorNav = manifestNav.SelectSingleNode("owner");
            if (authorNav != null)
            {
                Package.Owner = Util.ReadElement(authorNav, "name");
                Package.Organization = Util.ReadElement(authorNav, "organization");
                Package.Url = Util.ReadElement(authorNav, "url");
                Package.Email = Util.ReadElement(authorNav, "email");
            }
            XPathNavigator licenseNav = manifestNav.SelectSingleNode("license");
            if (licenseNav != null)
            {
                string licenseSrc = Util.ReadAttribute(licenseNav, "src");
                if (string.IsNullOrEmpty(licenseSrc))
                {
                    Package.License = licenseNav.Value;
                }
                else
                {
                    Package.License = ReadTextFromFile(licenseSrc);
                }
            }
            if (string.IsNullOrEmpty(Package.License))
            {
                Package.License = Util.PACKAGE_NoLicense;
            }
            XPathNavigator relNotesNav = manifestNav.SelectSingleNode("releaseNotes");
            if (relNotesNav != null)
            {
                string relNotesSrc = Util.ReadAttribute(relNotesNav, "src");
                if (string.IsNullOrEmpty(relNotesSrc))
                {
                    Package.ReleaseNotes = relNotesNav.Value;
                }
                else
                {
                    Package.ReleaseNotes = ReadTextFromFile(relNotesSrc);
                }
            }
            if (string.IsNullOrEmpty(Package.License))
            {
                Package.License = Util.PACKAGE_NoReleaseNotes;
            }
            IDependency dependency = null;
            foreach (XPathNavigator dependencyNav in manifestNav.CreateNavigator().Select("dependencies/dependency"))
            {
                dependency = DependencyFactory.GetDependency(dependencyNav);
                if (!dependency.IsValid)
                {
                    Log.AddFailure(dependency.ErrorMessage);
                    return;
                }
            }
            ReadComponents(manifestNav);
        }
        public override void Rollback()
        {
            for (int index = 0; index <= ComponentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = ComponentInstallers.Values[index];
                if (compInstaller.Version > Package.InstalledVersion && compInstaller.Completed)
                {
                    Log.AddInfo(Util.COMPONENT_RollingBack + " - " + compInstaller.Type);
                    compInstaller.Rollback();
                    Log.AddInfo(Util.COMPONENT_RolledBack + " - " + compInstaller.Type);
                }
            }
            if (InstalledPackage == null)
            {
                PackageController.DeletePackage(Package);
            }
            else
            {
                PackageController.SavePackage(InstalledPackage);
            }
        }
        public override void UnInstall()
        {
            for (int index = 0; index <= ComponentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = ComponentInstallers.Values[index];
                FileInstaller fileInstaller = compInstaller as FileInstaller;
                if (fileInstaller != null)
                {
                    fileInstaller.DeleteFiles = DeleteFiles;
                }
                Log.ResetFlags();
                Log.AddInfo(Util.UNINSTALL_StartComp + " - " + compInstaller.Type);
                compInstaller.UnInstall();
                Log.AddInfo(Util.COMPONENT_UnInstalled + " - " + compInstaller.Type);
                if (Log.Valid)
                {
                    Log.AddInfo(Util.UNINSTALL_SuccessComp + " - " + compInstaller.Type);
                }
                else
                {
                    Log.AddWarning(Util.UNINSTALL_WarningsComp + " - " + compInstaller.Type);
                }
            }
            PackageController.DeletePackage(Package);
        }
    }
}
