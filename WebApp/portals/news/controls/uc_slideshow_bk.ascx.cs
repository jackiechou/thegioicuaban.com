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
    public partial class uc_slideshow_bk : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               // LoadSlideData();
            }
        }

        //private void LoadSlideData()
        //{            
        //    //string code = "HOT_NEWS";
        //    string code = "DAILY_NEWS";
        //    int record = 8;
        //    string strHTML = string.Empty;
        //    string Slide_01 = string.Empty, Slide_02 = string.Empty;
        //    string path_front_image = "/user_files/images/article_images/front_images/";
        //    ArticleController article_obj = new ArticleController();
        //    DataTable dt = article_obj.GetListByNumCode(code, record);
        //    if (dt.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            Slide_01 += "<div class='item'>"
        //                            + "<a href='#'>"
        //                            +"<img height='96' width='132' alt=\"" + dt.Rows[i]["Alias"].ToString() + "\" src='" + path_front_image + dt.Rows[i]["FrontImage"].ToString() + "' />"
        //                            + dt.Rows[i]["TiTle"].ToString() + "</a>"
        //                        +"</div>";
        //        }

        //        for (int i = 4; i < dt.Rows.Count; i++)
        //        {
        //            Slide_02 += "<div class='item'>"
        //                            + "<a href='#'>"
        //                            + "<img height='96' width='132' alt=\"" + dt.Rows[i]["Alias"].ToString() + "\" src='" + path_front_image + dt.Rows[i]["FrontImage"].ToString() + "' />"
        //                            + dt.Rows[i]["TiTle"].ToString() + "</a>"
        //                        + "</div>";
        //        }
                
        //        //ltrSlide_02.Text = Slide_02;
        //        strHTML = "<div class=\"slide\">"
        //                        + Slide_01
        //                    + "</div>"
        //                    + "<div class=\"slide\" style=\"float: left; width: 980px;\">"
        //                        + Slide_02
        //                    + "</div>";
        //    }
        //    ltrSlide.Text = strHTML;
        //}
    }
}