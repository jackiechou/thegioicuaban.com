using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CommonLibrary.Common
{
    public static class Guard
    {
        public static void Against(bool condition, string message, params object[] args)
        {
            Against(condition, string.Format(CultureInfo.CurrentUICulture, message, args));
        }

        public static void Against(bool condition, string message)
        {
            if ((condition))
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
