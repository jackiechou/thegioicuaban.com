using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Services.Banner;
using System.Data;

namespace WebApp.portals.news
{
    public partial class banner_rotator_demo : System.Web.UI.Page
    {
        private string banner_dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"];
        private string thumb_banner_dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"] + "/thumb_images";
        private string main_banner_dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"] + "/main_images";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadBannerList();
            }
        }

        public void LoadBannerList()
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
                                +"<a href=\""+MainFilePath+"\" title=\"Sexy Surf\"><img src=\""+ThumbFilePath+"\" /></a>"
                              +"</li>";

                    //if (i < dt.Rows.Count - 1)
                    //      result += "<img src=\"" + FilePath + "\" alt=\"" + Title + "\"  />";
                    //else
                    //    result += "<img src=\"" + FilePath + "\" alt=\"" + Title + "\"  />";
                }
            }
            else
                result = "Không có dữ liệu";
            Literal_TopBanner.Text = "<ul>" + result+"</ul>";
        }
    }
}