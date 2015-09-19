using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Entities.Tabs;
using System.Configuration;
using System.IO;
using System.Web.UI.HtmlControls;

namespace WebApp.modules.admin.tabs
{
    public partial class admin_front_tab_control : System.Web.UI.Page
    {
        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Theme = "default";
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            Page.Culture = "vi-VN";
            Page.UICulture = "vi";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string title = string.Empty, portalId = string.Empty, selectedTabId = string.Empty, createdTabId = string.Empty, createdTabPath = string.Empty,
                contentItemKey = string.Empty, keyWords = string.Empty, lang = string.Empty, selectedTabControlPath = string.Empty;
            
            if (Request.QueryString["selectedTabId"] != null && Request.QueryString["selectedTabId"].ToString() != string.Empty)
                selectedTabId = Request.QueryString["SelectedTabId"].ToString();
            if (Request.QueryString["createdTabId"] != null && Request.QueryString["createdTabId"].ToString() != string.Empty)
                createdTabId = Request.QueryString["createdTabId"].ToString();
            if (Request.QueryString["createdTabPath"] != null && Request.QueryString["createdTabPath"].ToString() != string.Empty)
                createdTabPath = Request.QueryString["createdTabPath"].ToString();
            if (Request.QueryString["portalId"] != null && Request.QueryString["portalId"].ToString() != string.Empty)
                portalId = Request.QueryString["portalId"].ToString();
            if (Request.QueryString["contentItemKey"] != null && Request.QueryString["contentItemKey"].ToString() != string.Empty)
                contentItemKey = Request.QueryString["contentItemKey"].ToString();
            if (Request.QueryString["keyWords"] != null && Request.QueryString["keyWords"].ToString() != string.Empty)
                keyWords = Request.QueryString["keyWords"].ToString();
            if (Request.QueryString["lang"] != null && Request.QueryString["lang"].ToString() != string.Empty)
                lang = Request.QueryString["lang"].ToString();

              
            aspnet_Tab entity = TabController.GetTabPathByTabId(Convert.ToInt32(selectedTabId));
            title = entity.Title;
            selectedTabControlPath = "/" + ConfigurationManager.AppSettings["admin_folder"] + entity.TabPath ;

            if (File.Exists(Server.MapPath(selectedTabControlPath)))
            {                
                loadUserControlToPlaceHolder(PlaceHolder1, selectedTabControlPath);
            }                      
        }
        #region LOAD USER CONTROL TO PLACE HOLDER ========================================================================
        public void loadUserControlToPlaceHolder(PlaceHolder myPlaceHolder, string controlPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(controlPath))
                {
                    myPlaceHolder.Controls.Clear();
                    string control_path = System.Text.RegularExpressions.Regex.Replace(controlPath, @"<[^>]*>", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    //UserControl userControl = (System.Web.UI.UserControl)this.LoadControl(controlPath);
                    //userControl.EnableViewState = false;
                    //myPlaceHolder.Controls.Add(userControl);

                    //Add div 
                    HtmlGenericControl gel = new HtmlGenericControl("div");
                    //Assign usercontrol on web page
                    Control ct = Page.LoadControl(control_path);
                    //add user control to div
                    gel.Controls.Add(ct);
                    //add div to placeholder                  
                    myPlaceHolder.Controls.Add(gel);
                }

            }
            catch (HttpException ex) { ex.ToString(); }
        }
        #endregion =================================================================================================

    }
}