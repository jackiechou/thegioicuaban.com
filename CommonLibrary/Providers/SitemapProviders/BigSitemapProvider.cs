using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Sitemap;

namespace CommonLibrary.Providers.SitemapProviders
{
    public class BigSitemapProvider : SitemapProvider
    {
        public override System.Collections.Generic.List<SitemapUrl> GetUrls(int portalId, CommonLibrary.Entities.Portal.PortalSettings ps, string version)
        {
            List<SitemapUrl> urls = new List<SitemapUrl>();
            for (int i = 0; i <= 50; i++)
            {
                urls.Add(GetPageUrl(i));
            }

            return urls;
        }
        

        private SitemapUrl GetPageUrl(int index)
        {
            SitemapUrl pageUrl = new SitemapUrl();
            pageUrl.Url = string.Format("http://mysite/page_{0}.aspx", index);
            pageUrl.Priority = 0.5F;
            pageUrl.LastModified = DateTime.Now;
            pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily;

            return pageUrl;
        }
    }
}
