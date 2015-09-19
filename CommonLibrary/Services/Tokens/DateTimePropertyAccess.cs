using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Tokens.PropertyAccess;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Services.Tokens
{
    public class DateTimePropertyAccess : IPropertyAccess
    {
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            DateTime now = UserTime.CurrentTimeForUser(AccessingUser);
            switch (strPropertyName.ToLower())
            {
                case "current":
                    if (strFormat == string.Empty)
                        strFormat = "D";
                    return now.ToString(strFormat, formatProvider);
                case "now":
                    if (strFormat == string.Empty)
                        strFormat = "g";
                    return now.ToString(strFormat, formatProvider);
                default:
                    PropertyNotFound = true;
                    return string.Empty;
            }
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.secureforCaching; }
        }
    }
}
