using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CommonLibrary.Entities.Portal;

namespace CommonLibrary.Services.Sitemap
{
    public class SitemapHandler : IHttpHandler
    {

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            try
            {
                HttpResponse Response = context.Response;
                PortalSettings ps = PortalController.GetCurrentPortalSettings();

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                SitemapBuilder builder = new SitemapBuilder(ps);

                if ((context.Request.QueryString["i"] != null))
                {
                    // This is a request for one of the files that build the sitemapindex
                    builder.GetSitemapIndexFile(context.Request.QueryString["i"], Response.Output);
                }
                else
                {
                    builder.BuildSiteMap(Response.Output);
                }

            }
            catch (Exception exc)
            {
                CommonLibrary.Services.Exceptions.Exceptions.LogException(exc);
            }

        }

    }
}
