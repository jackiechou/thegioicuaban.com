using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using CommonLibrary.Modules;
using System.IO;
using CommonLibrary.Common;
using CommonLibrary.Services.Banner;

namespace WebApp.modules.admin.banners
{
    public partial class admin_banners_edit : System.Web.UI.Page
    {
        private string banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"];
        private string thumb_banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"]+"/thumb_images";
        private string main_banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"]+"/main_images";
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
            txtWidth.Text = "0";
            txtHeight.Text = "0";
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

        #region BANNER POSITION ==================================================
        protected void LoadPosition2DDL(string selected_value)
        {
            ddlPosition.Items.Clear(); //DROPDOWNLIST        
            string status = "1";
            BannerPosition objTree = new BannerPosition();
            DataTable dtNodes = objTree.GetListByStatus(status); //select all the nodes from DB
            RecursiveFillTree(dtNodes, 0);

            ddlPosition.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlPosition.SelectedValue = selected_value;
        }
        int level = -1;
        private void RecursiveFillTree(DataTable dtParent, int parentID)
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
            dv.RowFilter = string.Format("ParentId = {0}", parentID);

            int i;

            if (dv.Count > 0)
            {
                for (i = 0; i < dv.Count; i++)
                {
                    //DROPDOWNLIST
                    ddlPosition.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["BannerPosition"].ToString()), dv[i]["Id"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["Id"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }
        #endregion ====================================================== 

        #region Culture ======================================================     
        protected void PopulateCulture2DDL(string selected_value)
        {
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlCultureList.Items.Clear();
            ddlCultureList.DataSource = dt;
            ddlCultureList.DataTextField = "CultureName";
            ddlCultureList.DataValueField = "CultureCode";
            ddlCultureList.DataBind();
            ddlCultureList.SelectedValue = selected_value;
        }
        #endregion ============================================================

        private void LoadData()
        {
            BannerController banner_obj = new BannerController();
            DataTable dt = banner_obj.GetDetailById(_idx);
            txtTitle.Text = dt.Rows[0]["Title"].ToString();
            txtDescription.Text = dt.Rows[0]["Description"].ToString();
            txtTags.Text = dt.Rows[0]["Tags"].ToString();
            txtWidth.Text = dt.Rows[0]["Width"].ToString();
            txtHeight.Text = dt.Rows[0]["Height"].ToString();
           
            if (!string.IsNullOrEmpty(dt.Rows[0]["StartDate"].ToString()))
            {
                DateTime _StartDate = Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString());
                txtStartDate.Text = _StartDate.ToString("dd/MM/yyyy");
            }

            if (!string.IsNullOrEmpty(dt.Rows[0]["EndDate"].ToString()))
            {
                DateTime _EndDate = Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString());
                txtEndDate.Text = _EndDate.ToString("dd/MM/yyyy");
            }

            string Status= dt.Rows[0]["Status"].ToString();
            if (Status == "1")
                ChkBoxStatus.Checked = true;
            else
                ChkBoxStatus.Checked = false;

            string position = dt.Rows[0]["Position"].ToString();
            LoadPosition2DDL(position);

            string CultureCode = dt.Rows[0]["CultureCode"].ToString();
            PopulateCulture2DDL(CultureCode);

            //BAT DAU XU LY IMAGE ======================================================
            string ThumbImage = dt.Rows[0]["ThumbImage"].ToString();
            string MainImage = dt.Rows[0]["MainImage"].ToString();
            ViewState["ThumbImage"] = ThumbImage;
            ViewState["MainImage"] = MainImage;

            string file_path = string.Empty;
            if (ThumbImage.Length > 0)
            {
                if (ThumbImage.Substring(0, 3) == "http")
                    file_path = ThumbImage;
                else
                    file_path = thumb_banner_dir_path + "/" + ThumbImage;
            }
            else
                file_path = "~/images/no_image.jpg";

            imgPhoto.Width = 50;
            imgPhoto.Height = 50;
            imgPhoto.ImageUrl = file_path;
            //KET THUC XU LY IMAGE ======================================================
        }

        private int UpdateData()
        {
            int vendorid = Convert.ToInt32(Session["VendorId"].ToString());
            if (ddlPosition.SelectedValue == string.Empty)
            {             
                string scriptCode = "<script>alert('Vui lòng chọn vị trí!!!');</script>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
            }
            string culturecode = ddlCultureList.SelectedValue;
            int height = Convert.ToInt32(txtHeight.Text);
            int width = Convert.ToInt32(txtWidth.Text);
            string title = txtTitle.Text;
            string url = txtUrl.Text;
            int position = Convert.ToInt32(ddlPosition.SelectedValue);
            string description = txtDescription.Text;
            string tags = txtTags.Text;
            bool bStatus = ChkBoxStatus.Checked;
            string status = string.Empty;
            if (bStatus == true)
            {
                status = "1";
            }
            else
            {
                status = "0";
            }      

            /*** UPLOAD ****************************************************************************************************************************************/
            string thumb_image = "", main_image = "";
            HttpPostedFile myfile = FileInput.PostedFile;
            if (myfile.FileName != string.Empty)
            {
                if (myfile.ContentLength > 0)
                {
                    string banner_img_path = Server.MapPath(banner_dir_path);
                    string front_img_path = Server.MapPath(thumb_banner_dir_path);
                    string main_img_path = Server.MapPath(main_banner_dir_path);

                    if (!Directory.Exists(banner_img_path))
                        Directory.CreateDirectory(banner_img_path);

                    if (!Directory.Exists(front_img_path))
                        Directory.CreateDirectory(front_img_path);

                    if (!Directory.Exists(main_img_path))
                        Directory.CreateDirectory(main_img_path);

                    FileHandleClass file_handle_obj = new FileHandleClass();
                    string[] FileImg = new String[2];
                    FileImg = file_handle_obj.uploadFrontMainInputFile(myfile, front_img_path, main_img_path, 120, 120);
                    main_image = FileImg[0].ToString();
                    thumb_image = FileImg[1].ToString();
                    imgPhoto.ImageUrl = thumb_banner_dir_path + "/" + thumb_image;

                    string Orginal_front_image = ViewState["ThumbImage"].ToString();
                    string Orginal_main_image = ViewState["MainImage"].ToString();
                    file_handle_obj.DeleteFile(Orginal_front_image, front_img_path);
                    file_handle_obj.DeleteFile(Orginal_main_image, main_img_path);
                }
            }
            else
            {
                main_image = ViewState["MainImage"].ToString();
                thumb_image = ViewState["ThumbImage"].ToString();
            }
            /*************************************************************************************************************************************************/



            #region xu ly thoi gian  ====================================================================================
            System.Globalization.DateTimeFormatInfo MyDateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo();
            MyDateTimeFormatInfo.ShortDatePattern = "dd/MM/yyyy";

            string start_date = null, end_date = null;
            if (txtStartDate.Text != string.Empty)
            {
                DateTime _start_date = DateTime.Parse(txtStartDate.Text, MyDateTimeFormatInfo);
                start_date = _start_date.ToString("yyyy-MM-dd");
            }
            if (txtEndDate.Text != string.Empty)
            {
                DateTime _end_date = DateTime.Parse(txtEndDate.Text, MyDateTimeFormatInfo);
                end_date = _end_date.ToString("yyyy-MM-dd");
            }
            if (txtStartDate.Text != string.Empty && txtEndDate.Text != string.Empty)
            {
                DateTime _start_date = DateTime.Parse(txtStartDate.Text, MyDateTimeFormatInfo);
                DateTime _end_date = DateTime.Parse(txtEndDate.Text, MyDateTimeFormatInfo);

                if (DateTime.Compare(_start_date, _end_date) > 0)
                {
                    string scriptCode = "<script>alert('Thời điểm bắt đầu phải nhỏ hơn thời điểm kết thúc');</script>";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                }
            }
            #endregion ================================================================================================

            BannerController banner_obj = new BannerController();
            int i = banner_obj.Update(vendorid, _idx, culturecode, title, main_image, thumb_image, url, position, description, tags, width, height, start_date, end_date, status);           
            return i;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int i = 0;
                if (_mode == UIMode.mode.edit)
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

        protected void OrFieldValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (FileInput.Value.Length <= 0 && txtUrl.Text.Length <= 0)
                args.IsValid = false;
            else
                args.IsValid = true;
        }
    }
}