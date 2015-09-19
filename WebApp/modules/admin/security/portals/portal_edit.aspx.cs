using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using CommonLibrary.Modules;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common;

namespace WebApp.modules.admin.security.portals
{
    public partial class portal_edit : System.Web.UI.Page
    {   
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

        private void Page_Init(object sender, EventArgs e)
        {
            txtHostFee.Text = "0";
            txtHostSpace.Text = "0";         
        }

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                _idx =Convert.ToInt32(Request.QueryString["idx"]);
                LoadData();                

                txtHostFee.Attributes["onkeyup"] = "javascript: FormatNumber(this);return false;";
                txtHostFee.Attributes.Add("value", txtHostFee.Text);
                txtHostFee.Attributes.Add("style", "text-align:right");

                txtHostSpace.Attributes.Add("style", "text-align:right");
                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Application =======================================       
        private void PopulateApplicationList2DDL(string selected_value)
        {
            ddlApplicationList.Items.Clear();

            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_app = app_obj.GetApps();
            ddlApplicationList.AutoPostBack = true;
            ddlApplicationList.DataSource = dt_app;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
            ddlApplicationList.Items.Insert(0, new ListItem("-Chọn ứng dụng-", ""));
            ddlApplicationList.SelectedValue = selected_value;
        }
        #endregion

        #region Language ======================================================
        protected void PopulateLanguage2DDL(string selected_value)
        {
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlLanguageList.Items.Clear();
            ddlLanguageList.DataSource = dt;
            ddlLanguageList.DataTextField = "CultureName";
            ddlLanguageList.DataValueField = "CultureCode";
            ddlLanguageList.DataBind();
            ddlLanguageList.SelectedValue = selected_value;
        }
        #endregion ============================================================

       private void LoadData()
        {
            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetDetails(_idx);

            string PortalName = dt.Rows[0]["PortalName"].ToString();
            string ApplicationId = dt.Rows[0]["ApplicationId"].ToString();
            string ExpiryDate = dt.Rows[0]["ExpiryDate"].ToString();
            string Currency = dt.Rows[0]["Currency"].ToString();
            string HostFee = dt.Rows[0]["HostFee"].ToString();
            string HostSpace = dt.Rows[0]["HostSpace"].ToString();         
            string DefaultLanguage = dt.Rows[0]["DefaultLanguage"].ToString();
            string HomeDirectory = dt.Rows[0]["HomeDirectory"].ToString();
            string Url = dt.Rows[0]["Url"].ToString();  
            string KeyWords = dt.Rows[0]["KeyWords"].ToString();
            string FooterText = dt.Rows[0]["FooterText"].ToString();
            string Description = dt.Rows[0]["Description"].ToString();

            txtPortalName.Text = PortalName;
            txtExpiryDate.Text = ExpiryDate;
            txtHostFee.Text = HostFee;
            txtHostSpace.Text = HostSpace;            
            
            txtHomeDirectory.Text = HomeDirectory;
            txtUrl.Text = Url;
            txtKeyWords.Text = KeyWords;
            txtFooterText.Text = FooterText;
            txtDescription.Text = Description;

            PopulateApplicationList2DDL(ApplicationId);
            PopulateLanguage2DDL(DefaultLanguage);
           // PopulateCurrencyTypeList2DDL(Currency);

            string logo_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/portal_images/logo";
            string background_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/portal_images/backgrounds";
            string LogoFile = dt.Rows[0]["LogoFile"].ToString();
            HiddenLogoFile.Value = LogoFile;            
            Logo_Img.ImageUrl = logo_dir_path + "/" + LogoFile;
            Logo_Img.Height = 50;
            Logo_Img.Width = 50;
           
            string BackgroundFile = dt.Rows[0]["BackgroundFile"].ToString();
            HiddenBackgroundFile.Value = BackgroundFile;           
            Background_Img.ImageUrl = background_dir_path + "/" + BackgroundFile;
            Background_Img.Height = 50;
            Background_Img.Width = 50;
           
        }

        private int UpdateData()
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            string PortalName = txtPortalName.Text;           
            string Currency = ddlCurrencyTypeList.SelectedValue;
            string HostFee = txtHostFee.Text;
            int HostSpace = Convert.ToInt32(txtHostSpace.Text);          
            string DefaultLanguage = ddlLanguageList.Text;
            string HomeDirectory = txtHomeDirectory.Text;
            string Url = txtUrl.Text;
            string KeyWords = txtKeyWords.Text;
            string FooterText = txtFooterText.Text;
            string Description = txtDescription.Text;
            string LastModifiedByUserId = Session["UserId"].ToString(); ;

            #region UPLOAD ************************************************************************************************************/            
            string logo_dir_path = HttpContext.Current.Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/portal_images/logo");
            string background_dir_path = HttpContext.Current.Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/portal_images/backgrounds");
            string LogoFile = string.Empty; string BackgroundFile = string.Empty;            
            FileHandleClass file_handle_obj = new FileHandleClass();

            HttpPostedFile _logo_file = InputLogoFile.PostedFile;
            if (_logo_file.ContentLength > 0)
            {
                string logo_file_path = logo_dir_path + "/" + HiddenLogoFile.Value;
                if (File.Exists(logo_file_path))
                {
                    File.Delete(logo_file_path);
                }
                LogoFile = file_handle_obj.uploadInputFile(_logo_file, logo_dir_path);
            }
            else
            {
                LogoFile = HiddenLogoFile.Value;
            }

            HttpPostedFile _background_file = InputBackgroundFile.PostedFile;
            if (_background_file.ContentLength > 0)
            {
                string background_file_path = background_dir_path + "/" + HiddenLogoFile.Value;
                if (File.Exists(background_file_path))
                {
                    File.Delete(background_file_path);
                }
                BackgroundFile = file_handle_obj.uploadInputFile(_background_file, background_dir_path);
            }
            else
            {
                BackgroundFile = HiddenBackgroundFile.Value;
            }
            #endregion ================================================================================

            #region xu ly thoi gian  =====================================================================================
            System.Globalization.DateTimeFormatInfo MyDateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo();
            MyDateTimeFormatInfo.ShortDatePattern = "dd/MM/yyyy";
            string ExpiryDate = null;

            if (txtExpiryDate.Text != string.Empty)
            {
                DateTime _ExpiryDate = DateTime.Parse(txtExpiryDate.Text, MyDateTimeFormatInfo);
                ExpiryDate = _ExpiryDate.ToString("yyyy-MM-dd");
            }
            #endregion ====================================================================================================

            PortalController portal_obj = new PortalController();
            int i = portal_obj.Update(_idx, ApplicationId, PortalName, ExpiryDate, Currency, HostFee, HostSpace, DefaultLanguage,
                 HomeDirectory, Url, LogoFile, BackgroundFile, KeyWords, FooterText, Description, LastModifiedByUserId);
            return i;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);               
                int i = UpdateData();
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