using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Tabs;
using System.IO;
using System.Configuration;
using System.Web.UI.HtmlControls;

namespace WebApp.templates.admin_templates.default_temp
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        #region SESSION PROPERTIES ===========================
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
        #endregion ===========================================

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadProfile();              
            }    
                
        }
        private void LoadProfile()
        {
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            string fullname = string.Empty, email = string.Empty;
            string result = string.Empty;

            if (UserName != string.Empty)
            {
                UserController user_controller_obj = new UserController();
                DataTable dt = user_controller_obj.GetDetailsByUserName(UserName);
                email = dt.Rows[0]["Email"].ToString();
                fullname = dt.Rows[0]["FullName"].ToString();
                Session.Timeout = 216000;

                result = "<div class=\"userinfo\">"
                              + "<img src=\"../images/icons/16/user_avatar.png\" alt=\"user_avatar\" />"
                              + "<span>" + UserName + "</span>"
                         + "</div>"
                         + "<div style=\"display: none;\" class=\"userinfodrop\">"
                              + "<div class=\"avatar\">"
                                    + "<a href=\"#\"><img src='" + baseUrl + "/images/icons/avatarbig.png' alt='big_user' /></a>"
                              + "</div>"
                              + "<div class=\"userdata\">"
                                    + "<h4>" + fullname + "</h4>"
                                    + "<p class='email'>" + email + "</p>"
                                    + "<ul>"
                                        + "<li><a class=\"userdata_link\" onclick=\"javascript:return ShowUserProfileModal('" + UserName + "');\">Edit Profile</a></li>"
                                        + "<li><a class=\"userdata_link\" rel=\"nofollow\" href='/logout.aspx'>Sign Out</a></li>"
                                    + "</ul>"
                              + "</div>"
                         + "</div>";
                Literal_Head.Text = result;
            }else
                Response.RedirectToRoutePermanent("admin_login");
            
        }
    }
}