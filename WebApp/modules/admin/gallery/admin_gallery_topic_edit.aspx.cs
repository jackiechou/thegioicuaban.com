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

namespace WebApp.modules.admin.gallery
{
    public partial class admin_gallery_topic_edit : System.Web.UI.Page
    {
        GalleryTopic gallery_topic_obj = new GalleryTopic();
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
            Gallery_Topic topic_obj = GalleryTopic.GetDetails(_idx);
            txtName.Text = topic_obj.Gallery_TopicName;
            txtDescription.Text = topic_obj.Description;
            string sortkey = topic_obj.SortKey.ToString();
            rdlStatus.SelectedValue = topic_obj.Status.ToString();
            ShowTreeNodes_Topics(topic_obj.ParentId.ToString());
        }

        private int? AddData()
        {
            Gallery_Topic topic_obj = new Gallery_Topic();
            topic_obj.ParentId = Convert.ToInt32(ddlTreeNode_Topics.SelectedValue);
            topic_obj.Gallery_TopicName = txtName.Text;
            topic_obj.Alias = ModuleClass.convertTitle2Link(txtName.Text);
            topic_obj.Description = txtDescription.Text;
            topic_obj.Status = Convert.ToChar(rdlStatus.SelectedValue);
            topic_obj.PostedDate = System.DateTime.Now;
            topic_obj.UserLog = System.Guid.Parse(Session["UserId"].ToString());
            int? i = GalleryTopic.InsertData(topic_obj);
            return i;
        }

        private int? UpdateData()
        {
            Gallery_Topic topic_obj = new Gallery_Topic();
            topic_obj.ParentId = Convert.ToInt32(ddlTreeNode_Topics.SelectedValue);
            topic_obj.Gallery_TopicName = txtName.Text;
            topic_obj.Gallery_TopicId = _idx;
            topic_obj.Alias = ModuleClass.convertTitle2Link(txtName.Text);
            topic_obj.Description = txtDescription.Text;
            topic_obj.Status = Convert.ToChar(rdlStatus.SelectedValue);
            topic_obj.PostedDate = System.DateTime.Now;
            topic_obj.UserLog = System.Guid.Parse(Session["UserId"].ToString());
            int? i = GalleryTopic.UpdateData(topic_obj);
            return i;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int? i = 0;
                if (_mode == UIMode.mode.add)
                {
                    i = AddData();
                    switch (i)
                    {
                        case -1:
                            lblErrorMsg.Text = "Thông tin không đầy đủ";
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 0;
                            break;
                        case -2:
                            lblErrorMsg.Text = "Tiến trình xử lý bị lỗi";
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                        case -3:
                            lblErrorMsg.Text = "Dữ liệu đã tồn tại";
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 1;
                            break;
                        case 1:
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
                        case -1:
                            lblErrorMsg.Text = "Thông tin không đầy đủ";
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 0;
                            break;
                        case -2:
                            lblErrorMsg.Text = "Tiến trình xử lý bị lỗi";
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                        case -3:
                            lblErrorMsg.Text = "Dữ liệu đã tồn tại";
                            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                            MultiView1.ActiveViewIndex = 2;
                            break;
                        case 1:
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