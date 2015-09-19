using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using System.Web;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Entities.Users
{
    public class UserTime
    {
        public UserTime()
        {
        }

        public static int GetSessionTimeOutFromWebConfig()
        {
            System.Xml.XmlDocument x = new System.Xml.XmlDocument();
            x.Load(AppDomain.CurrentDomain.BaseDirectory + "web.config");
            System.Xml.XmlNode node = x.SelectSingleNode("/configuration/system.web/authentication/forms");
            int time_out = int.Parse(node.Attributes["timeout"].Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            return time_out;
        }

        //public int TimeOut_Get()
        //{
        //    //Get formsauthentication TimeOut value Kludge, timeout property is not exposed in the class
        //    FormsAuthentication.SetAuthCookie("Username", false);
        //    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Response.Cookies[FormsAuthentication.FormsCookieName].Value);
        //    TimeSpan ts = new TimeSpan(ticket.Expiration.Ticks - ticket.IssueDate.Ticks);
        //    return ts.Minutes;
        //}

        //public static int SaveSessionTimeOutFromFormAuthenticationTicket2Cookie(string username, int timeout, bool remember_checked, string roles)
        //{
        //    // Create forms authentication ticket
        //    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket (
        //    1, // Ticket version
        //    username, // Username to be associated with this ticket
        //    DateTime .Now, // Date/time ticket was issued
        //    DateTime .Now.AddMinutes(timeout), // Date and time the cookie will expire
        //    remember_checked, // if user has chcked rememebr me then create persistent cookie
        //    roles, // store the user data, in this case roles of the user
        //    FormsAuthentication .FormsCookiePath); // Cookie path specified in the web.config file in <Forms> tag if any.

        //    // To give more security it is suggested to hash it
        //    string hashCookies = FormsAuthentication .Encrypt(ticket);
        //    HttpCookie cookie = new HttpCookie ( FormsAuthentication .FormsCookieName, hashCookies); // Hashed ticket

        //    // Add the cookie to the response, user browser
        //    Response.Cookies.Add(cookie); 
        //}

        //public object ManageSessionTimeOut()
        //{
        //    DateTime StartTime;
        //    DateTime EndTime;
        //    TimeSpan Duration;
        //    int intDifference;

        //    if (Session["LastRefreshTime"] == null)
        //    {
        //        StartTime = DateTime.Now;
        //    }
        //    else
        //    {
        //        StartTime = Session["LastRefreshTime"];
        //    }
        //    EndTime = DateTime.Now;
        //    Duration = EndTime.Subtract(StartTime);
        //    Session["LastRefreshTime"] = EndTime;

        //    intDifference = Duration.Hours.ToString() * 60 * 60 + Duration.Minutes.ToString() * 60 + Duration.Seconds.ToString();

        //    if ((intDifference < 59))
        //    {
        //        Session["TimeOut"] = 0;
        //    }
        //    else
        //    {
        //        Session["TimeOut"] = Session["TimeOut"] + 59;
        //    }

        //    if ((Session["TimeOut"] >= "177"))
        //    {
        //        Session.Abandon();
        //    }
        //}

        public DateTime ConvertToUserTime(DateTime dt, double ClientTimeZone)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return dt.AddMinutes(FromClientToServerFactor(ClientTimeZone, _portalSettings.TimeZoneOffset));
        }
        public DateTime ConvertToServerTime(DateTime dt, double ClientTimeZone)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            return dt.AddMinutes(FromServerToClientFactor(ClientTimeZone, _portalSettings.TimeZoneOffset));
        }
        public static DateTime CurrentTimeForUser(UserInfo User)
        {
            if (User == null || User.UserID == -1 || User.Profile.TimeZone == Null.NullInteger)
            {
                int intOffset = 0;
                PortalSettings objSettings = PortalController.GetCurrentPortalSettings();
                if (objSettings != null)
                {
                    intOffset = objSettings.TimeZoneOffset;
                }
                else
                {
                    PortalInfo objPCtr = new PortalController().GetPortal(User.PortalID);
                    intOffset = objPCtr.TimeZoneOffset;
                }
                return DateTime.UtcNow.AddMinutes(intOffset);
            }
            else
            {
                return DateTime.UtcNow.AddMinutes(User.Profile.TimeZone);
            }
        }
        public DateTime CurrentUserTime
        {
            get
            {
                HttpContext context = HttpContext.Current;
                PortalSettings objSettings = PortalController.GetCurrentPortalSettings();
                if (!context.Request.IsAuthenticated)
                {
                    return DateTime.UtcNow.AddMinutes(objSettings.TimeZoneOffset);
                }
                else
                {
                    return DateTime.UtcNow.AddMinutes(objSettings.TimeZoneOffset).AddMinutes(ClientToServerTimeZoneFactor);
                }
            }
        }
        public double ClientToServerTimeZoneFactor
        {
            get
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                return FromClientToServerFactor(objUserInfo.Profile.TimeZone, _portalSettings.TimeZoneOffset);
            }
        }
        public double ServerToClientTimeZoneFactor
        {
            get
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                return FromServerToClientFactor(objUserInfo.Profile.TimeZone, _portalSettings.TimeZoneOffset);
            }
        }
        private double FromClientToServerFactor(double Client, double Server)
        {
            return Client - Server;
        }
        private double FromServerToClientFactor(double Client, double Server)
        {
            return Server - Client;
        }
    }
}
