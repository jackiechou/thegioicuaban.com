using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using CommonLibrary.Modules;
using MediaLibrary;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.media
{
    public partial class admin_background_audio_edit : System.Web.UI.Page
    {
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
                        PopulatePortalList2DDL();
                        PopulateMediaTypeList2DDL();
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

        #region Media Types ======================================================
        protected void PopulateMediaTypeList2DDL()
        {
            //MediaTypes media_type_obj = new MediaTypes();
            //DataTable dt = media_type_obj.GetList();

            //ddlMediaTypeList.Items.Clear();
            //ddlMediaTypeList.DataSource = dt;
            //ddlMediaTypeList.DataTextField = "TypeName";
            //ddlMediaTypeList.DataValueField = "TypeId";
            //ddlMediaTypeList.DataBind();
            //ddlMediaTypeList.SelectedIndex = 0;
        }       
        protected void PopulateMediaTypeList2DDL(string selected_value)
        {
            //MediaTypes media_type_obj = new MediaTypes();
            //DataTable dt = media_type_obj.GetList();

            //ddlMediaTypeList.Items.Clear();
            //ddlMediaTypeList.DataSource = dt;
            //ddlMediaTypeList.DataTextField = "TypeName";
            //ddlMediaTypeList.DataValueField = "TypeId";
            //ddlMediaTypeList.DataBind();
            //ddlMediaTypeList.SelectedValue = selected_value;
        }
        #endregion ============================================================  

        private void LoadData()
        {
            MediaFiles media_obj = new MediaFiles();
            DataTable dt = media_obj.GetDetailById(_idx);

            string PortalId = dt.Rows[0]["PortalId"].ToString();
            string TypeId = dt.Rows[0]["TypeId"].ToString();
            string Title  = dt.Rows[0]["Title"].ToString();
            string Description = dt.Rows[0]["Description"].ToString();
            string FileName = dt.Rows[0]["FileName"].ToString();

            PopulatePortalList2DDL(PortalId);
            PopulateMediaTypeList2DDL(TypeId);

            txtTitle.Text = Title;
            txtDescription.Text = Description;
            Literal_FileName.Text = FileName;
            string dir_path = System.Configuration.ConfigurationManager.AppSettings["upload_background_audio_dir"];
            string file_path = "../../../" + dir_path + "/" + FileName;
            ViewState["filename"] = FileName;


            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<object type=\"application/x-shockwave-flash\" data=\"../../../scripts/plugins/dewplayer/dewplayer-vol.swf?mp3=" + file_path + "\" width=\"240\" height=\"20\" id=\"dewplayer-vol\"><param name=\"wmode\" value=\"transparent\" /><param name=\"movie\" value=\"../../../scripts/plugins/dewplayer/dewplayer-vol.swf?mp3=" + file_path + "/></object>");
            Literal_Preview.Text = sb.ToString();           
        }

        private int AddData()
        {
            string userid = Session["UserId"].ToString();
            int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
            int typeid = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            string title = txtTitle.Text;
            string description = txtDescription.Text;

            /*** UPLOAD *************************************************************************************************************/
            string dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_background_audio_dir"];
            HttpPostedFile posted_file = FileInput.PostedFile;
            string filename = "";
            if (posted_file.ContentLength > 0)
            {
                ModuleClass module_obj = new ModuleClass();
                filename = module_obj.GetEncodeString(System.IO.Path.GetFileName(posted_file.FileName));
                string savePath = Server.MapPath(dir_path +"/" + filename);
                posted_file.SaveAs(savePath);
            }
            /************************************************************************************************************************/

            MediaFiles media_obj = new MediaFiles();
            //int i = media_obj.Insert(userid,portalid,typeid,filename, title, description);
            int i=0;
            return i;
        }

        private int UpdateData()
        {
            string userid = Session["UserId"].ToString();
            int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
            int typeid = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            string title = txtTitle.Text;
            string description = txtDescription.Text;


            /*** UPLOAD *************************************************************************************************************/
            string dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_background_audio_dir"];
            HttpPostedFile icon_file = FileInput.PostedFile;
            string filename = "";
            if (icon_file != null)
            {
                if (icon_file.ContentLength > 0)
                {
                    ModuleClass module_obj = new ModuleClass();
                    module_obj.deleteFile(ViewState["filename"].ToString(), dir_path);

                    filename = module_obj.GetEncodeString(System.IO.Path.GetFileName(icon_file.FileName));
                    string savePath = Server.MapPath(dir_path + "/" + filename);
                    icon_file.SaveAs(savePath);
                }
                else
                {
                    filename = ViewState["filename"].ToString();
                }
            }            
            /************************************************************************************************************************/

            //MediaClass media_obj = new MediaClass();
            //int i = media_obj.Update(userid, _idx,portalid,typeid, filename, title, description);
            int i=0;
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