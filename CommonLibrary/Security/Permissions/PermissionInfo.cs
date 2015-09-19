using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class PermissionInfo : Entities.BaseEntityInfo
    {
        private int _ModuleDefID;
        private string _PermissionCode;
        private int _PermissionID;
        private string _PermissionKey;
        private string _PermissionName;
        [XmlIgnore()]
        public int ModuleDefID
        {
            get { return _ModuleDefID; }
            set { _ModuleDefID = value; }
        }
        [XmlElement("permissioncode")]
        public string PermissionCode
        {
            get { return _PermissionCode; }
            set { _PermissionCode = value; }
        }
        [XmlElement("permissionid")]
        public int PermissionID
        {
            get { return _PermissionID; }
            set { _PermissionID = value; }
        }
        [XmlElement("permissionkey")]
        public string PermissionKey
        {
            get { return _PermissionKey; }
            set { _PermissionKey = value; }
        }
        [XmlIgnore()]
        public string PermissionName
        {
            get { return _PermissionName; }
            set { _PermissionName = value; }
        }
        protected override void FillInternal(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            PermissionID = Null.SetNullInteger(dr["PermissionID"]);
            ModuleDefID = Null.SetNullInteger(dr["ModuleDefID"]);
            PermissionCode = Null.SetNullString(dr["PermissionCode"]);
            PermissionKey = Null.SetNullString(dr["PermissionKey"]);
            PermissionName = Null.SetNullString(dr["PermissionName"]);
        }
    }
}
