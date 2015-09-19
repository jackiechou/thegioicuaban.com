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
    public class TabPermissionInfo : PermissionInfoBase, IHydratable
    {
        int _TabPermissionID;
        int _TabID;
        public TabPermissionInfo()
            : base()
        {
            _TabPermissionID = Null.NullInteger;
            _TabID = Null.NullInteger;
        }
        public TabPermissionInfo(PermissionInfo permission)
            : this()
        {
            this.ModuleDefID = permission.ModuleDefID;
            this.PermissionCode = permission.PermissionCode;
            this.PermissionID = permission.PermissionID;
            this.PermissionKey = permission.PermissionKey;
            this.PermissionName = permission.PermissionName;
        }
        [XmlElement("tabpermissionid")]
        public int TabPermissionID
        {
            get { return _TabPermissionID; }
            set { _TabPermissionID = value; }
        }
        [XmlElement("tabid")]
        public int TabID
        {
            get { return _TabID; }
            set { _TabID = value; }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            TabPermissionID = Null.SetNullInteger(dr["TabPermissionID"]);
            TabID = Null.SetNullInteger(dr["TabID"]);
        }
        [XmlIgnore()]
        public int KeyID
        {
            get { return TabPermissionID; }
            set { TabPermissionID = value; }
        }
    }
}
