using System;
using System.Web.UI;
using System.IO;
using System.Reflection;

[assembly: TagPrefix("CommonLibrary.Resources", "ww")]

[assembly: WebResource("CommonLibrary.Resources.jquery.js", "application/x-javascript")]
[assembly: WebResource("CommonLibrary.Resources.ww.jquery.js", "application/x-javascript")]


[assembly: WebResource("CommonLibrary.Resources.warning.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.info.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.loading.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.loading_small.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.close.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.help.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.calendar.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.editresources.png", "image/png")]
[assembly: WebResource("CommonLibrary.Resources.extractresources.png", "image/png")]
[assembly: WebResource("CommonLibrary.Resources.localize.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.refresh.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.recycle.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.rename.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.delete.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.createtable.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.backup.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.import.gif", "image/gif")]
[assembly: WebResource("CommonLibrary.Resources.new.gif", "image/gif")]

namespace CommonLibrary.Resources
{
    /// <summary>
    /// Class is used as to consolidate access to resources
    /// </summary>
    public class ControlResources
    {
        /* Embedded Script Resources */
        public const string JQUERY_SCRIPT_RESOURCE = "CommonLibrary.Resources.jquery.js";
        public const string WWJQUERY_SCRIPT_RESOURCE = "CommonLibrary.Resources.ww.jquery.js";

        /*  Icon Resource Strings */
        public const string INFO_ICON_RESOURCE = "CommonLibrary.Resources.info.gif";
        public const string WARNING_ICON_RESOURCE = "CommonLibrary.Resources.warning.gif";
        public const string CLOSE_ICON_RESOURCE = "CommonLibrary.Resources.close.gif";
        public const string HELP_ICON_RESOURCE = "CommonLibrary.Resources.help.gif";
        public const string LOADING_ICON_RESOURCE = "CommonLibrary.Resources.loading.gif";
        public const string LOADING_SMALL_ICON_RESOURCE = "CommonLibrary.Resources.loading_small.gif";
        public const string CALENDAR_ICON_RESOURCE = "CommonLibrary.Resources.calendar.gif";

        public const string INFO_ICON_EDITRESOURCES = "CommonLibrary.Resources.editresources.png";
        public const string INFO_ICON_EXTRACTRESOURCES = "CommonLibrary.Resources.extractresources.png";
        public const string INFO_ICON_LOCALIZE = "CommonLibrary.Resources.localize.gif";
        public const string INFO_ICON_REFRESH = "CommonLibrary.Resources.refresh.gif";
        public const string INFO_ICON_RECYCLE = "CommonLibrary.Resources.recycle.gif";
        public const string INFO_ICON_DELETE = "CommonLibrary.Resources.delete.gif";
        public const string INFO_ICON_RENAME = "CommonLibrary.Resources.rename.gif";
        public const string INFO_ICON_NEW = "CommonLibrary.Resources.new.gif";
        public const string INFO_ICON_CREATETABLE = "CommonLibrary.Resources.createtable.gif";
        public const string INFO_ICON_IMPORT = "CommonLibrary.Resources.import.gif";
        public const string INFO_ICON_BACKUP = "CommonLibrary.Resources.backup.gif";

        /* Content Types */
        public const string STR_JsonContentType = "application/json";
        public const string STR_JavaScriptContentType = "application/x-javascript";
        public const string STR_UrlEncodedContentType = "application/x-www-form-urlencoded";
        public const string STR_XmlContentType = "text/xml";


        /// <summary>
        /// Returns a string resource from a given assembly.
        /// </summary>
        /// <param name="assembly">Assembly reference (ie. typeof(ControlResources).Assembly) </param>
        /// <param name="ResourceName">Name of the resource to retrieve</param>
        /// <returns></returns>
        public static string GetStringResource(Assembly assembly, string ResourceName)
        {
            Stream st = assembly.GetManifestResourceStream(ResourceName);
            StreamReader sr = new StreamReader(st);
            string content = sr.ReadToEnd();
            st.Close();
            return content;
        }


        /// <summary>
        /// Returns a string resource from the from the ControlResources Assembly
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        public static string GetStringResource(string ResourceName)
        {
            return GetStringResource(typeof(ControlResources).Assembly, ResourceName);
        }
    }
}
