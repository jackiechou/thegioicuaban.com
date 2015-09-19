using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Entities;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Installer.Log;

namespace CommonLibrary.Services.Installer.Packages
{
    [Serializable()]
    public class PackageInfo : BaseEntityInfo
    {
        private int _PackageID = Null.NullInteger;
        private int _PortalID = Null.NullInteger;
        private string _Description;
        private string _FriendlyName;
        private bool _IsValid = true;
        private System.Version _InstalledVersion = new Version(0, 0, 0);
        private InstallerInfo _InstallerInfo;
        private bool _IsSystemPackage;
        private string _License;
        private string _Manifest;
        private string _Name;
        private string _PackageType;
        private string _ReleaseNotes;
        private System.Version _Version = new Version(0, 0, 0);
        private string _Owner;
        private string _Organization;
        private string _Url;
        private string _Email;
        private PackageInfo InstalledPackage;
        public PackageInfo(InstallerInfo info)
        {
            AttachInstallerInfo(info);
        }
        public PackageInfo()
        {
        }
        public int PackageID
        {
            get { return _PackageID; }
            set { _PackageID = value; }
        }
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        public string Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }
        public string Organization
        {
            get { return _Organization; }
            set { _Organization = value; }
        }
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        [XmlIgnore()]
        public Dictionary<string, InstallFile> Files
        {
            get { return InstallerInfo.Files; }
        }
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set { _FriendlyName = value; }
        }
        [XmlIgnore()]
        public InstallerInfo InstallerInfo
        {
            get { return _InstallerInfo; }
        }
        public System.Version InstalledVersion
        {
            get { return _InstalledVersion; }
            set { _InstalledVersion = value; }
        }
        public InstallMode InstallMode
        {
            get { return InstallerInfo.InstallMode; }
        }
        public bool IsSystemPackage
        {
            get { return _IsSystemPackage; }
            set { _IsSystemPackage = value; }
        }
        public bool IsValid
        {
            get { return _IsValid; }
        }
        public string License
        {
            get { return _License; }
            set { _License = value; }
        }
        [XmlIgnore()]
        public Logger Log
        {
            get { return InstallerInfo.Log; }
        }
        public string Manifest
        {
            get { return _Manifest; }
            set { _Manifest = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string ReleaseNotes
        {
            get { return _ReleaseNotes; }
            set { _ReleaseNotes = value; }
        }
        public string PackageType
        {
            get { return _PackageType; }
            set { _PackageType = value; }
        }
        public System.Version Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        public void AttachInstallerInfo(InstallerInfo installer)
        {
            _InstallerInfo = installer;
        }
    }
}
