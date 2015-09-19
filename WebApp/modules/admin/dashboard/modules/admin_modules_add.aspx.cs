using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.dashboard.modules
{
    public partial class admin_modules_add : System.Web.UI.Page
    {
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

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {                
                LoadPoratlList2DDL();
                LoadTabList2DDL();
            }
            MultiView1.ActiveViewIndex = 0;
            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Portal List ======================================================
        private void LoadPoratlList2DDL()
        {
            ddlPortalList.Items.Clear();

            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetList(); //select all the nodes from DB
            ddlPortalList.DataSource = dt;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedIndex = 1;
            ddlPortalList.Enabled = false;
        }
        #endregion ================================================================

         #region Tab List ======================================================
        private void LoadTabList2DDL()
        {
            ddlTabList.Items.Clear();

            TabController tab_obj = new TabController();
            DataSet ds = tab_obj.GetActiveList(); //select all the nodes from DB
            ddlTabList.DataSource = ds.Tables[0];
            ddlTabList.DataTextField = "TabName";
            ddlTabList.DataValueField = "TabId";
            ddlTabList.DataBind();
            ddlTabList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlTabList.SelectedIndex = 1;
            ddlTabList.Enabled = false;
        }
        #endregion ================================================================
                

        
        private int AddData()
        {           
            string PortalId = ddlPortalList.SelectedValue;
            if (ddlPortalList.SelectedValue == "0")
            {
                Response.Write("<script>alert('Vui lòng chọn ứng dụng');window.history.back();</script>");
                Response.End();
                return -1;
            }
            else
            {                
                Modules module_obj = new Modules();
                string ModuleTitle = txtName.Text;

                bool AllTab = false, IsAdmin = false, IsDeleted = false, InheritViewPermissions = false;
                if (ckbAllTab.Checked)
                    AllTab = true;
                if (ckbIsAdmin.Checked)
                    IsAdmin = true;
                if (ckbIsDeleted.Checked)
                    IsDeleted = true;
                if (ckbckbInheritViewPermissions.Checked)
                    InheritViewPermissions = true;

                int result = module_obj.Insert(PortalId, ModuleTitle, AllTab, IsAdmin, IsDeleted, InheritViewPermissions);
                return result;
            }
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
             Page.Validate("ValidationCheck");
             if (Page.IsValid)
             {
                 System.Threading.Thread.Sleep(2000);
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