using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CommonLibrary.Common.Utilities
{
    public static class SystemUtils
    {
        /// <summary>
        /// Returns the logon password stored in the registry if Auto-Logon is used.
        /// This function is used privately for demos when I need to specify a login username and password.
        /// </summary>
        /// <param name="getUserName"></param>
        /// <returns></returns>
        public static string GetSystemPassword(bool getUserName)
        {
            var RegKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            if (RegKey == null)
                return string.Empty;

            string Password;
            if (!getUserName)
                Password = (string)RegKey.GetValue("DefaultPassword");
            else
                Password = (string)RegKey.GetValue("DefaultUsername");

            if (Password == null)
                return string.Empty;

            return (string)Password;
        }
    }
}
