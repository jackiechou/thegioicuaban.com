using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Modules;
using CommonLibrary.Common;
using System.IO;
using CommonLibrary.Entities.Tabs;
using System.Data;
using CommonLibrary.Services.Url.FriendlyUrl;

namespace WebApp.modules.admin.tabs
{
    public partial class admin_front_tab_router_add : System.Web.UI.Page
    {
        RouteController route_obj = new RouteController();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string selectedContent = Request.QueryString["selectedContent"].ToString();
                string selectedItemId = Request.QueryString["selectedItemId"].ToString();
                string selectedTabId = Request.QueryString["selectedTabId"].ToString();
                string contentItemKey = Request.QueryString["contentItemKey"].ToString();
                string keyWords = Request.QueryString["keyWords"].ToString();
                string createdTabId = Request.QueryString["createdTabId"].ToString();
                string createdTabPath = Request.QueryString["createdTabPath"].ToString();
                string portalId = Request.QueryString["portalId"].ToString();
                string lang = Request.QueryString["lang"].ToString();
                string routerid = Request.QueryString["router"].ToString();

                var data_route = route_obj.GetDetails(int.Parse(routerid));
                string routerUrl = data_route.RouteUrl;
                int countparam = data_route.RouteValueDictionary.Split(',').Length ;
                string url = string.Empty;
                int slash = -1;
                if (routerUrl.IndexOf("{", 0) > -1)
                {
                    slash = routerUrl.IndexOf("{", 0) - 1;
                    url = routerUrl.Substring(0, slash);
                }
                else
                    url = routerUrl;

                string RewritedUrl = url + "/" + selectedContent + "/" + selectedItemId + "/o_" + selectedTabId + "/" + lang;
                
                string UserId = Session["UserId"].ToString();
                int j = UpdateTabLink(createdTabId, RewritedUrl,data_route.RouteId, UserId);

                if (j == 1)
                {
                    lblResult.Text = "Cập nhật thành công";
                    MultiView1.ActiveViewIndex = 1;
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                }
            }
        }

        private int UpdateTabLink(string TabId, string TabPath, int routerid, string LastModifiedByUserId)
        {
            TabController tab_obj = new TabController();
            int result = tab_obj.UpdateLinkFrontPage(TabId,TabPath,routerid,LastModifiedByUserId);
            return result;
        }

    }
}