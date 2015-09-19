using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

         
            routes.MapPageRoute("news_index", "trang-chu", "~/portals/news/index.aspx", false);
            routes.MapPageRoute("news_content", "tin-tuc/{alias}/{code}/{menuid}/{lang}", "~/portals/news/new_categories.aspx", false, new System.Web.Routing.RouteValueDictionary { { "alias", "" }, { "code", "" }, { "menuid", "0" }, { "lang", "vi-VN" } });
            routes.MapPageRoute("news_content_details", "tin-chi-tiet/{alias}/{code}/{menuid}/{lang}", "~/portals/news/details.aspx", false, new System.Web.Routing.RouteValueDictionary { { "alias", "" }, { "code", "" }, { "menuid", "0" }, { "lang", "vi-VN" } });
            routes.MapPageRoute("news_gallery", "bo-suu-tap/{alias}/{topicid}/{menuid}/{lang}", "~/portals/news/gallery.aspx", false, new System.Web.Routing.RouteValueDictionary { { "alias", "" }, { "topicid", "1" }, { "menuid", "0" }, { "lang", "vi-VN" } });
            routes.MapPageRoute("news_gallery_details", "bo-suu-tap-chi-tiet/{collectionid}", "~/portals/news/gallery_details.aspx", false, new System.Web.Routing.RouteValueDictionary { { "collectionid", "1" } });
            routes.MapPageRoute("news_contact", "lien-he", "~/portals/news/contact.aspx", false);


            routes.MapPageRoute("admin_dangnhap", "dangnhap", "~/login.aspx");
            routes.MapPageRoute("admin_login", "login", "~/login.aspx");
            routes.MapPageRoute("admin_index_tabid", "index/{tabid}", "~/index.aspx", true,
                new RouteValueDictionary { { "tabid", string.Empty } });
        }
    }
}