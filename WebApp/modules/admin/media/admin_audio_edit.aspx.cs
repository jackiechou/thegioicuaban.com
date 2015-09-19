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
    public partial class admin_audio_edit : System.Web.UI.Page
    {
        private static string upload_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"];
        private static string upload_front_image_dir = "~/" + upload_image_dir + "/media_images/audio_images/front_images";
        private static string upload_main_image_dir = "~/" + upload_image_dir + "/media_images/audio_images/main_images";

        MediaFiles media_obj = new MediaFiles();

        private int _idx
        {
            get
            {
                if (ViewState["idx"] == null)
                    ViewState["idx"] = -1;
                return (int)ViewState["idx"];
            }
            set
            {
                ViewState["idx"] = value;
            }
        }

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "AUDIO";
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
                string qsuimode = Request.QueryString["mode"];

                if (string.IsNullOrEmpty(qsuimode) == false)
                {
                    _idx = Convert.ToInt32(Request.QueryString["idx"]);
                    LoadData();
                }
                Session["FileUpload1"] = null;
                MultiView1.ActiveViewIndex = 0;
            }
            else
            {
                if (FileUpload1.HasFile)
                {
                    Session["FileUpload1"] = FileUpload1;
                }
                else if (Session["FileUpload1"] != null)
                {
                    FileUpload1 = (FileUpload)Session["FileUpload1"];
                }
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Media Albums ======================================================
        protected void PopulateMediaAlbumList2DDL(string selected_value)
        {
            List<Media_Albums> lst = MediaAlbums.GetListByStatus("1");

            ddlAlbumList.Items.Clear();
            ddlAlbumList.DataSource = lst;
            ddlAlbumList.DataTextField = "AlbumName";
            ddlAlbumList.DataValueField = "AlbumId";
            ddlAlbumList.DataBind();
            ddlAlbumList.SelectedValue = selected_value;
            ddlAlbumList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region Media Composers ======================================================
        protected void PopulateMediaComposerList2DDL(string selected_value)
        {
            List<Media_Composers> lst = MediaComposers.GetListByStatus("1");

            ddlComposerList.Items.Clear();
            ddlComposerList.DataSource = lst;
            ddlComposerList.DataTextField = "ComposerName";
            ddlComposerList.DataValueField = "ComposerId";
            ddlComposerList.DataBind();
            ddlComposerList.SelectedValue = selected_value;
            ddlComposerList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region Media Artists ======================================================
        protected void PopulateMediaArtistList2DDL(string selected_value)
        {
            List<Media_Artists> lst = MediaArtists.GetListByStatus("1");

            ddlArtistList.Items.Clear();
            ddlArtistList.DataSource = lst;
            ddlArtistList.DataTextField = "ArtistName";
            ddlArtistList.DataValueField = "ArtistId";
            ddlArtistList.DataBind();
            ddlArtistList.SelectedValue = selected_value;
            ddlArtistList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region Media PlayLists ======================================================
        protected void PopulateMediaPlayList2DDL(string selected_value)
        {
            MediaPlayLists media_playlist_obj = new MediaPlayLists();
            List<Media_PlayLists> lst = media_playlist_obj.GetListByStatus("1");

            ddlPlayList.Items.Clear();
            ddlPlayList.DataSource = lst;
            ddlPlayList.DataTextField = "PlayListName";
            ddlPlayList.DataValueField = "PlayListId";
            ddlPlayList.DataBind();
            ddlPlayList.SelectedValue = selected_value;
            ddlPlayList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region Media Types =====================================================
        protected void PopulateMediaTypeList2DDL(string selected_value)
        {
            MediaTypes media_type_obj = new MediaTypes();
            List<Media_Types> lst = media_type_obj.GetListByStatus("1");

            ddlMediaTypeList.Items.Clear();
            ddlMediaTypeList.DataSource = lst;
            ddlMediaTypeList.DataTextField = "TypeName";
            ddlMediaTypeList.DataValueField = "TypeId";
            ddlMediaTypeList.DataBind();
            ddlMediaTypeList.SelectedValue = selected_value;
            ddlMediaTypeList.Enabled = false;
        }
        #endregion ============================================================

        #region Media Topics ======================================================
        protected void PopulateMediaTopicList2DDL(string selected_value)
        {
            MediaTopics media_topic_obj = new MediaTopics();
            List<Media_Topics> lst = media_topic_obj.GetListByTypeStatus(1, "1");

            ddlMediaTopicList.Items.Clear();
            ddlMediaTopicList.DataSource = lst;
            ddlMediaTopicList.DataTextField = "Name";
            ddlMediaTopicList.DataValueField = "TopicId";
            ddlMediaTopicList.DataBind();
            ddlMediaTopicList.SelectedValue = selected_value;
        }
        #endregion ============================================================

        private void LoadData()
        {
            MediaFiles media_obj = new MediaFiles();
            DataTable dt = media_obj.GetDetailById(_idx);
                        
            ViewState["TopicId"] = dt.Rows[0]["TopicId"].ToString();
            string TypeId = dt.Rows[0]["TypeId"].ToString();
            string Title = dt.Rows[0]["Title"].ToString();
            string Description = dt.Rows[0]["Description"].ToString();
            ViewState["FileName"] = dt.Rows[0]["FileName"].ToString();
            ViewState["FileUrl"] = dt.Rows[0]["FileUrl"].ToString();
            string Source = dt.Rows[0]["Source"].ToString();
            ViewState["Photo"] = dt.Rows[0]["Photo"].ToString();
            ViewState["Thumbnail"] = dt.Rows[0]["Thumbnail"].ToString();
            imgPhoto.ImageUrl = upload_front_image_dir + "/" + ViewState["Thumbnail"].ToString();
            string AutoStart = dt.Rows[0]["AutoStart"].ToString();
            string MediaLoop = dt.Rows[0]["MediaLoop"].ToString();
            string Status = dt.Rows[0]["Status"].ToString();
            string PlayListId = dt.Rows[0]["PlayListId"].ToString();
            string AlbumId = dt.Rows[0]["AlbumId"].ToString();
            string ArtistId = dt.Rows[0]["ArtistId"].ToString();
            string ComposerId = dt.Rows[0]["ComposerId"].ToString();
            
            PopulateMediaTopicList2DDL(ViewState["TopicId"].ToString());
            PopulateMediaTypeList2DDL(TypeId);
            PopulateMediaPlayList2DDL(PlayListId);
            PopulateMediaAlbumList2DDL(AlbumId);
            PopulateMediaArtistList2DDL(ArtistId);
            PopulateMediaComposerList2DDL(ComposerId);

            if (AutoStart == "True")
                chkAutoStart.Checked = true;
            if (MediaLoop == "True")
                chkMedialoop.Checked = true;
            if (Status == "1")
                chkIsFilePublished.Checked = true;           
            txtSource.Text = Source;
            txtFileTitle.Text = Title;
            txtDescription.Value = Description;
            txtFileId.Value = _idx.ToString();
        }

        public String[] GetArray(String str)
        {
            String[] Keyword = str.Split(',');
            return Keyword;
        }

        private int UpdateData()
        {
            string userid = Session["UserId"].ToString();
            int typeid = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            int topicid = Convert.ToInt32(ddlMediaTopicList.SelectedValue);
            int albumid = Convert.ToInt32(ddlAlbumList.SelectedValue);
            int artistid = Convert.ToInt32(ddlArtistList.SelectedValue);
            int composerid = Convert.ToInt32(ddlComposerList.SelectedValue);
            int playlistid = Convert.ToInt32(ddlPlayList.SelectedValue);
            string title = txtFileTitle.Text;
            string description = txtDescription.Value;
            string dimension =string.Empty;
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

            string Orginal_front_image = ViewState["Thumbnail"].ToString();
            string Orginal_main_image = ViewState["Photo"].ToString();

            //if (FileUpload1.HasFile)
            if (Session["FileUpload1"] != null && Session["FileUpload1"].ToString() != string.Empty)
            {
                FileHandleClass file_bj = new FileHandleClass();
                FileImg = file_bj.upload_front_main_image(FileUpload1, front_path, main_path, 120, 120);
                main_image = FileImg[0];
                front_image = FileImg[1];
                //System.Drawing.Image img1 = System.Drawing.Image.FromFile(front_path+ "/" + front_image);                
                imgPhoto.ImageUrl = upload_front_image_dir + "/" + front_image;
                DeleteFrontImage(Orginal_front_image);
                DeleteMainImage(Orginal_main_image);
            }
            else
            {
                front_image = Orginal_front_image;
                main_image = Orginal_main_image;
            }
            ////========================================================================================================================

            /*** UPLOAD FILE*************************************************************************************************************/
            string filename = string.Empty; string fileurl = string.Empty, file_ext = string.Empty;
            string Orginal_File = ViewState["FileName"].ToString();
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTypes media_type_obj = new MediaTypes();
            Media_Types type_obj = media_type_obj.GetDetails(TypeId);
            string TypeExt = type_obj.TypeExt.Trim();
            string TypePath = type_obj.TypePath;
            string dir_path = Server.MapPath("~/" + TypePath + "/" + topicid);
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
                            fileurl = string.Empty;
                            DeleteMediaFile(Orginal_File);
                        }
                    }
                }
                else
                    filename = ViewState["FileName"].ToString();
            }
            else if (rdlFileUpload.SelectedValue == "1")
            {
                file_ext = System.IO.Path.GetExtension(txtFileUrl.Text).ToLower().Trim();
                filename = System.IO.Path.GetFileName(txtFileUrl.Text);
                for (int i = 0; i < arr_list.Length; i++)
                {
                    if (file_ext == arr_list[i].ToString())
                    {
                        fileurl = txtFileUrl.Text;
                        filename = string.Empty;
                    }
                }
                if (fileurl.Length <= 0)
                {
                    fileurl = ViewState["FileUrl"].ToString();
                }
            }
            if (filename == string.Empty && fileurl == string.Empty)
            {
                filename = ViewState["FileName"].ToString();
                fileurl = ViewState["FileUrl"].ToString();
            }
            /************************************************************************************************************************/

            MediaFiles media_obj = new MediaFiles();
            int relust = media_obj.Update(_idx, userid, typeid, topicid, playlistid, albumid, artistid, composerid, filename, fileurl, title, description,
                           autostart, medialoop, dimension, source, main_image, front_image, status);
            return relust;
        }

        private void DeleteMediaFile(string file_name)
        {
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTypes media_type_obj = new MediaTypes();
            Media_Types type_obj = media_type_obj.GetDetails(TypeId);
            string TypePath = type_obj.TypePath;
            string dir_path = Server.MapPath("~" + TypePath + "/" + ViewState["TopicId"].ToString());
            if (Directory.Exists(dir_path))
            {
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_handle_obj.deleteFile(file_name, dir_path);
            }
        }

        public static string[] getDirectionFromFile(string file)
        {
            string[] result = new string[2];
            string filename = string.Empty, direction = string.Empty;

            if (file.IndexOf("/") > -1)
            {
                result[0] = file.Substring(0, file.LastIndexOf("/"));//direction 
                result[1] = file.Remove(0, file.LastIndexOf("/")).Trim('/');//filename
            }
            else
            {
                result[0] = "";//direction 
                result[1] = file;//filename
            }

            return result;
        }

        private void DeleteFrontImage(string Orginal_front_image)
        {
            string[] result = new string[2];
            result = getDirectionFromFile(Orginal_front_image);
            string front_image = upload_front_image_dir + "/" + result[0];
            string filename = result[1].ToString();
            string _front_dir_img_path = Server.MapPath(front_image);
            if (Directory.Exists(_front_dir_img_path))
            {
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_handle_obj.deleteFile(filename, _front_dir_img_path);
            }
        }

        private void DeleteMainImage(string Orginal_main_image)
        {
            string[] result = new string[2];
            result = getDirectionFromFile(Orginal_main_image);
            string main_image = upload_main_image_dir + "/" + result[0];
            string filename = result[1].ToString();
            string _main_dir_img_path = Server.MapPath(main_image);
            if (Directory.Exists(_main_dir_img_path))
            {
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_handle_obj.deleteFile(filename, _main_dir_img_path);
            }
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int i = 0;
                i = UpdateData();
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
                    MultiView1.ActiveViewIndex = 2;
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