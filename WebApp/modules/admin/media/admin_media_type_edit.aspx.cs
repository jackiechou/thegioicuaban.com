using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using MediaLibrary;

namespace WebApp.modules.admin.media
{
    public partial class admin_media_type_edit : System.Web.UI.Page
    {
        private Media_Types EditingMediaTypes
        {
            get
            {
                return Session["EditingMediaTypes"] as Media_Types;
            }
            set
            {
                Session["EditingMediaTypes"] = value;
            }
        }

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
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (!Page.IsPostBack)
            {
                string qsuimode = Request.QueryString["mode"];

                if (string.IsNullOrEmpty(qsuimode) == false)
                {
                    _mode = (UIMode.mode)Enum.Parse(typeof(UIMode.mode), qsuimode);
                    if (_mode == UIMode.mode.add)
                    {
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
            //PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            //btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            //btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
            //Page.ClientScript.RegisterOnSubmitStatement(GetType(), "ServerForm",
            //    "if (this.submitted) return false; this.submitted = true; return true;");
        }



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
            MediaTypes media_type_obj = new MediaTypes();
            Media_Types type_obj = media_type_obj.GetDetails(_idx);
            EditingMediaTypes = type_obj;

            txtTypeName.Text = type_obj.TypeName;
            txtTypeExt.Text = type_obj.TypeExt;
            txtTypePath.Text = type_obj.TypePath;
            txtDescription.Text = type_obj.Description;
            string Status = type_obj.Status;
            PopulateStatus2DDL(Status);
        }

        private int AddData()
        {
            string UserId = Session["UserId"].ToString();
            string TypeName = txtTypeName.Text;
            string TypeExt = txtTypeExt.Text;
            string TypePath = txtTypePath.Text;
            string Description = txtDescription.Text;
            string Status = rdlStatus.SelectedValue;
            
            MediaTypes media_type_obj = new MediaTypes();
            int i = media_type_obj.Insert(UserId, TypeName, TypeExt, TypePath, Description, Status);    
            return i;
        }

        private int UpdateData()
        {
            string UserId = Session["UserId"].ToString();
            string TypeName = txtTypeName.Text;
            string TypeExt = txtTypeExt.Text;
            string TypePath = txtTypePath.Text;
            string Description = txtDescription.Text;
            string Status = rdlStatus.SelectedValue;
            MediaTypes media_type_obj = new MediaTypes();
            int i = media_type_obj.Update(_idx, UserId, TypeName, TypeExt, TypePath, Description, Status);   
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
                        if (Page.IsPostBack)
                            ScriptManager.RegisterStartupScript(Page, typeof(Page), this.ClientID, "onSuccess();", true);
                        else
                            Page.ClientScript.RegisterStartupScript(this.GetType(), this.ClientID, "onSuccess();", true);
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