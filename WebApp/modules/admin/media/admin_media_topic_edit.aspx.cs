using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using CommonLibrary.Modules;
using CommonLibrary.Entities.Tabs;
using System.IO;
using MediaLibrary;

namespace WebApp.modules.admin.media
{
    public partial class admin_media_topic_edit : System.Web.UI.Page
    {
        private static string upload_image_dir = System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"];
        private static string upload_front_image_dir = "~/" + upload_image_dir + "/media_images/topic_images";
        
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
                        PopulateMediaTypeList2DDL();
                        PopulateMediaTopicList2DDL();
                        PopulateStatus2DDL();
                    } 
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx = Convert.ToInt32(Request.QueryString["idx"]);
                        LoadData();
                    }
                    hdnWindowUIMODE.Value = _mode.ToString();
                }

                MultiView1.ActiveViewIndex = 0;
            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        #region Media Types ========================================================
        protected void PopulateMediaTypeList2DDL()
        {
            MediaTypes media_type_obj = new MediaTypes();
            List<Media_Types> lst = media_type_obj.GetListByStatus("1");

            ddlMediaTypeList.Items.Clear();
            ddlMediaTypeList.DataSource = lst;
            ddlMediaTypeList.DataTextField = "TypeName";
            ddlMediaTypeList.DataValueField = "TypeId";
            ddlMediaTypeList.DataBind();
            ddlMediaTypeList.SelectedIndex = 0;
            ddlMediaTypeList.AutoPostBack = true;
        }

        protected void PopulateMediaTypeList2DDL(string selected_value)
        {
            MediaTypes media_type_obj = new MediaTypes();
            List<Media_Types> lst = media_type_obj.GetListByStatus("1");

            ddlMediaTypeList.Items.Clear();
            ddlMediaTypeList.DataSource = lst;
            ddlMediaTypeList.DataTextField = "TypeName";
            ddlMediaTypeList.DataValueField = "TypeId";
            ddlMediaTypeList.DataBind();
            ddlMediaTypeList.SelectedValue = selected_value;
            ddlMediaTypeList.AutoPostBack = true;
        }
        protected void ddlMediaTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateMediaTopicList2DDL();            
        }
        #endregion ============================================================

        #region Media Topics ======================================================
        private void PopulateMediaTopicList2DDL()
        {
            ddlMediaTopicList.Items.Clear(); //DROPDOWNLIST  
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTopics media_topic_obj = new MediaTopics();
            List<Media_Topics> lst = media_topic_obj.GetListByTypeStatus(TypeId,"1");
            if (lst.Count > 0)
            {
                DataTable dt = CommonLibrary.Data.DataUtils.ConvertToDataTable(lst);
                RecursiveFillTree(dt, 0);
            }
            ddlMediaTopicList.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlMediaTopicList.SelectedIndex = 0;

        }

        private void PopulateMediaTopicList2DDL(string select_value)
        {
            ddlMediaTopicList.Items.Clear(); //DROPDOWNLIST  
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTopics media_topic_obj = new MediaTopics();
            List<Media_Topics> lst = media_topic_obj.GetListByTypeStatus(TypeId, string.Empty);
            if (lst.Count > 0)
            {
                DataTable dt = CommonLibrary.Data.DataUtils.ConvertToDataTable(lst);
                RecursiveFillTree(dt, 0);
            }
            ddlMediaTopicList.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlMediaTopicList.SelectedValue = select_value;

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
                    ddlMediaTopicList.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["Name"].ToString()), dv[i]["TopicId"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["TopicId"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }
        #endregion ============================================================ 
        
        #region Status ==================================================
        protected void PopulateStatus2DDL()
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Active", "1"));
            lstColl.Add(new ListItem("InActive", "0"));

            rdlStatus.DataSource = lstColl;
            rdlStatus.DataTextField = "Text";
            rdlStatus.DataValueField = "Value";
            rdlStatus.DataBind();
            //rdlStatus.Items.Insert(0, new ListItem("Chọn trạng thái", ""));
            rdlStatus.SelectedIndex = 0; // Select the first item
            rdlStatus.AutoPostBack = true;
        }
        protected void PopulateStatus2DDL(string selected_value)
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Active", "1"));
            lstColl.Add(new ListItem("InActive", "0"));

            rdlStatus.DataSource = lstColl;
            rdlStatus.DataTextField = "Text";
            rdlStatus.DataValueField = "Value";
            rdlStatus.DataBind();
            //rdlStatus.Items.Insert(0, new ListItem("Chọn trạng thái", ""));
            rdlStatus.SelectedValue = selected_value; // Select the first item
            rdlStatus.AutoPostBack = true;
        }
        #endregion ======================================================

        private void LoadData()
        {
            MediaTopics media_topic_obj = new MediaTopics();
            Media_Topics topic_obj = media_topic_obj.GetDetailById(_idx);
            txtTopicName.Text = topic_obj.Name;
            txtDescription.Text = topic_obj.Description;
            string Status = topic_obj.Status;
            string ParentId = topic_obj.ParentId.ToString();
            PopulateMediaTypeList2DDL(topic_obj.TypeId.ToString());
            PopulateMediaTopicList2DDL(ParentId);
            PopulateStatus2DDL(Status);
            string Photo = topic_obj.Photo;
            imgPhoto.ImageUrl = upload_front_image_dir + "/" + Photo;
            imgPhoto.Height = 70;
            imgPhoto.Width = 50;
            ViewState["Photo"] = Photo;
        }

        private int AddData()
        {
            string UserId = Session["UserId"].ToString();
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            string Description = txtDescription.Text;
            string TopicName = txtTopicName.Text;
            string Status = rdlStatus.SelectedValue;
            int ParentId = Convert.ToInt32(ddlMediaTopicList.SelectedValue);

            /*** UPLOAD ************************************************************************************************************/
            string[] FileImg = new String[2];
            string Photo = string.Empty;
            string front_path = Server.MapPath(upload_front_image_dir);
            HttpPostedFile myfile = myFile.PostedFile;
            if (myfile.ContentLength > 0)
            {
                if (!System.IO.Directory.Exists(front_path))
                    System.IO.Directory.CreateDirectory(front_path);

                FileHandleClass file_obj = new FileHandleClass();
                Photo = file_obj.uploadFixedInputFile(myfile, front_path, 120, 120);
            }
            ////========================================================================================================================

            MediaTopics media_topic_obj = new MediaTopics();
            int i = media_topic_obj.Insert(UserId,TypeId, ParentId, TopicName, Photo, Description, Status);
            return i;
        }

        private int UpdateData()
        {
            string UserId = Session["UserId"].ToString();
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            string TopicName = txtTopicName.Text;
            string Description = txtDescription.Text;
            string Status = rdlStatus.SelectedValue;
            int ParentId = Convert.ToInt32(ddlMediaTopicList.SelectedValue);

            /*** UPLOAD ************************************************************************************************************/
            string[] FileImg = new String[2];
            string Photo = string.Empty;
            string front_path = Server.MapPath(upload_front_image_dir);
            string orginal_filename = ViewState["Photo"].ToString();
            HttpPostedFile myfile = myFile.PostedFile;
            if (myfile.ContentLength > 0)
            {
                if (System.IO.Directory.Exists(front_path))
                {
                    FileHandleClass file_obj = new FileHandleClass();
                    Photo = file_obj.uploadFixedInputFile(myfile, front_path, 120, 120);
                    file_obj.deleteFile(orginal_filename, front_path);
                }
                else
                {
                    string scriptCode = "<script>alert('Đường dẫn không tồn tại.');</script>";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptName", scriptCode);
                }
            }
            else
            {
                Photo = ViewState["Photo"].ToString();
            }
            ////========================================================================================================================

            MediaTopics media_topic_obj = new MediaTopics();            
            int i = media_topic_obj.Update(UserId,TypeId, _idx, ParentId, TopicName, Photo, Description, Status);
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
    }
}