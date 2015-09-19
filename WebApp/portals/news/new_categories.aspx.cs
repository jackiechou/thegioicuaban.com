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
    public partial class new_categories : System.Web.UI.Page
    {
        private string _code
        {
            get
            {
                if (ViewState["code"] == null)
                    ViewState["code"] = -1;
                return (string)ViewState["code"];
            }
            set
            {
                ViewState["code"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Page.RouteData.Values.Count > 0)
                {
                    _code = Page.RouteData.Values["code"].ToString();
                    if (_code != null || _code.ToString() != string.Empty)
                    {
                        GetEventList(_code);
                    }
                }
            }
        }

        private void GetEventList(string code)
        {
            int record = 10;
            string strHTML = string.Empty;
            string path_image = "/user_files/images/article_images/front_images/";
           
            ArticleController article_obj = new ArticleController();
            DataTable dt = article_obj.GetListByNumCode(code, record);
            if (dt.Rows.Count > 0)
            {
                ltrCateName.Text = code;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string img = string.Empty;
                    if (dt.Rows[i]["FrontImage"].ToString() != "")
                        img = path_image + dt.Rows[i]["FrontImage"].ToString();
                    else
                        img = "/images/no_image.jpg";

                    strHTML += "<div class=\"row_item\">"
                                + "<h3><a title=\"" + dt.Rows[i]["Alias"].ToString() + "\" href=\"/tin-chi-tiet/" + dt.Rows[i]["Alias"].ToString() + "/" + code + "\"><span class='tick_header'></span>" + dt.Rows[i]["Title"].ToString() + "</h3>"
                                + "<div class=\"clear\"></div>"
                                + "<div class=\"item_thumb\"><a title=\"" + dt.Rows[i]["Alias"].ToString() + "\" href=\"/tin-chi-tiet/" + dt.Rows[i]["Alias"].ToString() + "/" + code + "\">"
                                + "<img height=\"150px\" width=\"140px\" alt=\"" + dt.Rows[i]["Alias"].ToString() + "\" src='" + img + "' /></a></div>"
                                + "<div class=\"item_abstract\">" + dt.Rows[i]["Abstract"].ToString() + "</div>"                            
                                + "<div class=\"clear\"></div>"
                                + "</div>";
                }
                ltrNewCode.Text = strHTML;
                ltrCateName.Text = dt.Rows[0]["CategoryName"].ToString();
            }
        }
    }
}