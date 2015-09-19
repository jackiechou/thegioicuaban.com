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
    public partial class uc_topnews : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string CultureCode = "vi-VN";
                int iTotalItemCount = 3;
                GetEventList(CultureCode, iTotalItemCount);
            }
        }

        private void GetEventList(string CultureCode, int iTotalItemCount)
        {
            string ChildrenCode = string.Empty, strHTML = string.Empty;
            string Title = string.Empty;
            string Parent_Node = "HDDN";
            List<ArticleCategory> lst = ArticleCategoryController.GetAllChildrenNodesOfSelectedNode(Parent_Node, CultureCode, "2").Skip(1).ToList();
            foreach (var x in lst)
            {
                strHTML += GetNewsListByFixedNumCodeCultureCode(x.CategoryCode, CultureCode, iTotalItemCount);
            }
            divAdvItems.InnerHtml = strHTML;
        }


        private string GetNewsListByFixedNumCodeCultureCode(string Code, string CultureCode, int iTotalItemCount)
        {
            string strHTML = string.Empty, Alias = string.Empty, Headline = string.Empty, Source = string.Empty,
                 Title = string.Empty, FilePath = string.Empty, Link = string.Empty;
            string path = ResolveClientUrl("/user_files/images/article_images/front_images/");

            List<Article> lst = ArticleController.GetActiveListByFixedNumCode(Code, CultureCode, iTotalItemCount);
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    Title = item.Title;
                    Alias = item.Alias;
                    Source = item.Source;
                    Headline = item.Headline;
                    FilePath = path + item.FrontImage;
                    if (string.IsNullOrEmpty(item.NavigateUrl))
                        Link = "/tin-chi-tiet/" + Alias + "/" + Code;
                    else
                        Link = item.NavigateUrl;

                    strHTML += "<div class='divAdvItems'>"
                                 + "<div class='divTitle'><a target='_blank' href='" + Link + "' >" + Title + "</a></div>"
                                 + "<div class='itemmc'><a target='_blank' href='" + Link + "' >" + Source + "</a></div>"
                                 + "<div class='divImage'>"
                                    + "<a target='_blank' href='" + Link + "'><img vspace='0' hspace='0' border='0' align='left' alt='" + Alias + "' src='" + FilePath + "' /></a>"
                                    + "<div class='price'><a target='_blank' href='" + Link + "' >" + Headline + "</a></div>"
                                 + "</div>"
                               + "</div>"
                               + "<div class='divBorder'><span></span></div>";
                }
            }
            return strHTML;
        }

        //protected void NewPopulateData()
        //{
        //    string code = "NEWS_START";
        //    int record = 5;
        //    string strHTML = string.Empty;
        //    string path_main_image = "/user_files/images/article_images/main_images/";
        //    string path_front_image = "/user_files/images/article_images/front_images/";
        //    ArticleController article_obj = new ArticleController();
        //    DataTable dt = article_obj.GetListByNumCode(code, record);
        //    if (dt.Rows.Count > 0)
        //    {
        //            strHTML = "<div id=\"featured_news\" class=\"fl\">"  
        //                        +"<ul>"
        //                            + "<li class=\"topnews\">"
        //                                + " <a title=\"" + dt.Rows[0]["Headline"].ToString() + "\" href=\"#\">"
        //                                    + " <img width=\"380\" height=\"280\" alt=\"" + dt.Rows[0]["Alias"].ToString() + "\" src='" + path_main_image + dt.Rows[0]["MainImage"].ToString() + "' />"
        //                                    + "<div>"
        //                                        + "<h1>"
        //                                            + dt.Rows[0]["TiTle"].ToString()
        //                                        + "</h1>"
        //                                        + "<p class=\"chapeau\">"
        //                                            + dt.Rows[0]["Headline"].ToString()
        //                                        + "</p>"
        //                                    + "</div>"
        //                                + " </a>"
        //                            + "</li>"
        //                         + "</ul>"
        //                        +"</div>";
        //            for (int i = 1; i < dt.Rows.Count; i++)
        //            {
        //                strHTML += "<ul class=\"typicalevents\">"
        //                                + "<li>"
        //                                    + " <a title=\"" + dt.Rows[i]["Headline"].ToString() + "\" href=\"#\">"
        //                                        + " <img class=\"newsphoto_small\" width=\"80\" height=\"57\" alt=\"" + dt.Rows[i]["Alias"].ToString() + "\" src='" + path_front_image + dt.Rows[i]["FrontImage"].ToString() + "' />"
        //                                        + "<h4>"
        //                                            + dt.Rows[i]["TiTle"].ToString()
        //                                        + "</h4>"
        //                                    + " </a>"
        //                                + "</li>"
        //                          + "</ul>";
        //            }
        //        divTopNews.InnerHtml = strHTML;
        //    }
        //}
    }
}