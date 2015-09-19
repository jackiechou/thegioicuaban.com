using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Common.Utilities
{
    public class CookieUtils
    {
        private void deletecookie()
        {
            string[] theCookies = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
            foreach (string currentFile in theCookies)
            {
                try
                {

                    System.IO.File.Delete(currentFile);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
        }
    }
}
