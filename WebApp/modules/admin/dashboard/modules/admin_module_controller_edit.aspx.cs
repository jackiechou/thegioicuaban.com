using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Modules.Dashboard.Components.Modules;

namespace WebApp.modules.admin.dashboard.modules
{
    public partial class admin_module_controller_edit : System.Web.UI.Page
    {
        ModuleController module_control_obj = new ModuleController();
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
                    if (_mode == UIMode.mode.add)
                    {
                        string module_id = Request.QueryString["module_id"];
                        ViewState["module_id"] = module_id;
                        LoadModuleList2DDL(module_id);
                    }
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx = Convert.ToInt32(Request.QueryString["idx"]);
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

        protected void LoadControlTypeList2DDL(string selected_value)
        {            
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Tab", "1"));
            lstColl.Add(new ListItem("Module", "2"));


            ddlControlTypeList.DataSource = lstColl;
            ddlControlTypeList.DataTextField = "Text";
            ddlControlTypeList.DataValueField = "Value";
            ddlControlTypeList.DataBind();
            ddlControlTypeList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlControlTypeList.SelectedValue = selected_value;          
        }

        protected void LoadModuleList2DDL(string selected_value)
        {
            Modules module_obj = new Modules();
            DataTable dtNodes = module_obj.GetAll();
            ddlModules.Items.Clear();
            ddlModules.DataSource = dtNodes;
            ddlModules.DataTextField = "ModuleTitle";
            ddlModules.DataValueField = "ModuleID";
            ddlModules.DataBind();
            ddlModules.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlModules.SelectedValue = selected_value;
        }

        private void LoadData()
        {           
            DataTable dt = module_control_obj.GetDetails(_idx);

            LoadModuleList2DDL(dt.Rows[0]["ModuleID"].ToString());
            txtControlTitle.Text = dt.Rows[0]["ControlTitle"].ToString();
            txtControlKey.Text = dt.Rows[0]["ControlKey"].ToString();
           
            txtControlSrc.Text = dt.Rows[0]["ControlSrc"].ToString();
            txtControlSrc.Visible = true;
            LoadControlTypeList2DDL(dt.Rows[0]["ControlType"].ToString());
                   
            IconFile_Image.ImageUrl = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]
                    + "/module_images/" + dt.Rows[0]["IconFile"].ToString());
            IconFile_Image.Width = 50;
            IconFile_Image.Height = 50;   
        }

        private int AddData()
        {            
            int ModuleID =Convert.ToInt32(ViewState["module_id"].ToString());
            string Title = txtControlTitle.Text;
            int ControlType = int.Parse(ddlControlTypeList.SelectedValue);
            string ControlKey = txtControlKey.Text;
            string physical_path = Server.MapPath(ControlSrc_File.Value);
            string ControlSrc = GetVirtualPath(physical_path);
            //string ControlSrc = txtControlSrc.Text;
            //Server.HtmlEncode(Request.CurrentExecutionFilePath)
            //string Physical_ControlSrc=  ControlSrcFile.Value;
            //string ControlSrc = GetVirtualPath(Physical_ControlSrc.Value);
            //  Uri uriSiteRoot = new Uri(context.Request.Url.GetLeftPart(UriPartial.Authority));
            //string ControlSrc = uriSiteRoot + "/" + Physical_ControlSrc;


            /*** UPLOAD ****************************************************************************************************************************************/
            HttpPostedFile file = IconFile.PostedFile;
            string Icon_FileName = "";
            if (IconFile != null & IconFile.Value!=string.Empty)
            {
                Icon_FileName = System.IO.Path.GetFileName(file.FileName);
                string savePath = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]
                    + "/module_images/" + Icon_FileName);
                file.SaveAs(savePath);
                
            }
            /*******************************************************************************************************************************************/
            
            ModuleController module_control_obj = new ModuleController();
            int result = module_control_obj.Insert(ModuleID, Title, ControlKey, ControlSrc, ControlType, Icon_FileName);
            return result;
        }

        private int UpdateData()
        {           
            string Title = txtControlTitle.Text;
            int ModuleID = int.Parse(ddlModules.SelectedValue);
            int ControlType = int.Parse(ddlControlTypeList.SelectedValue);
            string ControlKey = txtControlKey.Text;

            string ControlSrc = string.Empty;
            if (ControlSrc_File.Value != string.Empty)
            {
                ControlSrc = GetVirtualPath(ControlSrc_File.Value);
            }
            else
            {
                ControlSrc = txtControlSrc.Text;
            }
            

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
            int result = module_control_obj.Update(ModuleID, _idx, Title, ControlKey, ControlSrc, ControlType, Icon_FileName);

            return result;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            //try
            //{
                System.Threading.Thread.Sleep(2000);

                if (!IsRefresh)
                {
                    if (_mode == UIMode.mode.add)
                    {
                        int result = AddData();
                        MultiView1.ActiveViewIndex = 1;

                        //if (result == 1)                                                 
                        //    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);                                                   
                        //else                       
                        //    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);                      
                    }                
                    
                    if(_mode == UIMode.mode.edit)
                    {
                        int result = UpdateData();
                        MultiView1.ActiveViewIndex = 1;

                        if (result == 1)
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                        else
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);                      

                    }                    
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