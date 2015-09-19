using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Host
{
    public class HostPropertyAccess //: DictionaryPropertyAccess
    {
        //public HostPropertyAccess()
        //    : base(Host.GetSecureHostSettingsDictionary())
        //{
        //}
        //public override string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Users.UserInfo AccessingUser, Services.Tokens.Scope CurrentScope, ref bool PropertyNotFound)
        //{
        //    if (strPropertyName.ToLower() == "hosttitle" || CurrentScope == Scope.Debug)
        //    {
        //        return base.GetProperty(strPropertyName, strFormat, formatProvider, AccessingUser, CurrentScope, ref PropertyNotFound);
        //    }
        //    else
        //    {
        //        PropertyNotFound = true;
        //        return PropertyAccess.ContentLocked;
        //    }
        //}
    }
}
