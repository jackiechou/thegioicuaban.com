using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Tokens.PropertyAccess;
using System.Collections;

namespace CommonLibrary.Services.Tokens
{
    public class DictionaryPropertyAccess : IPropertyAccess
    {
        IDictionary NameValueCollection;
        public DictionaryPropertyAccess(IDictionary list)
        {
            NameValueCollection = list;
        }
        public virtual string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            if (NameValueCollection == null)
                return string.Empty;
            object valueObject = NameValueCollection[strPropertyName];
            string OutputFormat = strFormat;
            if (string.IsNullOrEmpty(strFormat))
                OutputFormat = "g";
            if (valueObject != null)
            {
                switch (valueObject.GetType().Name)
                {
                    case "String":
                        return PropertyAccess.PropertyAccess.FormatString(Convert.ToString(valueObject), strFormat);
                    case "Boolean":
                        return (PropertyAccess.PropertyAccess.Boolean2LocalizedYesNo(Convert.ToBoolean(valueObject), formatProvider));
                    case "DateTime":
                    case "Double":
                    case "Single":
                    case "Int32":
                    case "Int64":
                        return (((IFormattable)valueObject).ToString(OutputFormat, formatProvider));
                    default:
                        return PropertyAccess.PropertyAccess.FormatString(valueObject.ToString(), strFormat);
                }
            }
            else
            {
                PropertyNotFound = true;
                return string.Empty;
            }
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }
    }
}
