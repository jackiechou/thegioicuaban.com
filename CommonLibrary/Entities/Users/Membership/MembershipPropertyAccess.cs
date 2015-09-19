using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Tokens.PropertyAccess;
using CommonLibrary.Services.Tokens;
using System.Globalization;

namespace CommonLibrary.Entities.Users.Membership
{
    public class MembershipPropertyAccess : IPropertyAccess
    {
        private readonly UserInfo objUser;        
        
        bool isSecure;
        public MembershipPropertyAccess(UserInfo User)
        {
            objUser = User;
        }
        public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider, 
            Entities.Users.UserInfo AccessingUser, Scope CurrentScope, ref bool PropertyNotFound)
        {

            UserMembership objMembership = new UserMembership(objUser);
            //UserMembership objMembership = objUser.Membership;
            bool UserQueriesHimself = (objUser.UserID == AccessingUser.UserID && objUser.UserID != -1);
            if (CurrentScope < Scope.DefaultSettings || (CurrentScope == Scope.DefaultSettings && !UserQueriesHimself) || ((CurrentScope != Scope.SystemMessages || objUser.IsSuperUser) && strPropertyName.ToLower().StartsWith("password")))
            {
                PropertyNotFound = true;
                return PropertyAccess.ContentLocked;
            }
            else
            {
                string OutputFormat = string.Empty;
                if (strFormat == string.Empty)
                    OutputFormat = "g";
                switch (strPropertyName.ToLower())
                {
                    case "approved":
                        return (PropertyAccess.Boolean2LocalizedYesNo(objMembership.Approved, formatProvider));
                    case "createdondate":
                        return (objMembership.CreatedDate.ToString(OutputFormat, formatProvider));
                    case "isonline":
                        return (PropertyAccess.Boolean2LocalizedYesNo(objMembership.IsOnLine, formatProvider));
                    case "lastactivitydate":
                        return (objMembership.LastActivityDate.ToString(OutputFormat, formatProvider));
                    case "lastlockoutdate":
                        return (objMembership.LastLockoutDate.ToString(OutputFormat, formatProvider));
                    case "lastlogindate":
                        return (objMembership.LastLoginDate.ToString(OutputFormat, formatProvider));
                    case "lastpasswordchangedate":
                        return (objMembership.LastPasswordChangeDate.ToString(OutputFormat, formatProvider));
                    case "lockedout":
                        return (PropertyAccess.Boolean2LocalizedYesNo(objMembership.LockedOut, formatProvider));
                    case "objecthydrated":
                        return (PropertyAccess.Boolean2LocalizedYesNo(true, formatProvider));
                    case "password":
                        return PropertyAccess.FormatString(objMembership.Password, strFormat);
                    case "passwordanswer":
                        return PropertyAccess.FormatString(objMembership.PasswordAnswer, strFormat);
                    case "passwordquestion":
                        return PropertyAccess.FormatString(objMembership.PasswordQuestion, strFormat);
                    case "updatepassword":
                        return (PropertyAccess.Boolean2LocalizedYesNo(objMembership.UpdatePassword, formatProvider));
                    case "username":
                        return (PropertyAccess.FormatString(objUser.Username, strFormat));
                    case "email":
                        return (PropertyAccess.FormatString(objUser.Email, strFormat));
                }
            }
            return PropertyAccess.GetObjectProperty(objMembership, strPropertyName, strFormat, formatProvider, ref PropertyNotFound);
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }
    }
}
