using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Entities.Portal;
using System.Data;
using CommonLibrary.Common;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Services.Url.FriendlyUrl;
using CommonLibrary.Data;
using System.IO;
using CommonLibrary.Modules;
using CommonLibrary.Entities.Content;

namespace WebApp.modules.admin.route
{
    public partial class admin_route_edit : System.Web.UI.Page
    {
        RouteController route_obj = new RouteController();
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

        protected string PortalId
        {
            get
            {
                if (Session["PortalId"] != null)
                    return Session["PortalId"].ToString();
                else
                    return string.Empty;
            }
        }

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
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
                        PopulatePortalList2DDL(PortalId);
                        PopulateContentItemList();
                        PopulateCulture2DDL();
                    }
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx = Convert.ToInt32(Request.QueryString["idx"]);
                        LoadData();
                    }
                    hdnWindowUIMODE.Value = _mode.ToString();
                }
                Session["update"] = Server.UrlEncode(System.DateTime.Now.ToString());
                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Portal List ===============================================================
        private void PopulatePortalList2DDL(string selected_value)
        {
            PortalController portal_obj = new PortalController();
            DataTable dtNodes = portal_obj.GetList();

            ddlPortalList.Items.Clear();
            ddlPortalList.DataSource = dtNodes;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            ddlPortalList.AutoPostBack = true;
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));           
            if (string.IsNullOrEmpty(selected_value))
            {
                ddlPortalList.SelectedIndex = 0;
                ddlPortalList.Enabled = true;
            }
            else
            {
                ddlPortalList.SelectedValue = selected_value;
                ddlPortalList.Enabled = false;
            }
        }
        protected void ddlPortalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateCulture2DDL();
        }
        #endregion ====================================================================

        #region Culture ======================================================
        protected void PopulateCulture2DDL()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            CultureClass culture_obj = new CultureClass();
            DataTable dt = culture_obj.GetListByPortalId(PortalId);

            ddlCultureList.Items.Clear();
            ddlCultureList.DataSource = dt;
            ddlCultureList.DataTextField = "CultureName";
            ddlCultureList.DataValueField = "CultureCode";
            ddlCultureList.DataBind();
            ddlCultureList.SelectedValue = "vi-VN";
            ddlCultureList.AutoPostBack = true;
        }
        protected void PopulateCulture2DDL(string selected_value)
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            CultureClass culture_obj = new CultureClass();
            DataTable dt = culture_obj.GetListByPortalId(PortalId);

            ddlCultureList.Items.Clear();
            ddlCultureList.DataSource = dt;
            ddlCultureList.DataTextField = "CultureName";
            ddlCultureList.DataValueField = "CultureCode";
            ddlCultureList.DataBind();
            ddlCultureList.SelectedValue = selected_value;
            ddlCultureList.AutoPostBack = true;
        }
        #endregion ============================================================

        #region ContentItems ===========================================================
        private void PopulateContentItemList()
        {
            ContentController content_controller_obj = new ContentController();
            int ContentTypeId = 1;
            DataTable dt = content_controller_obj.GetListByContentTypeId(ContentTypeId);

            ddlContentItem.Items.Clear();
            ddlContentItem.DataSource = dt;
            ddlContentItem.DataTextField = "ContentKey";
            ddlContentItem.DataValueField = "ContentItemID";
            ddlContentItem.DataBind();
            ddlContentItem.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlContentItem.SelectedIndex = 0;
        }
        private void PopulateContentItemList(string selected_value)
        {
            ContentController content_controller_obj = new ContentController();
            int ContentTypeId = 1;
            DataTable dt = content_controller_obj.GetListByContentTypeId(ContentTypeId);

            ddlContentItem.Items.Clear();
            ddlContentItem.DataSource = dt;
            ddlContentItem.DataTextField = "ContentKey";
            ddlContentItem.DataValueField = "ContentItemID";
            ddlContentItem.DataBind();
            ddlContentItem.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlContentItem.SelectedValue = selected_value;
        }
        #endregion =======================================================================

        private void LoadData()
        {
            var data_route = route_obj.GetDetails(_idx);
            PopulatePortalList2DDL(data_route.PortalId.ToString().Trim());
            PopulateCulture2DDL(data_route.CultureCode.ToString().Trim());
            PopulateContentItemList(data_route.ContentItemId.ToString().Trim());
            txtRouteName.Text = data_route.RouteName;
            lblPhysicalFile.Visible = true;
            lblPhysicalFile.Text = data_route.PhysicalFile;
            txtRouteUrl.Text = data_route.RouteUrl;
            txtRouteValueDictionary.Text = data_route.RouteValueDictionary;
            txtDescription.Text = data_route.Description;

            if (data_route.CheckPhysicalUrlAccess == true)
                chkCheckPhysicalUrlAccess.Checked = true;

            if (data_route.Discontinued == true)
                rdlDiscountinue.SelectedValue = "1";
            else
                rdlDiscountinue.SelectedValue = "0";
        }

        private int AddData()
        {
            int PortalId=Convert.ToInt32(ddlPortalList.SelectedValue);
            int ContentItemId = Convert.ToInt32(ddlContentItem.SelectedValue);
            string CultureCode=ddlCultureList.SelectedValue;
            string RouteName=txtRouteName.Text;
            string RouteUrl=txtRouteUrl.Text;          
            bool CheckPhysicalUrlAccess=false;
            if(chkCheckPhysicalUrlAccess.Checked==true)
                 CheckPhysicalUrlAccess=true;
            string RouteValueDictionary=txtRouteValueDictionary.Text;
            string Description=txtDescription.Text;
            bool Discontinued = false;
            if (rdlDiscountinue.SelectedValue == "1")
                Discontinued = true;

            string PhysicalFile = string.Empty;
            if (FileUpload_PhysicalFile.HasFile)
            {
                string Portal_HomePath = Session["HomeDirectory"].ToString();
                string FileName = FileUpload_PhysicalFile.PostedFile.FileName;
                PhysicalFile = "~/" + Portal_HomePath + "/" + FileName;
            }
            else
            {
                string scriptCode = "<script>alert('Vui lòng chọn file.');</script>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
            }

            int i = route_obj.Insert(PortalId,ContentItemId, CultureCode, RouteName, RouteUrl, PhysicalFile,
                                    CheckPhysicalUrlAccess, RouteValueDictionary, Description, Discontinued);
            return i;
        }

        private int UpdateData()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            int ContentItemId = Convert.ToInt32(ddlContentItem.SelectedValue);
            string CultureCode = ddlCultureList.SelectedValue;
            string RouteName = txtRouteName.Text;
            string RouteUrl = txtRouteUrl.Text;
           
            bool CheckPhysicalUrlAccess = false;
            if (chkCheckPhysicalUrlAccess.Checked == true)
                CheckPhysicalUrlAccess = true;
            string RouteValueDictionary = txtRouteValueDictionary.Text;
            string Description = txtDescription.Text;
            bool Discontinued = false;
            if (rdlDiscountinue.SelectedValue == "1")
                Discontinued = true;


            string PhysicalFile = string.Empty;
            if (FileUpload_PhysicalFile.HasFile)
            {
                string Portal_HomePath = Session["HomeDirectory"].ToString();
                string FileName = FileUpload_PhysicalFile.PostedFile.FileName;
                PhysicalFile = "~/" + Portal_HomePath + "/" + FileName;
            }
            else
                PhysicalFile = lblPhysicalFile.Text;


            int i = route_obj.Update(_idx, PortalId, ContentItemId, CultureCode, RouteName, RouteUrl, PhysicalFile,
                                    CheckPhysicalUrlAccess, RouteValueDictionary, Description, Discontinued);
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
                    switch (i)
                    {
                        case 0:
                            lblErrorMsg.Text = "Thêm dữ liệu không thành công";
                            //ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                        case 1:                               
                                int result_writefile = WriteFileRouter();
                            lblResult.Text = "Thêm dữ liệu thành công";
                            MultiView1.ActiveViewIndex = 1;
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                            break;
                        default:
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                    }
                }
                else if (_mode == UIMode.mode.edit)
                {
                    i = UpdateData();
                    switch (i)
                    {
                        case 0:
                            lblErrorMsg.Text = "Cập nhật không thành công";
                            //ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                        case 1:
                                
                                int result_writefile = WriteFileRouter();
                            lblResult.Text = "Cập nhật thành công";
                            MultiView1.ActiveViewIndex = 1;
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                            break;
                        default:
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                    }
                }
            }
        }

        private int WriteFileRouter()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            int ContentItemId = Convert.ToInt32(ddlContentItem.SelectedValue);
            string CultureCode = ddlCultureList.SelectedValue;
            bool Discontinued = false;
            if (rdlDiscountinue.SelectedValue == "1")
                Discontinued = true;

            string HomeDirectory = Session["HomeDirectory"].ToString();
           
            string RouterPath = Server.MapPath("~/routers");

            int result = RouteController.WriteFileRouter(PortalId,0, CultureCode, Discontinued, HomeDirectory, RouterPath, "app_routers.cs");
            return result;
        }

        private static string StringDirectionProcess(string RouteValueDictionary)
        {
            string optionparams = string.Empty;
                         
            if (RouteValueDictionary != "")
            {
                string[] items = RouteValueDictionary.Split(',');

                int numitem = 0;
                foreach (var item in items)
                {
                    string value = item.Split('=')[1];                   
                    string name = item.Split('=')[0];
                    optionparams += "{ \"" + name + "\"," + value + "}";
                    numitem++;
                    if (numitem < items.Length)
                    {
                        optionparams += ",";
                    }
                }
            }

            return optionparams;
         
        }
    }
}