using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ArticleLibrary;
using System.Data;

namespace WebApp.portals.news.controls
{
    public partial class uc_content : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                NewPopulateData();
            }
        }

        protected void NewPopulateData()
        {
            string code = "NEWS";
            int record = 4;
            string strHTML = string.Empty;
            string path_image = "/user_files/images/article_images/front_images/";
            ArticleController article_obj = new ArticleController();
            DataTable dt = article_obj.GetListByNumCode(code, record);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strHTML += "<ul class=\"typicalevents\">"
                                    + "<li>"
                                        + " <a title=\"" + dt.Rows[i]["Headline"].ToString() + "\" href=\"#\">"
                                            + " <img class=\"newsphoto_small\" width=\"80\" height=\"57\" alt=\"" + dt.Rows[i]["Alias"].ToString() + "\" src='" + path_image + dt.Rows[i]["FrontImage"].ToString() + "' />"
                                            + "<h4>" 
                                                + dt.Rows[i]["TiTle"].ToString()
                                            + "</h4>"
                                        + " </a>"
                                    + "</li>"
                              + "</ul>";
                }
                divContent.InnerHtml = strHTML;
            }
        }
    }
}