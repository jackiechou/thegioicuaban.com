﻿using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Modules;
using CommonLibrary.Common;
using CommonLibrary.Entities.Content;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Portal;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using CommonLibrary.Data;
using CommonLibrary.Services.Url.FriendlyUrl;
namespace WebApp.modules.admin.tabs
{
    public partial class admin_front_tabs_add : System.Web.UI.Page
    {
        private static string small_icon_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/tab_images/small_icons";
        private static string large_icon_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/tab_images/large_icons";
        #region SESSION PROPERTIES ===========================
        protected string ApplicationId
        {
            get
            {
                if (Session["ApplicationId"] != null)
                    return Session["ApplicationId"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string UserId
        {
            get
            {
                if (Session["UserId"] != null)
                    return Session["UserId"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string RoleId
        {
            get
            {
                if (Session["RoleId"] != null)
                    return Session["RoleId"].ToString();
                else
                    return string.Empty;
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

        protected string VendorId
        {
            get
            {
                if (Session["VendorId"] != null)
                    return Session["VendorId"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string IsSuperUser
        {
            get
            {
                if (Session["IsSuperUser"] != null)
                    return Session["IsSuperUser"].ToString();
                else
                    return string.Empty;
            }
        }


        protected string UpdatePassword
        {
            get
            {
                if (Session["UpdatePassword"] != null)
                    return Session["UpdatePassword"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string IsDeleted
        {
            get
            {
                if (Session["IsDeleted"] != null)
                    return Session["IsDeleted"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string UserName
        {
            get
            {
                if (Session["UserName"] != null)
                    return Session["UserName"].ToString();
                else
                    return string.Empty;
            }
        }

        protected string HomeDirectory
        {
            get
            {
                if (Session["HomeDirectory"] != null)
                    return Session["HomeDirectory"].ToString();
                else
                    return string.Empty;
            }
        }
        #endregion ===========================================

        DataTable dt = new DataTable();
        TabController tab_obj = new TabController();
        ModuleClass module_obj = new ModuleClass();

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
                string qsuimode = Request.QueryString["mode"];
                if (string.IsNullOrEmpty(qsuimode) == false)
                {
                    _mode = (UIMode.mode)Enum.Parse(typeof(UIMode.mode), qsuimode);
                    if (_mode == UIMode.mode.add)
                    {
                        LoadPortalionList2DDL();
                        ShowContentItemList();
                        ShowLanguage();
                        ShowTreeNodes_TabList();
                        ShowListFileInFolder();
                    }
                    hdnWindowUIMODE.Value = _mode.ToString();
                }
                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Portal List ================================================================
        private void LoadPortalionList2DDL()
        {
            ddlPortalList.Items.Clear();

            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetList(); //select all the nodes from DB
            ddlPortalList.DataSource = dt;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedValue = Session["PortalId"].ToString();
            ddlPortalList.Enabled = false;
        }
        #endregion

        #region ContentItems ===========================================================
        private void ShowContentItemList()
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

        protected void ddlContentItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowListInContent();
            ShowListRouterUrl();            
        }

        #endregion =======================================================================

        #region SELECTED PAGE ==============================================================
        private void ShowListFileInFolder()
        {
            string Portal_HomePath = Server.MapPath("~/" + HomeDirectory);
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Portal_HomePath);
            string[] extensions = new[] { ".aspx", ".ascx", ".htm", ".html" };
            IEnumerable<FileInfo> files = CommonLibrary.Common.Utilities.FileSystemUtils.GetFilesByExtensions(dirInfo, extensions);
            ddlPathFile.Items.Clear();
            ddlPathFile.DataSource = files;
            ddlPathFile.DataBind();
            ddlPathFile.Items.Insert(0, new ListItem("chọn trang hiển thị", "0"));
            ddlPathFile.SelectedIndex = 0;
        }
        #endregion =======================================================================

        #region BEGIN RELATED CONTROLS IN THE SAME CONTENT ITEM ===============================================================
        protected void ShowListInContent()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            int ContentItemId = Convert.ToInt32(ddlContentItem.SelectedValue);
            List<aspnet_Tab> lst = TabController.GetListByPortalId_ContentItemId_IsSecure(PortalId, ContentItemId, true);
            ddlListInContent.Items.Clear();
            ddlListInContent.DataSource = lst;
            ddlListInContent.DataTextField = "TabName";
            ddlListInContent.DataValueField = "TabId";
            ddlListInContent.DataBind();
            ddlListInContent.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlListInContent.SelectedIndex = 0;
        }

        protected void ddlListInContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowListRouterUrl();
        }
        #endregion END RELATED CONTROLS IN THE SAME CONTENT ITEM ===============================================================

        #region ROUTE =========================================================
        private void ShowListRouterUrl()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            int ContentItemId = Convert.ToInt32(ddlContentItem.SelectedValue);
            string CultureCode = ddlCultureCode.SelectedValue;
            List<aspnet_Routes> lstRouter = RouteController.GetListByPortalIdCultureCodeStatus(PortalId, ContentItemId, CultureCode, true);
            ddlRouter.Items.Clear();
            ddlRouter.DataSource = lstRouter;
            ddlRouter.DataTextField = "Description";
            ddlRouter.DataValueField = "RouteId";
            ddlRouter.DataBind();
            ddlRouter.SelectedIndex = 0;
        }
        #endregion ============================================================        

        #region ShowLanguage ===========================================================
        private void ShowLanguage()
        {
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlCultureCode.Items.Clear();
            ddlCultureCode.DataSource = dt;
            ddlCultureCode.DataTextField = "CultureName";
            ddlCultureCode.DataValueField = "CultureCode";
            ddlCultureCode.DataBind();
            ddlCultureCode.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlCultureCode.SelectedIndex = 1;
        }
        #endregion =======================================================================

        #region ParentTab ===========================================================
        private void ShowTreeNodes_TabList()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            bool IsSecure = false; //chkIsSecure.Checked;
            TabController tabs_obj = new TabController();
            DataTable dt = tabs_obj.GetActiveListByPortalIdIsSecure(PortalId, IsSecure);

            ddlParentTab.Items.Clear();
            ddlParentTab.DataSource = dt;
            RecursiveFillTree(dt, 0);

            ddlParentTab.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlParentTab.SelectedIndex = 0;
            ddlParentTab.AutoPostBack = true;
        }

        int level = -1;
        private void RecursiveFillTree(DataTable dtParent, int parentID)
        {
            level++; //on the each call level increment 1
            System.Text.StringBuilder appender = new System.Text.StringBuilder();

            for (int j = 0; j < level; j++)
            {
                appender.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
            }
            if (level > 0)
            {
                appender.Append("|__");
            }

            DataView dv = new DataView(dtParent);
            dv.RowFilter = string.Format("ParentId = {0}", parentID);

            int i;

            if (dv.Count > 0)
            {
                for (i = 0; i < dv.Count; i++)
                {
                    //DROPDOWNLIST
                    ddlParentTab.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["TabName"].ToString()), dv[i]["TabID"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["TabID"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }
        #endregion =======================================================================

        #region IsSecure List ============================================================
        protected void chkIsSecure_CheckedChanged(object sender, EventArgs e)
        {
            ShowTreeNodes_TabList();
        }
        #endregion =======================================================================

        private int[] AddData()
        {
            int[] result = new int[2];
            if (Session["UserId"] != null && Session["UserId"].ToString() != string.Empty)
            {
                int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
                int ContentItemId = int.Parse(ddlContentItem.SelectedValue);
                string TabName = txt_TabName.Text;
                string Title = txt_Title.Text;
                int ParentId = int.Parse(ddlParentTab.SelectedValue);
                string Description = ddlPathFile.SelectedItem.Text;
                string Keywords = txt_Keywords.Text;
                string Url = txt_Url.Text;
                string TabPath = ddlPathFile.SelectedValue;
                int RouterId = int.Parse(ddlRouter.SelectedValue);
                decimal SiteMapPriority = Convert.ToDecimal(txtSiteMapPriority.Text);

                string PageHeadText = txt_PageHeadText.Text;
                string PageFooterText = txt_PageFooterText.Text;
                string PageControlBar = txt_PageControlBar.Text;
                string CssClass = txt_CssClass.Text;
                string CultureCode = ddlCultureCode.SelectedValue;

                #region xu ly thoi gian  ====================================================================================
                System.Globalization.DateTimeFormatInfo MyDateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo();
                MyDateTimeFormatInfo.ShortDatePattern = "dd/MM/yyyy";

                string StartDate = null, EndDate = null;

                if (txt_StartDate.Text != string.Empty && txt_StartDate.Text != "__/__/____")
                {
                    DateTime _start_date = DateTime.Parse(txt_StartDate.Text, MyDateTimeFormatInfo);
                    StartDate = _start_date.ToString("yyyy-MM-dd");
                }
                if (txt_EndDate.Text != string.Empty && txt_EndDate.Text != "__/__/____")
                {
                    DateTime _end_date = DateTime.Parse(txt_EndDate.Text, MyDateTimeFormatInfo);
                    EndDate = _end_date.ToString("yyyy-MM-dd");
                }
                if (txt_StartDate.Text != string.Empty && txt_StartDate.Text != "__/__/____"
                    && txt_EndDate.Text != string.Empty && txt_EndDate.Text != "__/__/____")
                {
                    DateTime _start_date = DateTime.Parse(txt_StartDate.Text, MyDateTimeFormatInfo);
                    DateTime _end_date = DateTime.Parse(txt_EndDate.Text, MyDateTimeFormatInfo);

                    if (DateTime.Compare(_start_date, _end_date) > 0)
                    {
                        string scriptCode = "<script>alert('Thời điểm bắt đầu phải nhỏ hơn thời điểm kết thúc');</script>";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                    }
                }
                #endregion ================================================================================================

                /*** UPLOAD *************************************************************************************************************/
                HttpPostedFile icon_file = FileIconInput.PostedFile;
                HttpPostedFile icon_large_file = FileIconLargeInput.PostedFile;
                string IconFileName = "", IconFileLargeName = "", SmallIconPath = string.Empty, LargeIconPath = string.Empty;
                if (icon_file.ContentLength > 0)
                {
                    IconFileName = module_obj.GetEncodeString(System.IO.Path.GetFileName(icon_file.FileName));
                    SmallIconPath = Server.MapPath(small_icon_path + "/" + IconFileName);
                    icon_file.SaveAs(SmallIconPath);
                }

                if (icon_large_file.ContentLength > 0)
                {
                    IconFileLargeName = System.IO.Path.GetFileName(icon_large_file.FileName);
                    LargeIconPath = Server.MapPath(large_icon_path + "/" + IconFileLargeName);
                    icon_file.SaveAs(LargeIconPath);
                }
                /************************************************************************************************************************/

                bool IsDeleted = true, IsVisible = true, DisableLink = false, DisplayTitle = false, IsSecure = false, PermanentRedirect = false;
                IsDeleted = chkIsDelete.Checked;
                IsVisible = chkIsVisible.Checked;
                DisableLink = chkDisableLink.Checked;
                DisplayTitle = chkDisplayTitle.Checked;
                PermanentRedirect = chkPermanentRedirect.Checked;
                result = tab_obj.Insert(PortalId, ContentItemId, ParentId, TabName, Title, CssClass,
                    IconFileName, IconFileLargeName, Description, Keywords, DisplayTitle, IsDeleted, IsVisible,
                    DisableLink, IsSecure, PermanentRedirect, SiteMapPriority, Url, TabPath, RouterId, PageHeadText, PageFooterText, PageControlBar, StartDate, EndDate, CultureCode, UserId);
            }
            return result;
        }


        protected void btnOkay_Click(object sender, EventArgs e)
        {
            int[] result = new int[2];
            int i = 0;
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                if (_mode == UIMode.mode.add)
                {
                    result = AddData();
                    
                    i = result[0];
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
                        MultiView1.ActiveViewIndex = 1;
                    }
                    else if (i == -3)
                    {
                        lblErrorMsg.Text = "Dữ liệu đã tồn tại";
                        ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                        MultiView1.ActiveViewIndex = 1;
                    }
                    else if (i == 1)
                    {
                        lblResult.Text = "Cập nhật thành công";
                        string forward_url = string.Empty;

                       
                            if (ddlRouter.SelectedItem.Text != "Mặc định")
                            {
                                if (ddlPathFile.SelectedIndex > 0 && ddlListInContent.SelectedIndex > 0)
                                {
                                    if (ddlPortalList.SelectedValue != string.Empty)
                                        forward_url += "portalId=" + ddlPortalList.SelectedValue;
                                    if (ddlListInContent.SelectedValue != string.Empty)
                                        forward_url += "&selectedTabId=" + ddlListInContent.SelectedValue;
                                    if (result[1].ToString() != string.Empty)
                                        forward_url += "&createdTabId=" + result[1].ToString();
                                    if (ddlPathFile.SelectedValue != string.Empty)
                                        forward_url += "&createdTabPath=" + ddlPathFile.SelectedValue;
                                    if (ddlContentItem.SelectedItem.Text != string.Empty)
                                        forward_url += "&contentItemKey=" + ddlContentItem.SelectedItem.Text + "&keyWords=" + StringHandleClass.convertTitle2Link(ddlContentItem.SelectedItem.Text);
                                    if (ddlCultureCode.SelectedValue != string.Empty)
                                        forward_url += "&lang=" + ddlCultureCode.SelectedValue;
                                    if (ddlRouter.SelectedValue != string.Empty)
                                        forward_url += "&router=" + ddlRouter.SelectedValue;


                                    Response.Redirect("~/modules/admin/tabs/admin_front_tab_control.aspx?" + forward_url);
                                }
                            }
                            else
                            {
                                TabController tab_obj = new TabController();
                                RouteController route_obj = new RouteController();
                                var data_route = route_obj.GetDetails(int.Parse(ddlRouter.SelectedValue));
                                string routerUrl = data_route.RouteUrl;

                                int resultupdate = tab_obj.UpdateLinkFrontPage(result[1].ToString(), routerUrl,data_route.RouteId, Session["UserId"].ToString());
                            }
                       

                        MultiView1.ActiveViewIndex = 1;
                        ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                        MultiView1.ActiveViewIndex = 0;
                    }
                }
            }
        }
    }
}