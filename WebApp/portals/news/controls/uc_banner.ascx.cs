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
    public partial class uc_banner : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PopulateBanner();
            }
        }

        protected void PopulateBanner()
        {
            string code = "BANNER_NEWS";
            int record = 4;
            string base_url = CommonLibrary.Common.Utilities.UrlUtils.BaseSiteUrl;
            string strHtml = string.Empty, sliderImage = string.Empty, listBanner = string.Empty, Headline = string.Empty, Alias = string.Empty,
                Title = string.Empty, MainImage_Url = string.Empty, FrontImage_Url = string.Empty, NavigateUrl = string.Empty;
            const string pathMainImage = "/user_files/images/article_images/main_images/";
            const string pathFrontImage = "/user_files/images/article_images/front_images/";
            var article_obj = new ArticleController();
            DataTable dt = article_obj.GetListByNumCode(code, record);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Title = dt.Rows[i]["Title"].ToString();
                    Headline = dt.Rows[i]["Headline"].ToString();
                    Alias = dt.Rows[i]["Alias"].ToString();
                    FrontImage_Url = base_url + "/" + pathFrontImage + "/" + dt.Rows[i]["FrontImage"].ToString();
                    MainImage_Url = base_url + "/" + pathMainImage + "/" + dt.Rows[i]["MainImage"].ToString();
                    NavigateUrl = dt.Rows[i]["NavigateUrl"].ToString();
                    if (NavigateUrl == "")
                        NavigateUrl = "/tin-chi-tiet/" + Alias + "/" + code;
                    sliderImage += "<a href=\"" + NavigateUrl + "\" target=\"_blank\"><img src='" + MainImage_Url + "' alt='" + Title + "' /></a>";
                    listBanner += "<div class=\"thumb\">"
                           + "<div class=\"frame\"><img src='" + FrontImage_Url + "' alt='" + Title + "' /></div>"
                           + "<div class=\"thumb-content\"><p><a href=\"" + NavigateUrl + "\" target=\"_blank\">" + Title + "</a></p></div>"
                           + "<div style=\"clear:both;\"></div>"
                       + "</div>";
                }
                strHtml = "<div id=\"slider\">" + sliderImage + "</div>"
                          + "<div id=\"thumbs\">" + listBanner + "</div>";
            }
            divBanner.InnerHtml = strHtml;
        }
    }
}