using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GalleryLibrary;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using CommonLibrary.Modules;
using System.IO;
using CommonLibrary.Common.Utilities;

namespace WebApp.modules.admin.gallery
{
    public partial class admin_gallery_file_edit : System.Web.UI.Page
    {
        private static string upload_gallery_content_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_gallery_content_image_dir"];
        GalleryFile gallery_obj = new GalleryFile();
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
                        ShowTreeNodes_Topics();
                        CollectionList2DDL();
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

        #region tree node list - category =======================================
        private void ShowTreeNodes_Topics()
        {
            ddlTreeNode_Topics.Items.Clear(); //DROPDOWNLIST           

            List<Gallery_Topic> list = GalleryTopic.GetList('1');
            DataTable dt = LinqHelper.ToDataTable(list);
            RecursiveFillTree(dt, 0);

            ddlTreeNode_Topics.Items.Insert(0, new ListItem("- Root -", "")); //DROPDOWNLIST
            ddlTreeNode_Topics.SelectedIndex = 0;
            ddlTreeNode_Topics.AutoPostBack = true;
        }

        private void ShowTreeNodes_Topics(string select_value)
        {
            ddlTreeNode_Topics.Items.Clear(); //DROPDOWNLIST              
            List<Gallery_Topic> list = GalleryTopic.GetList('1');
            DataTable dt = LinqHelper.ToDataTable(list);
            RecursiveFillTree(dt, 0);

            ddlTreeNode_Topics.Items.Insert(0, new ListItem("- Root -", "")); //DROPDOWNLIST
            ddlTreeNode_Topics.SelectedValue = select_value;
            ddlTreeNode_Topics.AutoPostBack = true;
        }

        int level = -1;
        private void RecursiveFillTree(DataTable dtParent, int ParentId)
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
            dv.RowFilter = string.Format("ParentId = {0}", ParentId);

            int i;

            if (dv.Count > 0)
            {
                for (i = 0; i < dv.Count; i++)
                {
                    //DROPDOWNLIST
                    ddlTreeNode_Topics.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["Gallery_TopicName"].ToString()), dv[i]["Gallery_TopicId"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["Gallery_TopicId"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }
        protected void ddlTreeNode_Topics_SelectedIndexChanged(object sender, EventArgs e)
        {
            CollectionList2DDL();
        }
        #endregion ==============================================================

        #region Collection List =====================================================
        protected void CollectionList2DDL()
        {
            if (ddlTreeNode_Topics.SelectedValue != string.Empty)
            {
                GalleryCollection gallery_collection_obj = new GalleryCollection();
                int topicid = Convert.ToInt32(ddlTreeNode_Topics.SelectedValue);
                List<Gallery_Collection> list = gallery_collection_obj.GetList(topicid, '1');

                ddlCollection.Items.Clear();
                ddlCollection.DataSource = list;
                ddlCollection.DataTextField = "Title";
                ddlCollection.DataValueField = "CollectionId";
                ddlCollection.DataBind();                         
            }
            ddlCollection.Items.Insert(0, new ListItem("- Chọn -", ""));    
        }

        protected void CollectionList2DDL(string selected_value)
        {
            if (ddlTreeNode_Topics.SelectedValue != string.Empty)
            {
                GalleryCollection gallery_collection_obj = new GalleryCollection();
                int topicid = Convert.ToInt32(ddlTreeNode_Topics.SelectedValue);
                List<Gallery_Collection> list = gallery_collection_obj.GetList(topicid, '1');

                ddlCollection.Items.Clear();
                ddlCollection.DataSource = list;
                ddlCollection.DataTextField = "Title";
                ddlCollection.DataValueField = "CollectionId";
                ddlCollection.DataBind();
                ddlCollection.Items.Insert(0, new ListItem("- Chọn -", ""));
                ddlCollection.SelectedValue = selected_value;
            }
        }
        #endregion ============================================================

        private void LoadData()
        {
            CustomGalleryFiles file_obj = GalleryFile.GetDetails(_idx);
            ltrFileName.Text = file_obj.FileName;
            txtFileUrl.Text = file_obj.FileUrl;
            txtTags.Text = file_obj.Tags;
            txtDescription.Text = file_obj.Description;
            rdlStatus.SelectedValue = file_obj.Status.ToString();
            ShowTreeNodes_Topics(file_obj.Gallery_TopicId.ToString());
            string filename = file_obj.FileName;
            string fileurl = file_obj.FileUrl;
            if (filename != string.Empty)
            {
                rdlFileUpload.SelectedValue = "0";

                Uri requestUri = Context.Request.Url;
                string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
                imgPhoto.ImageUrl = baseUrl + "/" + upload_gallery_content_image_dir + "/" + file_obj.FileName;
            }
            if (filename == string.Empty && fileurl != string.Empty)
            {
                rdlFileUpload.SelectedValue = "1";
                imgPhoto.ImageUrl = fileurl;
            }
            ShowTreeNodes_Topics(file_obj.Gallery_TopicId.ToString());
            CollectionList2DDL(file_obj.CollectionId.ToString());
            ViewState["CollectionFileId"] = file_obj.CollectionFileId;
            ViewState["ListOrder"] = file_obj.ListOrder;
            ViewState["CreatedOnDate"] = file_obj.CreatedOnDate;
            ViewState["IPLog"] = file_obj.IPLog;
            ViewState["UserLog"] = file_obj.UserLog;
        }

        private bool AddData()
        {
            int CollectionId = Convert.ToInt32(ddlCollection.SelectedValue);
            string Description = txtDescription.Text;
            string User = Session["UserId"].ToString();
            string Status = rdlStatus.SelectedValue;
            string Tags = txtTags.Text;
            string FileName = string.Empty; string FileUrl = string.Empty;

            if (rdlFileUpload.SelectedValue == "0")
            {
                HttpPostedFile posted_file = File1.PostedFile;
                if (posted_file.ContentLength > 0)
                {
                    string dir_path = Server.MapPath("~/" + upload_gallery_content_image_dir);
                    if (!Directory.Exists(dir_path))
                        Directory.CreateDirectory(dir_path);
                    FileHandleClass file_obj = new FileHandleClass();
                    FileName = file_obj.uploadInputFile(posted_file, dir_path);
                }
                else
                    ShowMessageBox("Vui lòng chọn File.");
            }
            else
            {
                if(txtFileUrl.Text !=string.Empty)
                    FileUrl = txtFileUrl.Text;
                else
                    ShowMessageBox("Vui lòng chọn File.");
            }
            
            bool i = gallery_obj.InsertData(CollectionId, FileName, FileUrl, Description, User, Status, Tags);
            return i;
        }

        private bool UpdateData()
        {
            int CollectionId = Convert.ToInt32(ddlCollection.SelectedValue);
            int CollectionFileId = Convert.ToInt32(ViewState["CollectionFileId"].ToString());
            int ListOrder = Convert.ToInt32(ViewState["ListOrder"].ToString());
            DateTime CreatedOnDate = Convert.ToDateTime(ViewState["CreatedOnDate"].ToString());
            string IPLog = ViewState["IPLog"].ToString();
            string UserLog = ViewState["UserLog"].ToString();
            string Description = txtDescription.Text;
            string UserLastUpdate = Session["UserId"].ToString();
            string Status = rdlStatus.SelectedValue;
            string Tags = txtTags.Text;            
            
            string FileName = string.Empty, FileUrl = string.Empty;        
            if (rdlFileUpload.SelectedValue == "0")
            {
                HttpPostedFile posted_file = File1.PostedFile;
                if (posted_file.ContentLength > 0)
                {
                    string dir_path = Server.MapPath("~/" + upload_gallery_content_image_dir);
                   if (!Directory.Exists(dir_path))
                        Directory.CreateDirectory(dir_path);         
                    FileHandleClass file_obj = new FileHandleClass();
                    FileName = file_obj.uploadInputFile(posted_file, dir_path);
                    if (FileName != string.Empty)
                        DeleteFile(ltrFileName.Text, dir_path);
                }
                else
                    ShowMessageBox("Vui lòng chọn File.");
            }
            else{           
                if (txtFileUrl.Text != string.Empty)
                    FileUrl = txtFileUrl.Text;
                else
                    ShowMessageBox("Vui lòng chọn File.");
            }
            bool i = gallery_obj.UpdateData(_idx, CollectionFileId, CollectionId, FileName, FileUrl,
                                    ListOrder, Description, Tags, Status, CreatedOnDate, IPLog, UserLog, UserLastUpdate);           
            return i;
        }

        private void DeleteFile(string file_name, string dir_path)
        {            
            if (Directory.Exists(dir_path))
            {
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_handle_obj.deleteFile(file_name, dir_path);
            }
        }

        private void ShowMessageBox(string message)
        {
            string scriptCode = "<script>alert('" + message + "');</script>";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                bool i = false;
                if (_mode == UIMode.mode.add)
                {
                    i = AddData();
                    switch (i)
                    {
                        case false:
                            lblErrorMsg.Text = "Thêm dữ liệu không thành công";
                            //ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                        case true:
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
                        case false:
                            lblErrorMsg.Text = "Cập nhật không thành công";
                            //ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                        case true:
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
    }
}