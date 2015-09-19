using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CommonLibrary.Common.Utilities;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Security
{
    public class PortalSecurity
    {
        [FlagsAttribute()]
        public enum FilterFlag
        {
            MultiLine = 1,
            NoMarkup = 2,
            NoScripting = 4,
            NoSQL = 8,
            NoAngleBrackets = 16
        }
        private string FilterStrings(string strInput)
        {
            string TempInput = strInput;
            List<string> listStrings = new List<string>();
            listStrings.Add("<script[^>]*>.*?</script[^><]*>");
            listStrings.Add("<script");
            listStrings.Add("<input[^>]*>.*?</input[^><]*>");
            listStrings.Add("<object[^>]*>.*?</object[^><]*>");
            listStrings.Add("<embed[^>]*>.*?</embed[^><]*>");
            listStrings.Add("<applet[^>]*>.*?</applet[^><]*>");
            listStrings.Add("<form[^>]*>.*?</form[^><]*>");
            listStrings.Add("<option[^>]*>.*?</option[^><]*>");
            listStrings.Add("<select[^>]*>.*?</select[^><]*>");
            listStrings.Add("<iframe[^>]*>.*?</iframe[^><]*>");
            listStrings.Add("<iframe.*?<");
            listStrings.Add("<iframe.*?");
            listStrings.Add("<ilayer[^>]*>.*?</ilayer[^><]*>");
            listStrings.Add("<form[^>]*>");
            listStrings.Add("</form[^><]*>");
            listStrings.Add("javascript:");
            listStrings.Add("vbscript:");
            listStrings.Add("alert[\\s(&nbsp;)]*\\([\\s(&nbsp;)]*'?[\\s(&nbsp;)]*[\"(&quot;)]?");
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            string strReplacement = " ";
            if (TempInput.Contains("&gt;") == true && TempInput.Contains("&lt;") == true)
            {
                TempInput = HttpContext.Current.Server.HtmlDecode(TempInput);
                foreach (string s in listStrings)
                {
                    TempInput = Regex.Replace(TempInput, s, strReplacement, options);
                }
                TempInput = HttpContext.Current.Server.HtmlEncode(TempInput);
            }
            else
            {
                foreach (string s in listStrings)
                {
                    TempInput = Regex.Replace(TempInput, s, strReplacement, options);
                }
            }
            return TempInput;
        }
        private string FormatDisableScripting(string strInput)
        {
            string TempInput = strInput;
            TempInput = FilterStrings(TempInput);
            return TempInput;
        }
        private string FormatAngleBrackets(string strInput)
        {
            string TempInput = strInput.Replace("<", "");
            TempInput = TempInput.Replace(">", "");
            return TempInput;
        }
        private string FormatMultiLine(string strInput)
        {
            string TempInput = strInput.Replace(Environment.NewLine, "<br>");
            return TempInput.Replace("\r", "<br>");
        }
        private string FormatRemoveSQL(string strSQL)
        {
            const string BadStatementExpression = ";|--|create|drop|select|insert|delete|update|union|sp_|xp_|exec|/\\*.*\\*/|declare";
            return Regex.Replace(strSQL, BadStatementExpression, " ", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace("'", "''");
        }
        private bool IncludesMarkup(string strInput)
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            string strPattern = "<[^<>]*>";
            return Regex.IsMatch(strInput, strPattern, options);
        }
        private string BytesToHexString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(64);
            int counter;
            for (counter = 0; counter <= bytes.Length - 1; counter++)
            {
                hexString.Append(String.Format("{0:X2}", bytes[counter]));
            }
            return hexString.ToString();
        }
        public string CreateKey(int numBytes)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[numBytes];
            rng.GetBytes(buff);
            return BytesToHexString(buff);
        }
        public string Decrypt(string strKey, string strData)
        {
            if (String.IsNullOrEmpty(strData))
            {
                return "";
            }
            string strValue = "";
            if (!String.IsNullOrEmpty(strKey))
            {
                if (strKey.Length < 16)
                {
                    strKey = strKey + "XXXXXXXXXXXXXXXX".Substring(0, 16 - strKey.Length);
                }
                else
                {
                    strKey = strKey.Substring(0, 16);
                }
                byte[] byteKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(strKey.Substring(strKey.Length - 8, 8));
                byte[] byteData = new byte[strData.Length];
                try
                {
                    byteData = Convert.FromBase64String(strData);
                }
                catch
                {
                    strValue = strData;
                }
                if (String.IsNullOrEmpty(strValue))
                {
                    try
                    {
                        DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                        MemoryStream objMemoryStream = new MemoryStream();
                        CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateDecryptor(byteKey, byteVector), CryptoStreamMode.Write);
                        objCryptoStream.Write(byteData, 0, byteData.Length);
                        objCryptoStream.FlushFinalBlock();
                        System.Text.Encoding objEncoding = System.Text.Encoding.UTF8;
                        strValue = objEncoding.GetString(objMemoryStream.ToArray());
                    }
                    catch
                    {
                        strValue = "";
                    }
                }
            }
            else
            {
                strValue = strData;
            }
            return strValue;
        }
        public string Encrypt(string strKey, string strData)
        {
            string strValue = "";
            if (!String.IsNullOrEmpty(strKey))
            {
                if (strKey.Length < 16)
                {
                    strKey = strKey + "XXXXXXXXXXXXXXXX".Substring(0, 16 - strKey.Length);
                }
                else
                {
                    strKey = strKey.Substring(0, 16);
                }
                byte[] byteKey = Encoding.UTF8.GetBytes(strKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(strKey.Substring(strKey.Length - 8, 8));
                byte[] byteData = Encoding.UTF8.GetBytes(strData);
                DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                MemoryStream objMemoryStream = new MemoryStream();
                CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateEncryptor(byteKey, byteVector), CryptoStreamMode.Write);
                objCryptoStream.Write(byteData, 0, byteData.Length);
                objCryptoStream.FlushFinalBlock();
                strValue = Convert.ToBase64String(objMemoryStream.ToArray());
            }
            else
            {
                strValue = strData;
            }
            return strValue;
        }
        public string InputFilter(string UserInput, FilterFlag FilterType)
        {
            if (UserInput == null)
                return "";
            string TempInput = UserInput;
            if ((FilterType & FilterFlag.NoAngleBrackets) == FilterFlag.NoAngleBrackets)
            {
                bool RemoveAngleBrackets;
                if (Config.GetSetting("RemoveAngleBrackets") == null)
                {
                    RemoveAngleBrackets = false;
                }
                else
                {
                    RemoveAngleBrackets = bool.Parse(Config.GetSetting("RemoveAngleBrackets"));
                }
                if (RemoveAngleBrackets == true)
                {
                    TempInput = FormatAngleBrackets(TempInput);
                }
            }
            if ((FilterType & FilterFlag.NoSQL) == FilterFlag.NoSQL)
            {
                TempInput = FormatRemoveSQL(TempInput);
            }
            else
            {
                if ((FilterType & FilterFlag.NoMarkup) == FilterFlag.NoMarkup && IncludesMarkup(TempInput))
                {
                    TempInput = HttpUtility.HtmlEncode(TempInput);
                }
                if ((FilterType & FilterFlag.NoScripting) == FilterFlag.NoScripting)
                {
                    TempInput = FormatDisableScripting(TempInput);
                }
                if ((FilterType & FilterFlag.MultiLine) == FilterFlag.MultiLine)
                {
                    TempInput = FormatMultiLine(TempInput);
                }
            }
            return TempInput;
        }
        public void SignOut()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            HttpContext.Current.Response.Cookies["language"].Value = "";
            HttpContext.Current.Response.Cookies["authentication"].Value = "";
            HttpContext.Current.Response.Cookies["portalaliasid"].Value = null;
            HttpContext.Current.Response.Cookies["portalaliasid"].Path = "/";
            HttpContext.Current.Response.Cookies["portalaliasid"].Expires = DateTime.Now.AddYears(-30);
            HttpContext.Current.Response.Cookies["portalroles"].Value = null;
            HttpContext.Current.Response.Cookies["portalroles"].Path = "/";
            HttpContext.Current.Response.Cookies["portalroles"].Expires = DateTime.Now.AddYears(-30);
        }
        public static void ClearRoles()
        {
            HttpContext.Current.Response.Cookies["portalroles"].Value = null;
            HttpContext.Current.Response.Cookies["portalroles"].Path = "/";
            HttpContext.Current.Response.Cookies["portalroles"].Expires = DateTime.Now.AddYears(-30);
        }
        public static void ForceSecureConnection()
        {
            string URL = HttpContext.Current.Request.Url.ToString();
            if (URL.StartsWith("http://"))
            {
                URL = URL.Replace("http://", "https://");
                if (URL.IndexOf("?") == -1)
                {
                    URL = URL + "?ssl=1";
                }
                else
                {
                    URL = URL + "&ssl=1";
                }
                HttpContext.Current.Response.Redirect(URL, true);
            }
        }
        public static bool IsInRole(string role)
        {
            UserInfo objUserInfo = UserController.GetCurrentUserInfo();
            HttpContext context = HttpContext.Current;
            if ((!String.IsNullOrEmpty(role) && role != null && ((context.Request.IsAuthenticated == false && role == Globals.glbRoleUnauthUserName))))
            {
                return true;
            }
            else
            {
                return objUserInfo.IsInRole(role);
            }
        }
        public static bool IsInRoles(string roles)
        {
            UserInfo objUserInfo = UserController.GetCurrentUserInfo();
            bool blnIsInRoles = objUserInfo.IsSuperUser;
            if (!blnIsInRoles)
            {
                if (roles != null)
                {
                    HttpContext context = HttpContext.Current;
                    foreach (string role in roles.Split(new char[] { ';' }))
                    {
                        if (!string.IsNullOrEmpty(role))
                        {
                            if (role.StartsWith("!"))
                            {
                                PortalSettings settings = PortalController.GetCurrentPortalSettings();
                                if (!(settings.PortalId == objUserInfo.PortalID && settings.AdministratorId == objUserInfo.UserID))
                                {
                                    string denyRole = role.Replace("!", "");
                                    if (((context.Request.IsAuthenticated == false && denyRole == Globals.glbRoleUnauthUserName) || denyRole == Globals.glbRoleAllUsersName || objUserInfo.IsInRole(denyRole)))
                                    {
                                        blnIsInRoles = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (((context.Request.IsAuthenticated == false && role == Globals.glbRoleUnauthUserName) || role == Globals.glbRoleAllUsersName || objUserInfo.IsInRole(role)))
                                {
                                    blnIsInRoles = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return blnIsInRoles;
        }
    }
}
