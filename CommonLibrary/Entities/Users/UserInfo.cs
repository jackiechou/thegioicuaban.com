using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.ComponentModel;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Tokens.PropertyAccess;
using CommonLibrary.Services.Tokens;
using CommonLibrary.Common;
using CommonLibrary.Entities.Profile;
using CommonLibrary.Security.Roles;
using CommonLibrary.UI.WebControls;
using CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes;

namespace CommonLibrary.Entities.Users
{
    [Serializable()]
    public class UserInfo : BaseEntityInfo//, IPropertyAccess
    {
        private int _UserID;
        private string _Username;
        private string _DisplayName;
        private string _FullName;
        private string _Email;
        private int _PortalID;
        private bool _IsSuperUser;
        private int _AffiliateID;
        private UserMembership _Membership;
        private UserProfile _Profile;
        private string[] _Roles;
        private bool _RolesHydrated = Null.NullBoolean;
        private string _LastIPAddress;
        private bool _RefreshRoles = Null.NullBoolean;
        private bool _IsDeleted = Null.NullBoolean;
        public UserInfo()
        {
            _UserID = Null.NullInteger;
            _PortalID = Null.NullInteger;
            _IsSuperUser = Null.NullBoolean;
            _AffiliateID = Null.NullInteger;
            _Roles = new string[] {				
			};
        }
        [Browsable(false)]
        public int AffiliateID
        {
            get { return _AffiliateID; }
            set { _AffiliateID = value; }
        }
        [SortOrder(3), Required(true), MaxLength(128)]
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }
        [SortOrder(4), MaxLength(256), Required(true), RegularExpressionValidator(Globals.glbEmailRegEx)]
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        [SortOrder(1), MaxLength(50), Required(true)]
        public string FirstName
        {
            get { return Profile.FirstName; }
            set { Profile.FirstName = value; }
        }
        [Browsable(false)]
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set { _IsDeleted = value; }
        }
        [Browsable(false)]
        public bool IsSuperUser
        {
            get { return _IsSuperUser; }
            set { _IsSuperUser = value; }
        }
        [Browsable(false)]
        public string LastIPAddress
        {
            get { return _LastIPAddress; }
            set { _LastIPAddress = value; }
        }
        [SortOrder(2), MaxLength(50), Required(true)]
        public string LastName
        {
            get { return Profile.LastName; }
            set { Profile.LastName = value; }
        }
        [Browsable(false)]
        public Entities.Users.UserMembership Membership
        {
            get
            {
                if (_Membership == null)
                {
                    _Membership = new UserMembership(this);
                    if ((this.Username != null) && (!String.IsNullOrEmpty(this.Username)))
                    {
                        UserController.GetUserMembership(this);
                    }
                }
                return _Membership;
            }
            set { _Membership = value; }
        }
        [Browsable(false)]
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        [Browsable(false)]
        public UserProfile Profile
        {
            get
            {
                if (_Profile == null)
                {
                    _Profile = new UserProfile();
                    ProfileController.GetUserProfile(this);
                }
                return _Profile;
            }
            set { _Profile = value; }
        }
        [Browsable(false)]
        public bool RefreshRoles
        {
            get { return _RefreshRoles; }
            set { _RefreshRoles = value; }
        }
        [Browsable(false)]
        public string[] Roles
        {
            get
            {
                if (!_RolesHydrated)
                {
                    if (_UserID > Null.NullInteger)
                    {
                        RoleController controller = new RoleController();
                        _Roles = controller.GetRolesByUser(_UserID, PortalID);
                        _RolesHydrated = true;
                    }
                }
                return _Roles;
            }
            set
            {
                _Roles = value;
                _RolesHydrated = true;
            }
        }
        [Browsable(false)]
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        [SortOrder(0), MaxLength(100), IsReadOnly(true), Required(true)]
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }
        public bool IsInRole(string role)
        {
            if (IsSuperUser || role == Globals.glbRoleAllUsersName)
            {
                return true;
            }
            else
            {
                if ("[" + UserID + "]" == role)
                {
                    return true;
                }
                if (Roles != null)
                {
                    foreach (string strRole in Roles)
                    {
                        if (strRole == role)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void UpdateDisplayName(string format)
        {
            format = format.Replace("[USERID]", this.UserID.ToString());
            format = format.Replace("[FIRSTNAME]", this.FirstName);
            format = format.Replace("[LASTNAME]", this.LastName);
            format = format.Replace("[USERNAME]", this.Username);
            DisplayName = format;
        }

        string strAdministratorRoleName;
        private bool isAdminUser(ref UserInfo AccessingUser)
        {
            if (AccessingUser == null || AccessingUser.UserID == -1)
            {
                return false;
            }
            else if (String.IsNullOrEmpty(strAdministratorRoleName))
            {
                PortalInfo ps = new PortalController().GetPortal(AccessingUser.PortalID);
                strAdministratorRoleName = ps.AdministratorRoleName;
            }
            return AccessingUser.IsInRole(strAdministratorRoleName) || AccessingUser.IsSuperUser;
        }
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Services.Tokens.Scope CurrentScope, ref bool PropertyNotFound)
        {
            Scope internScope;
            if (this.UserID == -1 && CurrentScope > Scope.Configuration)
            {
                internScope = Scope.Configuration;
            }
            else if (this.UserID != AccessingUser.UserID && !isAdminUser(ref AccessingUser) && CurrentScope > Scope.DefaultSettings)
            {
                internScope = Scope.DefaultSettings;
            }
            else
            {
                internScope = CurrentScope;
            }
            string OutputFormat = string.Empty;
            if (strFormat == string.Empty)
                OutputFormat = "g";
            else
                OutputFormat = strFormat;
            switch (strPropertyName.ToLower())
            {
                case "verificationcode":
                    if (internScope < Scope.SystemMessages) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return this.PortalID.ToString() + "-" + this.UserID.ToString();
                case "affiliateid":
                    if (internScope < Scope.SystemMessages) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (this.AffiliateID.ToString(OutputFormat, formatProvider));
                case "displayname":
                    if (internScope < Scope.Configuration) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return PropertyAccess.FormatString(this.DisplayName, strFormat);
                case "email":
                    if (internScope < Scope.DefaultSettings) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (PropertyAccess.FormatString(this.Email, strFormat));
                case "firstname":
                    if (internScope < Scope.DefaultSettings) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (PropertyAccess.FormatString(this.FirstName, strFormat));
                case "issuperuser":
                    if (internScope < Scope.Debug) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (this.IsSuperUser.ToString(formatProvider));
                case "lastname":
                    if (internScope < Scope.DefaultSettings) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (PropertyAccess.FormatString(this.LastName, strFormat));
                case "portalid":
                    if (internScope < Scope.Configuration) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (this.PortalID.ToString(OutputFormat, formatProvider));
                case "userid":
                    if (internScope < Scope.DefaultSettings) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (this.UserID.ToString(OutputFormat, formatProvider));
                case "username":
                    if (internScope < Scope.DefaultSettings) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (PropertyAccess.FormatString(this.Username, strFormat));
                case "fullname":
                    if (internScope < Scope.Configuration) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (PropertyAccess.FormatString(this.DisplayName, strFormat));
                case "roles":
                    if (CurrentScope < Scope.SystemMessages) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
                    return (PropertyAccess.FormatString(string.Join(", ", this.Roles), strFormat));
            }
            PropertyNotFound = true;
            return string.Empty;
        }
        [Browsable(false)]
        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }
    }
}
