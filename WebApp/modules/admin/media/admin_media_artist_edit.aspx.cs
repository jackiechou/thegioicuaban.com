using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using MediaLibrary;
using System.IO;

namespace WebApp.modules.admin.media
{
    public partial class admin_media_artist_edit : System.Web.UI.Page
    {
        private static string upload_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"];
        private static string upload_front_image_dir = "~/" + upload_image_dir + "/media_images/artist_images/front_images";
        private static string upload_main_image_dir = "~/" + upload_image_dir + "/media_images/artist_images/main_images";

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
                        PopulateStatus2DDL();
                    }
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx = Convert.ToInt32(Request.QueryString["idx"]);
                        LoadData();
                    }
                    hdnWindowUIMODE.Value = _mode.ToString();
                }
                Session["FileUpload1"] = null;
                MultiView1.ActiveViewIndex = 0;
            }
            else
            {
                if (FileUpload1.HasFile)
                {
                    Session["FileUpload1"] = FileUpload1;
                    txtFile.Text = FileUpload1.FileName;
                }
                else if (Session["FileUpload1"] != null)
                {
                    FileUpload1 = (FileUpload)Session["FileUpload1"];
                    txtFile.Text = FileUpload1.FileName;
                }
            }
        }



        #region Status ==================================================
        protected void PopulateStatus2DDL()
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Active", "1"));
            lstColl.Add(new ListItem("InActive", "0"));

            rdlStatus.DataSource = lstColl;
            rdlStatus.DataTextField = "Text";
            rdlStatus.DataValueField = "Value";
            rdlStatus.DataBind();
            //rdlStatus.Items.Insert(0, new ListItem("Chọn trạng thái", ""));
            rdlStatus.SelectedIndex = 0; // Select the first item
            rdlStatus.AutoPostBack = true;
        }
        protected void PopulateStatus2DDL(string selected_value)
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Active", "1"));
            lstColl.Add(new ListItem("InActive", "0"));

            rdlStatus.DataSource = lstColl;
            rdlStatus.DataTextField = "Text";
            rdlStatus.DataValueField = "Value";
            rdlStatus.DataBind();
            //rdlStatus.Items.Insert(0, new ListItem("Chọn trạng thái", ""));
            rdlStatus.SelectedValue = selected_value; // Select the first item
            rdlStatus.AutoPostBack = true;
        }
        #endregion ======================================================

        private void LoadData()
        {
            MediaArtists artist_obj = new MediaArtists();
            Media_Artists entity = artist_obj.GetDetails(_idx);

            string FrontImage = entity.FrontImage;
            string MainImage = entity.MainImage;
            imgPhoto.ImageUrl = upload_front_image_dir + "/" + FrontImage;
            ViewState["FrontImage"] = FrontImage;
            ViewState["MainImage"] = MainImage;

            txtArtistName.Text = entity.ArtistName;
            txtDescription.Text = entity.Description;
            string Status = entity.Status;
            PopulateStatus2DDL(Status);
        }

        private int AddData()
        {
            string user_id = Session["UserId"].ToString();
            string artist_name = txtArtistName.Text;
            string description = txtDescription.Text;
            string status = rdlStatus.SelectedValue;

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
                imgPhoto.ImageUrl = upload_front_image_dir + "/" + front_image;
            }
            ////========================================================================================================================

            MediaArtists artist_obj = new MediaArtists();
            int i = artist_obj.Insert(user_id, artist_name, front_image,main_image, description, status);
            return i;
        }

        private int UpdateData()
        {
            string user_id = Session["UserId"].ToString();
            string artist_name = txtArtistName.Text;
            string description = txtDescription.Text;
            string status = rdlStatus.SelectedValue;

            /*** UPLOAD ************************************************************************************************************/
            string[] FileImg = new String[2];
            string front_image = string.Empty; string main_image = string.Empty;
            string front_path = Server.MapPath(upload_front_image_dir);
            string main_path = Server.MapPath(upload_main_image_dir);

            string Orginal_front_image = ViewState["FrontImage"].ToString();
            string Orginal_main_image = ViewState["MainImage"].ToString();

            //if (FileUpload1.HasFile)
            if (Session["FileUpload1"] != null && Session["FileUpload1"].ToString() != string.Empty)
            {
                FileHandleClass file_bj = new FileHandleClass();
                FileImg = file_bj.upload_front_main_image(FileUpload1, front_path, main_path, 120, 120);
                main_image = FileImg[0];
                front_image = FileImg[1];
                //System.Drawing.Image img1 = System.Drawing.Image.FromFile(front_path+ "/" + front_image);                
                imgPhoto.ImageUrl = upload_front_image_dir + "/" + front_image;
                string folderchild = Orginal_front_image.Substring(0, 10);
                DeleteFrontImage(Orginal_front_image);
                DeleteMainImage(Orginal_main_image);
            }
            else
            {
                front_image = Orginal_front_image;
                main_image = Orginal_main_image;
            }
            ////========================================================================================================================

            MediaArtists artist_obj = new MediaArtists();
            int i = artist_obj.Update(_idx, user_id, artist_name, front_image, main_image, description, status);
            return i;
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
                        if (Page.IsPostBack)
                            ScriptManager.RegisterStartupScript(Page, typeof(Page), this.ClientID, "onSuccess();", true);
                        else
                            Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID, "onSuccess();", true);
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