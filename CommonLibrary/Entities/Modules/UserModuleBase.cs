using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Localization;
using CommonLibrary.Services.Mail;
using CommonLibrary.Entities.Users;
using CommonLibrary.Common.Utilities;
using System.Collections;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common;
using CommonLibrary.UI.Skins.Controls;

namespace CommonLibrary.Entities.Modules
{
    public enum DisplayMode
    {
        All = 0,
        FirstLetter = 1,
        None = 2
    }
    public enum UsersControl
    {
        Combo = 0,
        TextBox = 1
    }
    public class UserModuleBase : PortalModuleBase
    {
        public static object GetSetting(int portalId, string settingKey)
        {
            Hashtable settings = UserController.GetUserSettings(portalId);
            if (settings[settingKey] == null)
            {
                settings = UserController.GetUserSettings(settings);
            }
            return settings[settingKey];
        }
        public static void UpdateSetting(int portalId, string key, string setting)
        {
            if (portalId == Null.NullInteger)
            {
                Host.HostSettingsController controller = new Host.HostSettingsController();
                controller.UpdateHostSetting(key, setting);
            }
            else
            {
                PortalController.UpdatePortalSetting(portalId, key, setting);
            }
        }
        public static void UpdateSettings(int portalId, Hashtable settings)
        {
            string key;
            string setting;
            IDictionaryEnumerator settingsEnumerator = settings.GetEnumerator();
            while (settingsEnumerator.MoveNext())
            {
                key = Convert.ToString(settingsEnumerator.Key);
                setting = Convert.ToString(settingsEnumerator.Value);
                UpdateSetting(portalId, key, setting);
            }
        }
        private UserInfo _User;
        protected bool AddUser
        {
            get { return (UserId == Null.NullInteger); }
        }
        protected bool IsAdmin
        {
            get { return this.IsEditable; }
        }
        protected bool IsHostTab
        {
            get { return (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId); }
        }
        protected bool IsEdit
        {
            get
            {
                bool _IsEdit = false;
                if (Request.QueryString["ctl"] != null)
                {
                    string ctl = Request.QueryString["ctl"];
                    if (ctl == "Edit")
                    {
                        _IsEdit = true;
                    }
                }
                return _IsEdit;
            }
        }
        protected bool IsProfile
        {
            get
            {
                bool _IsProfile = false;
                if (IsUser)
                {
                    if (PortalSettings.UserTabId != -1)
                    {
                        if (PortalSettings.ActiveTab.TabID == PortalSettings.UserTabId)
                        {
                            _IsProfile = true;
                        }
                    }
                    else
                    {
                        if (Request.QueryString["ctl"] != null)
                        {
                            string ctl = Request.QueryString["ctl"];
                            if (ctl == "Profile")
                            {
                                _IsProfile = true;
                            }
                        }
                    }
                }
                return _IsProfile;
            }
        }
        protected bool IsRegister
        {
            get
            {
                bool _IsRegister = false;
                if (!IsAdmin && !IsUser)
                {
                    _IsRegister = true;
                }
                return _IsRegister;
            }
        }
        protected bool IsUser
        {
            get
            {
                bool _IsUser = false;
                if (Request.IsAuthenticated)
                {
                    _IsUser = (User.UserID == UserInfo.UserID);
                }
                return _IsUser;
            }
        }
        protected int UserPortalID
        {
            get
            {
                if (IsHostTab)
                {
                    return Null.NullInteger;
                }
                else
                {
                    return PortalId;
                }
            }
        }
        public UserInfo User
        {
            get
            {
                if (_User == null)
                {
                    if (AddUser)
                    {
                        _User = InitialiseUser();
                    }
                    else
                    {
                        _User = UserController.GetUserById(UserPortalID, UserId);
                    }
                }
                return _User;
            }
            set
            {
                _User = value;
                if (_User != null)
                {
                    UserId = _User.UserID;
                }
            }
        }
        public new int UserId
        {
            get
            {
                int _UserId = Null.NullInteger;
                if (ViewState["UserId"] == null)
                {
                    if (Request.QueryString["userid"] != null)
                    {
                        _UserId = Int32.Parse(Request.QueryString["userid"]);
                        ViewState["UserId"] = _UserId;
                    }
                }
                else
                {
                    _UserId = Convert.ToInt32(ViewState["UserId"]);
                }
                return _UserId;
            }
            set { ViewState["UserId"] = value; }
        }
        private UserInfo InitialiseUser()
        {
            UserInfo newUser = new UserInfo();
            if (IsHostMenu)
            {
                newUser.IsSuperUser = true;
            }
            else
            {
                newUser.PortalID = PortalId;
            }
            //newUser.Profile.InitialiseProfile(PortalId);
            //newUser.Profile.TimeZone = this.PortalSettings.TimeZoneOffset;
            //string lc = new Localization.Localization().CurrentCulture;
            //if (string.IsNullOrEmpty(lc))
            //    lc = this.PortalSettings.DefaultLanguage;
            //newUser.Profile.PreferredLocale = lc;
            //string country = Null.NullString;
            //country = LookupCountry();
            //if (!String.IsNullOrEmpty(country))
            //{
            //    newUser.Profile.Country = country;
            //}
            int AffiliateId = Null.NullInteger;
            if (Request.Cookies["AffiliateId"] != null)
            {
                AffiliateId = int.Parse(Request.Cookies["AffiliateId"].Value);
            }
            newUser.AffiliateID = AffiliateId;
            return newUser;
        }
        //private string LookupCountry()
        //{
        //    string IP;
        //    bool IsLocal = false;
        //    bool _CacheGeoIPData = true;
        //    string _GeoIPFile;
        //    _GeoIPFile = "controls/CountryListBox/Data/GeoIP.dat";
        //    if (this.Page.Request.UserHostAddress == "127.0.0.1")
        //    {
        //        IsLocal = true;
        //        IP = this.Page.Request.UserHostAddress;
        //    }
        //    else
        //    {
        //        IP = this.Page.Request.UserHostAddress;
        //    }
        //    if (Context.Cache.Get("GeoIPData") == null && _CacheGeoIPData)
        //    {
        //        Context.Cache.Insert("GeoIPData", DotNetNuke.UI.WebControls.CountryLookup.FileToMemory(Context.Server.MapPath(_GeoIPFile)), new System.Web.Caching.CacheDependency(Context.Server.MapPath(_GeoIPFile)));
        //    }
        //    if (IsLocal)
        //    {
        //        return Null.NullString;
        //    }
        //    DotNetNuke.UI.WebControls.CountryLookup _CountryLookup;
        //    if (_CacheGeoIPData)
        //    {
        //        _CountryLookup = new DotNetNuke.UI.WebControls.CountryLookup((MemoryStream)Context.Cache.Get("GeoIPData"));
        //    }
        //    else
        //    {
        //        _CountryLookup = new DotNetNuke.UI.WebControls.CountryLookup(Context.Server.MapPath(_GeoIPFile));
        //    }
        //    string country = Null.NullString;
        //    try
        //    {
        //        country = _CountryLookup.LookupCountryName(IP);
        //    }
        //    catch (Exception ex)
        //    {
        //        Exceptions.LogException(ex);
        //    }
        //    return country;
        //}
        //protected void AddLocalizedModuleMessage(string message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType type, bool display)
        //{
        //    if (display)
        //    {
        //        UI.Skins.Skin.AddModuleMessage(this, message, type);
        //    }
        //}
        //protected void AddModuleMessage(string message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType type, bool display)
        //{
        //    AddLocalizedModuleMessage(Localization.GetString(message, LocalResourceFile), type, display);
        //}
        //protected string CompleteUserCreation(UserCreateStatus createStatus, UserInfo newUser, bool notify, bool register)
        //{
        //    string strMessage = "";
        //    DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType message = DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError;
        //    if (register)
        //    {
        //        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationAdmin, PortalSettings);
        //        switch (PortalSettings.UserRegistration)
        //        {
        //            case (int)Globals.PortalRegistrationType.PrivateRegistration:
        //                strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationPrivate, PortalSettings);
        //                if (string.IsNullOrEmpty(strMessage))
        //                {
        //                    strMessage += string.Format(Localization.GetString("PrivateConfirmationMessage", Localization.SharedResourceFile), newUser.Email);
        //                    message = DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess;
        //                }
        //                break;
        //            case (int)Globals.PortalRegistrationType.PublicRegistration:
        //                Mail.SendMail(newUser, MessageType.UserRegistrationPublic, PortalSettings);
        //                UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
        //                UserController.UserLogin(PortalSettings.PortalId, newUser.Username, newUser.Membership.Password, "", PortalSettings.PortalName, "", ref loginStatus, false);
        //                break;
        //            case (int)Globals.PortalRegistrationType.VerifiedRegistration:
        //                strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationVerified, PortalSettings);
        //                if (string.IsNullOrEmpty(strMessage))
        //                {
        //                    strMessage += string.Format(Localization.GetString("VerifiedConfirmationMessage", Localization.SharedResourceFile), newUser.Email);
        //                    message = DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess;
        //                }
        //                break;
        //        }
        //        if (!Null.IsNull(User.AffiliateID))
        //        {
        //            Services.Vendors.AffiliateController objAffiliates = new Services.Vendors.AffiliateController();
        //            objAffiliates.UpdateAffiliateStats(newUser.AffiliateID, 0, 1);
        //        }
        //        Localization.SetLanguage(newUser.Profile.PreferredLocale);
        //        if (IsRegister && message == ModuleMessage.ModuleMessageType.RedError)
        //        {
        //            AddLocalizedModuleMessage(string.Format(Localization.GetString("SendMail.Error", Localization.SharedResourceFile), strMessage), message, (!String.IsNullOrEmpty(strMessage)));
        //        }
        //        else
        //        {
        //            AddLocalizedModuleMessage(strMessage, message, (!String.IsNullOrEmpty(strMessage)));
        //        }
        //    }
        //    else
        //    {
        //        //if (notify)
        //        //{
        //        //    if (PortalSettings.UserRegistration == (int)Globals.PortalRegistrationType.VerifiedRegistration)
        //        //    {
        //        //        strMessage += Mails.SendMail(newUser, MessageType.UserRegistrationVerified, PortalSettings);
        //        //    }
        //        //    else
        //        //    {
        //        //        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationPublic, PortalSettings);
        //        //    }
        //        //}
        //    }
        //    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
        //    objEventLog.AddLog(newUser, PortalSettings, UserId, newUser.Username, Services.Log.EventLog.EventLogController.EventLogType.USER_CREATED);
        //    return strMessage;
        //}
       
    }
}
