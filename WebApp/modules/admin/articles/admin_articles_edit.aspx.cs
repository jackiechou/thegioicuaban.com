using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using AjaxControlToolkit;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using CommonLibrary.Modules;
using CommonLibrary.Common;
using ArticleLibrary;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.articles
{
    public partial class admin_articles_edit : System.Web.UI.Page
    {
        ModuleClass modules_obj = new ModuleClass();
        ArticleController article_obj = new ArticleController();
        ArticleCategoryController articleCate_obj = new ArticleCategoryController();
        DataTable dt = new DataTable();

        private static string upload_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"];
        private static string upload_front_image_dir = "~/" + upload_image_dir + "/article_images/front_images";
        private static string upload_main_image_dir = "~/" + upload_image_dir + "/article_images/main_images";

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
                        PopulatePortalList2DDL();
                        PopulateCulture2DDL();
                        ShowTreeNodes();
                        LoadStatus2RadioBtnList();
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
            ////btnOkay.OnClientClick = String.Format("this.disabled=true; __doPostBack('{0}','');", btnOkay.UniqueID);
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
            ddlPortalList.SelectedIndex = 0;
            ddlPortalList.AutoPostBack = true;
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

        private void LoadData()
        {
            dt = article_obj.GetDetailByID(_idx);
            string PortalId = dt.Rows[0]["PortalId"].ToString();
            string CultureId = dt.Rows[0]["CultureCode"].ToString();
            string CategoryCode = dt.Rows[0]["CategoryCode"].ToString();
            string Title = dt.Rows[0]["Title"].ToString();
            string Headline = dt.Rows[0]["Headline"].ToString();
            string FrontImage = dt.Rows[0]["FrontImage"].ToString();
            string MainImage = dt.Rows[0]["MainImage"].ToString();
            string Abstract = dt.Rows[0]["Abstract"].ToString();
            string MainText = dt.Rows[0]["MainText"].ToString();
            string Source = dt.Rows[0]["Source"].ToString();
            string Status = dt.Rows[0]["Status"].ToString();

            PopulatePortalList2DDL(PortalId);
            PopulateCulture2DDL(CultureId);
            ShowTreeNodes(CategoryCode);
            txtTitle.Text = Title;
            txtHeadline.Text = Headline;
            string front_image = FrontImage;
            if (front_image.Length > 0)
            {
                if (front_image.Substring(0, 3) == "http")
                {
                    front_Img.ImageUrl = front_image;
                }
                else
                {
                    string front_dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/article_images/front_images";
                    string file_path = "~/" + front_dir_path + "/" + FrontImage;
                    front_Img.Width = 50;
                    front_Img.Height = 50;
                    front_Img.ImageUrl = file_path;
                }
            }
            ViewState["FrontImage"] = FrontImage;
            ViewState["MainImage"] = MainImage;

            txtAbstract.Text = Abstract;
            FCKeditorContent.Value = MainText;
            txtSource.Text = Source;
            LoadStatus2RadioBtnList(Status);
        }

        #region tree node list - category =======================================
        private void ShowTreeNodes()
        {
            int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
            string culturecode = ddlCultureList.SelectedValue;
            ddlTreeNode_Category.Items.Clear(); //DROPDOWNLIST        

            ArticleCategoryController objTree = new ArticleCategoryController();
            DataTable dtNodes = objTree.GetActiveList(portalid, culturecode); //select all the nodes from DB
            RecursiveFillTree(dtNodes, 0);

            ddlTreeNode_Category.Items.Insert(0, new ListItem("-Chọn nhóm tin tức-", "0")); //DROPDOWNLIST
            ddlTreeNode_Category.SelectedIndex = 0;
        }
        private void ShowTreeNodes(string selected_value)
        {
            int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
            string culturecode = ddlCultureList.SelectedValue;
            ddlTreeNode_Category.Items.Clear(); //DROPDOWNLIST        

            ArticleCategoryController objTree = new ArticleCategoryController();
            DataTable dtNodes = objTree.GetActiveList(portalid, culturecode); //select all the nodes from DB
            RecursiveFillTree(dtNodes, 0);

            ddlTreeNode_Category.Items.Insert(0, new ListItem("-Chọn nhóm tin tức-", "0")); //DROPDOWNLIST
            //ddlTreeNode_Category.SelectedIndex = 0;
            ddlTreeNode_Category.SelectedValue = selected_value;
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
                    ddlTreeNode_Category.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["CategoryName"].ToString()), dv[i]["CategoryCode"].ToString()));
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
            string userid = Session["UserId"].ToString();
            int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
            string culturecode = ddlCultureList.SelectedValue;
            string code = ddlTreeNode_Category.SelectedValue;
            string title = txtTitle.Text;
            string headline = txtHeadline.Text;
            string abstract_info = txtAbstract.Text;
            string contents = FCKeditorContent.Value;
            string source = txtSource.Text;
            string status = rdlStatus.SelectedValue;
            string navigateurl = txtNavigateUrl.Text;
            /*** UPLOAD ****************************************************************************************************************************************/
            string front_img = "", main_img = "";
            HttpPostedFile myfile = FileInput.PostedFile;
            if (myfile.FileName != string.Empty)
            {
                string front_img_path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/article_images/front_images");
                string main_img_path = Server.MapPath("~/" + System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"] + "/article_images/main_images");

                FileHandleClass file_handle_obj = new FileHandleClass();
                string[] FileImg = new String[2];
                FileImg = file_handle_obj.uploadFrontMainInputFile(myfile, front_img_path, main_img_path, 120, 120);
                main_img = FileImg[0].ToString();
                front_img = FileImg[1].ToString();
            }
            /*************************************************************************************************************************************************/
            ArticleController article_obj = new ArticleController();
            int i = article_obj.Insert(userid, portalid, culturecode, code, title, headline, abstract_info, front_img, main_img, contents, source, navigateurl, status);
            return i;
        }

        protected int UpdateData()
        {
            string userid = Session["UserId"].ToString();
            int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
            string culturecode = ddlCultureList.SelectedValue;
            string code = ddlTreeNode_Category.SelectedValue;
            string title = txtTitle.Text;
            string headline = txtHeadline.Text;
            string abstract_info = txtAbstract.Text;
            string contents = FCKeditorContent.Value;
            string source = txtSource.Text;
            string status = rdlStatus.SelectedValue;
            string navigateurl = txtNavigateUrl.Text;
            /*** UPLOAD ****************************************************************************************************************************************/
            string front_img = "", main_img = "";
            HttpPostedFile myfile = FileInput.PostedFile;
            if (myfile.FileName != string.Empty)
            {
                string front_img_path = Server.MapPath(upload_front_image_dir);
                string main_img_path = Server.MapPath(upload_main_image_dir);

                FileHandleClass file_handle_obj = new FileHandleClass();
                string[] FileImg = new String[2];
                FileImg = file_handle_obj.uploadFrontMainInputFile(myfile, front_img_path, main_img_path, 120, 120);
                main_img = FileImg[0].ToString();
                front_img = FileImg[1].ToString();
                front_Img.ImageUrl = upload_front_image_dir + "/" + front_img;

                string Orginal_front_image = ViewState["FrontImage"].ToString();
                string Orginal_main_image = ViewState["MainImage"].ToString();
                file_handle_obj.DeleteFile(Orginal_front_image, front_img_path);
                file_handle_obj.DeleteFile(Orginal_main_image, main_img_path);
            }
            else
            {
                main_img = ViewState["MainImage"].ToString();
                front_img = ViewState["FrontImage"].ToString();
            }
            /*************************************************************************************************************************************************/

            int i = article_obj.Update(userid, portalid, culturecode, _idx, code, title, headline, abstract_info, front_img, main_img, contents, source, navigateurl, status);

            return i;
        }
    }
}