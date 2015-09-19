using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Globalization;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Entities.Portal;


namespace WebApp.modules.admin.dashboard.modules
{
    public partial class admin_modules_edit : System.Web.UI.Page
    {
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
                        _idx = int.Parse(Request.QueryString["idx"]);
                        LoadData();
                        hdnWindowUIMODE.Value = _mode.ToString();
                    }

                }
                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Application List ================================================================
        private void LoadApplicationList2DDL(string selected_value)
        {
            ddlApplicationList.Items.Clear();

            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_gametypes = app_obj.GetApps(); //select all the nodes from DB
            ddlApplicationList.DataSource = dt_gametypes;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
           // ddlApplicationList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlApplicationList.SelectedValue = selected_value;
            ddlApplicationList.Enabled = false;
        }
        protected void ddlApplicationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPortalList2DDL();
        }
        #endregion

        #region Portal List ================================================================
        private void LoadPortalList2DDL()
        {
            ddlPortalList.Items.Clear();
            string ApplicationId = ddlApplicationList.SelectedValue;
            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetListByApplicationId(ApplicationId);
            ddlPortalList.DataSource = dt;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedIndex = 0;            
        }
        private void LoadPortalList2DDL(string selected_value)
        {
            ddlPortalList.Items.Clear();
            string ApplicationId = ddlApplicationList.SelectedValue;
            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetListByApplicationId(ApplicationId); 
            ddlPortalList.DataSource = dt;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedValue = selected_value;
            //ddlPortalList.Enabled = false;
        }
        #endregion

        private void LoadData()
        {
            Modules module_obj = new Modules();
            DataTable dt = module_obj.GetDetails(_idx);

            LoadApplicationList2DDL(Session["ApplicationId"].ToString());
            LoadPortalList2DDL(dt.Rows[0]["PortalId"].ToString());
            txtModuleTitle.Text = dt.Rows[0]["ModuleTitle"].ToString();
            ckbAllTab.Checked = Convert.ToBoolean(dt.Rows[0]["AllTabs"].ToString());
            ckbIsAdmin.Checked = Convert.ToBoolean(dt.Rows[0]["IsAdmin"].ToString());
            ckbIsDeleted.Checked = Convert.ToBoolean(dt.Rows[0]["IsDeleted"].ToString());
            ckbInheritViewPermissions.Checked = Convert.ToBoolean(dt.Rows[0]["InheritViewPermissions"].ToString());

            string ModuleId = dt.Rows[0]["ModuleId"].ToString();
            IframePermission.Attributes.Add("src", "admin_module_permission_edit.aspx?idx=" + _idx);
            IframeModuleController.Attributes.Add("src", "admin_module_controller.aspx?idx=" + _idx);
        }

        private int UpdateData()
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            string ModuleTitle = txtModuleTitle.Text;

            bool AllTab = false, IsAdmin = false, IsDeleted = false, InheritViewPermissions = false;
            if (ckbAllTab.Checked)
                AllTab = true;
            if (ckbIsAdmin.Checked)
                IsAdmin = true;
            if (ckbIsDeleted.Checked)
                IsDeleted = true;
            if (ckbInheritViewPermissions.Checked)
                InheritViewPermissions = true;
            Modules module_obj = new Modules();
            int result = module_obj.Update(_idx, ApplicationId, ModuleTitle, AllTab, IsAdmin, IsDeleted, InheritViewPermissions);
            return result;                
           
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

       
    }
}