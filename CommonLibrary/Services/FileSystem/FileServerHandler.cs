using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using CommonLibrary.Common;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Common.Utilities;
using System.Globalization;
using CommonLibrary.Entities.Portal;

namespace CommonLibrary.Services.FileSystem
{
    public class FileServerHandler : IHttpHandler
    {
        public FileServerHandler()
        {
        }
        public void ProcessRequest(System.Web.HttpContext context)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            int TabId = -1;
            int ModuleId = -1;
            try
            {
                if (context.Request.QueryString["tabid"] != null)
                {
                    Int32.TryParse(context.Request.QueryString["tabid"], out TabId);
                }
                if (context.Request.QueryString["mid"] != null)
                {
                    Int32.TryParse(context.Request.QueryString["mid"], out ModuleId);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw new HttpException(404, "Not Found");
            }
            string Language = _portalSettings.DefaultLanguage;
            if (context.Request.QueryString["language"] != null)
            {
                Language = context.Request.QueryString["language"];
            }
            else
            {
                if (context.Request.Cookies["language"] != null)
                {
                    Language = context.Request.Cookies["language"].Value;
                }
            }
            if (Localization.Localization.LocaleIsEnabled(Language))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(Language);
                Localization.Localization.SetLanguage(Language);
            }
            string URL = "";
            bool blnClientCache = true;
            bool blnForceDownload = false;
            if (context.Request.QueryString["fileticket"] != null)
            {
                URL = "FileID=" + UrlUtils.DecryptParameter(context.Request.QueryString["fileticket"]);
            }
            if (context.Request.QueryString["userticket"] != null)
            {
                URL = "UserId=" + UrlUtils.DecryptParameter(context.Request.QueryString["userticket"]);
            }
            if (context.Request.QueryString["link"] != null)
            {
                URL = context.Request.QueryString["link"];
                if (URL.ToLowerInvariant().StartsWith("fileid="))
                {
                    URL = "";
                }
            }
            if (!String.IsNullOrEmpty(URL))
            {
                UrlController objUrls = new UrlController();
                objUrls.UpdateUrlTracking(_portalSettings.PortalId, URL, ModuleId, -1);
                TabType UrlType = Globals.GetURLType(URL);
                if (UrlType != TabType.File)
                {
                    URL = Common.Globals.LinkClick(URL, TabId, ModuleId, false);
                }
                if (UrlType == TabType.File && URL.ToLowerInvariant().StartsWith("fileid=") == false)
                {
                    FileController objFiles = new FileController();
                    URL = "FileID=" + objFiles.ConvertFilePathToFileId(URL, _portalSettings.PortalId);
                }
                if (context.Request.QueryString["clientcache"] != null)
                {
                    blnClientCache = bool.Parse(context.Request.QueryString["clientcache"]);
                }
                if ((context.Request.QueryString["forcedownload"] != null) || (context.Request.QueryString["contenttype"] != null))
                {
                    blnForceDownload = bool.Parse(context.Request.QueryString["forcedownload"]);
                }
                context.Response.Clear();
                try
                {
                    switch (UrlType)
                    {
                        case TabType.File:
                            if (TabId == Null.NullInteger)
                            {
                                if (!FileSystemUtils.DownloadFile(_portalSettings.PortalId, int.Parse(UrlUtils.GetParameterValue(URL)), blnClientCache, blnForceDownload))
                                {
                                    throw new HttpException(404, "Not Found");
                                }
                            }
                            else
                            {
                                if (!FileSystemUtils.DownloadFile(_portalSettings, int.Parse(UrlUtils.GetParameterValue(URL)), blnClientCache, blnForceDownload))
                                {
                                    throw new HttpException(404, "Not Found");
                                }
                            }
                            break;
                        case TabType.Url:
                            if (objUrls.GetUrl(_portalSettings.PortalId, URL) != null)
                            {
                                context.Response.Redirect(URL, true);
                            }
                            break;
                        default:
                            context.Response.Redirect(URL, true);
                            break;
                    }
                }
                catch (ThreadAbortException ex)
                {
                    ex.ToString();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    throw new HttpException(404, "Not Found");
                }
            }
            else
            {
                throw new HttpException(404, "Not Found");
            }
        }
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
