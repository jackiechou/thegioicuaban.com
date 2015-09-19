using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Entities.Users;

namespace WebApp.templates.admin_templates.default_temp
{
    public partial class PortalSite : System.Web.UI.MasterPage
    {
        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            Page.Culture = "vi-VN";
            Page.UICulture = "vi";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] != null && Session["UserName"].ToString() != string.Empty)
            {
                Literal_UserInfo.Text = LoadProfile();
            }
        }

        private string LoadProfile()
        {
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            string username = string.Empty, fullname = string.Empty, email = string.Empty, link_logout = string.Empty;
            string result = string.Empty;
            link_logout = baseUrl + "/logout.aspx";


            if (Session["UserName"] != null && Session["UserName"].ToString() != string.Empty)
            {
                username = Session["UserName"].ToString();

                UserController user_controller_obj = new UserController();
                DataTable dt = user_controller_obj.GetDetailsByUserName(username);
                email = dt.Rows[0]["Email"].ToString();
                fullname = dt.Rows[0]["FullName"].ToString();
                Session.Timeout = 216000;

                result = "<div class=\"userinfo\">"
                              + "<img src='" + baseUrl + "/images/shopcarts/user_avatar.png' alt=\"user_avatar\" />"
                              + "<span>" + username + "</span>"
                         + "</div>"
                         + "<div style=\"display: none;\" class=\"userinfodrop\">"
                              + "<div class=\"avatar\">"
                                    + "<a href=\"#\"><img src='" + baseUrl + "/images/shopcarts/avatarbig.png' alt='big_user' /></a>"
                              + "</div>"
                              + "<div class=\"userdata\">"
                                    + "<h4>" + fullname + "</h4>"
                                    + "<p class='email'>" + email + "</p>"
                                    + "<ul>"
                                        + "<li><a class=\"userdata_link\" onclick=\"javascript:return ShowUserProfileModal('" + username + "');\">Edit Profile</a></li>"
                                        + "<li><a class=\"userdata_link\" href='" + link_logout + "'>Sign Out</a></li>"
                                    + "</ul>"
                              + "</div>"
                         + "</div>";

            }
            return result;
        }
    }
}