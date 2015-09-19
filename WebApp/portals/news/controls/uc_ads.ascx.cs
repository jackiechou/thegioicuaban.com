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
    public partial class uc_ads : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ApsPopulateData();
            }
        }

        protected void ApsPopulateData()
        {
            string code = "ADS_NEWS";
            int record = 4;
            string strHTML = string.Empty, Alias = string.Empty, Headline = string.Empty, Source = string.Empty,
                 Abstract = string.Empty, FilePath = string.Empty, Link = string.Empty;
            
            ArticleController article_obj = new ArticleController();
            DataTable dt = article_obj.GetListByNumCode(code, record);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Abstract = dt.Rows[i]["Abstract"].ToString();
                    Alias = dt.Rows[i]["Alias"].ToString();                    
                    Headline = dt.Rows[i]["Headline"].ToString();
                    Source = dt.Rows[i]["Source"].ToString();

                    if (!string.IsNullOrEmpty(dt.Rows[i]["FrontImage"].ToString()))
                        FilePath = "/user_files/images/article_images/front_images/" + dt.Rows[i]["FrontImage"].ToString();
                    else
                        FilePath = "/images/no_image.jpg";


                    if (string.IsNullOrEmpty(dt.Rows[i]["NavigateUrl"].ToString()))
                        Link = "/tin-chi-tiet/" + Alias + "/" + code;
                    else
                        Link = dt.Rows[i]["NavigateUrl"].ToString();

                    if (string.IsNullOrEmpty(dt.Rows[i]["Source"].ToString()))
                        Source = Request.Url.Host;
                    else
                        Source = dt.Rows[i]["Source"].ToString();                    

                    strHTML += "<div class='divAdvItems'>"
                                 + "<div class='divTitle'><a target='_self' href='" + Link + "' >" + Headline + "</a></div>"
                                 + "<div class='itemmc'><a target='_self' href='" + Link + "' >Nguồn: " + Source + "</a></div>"
                                 + "<div class='divImage'>"
                                    + "<a target='_self' href='" + Link + "'><img vspace='0' hspace='0' border='0' align='left' alt='" + Alias + "' src='" + FilePath + "' /></a>"
                                    + "<div class='price'><a target='_self' href='" + Link + "' >" + Abstract + "</a></div>"
                                 + "</div>"
                               + "</div>"
                               + "<div class='divBorder'><span></span></div>";
                }
                divAds.InnerHtml = strHTML;
            }
        }
    }
}