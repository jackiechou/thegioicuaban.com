using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ArticleLibrary;

namespace WebApp.portals.news.controls
{
    public partial class uc_news_slide : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
            link.Href = "~/portals/news/skins/default_skin/css/widget.css";
            link.Attributes.Add("rel", "stylesheet");
            link.Attributes.Add("type", "text/css");
            Page.Header.Controls.Add(link);

            System.Web.UI.HtmlControls.HtmlGenericControl ctrl = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
            ctrl.Attributes.Add("type", "text/javascript");
            ctrl.Attributes.Add("src", Page.ResolveUrl("~/portals/news/skins/default_skin/js/slides.min.jquery.js"));
            this.Page.Header.Controls.Add(ctrl);

            //System.Web.UI.HtmlControls.HtmlGenericControl ctrl = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
            //ctrl.Attributes.Add("type", "text/javascript");
            //ctrl.Attributes.Add("src", Page.ResolveUrl("~/scripts/jquery/jquery.min.js"));
            //this.Page.Header.Controls.Add(ctrl);

            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "jQueryScripts", "<script type=\"text/javascript\" src='" + Page.ResolveUrl("~/scripts/jquery/jquery.min.js") + "'></script>", false);
            //Page.ClientScript.RegisterClientScriptInclude(this.GetType(),"jQueryScripts", Page.ResolveUrl("~/scripts/jquery/jquery.min.js"));

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadHotNews();
            }
        }


        private void LoadHotNews()
        {
            string strHTML = string.Empty;
            //string code = "BEAUTY_NEWS";
            string code = "DAILY_NEWS";
            int totalItemCount = 5;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            string CategoryCode = string.Empty, ForwardUrl = string.Empty, Title = string.Empty,
                   Abstract = string.Empty, Alias = string.Empty, Headline = string.Empty, FilePath = string.Empty,
                   MainImage = string.Empty;

            List<Article> lst = ArticleController.GetActiveListByFixedNumCode(code, totalItemCount);
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    CategoryCode = item.CategoryCode;
                    Title = item.Title;
                    Alias = item.Alias;
                    Abstract = item.Abstract;
                    Headline = item.Headline;
                    MainImage = item.MainImage;
                    //FilePath = baseUrl + "/user_files/images/article_images/main_images/" + MainImage;
                    FilePath = "http://backend.ngoisao.vn/data/cnn_270x162/2012/12/10/ncngoai567.jpg?1355243125310";
                    ForwardUrl = baseUrl + "/portals/news/details.aspx?code=" + CategoryCode + "&alias=" + Alias;

                    strHTML += "<div class=\"widget-item\">"
                               + "<p>"
                                   + "<a href='" + ForwardUrl + "' title='" + Title + "' target='_blank'>"
                                   + "<img src='" + FilePath + "' width='570' height='270' alt='" + Title + "'></a>"
                               + "</p>"
                               + "<a class='widget-title' href='" + ForwardUrl + "' title='" + Title + "' target='_blank'>" + Title + "</a>"
                           + "</div>";
                }


            }

            divSlideContainer.InnerHtml = "<div class=\"widget\">"
                                            + "<div class=\"widget-header\">"
                                                + "<a title=\"5eagles.com.vn\" target=\"_blank\" href=\"http://5eagles.com.vn\">&nbsp;</a>"
                                            + "</div>"
                                            + "<div id=\"widget-slider\" class=\"widget-slider\">"
                                                    + "<div class=\"slides_container\">"
                                                            + strHTML
                                                    + "</div>"
                                                    + "<div class=\"widget-copyright\">"
                                                    + "<a title=\"5eagles.com.vn\" target=\"_blank\" href=\"http://5eagles.com.vn\">www.5eagles.com.vn</a>"
                                                    + "</div>"
                                            + "</div>"
                                       + " </div>";
        }
    }
}