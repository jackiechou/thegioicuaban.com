using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using CommonLibrary.Modules;
using CommonLibrary.Common;
using CommonLibrary.Services.Banner;
using System.IO;

namespace WebApp.modules.admin.banners
{
    public partial class admin_banners_add : System.Web.UI.Page
    {
        private string banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"];
        private string thumb_banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"] + "/thumb_images";
        private string main_banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"] + "/main_images";

        #region SESSION PROPERTIES ===========================
        protected string UserId
        {
            get
            {
                if (Session["UserId"] != null)
                    return Session["UserId"].ToString();
                else
                    return string.Empty;
            }
        }
        protected string VendorId
        {
            get
            {
                if (Session["VendorId"] != null)
                    return Session["VendorId"].ToString();
                else
                    return string.Empty;
            }
        }

        #endregion ===========================================

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

            DateTime CurrentDays = System.DateTime.Now;
            DateTime EndDate = CurrentDays.AddYears(1);
            txtStartDate.Text = CurrentDays.ToString("dd/MM/yyyy");
            txtEndDate.Text = EndDate.ToString("dd/MM/yyyy");
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
                        LoadPosition2DDL();
                        PopulateCulture2DDL();
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
        protected void LoadPosition2DDL()
        {
            //Load list item to dropdownlist
            //ListItemCollection lstColl = new ListItemCollection();
            //lstColl.Add(new ListItem("Top", "top"));
            //lstColl.Add(new ListItem("Left", "left"));
            //lstColl.Add(new ListItem("Right", "right"));
            //lstColl.Add(new ListItem("Bottom", "bottom"));

            //ddlPosition.DataSource = lstColl;
            //ddlPosition.DataTextField = "Text";
            //ddlPosition.DataValueField = "Value";
            //ddlPosition.DataBind();
            //ddlPosition.Items.Insert(0, new ListItem("- Chọn vị trí -", "")); // add the new item at the top of the list
            //ddlPosition.SelectedIndex = 0; // Select the first item

            ddlPosition.Items.Clear(); //DROPDOWNLIST        
            string status = "1";
            BannerPosition objTree = new BannerPosition();
            DataTable dtNodes = objTree.GetListByStatus(status); //select all the nodes from DB
            RecursiveFillTree(dtNodes, 0);

            //ddlPosition.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlPosition.SelectedIndex = 0;
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
        protected void PopulateCulture2DDL()
        {
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlCultureList.Items.Clear();
            ddlCultureList.DataSource = dt;
            ddlCultureList.DataTextField = "CultureName";
            ddlCultureList.DataValueField = "CultureCode";
            ddlCultureList.DataBind();
            ddlCultureList.SelectedValue = "vi-VN";
        }
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

        private int AddData()
        {
            string culturecode = ddlCultureList.SelectedValue;
            int vendorid = Convert.ToInt32(VendorId);
            int height = Convert.ToInt32(txtHeight.Text);
            int width = Convert.ToInt32(txtWidth.Text);
            string title = txtTitle.Text;
            string url = txtUrl.Text;
            int position =Convert.ToInt32(ddlPosition.SelectedValue);
            string description = txtDescription.Text;
            string tags = txtTags.Text;
           
            /*** UPLOAD ****************************************************************************************************************************************/
            string thumb_image = "", main_image = "";
            HttpPostedFile myfile = FileInput.PostedFile;
            if (myfile.FileName != string.Empty)
            {
                string front_img_path = Server.MapPath(thumb_banner_dir_path);
                string main_img_path = Server.MapPath(main_banner_dir_path);

                if (!Directory.Exists(banner_dir_path))
                    Directory.CreateDirectory(banner_dir_path);

                if (!Directory.Exists(front_img_path))
                    Directory.CreateDirectory(front_img_path);

                if (!Directory.Exists(main_img_path))
                    Directory.CreateDirectory(main_img_path);

                FileHandleClass file_handle_obj = new FileHandleClass();
                string[] FileImg = new String[2];
                FileImg = file_handle_obj.uploadFrontMainInputFile(myfile, front_img_path, main_img_path, 120, 120);
                main_image = FileImg[0].ToString();
                thumb_image = FileImg[1].ToString();
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

            bool bStatus = ChkBoxStatus.Checked;
            string status = string.Empty;
            if (bStatus == true)           
                status = "1";           
            else           
                status = "0";
            
            BannerController banner_obj = new BannerController();
            int i = banner_obj.Insert(vendorid, culturecode, title, main_image, thumb_image, url, position, description, tags, width, height, start_date, end_date, status);
            return i;            
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
            }
        }
    }
}