using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using AjaxControlToolkit;
using CommonLibrary.Modules;
using CommonLibrary.Common;
using ArticleLibrary;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.articles
{
    public partial class admin_article_categories_edit : System.Web.UI.Page
    {

        ArticleCategoryController category_obj = new ArticleCategoryController();      

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
                        int PortalId = Convert.ToInt32(Session["PortalId"].ToString());
                        string CultureCode = "vi-VN";
                        string ParentId = "0";

                        PopulatePortalList2DDL();
                        PopulateCulture2DDL();
                        LoadStatus2RadioBtnList();
                        ShowTreeNodes(PortalId, CultureCode, ParentId);                        
                        hdnWindowUIMODE.Value = _mode.ToString();
                    }
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx =Convert.ToInt32(Request.QueryString["idx"]);
                        LoadData();
                        hdnWindowUIMODE.Value = _mode.ToString();
                    }                  

                }
                MultiView1.ActiveViewIndex = 0;

            }
        }

        #region Portal List ==================================================
        private void PopulatePortalList2DDL()
        {
            PortalController portal_obj = new PortalController();
            DataTable dtNodes = portal_obj.GetList();

            ddlPortalList.Items.Clear();
            ddlPortalList.DataSource = dtNodes;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));

            string PortalId = Session["PortalId"].ToString();
            if(!string.IsNullOrEmpty(PortalId))
            {
                if (PortalId == "0")
                {
                    ddlPortalList.SelectedIndex = 0;
                    ddlPortalList.Enabled = true;
                }
                else
                {
                    ddlPortalList.SelectedValue = PortalId;
                    ddlPortalList.Enabled = false;
                }

            }
        }
        private void PopulatePortalList2DDL(string selected_value)
        {
            PortalController portal_obj = new PortalController();
            DataTable dtNodes = portal_obj.GetList();

            ddlPortalList.Items.Clear();
            ddlPortalList.DataSource = dtNodes;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedValue = selected_value;
        }
        #endregion ===============================================================

        #region Culture ======================================================
        protected void PopulateCulture2DDL()
        {
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlCultureList.Items.Clear();
            ddlCultureList.DataSource = dt;
            ddlCultureList.DataTextField = "CultureName";
            ddlCultureList.DataValueField = "CultureCode";
            ddlCultureList.DataBind();
            ddlCultureList.SelectedValue = "vi-VN";
        }
        protected void PopulateCulture2DDL(string selected_value)
        {
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlCultureList.Items.Clear();
            ddlCultureList.DataSource = dt;
            ddlCultureList.DataTextField = "CultureName";
            ddlCultureList.DataValueField = "CultureCode";
            ddlCultureList.DataBind();
            ddlCultureList.SelectedValue = selected_value;
        }
        #endregion ============================================================

        #region tree node list - category =======================================
        private void ShowTreeNodes(int PortalId, string CultureCode, string ParentId)
        {
            lstTreeNode.Items.Clear(); //LISTBOX  
            DataTable dtNodes = category_obj.GetActiveList(PortalId, CultureCode); //select all the nodes from DB
            RecursiveFillTree(dtNodes, 0);

            lstTreeNode.Items.Insert(0, new ListItem("- Root -", "0")); //LISTBOX
            lstTreeNode.SelectedValue = ParentId;
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
                    lstTreeNode.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["CategoryName"].ToString()), dv[i]["CategoryId"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["CategoryId"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }
        #endregion ==============================================================

        #region Status ============================================================
        protected void LoadStatus2RadioBtnList()
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Published", "2"));
            lstColl.Add(new ListItem("Active", "1"));
            lstColl.Add(new ListItem("InActive", "0"));

            rdlStatus.DataSource = lstColl;
            rdlStatus.DataTextField = "Text";
            rdlStatus.DataValueField = "Value";
            rdlStatus.DataBind();
            rdlStatus.SelectedIndex = 0; // Select the first item  
            rdlStatus.AutoPostBack = false;
        }
        protected void LoadStatus2RadioBtnList(string selected_value)
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Published", "2"));
            lstColl.Add(new ListItem("Active", "1"));
            lstColl.Add(new ListItem("InActive", "0"));

            rdlStatus.DataSource = lstColl;
            rdlStatus.DataTextField = "Text";
            rdlStatus.DataValueField = "Value";
            rdlStatus.DataBind();
            rdlStatus.SelectedValue = selected_value;
            rdlStatus.AutoPostBack = false;
        }
        #endregion ================================================================

        protected void LoadData()
        {           
            dt = category_obj.GetDetailByID(_idx);
            string PortalId = dt.Rows[0]["PortalId"].ToString();
            string CultureCode = dt.Rows[0]["CultureCode"].ToString();
            string ParentId = dt.Rows[0]["ParentId"].ToString();
            string CategoryCode = dt.Rows[0]["CategoryCode"].ToString();
            string CategoryName = dt.Rows[0]["CategoryName"].ToString();
            string Description = dt.Rows[0]["Description"].ToString();
            //string SortKey = dt.Rows[0]["SortKey"].ToString();

            txtCode.Text = CategoryCode;
            txtName.Text = CategoryName;
            txtDescription.Text = Description; 

            LoadStatus2RadioBtnList(dt.Rows[0]["Status"].ToString());
            
            string dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/article_category_images";
            ViewState["image"] = dt.Rows[0]["CategoryImage"].ToString();
            imgPhoto.ImageUrl = dir_path + "/" + dt.Rows[0]["CategoryImage"].ToString();
            /***** PARENT ID ***********************************************/
            ViewState["parentid"] = dt.Rows[0]["ParentId"].ToString();
            ShowTreeNodes(Convert.ToInt32(PortalId), CultureCode, ParentId);
            /******************************************************************/

            PopulatePortalList2DDL(PortalId);
            PopulateCulture2DDL(CultureCode);           
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
                        MultiView1.ActiveViewIndex = 1;
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
                else if (_mode == UIMode.mode.edit)
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

        private int AddData()
        {        
            int parentid = Convert.ToInt32(lstTreeNode.SelectedValue);
            int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
            string culturecode = ddlCultureList.SelectedValue;
            string code = txtCode.Text;
            string cate_name = txtName.Text;
            string description = txtDescription.Text;            
            string status = rdlStatus.SelectedValue;           

            /*** UPLOAD ****************************************************************************************************************************************/
            HttpPostedFile myfile = FileInput.PostedFile;   
            string file_name = null;
            if (myfile != null)
            {
                if (myfile.ContentLength > 0)
                {
                    string dir_img_path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/article_category_images");
                    FileHandleClass file_handle_obj = new FileHandleClass();
                    file_name = file_handle_obj.uploadInputFile(myfile, dir_img_path);
                }
            }
            /***************************************************************************************************************************************************/

            string userid = Session["UserId"].ToString();
            int i = category_obj.Insert(userid, portalid, culturecode, parentid, code, cate_name, file_name, description, status);
            return i;
        }

        protected int UpdateData()
        {         
            string strParentID = null;
            if (lstTreeNode.SelectedValue == _idx.ToString())
            {
                strParentID = ViewState["parentid"].ToString();
            }
            else
            {
                strParentID = lstTreeNode.SelectedValue;
            }
            int parentid = Convert.ToInt32(strParentID);
            int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
            string culturecode = ddlCultureList.SelectedValue;
            string code = txtCode.Text;
            string name = txtName.Text;
            string description = txtDescription.Text;
            string status = rdlStatus.SelectedValue;             

            HttpPostedFile myfile = FileInput.PostedFile;
            string file_name = null;
            if (myfile.FileName != string.Empty)
            {
                string dir_img_path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/article_category_images");
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_name = file_handle_obj.uploadInputFile(myfile, dir_img_path);
            }
            else
            {
                file_name = ViewState["image"].ToString();
            }
            string userid = Session["UserId"].ToString();
            int i = category_obj.Update(userid, portalid, culturecode, _idx, parentid, code, name, file_name, description, status);            
            return i;
        }

    }
}