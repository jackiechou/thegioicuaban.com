using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ArticleLibrary;
using System.Data;

namespace WebApp.portals.news
{
    public partial class vticker_demo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PopulateData("DN");
            }
        }

        protected void PopulateData(string code)
        {
            string strHTML = string.Empty, path_image = string.Empty, path_image1 = string.Empty, NavigateUrl = string.Empty, NavigateUrl1 = string.Empty;
            ArticleController article_obj = new ArticleController();
            DataTable dt = article_obj.GetListByCode(code);
            if (dt.Rows.Count > 1)
            {
                Uri requestUri = Context.Request.Url;
                string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);

                int total = 0;
                if (dt.Rows.Count / 2 == 0)
                    total = dt.Rows.Count;
                else
                    total = dt.Rows.Count - 1;

                for (int i = 0; i < total; i += 2)
                {
                    if (dt.Rows[i]["Frontimage"].ToString() != "")
                        path_image = baseUrl + "/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/article_images/front_images/" + dt.Rows[i]["Frontimage"].ToString();
                    else
                        path_image = baseUrl + "/images/no_image.jpg";

                    if (dt.Rows[i + 1]["Frontimage"].ToString() != "")
                        path_image1 = baseUrl + "/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/article_images/front_images/" + dt.Rows[i + 1]["Frontimage"].ToString();
                    else
                        path_image1 = baseUrl + "/images/no_image.jpg";

                    if (dt.Rows[i]["NavigateUrl"].ToString() != "")
                        NavigateUrl = dt.Rows[i]["NavigateUrl"].ToString();
                    else
                        NavigateUrl = "#";
                    if (dt.Rows[i + 1]["NavigateUrl"].ToString() != "")
                        NavigateUrl1 = dt.Rows[i + 1]["NavigateUrl"].ToString();
                    else
                        NavigateUrl1 = "#";
                    strHTML += "<li class=\"item\">"                                     
                                    + "<a class=\"col_odd\" href='" + NavigateUrl + "'><img width='120px' height=120px' src=\"" + path_image + "\" alt=\"" + dt.Rows[i]["Alias"].ToString() + "\" /></a>"                                     
                                    + "<a class=\"col_even\" href='" + NavigateUrl1 + "'><img width='120px' height=120px' src=\"" + path_image1 + "\" alt=\"" + dt.Rows[i + 1]["Alias"].ToString() + "\" /></a>"                                                                         
                                + "</li>";
                }
            }
            else
                strHTML = "<li class=\"item\">Dữ liệu không hợp lý</li>";
            scroller.InnerHtml = "<ul id=\"scroller_vticker\" class=\"ticker\">" + strHTML + "</ul>";
        }    
    }
}