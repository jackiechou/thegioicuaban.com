using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Tokens.PropertyAccess;
using System.Globalization;
using CommonLibrary.Services.Localization;

namespace CommonLibrary.Services.Tokens
{
    public class CulturePropertyAccess : IPropertyAccess
    {
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            CultureInfo ci = formatProvider;
            if (strPropertyName.ToLower() == CultureDropDownTypes.EnglishName.ToString().ToLowerInvariant())
            {
                return PropertyAccess.PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.EnglishName), strFormat);
            }
            else if (strPropertyName.ToLower() == CultureDropDownTypes.Lcid.ToString().ToLowerInvariant())
            {
                return ci.LCID.ToString();
            }
            else if (strPropertyName.ToLower() == CultureDropDownTypes.Name.ToString().ToLowerInvariant())
            {
                return PropertyAccess.PropertyAccess.FormatString(ci.Name, strFormat);
            }
            else if (strPropertyName.ToLower() == CultureDropDownTypes.NativeName.ToString().ToLowerInvariant())
            {
                return PropertyAccess.PropertyAccess.FormatString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ci.NativeName), strFormat);
            }
            else if (strPropertyName.ToLower() == CultureDropDownTypes.TwoLetterIsoCode.ToString().ToLowerInvariant())
            {
                return PropertyAccess.PropertyAccess.FormatString(ci.TwoLetterISOLanguageName, strFormat);
            }
            else if (strPropertyName.ToLower() == CultureDropDownTypes.ThreeLetterIsoCode.ToString().ToLowerInvariant())
            {
                return PropertyAccess.PropertyAccess.FormatString(ci.ThreeLetterISOLanguageName, strFormat);
            }
            else if (strPropertyName.ToLower() == CultureDropDownTypes.DisplayName.ToString().ToLowerInvariant())
            {
                return PropertyAccess.PropertyAccess.FormatString(ci.DisplayName, strFormat);
            }
            PropertyNotFound = true;
            return string.Empty;
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.fullyCacheable; }
        }
    }
}