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
    public partial class demo_topbanner : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadBannerList();
            }
        }

        public void LoadBannerList()
        {
            BannerController banner_obj = new BannerController();
            int num_rows = 3; int position = 1; string status = "1"; string result = string.Empty, Title = string.Empty, FilePath = string.Empty;
            Uri requestUri = Context.Request.Url;
            string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
            DataTable dt = banner_obj.GetListByNumPosition(num_rows, position, status);
            if (dt.Rows.Count > 0)
            { 
                for (int i = 0; i < dt.Rows.Count; i++)
                {                   
                    FilePath = baseUrl + "/user_files/images/banner_images/" + dt.Rows[i]["FileName"].ToString();
                    Title = dt.Rows[i]["Title"].ToString();
                    result += "<img src=\"" + FilePath + "\" alt=\"" + Title + "\"  />";
                    //if (i < dt.Rows.Count - 1)
                    //      result += "<img src=\"" + FilePath + "\" alt=\"" + Title + "\"  />";
                    //else
                    //    result += "<img src=\"" + FilePath + "\" alt=\"" + Title + "\"  />";
                }
            }
            else
                result = "Không có dữ liệu";
            Literal_TopBanner.Text = result;
        }
    }
}