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
    public partial class uc_news : System.Web.UI.UserControl
    {
        ArticleCategoryController cate_obj = new ArticleCategoryController();
        ArticleController article_obj = new ArticleController();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CategoriePopulateData();
            }
        }

        private string PopulateNewsByCode(string code, int records)
        {
            string path_image = "/user_files/images/article_images/front_images/";
            string strHTML = string.Empty;
            DataTable dt = article_obj.GetListByNumCode(code, records);
            if (dt.Rows.Count > 0)
            {
                string result = string.Empty;
                for (int j = 1; j < dt.Rows.Count; j++)
                {
                    result += "<li>"
                               + "<h2><a class=\"news_title\" title=\"" + dt.Rows[j]["Alias"].ToString() + "\" href=\"#\">" + dt.Rows[j]["Title"].ToString() + "</a></h2>"
                           + "</li>";
                }
                strHTML = "<div class=\"showcase_group\">"
                            + "<ul class=\"showcase_listing\">"
                                + "<h1><a title=\"" + dt.Rows[0]["CategoryCode"].ToString() + "\" href=\"#\">" + dt.Rows[0]["CategoryName"].ToString() + "</a></h1>"
                                + "<li class=\"topnews\">"
                                    + " <a title=\"" + dt.Rows[0]["Title"].ToString() + "\" href=\"#\">"
                                    + "<img style='width:150;height:104' alt=\"" + dt.Rows[0]["Alias"].ToString() + "\" src='" + path_image + dt.Rows[0]["FrontImage"].ToString() + "' /></a>"
                                    + "<h2><a class=\"news_title\" title=\"" + dt.Rows[0]["Alias"].ToString() + "\" href=\"#\">" + dt.Rows[0]["Title"].ToString() + "</a></h2>"
                                + "</li>"
                                + result
                            + "</ul>"
                        + "</div>";
            }
            return strHTML;
        }


        protected void CategoriePopulateData()
        {
            int CateId = 3;
            int num_rows = 10;
            string status = "2";
            string strHTML = string.Empty;
            ArticleCategoryController cate_obj = new ArticleCategoryController();
            DataTable dtCate = cate_obj.GetTreeNumListByCateIdStatus(CateId, num_rows, status);

            if (dtCate.Rows.Count > 0)
            {
                for (int i = 1; i < dtCate.Rows.Count; i++)
                {
                    strHTML += PopulateNewsByCode(dtCate.Rows[i]["CategoryCode"].ToString(), 3);
                }
                divCategories.InnerHtml = strHTML;
            }
        }
    }
}