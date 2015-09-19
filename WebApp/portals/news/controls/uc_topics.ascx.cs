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
    public partial class uc_topics : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                TopicPopulateData();
            }
        }

        protected void TopicPopulateData()
        {
            string code = "NEWS_START";
            int record = 10;
            string strHTML = string.Empty;
            string path_image = "/user_files/images/article_images/front_images/";
            string scrimage = "/portals/news/skin/images/common/reddot.gif";
            ArticleController article_obj = new ArticleController();
            DataTable dt = article_obj.GetListByNumCode(code, record);

            //strHTML = "<div class=\"header-topic\">"
            //            + "<div style=\"float:left\">"
            //                + "<p class=\"title-topic\"><a href=\"#\">" + dt.Rows[0]["CategoryName"].ToString() + "</a></p></div>"
            //                + "<div class=\"xemtatca\" ><a href=\"#\" >xem thêm</a></div>"
            //            + "</div>"
            //            +"<div class=\"content-topic\">";

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    strHTML += "<div class=\"inner-content-topic\">"
                                   + "<div class=\"content-item\">"
                                        + "<div class=\"content-inner-item\">"
                                            + " <a title=\"\" href=\"\">"
                                                + " <img alt=\"\" class=\"content-inner-item\" src='" + path_image + dt.Rows[i]["FrontImage"].ToString() + "' />"
                                            + " </a>"
                                        + "</div>"
                                   + "</div>"
                                   + "<div class=\"content_read\">"
                                        + "<div>"
                                            + " <p class=\"tieudebaiviet\">"
                                                + " <a href=\"#\">"
                                                    + dt.Rows[i]["Title"].ToString()
                                                +"</a>"
                                            + " </p>"
                                        + "</div>"
                                        + "<div class=\"motabaiviet\">"
                                            + " <p style=\"padding:5px 0px 0px 0px; margin:0px;\">"
                                            + " </p>"
                                        + "</div>"
                                        + "<div datetime=\"motabaiviet\" style=\"padding:10px 0px 0px 0px;display:none\">"
                                            + dt.Rows[i]["DateCreated"].ToString()
                                        + "</div>"
                                   + "</div>"
                             + "</div>";
                }
                string result = string.Empty;
                for (int i = 2; i < dt.Rows.Count; i++)
                {
                    result += "<div class=\"content-subnews\">"
                                    + " <img alt=\"\" src='" + scrimage + "' />"
                                    + " <a title=\"" + dt.Rows[i]["Alias"].ToString() + "\" href=\"\">"
                                    + dt.Rows[i]["TiTle"].ToString() + " </a>"
                            + " </div>";
                }
                strHTML += "<div class=\"content-item-last\">"
                                + "<div style=\"height:130px;overflow:hidden\">"
                                    + result
                                + "</div>"
                           + "</div>"
                           + "</div>";
                divTopics.InnerHtml = strHTML;
            }
        }
    }
}