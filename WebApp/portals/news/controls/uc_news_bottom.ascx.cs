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
    public partial class uc_news_bottom : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CodeArray();
            }
        }

        private void CodeArray()
        {
            string[] codeArray = new string[] { "XTTM", "THH", "NCGT", 
                                        "DNCB", "HTHH" };
            string div=string.Empty;
            string result = string.Empty;
            foreach (string code in codeArray)
            {
                if (codeArray.First() == code)
                    div = "box-widget first";
                else
                    div = "box-widget";
                result += NewBottomPopulateData(code, div);
            }
            ltrNewBottom.Text = result;
        }

        private string NewBottomPopulateData(string code, string div)
        {
            int record = 3;
            string strHTML = string.Empty;
            string newtop = string.Empty;
            string li = string.Empty;
            string path_front_image = "/user_files/images/article_images/front_images/";
            ArticleController article_obj = new ArticleController();
            DataTable dt = article_obj.GetListByNumCode(code, record);
            if (dt.Rows.Count > 0)
            {
                newtop += "<div class='box-widget-header'><h2><a href='#'>" + dt.Rows[0]["CategoryName"].ToString() + "</a></h2></div>"
                                + "<div class='box-widget-bottom fx'>"
                                    + "<div class='box-widget-bottom-ct'>"
                                        + "<a class='thumb' href=''><img height='150' width='150' src='" + path_front_image + dt.Rows[0]["FrontImage"].ToString() + "' alt=" + dt.Rows[0]["Alias"].ToString() + "></a>"
                                        + "<h3><a title=\"" + dt.Rows[0]["Alias"].ToString() + "\" class='title' href=''>" + dt.Rows[0]["Title"].ToString() + "</a></h3>";

                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    li += "<li><a href='#' title=\"" + dt.Rows[i]["Alias"].ToString() + "\">" + dt.Rows[i]["Title"].ToString() + "</a></li>";
                }
            }
            //====================================================================================
            strHTML += "<div class=\"" + div + "\">"
                                + newtop
                                + "<ul>" + li + "</ul>"
                                + "</div>"
                            + "</div>"
                        + "</div>";
            return strHTML;
        }
    }
}