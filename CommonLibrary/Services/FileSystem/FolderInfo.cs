using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Portal;
using System.Web;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Common;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Services.FileSystem
{
    [XmlRoot("folder", IsNullable = false)]
    [Serializable()]
    public class FolderInfo : IHydratable
    {
        private int _folderID;
        private int _portalID;
        private string _folderPath;
        private int _storageLocation;
        private bool _isProtected;
        private bool _isCached = false;
        private System.DateTime _lastUpdated;
        private Security.Permissions.FolderPermissionCollection _FolderPermissions;
        [XmlIgnore()]
        public int FolderID
        {
            get { return _folderID; }
            set { _folderID = value; }
        }
        [XmlIgnore()]
        public string FolderName
        {
            get
            {
                string _folderName = FileSystemUtils.RemoveTrailingSlash(_folderPath);
                if (!String.IsNullOrEmpty(_folderName) && _folderName.LastIndexOf("/") > -1)
                {
                    _folderName = _folderName.Substring(_folderName.LastIndexOf("/") + 1);
                }
                return _folderName;
            }
        }
        [XmlElement("folderpath")]
        public string FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; }
        }
        [XmlIgnore()]
        public bool IsCached
        {
            get { return _isCached; }
            set { _isCached = value; }
        }
        [XmlIgnore()]
        public bool IsProtected
        {
            get { return _isProtected; }
            set { _isProtected = value; }
        }
        [XmlIgnore()]
        public System.DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set { _lastUpdated = value; }
        }
        [XmlIgnore()]
        public string PhysicalPath
        {
            get
            {
                string _PhysicalPath;
                PortalSettings PortalSettings = null;
                if (HttpContext.Current != null)
                {
                    PortalSettings = PortalController.GetCurrentPortalSettings();
                }
                if (PortalID == Null.NullInteger)
                {
                    _PhysicalPath = Globals.HostMapPath + FolderPath;
                }
                else
                {
                    if (PortalSettings == null || PortalSettings.PortalId != PortalID)
                    {
                        PortalController objPortals = new PortalController();
                        PortalInfo objPortal = objPortals.GetPortal(PortalID);
                        _PhysicalPath = objPortal.HomeDirectoryMapPath + FolderPath;
                    }
                    else
                    {
                        _PhysicalPath = PortalSettings.HomeDirectoryMapPath + FolderPath;
                    }
                }
                return _PhysicalPath.Replace("/", "\\");
            }
        }
        [XmlIgnore()]
        public int PortalID
        {
            get { return _portalID; }
            set { _portalID = value; }
        }
        [XmlElement("storagelocation")]
        public int StorageLocation
        {
            get { return _storageLocation; }
            set { _storageLocation = value; }
        }
        [XmlIgnore()]
        public FolderPermissionCollection FolderPermissions
        {
            get
            {
                if (_FolderPermissions == null)
                {
                    _FolderPermissions = new FolderPermissionCollection(FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalID, FolderPath));
                }
                return _FolderPermissions;
            }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            FolderID = Null.SetNullInteger(dr["FolderID"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            FolderPath = Null.SetNullString(dr["FolderPath"]);
            IsCached = Null.SetNullBoolean(dr["IsCached"]);
            IsProtected = Null.SetNullBoolean(dr["IsProtected"]);
            StorageLocation = Null.SetNullInteger(dr["StorageLocation"]);
            LastUpdated = Null.SetNullDateTime(dr["LastUpdated"]);
        }
        [XmlIgnore()]
        public int KeyID
        {
            get { return FolderID; }
            set { FolderID = value; }
        }
    }
}
