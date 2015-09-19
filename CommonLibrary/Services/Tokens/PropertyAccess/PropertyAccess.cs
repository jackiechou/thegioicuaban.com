using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Tokens.PropertyAccess
{
    public class PropertyAccess : IPropertyAccess
    {
        object obj;
        public PropertyAccess(object TokenSource)
        {
            obj = TokenSource;
        }
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            if (obj == null)
                return string.Empty;
            return PropertyAccess.GetObjectProperty(obj, strPropertyName, strFormat, formatProvider, ref PropertyNotFound);
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }
        public static string ContentLocked
        {
            get { return "*******"; }
        }
        public static string Boolean2LocalizedYesNo(bool value, System.Globalization.CultureInfo formatProvider)
        {
            string strValue = Convert.ToString(value ? "Yes" : "No");
            return Localization.Localization.GetString(strValue, null, formatProvider.ToString());
        }
        public static string FormatString(string value, string format)
        {
            if (format.Trim() == string.Empty)
            {
                return value;
            }
            else if (value != string.Empty)
            {
                return string.Format(format, value);
            }
            else
            {
                return string.Empty;
            }
        }
        public static string GetObjectProperty(object objObject, string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, ref bool PropertyNotFound)
        {
            PropertyInfo objProperty = null;
            PropertyNotFound = false;
            if (CBO.GetProperties(objObject.GetType()).TryGetValue(strPropertyName, out objProperty))
            {
                object propValue = objProperty.GetValue(objObject, null);
                Type t = typeof(string);
                if (propValue != null)
                {
                    switch (objProperty.PropertyType.Name)
                    {
                        case "String":
                            return FormatString(Convert.ToString(propValue), strFormat);
                        case "Boolean":
                            return (PropertyAccess.Boolean2LocalizedYesNo(Convert.ToBoolean(propValue), formatProvider));
                        case "DateTime":
                        case "Double":
                        case "Single":
                        case "Int32":
                        case "Int64":
                            if (strFormat == string.Empty)
                                strFormat = "g";
                            return (((IFormattable)propValue).ToString(strFormat, formatProvider));
                    }
                }
                else
                {
                    return "";
                }
            }
            PropertyNotFound = true;
            return string.Empty;
        }
    }
}
