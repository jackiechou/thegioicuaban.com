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
using CommonLibrary.Common.Utilities;

namespace WebApp.modules.admin.media
{
    public partial class admin_media_file_edit : System.Web.UI.Page
    {
        private static string upload_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"];
        private static string front_image_dir = "~/" + upload_image_dir + "/media_images/photo";
        private static string main_image_dir = "~/" + upload_image_dir + "/media_images/thumbnails";
        MediaFiles media_obj = new MediaFiles();

        public UIMode.mode _mode
        {
            get
            {
                if (ViewState["mode"] == null)
                    ViewState["mode"] = new UIMode.mode();
                return (UIMode.mode)ViewState["mode"];
            }
            set
            {
                ViewState["mode"] = value;
            }
        }

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
            Page.Title = "5EAGLES";
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
                    _mode = (UIMode.mode)Enum.Parse(typeof(UIMode.mode), qsuimode);
                    if (_mode == UIMode.mode.add)
                    {
                        PopulateMediaTopicList2DDL();
                        PopulateMediaTypeList2DDL();
                    }
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx = Convert.ToInt32(Request.QueryString["idx"]);
                        LoadData();
                    }
                    hdnWindowUIMODE.Value = _mode.ToString();
                }

                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }


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
            ddlMediaTypeList.SelectedIndex = 1;
            ddlMediaTypeList.AutoPostBack = true;
        }
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
        private void PopulateMediaTopicList2DDL()
        {
            ddlMediaTopicList.Items.Clear(); //DROPDOWNLIST  
            MediaTopics media_topic_obj = new MediaTopics();
            List<Media_Topics> lst = media_topic_obj.GetListByStatus("1");
            if (lst.Count > 0)
            {
                DataTable dt = CommonLibrary.Data.DataUtils.ConvertToDataTable(lst);
                RecursiveFillTree(dt, 0);
            }
            ddlMediaTopicList.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlMediaTopicList.SelectedIndex = 0;

        }

        private void PopulateMediaTopicList2DDL(string select_value)
        {
            ddlMediaTopicList.Items.Clear(); //DROPDOWNLIST  
            MediaTopics media_topic_obj = new MediaTopics();
            List<Media_Topics> lst = media_topic_obj.GetListByStatus("1");
            if (lst.Count > 0)
            {
                DataTable dt = CommonLibrary.Data.DataUtils.ConvertToDataTable(lst);
                RecursiveFillTree(dt, 0);
            }
            ddlMediaTopicList.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlMediaTopicList.SelectedValue = select_value;

        }

        int level = -1;
        private void RecursiveFillTree(DataTable dtParent, int ParentId)
        {
            level++; //on the each call level increment 1
            System.Text.StringBuilder appender = new System.Text.StringBuilder();

            for (int j = 0; j < level; j++)
            {
                appender.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
            }
            if (level > 0)
            {
                appender.Append("|__");
            }

            DataView dv = new DataView(dtParent);
            dv.RowFilter = string.Format("ParentId = {0}", ParentId);

            int i;

            if (dv.Count > 0)
            {
                for (i = 0; i < dv.Count; i++)
                {
                    //DROPDOWNLIST
                    ddlMediaTopicList.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["Name"].ToString()), dv[i]["TopicId"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["TopicId"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }      
        #endregion ============================================================  

        private void LoadData()
        {
            MediaFiles media_obj = new MediaFiles();
            DataTable dt = media_obj.GetDetailById(_idx);

            string TopicId = dt.Rows[0]["TopicId"].ToString();
            string TypeId = dt.Rows[0]["TypeId"].ToString();
            string Title = dt.Rows[0]["Title"].ToString();
            string Dimension = dt.Rows[0]["Dimension"].ToString();
            string Description = dt.Rows[0]["Description"].ToString();
            string FileName = dt.Rows[0]["FileName"].ToString();
            string Source = dt.Rows[0]["Source"].ToString();
            ViewState["Photo"] = dt.Rows[0]["Photo"].ToString();
            ViewState["Thumbnail"] = dt.Rows[0]["Thumbnail"].ToString();
            string AutoStart = dt.Rows[0]["AutoStart"].ToString();
            string MediaLoop = dt.Rows[0]["MediaLoop"].ToString();
            string Status = dt.Rows[0]["Status"].ToString();

            PopulateMediaTopicList2DDL(TopicId);
            PopulateMediaTypeList2DDL(TypeId);

            if (AutoStart == "True")
                chkAutoStart.Checked = true;
            if (MediaLoop == "True")
                chkMedialoop.Checked = true;
            if (Status == "1")
                chkIsFilePublished.Checked = true;
            txtDimension.Text = Dimension;
            txtSource.Text = Source;
            txtFileTitle.Text = Title;
            txtDescription.Value = Description;
            //Literal_FileName.Text = FileName;
            string dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_video_dir"];
            string file_path = "../../../" + dir_path + "/" + FileName;


            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<object type=\"application/x-shockwave-flash\" data=\"../../../scripts/plugins/dewplayer/dewplayer-vol.swf?mp3=" + file_path + "\" width=\"240\" height=\"20\" id=\"dewplayer-vol\"><param name=\"wmode\" value=\"transparent\" /><param name=\"movie\" value=\"../../../scripts/plugins/dewplayer/dewplayer-vol.swf?mp3=" + file_path + "/></object>");
            //Literal_Preview.InnerHtml = sb.ToString();
        }

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
            /*** UPLOAD IMAGE************************************************************************************************************/
            string[] FileImg = new String[2];
            string photo = string.Empty; string thumbnail = string.Empty;
            string front_path = Server.MapPath(front_image_dir);
            string main_path = Server.MapPath(main_image_dir);
            HttpPostedFile myfile = File2.PostedFile;
            if (chkAutoCreateThumbnail.Checked == true)
            {
                if (myfile.ContentLength > 0)
                {
                    if (System.IO.Directory.Exists(front_path) && System.IO.Directory.Exists(main_path))
                    {
                        FileHandleClass file_obj = new FileHandleClass();
                        FileImg = file_obj.uploadFrontMainInputFile(myfile, front_path, main_path, 120, 120);
                        photo = FileImg[0];
                        thumbnail = FileImg[1];
                    }
                    else
                    {
                        string scriptCode = "<script>alert('Đường dẫn không tồn tại.');</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                    }
                }
            }
            else
            {
                if (myfile.ContentLength > 0)
                {
                    FileHandleClass file_handle_obj = new FileHandleClass();
                    photo = file_handle_obj.uploadFixedInputFile(myfile, front_path, 120, 120);
                }
            }
            ////========================================================================================================================

            /*** UPLOAD FILE*************************************************************************************************************/
            string filename = string.Empty; string fileurl = string.Empty,file_ext = string.Empty;
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTypes media_type_obj = new MediaTypes();
            Media_Types type_obj = media_type_obj.GetDetails(TypeId);
            string TypeExt = type_obj.TypeExt.Trim();
            string TypePath = type_obj.TypePath;
            string dir_path = Server.MapPath("~/" + TypePath);
            string[] arr_list = GetArray(TypeExt);

            if (rdlFileUpload.SelectedValue == "0")
            {
                HttpPostedFile posted_file = File1.PostedFile;
                if (posted_file.ContentLength > 0)
                {
                    file_ext = System.IO.Path.GetExtension(posted_file.FileName).ToLower().Trim();
                    if (Directory.Exists(dir_path))
                    {
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
            int relust = media_obj.Insert(userid, vendorid, typeid, topicid, playlistid, albumid,artistid, composerid, filename, fileurl, title, description,
                           autostart, medialoop, dimension, source, photo, thumbnail, status);
            return relust;
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
            /*** UPLOAD IMAGE************************************************************************************************************/

            string Orginal_Photo = ViewState["Photo"].ToString();
            string Orginal_Thumbnail = ViewState["Thumbnail"].ToString();

            string[] FileImg = new String[2];
            string photo = string.Empty; string thumbnail = string.Empty;
            string front_path = Server.MapPath(front_image_dir);
            string main_path = Server.MapPath(main_image_dir);
            HttpPostedFile myfile = File2.PostedFile;
            if (chkAutoCreateThumbnail.Checked == true)
            {
                if (myfile.ContentLength > 0)
                {
                    if (System.IO.Directory.Exists(front_path) && System.IO.Directory.Exists(main_path))
                    {
                        FileHandleClass file_obj = new FileHandleClass();
                        FileImg = file_obj.uploadFrontMainInputFile(myfile, front_path, main_path, 120, 120);
                        photo = FileImg[0];
                        thumbnail = FileImg[1];
                        DeleteFrontImage(Orginal_Photo);
                        DeleteMainImage(Orginal_Thumbnail);
                    }
                    else
                    {
                        string scriptCode = "<script>alert('Đường dẫn không tồn tại.');</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                    }
                }
                else
                {
                    photo = ViewState["Photo"].ToString();
                    thumbnail = ViewState["Thumbnail"].ToString();
                }
            }
            else
            {
                if (myfile.ContentLength > 0)
                {
                    FileHandleClass file_handle_obj = new FileHandleClass();
                    photo = file_handle_obj.uploadFixedInputFile(myfile, front_path, 120, 120);
                    DeleteFrontImage(Orginal_Photo);
                }
                else
                {
                    photo = ViewState["Photo"].ToString();
                    thumbnail = ViewState["Thumbnail"].ToString();
                }
            }
            ////========================================================================================================================
                        
            /*** UPLOAD FILE*************************************************************************************************************/
            string filename = string.Empty; string fileurl = string.Empty, file_ext = string.Empty;
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTypes media_type_obj = new MediaTypes();
            Media_Types type_obj = media_type_obj.GetDetails(TypeId);
            string TypeExt = type_obj.TypeExt.Trim();
            string TypePath = type_obj.TypePath;
            string dir_path = Server.MapPath("~/" + TypePath);
            string[] arr_list = GetArray(TypeExt);

            if (rdlFileUpload.SelectedValue == "0")
            {
                HttpPostedFile posted_file = File1.PostedFile;
                if (posted_file.ContentLength > 0)
                {
                    file_ext = System.IO.Path.GetExtension(myfile.FileName).ToLower().Trim();
                    if (Directory.Exists(dir_path))
                    {
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
            int relust = media_obj.Update(_idx, userid, typeid, topicid, playlistid, albumid, artistid, composerid, filename, fileurl, title, description,
                           autostart, medialoop, dimension, source, photo, thumbnail, status);
            return relust;
        }


        private void DeleteFrontImage(string file_name)
        {
            string _front_dir_img_path = Server.MapPath(front_image_dir);
            if (Directory.Exists(_front_dir_img_path))
            {
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_handle_obj.deleteFile(file_name, _front_dir_img_path);
            }
        }

        private void DeleteMainImage(string file_name)
        {
            string _main_dir_img_path = Server.MapPath(main_image_dir);
            if (Directory.Exists(_main_dir_img_path))
            {
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_handle_obj.deleteFile(file_name, _main_dir_img_path);
            }
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int i = 0;
                if (_mode == UIMode.mode.add)
                {
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
                else if (_mode == UIMode.mode.edit)
                {
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
}