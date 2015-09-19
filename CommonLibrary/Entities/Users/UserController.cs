using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Threading;
using CommonLibrary.Security.Membership;
using CommonLibrary.Services.Messaging;
using CommonLibrary.Security;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Entities.Profile;
using CommonLibrary.Services.Localization;
using CommonLibrary.Services.Messaging.Data;
using CommonLibrary.Security.Roles;
using CommonLibrary.Services.Log.EventLog;
using CommonLibrary.Data;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Exceptions;

namespace CommonLibrary.Entities.Users
{
    public class UserController
    {
        string IP = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        private static MembershipProvider memberProvider = MembershipProvider.Instance();
        private static MessagingController _messagingController = new MessagingController();
        private static void AddEventLog(int portalId, string username, int userId, string portalName, string Ip, UserLoginStatus loginStatus)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            PortalSecurity objSecurity = new PortalSecurity();
            objEventLogInfo.AddProperty("IP", Ip);
            objEventLogInfo.LogPortalID = portalId;
            objEventLogInfo.LogPortalName = portalName;
            objEventLogInfo.LogUserName = objSecurity.InputFilter(username, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup);
            objEventLogInfo.LogUserID = userId;
            objEventLogInfo.LogTypeKey = loginStatus.ToString();
            objEventLog.AddLog(objEventLogInfo);
        }
        private static object GetCachedUserByPortalCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalId = (int)cacheItemArgs.ParamList[0];
            string username = (string)cacheItemArgs.ParamList[1];
            return memberProvider.GetUserByUserName(portalId, username);
        }
        static internal Hashtable GetUserSettings(Hashtable settings)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            if (settings["Column_FirstName"] == null)
            {
                settings["Column_FirstName"] = false;
            }
            if (settings["Column_LastName"] == null)
            {
                settings["Column_LastName"] = false;
            }
            if (settings["Column_DisplayName"] == null)
            {
                settings["Column_DisplayName"] = true;
            }
            if (settings["Column_Address"] == null)
            {
                settings["Column_Address"] = true;
            }
            if (settings["Column_Telephone"] == null)
            {
                settings["Column_Telephone"] = true;
            }
            if (settings["Column_Email"] == null)
            {
                settings["Column_Email"] = false;
            }
            if (settings["Column_CreatedDate"] == null)
            {
                settings["Column_CreatedDate"] = true;
            }
            if (settings["Column_LastLogin"] == null)
            {
                settings["Column_LastLogin"] = false;
            }
            if (settings["Column_Authorized"] == null)
            {
                settings["Column_Authorized"] = true;
            }
            if (settings["Display_Mode"] == null)
            {
                settings["Display_Mode"] = DisplayMode.All;
            }
            else
            {
                settings["Display_Mode"] = (DisplayMode)settings["Display_Mode"];
            }
            if (settings["Display_SuppressPager"] == null)
            {
                settings["Display_SuppressPager"] = false;
            }
            if (settings["Records_PerPage"] == null)
            {
                settings["Records_PerPage"] = 10;
            }
            if (settings["Profile_DefaultVisibility"] == null)
            {
                settings["Profile_DefaultVisibility"] = UserVisibilityMode.AdminOnly;
            }
            else
            {
                settings["Profile_DefaultVisibility"] = (UserVisibilityMode)settings["Profile_DefaultVisibility"];
            }
            if (settings["Profile_DisplayVisibility"] == null)
            {
                settings["Profile_DisplayVisibility"] = true;
            }
            if (settings["Profile_ManageServices"] == null)
            {
                settings["Profile_ManageServices"] = true;
            }
            if (settings["Redirect_AfterLogin"] == null)
            {
                settings["Redirect_AfterLogin"] = -1;
            }
            if (settings["Redirect_AfterRegistration"] == null)
            {
                settings["Redirect_AfterRegistration"] = -1;
            }
            if (settings["Redirect_AfterLogout"] == null)
            {
                settings["Redirect_AfterLogout"] = -1;
            }
            if (settings["Security_CaptchaLogin"] == null)
            {
                settings["Security_CaptchaLogin"] = false;
            }
            if (settings["Security_CaptchaRegister"] == null)
            {
                settings["Security_CaptchaRegister"] = false;
            }
            if (settings["Security_EmailValidation"] == null)
            {
                settings["Security_EmailValidation"] = Globals.glbEmailRegEx;
            }
            if (settings["Security_RequireValidProfile"] == null)
            {
                settings["Security_RequireValidProfile"] = false;
            }
            if (settings["Security_RequireValidProfileAtLogin"] == null)
            {
                settings["Security_RequireValidProfileAtLogin"] = true;
            }
            if (settings["Security_UsersControl"] == null)
            {
                if (_portalSettings != null && _portalSettings.Users > 1000)
                {
                    settings["Security_UsersControl"] = UsersControl.TextBox;
                }
                else
                {
                    settings["Security_UsersControl"] = UsersControl.Combo;
                }
            }
            else
            {
                settings["Security_UsersControl"] = (UsersControl)settings["Security_UsersControl"];
            }
            if (settings["Security_DisplayNameFormat"] == null)
            {
                settings["Security_DisplayNameFormat"] = "";
            }
            return settings;
        }
        public static bool ChangePassword(UserInfo user, string oldPassword, string newPassword)
        {
            bool retValue = Null.NullBoolean;
            if (ValidatePassword(newPassword))
            {
                retValue = memberProvider.ChangePassword(user, oldPassword, newPassword);
                user.Membership.UpdatePassword = false;
                UpdateUser(user.PortalID, user);
            }
            else
            {
                throw new Exception("Invalid Password");
            }
            return retValue;
        }
        public static bool ChangePasswordQuestionAndAnswer(UserInfo user, string password, string passwordQuestion, string passwordAnswer)
        {
            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog(user, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.USER_UPDATED);
            return memberProvider.ChangePasswordQuestionAndAnswer(user, password, passwordQuestion, passwordAnswer);
        }
        public static void CheckInsecurePassword(string Username, string Password, ref UserLoginStatus loginStatus)
        {
            if (Username == "admin" && (Password == "admin" || Password == "dnnadmin"))
            {
                loginStatus = UserLoginStatus.LOGIN_INSECUREADMINPASSWORD;
            }
            if (Username == "host" && (Password == "host" || Password == "dnnhost"))
            {
                loginStatus = UserLoginStatus.LOGIN_INSECUREHOSTPASSWORD;
            }
        }
        public static UserCreateStatus CreateUser(ref UserInfo objUser)
        {
            UserCreateStatus createStatus = UserCreateStatus.AddUser;
            createStatus = memberProvider.CreateUser(ref objUser);
            if (createStatus == UserCreateStatus.Success)
            {
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_CREATED);
                DataCache.ClearPortalCache(objUser.PortalID, false);
                if (!objUser.IsSuperUser)
                {
                    RoleController objRoles = new RoleController();
                    RoleInfo objRole;
                    ArrayList arrRoles = objRoles.GetPortalRoles(objUser.PortalID);
                    int i;
                    for (i = 0; i <= arrRoles.Count - 1; i++)
                    {
                        objRole = (RoleInfo)arrRoles[i];
                        if (objRole.AutoAssignment == true)
                        {
                            objRoles.AddUserRole(objUser.PortalID, objUser.UserID, objRole.RoleID, Null.NullDate, Null.NullDate);
                        }
                    }
                }
            }
            return createStatus;
        }
        public static bool DeleteUser(ref UserInfo objUser, bool notify, bool deleteAdmin)
        {
            bool CanDelete = true;
            IDataReader dr = null;
            try
            {
                dr = DataProvider.Instance().GetPortal(objUser.PortalID, PortalController.GetActivePortalLanguage(objUser.PortalID));
                if (dr.Read())
                {
                    if (objUser.UserID == Convert.ToInt32(dr["AdministratorId"]))
                    {
                        CanDelete = deleteAdmin;
                    }
                }
                if (CanDelete)
                {
                    FolderPermissionController.DeleteFolderPermissionsByUser(objUser);
                    ModulePermissionController.DeleteModulePermissionsByUser(objUser);
                    TabPermissionController.DeleteTabPermissionsByUser(objUser);
                    CanDelete = memberProvider.DeleteUser(objUser);
                }
                if (CanDelete)
                {
                    PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog("Username", objUser.Username, _portalSettings, objUser.UserID, Services.Log.EventLog.EventLogController.EventLogType.USER_DELETED);
                    if (notify && !objUser.IsSuperUser)
                    {
                        Message _message = new Message();
                        _message.FromUserID = _portalSettings.AdministratorId;
                        _message.ToUserID = _portalSettings.AdministratorId;
                        _message.Subject = Localization.GetSystemMessage(objUser.Profile.PreferredLocale, _portalSettings, "EMAIL_USER_UNREGISTER_SUBJECT", objUser, Localization.GlobalResourceFile, null, "", _portalSettings.AdministratorId);
                        _message.Body = Localization.GetSystemMessage(objUser.Profile.PreferredLocale, _portalSettings, "EMAIL_USER_UNREGISTER_BODY", objUser, Localization.GlobalResourceFile, null, "", _portalSettings.AdministratorId);
                        _message.Status = MessageStatusType.Unread;
                        //_messagingController.SaveMessage(_message);
                        Services.Mail.Mail.SendEmail(_portalSettings.Email, _portalSettings.Email, _message.Subject, _message.Body);
                    }
                    DataCache.ClearPortalCache(objUser.PortalID, false);
                    DataCache.ClearUserCache(objUser.PortalID, objUser.Username);
                }
            }
            catch (Exception Exc)
            {
                Exceptions.LogException(Exc);
                CanDelete = false;
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return CanDelete;
        }
        public static void DeleteUsers(int portalId, bool notify, bool deleteAdmin)
        {
            ArrayList arrUsers = GetUsers(portalId);
            for (int i = 0; i < arrUsers.Count; i++)
            {
                UserInfo objUser = arrUsers[i] as UserInfo;
                DeleteUser(ref objUser, notify, deleteAdmin);
            }
        }
        public static void DeleteUnauthorizedUsers(int portalId)
        {
            ArrayList arrUsers = GetUsers(portalId);
            for (int i = 0; i < arrUsers.Count; i++)
            {
                UserInfo objUser = arrUsers[i] as UserInfo;
                if (objUser.Membership.Approved == false || objUser.Membership.LastLoginDate == Null.NullDate)
                {
                    DeleteUser(ref objUser, true, false);
                }
            }
        }
        public static ArrayList FillUserCollection(int portalId, IDataReader dr, ref int totalRecords)
        {
            ArrayList arrUsers = new ArrayList();
            try
            {
                UserInfo obj;
                while (dr.Read())
                {
                    obj = FillUserInfo(portalId, dr, false);
                    arrUsers.Add(obj);
                }
                bool nextResult = dr.NextResult();
                totalRecords = Globals.GetTotalRecords(ref dr);
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return arrUsers;
        }
        public static ArrayList FillUserCollection(int portalId, IDataReader dr)
        {
            ArrayList arrUsers = new ArrayList();
            try
            {
                UserInfo obj;
                while (dr.Read())
                {
                    obj = FillUserInfo(portalId, dr, false);
                    arrUsers.Add(obj);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return arrUsers;
        }
        public static UserInfo FillUserInfo(int portalId, IDataReader dr, bool closeDataReader)
        {
            UserInfo objUserInfo = null;
            try
            {
                bool bContinue = true;
                if (closeDataReader)
                {
                    bContinue = false;
                    if (dr.Read())
                    {
                        if (string.Equals(dr.GetName(0), "UserID", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bContinue = true;
                        }
                    }
                }
                if (bContinue)
                {
                    objUserInfo = new UserInfo();
                    objUserInfo.PortalID = portalId;
                    objUserInfo.IsSuperUser = Null.SetNullBoolean(dr["IsSuperUser"]);
                    objUserInfo.IsDeleted = Null.SetNullBoolean(dr["IsDeleted"]);
                    objUserInfo.UserID = Null.SetNullInteger(dr["UserID"]);
                    objUserInfo.FirstName = Null.SetNullString(dr["FirstName"]);
                    objUserInfo.LastName = Null.SetNullString(dr["LastName"]);
                    objUserInfo.RefreshRoles = Null.SetNullBoolean(dr["RefreshRoles"]);
                    objUserInfo.DisplayName = Null.SetNullString(dr["DisplayName"]);
                    objUserInfo.AffiliateID = Null.SetNullInteger(Null.SetNull(dr["AffiliateID"], objUserInfo.AffiliateID));
                    objUserInfo.Username = Null.SetNullString(dr["Username"]);
                    GetUserMembership(objUserInfo);
                    objUserInfo.Email = Null.SetNullString(dr["Email"]);
                    objUserInfo.Membership.UpdatePassword = Null.SetNullBoolean(dr["UpdatePassword"]);
                    if (!objUserInfo.IsSuperUser)
                    {
                        objUserInfo.Membership.Approved = Null.SetNullBoolean(dr["Authorised"]);
                    }
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, closeDataReader);
            }
            return objUserInfo;
        }
        public static string GeneratePassword()
        {
            return GeneratePassword(MembershipProviderConfig.MinPasswordLength + 4);
        }
        public static string GeneratePassword(int length)
        {
            return memberProvider.GeneratePassword(length);
        }
        public static UserInfo GetCachedUser(int portalId, string username)
        {
            string cacheKey = string.Format(DataCache.UserCacheKey, portalId, username);
            return CBO.GetCachedObject<UserInfo>(new CacheItemArgs(cacheKey, DataCache.UserCacheTimeOut, DataCache.UserCachePriority, portalId, username), GetCachedUserByPortalCallBack);
        }
        public static UserInfo GetCurrentUserInfo()
        {
            if ((HttpContext.Current == null))
            {
                if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    return new UserInfo();
                }
                else
                {
                    PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                    if (_portalSettings != null)
                    {
                        UserInfo objUser = GetCachedUser(_portalSettings.PortalId, Thread.CurrentPrincipal.Identity.Name);
                        if (objUser != null)
                        {
                            return objUser;
                        }
                        else
                        {
                            return new UserInfo();
                        }
                    }
                    else
                    {
                        return new UserInfo();
                    }
                }
            }
            else
            {
                UserInfo objUser = (UserInfo)HttpContext.Current.Items["UserInfo"];
                if (objUser != null)
                {
                    return objUser;
                }
                else
                {
                    return new UserInfo();
                }
            }
        }
        public static ArrayList GetOnlineUsers(int PortalId)
        {
            return memberProvider.GetOnlineUsers(PortalId);
        }
        public static string GetPassword(ref UserInfo user, string passwordAnswer)
        {
            if (MembershipProviderConfig.PasswordRetrievalEnabled)
            {
                user.Membership.Password = memberProvider.GetPassword(user, passwordAnswer);
            }
            else
            {
                throw new ConfigurationErrorsException("Password Retrieval is not enabled");
            }
            return user.Membership.Password;
        }
        public static ArrayList GetUnAuthorizedUsers(int portalId)
        {
            return memberProvider.GetUnAuthorizedUsers(portalId);
        }
        public static UserInfo GetUserById(int portalId, int userId)
        {
            return memberProvider.GetUser(portalId, userId);
        }
        public static UserInfo GetUserByName(int portalId, string username)
        {
            return GetCachedUser(portalId, username);
        }
        public static int GetUserCountByPortal(int portalId)
        {
            return memberProvider.GetUserCountByPortal(portalId);
        }
        public static string GetUserCreateStatus(UserCreateStatus UserRegistrationStatus)
        {
            switch (UserRegistrationStatus)
            {
                case UserCreateStatus.DuplicateEmail:
                    return Localization.GetString("UserEmailExists");
                case UserCreateStatus.InvalidAnswer:
                    return Localization.GetString("InvalidAnswer");
                case UserCreateStatus.InvalidEmail:
                    return Localization.GetString("InvalidEmail");
                case UserCreateStatus.InvalidPassword:
                    string strInvalidPassword = Localization.GetString("InvalidPassword");
                    strInvalidPassword = strInvalidPassword.Replace("[PasswordLength]", MembershipProviderConfig.MinPasswordLength.ToString());
                    strInvalidPassword = strInvalidPassword.Replace("[NoneAlphabet]", MembershipProviderConfig.MinNonAlphanumericCharacters.ToString());
                    return strInvalidPassword;
                case UserCreateStatus.PasswordMismatch:
                    return Localization.GetString("PasswordMismatch");
                case UserCreateStatus.InvalidQuestion:
                    return Localization.GetString("InvalidQuestion");
                case UserCreateStatus.InvalidUserName:
                    return Localization.GetString("InvalidUserName");
                case UserCreateStatus.UserRejected:
                    return Localization.GetString("UserRejected");
                case UserCreateStatus.DuplicateUserName:
                case UserCreateStatus.UserAlreadyRegistered:
                case UserCreateStatus.UsernameAlreadyExists:
                    return Localization.GetString("UserNameExists");
                case UserCreateStatus.ProviderError:
                case UserCreateStatus.DuplicateProviderUserKey:
                case UserCreateStatus.InvalidProviderUserKey:
                    return Localization.GetString("RegError");
                default:
                    throw new ArgumentException("Unknown UserCreateStatus value encountered", "UserRegistrationStatus");
            }
        }
        public static void GetUserMembership(UserInfo objUser)
        {
            memberProvider.GetUserMembership(ref objUser);
        }
        public static Hashtable GetDefaultUserSettings()
        {
            return GetUserSettings(new Hashtable());
        }
        public static Hashtable GetUserSettings(int portalId)
        {
            Hashtable settings = GetDefaultUserSettings();
            Dictionary<string, string> settingsDictionary;
            if (portalId == Null.NullInteger)
            {
                settingsDictionary = Host.Host.GetHostSettingsDictionary();
            }
            else
            {
                settingsDictionary = PortalController.GetPortalSettingsDictionary(portalId);
            }
            string prefix;
            int index;
            foreach (KeyValuePair<string, string> kvp in settingsDictionary)
            {
                index = kvp.Key.IndexOf("_");
                if (index > 0)
                {
                    prefix = kvp.Key.Substring(0, index + 1);
                    switch (prefix)
                    {
                        case "Column_":
                        case "Display_":
                        case "Profile_":
                        case "Records_":
                        case "Redirect_":
                        case "Security_":
                            settings[kvp.Key] = kvp.Value;
                            break;
                    }
                }
            }
            return settings;
        }
        public static ArrayList GetUsers(int portalId)
        {
            int total = 0;
            return GetUsers(portalId, -1, -1, ref total);
        }
        public static ArrayList GetUsers(int portalId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsers(portalId, pageIndex, pageSize, ref totalRecords);
        }
        public static ArrayList GetUsersByEmail(int portalId, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize, ref totalRecords);
        }
        public static ArrayList GetUsersByUserName(int portalId, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByUserName(portalId, userNameToMatch, pageIndex, pageSize, ref totalRecords);
        }
        public static ArrayList GetUsersByProfileProperty(int portalId, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords)
        {
            return memberProvider.GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize, ref totalRecords);
        }
        public static string ResetPassword(UserInfo user, string passwordAnswer)
        {
            if (MembershipProviderConfig.PasswordResetEnabled)
            {
                user.Membership.Password = memberProvider.ResetPassword(user, passwordAnswer);
            }
            else
            {
                throw new ConfigurationErrorsException("Password Reset is not enabled");
            }
            return user.Membership.Password;
        }
        public static void SetAuthCookie(string username, bool CreatePersistentCookie)
        {
        }
        public static string SettingsKey(int portalId)
        {
            return "UserSettings|" + portalId.ToString();
        }
        public static bool UnLockUser(UserInfo user)
        {
            bool retValue = false;
            retValue = memberProvider.UnLockUser(user);
            DataCache.ClearUserCache(user.PortalID, user.Username);
            return retValue;
        }
        public static void UpdateUser(int portalId, UserInfo objUser)
        {
            UpdateUser(portalId, objUser, true);
        }
        /// <summary>
        /// updates a user
        /// </summary>
        /// <param name="portalId">the portalid of the user</param>
        /// <param name="objUser">the user object</param>
        /// <param name="loggedAction">whether or not the update calls the eventlog - the eventlogtype must still be enabled for logging to occur</param>
        /// <remarks></remarks>
        public static void UpdateUser(int portalId, UserInfo objUser, bool loggedAction)
        {
            //Update the User
            memberProvider.UpdateUser(objUser);
            if (loggedAction)
            {
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_UPDATED);
            }
            DataCache.ClearUserCache(portalId, objUser.Username);
        }
        public static UserInfo UserLogin(int portalId, string Username, string Password, string VerificationCode, string PortalName, string IP, ref UserLoginStatus loginStatus, bool CreatePersistentCookie)
        {
            loginStatus = UserLoginStatus.LOGIN_FAILURE;
            UserInfo objUser = ValidateUser(portalId, Username, Password, VerificationCode, PortalName, IP, ref loginStatus);
            if (objUser != null)
            {
                UserLogin(portalId, objUser, PortalName, IP, CreatePersistentCookie);
            }
            else
            {
                AddEventLog(portalId, Username, Null.NullInteger, PortalName, IP, loginStatus);
            }
            return objUser;
        }
        public static void UserLogin(int portalId, UserInfo user, string PortalName, string IP, bool CreatePersistentCookie)
        {
            if (user.IsSuperUser)
            {
                AddEventLog(portalId, user.Username, user.UserID, PortalName, IP, UserLoginStatus.LOGIN_SUPERUSER);
            }
            else
            {
                AddEventLog(portalId, user.Username, user.UserID, PortalName, IP, UserLoginStatus.LOGIN_SUCCESS);
            }
            user.LastIPAddress = IP;
            UserController.UpdateUser(portalId, user);
            FormsAuthentication.SetAuthCookie(user.Username, CreatePersistentCookie);
            int PersistentCookieTimeout = Config.GetPersistentCookieTimeout();
            if (CreatePersistentCookie == true)
            {
                FormsAuthenticationTicket AuthenticationTicket = new FormsAuthenticationTicket(user.Username, true, PersistentCookieTimeout);
                string EncryptedAuthTicket = FormsAuthentication.Encrypt(AuthenticationTicket);
                HttpCookie AuthCookie = new HttpCookie(FormsAuthentication.FormsCookieName, EncryptedAuthTicket);
                AuthCookie.Expires = AuthenticationTicket.Expiration;
                AuthCookie.Domain = FormsAuthentication.CookieDomain;
                AuthCookie.Path = FormsAuthentication.FormsCookiePath;
                HttpContext.Current.Response.Cookies.Set(AuthCookie);
            }
        }
        public static bool ValidatePassword(string password)
        {
            bool isValid = true;
            Regex rx;
            if (password.Length < MembershipProviderConfig.MinPasswordLength)
            {
                isValid = false;
            }
            rx = new Regex("[^0-9a-zA-Z]");
            if (rx.Matches(password).Count < MembershipProviderConfig.MinNonAlphanumericCharacters)
            {
                isValid = false;
            }
            if (!String.IsNullOrEmpty(MembershipProviderConfig.PasswordStrengthRegularExpression) && isValid)
            {
                rx = new Regex(MembershipProviderConfig.PasswordStrengthRegularExpression);
                isValid = rx.IsMatch(password);
            }
            return isValid;
        }
        public static UserInfo ValidateUser(int portalId, string Username, string Password, string VerificationCode, string PortalName, string IP, ref UserLoginStatus loginStatus)
        {
            return ValidateUser(portalId, Username, Password, "DNN", VerificationCode, PortalName, IP, ref loginStatus);
        }
        public static UserInfo ValidateUser(int portalId, string Username, string Password, string authType, string VerificationCode, string PortalName, string IP, ref UserLoginStatus loginStatus)
        {
            loginStatus = UserLoginStatus.LOGIN_FAILURE;
            UserInfo objUser = memberProvider.UserLogin(portalId, Username, Password, authType, VerificationCode, ref loginStatus);
            if (loginStatus == UserLoginStatus.LOGIN_USERLOCKEDOUT || loginStatus == UserLoginStatus.LOGIN_FAILURE)
            {
                AddEventLog(portalId, Username, Null.NullInteger, PortalName, IP, loginStatus);
            }
            if (loginStatus == UserLoginStatus.LOGIN_SUCCESS || loginStatus == UserLoginStatus.LOGIN_SUPERUSER)
            {
                CheckInsecurePassword(Username, Password, ref loginStatus);
            }
            return objUser;
        }
        public static UserValidStatus ValidateUser(UserInfo objUser, int portalId, bool ignoreExpiring)
        {
            UserValidStatus validStatus = UserValidStatus.VALID;
            if (objUser.Membership.UpdatePassword)
            {
                validStatus = UserValidStatus.UPDATEPASSWORD;
            }
            else if (PasswordConfig.PasswordExpiry > 0)
            {
                DateTime expiryDate = objUser.Membership.LastPasswordChangeDate.AddDays(PasswordConfig.PasswordExpiry);
                if (expiryDate < DateTime.Now)
                {
                    validStatus = UserValidStatus.PASSWORDEXPIRED;
                }
                else if (expiryDate < DateTime.Now.AddDays(PasswordConfig.PasswordExpiryReminder) && (!ignoreExpiring))
                {
                    validStatus = UserValidStatus.PASSWORDEXPIRING;
                }
            }
            if (validStatus == UserValidStatus.VALID)
            {
                bool validProfile = Convert.ToBoolean(UserModuleBase.GetSetting(portalId, "Security_RequireValidProfileAtLogin"));
                if (validProfile && (!ProfileController.ValidateProfile(portalId, objUser.Profile)))
                {
                    validStatus = UserValidStatus.UPDATEPROFILE;
                }
            }
            return validStatus;
        }
        private int _PortalId;
        private string _DisplayName;
        public string DisplayFormat
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }
        public int PortalId
        {
            get { return _PortalId; }
            set { _PortalId = value; }
        }
        public UserInfo GetUser(int portalId, int userId)
        {
            return GetUserById(portalId, userId);
        }
        public void UpdateDisplayNames()
        {
            ArrayList arrUsers = GetUsers(PortalId);
            foreach (UserInfo objUser in arrUsers)
            {
                objUser.UpdateDisplayName(DisplayFormat);
                UpdateUser(PortalId, objUser);
            }
        }

        #region Methods ===============================================================================
        public string[] GetUserPassByEmail(string Email)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_GetUserPassByEmail", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.Add("@o_return", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_username", SqlDbType.NVarChar, int.MaxValue).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_password", SqlDbType.NVarChar, int.MaxValue).Direction = ParameterDirection.Output;
            con.Open();
            cmd.ExecuteNonQuery();
            string[] result = new string[3];
            result[0] = cmd.Parameters["@o_return"].Value.ToString();
            result[1] = cmd.Parameters["@o_username"].Value.ToString();
            result[2] = cmd.Parameters["@o_password"].Value.ToString();
            con.Close();
            return result;
        }


        public string[] CheckLogin(string UserName, string Password)
        {
            MD5CryptEncrypt md5_obj = new MD5CryptEncrypt();
            string Hashed_Password = md5_obj.getMd5Hash(Password);
            string PageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString();

            SqlCommand cmd = new SqlCommand("aspnet_Users_CheckUserLogin", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Password", Hashed_Password);
            cmd.Parameters.AddWithValue("@PageUrl", PageUrl);
            cmd.Parameters.AddWithValue("@IPAddress", IP);
            cmd.Parameters.Add("@ApplicationId", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@RoleId", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@HomeDirectory", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@PortalId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@VendorId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@IsSuperUser", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@UpdatePassword", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@IsDeleted", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_return", SqlDbType.Int).Direction = ParameterDirection.Output;
            con.Open();
            cmd.ExecuteNonQuery();
            string[] result = new string[10];
            result[0] = cmd.Parameters["@ApplicationId"].Value.ToString();
            result[1] = cmd.Parameters["@UserId"].Value.ToString();
            result[2] = cmd.Parameters["@RoleId"].Value.ToString();
            result[3] = cmd.Parameters["@PortalId"].Value.ToString();
            result[4] = cmd.Parameters["@VendorId"].Value.ToString();
            result[5] = cmd.Parameters["@HomeDirectory"].Value.ToString();
            result[6] = cmd.Parameters["@IsSuperUser"].Value.ToString();
            result[7] = cmd.Parameters["@UpdatePassword"].Value.ToString();
            result[8] = cmd.Parameters["@IsDeleted"].Value.ToString();
            result[9] = cmd.Parameters["@o_return"].Value.ToString();
            con.Close();
            return result;
        }

        public string[] CheckLogin(string ApplicationId, string UserName, string Password)
        {
            MD5CryptEncrypt md5_obj = new MD5CryptEncrypt();
            string Hashed_Password = md5_obj.getMd5Hash(Password);
            string PageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString();

            SqlCommand cmd = new SqlCommand("aspnet_Users_CheckLogin", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Password", Hashed_Password);
            cmd.Parameters.AddWithValue("@PageUrl", PageUrl);
            cmd.Parameters.AddWithValue("@IPAddress", IP);
            cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@RoleId", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_return", SqlDbType.Int, 10).Direction = ParameterDirection.Output;
            con.Open();
            cmd.ExecuteNonQuery();
            string[] result = new string[3];
            result[0] = cmd.Parameters["@UserId"].Value.ToString();
            result[1] = cmd.Parameters["@RoleId"].Value.ToString();
            result[2] = cmd.Parameters["@o_return"].Value.ToString();
            con.Close();
            return result;
        }

        public string[] CheckUser(string UserName, string Password)
        {
            MD5CryptEncrypt md5_obj = new MD5CryptEncrypt();
            string Hashed_Password = md5_obj.getMd5Hash(Password);
            string PageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString();

            SqlCommand cmd = new SqlCommand("aspnet_Users_CheckUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Password", Hashed_Password);
            cmd.Parameters.AddWithValue("@PageUrl", PageUrl);
            cmd.Parameters.AddWithValue("@IPAddress", IP);
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            string[] result = new string[3];
            result[0] = cmd.Parameters["@UserId"].Value.ToString();
            result[1] = cmd.Parameters["@o_return"].Value.ToString();
            con.Close();
            return result;
        }

        public DataTable GetUsers(string ApplicationId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Users_GetUsers", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Users_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@UserId", UserId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailsByUserName(string UserName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Users_GetDetailsByUserName", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@UserName", UserName);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string CreateUser(string AppId, string UserName, byte IsUserAnonymous)
        {
            System.Guid ApplicationId = new System.Guid(AppId);
            DateTime LastActivityDate = System.DateTime.Now;

            SqlCommand cmd = new SqlCommand("aspnet_Users_CreateUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@IsUserAnonymous", IsUserAnonymous);
            cmd.Parameters.AddWithValue("@LastActivityDate", LastActivityDate);
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string UserId = cmd.Parameters["@UserId"].Value.ToString();
            con.Close();
            return UserId;
        }

        public int DeleteUser(string ApplicationName, string UserName, int TablesToDeleteFrom)
        {
            // Delete from Membership table if (@TablesToDeleteFrom & 1) is set
            // Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
            //-- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
            //    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
            //    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set

            SqlCommand cmd = new SqlCommand("aspnet_Users_DeleteUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@TablesToDeleteFrom", TablesToDeleteFrom);
            cmd.Parameters.Add(new SqlParameter("@NumTablesDeletedFrom", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@NumTablesDeletedFrom"].Value;
            con.Close();
            return retunvalue;
        }

        public int Insert(string ApplicationId, int PortalId, int VendorId, string RoleId, string Username,
            string Password, string PasswordSalt, string PasswordQuestion, string PasswordAnswer,
            string FullName, string DisplayName, string Address, string MobilePIN, string Phone, string Email,
            int IsSuperUser, int UpdatePassword, int IsDeleted, int IsApproved, string Comment)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Users_Insert", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@Username", Username);
            cmd.Parameters.AddWithValue("@Password", Password);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);
            cmd.Parameters.AddWithValue("@PasswordQuestion", PasswordQuestion);
            cmd.Parameters.AddWithValue("@PasswordAnswer", PasswordAnswer);
            cmd.Parameters.AddWithValue("@FullName", FullName);
            cmd.Parameters.AddWithValue("@DisplayName", DisplayName);
            cmd.Parameters.AddWithValue("@Address", Address);
            cmd.Parameters.AddWithValue("@MobilePIN", MobilePIN);
            cmd.Parameters.AddWithValue("@Phone", Phone);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@IsSuperUser", IsSuperUser);
            cmd.Parameters.AddWithValue("@UpdatePassword", UpdatePassword);
            cmd.Parameters.AddWithValue("@IsDeleted", IsDeleted);
            cmd.Parameters.AddWithValue("@IsApproved", IsApproved);
            cmd.Parameters.AddWithValue("@LastIPAddress", IP);
            cmd.Parameters.AddWithValue("@Comment", Comment);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }


        public int Update(string UserId, string ApplicationId, int PortalId, int VendorId, string RoleId, string Username,
            string Password, string PasswordSalt, string PasswordQuestion, string PasswordAnswer,
            string FullName, string DisplayName, string Address, string MobilePIN, string Phone, string Email,
            int IsSuperUser, int UpdatePassword, int IsDeleted, int IsApproved, int IsLockedOut, string Comment)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Users_Update", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.AddWithValue("@Username", Username);
            cmd.Parameters.AddWithValue("@Password", Password);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);
            cmd.Parameters.AddWithValue("@PasswordQuestion", PasswordQuestion);
            cmd.Parameters.AddWithValue("@PasswordAnswer", PasswordAnswer);
            cmd.Parameters.AddWithValue("@FullName", FullName);
            cmd.Parameters.AddWithValue("@DisplayName", DisplayName);
            cmd.Parameters.AddWithValue("@Address", Address);
            cmd.Parameters.AddWithValue("@MobilePIN", MobilePIN);
            cmd.Parameters.AddWithValue("@Phone", Phone);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@IsSuperUser", IsSuperUser);
            cmd.Parameters.AddWithValue("@UpdatePassword", UpdatePassword);
            cmd.Parameters.AddWithValue("@IsDeleted", IsDeleted);
            cmd.Parameters.AddWithValue("@IsApproved", IsApproved);
            cmd.Parameters.AddWithValue("@IsLockedOut", IsLockedOut);
            cmd.Parameters.AddWithValue("@LastIPAddress", IP);
            cmd.Parameters.AddWithValue("@Comment", Comment);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Edit(string Username, string Password, string PasswordSalt, string PasswordQuestion, string PasswordAnswer,
            string FullName, string DisplayName, string Address, string Phone, string Email)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Users_Edit", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Username", Username);
            cmd.Parameters.AddWithValue("@Password", Password);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);
            cmd.Parameters.AddWithValue("@PasswordQuestion", PasswordQuestion);
            cmd.Parameters.AddWithValue("@PasswordAnswer", PasswordAnswer);
            cmd.Parameters.AddWithValue("@FullName", FullName);
            cmd.Parameters.AddWithValue("@DisplayName", DisplayName);
            cmd.Parameters.AddWithValue("@Address", Address);
            cmd.Parameters.AddWithValue("@Phone", Phone);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@LastIPAddress", IP);

            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int LockUser(string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Users_LockUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UnlockUser(string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Users_UnlockUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(string UserId, int TablesToDeleteFrom)
        {
            // Delete from Membership table if (@TablesToDeleteFrom & 1) is set
            // Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
            //-- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
            //    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
            //    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set

            SqlCommand cmd = new SqlCommand("aspnet_Users_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.AddWithValue("@TablesToDeleteFrom", TablesToDeleteFrom);
            cmd.Parameters.Add(new SqlParameter("@NumTablesDeletedFrom", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@NumTablesDeletedFrom"].Value;
            con.Close();
            return retunvalue;
        }
        #endregion =================================================================================================
    }
}
