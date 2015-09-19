using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Portal;
using System.Web;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Common;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Data;

namespace CommonLibrary.Services.Authentication
{
    public class AuthenticationController
    {
        private static DataProvider provider = DataProvider.Instance();
        private static object GetAuthenticationServicesCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillCollection<AuthenticationInfo>(provider.GetAuthenticationServices());
        }
        public static int AddAuthentication(AuthenticationInfo authSystem)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(authSystem, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.AUTHENTICATION_CREATED);
            return provider.AddAuthentication(authSystem.PackageID, authSystem.AuthenticationType, authSystem.IsEnabled, authSystem.SettingsControlSrc, authSystem.LoginControlSrc, authSystem.LogoffControlSrc, UserController.GetCurrentUserInfo().UserID);
        }
        public static int AddUserAuthentication(int userID, string authenticationType, string authenticationToken)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("userID/authenticationType", userID.ToString() + "/" + authenticationType.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.AUTHENTICATION_USER_CREATED);
            return provider.AddUserAuthentication(userID, authenticationType, authenticationToken, UserController.GetCurrentUserInfo().UserID);
        }
        public static void DeleteAuthentication(AuthenticationInfo authSystem)
        {
            provider.DeleteAuthentication(authSystem.AuthenticationID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(authSystem, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.AUTHENTICATION_DELETED);
        }
        public static AuthenticationInfo GetAuthenticationService(int authenticationID)
        {
            AuthenticationInfo authInfo = null;
            foreach (AuthenticationInfo authService in GetAuthenticationServices())
            {
                if (authService.AuthenticationID == authenticationID)
                {
                    authInfo = authService;
                    break;
                }
            }
            if (authInfo == null)
            {
                return CBO.FillObject<AuthenticationInfo>(provider.GetAuthenticationService(authenticationID));
            }
            return authInfo;
        }
        public static AuthenticationInfo GetAuthenticationServiceByPackageID(int packageID)
        {
            AuthenticationInfo authInfo = null;
            foreach (AuthenticationInfo authService in GetAuthenticationServices())
            {
                if (authService.PackageID == packageID)
                {
                    authInfo = authService;
                    break;
                }
            }
            if (authInfo == null)
            {
                return CBO.FillObject<AuthenticationInfo>(provider.GetAuthenticationServiceByPackageID(packageID));
            }
            return authInfo;
        }
        public static AuthenticationInfo GetAuthenticationServiceByType(string authenticationType)
        {
            AuthenticationInfo authInfo = null;
            foreach (AuthenticationInfo authService in GetAuthenticationServices())
            {
                if (authService.AuthenticationType == authenticationType)
                {
                    authInfo = authService;
                    break;
                }
            }
            if (authInfo == null)
            {
                return CBO.FillObject<AuthenticationInfo>(provider.GetAuthenticationServiceByType(authenticationType));
            }
            return authInfo;
        }
        public static List<AuthenticationInfo> GetAuthenticationServices()
        {
            return CBO.GetCachedObject<List<AuthenticationInfo>>(new CacheItemArgs(DataCache.AuthenticationServicesCacheKey, DataCache.AuthenticationServicesCacheTimeOut, DataCache.AuthenticationServicesCachePriority), GetAuthenticationServicesCallBack);
        }
        public static AuthenticationInfo GetAuthenticationType()
        {
            AuthenticationInfo objAuthentication = null;
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                try
                {
                    objAuthentication = GetAuthenticationServiceByType(HttpContext.Current.Request["authentication"]);
                }
                catch
                {
                }
            }
            return objAuthentication;
        }
        public static List<AuthenticationInfo> GetEnabledAuthenticationServices()
        {
            List<AuthenticationInfo> enabled = new List<AuthenticationInfo>();
            foreach (AuthenticationInfo authService in GetAuthenticationServices())
            {
                if (authService.IsEnabled)
                {
                    enabled.Add(authService);
                }
            }
            return enabled;
        }
        public static string GetLogoffRedirectURL(PortalSettings settings, HttpRequest request)
        {
            string _RedirectURL = "";
            object setting = UserModuleBase.GetSetting(settings.PortalId, "Redirect_AfterLogout");
            if (Convert.ToInt32(setting) == Null.NullInteger)
            {
                if (TabPermissionController.CanViewPage())
                {
                    _RedirectURL = Globals.NavigateURL(settings.ActiveTab.TabID);
                }
                else if (settings.HomeTabId != -1)
                {
                    _RedirectURL = Globals.NavigateURL(settings.HomeTabId);
                }
                else
                {
                    _RedirectURL = Globals.GetPortalDomainName(settings.PortalAlias.HTTPAlias, request, true) + "/" + Globals.glbDefaultPage;
                }
            }
            else
            {
                _RedirectURL = Globals.NavigateURL(Convert.ToInt32(setting));
            }
            return _RedirectURL;
        }
        public static void SetAuthenticationType(string value)
        {
            SetAuthenticationType(value, false);
        }
        public static void SetAuthenticationType(string value, bool CreatePersistentCookie)
        {
            try
            {
                int PersistentCookieTimeout = Config.GetPersistentCookieTimeout();
                HttpResponse Response = HttpContext.Current.Response;
                if (Response == null)
                {
                    return;
                }
                System.Web.HttpCookie cookie = null;
                cookie = Response.Cookies.Get("authentication");
                if ((cookie == null))
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        cookie = new System.Web.HttpCookie("authentication", value);
                        if (CreatePersistentCookie)
                        {
                            cookie.Expires = DateTime.Now.AddMinutes(PersistentCookieTimeout);
                        }
                        Response.Cookies.Add(cookie);
                    }
                }
                else
                {
                    cookie.Value = value;
                    if (!String.IsNullOrEmpty(value))
                    {
                        if (CreatePersistentCookie)
                        {
                            cookie.Expires = DateTime.Now.AddMinutes(PersistentCookieTimeout);
                        }
                        Response.Cookies.Set(cookie);
                    }
                    else
                    {
                        Response.Cookies.Remove("authentication");
                    }
                }
            }
            catch
            {
                return;
            }
        }
        public static void UpdateAuthentication(AuthenticationInfo authSystem)
        {
            provider.UpdateAuthentication(authSystem.AuthenticationID, authSystem.PackageID, authSystem.AuthenticationType, authSystem.IsEnabled, authSystem.SettingsControlSrc, authSystem.LoginControlSrc, authSystem.LogoffControlSrc, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(authSystem, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.AUTHENTICATION_UPDATED);
        }
    }
}
