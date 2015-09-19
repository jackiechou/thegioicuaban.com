using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Common;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public abstract class PermissionInfoBase : PermissionInfo
    {
        private bool _AllowAccess;
        private string _DisplayName;
        private int _RoleID;
        private string _RoleName;
        private int _UserID;
        private string _Username;
        public PermissionInfoBase()
            : base()
        {
            _RoleID = int.Parse(Globals.glbRoleNothing);
            _AllowAccess = false;
            _RoleName = Null.NullString;
            _UserID = Null.NullInteger;
            _Username = Null.NullString;
            _DisplayName = Null.NullString;
        }
        [XmlElement("allowaccess")]
        public bool AllowAccess
        {
            get { return _AllowAccess; }
            set { _AllowAccess = value; }
        }
        [XmlElement("displayname")]
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }
        [XmlElement("roleid")]
        public int RoleID
        {
            get { return _RoleID; }
            set { _RoleID = value; }
        }
        [XmlElement("rolename")]
        public string RoleName
        {
            get { return _RoleName; }
            set { _RoleName = value; }
        }
        [XmlElement("userid")]
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        [XmlElement("username")]
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }
        protected override void FillInternal(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            UserID = Null.SetNullInteger(dr["UserID"]);
            Username = Null.SetNullString(dr["Username"]);
            DisplayName = Null.SetNullString(dr["DisplayName"]);
            if (UserID == Null.NullInteger)
            {
                RoleID = Null.SetNullInteger(dr["RoleID"]);
                RoleName = Null.SetNullString(dr["RoleName"]);
            }
            else
            {
                RoleID = int.Parse(Globals.glbRoleNothing);
                RoleName = "";
            }
            AllowAccess = Null.SetNullBoolean(dr["AllowAccess"]);
        }
    }
}
