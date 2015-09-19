using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Tokens.PropertyAccess;

namespace CommonLibrary.Services.Tokens
{
    public class EmptyPropertyAccess : IPropertyAccess
    {
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            return string.Empty;
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }
    }
}
