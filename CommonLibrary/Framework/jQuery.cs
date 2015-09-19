using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using CommonLibrary.Common;
using CommonLibrary.Services.Localization;
using System.Text.RegularExpressions;
using CommonLibrary.Common.Utilities;
using System.IO;
using CommonLibrary.Entities.Host;
using CommonLibrary.Services.Exceptions;

namespace CommonLibrary.Framework
{
    public class jQuery
    {
        private const string jQueryDebugFile = "~/Resources/Shared/Scripts/jquery/jquery.js";
        private const string jQueryMinFile = "~/Resources/Shared/Scripts/jquery/jquery.min.js";
        private const string jQueryVersionKey = "jQueryVersionKey";
        private const string jQueryVersionMatch = "(?<=jquery:\\s\")(.*)(?=\")";
        public const string DefaultHostedUrl = "http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js";
        private static bool GetSettingAsBoolean(string key, bool defaultValue)
        {
            bool retValue = defaultValue;
            try
            {
                object setting = HttpContext.Current.Items[key];
                if (setting != null)
                {
                    retValue = Convert.ToBoolean(setting);
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            return retValue;
        }
        private static bool IsScriptRegistered()
        {
            return HttpContext.Current.Items["jquery_registered"] != null;
        }
        public static bool IsInstalled
        {
            get
            {
                string minFile = JQueryFileMapPath(true);
                string dbgFile = JQueryFileMapPath(false);
                return File.Exists(minFile) || File.Exists(dbgFile);
            }
        }
        public static bool IsRequested
        {
            get { return GetSettingAsBoolean("jQueryRequested", false); }
        }
        public static bool UseDebugScript
        {
            get { return Host.jQueryDebug; }
        }
        public static bool UseHostedScript
        {
            get { return Host.jQueryHosted; }
        }
        public static string HostedUrl
        {
            get { return Host.jQueryUrl; }
        }
        public static string Version
        {
            get
            {
                string ver = Convert.ToString(DataCache.GetCache(jQueryVersionKey));
                if (string.IsNullOrEmpty(ver))
                {
                    if (IsInstalled)
                    {
                        string jqueryFileName = JQueryFileMapPath(false);
                        string jfiletext = File.ReadAllText(jqueryFileName);
                        Match verMatch = Regex.Match(jfiletext, jQueryVersionMatch);
                        if (verMatch != null)
                        {
                            ver = verMatch.Value;
                            //DataCache.SetCache(jQueryVersionKey, ver, new System.Web.Caching.CacheDependency(jqueryFileName));
                        }
                        else
                        {
                            ver = Localization.GetString("jQuery.UnknownVersion.Text");
                        }
                    }
                    else
                    {
                        ver = Localization.GetString("jQuery.NotInstalled.Text");
                    }
                }
                return ver;
            }
        }
        public static string JQueryFileMapPath(bool GetMinFile)
        {
            return HttpContext.Current.Server.MapPath(JQueryFile(GetMinFile));
        }
        public static string JQueryFile(bool GetMinFile)
        {
            string jfile = jQueryDebugFile;
            if (GetMinFile)
            {
                jfile = jQueryMinFile;
            }
            return Globals.ResolveUrl(jfile);
        }
        public static string GetJQueryScriptReference()
        {
            string scriptsrc = HostedUrl;
            if (!UseHostedScript)
            {
                scriptsrc = JQueryFile(!UseDebugScript);
            }
            return string.Format(Globals.glbScriptFormat, scriptsrc);
        }
        public static void RegisterScript(System.Web.UI.Page page)
        {
            RegisterScript(page, jQuery.GetJQueryScriptReference());
        }
        public static void RegisterScript(System.Web.UI.Page page, string script)
        {
            if (!IsScriptRegistered())
            {
                HttpContext.Current.Items["jquery_registered"] = true;
                Literal headscript = new Literal();
                headscript.Text = script;
                Control objCSS = page.Header.FindControl("SCRIPTS");
                objCSS.Controls.AddAt(0, headscript);
            }
        }
        public static void RequestRegistration()
        {
            HttpContext.Current.Items["jQueryRequested"] = true;
        }
       
    }
}
