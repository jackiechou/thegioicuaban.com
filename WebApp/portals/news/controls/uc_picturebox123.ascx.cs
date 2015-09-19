using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules;
using GalleryLibrary;
using System.Data;

namespace WebApp.portals.news.controls
{
    public partial class uc_picturebox123 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PopulateData();
            }
        }

        protected void PopulateData()
        {
            string result = string.Empty;
            string strHTML = string.Empty;

            GalleryCollection gallery_collection_obj = new GalleryCollection();
            List<CustomCollectionFiles> lst = gallery_collection_obj.GetCollectionListForMaster(2, '1');
            DataTable dt = LinqHelper.ToDataTable(lst);
            if (dt.Rows.Count > 0)
            {

                string path_image = "/user_files/images/gallery_images/collection/";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    result += "<li style=\"width: 170px;\" jcarouselindex=\"1\" class=\"jcarousel-item jcarousel-item-horizontal jcarousel-item-1 jcarousel-item-1-horizontal\">"
                                + "<a href='' title='" + dt.Rows[i]["Title"].ToString() + "'>"
                                + "<img class=\"imgPortal\" src='" + path_image + dt.Rows[i]["IconFile"].ToString() + "' alt='" + dt.Rows[i]["Title"].ToString() + "' title='" + dt.Rows[i]["Title"].ToString() + "' style=\"width: 150px;height: 104px;\">"
                                + "</a>"
                                + "<p class=\"titleBox\">"
                                  + " <a href='' title='" + dt.Rows[i]["Title"].ToString() + "' >" + dt.Rows[i]["Title"].ToString() + "</a>"
                                + "</p>"
                                + "</li>";
                }
                strHTML = "<div style=\"display: block;\" class=\"jcarousel-container jcarousel-container-horizontal\">"
                                            + "<div class=\"jcarousel-clip jcarousel-clip-horizontal\">"
                                                + "<ul  id=\"ulCarousel\" style=\"width: 1800px; left: -1200.66px;\" class=\"jcarousel-list jcarousel-list-horizontal\">"
                                                    + result
                                                + "</ul>"
                                            + "</div>"
                                        + "</div>";
                divPictuerBox.InnerHtml = strHTML;
            }
        }
    }
}