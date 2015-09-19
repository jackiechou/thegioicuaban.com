using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using CommonLibrary.Entities.Portal;

namespace CommonLibrary.Services.Personalization
{
    public class Personalization
    {
        private static PersonalizationInfo LoadProfile()
        {
            HttpContext context = HttpContext.Current;
            PersonalizationInfo objPersonalization = (PersonalizationInfo)context.Items["Personalization"];
            if (objPersonalization == null)
            {
                PortalSettings _portalSettings = (PortalSettings)context.Items["PortalSettings"];
                Entities.Users.UserInfo UserInfo = Entities.Users.UserController.GetCurrentUserInfo();
                PersonalizationController objPersonalizationController = new PersonalizationController();
                objPersonalization = objPersonalizationController.LoadProfile(UserInfo.UserID, _portalSettings.PortalId);
                context.Items.Add("Personalization", objPersonalization);
            }
            return objPersonalization;
        }
        public static object GetProfile(string NamingContainer, string Key)
        {
            return GetProfile(LoadProfile(), NamingContainer, Key);
        }
        public static object GetProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key)
        {
            if (objPersonalization != null)
            {
                return objPersonalization.Profile[NamingContainer + ":" + Key];
            }
            else
            {
                return "";
            }
        }
        public static void RemoveProfile(string NamingContainer, string Key)
        {
            RemoveProfile(LoadProfile(), NamingContainer, Key);
        }
        public static void RemoveProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key)
        {
            if (objPersonalization != null)
            {
                ((Hashtable)objPersonalization.Profile).Remove(NamingContainer + ":" + Key);
                objPersonalization.IsModified = true;
            }
        }
        public static void SetProfile(string NamingContainer, string Key, object Value)
        {
            SetProfile(LoadProfile(), NamingContainer, Key, Value);
        }
        public static void SetProfile(PersonalizationInfo objPersonalization, string NamingContainer, string Key, object value)
        {
            if (objPersonalization != null)
            {
                objPersonalization.Profile[NamingContainer + ":" + Key] = value;
                objPersonalization.IsModified = true;
            }
        }
    }
}
