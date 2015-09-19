using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class DesktopModulePermissionInfo : PermissionInfoBase, IHydratable
    {
        int _DesktopModulePermissionID;
        int _PortalDesktopModuleID;
        public DesktopModulePermissionInfo()
            : base()
        {
            _DesktopModulePermissionID = Null.NullInteger;
            _PortalDesktopModuleID = Null.NullInteger;
        }
        public DesktopModulePermissionInfo(PermissionInfo permission)
            : this()
        {
            this.ModuleDefID = permission.ModuleDefID;
            this.PermissionCode = permission.PermissionCode;
            this.PermissionID = permission.PermissionID;
            this.PermissionKey = permission.PermissionKey;
            this.PermissionName = permission.PermissionName;
        }
        public int DesktopModulePermissionID
        {
            get { return _DesktopModulePermissionID; }
            set { _DesktopModulePermissionID = value; }
        }
        public int PortalDesktopModuleID
        {
            get { return _PortalDesktopModuleID; }
            set { _PortalDesktopModuleID = value; }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !object.ReferenceEquals(this.GetType(), obj.GetType()))
            {
                return false;
            }
            DesktopModulePermissionInfo perm = (DesktopModulePermissionInfo)obj;
            return (this.AllowAccess == perm.AllowAccess) && (this.PortalDesktopModuleID == perm.PortalDesktopModuleID) && (this.RoleID == perm.RoleID) && (this.PermissionID == perm.PermissionID);
        }
        public void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            DesktopModulePermissionID = Null.SetNullInteger(dr["DesktopModulePermissionID"]);
            PortalDesktopModuleID = Null.SetNullInteger(dr["PortalDesktopModuleID"]);
        }
        public int KeyID
        {
            get { return DesktopModulePermissionID; }
            set { DesktopModulePermissionID = value; }
        }
    }
}
