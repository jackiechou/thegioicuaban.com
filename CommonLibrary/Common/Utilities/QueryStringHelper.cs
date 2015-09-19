using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CommonLibrary.Common.Utilities
{
    public static class QueryStringHelper
    {

        private static string GetStringValue(string strInput)
        {
            string value = HttpContext.Current.Request.QueryString[strInput];

            if (value == null)
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        private static int? GetIntValue(string strInput)
        {
            string value = HttpContext.Current.Request.QueryString[strInput];

            if (value == null)
            {
                return null;
            }
            else
            {
                return int.Parse(value); ;
            }
        }

        private static bool HasValue(string strInput)
        {
            return HttpContext.Current.Request.QueryString[strInput] != null;
        }

    }
}
