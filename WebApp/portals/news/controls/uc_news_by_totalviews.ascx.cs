using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ArticleLibrary;

namespace WebApp.portals.news.controls
{
    public partial class uc_news_by_totalviews : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string CultureCode = "vi-VN";
                int iTotalItemCount = 1;
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
            string strHTML = string.Empty, Alias = string.Empty, Headline = string.Empty, Source = string.Empty, TotalViews = string.Empty,
                 Abstract = string.Empty, FilePath = string.Empty, Link = string.Empty;          

            List<Article> lst = ArticleController.GetActiveListByTotalViewsFixedNum(Code, CultureCode, iTotalItemCount);
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    Abstract = item.Abstract;
                    Alias = item.Alias;                   
                    Headline = item.Headline;
                   
                    TotalViews = item.TotalViews.ToString();
                    if (string.IsNullOrEmpty(item.NavigateUrl))
                        Link = "/tin-chi-tiet/" + Alias + "/" + Code;
                    else
                        Link = item.NavigateUrl;

                    if (string.IsNullOrEmpty(item.Source))
                        Source = Request.Url.Host;
                    else
                        Source = item.Source;

                    if (!string.IsNullOrEmpty(item.FrontImage))
                        FilePath = ResolveClientUrl("/user_files/images/article_images/front_images/") + item.FrontImage;
                    else
                        FilePath = "/images/no_image.jpg";


                    strHTML += "<div class='divAdvItems'>"
                                 + "<div class='divTitle'><a target='_blank' href='" + Link + "' >" + Headline + "</a></div>"
                                 + "<div class='totalviews'>Lượt xem: " + TotalViews + "</div>"
                                 + "<div class='divImage'>"
                                    + "<a target='_self' href='" + Link + "'><img vspace='0' hspace='0' border='0' align='left' alt='" + Alias + "' src='" + FilePath + "' /></a>"
                                    + "<div class='price'><a target='_self' href='" + Link + "' >" + Abstract + "</a></div>"
                                 + "</div>"
                                 + "<div class='itemmc'><a target='_self' href='" + Link + "' >Nguồn: " + Source + "</a></div>"
                                 + "<div class='clear'></div>"
                               + "</div>"                               
                               + "<div class='divBorder'><span></span></div>";
                }
            }
            return strHTML;
        }
    }
}