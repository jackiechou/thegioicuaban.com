using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.Xml.Serialization;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class ModulePermissionInfo : PermissionInfoBase, IHydratable
    {
        int _ModulePermissionID;
        int _ModuleID;
        public ModulePermissionInfo()
            : base()
        {
            _ModulePermissionID = Null.NullInteger;
            _ModuleID = Null.NullInteger;
        }
        public ModulePermissionInfo(PermissionInfo permission)
            : this()
        {
            this.ModuleDefID = permission.ModuleDefID;
            this.PermissionCode = permission.PermissionCode;
            this.PermissionID = permission.PermissionID;
            this.PermissionKey = permission.PermissionKey;
            this.PermissionName = permission.PermissionName;
        }
        [XmlElement("modulepermissionid")]
        public int ModulePermissionID
        {
            get { return _ModulePermissionID; }
            set { _ModulePermissionID = value; }
        }
        [XmlElement("moduleid")]
        public int ModuleID
        {
            get { return _ModuleID; }
            set { _ModuleID = value; }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !object.ReferenceEquals(this.GetType(), obj.GetType()))
            {
                return false;
            }
            ModulePermissionInfo perm = (ModulePermissionInfo)obj;
            return (this.AllowAccess == perm.AllowAccess) && (this.ModuleID == perm.ModuleID) && (this.RoleID == perm.RoleID) && (this.PermissionID == perm.PermissionID);
        }
        public void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            ModulePermissionID = Null.SetNullInteger(dr["ModulePermissionID"]);
            ModuleID = Null.SetNullInteger(dr["ModuleID"]);
        }
        [XmlIgnore()]
        public int KeyID
        {
            get { return ModulePermissionID; }
            set { ModulePermissionID = value; }
        }
    }
}
