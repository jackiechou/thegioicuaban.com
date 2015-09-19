using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Configuration;
using CommonLibrary.Entities.Tabs;
using System.Data;

namespace WebApp
{
    public partial class index : System.Web.UI.Page
    {
        #region SESSION PROPERTIES ===========================
        protected string ApplicationId
        {
            get
            {
                if (Session["ApplicationId"] != null)
                    return Session["ApplicationId"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string UserId
        {
            get
            {
                if (Session["UserId"] != null)
                    return Session["UserId"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string RoleId
        {
            get
            {
                if (Session["RoleId"] != null)
                    return Session["RoleId"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string PortalId
        {
            get
            {
                if (Session["PortalId"] != null)
                    return Session["PortalId"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string VendorId
        {
            get
            {
                if (Session["VendorId"] != null)
                    return Session["VendorId"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string IsSuperUser
        {
            get
            {
                if (Session["IsSuperUser"] != null)
                    return Session["IsSuperUser"].ToString();
                else
                    return string.Empty;
            }
        }


        protected string UpdatePassword
        {
            get
            {
                if (Session["UpdatePassword"] != null)
                    return Session["UpdatePassword"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string IsDeleted
        {
            get
            {
                if (Session["IsDeleted"] != null)
                    return Session["IsDeleted"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string UserName
        {
            get
            {
                if (Session["UserName"] != null)
                    return Session["UserName"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string HomeDirectory
        {
            get
            {
                if (Session["HomeDirectory"] != null)
                    return Session["HomeDirectory"].ToString();
                else
                    return string.Empty;
            }
        }
        #endregion ===========================================

        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInit(e);
        //    if (Context.Session != null)
        //    {
        //        //check whether a new session was generated
        //        if (Session.IsNewSession)
        //        {                    
        //            string szCookieHeader = Request.Headers["ASP.NET_SessionId"];
        //            if ((null != szCookieHeader) && (szCookieHeader.Length > 0))                    
        //                Session["IsSessionTimeOut"] = true;
        //            else
        //                Response.RedirectToRoutePermanent("admin_login");
        //       }

        //        //check whether a cookies had already been associated with this request
        //        //HttpCookie sessionCookie = Request.Cookies["ASP.NET_SessionId"];
        //        //if (sessionCookie != null)
        //        //{
        //        //    string sessionValue = sessionCookie.Value;
        //        //    if (!string.IsNullOrEmpty(sessionValue))
        //        //    {
        //        //        // we have session timeout condition!
        //        //        // Response.Redirect("SessionTimeout.htm");
        //        //        Session["IsSessionTimeOut"] = true;
        //        //    }
        //        //}else
        //        //    Response.RedirectToRoutePermanent("admin_login");  
        //   }            
        //}


        public void Page_PreInit(Object sender, EventArgs e)
        {
            string page_title = "ADMIN";
            string page_theme = "default";
            Page.Title = page_title.Substring(0, page_title.Length - 5); ;
            Page.Theme = page_theme;
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            Page.Culture = "vi-VN";
            Page.UICulture = "vi";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserId != string.Empty)
            {
                string TabPath = string.Empty, page_url = string.Empty, title = string.Empty; int tabid = 1;
                if (Page.RouteData.Values.Count > 0)
                {
                    string strTabId = Page.RouteData.Values["tabid"].ToString();
                    if (!string.IsNullOrEmpty(strTabId))
                    {                        
                        tabid = Convert.ToInt32(strTabId);
                        PopulateNavigationSteps(tabid);

                        aspnet_Tab entity = TabController.GetTabPathByTabId(tabid);
                        TabPath = System.Text.RegularExpressions.Regex.Replace(entity.TabPath, @"\s+", " ").Trim();
                        page_url = "/" + ConfigurationManager.AppSettings["admin_folder"] + TabPath;
                        if (File.Exists(Server.MapPath(page_url)))
                            loadUserControlToPlaceHolder(PlaceHolder1, page_url);
                        else
                        {
                            string scriptCode = "<script>alert('Không tồn tại đường dẫn này.');</script>";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                        }
                    }
                    else
                    {
                        string scriptCode = "<script>alert('Không tìm thấy tham số cần truyền');</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                    }
                }
                else
                {
                    string scriptCode = "<script>alert('Không tìm thấy tham số truyền vào');</script>";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                }                
            }
            else
                Response.RedirectToRoutePermanent("admin_login");
                        
        }

        private void PopulateNavigationSteps(int TabId)
        {
             TabController entity = new TabController();
             DataTable dt = entity.GetAllParentNodesOfSelectedNode(TabId,1);
             string strHTML = string.Empty, Title = string.Empty, Liz = string.Empty, Link = string.Empty;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++) 
                {
                    Title = dt.Rows[i]["Title"].ToString();                     
                    Link = "/index/" + dt.Rows[i]["TabId"].ToString();
                    Liz += "<li><a href=\"" + Link + "\" title=\"" + Title + "\" target='_self'><span>" + Title + "</span></a>";
                }
            }

            strHTML = "<ul><li class=\"first\"><a href=\"#\">Home</a></li>"
                        + Liz
                        +"</ul>";
            Literal_Title.Text = strHTML;

        }

        #region LOAD USER CONTROL TO PLACE HOLDER ========================================================================
        public void loadUserControlToPlaceHolder(PlaceHolder myPlaceHolder, string controlPath)
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
        #endregion =================================================================================================

        #region LOAD WEB PAGE ==================================================
        protected void load_page(string page)
        {
            if (page != "")
            {
                //HtmlControl frame1 = (HtmlControl)this.FindControl("frame1");
                frame1.Visible = true;
                frame1.Attributes["src"] = page;
            }
        }
        #endregion =============================================================

        //private void addControl(string controlPath)
        //{
        //    if (!string.IsNullOrEmpty(controlPath))/
        //    {                   
        //        string control_path = System.Text.RegularExpressions.Regex.Replace(controlPath, @"<[^>]*>", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //        divContent.Controls.Add(LoadControl(controlPath));                  
        //    }            
        //}
    }
}
