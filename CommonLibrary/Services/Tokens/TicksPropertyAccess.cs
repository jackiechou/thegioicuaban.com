using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Tokens.PropertyAccess;

namespace CommonLibrary.Services.Tokens
{
    public class TicksPropertyAccess : IPropertyAccess
    {
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Entities.Users.UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            switch (strPropertyName.ToLower())
            {
                case "now":
                    return DateTime.Now.Ticks.ToString();
                case "today":
                    return DateTime.Today.Ticks.ToString();
                case "ticksperday":
                    return TimeSpan.TicksPerDay.ToString();
            }
            PropertyNotFound = true;
            return string.Empty;
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.secureforCaching; }
        }
    }
}
