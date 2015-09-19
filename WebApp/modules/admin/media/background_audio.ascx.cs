using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MediaLibrary;

namespace WebApp.modules.admin.media
{
    public partial class background_audio : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Literal_SeletedItem.Text = LoadSelectedBackgroundMusic();
            }
        }

        protected string LoadSelectedBackgroundMusic()
        {
            DataTable dt = new DataTable();
            MediaFiles media_obj = new MediaFiles();
            dt = media_obj.GetSelectedItem(1);
            string result = string.Empty;
            if (dt.Rows.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=10,0,0,0\"  width=\"15\" height=\"15\">");
                sb.AppendLine("<param name=\"movie\" value=\"../../scripts/plugins/audioplay_0.9.9/audioplay.swf?file=../../user_files/media/audio/background_audio/" + dt.Rows[0]["FileName"].ToString() + "&buttondir=../../scripts/plugins/audioplay_0.9.9/buttons/negative_small&mode=playpause&auto=no&listenstop=yes&sendstop=yes&repeat=0&mastervol=5&bgcolor=0xffffff\">");
                sb.AppendLine("<param name=\"quality\" value=\"high\">");
                sb.AppendLine("<param name=\"wmode\" value=\"transparent\">");
                sb.AppendLine("<param name=\"allowscriptaccess\" value=\"always\">");
                sb.AppendLine("<embed src=\"../../scripts/plugins/audioplay_0.9.9/audioplay.swf?file=../../user_files/media/audio/background_audio/" + dt.Rows[0]["FileName"].ToString() + "&buttondir=../../scripts/plugins/audioplay_0.9.9/buttons/negative_small&mode=playpause&auto=no&listenstop=yes&sendstop=yes&repeat=0&mastervol=5&bgcolor=0xffffff\" quality=\"high\" wmode=\"transparent\" width=\"15\" height=\"15\" type=\"application/x-shockwave-flash\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\"></embed>");
                sb.AppendLine("</object>");

                result = sb.ToString();
            }
            else
            {
                result = "";
            }
            return result;

        }
    }
}