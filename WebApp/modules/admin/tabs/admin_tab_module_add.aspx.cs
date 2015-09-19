using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Entities.Tabs;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;

namespace WebApp.modules.admin.tabs
{
    public partial class admin_tab_module_add : System.Web.UI.Page
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
        
        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                _idx = Convert.ToInt32(Request.QueryString["idx"]);
                LoadModuleList2DDL();
                LoadPaneList2DDL();
                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region MODULE List ================================================================
        private void LoadModuleList2DDL()
        {
            ddlModuleList.Items.Clear();

            Modules module_obj = new Modules();
            DataTable dt = module_obj.GetAll(); //select all the nodes from DB
            ddlModuleList.DataSource = dt;
            ddlModuleList.DataTextField = "ModuleTitle";
            ddlModuleList.DataValueField = "ModuleID";
            ddlModuleList.DataBind();
            ddlModuleList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlModuleList.SelectedIndex = 0;           
        }
        #endregion

        public string GetVirtualPath(string physicalPath)
        {
            //string physicalPath = Server.HtmlEncode(Request.PhysicalPath);
            string rootpath = Server.MapPath("~/");
            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");
            return physicalPath;
        }

        #region PaneList =================================================
        protected void LoadPaneList2DDL()
        {
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("ContentPane", "ContentPane"));
            lstColl.Add(new ListItem("Banner", "Banner"));
            lstColl.Add(new ListItem("DivContainer", "DivContainer"));
            lstColl.Add(new ListItem("TopPane", "TopPane"));
            lstColl.Add(new ListItem("LeftPane", "LeftPane"));
            lstColl.Add(new ListItem("RightPane", "RightPane"));
            lstColl.Add(new ListItem("BottomPane", "BottomPane"));

            ddlPaneName.DataSource = lstColl;
            ddlPaneName.DataTextField = "Text";
            ddlPaneName.DataValueField = "Value";
            ddlPaneName.DataBind();
            ddlPaneName.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPaneName.SelectedIndex=0;
        }
        #endregion =======================================================

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            int ModuleID = int.Parse(ddlModuleList.SelectedValue);
            string PaneName = ddlPaneName.SelectedValue;        
           
            TabModules tab_module_obj = new TabModules();
            tab_module_obj.Insert(_idx,ModuleID,PaneName);
        }
    }
}