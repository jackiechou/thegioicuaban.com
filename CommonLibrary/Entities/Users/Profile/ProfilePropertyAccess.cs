using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Tokens.PropertyAccess;
using CommonLibrary.Services.Tokens;
using CommonLibrary.Entities.Portal;
using System.Web;
using CommonLibrary.Common;
using CommonLibrary.Entities.Profile;

namespace CommonLibrary.Entities.Users.Profile
{
    public class ProfilePropertyAccess : IPropertyAccess
    {
        UserInfo objUser;
        string strAdministratorRoleName;
        public ProfilePropertyAccess(UserInfo user)
        {
            this.objUser = user;
        }
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Entities.Users.UserInfo AccessingUser, Scope currentScope, ref bool PropertyNotFound)
        {
            string result = string.Empty;
            if (currentScope >= Scope.DefaultSettings && objUser != null && objUser.Profile != null)
            {
                CommonLibrary.Entities.Users.UserProfile objProfile = objUser.Profile;
                foreach (ProfilePropertyDefinition prop in objProfile.ProfileProperties)
                {
                    if (prop.PropertyName.ToLower() == strPropertyName.ToLower())
                    {
                        if (CheckAccessLevel(prop.Visibility, AccessingUser))
                        {
                            result = GetRichValue(prop, strFormat, formatProvider);
                        }
                        else
                        {
                            PropertyNotFound = true;
                            result = PropertyAccess.ContentLocked;
                        }
                        //break;
                    }
                }
            }
            PropertyNotFound = true;
            return result;
        }
        public static string GetRichValue(ProfilePropertyDefinition prop, string strFormat, System.Globalization.CultureInfo formatProvider)
        {
            string result = "";
            if (!String.IsNullOrEmpty(prop.PropertyValue) || DisplayDataType(prop).ToLower() == "image")
            {
                switch (DisplayDataType(prop).ToLower())
                {
                    case "truefalse":
                        result = PropertyAccess.Boolean2LocalizedYesNo(Convert.ToBoolean(prop.PropertyValue), formatProvider);
                        break;
                    case "date":
                    case "datetime":
                        if (strFormat == string.Empty)
                            strFormat = "g";
                        result = DateTime.Parse(prop.PropertyValue).ToString(strFormat, formatProvider);
                        break;
                    case "integer":
                        if (strFormat == string.Empty)
                            strFormat = "g";
                        result = int.Parse(prop.PropertyValue).ToString(strFormat, formatProvider);
                        break;
                    case "page":
                        Entities.Tabs.TabController TabCtrl = new Entities.Tabs.TabController();
                        int tabid;
                        if (int.TryParse(prop.PropertyValue, out tabid))
                        {
                            Entities.Tabs.TabInfo Tab = TabCtrl.GetTab(tabid, Null.NullInteger, false);
                            if (Tab != null)
                            {
                                result = string.Format("<a href='{0}'>{1}</a>", Globals.NavigateURL(tabid), Tab.LocalizedTabName);
                            }
                        }
                        break;
                    case "image":
                        //File is stored as a FileID
                        int fileID;
                        if (Int32.TryParse(prop.PropertyValue, out fileID) && fileID > 0)
                        {
                            result = Globals.LinkClick(String.Format("fileid={0}", fileID), Null.NullInteger, Null.NullInteger);
                        }
                        else
                        {
                            result = Globals.ResolveUrl("~/images/spacer.gif");
                        }
                        break;
                    case "richtext":
                        result = PropertyAccess.FormatString(HttpUtility.HtmlDecode(prop.PropertyValue), strFormat);
                        break;
                    default:
                        result = HttpUtility.HtmlEncode(PropertyAccess.FormatString(prop.PropertyValue, strFormat));
                        break;
                }
            }
            return result;
        }
        private static string DisplayDataType(Entities.Profile.ProfilePropertyDefinition definition)
        {
            string CacheKey = string.Format("DisplayDataType:{0}", definition.DataType);
            string strDataType = Convert.ToString(DataCache.GetCache(CacheKey)) + "";
            if (strDataType == string.Empty)
            {
                Common.Lists.ListController objListController = new Common.Lists.ListController();
                strDataType = objListController.GetListEntryInfo(definition.DataType).Value;
                DataCache.SetCache(CacheKey, strDataType);
            }
            return strDataType;
        }
        private bool CheckAccessLevel(UserVisibilityMode VisibilityMode, Entities.Users.UserInfo AccessingUser)
        {
            if (String.IsNullOrEmpty(strAdministratorRoleName) && !AccessingUser.IsSuperUser)
            {
                PortalInfo ps = new PortalController().GetPortal(objUser.PortalID);
                strAdministratorRoleName = ps.AdministratorRoleName;
            }
            return VisibilityMode == UserVisibilityMode.AllUsers || (VisibilityMode == UserVisibilityMode.MembersOnly && AccessingUser != null && AccessingUser.UserID != -1) || (AccessingUser.IsSuperUser || objUser.UserID == AccessingUser.UserID || AccessingUser.IsInRole(strAdministratorRoleName));
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }
    }
}
