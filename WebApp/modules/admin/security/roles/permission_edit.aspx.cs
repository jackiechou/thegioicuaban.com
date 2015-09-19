using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Security.Permissions;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;

namespace WebApp.modules.admin.security.roles
{
    public partial class permission_edit : System.Web.UI.Page
    {
        PermissionController permission_obj = new PermissionController();
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
                        _idx =Convert.ToInt32(Request.QueryString["idx"]);
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

      
        private void LoadData()
        {
            DataTable dt = permission_obj.GetDetails(_idx);
            txtPermissionCode.Text = dt.Rows[0]["PermissionCode"].ToString();
            txtPermissionKey.Text = dt.Rows[0]["PermissionKey"].ToString();
            txtPermissionName.Text = dt.Rows[0]["PermissionName"].ToString();
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
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
        }

        private void AddData()
        {
            string PermissionCode  = txtPermissionCode.Text;
            string PermissionKey = txtPermissionKey.Text;
            string PermissionName = txtPermissionName.Text;
            permission_obj.Insert(PermissionCode, PermissionKey, PermissionName);            
        }

        private void UpdateData()
        {
            string PermissionCode = txtPermissionCode.Text;
            string PermissionKey = txtPermissionKey.Text;
            string PermissionName = txtPermissionName.Text;
            permission_obj.Update(_idx, PermissionCode, PermissionKey, PermissionName); 
        }
    }
}