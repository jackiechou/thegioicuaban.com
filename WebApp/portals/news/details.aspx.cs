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
    public partial class details : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Page.RouteData.Values.Count > 0)
                {
                    if (Page.RouteData.Values["alias"].ToString() != "" && Page.RouteData.Values["code"].ToString() != "")
                    {
                        string alias = Page.RouteData.Values["alias"].ToString();
                        string code = Page.RouteData.Values["code"].ToString();
                        ViewState["alias"] = alias;
                        ViewState["code"] = code;
                        PopulateEntryContent(alias, code);
                        PopulateRelatedNews(code, alias);
                      
                    }
                }      
            }
        }


        private string GetCategoryNavigation(string CategoryCode, string CultureCode, string Status)
        {                      
            List<ArticleCategory> lst = ArticleCategoryController.GetAllParentNodesBySelectedNodeStatus(CategoryCode, CultureCode, Status);
            string strHTML = string.Empty, name = string.Empty ,url = string.Empty, link = string.Empty;
            if (lst.Count > 0)
            {                
                foreach (var x in lst)
                {
                    name = x.CategoryName;
                    link = "/tin-tuc/" + x.CategoryCode;
                    //url += "<div><a href=\"#\" itemprop=\"url\" title=\"" + name + "\"><span itemprop=\"title\">" + name + "</span></a></div>";
                          //  + "<div><a class=\"path-a\" href=\""+link+"\" itemprop=\"url\" title=\"" + name + "\"><span itemprop=\"title\">"+ name +"</span></a></div>";
                    url += "<a href=\"" + link + "\" itemprop=\"url\" title=\"" + name + "\" target='_self'><span itemprop=\"title\">" + name + "</span></a>";
                }
            }

            strHTML =   "<div class=\"clorB\">"
                        + "<div id=\"ct-breadcrumbs\">"
                        + "<div class=\"fLeft navigation\">"
                        + "<div>"      
                        +    url
                        + "</div>"	
				        + "</div>"	
                        + "</div>"
                        + "</div>";
            return strHTML;
        }

        private void PopulateRelatedNews(string code, string alias)
        {
            string strHTML = string.Empty, list = string.Empty, strCreateDate = string.Empty;
           int totalItemCount = 10;
           List<Article> lst = ArticleController.GetActiveListByFixedNumCode(code, totalItemCount);
           DataTable dt = CommonLibrary.Common.Utilities.CollectionsUtil.ConvertTo(lst);
           
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i]["DateCreated"].ToString()))
                    {
                        DateTime _strCreateDate = Convert.ToDateTime(dt.Rows[i]["DateCreated"].ToString());
                        strCreateDate = "<span class='date'>(" + _strCreateDate.ToString("dd/MM/yy") + ")</span>";
                    }

                    if (dt.Rows[i]["Alias"].ToString() != alias)
                    {
                        list += "<li><a  target='_self' title=\"" + dt.Rows[i]["Headline"].ToString() + "\" href=\"/tin-chi-tiet/" + dt.Rows[i]["Alias"].ToString() + "/" + code + "\"><span>" 
                            + dt.Rows[i]["Headline"].ToString() + "</span>" + strCreateDate + "</a></li>";
                    }
                }
            }

            ltrRelationNews.Text = "<div class=\"detail-related\">"
                                   + "<ul>"
                                   + list
                                   + "</ul>"
                               + "</div>";
        }

        private void PopulateEntryContent(string _alias, string _code)
        {
            string strHTML = string.Empty, strCreateDate = string.Empty;

            ArticleController article_obj = new ArticleController();
            DataTable dt = article_obj.GetDetailByCodeAlias(_alias, _code);            
            if (dt.Rows.Count > 0)
            {
                string CultureCode = dt.Rows[0]["CultureCode"].ToString().Trim();
                string CategoryName = dt.Rows[0]["CategoryName"].ToString();
                string Category_Alias = dt.Rows[0]["Category_Alias"].ToString();
                string Title = dt.Rows[0]["Title"].ToString();
                string Abstract = dt.Rows[0]["Abstract"].ToString();
                string Headline = dt.Rows[0]["Headline"].ToString();
                string MainText = dt.Rows[0]["MainText"].ToString();
                string Source = dt.Rows[0]["Source"].ToString();
                string Tags = dt.Rows[0]["Tags"].ToString();
                

                if (!string.IsNullOrEmpty(dt.Rows[0]["DateCreated"].ToString()))
                {
                    DateTime _strCreateDate = Convert.ToDateTime(dt.Rows[0]["DateCreated"].ToString());
                    strCreateDate = _strCreateDate.ToString("dd/MM/yyyy");
                }

                string Category_Header = GetCategoryNavigation(_code, CultureCode, "2");
               
                strHTML = Category_Header
                            + "<div class=\"entry\">"
                            + "<h1 class=\"entry-title\">" + Headline + "</h1>"
                            + "<div class=\"detail-cal\">Ngày đăng: "+ strCreateDate + "</div>"
                            + "<h2 class=\"entry-sapo\">" + Abstract + "</h2>"
                            + "<div class=\"main-detail-content\">" + MainText + "</div>"
                            + "<div class=\"author\">Nguồn: " + Source + "</div>"
                            + "<div class=\"ct-tags\"><span class='tag_heading'>Tags:</span><a href=\"/tin-tuc/\" target='_self'>" + Tags + "</a></div>"                          
                            + "<div class=\"clear\"></div>"
                            + "</div>";
            }
            else
            {
                strHTML = "Không có dữ liệu";
            }
            LiteralEntry.Text = strHTML;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                if (hiddenCaptcha.Value != null)
                {
                    if (txtCaptcha.Text == hiddenCaptcha.Value)
                    {
                        ArticleController article_obj=new ArticleController();
                        DataTable dt=article_obj.GetDetailByCodeAlias(ViewState["alias"].ToString(),ViewState["code"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            int articleid = Convert.ToInt32(dt.Rows[0]["ArticleId"].ToString());
                            //string userid = null;
                            string name = txtName.Text;
                            string email = txtEmail.Text;
                            //string comment_text = txtComment.Value;                            
                            string comment_text = Server.HtmlEncode(txtComments.Text);
                            int is_reply = 1;
                            string publish = "2";
                            ArticleCommentController comment_obj = new ArticleCommentController();
                            int i = comment_obj.Insert(articleid, name, comment_text, email, is_reply, publish);
                            if (i == 1)
                            {
                                string scriptCode = "<script>alert('Thông điệp đã được gửi đi.');window.location.href='/trang-chu/';</script>";
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                            }
                            else if (i == -1)
                            {
                                string scriptCode = "<script>alert('Thông tin không đủ.');</script>";
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                            }
                            else
                            {
                                string scriptCode = "<script>alert('Tiến trình gửi bị lỗi.');</script>";
                                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                            }
                        }
                    }
                    else if (txtCaptcha.Text != hiddenCaptcha.Value)
                    {
                        lblResult.Text = "Vui lòng thử lại lần nữa &nbsp;<div style=\"float:left; background:url(images/icons/OK_not_OK_Icons.png);background-position:100%;height:30px; width:30px\"></div>";
                        txtCaptcha.Text = "";
                    }
                }
            }
        }
    }
}