using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Common;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Modules;

namespace WebApp.modules.admin.security.portals
{
    public partial class portal_add : System.Web.UI.Page
    {
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
                PopulateApplicationList2DDL();
                PopulateLanguage2DDL();
                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
            //btnOkay.OnClientClick = String.Format("this.disabled=true; __doPostBack('{0}','');", btnOkay.UniqueID);
        }

        #region Application =======================================
        private void PopulateApplicationList2DDL()
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
            ddlApplicationList.SelectedIndex = 0;
        }        
        #endregion

        #region Language ======================================================
        protected void PopulateLanguage2DDL(){
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlLanguageList.Items.Clear();
            ddlLanguageList.DataSource = dt;
            ddlLanguageList.DataTextField = "CultureName";
            ddlLanguageList.DataValueField = "CultureCode";
            ddlLanguageList.DataBind();
            ddlLanguageList.SelectedIndex = 0;
        }
        #endregion ============================================================

       private int AddData()
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
            string CreatedByUserId = Session["UserId"].ToString();

            #region UPLOAD ************************************************************************************************************/
            string[] array_files = new String[2];
            string LogoFile = string.Empty; string BackgroundFile = string.Empty;
            string logo_dir_path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/portal_images/logo");
            string background_dir_path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/portal_images/backgrounds");
            FileHandleClass file_handle_obj = new FileHandleClass();

            HttpPostedFile _logo_file = InputLogoFile.PostedFile;
            if (_logo_file.ContentLength > 0)
            {                
                LogoFile = file_handle_obj.uploadInputFile(_logo_file, logo_dir_path);               
            }

            HttpPostedFile _background_file = InputBackgroundFile.PostedFile;
            if (_background_file.ContentLength > 0)
            {
                BackgroundFile = file_handle_obj.uploadInputFile(_background_file, background_dir_path);
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
            int i = portal_obj.Insert(ApplicationId, PortalName, ExpiryDate, Currency, HostFee, HostSpace, DefaultLanguage,
                HomeDirectory, Url, LogoFile, BackgroundFile, KeyWords, FooterText, Description, CreatedByUserId);
            return i;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                string result = string.Empty;
   
                int i = AddData();
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