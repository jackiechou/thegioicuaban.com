using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using GalleryLibrary;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Modules;
using System.IO;
namespace WebApp.modules.admin.gallery
{
    public partial class admin_gallery_collection_edit : System.Web.UI.Page
    {
        GalleryCollection gallery_collection_obj = new GalleryCollection();
        private static string upload_gallery_collection_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_gallery_collection_image_dir"];
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

            ddlTreeNode_Topics.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlTreeNode_Topics.SelectedIndex = 0;

        }

        private void ShowTreeNodes_Topics(string select_value)
        {
            ddlTreeNode_Topics.Items.Clear(); //DROPDOWNLIST              
            List<Gallery_Topic> list = GalleryTopic.GetList('1');
            DataTable dt = LinqHelper.ToDataTable(list);
            RecursiveFillTree(dt, 0);

            ddlTreeNode_Topics.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlTreeNode_Topics.SelectedValue = select_value;

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
        #endregion ==============================================================

        private void LoadData()
        { 
            Gallery_Collection collection_obj = gallery_collection_obj.GetDetails(_idx);
            txtTitle.Text = collection_obj.Title;
            ltrFileName.Text = collection_obj.IconFile;
            txtTags.Text = collection_obj.Tags;
            txtDescription.Text = collection_obj.Description;
            rdlStatus.SelectedValue = collection_obj.Status.ToString();
            ShowTreeNodes_Topics(collection_obj.TopicId.ToString());

            string file_name = string.Empty;
            if (collection_obj.IconFile != string.Empty)
            {
                Uri requestUri = Context.Request.Url;
                string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
                imgPhoto.ImageUrl = baseUrl + "/" + upload_gallery_collection_image_dir + "/" + collection_obj.IconFile;
            }
            
            ViewState["ListOrder"] = collection_obj.ListOrder;
            ViewState["IPLog"] = collection_obj.IPLog;
            ViewState["CreatedOnDate"] = collection_obj.CreatedOnDate;
            ViewState["UserLog"] = collection_obj.UserLog;
        }

        private bool AddData()
        {
            string IconFile=string.Empty;            
            int TopicId = Convert.ToInt32(ddlTreeNode_Topics.SelectedValue);
            string Title = txtTitle.Text;
            string Description = txtDescription.Text;
            string Status = rdlStatus.SelectedValue;
            string Tags = txtTags.Text;
            string Url = txtUrl.Text;
            string UserLog = Session["UserId"].ToString();
            
            HttpPostedFile posted_file = File1.PostedFile;
            if (posted_file.ContentLength > 0)
            {
                string dir_path = Server.MapPath("~/" + upload_gallery_collection_image_dir);
                if (Directory.Exists(dir_path))
                    Directory.CreateDirectory(dir_path);             

                FileHandleClass collection_obj = new FileHandleClass();
                IconFile = collection_obj.uploadInputFile(posted_file, dir_path);              
            }           
            bool i = gallery_collection_obj.InsertData(TopicId, Title, IconFile, Description, Tags, Url, UserLog, Status);
            return i;
        }

        private bool UpdateData()
        {
            string IconFile = string.Empty;
            string dir_path = Server.MapPath("~/" + upload_gallery_collection_image_dir);
            HttpPostedFile posted_file = File1.PostedFile;
            if (posted_file.ContentLength > 0)
            {
                if (!Directory.Exists(dir_path))
                    Directory.CreateDirectory(dir_path);    
                FileHandleClass collection_obj = new FileHandleClass();
                IconFile = collection_obj.uploadInputFile(posted_file, dir_path);               
                DeleteFile(ltrFileName.Text, dir_path);                
            }
            else
                IconFile = ltrFileName.Text;
            int TopicId = Convert.ToInt32(ddlTreeNode_Topics.SelectedValue);
            string Title = txtTitle.Text;            
            string Status = rdlStatus.SelectedValue;
            string Tags = txtTags.Text;
            string Url = txtUrl.Text;
            string UserLastUpdate = Session["UserId"].ToString();
            string Description = txtDescription.Text;
            int ListOrder = Convert.ToInt32(ViewState["ListOrder"].ToString());
            string IPLog = ViewState["IPLog"].ToString();
            DateTime CreatedOnDate = Convert.ToDateTime(ViewState["CreatedOnDate"].ToString());
            string UserLog = ViewState["UserLog"].ToString();

            bool i = gallery_collection_obj.UpdateData(_idx, TopicId, Title, IconFile, Description, ListOrder, Tags, Url,
                                                    UserLog, UserLastUpdate, CreatedOnDate, IPLog, Status);
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