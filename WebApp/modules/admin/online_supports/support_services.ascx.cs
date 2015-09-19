using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Net;
using System.IO;
using ArticleLibrary;

namespace WebApp.modules.admin.online_supports
{
    public partial class support_services : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblSupportOnline.Text = LoadSupportOnline();
        }

        public string LoadSupportOnline()
        {
            string result = null;
            try
            {
                ArticleController article_obj = new ArticleController();
                DataTable dt = article_obj.GetDetailByID(1);
                string Hotline = dt.Rows[0]["Hotline"].ToString();
                string SupportOnline = dt.Rows[0]["SupportOnline"].ToString();

                result = "<div id=\"service-online\">"
                            + "<h4>Dịch vụ khách hàng trực tuyến</h4>"
                            + "<div class=\"hotline\">" 
                                + Hotline 
                            + "</div>"
                            + "<div class='content-details'>"
                                +"<div style=\"text-align:center\">"
                                    + SupportOnline
                                + "</div>"
                        + "</div>";
            }
            catch (IndexOutOfRangeException ex) { ex.ToString(); }
            return result;
        }      
    }
}