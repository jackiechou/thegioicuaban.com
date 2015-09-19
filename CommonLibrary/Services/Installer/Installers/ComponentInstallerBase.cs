using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Security;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Installer.Packages;
using CommonLibrary.Services.Installer.Log;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Installers
{
    public abstract class ComponentInstallerBase
    {
        private bool _Completed = Null.NullBoolean;
        private PackageInfo _PackageInfo;
        private SecurityAccessLevel _SecurityAccessLevel = SecurityAccessLevel.Host;
        private string _Type;
        private System.Version _Version;
        public virtual string AllowableFiles
        {
            get { return Null.NullString; }
        }
        public bool Completed
        {
            get { return _Completed; }
            set { _Completed = value; }
        }
        public InstallMode InstallMode
        {
            get { return Package.InstallMode; }
        }
        public Logger Log
        {
            get { return Package.Log; }
        }
        public PackageInfo Package
        {
            get { return _PackageInfo; }
            set { _PackageInfo = value; }
        }
        public Dictionary<string, InstallFile> PackageFiles
        {
            get { return Package.Files; }
        }
        public string PhysicalSitePath
        {
            get { return Package.InstallerInfo.PhysicalSitePath; }
        }
        public virtual bool SupportsManifestOnlyInstall
        {
            get { return true; }
        }
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        public System.Version Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        public abstract void Commit();
        public abstract void Install();
        public abstract void ReadManifest(XPathNavigator manifestNav);
        public abstract void Rollback();
        public abstract void UnInstall();
    }
}
