using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Services.Tokens.PropertyAccess
{
    public interface IPropertyAccess
    {
        string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound);
        CacheLevel Cacheability { get; }
    }
}
