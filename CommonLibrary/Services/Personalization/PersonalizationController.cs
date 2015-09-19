using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Data;
using CommonLibrary.Common;
using System.Web;
using CommonLibrary.Common.Utilities;
using System.Collections;
using System.Data;

namespace CommonLibrary.Services.Personalization
{
    public class PersonalizationController
    {
        public void LoadProfile(HttpContext objHTTPContext, int UserId, int PortalId)
        {
            if ((PersonalizationInfo)HttpContext.Current.Items["Personalization"] == null)
            {
                objHTTPContext.Items.Add("Personalization", LoadProfile(UserId, PortalId));
            }
        }
        public PersonalizationInfo LoadProfile(int UserId, int PortalId)
        {
            PersonalizationInfo objPersonalization = new PersonalizationInfo();
            objPersonalization.UserId = UserId;
            objPersonalization.PortalId = PortalId;
            objPersonalization.IsModified = false;
            string profileData = Null.NullString;
            if (UserId > Null.NullInteger)
            {
                IDataReader dr = null;
                try
                {
                    dr = DataProvider.Instance().GetProfile(UserId, PortalId);
                    if (dr.Read())
                    {
                        profileData = dr["ProfileData"].ToString();
                    }
                    else
                    {
                        try
                        {
                            DataProvider.Instance().AddProfile(UserId, PortalId);
                        }
                        catch
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.Exceptions.LogException(ex);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
            }
            else
            {
                HttpContext context = HttpContext.Current;
                if (context != null && context.Request != null && context.Request.Cookies["Personalization"] != null)
                {
                    profileData = context.Request.Cookies["Personalization"].Value;
                }
            }
            if (string.IsNullOrEmpty(profileData))
            {
                objPersonalization.Profile = new Hashtable();
            }
            else
            {
                objPersonalization.Profile = Globals.DeserializeHashTableXml(profileData);
            }
            return objPersonalization;
        }
        public void SaveProfile(PersonalizationInfo objPersonalization)
        {
            SaveProfile(objPersonalization, objPersonalization.UserId, objPersonalization.PortalId);
        }
        public void SaveProfile(HttpContext objHTTPContext, int UserId, int PortalId)
        {
            PersonalizationInfo objPersonalization = (PersonalizationInfo)objHTTPContext.Items["Personalization"];
            SaveProfile(objPersonalization, UserId, PortalId);
        }
        public void SaveProfile(PersonalizationInfo objPersonalization, int UserId, int PortalId)
        {
            if (objPersonalization != null)
            {
                if (objPersonalization.IsModified)
                {
                    string ProfileData = Globals.SerializeHashTableXml(objPersonalization.Profile);
                    if (UserId > Null.NullInteger)
                    {
                        DataProvider.Instance().UpdateProfile(UserId, PortalId, ProfileData);
                    }
                    else
                    {
                        HttpContext context = HttpContext.Current;
                        if (context != null && context.Response != null)
                        {
                            HttpCookie personalizationCookie = new HttpCookie("DNNPersonalization");
                            personalizationCookie.Value = ProfileData;
                            personalizationCookie.Expires = DateTime.Now.AddDays(30);
                            context.Response.Cookies.Add(personalizationCookie);
                        }
                    }
                }
            }
        }
    }
}
