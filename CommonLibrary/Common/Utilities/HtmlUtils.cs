using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using CommonLibrary.Services.Upgrade;

namespace CommonLibrary.Common.Utilities
{
    public class HtmlUtils
    {
        private static Regex HtmlDetectionRegex = new Regex("^\\s*<\\w*(.*\\s*)*\\w?>\\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static string Clean(string HTML, bool RemovePunctuation)
        {
            HTML = StripTags(HTML, true);
            HTML = System.Web.HttpUtility.HtmlDecode(HTML);
            if (RemovePunctuation)
            {
                HTML = StripPunctuation(HTML, true);
            }
            HTML = StripWhiteSpace(HTML, true);
            return HTML;
        }
        public static string FormatEmail(string Email)
        {
            return FormatEmail(Email, true);
        }
        public static string FormatEmail(string Email, bool cloak)
        {
            string formatEmail = "";
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Email.Trim()))
            {
                if (Email.IndexOf("@") != -1)
                {
                    formatEmail = "<a href=\"mailto:" + Email + "\">" + Email + "</a>";
                }
                else
                {
                    formatEmail = Email;
                }
            }
            if (cloak)
            {
                formatEmail = Globals.CloakText(formatEmail);
            }
            return formatEmail;
        }
        public static string FormatText(string HTML, bool RetainSpace)
        {
            string brMatch = "\\s*<\\s*[bB][rR]\\s*/\\s*>\\s*";
            string RepString=string.Empty;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            return System.Text.RegularExpressions.Regex.Replace(HTML, brMatch, Environment.NewLine);
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Formats String as Html by replacing linefeeds by <br/>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="strText">Text to format</param>
        /// <returns>The formatted html</returns>
        /// <history>
        /// [cnurse]	12/13/2004	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string ConvertToHtml(string strText)
        {

            string strHtml = strText;

            if (!string.IsNullOrEmpty(strHtml))
            {
                strHtml = strHtml.Replace("\n", "");
                strHtml = strHtml.Replace("\r", "<br />");
            }

            return strHtml;
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Formats Html as text by removing <br/> tags and replacing by linefeeds
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="strHtml">Html to format</param>
        /// <returns>The formatted text</returns>
        /// <history>
        /// [cnurse]	12/13/2004	Documented and modified to use HtmlUtils methods
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string ConvertToText(string strHtml)
        {

            string strText = strHtml;

            if (!string.IsNullOrEmpty(strText))
            {
                //First remove white space (html does not render white space anyway and it screws up the conversion to text)
                //Replace it by a single space
                strText = HtmlUtils.StripWhiteSpace(strText, true);

                //Replace all variants of <br> by Linefeeds
                strText = HtmlUtils.FormatText(strText, false);
            }



            return strText;
        }
        public static string FormatWebsite(object Website)
        {
            string formatWebsite = "";
            if (Website != DBNull.Value)
            {
                if (!String.IsNullOrEmpty(Website.ToString().Trim()))
                {
                    if (Website.ToString().IndexOf(".") > -1)
                    {
                        formatWebsite = "<a href=\"" + (Website.ToString().IndexOf("://") > -1 ? "" : "http://") + Website.ToString() + "\">" + Website.ToString() + "</a>";
                    }
                    else
                    {
                        formatWebsite = Website.ToString();
                    }
                }
            }
            return formatWebsite;
        }
        public static string Shorten(string txt, int length, string suffix)
        {
            string results;
            if (txt.Length > length)
            {
                results = txt.Substring(0, length) + suffix;
            }
            else
            {
                results = txt;
            }
            return results;
        }
        [Obsolete("This method has been deprecated. Please use System.Web.HtmlUtility.HtmlDecode")]
        public static string StripEntities(string HTML, bool RetainSpace)
        {
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            return System.Text.RegularExpressions.Regex.Replace(HTML, "&[^;]*;", RepString);
        }
        public static string StripTags(string HTML, bool RetainSpace)
        {
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            return System.Text.RegularExpressions.Regex.Replace(HTML, "<[^>]*>", RepString);
        }
        public static string StripPunctuation(string HTML, bool RetainSpace)
        {
            string punctuationMatch = "[~!#\\$%\\^&*\\(\\)-+=\\{\\[\\}\\]\\|;:\\x22'<,>\\.\\?\\\\\\t\\r\\v\\f\\n]";
            System.Text.RegularExpressions.Regex afterRegEx = new System.Text.RegularExpressions.Regex(punctuationMatch + "\\s");
            System.Text.RegularExpressions.Regex beforeRegEx = new System.Text.RegularExpressions.Regex("\\s" + punctuationMatch);
            string retHTML = HTML + " ";
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            while (beforeRegEx.IsMatch(retHTML))
            {
                retHTML = beforeRegEx.Replace(retHTML, RepString);
            }
            while (afterRegEx.IsMatch(retHTML))
            {
                retHTML = afterRegEx.Replace(retHTML, RepString);
            }
            return retHTML;
        }
        public static string StripWhiteSpace(string HTML, bool RetainSpace)
        {
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            if (HTML == Null.NullString)
            {
                return Null.NullString;
            }
            else
            {
                return System.Text.RegularExpressions.Regex.Replace(HTML, "\\s+", RepString);
            }
        }
        public static string StripNonWord(string HTML, bool RetainSpace)
        {
            string RepString;
            if (RetainSpace)
            {
                RepString = " ";
            }
            else
            {
                RepString = "";
            }
            if (HTML == null)
            {
                return HTML;
            }
            else
            {
                return System.Text.RegularExpressions.Regex.Replace(HTML, "\\W*", RepString);
            }
        }

        /// <summary>
        /// Determines wether or not the passed in string contains any HTML tags
        /// </summary>
        /// <param name="text">Text to be inspected</param>
        /// <returns>True for HTML and False for plain text</returns>
        /// <remarks></remarks>
        public static bool IsHtml(string text)
        {

            if ((string.IsNullOrEmpty(text)))
            {
                return false;
            }

            return HtmlDetectionRegex.IsMatch(text);
        }

        public static string FixHtmlCode(string html)
        {
            html = html.Replace("  ", "");
            html = html.Replace("  ", "");
            html = html.Replace("\t", "");
            html = html.Replace("[", "");
            html = html.Replace("]", "");
            html = html.Replace("<", "");
            html = html.Replace(">", "");
            html = html.Replace("\r\n", "");
            return html;
        }


        public static void WriteError(HttpResponse response, string file, string message)
        {
            response.Write("<h2>Error Details</h2>");
            response.Write("<table cellspacing=0 cellpadding=0 border=0>");
            response.Write("<tr><td><b>File</b></td><td><b>" + file + "</b></td></tr>");
            response.Write("<tr><td><b>Error</b>&nbsp;&nbsp;</td><td><b>" + message + "</b></td></tr>");
            response.Write("</table>");
            response.Write("<br><br>");
            response.Flush();
        }
        public static void WriteFeedback(HttpResponse response, Int32 indent, string message)
        {
            WriteFeedback(response, indent, message, true);
        }
        public static void WriteFeedback(HttpResponse response, Int32 indent, string message, bool showtime)
        {
            bool showInstallationMessages = true;
            string ConfigSetting = Config.GetSetting("ShowInstallationMessages");
            if (ConfigSetting != null)
            {
                showInstallationMessages = bool.Parse(ConfigSetting);
            }
            if (showInstallationMessages)
            {
                TimeSpan timeElapsed = Upgrade.RunTime;
                string strMessage = "";
                if (showtime == true)
                {
                    strMessage += timeElapsed.ToString().Substring(0, timeElapsed.ToString().LastIndexOf(".") + 4) + " -";
                }
                for (int i = 0; i <= indent; i++)
                {
                    strMessage += "&nbsp;";
                }
                strMessage += message;
                response.Write(strMessage);
                response.Flush();
            }
        }
        public static void WriteFooter(HttpResponse response)
        {
            response.Write("</body>");
            response.Write("</html>");
            response.Flush();
        }
        public static void WriteHeader(HttpResponse response, string mode)
        {
            response.Buffer = false;
            if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm")))
            {
                if (File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.template.htm")))
                {
                    File.Copy(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.template.htm"), System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm"));
                }
            }
            if (File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm")))
            {
                response.Write(FileSystemUtils.ReadFile(System.Web.HttpContext.Current.Server.MapPath("~/Install/Install.htm")));
            }
            switch (mode)
            {
                case "install":
                    response.Write("<h1>Installing DotNetNuke</h1>");
                    break;
                case "upgrade":
                    response.Write("<h1>Upgrading DotNetNuke</h1>");
                    break;
                case "addPortal":
                    response.Write("<h1>Adding New Portal</h1>");
                    break;
                case "installResources":
                    response.Write("<h1>Installing Resources</h1>");
                    break;
                case "executeScripts":
                    response.Write("<h1>Executing Scripts</h1>");
                    break;
                case "none":
                    response.Write("<h1>Nothing To Install At This Time</h1>");
                    break;
                case "noDBVersion":
                    response.Write("<h1>New DotNetNuke Database</h1>");
                    break;
                case "error":
                    response.Write("<h1>Error Installing DotNetNuke</h1>");
                    break;
                default:
                    response.Write("<h1>" + mode + "</h1>");
                    break;
            }
            response.Flush();
        }
        public static void WriteSuccessError(HttpResponse response, bool bSuccess)
        {
            if (bSuccess)
            {
                WriteFeedback(response, 0, "<font color='green'>Success</font><br>", false);
            }
            else
            {
                WriteFeedback(response, 0, "<font color='red'>Error!</font><br>", false);
            }
        }
        public static void WriteScriptSuccessError(HttpResponse response, bool bSuccess, string strLogFile)
        {
            if (bSuccess)
            {
                WriteFeedback(response, 0, "<font color='green'>Success</font><br>", false);
            }
            else
            {
                WriteFeedback(response, 0, "<font color='red'>Error! (see " + strLogFile + " for more information)</font><br>", false);
            }
        }
    }
}
