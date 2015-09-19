using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security.Membership;

namespace CommonLibrary.Entities.Users.Users_Online
{
    public class UserOnlineController
    {
        private static MembershipProvider memberProvider = MembershipProvider.Instance();
        public void ClearUserList()
        {
            string key = "OnlineUserList";
            DataCache.RemoveCache(key);
        }
        public int GetOnlineTimeWindow()
        {
            return Host.Host.UsersOnlineTimeWindow;
        }
        public Hashtable GetUserList()
        {
            string key = "OnlineUserList";
            Hashtable userList = (Hashtable)DataCache.GetCache(key);
            if ((userList == null))
            {
                userList = new Hashtable();
                DataCache.SetCache(key, userList);
            }
            return userList;
        }
        public bool IsEnabled()
        {
            return Host.Host.EnableUsersOnline;
        }
        public bool IsUserOnline(UserInfo user)
        {
            bool isOnline = false;
            if (IsEnabled())
            {
                isOnline = memberProvider.IsUserOnline(user);
            }
            return isOnline;
        }
        public void SetUserList(Hashtable userList)
        {
            string key = "OnlineUserList";
            DataCache.SetCache(key, userList);
        }
        private void TrackAnonymousUser(HttpContext context)
        {
            string cookieName = "DotNetNukeAnonymous";
            PortalSettings portalSettings = (PortalSettings)context.Items["PortalSettings"];
            if (portalSettings == null)
            {
                return;
            }
            AnonymousUserInfo user;
            Hashtable userList = GetUserList();
            string userID;
            HttpCookie cookie = context.Request.Cookies[cookieName];
            if ((cookie == null))
            {
                userID = Guid.NewGuid().ToString();
                cookie = new HttpCookie(cookieName);
                cookie.Value = userID;
                cookie.Expires = DateTime.Now.AddMinutes(20);
                context.Response.Cookies.Add(cookie);
                user = new AnonymousUserInfo();
                user.UserID = userID;
                user.PortalID = portalSettings.PortalId;
                user.TabID = portalSettings.ActiveTab.TabID;
                user.CreationDate = DateTime.Now;
                user.LastActiveDate = DateTime.Now;
                if (!userList.Contains(userID))
                {
                    userList[userID] = user;
                }
            }
            else
            {
                if ((cookie.Value == null))
                {
                    context.Response.Cookies[cookieName].Expires = new System.DateTime(1999, 10, 12);
                    return;
                }
                userID = cookie.Value;
                if ((userList[userID] == null))
                {
                    userList[userID] = new AnonymousUserInfo();
                    ((AnonymousUserInfo)userList[userID]).CreationDate = DateTime.Now;
                }
                user = (AnonymousUserInfo)userList[userID];
                user.UserID = userID;
                user.PortalID = portalSettings.PortalId;
                user.TabID = portalSettings.ActiveTab.TabID;
                user.LastActiveDate = DateTime.Now;
                cookie = new HttpCookie(cookieName);
                cookie.Value = userID;
                cookie.Expires = DateTime.Now.AddMinutes(20);
                context.Response.Cookies.Add(cookie);
            }
        }
        private void TrackAuthenticatedUser(HttpContext context)
        {
            PortalSettings portalSettings = (PortalSettings)context.Items["PortalSettings"];
            if (portalSettings == null)
            {
                return;
            }
            UserInfo objUserInfo = UserController.GetCurrentUserInfo();
            Hashtable userList = GetUserList();
            OnlineUserInfo user = new OnlineUserInfo();
            if (objUserInfo.UserID > 0)
            {
                user.UserID = objUserInfo.UserID;
            }
            user.PortalID = portalSettings.PortalId;
            user.TabID = portalSettings.ActiveTab.TabID;
            user.LastActiveDate = DateTime.Now;
            if ((userList[objUserInfo.UserID.ToString()] == null))
            {
                user.CreationDate = user.LastActiveDate;
            }
            userList[objUserInfo.UserID.ToString()] = user;
            SetUserList(userList);
        }
        public void TrackUsers()
        {
            HttpContext context = HttpContext.Current;
            if (context.Items["CheckedUsersOnlineCookie"] != null)
            {
                return;
            }
            else
            {
                context.Items["CheckedUsersOnlineCookie"] = "true";
            }
            if ((context.Request.IsAuthenticated))
            {
                TrackAuthenticatedUser(context);
            }
            else
            {
                if ((context.Request.Browser.Cookies))
                {
                    TrackAnonymousUser(context);
                }
            }
        }
        public void UpdateUsersOnline()
        {
            Hashtable userList = GetUserList();
            Hashtable listToProcess = (Hashtable)userList.Clone();
            ClearUserList();
            try
            {
                memberProvider.UpdateUsersOnline(listToProcess);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            memberProvider.DeleteUsersOnline(GetOnlineTimeWindow());
        }
    }
}
