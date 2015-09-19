using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ArticleLibrary;
using CommonLibrary.Modules;

namespace WebApp.portals.news
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadArticleCategory();
            }
        }

        private void LoadArticleCategory()
        {
            string ChildrenCode = string.Empty, strHTML = string.Empty;
            string Title = string.Empty;
            List<ArticleCategory> lst = ArticleCategoryController.GetAllChildrenNodesOfSelectedNode("HDDN", "vi-VN", "2").Skip(1).ToList();
            foreach (var x in lst)
            {
                strHTML += LoadTinHiepHoi(x.CategoryCode, x.CategoryName);
            }
            lblNews.Text = strHTML;
        }
        private string LoadTinHiepHoi(string code, string CateName)
        {
            string strHTML = string.Empty, result = string.Empty, Abstract = string.Empty, Alias = string.Empty, CreateDate = string.Empty, Headline = string.Empty;
            int totalItemCount = 5;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            string main_image_path = "/user_files/images/article_images/front_images/";
            string file_path = baseUrl + main_image_path;
            List<Article> lst = ArticleController.GetActiveListByFixedNumCode(code, totalItemCount);
            DataTable dt = CommonLibrary.Common.Utilities.CollectionsUtil.ConvertTo(lst);
            if (dt.Rows.Count > 0)
            {
                string img = string.Empty;                
                if (!string.IsNullOrEmpty(dt.Rows[0]["FrontImage"].ToString()))
                    img = file_path + dt.Rows[0]["FrontImage"].ToString();
                else
                    img = "/images/no_image.jpg";
                if (!string.IsNullOrEmpty(dt.Rows[0]["DateCreated"].ToString()))
                {
                    DateTime _CreateDate = Convert.ToDateTime(dt.Rows[0]["DateCreated"].ToString());
                    CreateDate = "(" + _CreateDate.ToString("dd/MM/yyyy") + ")";
                }
             
                Alias = dt.Rows[0]["Alias"].ToString();
                if (!string.IsNullOrEmpty(dt.Rows[0]["Headline"].ToString()) && dt.Rows[0]["Headline"].ToString().Length > 300)
                    Headline = StringHandleClass.cutSubString(dt.Rows[0]["Headline"].ToString(), 300) + "...";
                else
                    Headline = dt.Rows[0]["Headline"].ToString();


                if (!string.IsNullOrEmpty(dt.Rows[0]["Abstract"].ToString()) && dt.Rows[0]["Abstract"].ToString().Length > 300)
                    Abstract = StringHandleClass.cutSubString(dt.Rows[0]["Abstract"].ToString(), 300) + "...";
                else
                    Abstract = dt.Rows[0]["Abstract"].ToString(); 

                result += "<div class=\"first\">"
                               + "<h3><a title=\"" + Headline + "\" href=\"/tin-chi-tiet/" + Alias + "/" + code + "\" target='_self'>" + Headline + "</a></h3>"

                               + " <a href=\"/tin-chi-tiet/" + Alias + "/" + code + "\" class=\"thumb t-left\" title=\"" + Headline + "\" target='_blank'>"
                               + "    <img width=\"120\" height=\"120\" title=\"" + Title + "\"  alt=\"" + Alias + "\" src=\"" + img + "\">"
                               + " </a>"
                               + "<div class=\"sapo\">" + Abstract + "</div>"
                                + " <span class='date'>Ngày đăng: " + CreateDate + "</span>"
                               + "<div style=\"clear:both\"></div>"                              
                               + "</div>"
                               + LoadTinLienQuan(dt);

                strHTML = "<div class=\"box-widget box-home-item style-1\">"
                           + "<div class=\"box-widget-header\">"
                           + "<h2><a href=\"/tin-tuc/" + code + "\" target='_blank'>" + CateName + "</a></h2>"
                           + "<a class=\"rss\" href=\"/tin-tuc/" + code + "\"  target='_blank'></a>"
                           + "</div>"
                           + "<div class=\"entry-content\">"
                           + "<div class=\"entry-content-fx fx\">"
                           + "<div class=\"entry-content-fxb\">"
                           + "<div class=\"entry-content-fxh\">"
                           + result
                           + "<div class=\"clear\"></div>"
                           + "</div>"
                           + "</div>"
                           + "</div>"
                             + "</div>"
                          + "</div>";
            }
            return strHTML;
        }

        private string LoadTinLienQuan(DataTable dt)
        {
            string strHTML = string.Empty, result = string.Empty,strAlias = string.Empty, strCreateDate = string.Empty, strHeadline = string.Empty;

            if (dt.Rows.Count > 1)
            {   
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    strAlias = dt.Rows[i]["Alias"].ToString();
                    strHeadline = dt.Rows[i]["Headline"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[i]["DateCreated"].ToString()))
                    {
                        DateTime _strCreateDate = Convert.ToDateTime(dt.Rows[i]["DateCreated"].ToString());
                        strCreateDate = "(" + _strCreateDate.ToString("dd/MM/yy") + ")";
                    }
                    result += "<li><a title=\"\" href=\"/tin-chi-tiet/" + strAlias + "/" + dt.Rows[i]["CategoryCode"].ToString() + "\" target='_blank'>" + strHeadline + " <span class='date'>" + strCreateDate + "</span></a></li>";
                }
                strHTML = "<div class=\"morelink\"><ul>" + result + "</ul></div>";
            }    
            else
                strHTML = "<div class=\"morelink\"><ul><li>" + result + "</li></ul></div>";
            return strHTML;
        }
    }
}