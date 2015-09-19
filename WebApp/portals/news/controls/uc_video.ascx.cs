using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaLibrary;

namespace WebApp.portals.news.controls
{
    public partial class uc_video : System.Web.UI.UserControl
    {
        private static string baseUrl = CommonLibrary.Common.Utilities.UrlUtils.BaseSiteUrl;
        private static string upload_media_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_media_image_dir"];
        private static string upload_video_dir = System.Configuration.ConfigurationManager.AppSettings["upload_video_dir"];
        private static string media_player_path = "portals/news/user_files/media/video/mediaplayer.swf";


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadVideo();
            }
        }
        private void LoadVideo()
        {            
            string strHTML = string.Empty, firstItem = "", otherItems = string.Empty;
           
            List<Media_Files> items = MediaFiles.GetActiveListByTypeIdFixedNum(4,5);

            if (items.Count > 0)
            {
                //items.Select((item, index) => index == 0 ? GetResultFromFirstItem(item) : GetResultFromOtherItem(items));
                foreach (var item in items)
                {
                    if (items.First() == item)
                        firstItem = GetResultFromFirstItem(item);

                    foreach (var _item in items.Skip(1))
                    {
                        otherItems += GetResultFromOtherItem(_item);
                    }
                }

                strHTML = "<div id=\"playvideoitem\">" + firstItem + "</div>"
                       + "<div class=\"others\">" + otherItems + "</div>";
            }
            else
                strHTML = "Không tìm thấy dữ liệu";
           
            lblVideoList.Text = strHTML;                                   
        }

        private string GetResultFromFirstItem(Media_Files item)
        {
            string Title = item.Title;
            string FileName = item.FileName;
            string FileUrl =  item.FileUrl;
            string  Photo =  item.Photo;
            string  Thumbnail=  item.Thumbnail;
            string thumnail_path = string.Empty, video_url=string.Empty;

            if (Thumbnail != string.Empty)
                thumnail_path = baseUrl + "/" + upload_media_image_dir + "/thumbnails/" + Thumbnail;
            else
                thumnail_path = baseUrl + "/images/no_image.jpg";      

             if (FileName != string.Empty)
                    video_url = baseUrl + "/" + upload_video_dir + "/" + FileName;
                else
                    video_url = "Không tìm thấy file trên hệ thống";       


            string firstItem = "<a title=\"" + Title + "\" class=\"title\">" + Title + "</a>"
                           + "<div id=\"play-box\">"
                           + "	<embed height=\"180\" width=\"300\" flashvars=\"width=300&amp;height=180&amp;file=" + video_url
                                       + "&amp;image=" + thumnail_path + "\" wmode=\"transparent\" allowfullscreen=\"true\" allowscriptaccess=\"always\" quality=\"high\" name=\"MediaPlayer\" id=\"MediaPlayer\""
                                       + " src=\"" + media_player_path + "\" type=\"application/x-shockwave-flash\">"
                           + "</div>";    
            return firstItem;
        }

        private string GetResultFromOtherItem(Media_Files item)
        {
            string otherItems = string.Empty, photo_path = string.Empty, thumnail_path = string.Empty, video_url = string.Empty;            
           
            if (item.FileName != string.Empty)
                video_url = baseUrl + "/" + upload_video_dir + "/" + item.FileName;
            else
                video_url = "Không tìm thấy file trên hệ thống";    

            if (item.Photo != string.Empty)
                photo_path = baseUrl + "/" + upload_media_image_dir + "/photo/" + item.Photo;
            else
                photo_path = baseUrl + "/images/no_image.jpg";

            if (item.Thumbnail != string.Empty)
                thumnail_path = baseUrl + "/" + upload_media_image_dir + "/thumbnails/" + item.Thumbnail;
            else
                thumnail_path = baseUrl + "/images/no_image.jpg";  

            otherItems = "<a onclick=\"playVideo('" + photo_path + "','" + video_url + "','" + item.Title + "')\" href=\"javascript:;\" title=\"" + item.Title + "\">"
                    + "<img alt=\"" + item.Title + "\" src=\"" + thumnail_path + "\">"
                    + "</a>";
            return otherItems;
        }

    }
}