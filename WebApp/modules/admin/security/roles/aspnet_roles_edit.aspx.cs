using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Security.Roles;
using CommonLibrary.Application;

namespace WebApp.modules.admin.security.roles
{
    public partial class aspnet_roles_edit : System.Web.UI.Page
    {
        RoleController role_obj = new RoleController();
        DataTable dt = new DataTable();

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

        private string _idx
        {
            get
            {
                if (ViewState["idx"] == null)
                    ViewState["idx"] = "";
                return (string)ViewState["idx"];
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
                    if (_mode == UIMode.mode.add)
                    {
                        LoadApplicationList2DDL();
                    }
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx = Request.QueryString["idx"];
                        LoadData();
                        hdnWindowUIMODE.Value = _mode.ToString();
                    }

                }
                MultiView1.ActiveViewIndex = 0;

            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
            //btnOkay.OnClientClick = String.Format("this.disabled=true; __doPostBack('{0}','');", btnOkay.UniqueID);

        }

        #region Application =======================================
        private void LoadApplicationList2DDL()
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
        private void LoadApplicationList2DDL(string selected_value)
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

        private void LoadData()
        {
            DataTable dt = role_obj.GetDetails(_idx);

            LoadApplicationList2DDL(dt.Rows[0]["ApplicationId"].ToString());
            txtRoleName.Text = dt.Rows[0]["RoleName"].ToString();
            txtDescription.Text = dt.Rows[0]["Description"].ToString();
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            //try
            //{
            System.Threading.Thread.Sleep(1000);
            if (_mode == UIMode.mode.add)
            {
                AddData();
            }
            else if (_mode == UIMode.mode.edit)
            {
                UpdateData();
            }
            MultiView1.ActiveViewIndex = 1;
            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);

            //}
            //catch
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
            //    MultiView1.ActiveViewIndex = 1;
            //}
        }

        private void AddData()
        {
            string app_name = ddlApplicationList.SelectedItem.Text;
            string rolename = txtRoleName.Text;
            string description = txtDescription.Text;

            role_obj.CreateRole(app_name, rolename, description);
        }

        private void UpdateData()
        {
            string app_name = ddlApplicationList.SelectedItem.Text;
            string rolename = txtRoleName.Text;
            string description = txtDescription.Text;
            role_obj.UpdateRole(app_name, _idx, rolename, description);
        }
    }
}