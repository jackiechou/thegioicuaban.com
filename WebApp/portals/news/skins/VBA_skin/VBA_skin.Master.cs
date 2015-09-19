using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ArticleLibrary;
using System.Data;
using CommonLibrary.Services.Banner;
using CommonLibrary.UI;
namespace WebApp.portals.news.skins.VBA_skin
{
    public partial class VBA_skin : BaseMasterPage
    {
        private string banner_dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"];
        private string thumb_banner_dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"] + "/thumb_images";
        private string main_banner_dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"] + "/main_images";


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadBannerList();
                LoadNewsHeaderSlide();
                LoadLogoInfo();                
                load_adv_left(2);
                load_adv_right(2);
                LoadSlideData("DN");
            }
        }

        private void LoadBannerList()
        {
            int num_rows = 3; int position = 1; string status = "1"; string result = string.Empty, Title = string.Empty,
                ThumbFilePath = string.Empty, MainFilePath = string.Empty;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            BannerController banner_obj = new BannerController();
            DataTable dt = banner_obj.GetListByNumPosition(num_rows, position, status);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ThumbFilePath = baseUrl + "/" + main_banner_dir_path + "/" + dt.Rows[i]["ThumbImage"].ToString();
                    MainFilePath = baseUrl + "/" + main_banner_dir_path + "/" + dt.Rows[i]["MainImage"].ToString();
                    Title = dt.Rows[i]["Title"].ToString();
                    result += "<li data-delay=\"6000\">"
                                + "<a href=\"" + MainFilePath + "\" title=\"Sexy Surf\"><img src=\"" + ThumbFilePath + "\" /></a>"
                              + "</li>";

                    //if (i < dt.Rows.Count - 1)
                    //      result += "<img src=\"" + FilePath + "\" alt=\"" + Title + "\"  />";
                    //else
                    //    result += "<img src=\"" + FilePath + "\" alt=\"" + Title + "\"  />";
                }
            }
            else
                result = "Không có dữ liệu";
            Literal_TopBanner.Text = "<ul>" + result + "</ul>";
        }

        private void LoadNewsHeaderSlide()
        {
            string strHTML = string.Empty, result = string.Empty, link = string.Empty, Source = string.Empty;
            string code = "TT_SK";
            string lang = "vi-VN";
            int totalItemCount = 3;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            string main_image_path = "/user_files/images/article_images/main_images/";
            string file_path = baseUrl + main_image_path;
            List<Article> lst = ArticleController.GetActiveListByFixedNumCode(code, lang, totalItemCount);
            foreach (var item in lst)
            {
                if (string.IsNullOrEmpty(item.Source))
                    Source = Request.Url.Host;
                else
                    Source = item.Source;

                result += "<li class=\"news-item\"><a class=\"news-item-link\" title=\"" + item.Title + "\" href=\"/tin-chi-tiet/" + item.Alias + "/" + code + "\">"
                    + item.Headline
                    + "<span class='author'>Nguồn:" + Source + "</span>"
                    + "</a></li>";
            }
            strHTML = "<ul id=\"news_ticker\" class=\"ticker\">" + result + "</ul>";
            newsticker_scroller.InnerHtml = strHTML;
        }

        private void LoadLogoInfo()
        {
            string strHTML = string.Empty;
            //Vendors Vendors_obj = new Vendors();
            //DataTable dt = Vendors_obj.GetListByPortalId(0);
            //string path_image = "/user_files/images/vendors_images/";
            //if (dt.Rows.Count > 0)
            //{
            //    string VendorName = dt.Rows[0]["VendorName"].ToString();
            //    string Address = "Địa chỉ: " + dt.Rows[0]["Address"].ToString();
            //    string Telephone = "Điện thoại: " + dt.Rows[0]["Telephone"].ToString();
            //    string Hotline = "Hotline: " + dt.Rows[0]["Hotline"].ToString();

            //    ltrLogo.Text = "<a href=\"/trang-chu/\" class=\"fl logo\" id=\"ai-logo\" style=\"background:url(" + path_image + dt.Rows[0]["LogoFile"].ToString() + ")\">"
            //             + VendorName + "</a>"
            //             + "<div id=\"ai-title\">" + VendorName + "</div>"
            //             + "<h1><span class=\"description\">" + dt.Rows[0]["Slogan"].ToString() + "</span></h1>";

            //    ltrVendorName.Text = VendorName + " - " + Address;
            //}
        }

        protected void load_adv_left(int num_rows)
        {
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);

            int position = 4; string status = "1", result = string.Empty, Title = null, MainFilePath = null;
            BannerController banner_obj = new BannerController();       
            DataTable dt = banner_obj.GetListByNumPosition(num_rows, position, status);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows.Count > 0)
                    {
                        Title = dt.Rows[i]["Title"].ToString();
                        MainFilePath = baseUrl + "/" + main_banner_dir_path + "/" + dt.Rows[i]["MainImage"].ToString();
                        result += "<li class=\"news-item\"><a class=\"news-item-link\" href='#'><img src='" + MainFilePath + "' width='709' height='306' alt='" + Title + "' /></a></li>";
                    }
                }
            }
            else
            {
                result += "<li class=\"news-item\">Không có dữ liệu</li>";
            }
            adv_left.InnerHtml = "<ul id=\"adv_left_inner\" class=\"ticker\">" + result + "</ul>";

        }

        protected void load_adv_right(int num_rows)
        {            
            int position = 5; 
            string status = "1", result = string.Empty, Title = null, MainFilePath = null;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            BannerController banner_obj = new BannerController();
            DataTable dt = banner_obj.GetListByNumPosition(num_rows, position, status);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MainFilePath = baseUrl + "/" + main_banner_dir_path + "/" + dt.Rows[i]["MainImage"].ToString();
                    Title = dt.Rows[i]["Title"].ToString();
                    result += "<li class=\"news-item\"><a class=\"news-item-link\" href='#'><img src='" + MainFilePath + "' width='709' height='306' alt='" + Title + "' /></a></li>";
                }
            }else
                result = "<li class=\"news-item\">Đang Cập Nhật</li>";
            adv_right.InnerHtml = "<ul id=\"adv_right_inner\" class=\"ticker\">" + result + "</ul>";

        }
        protected void LoadSlideData(string code)
        {
            string strHTML = string.Empty, Alias = string.Empty, Slide = string.Empty, FrontImagePath = string.Empty, Headline = string.Empty, NavigateUrl= string.Empty;
            string path_front_image = "/user_files/images/article_images/front_images/";
            ArticleController article_obj = new ArticleController();
            DataTable dt = article_obj.GetListByCode(code);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Alias = dt.Rows[i]["Alias"].ToString();

                    if (!string.IsNullOrEmpty(dt.Rows[i]["NavigateUrl"].ToString()))
                        NavigateUrl = dt.Rows[i]["NavigateUrl"].ToString();
                    else
                        NavigateUrl = "#";
                    
                    if (!string.IsNullOrEmpty(dt.Rows[i]["FrontImage"].ToString()))
                        FrontImagePath = path_front_image + dt.Rows[i]["FrontImage"].ToString();
                    else
                        FrontImagePath = "/images/no_image.jpg";

                    if(!string.IsNullOrEmpty(dt.Rows[i]["Headline"].ToString()))
                    {
                        if (dt.Rows[i]["Headline"].ToString().Length > 20)
                            Headline = dt.Rows[i]["Headline"].ToString().Substring(0, 20);
                        else
                            Headline = dt.Rows[i]["Headline"].ToString();

                         Slide += "<div>"
                                + "<a href='" + NavigateUrl + "'>"
                                + "<img height='94' width='120' alt=\"" + Alias + "\" src='" + FrontImagePath + "' /><br />"
                                + "<span class=\"thumbnail-text\">" + Headline + "</span></a>"
                            + "</div>";
                    }else{
                         Slide += "<div>"
                                + "<a href='" + NavigateUrl + "'>"
                                + "<img height='94' width='120' alt=\"" + Alias + "\" src='" + FrontImagePath + "' /></a>"                                
                            + "</div>";
                    }

                   
                }
                strHTML = "<div id=\"carouselSider\">"
                                + Slide
                            + "</div>";
            }
            carouselSider.InnerHtml = strHTML;
        }
    }
}