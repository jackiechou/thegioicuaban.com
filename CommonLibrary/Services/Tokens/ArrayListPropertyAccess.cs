using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Tokens.PropertyAccess;
using System.Collections;

namespace CommonLibrary.Services.Tokens
{
    public class ArrayListPropertyAccess : IPropertyAccess
    {

        ArrayList custom;

        public ArrayListPropertyAccess(ArrayList list)
        {
            custom = list;
        }

        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            if (custom == null) return string.Empty;
            object valueObject = null;
            string OutputFormat = strFormat;
            if (string.IsNullOrEmpty(strFormat)) OutputFormat = "g";
            int intIndex = int.Parse(strPropertyName);
            if ((custom != null) && custom.Count > intIndex)
            {
                valueObject = custom[intIndex].ToString();
            }
            if ((valueObject != null))
            {
                switch (valueObject.GetType().Name)
                {
                    case "String":
                        return PropertyAccess.PropertyAccess.FormatString((string)valueObject, strFormat);
                    case "Boolean":
                        return (PropertyAccess.PropertyAccess.Boolean2LocalizedYesNo((bool)valueObject, formatProvider));
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
