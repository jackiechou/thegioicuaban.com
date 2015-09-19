using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Text.RegularExpressions;
using CommonLibrary.Entities.Modules;
namespace WebApp.modules.admin.dashboard.modules
{
    public partial class admin_module_controller_add : System.Web.UI.Page
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
                LoadControlTypeList2DDL();

                Modules module_obj = new Modules();
                Literal_ModuleName.Text = module_obj.GetModuleTitleByModuleId(_idx);
                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        protected void LoadControlTypeList2DDL()
        {
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Tab", "1"));
            lstColl.Add(new ListItem("Module", "2"));

            ddlControlType.DataSource = lstColl;
            ddlControlType.DataTextField = "Text";
            ddlControlType.DataValueField = "Value";
            ddlControlType.DataBind();
            ddlControlType.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlControlType.SelectedIndex = 0;
        }

        private int AddData()
        {          
            string Title = txtControlTitle.Text;
            int Type = int.Parse(ddlControlType.SelectedValue);
            string Key = txtControlKey.Text;
            string ControlSrc = string.Empty;
            if (ControlSrc_File.Value != string.Empty)
            {
                ControlSrc = GetVirtualPath(ControlSrc_File.Value);
            }
            //string Physical_ControlSrc=  ControlSrcFile.Value;
            //string ControlSrc = GetVirtualPath(Physical_ControlSrc.Value);
            //  Uri uriSiteRoot = new Uri(context.Request.Url.GetLeftPart(UriPartial.Authority));
            //string ControlSrc = uriSiteRoot + "/" + Physical_ControlSrc;


            /*** UPLOAD ****************************************************************************************************************************************/
            HttpPostedFile file = IconFile.PostedFile;
            string Icon_FileName = "";
            if (file.ContentLength > 0)
            {
                Icon_FileName = System.IO.Path.GetFileName(file.FileName);
                string savePath = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]
                    + "/module_images/" + Icon_FileName);
                file.SaveAs(savePath);
            }
            /*******************************************************************************************************************************************/
            
            ModuleController module_control_obj = new ModuleController();
            int result = module_control_obj.Insert(_idx, Title, Key, ControlSrc, Type, Icon_FileName);
            return result;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            //try
            //{
            if (!IsRefresh)
            {
                int result = AddData();
                if (result == 1)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                }
                else
                {
                    if (result == -1)
                    {
                        //Response.Write("<script>alert('Info is not valid');window.history.back();</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    }
                    else if (result == -2)
                    {
                        // Response.Write("<script>alert('Insert error.');window.history.back();</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    }
                    else if (result == 3)
                    {
                        // Response.Write("<script>alert('Module ID existsed');window.history.back();</script>");                        
                        ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    }
                }
                MultiView1.ActiveViewIndex = 1;
            }

            //}
            //catch
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
            //    MultiView1.ActiveViewIndex = 1;
            //}
        }

        public string GetVirtualPath(string physicalPath)
        {
            //string physicalPath = Server.HtmlEncode(Request.PhysicalPath);
            string rootpath = Server.MapPath("~/");
            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");
            return physicalPath;
        }

        //string myVirtualPath = GetVirtualPath(HttpContext.Current.Request.Raw));
        //public static string GetVirtualPath(string url)
        //{
        //    if (HttpContext.Current.Request.ApplicationPath == "/")
        //    {
        //        return "~" + url;
        //    }

        //    return Regex.Replace(url, "^" +
        //                   HttpContext.Current.Request.ApplicationPath + "(.+)$", "~$1");

        //}

        #region Prevent RERESH PAGE ============================================================================
        private bool _refreshState;
        private bool _isRefresh;

        public bool IsRefresh
        {
            get { return _isRefresh; }
        }
        // Overrides base class' LoadViewState to check and prevent a refresh 
        protected override void LoadViewState(object savedState)
        {
            object[] AllStates = (object[])savedState;
            base.LoadViewState(AllStates[0]); _refreshState = Convert.ToBoolean(AllStates[1]);
            _isRefresh = _refreshState == (Session["__ISREFRESH "] == null ? false : (bool)Session["__ISREFRESH "]);
        }
        // Adds custom data into viewstate by overrinding base class method  
        protected override object SaveViewState()
        {
            Session["__ISREFRESH "] = _refreshState;
            object[] AllStates = new object[2];
            AllStates[0] = base.SaveViewState();
            AllStates[1] = !_refreshState;
            return AllStates;
        }
        #endregion  =========================================================================================== 
        
    }
}