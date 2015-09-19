using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Modules;
using System.IO;
using MediaLibrary;

namespace WebApp.modules.admin.media
{
    public partial class admin_video_add : System.Web.UI.Page
    {
        private static string upload_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"];
        private static string upload_front_image_dir = "~/" + upload_image_dir + "/media_images/video_images/front_images";
        private static string upload_main_image_dir = "~/" + upload_image_dir + "/media_images/video_images/main_images";
        MediaFiles media_obj = new MediaFiles();

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "VIDEO";
            Page.Theme = "default";
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            Page.Culture = "vi-VN";
            Page.UICulture = "vi";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {                
                PopulateMediaTypeList2DDL();
                PopulateMediaTopicList2DDL();
                PopulateMediaAlbumList2DDL();
                PopulateMediaArtistList2DDL();
                PopulateMediaComposerList2DDL();
                PopulateMediaPlayList2DDL();
                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }


        #region Media Composers ======================================================
        protected void PopulateMediaComposerList2DDL()
        {
            List<Media_Composers> lst = MediaComposers.GetListByStatus("1");

            ddlComposerList.Items.Clear();
            ddlComposerList.DataSource = lst;
            ddlComposerList.DataTextField = "ComposerName";
            ddlComposerList.DataValueField = "ComposerId";
            ddlComposerList.DataBind();
            ddlComposerList.SelectedValue = "1";
            ddlComposerList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region Media Artists ======================================================
        protected void PopulateMediaArtistList2DDL()
        {
            List<Media_Artists> lst = MediaArtists.GetListByStatus("1");

            ddlArtistList.Items.Clear();
            ddlArtistList.DataSource = lst;
            ddlArtistList.DataTextField = "ArtistName";
            ddlArtistList.DataValueField = "ArtistId";
            ddlArtistList.DataBind();
            ddlComposerList.SelectedValue = "1";
            ddlArtistList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region Media Albums ======================================================
        protected void PopulateMediaAlbumList2DDL()
        {
            List<Media_Albums> lst = MediaAlbums.GetListByStatus("1");

            ddlAlbumList.Items.Clear();
            ddlAlbumList.DataSource = lst;
            ddlAlbumList.DataTextField = "AlbumName";
            ddlAlbumList.DataValueField = "AlbumId";
            ddlAlbumList.DataBind();
            ddlAlbumList.SelectedValue = "1";
            ddlAlbumList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region Media PlayLists ======================================================
        protected void PopulateMediaPlayList2DDL()
        {
            MediaPlayLists media_playlist_obj = new MediaPlayLists();
            List<Media_PlayLists> lst = media_playlist_obj.GetListByStatus("1");

            ddlPlayList.Items.Clear();
            ddlPlayList.DataSource = lst;
            ddlPlayList.DataTextField = "PlayListName";
            ddlPlayList.DataValueField = "PlayListId";
            ddlPlayList.DataBind();
            ddlPlayList.SelectedValue = "1";
            ddlPlayList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region Media Types ======================================================
        protected void PopulateMediaTypeList2DDL()
        {
            MediaTypes media_type_obj = new MediaTypes();
            List<Media_Types> lst = media_type_obj.GetListByStatus("1");

            ddlMediaTypeList.Items.Clear();
            ddlMediaTypeList.DataSource = lst;
            ddlMediaTypeList.DataTextField = "TypeName";
            ddlMediaTypeList.DataValueField = "TypeId";
            ddlMediaTypeList.DataBind();
            ddlMediaTypeList.SelectedValue = "2";
            ddlMediaTypeList.AutoPostBack = true;
            ddlMediaTypeList.Enabled = false;
        }
        #endregion ============================================================

        #region Media Topics ======================================================
        protected void PopulateMediaTopicList2DDL()
        {
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTopics media_topic_obj = new MediaTopics();
            List<Media_Topics> lst = media_topic_obj.GetListByTypeStatus(TypeId, "1");

            ddlMediaTopicList.Items.Clear();
            ddlMediaTopicList.DataSource = lst;
            ddlMediaTopicList.DataTextField = "Name";
            ddlMediaTopicList.DataValueField = "TopicId";
            ddlMediaTopicList.DataBind();
            ddlMediaTopicList.SelectedIndex = 0;
            ddlMediaTopicList.AutoPostBack = true;
        }
        #endregion ============================================================
        
        private int AddData()
        {
            string userid = Session["UserId"].ToString();
            int vendorid = Convert.ToInt32(Session["VendorId"].ToString());
            int typeid = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            int topicid = Convert.ToInt32(ddlMediaTopicList.SelectedValue);
            int albumid = Convert.ToInt32(ddlAlbumList.SelectedValue);
            int artistid = Convert.ToInt32(ddlArtistList.SelectedValue);
            int composerid = Convert.ToInt32(ddlComposerList.SelectedValue);
            int playlistid = Convert.ToInt32(ddlPlayList.SelectedValue);
            string title = txtFileTitle.Text;
            string description = txtDescription.Value;
            string dimension = txtDimension.Text;
            string source = txtSource.Text;
            string status = "0";
            int autostart = 0;
            int medialoop = 0;

            if (chkAutoStart.Checked == true)
                autostart = 1;
            if (chkMedialoop.Checked == true)
                medialoop = 1;
            if (chkIsFilePublished.Checked == true)
                status = "1";
            /*** UPLOAD ************************************************************************************************************/
            string[] FileImg = new String[2];
            string front_image = string.Empty; string main_image = string.Empty;
            string front_path = Server.MapPath(upload_front_image_dir);
            string main_path = Server.MapPath(upload_main_image_dir);

            if (FileUpload1.HasFile)
            {
                FileHandleClass file_bj = new FileHandleClass();
                FileImg = file_bj.upload_front_main_image(FileUpload1, front_path, main_path, 120, 120);
                main_image = FileImg[0];
                front_image = FileImg[1];
                //System.Drawing.Image img1 = System.Drawing.Image.FromFile(front_path+ "/" + front_image);                
                //imgPhoto.ImageUrl = upload_front_image_dir + "/" + front_image;
            }
            ////========================================================================================================================

            /*** UPLOAD FILE*************************************************************************************************************/
            string filename = string.Empty; string fileurl = string.Empty, file_ext = string.Empty;
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTypes media_type_obj = new MediaTypes();
            Media_Types type_obj = media_type_obj.GetDetails(TypeId);
            string TypeExt = type_obj.TypeExt;
            string TypePath = type_obj.TypePath;
            string dir_path = Server.MapPath("~" + TypePath + "/" + topicid);
            string[] arr_list = GetArray(TypeExt);

            if (rdlFileUpload.SelectedValue == "0")
            {
                HttpPostedFile posted_file = File1.PostedFile;
                if (posted_file.ContentLength > 0)
                {
                    file_ext = System.IO.Path.GetExtension(posted_file.FileName).ToLower().Trim();
                    if (!Directory.Exists(dir_path))
                        Directory.CreateDirectory(dir_path);

                    for (int i = 0; i < arr_list.Length; i++)
                    {
                        if (file_ext == arr_list[i].ToString())
                        {
                            FileHandleClass file_obj = new FileHandleClass();
                            filename = file_obj.uploadInputFile(posted_file, dir_path);
                        }
                    }
                }
            }
            else
            {
                file_ext = System.IO.Path.GetExtension(txtFileUrl.Text).ToLower().Trim();
                filename = System.IO.Path.GetFileName(txtFileUrl.Text);
                for (int i = 0; i < arr_list.Length; i++)
                {
                    if (file_ext == arr_list[i].ToString())
                    {
                        fileurl = txtFileUrl.Text;
                    }
                }
            }
            /************************************************************************************************************************/

            MediaFiles media_obj = new MediaFiles();
            int relust = media_obj.Insert(userid, vendorid, typeid, topicid, playlistid, albumid, artistid, composerid, filename, fileurl, title, description,
                           autostart, medialoop, dimension, source, main_image, front_image, status);
            return relust;
        }

        public String[] GetArray(String str)
        {
            String[] Keyword = str.Split(',');
            return Keyword;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int i = 0;
                i = AddData();
                if (i == -1)
                {
                    lblErrorMsg.Text = "Thông tin không đầy đủ";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 0;
                }
                else if (i == -2)
                {
                    lblErrorMsg.Text = "Tiến trình xử lý bị lỗi";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 2;
                }
                else if (i == -3)
                {
                    lblErrorMsg.Text = "Dữ liệu đã tồn tại";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 1;
                }
                else if (i == 1)
                {
                    lblResult.Text = "Cập nhật thành công";
                    MultiView1.ActiveViewIndex = 1;
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 2;
                }
            }
        }
    }
}