using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Sitemap;
using CommonLibrary.Common;
using System.Web;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Providers.SitemapProviders
{
    public class CoreSitemapProvider : CommonLibrary.Services.Sitemap.SitemapProvider
    {

        private bool useLevelBasedPagePriority;
        private float minPagePriority;
        private bool includeHiddenPages;

        private PortalSettings ps;
        /// <summary>
        /// Includes page urls on the sitemap
        /// </summary>
        /// <remarks>
        /// Pages that are included:
        /// - are not deleted
        /// - are not disabled
        /// - are normal pages (not links,...)
        /// - are visible (based on date and permissions)
        /// </remarks>

        public override List<SitemapUrl> GetUrls(int portalId, PortalSettings ps, string version)
        {
            TabController objTabs = new TabController();
            SitemapUrl pageUrl = null;
            List<SitemapUrl> urls = new List<SitemapUrl>();

            useLevelBasedPagePriority = bool.Parse(PortalController.GetPortalSetting("SitemapLevelMode", portalId, "False"));
            minPagePriority = float.Parse(PortalController.GetPortalSetting("SitemapMinPriority", portalId, "0.1"));
            includeHiddenPages = bool.Parse(PortalController.GetPortalSetting("SitemapIncludeHidden", portalId, "True"));

            this.ps = ps;

            foreach (TabInfo objTab in objTabs.GetTabsByPortal(portalId).Values)
            {

                if (!objTab.IsDeleted && !objTab.DisableLink && objTab.TabType == TabType.Normal && (Null.IsNull(objTab.StartDate) || objTab.StartDate < DateTime.Now) && (Null.IsNull(objTab.EndDate) || objTab.EndDate > DateTime.Now) && IsTabPublic(objTab.TabPermissions))
                {
                    if (includeHiddenPages | objTab.IsVisible)
                    {
                        foreach (string languageCode in CommonLibrary.Services.Localization.Localization.GetLocales(portalId).Keys)
                        {
                            pageUrl = GetPageUrl(objTab, languageCode);
                            urls.Add(pageUrl);
                        }
                    }
                }
            }

            return urls;
        }

        /// <summary>
        /// Return the sitemap url node for the page
        /// </summary>
        /// <param name="objTab">The page being indexed</param>
        /// <returns>A SitemapUrl object for the current page</returns>
        /// <remarks></remarks>

        private SitemapUrl GetPageUrl(TabInfo objTab, string language)
        {
            SitemapUrl pageUrl = new SitemapUrl();
            pageUrl.Url = CommonLibrary.Common.Globals.NavigateURL(objTab.TabID, objTab.IsSuperTab, ps, "", language);

            if (pageUrl.Url.ToLower().IndexOf(ps.PortalAlias.HTTPAlias.ToLower()) == -1)
            {
                // code to fix a bug in dnn5.1.2 for navigateurl
                if ((HttpContext.Current != null))
                {
                    pageUrl.Url = Globals.AddHTTP(HttpContext.Current.Request.Url.Host + pageUrl.Url);
                }
                else
                {
                    // try to use the portalalias
                    pageUrl.Url = Globals.AddHTTP(ps.PortalAlias.HTTPAlias.ToLower()) + pageUrl.Url;
                }
            }
            pageUrl.Priority = GetPriority(objTab);
            pageUrl.LastModified = DateTime.Now;
            pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily;

            return pageUrl;
        }

        /// <summary>
        /// When page level priority is used, the priority for each page will be computed from 
        /// the hierarchy level of the page. 
        /// Top level pages will have a value of 1, second level 0.9, third level 0.8, ... 
        /// </summary>
        /// <param name="objTab">The page being indexed</param>
        /// <returns>The priority assigned to the page</returns>
        /// <remarks></remarks>

        protected float GetPriority(TabInfo objTab)
        {
            float priority = objTab.SiteMapPriority;

            if (useLevelBasedPagePriority)
            {
                if (objTab.Level >= 9)
                {
                    priority = 0.1F;
                }
                else
                {
                    priority = Convert.ToSingle(1 - (objTab.Level * 0.1));
                }

                if (priority < minPagePriority)
                {
                    priority = minPagePriority;
                }
            }

            return priority;
        }

        #region "Security Check"
        public virtual bool IsTabPublic(CommonLibrary.Security.Permissions.TabPermissionCollection objTabPermissions)
        {
            string roles = objTabPermissions.ToString("VIEW");
            bool hasPublicRole = false;


            if ((roles != null))
            {
                // permissions strings are encoded with Deny permissions at the beginning and Grant permissions at the end for optimal performance
                foreach (string role in roles.Split(new char[] { ';' }))
                {
                    if (!string.IsNullOrEmpty(role))
                    {
                        // Deny permission
                        if (role.StartsWith("!"))
                        {
                            string denyRole = role.Replace("!", "");
                            if ((denyRole == Globals.glbRoleUnauthUserName || denyRole == Globals.glbRoleAllUsersName))
                            {
                                hasPublicRole = false;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                            // Grant permission
                        }
                        else
                        {
                            if ((role == Globals.glbRoleUnauthUserName || role == Globals.glbRoleAllUsersName))
                            {
                                hasPublicRole = true;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                }

            }

            return hasPublicRole;
        }
        #endregion

    }
}
