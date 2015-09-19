using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class FolderPermissionInfo : PermissionInfoBase, IHydratable
    {
        private int _folderPermissionID;
        private string _folderPath;
        private int _portalID;
        private int _folderID;
        public FolderPermissionInfo()
        {
            _folderPermissionID = Null.NullInteger;
            _folderPath = Null.NullString;
            _portalID = Null.NullInteger;
            _folderID = Null.NullInteger;
        }
        public FolderPermissionInfo(PermissionInfo permission)
            : this()
        {
            this.ModuleDefID = permission.ModuleDefID;
            this.PermissionCode = permission.PermissionCode;
            this.PermissionID = permission.PermissionID;
            this.PermissionKey = permission.PermissionKey;
            this.PermissionName = permission.PermissionName;
        }
        [XmlIgnore()]
        public int FolderPermissionID
        {
            get { return _folderPermissionID; }
            set { _folderPermissionID = value; }
        }
        [XmlIgnore()]
        public int FolderID
        {
            get { return _folderID; }
            set { _folderID = value; }
        }
        [XmlIgnore()]
        public int PortalID
        {
            get { return _portalID; }
            set { _portalID = value; }
        }
        [XmlElement("folderpath")]
        public string FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            FolderPermissionID = Null.SetNullInteger(dr["FolderPermissionID"]);
            FolderID = Null.SetNullInteger(dr["FolderID"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            FolderPath = Null.SetNullString(dr["FolderPath"]);
        }
        [XmlIgnore()]
        public int KeyID
        {
            get { return FolderPermissionID; }
            set { FolderPermissionID = value; }
        }
    }
}
