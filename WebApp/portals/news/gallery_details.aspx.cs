using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GalleryLibrary;

namespace WebApp.portals.news
{
    public partial class gallery_details : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Page.RouteData.Values.Count > 0)
                {
                    if (Page.RouteData.Values["collectionid"].ToString() != "")
                        PopulateGalleryFileListByCollectionId(Page.RouteData.Values["collectionid"].ToString());
                }
            }
        }

        public void PopulateGalleryFileListByCollectionId(string CollectionId)
        {
            string strHTML = string.Empty;            
            List<CustomGalleryFiles> _lst = GalleryFile.GetList(Convert.ToInt32(CollectionId), '1');
            if (_lst.Count > 0)
            {
                string contents = string.Empty, file_path = string.Empty, file_name = string.Empty;
                Uri requestUri = Context.Request.Url;
                string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
                string path_image = baseUrl + "/" + System.Configuration.ConfigurationManager.AppSettings["upload_gallery_content_image_dir"];
                foreach (var x in _lst)
                {
                    if (!string.IsNullOrEmpty(x.FileName) && x.FileName.Length > 50)
                        file_name = x.FileName.Substring(0, 50) + "...";

                    if (string.IsNullOrEmpty(x.FileUrl))
                        file_path = path_image +"/"+ x.FileName;
                    else
                        file_path = x.FileUrl;

                    contents += "<a class='highslide' onclick=\"return hs.expand(this)\" href='" + file_path + "' title='" + file_name + "'>"
                                     + "<img src='" + file_path + "'  width=\"150px\" height=\"150px\" alt='" + file_name + "' >"
                               + "</a>";
                }
                strHTML = "<div class=\"highslide-gallery\" style=\"width: 600px; margin: auto\">" + contents + "</div><div class=\"clear\"></div>";
            }
            else
                strHTML = "<div class=\"highslide-gallery\" style=\"width: 600px; margin: auto\">Không có dữ liệu</div><div class=\"clear\"></div>";

            divGalleryFileList.InnerHtml= strHTML;
        }
    }
}