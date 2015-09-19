using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Content;

namespace WebApp.modules.admin.contentlist
{
    public partial class content_items_edit : System.Web.UI.Page
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

        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInit(e);

        //    string qsuimode = Request.QueryString["mode"];
        //    if (string.IsNullOrEmpty(qsuimode) == false)
        //    {
        //        _mode = (UIMode.mode)Enum.Parse(typeof(UIMode.mode), qsuimode);
        //        if (_mode == UIMode.mode.add)
        //        {
        //            LoadContentTypeList2DDL();
        //            rdlIndexed.SelectedValue = "1";
        //        }
        //    }
        //}

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
                        LoadContentTypeList2DDL();                      
                        rdlIndexed.SelectedValue = "1";                        
                    }
                    if (_mode == UIMode.mode.edit)
                    {
                        _idx = Convert.ToInt32(Request.QueryString["idx"]);
                        LoadData();
                    }
                    hdnWindowUIMODE.Value = _mode.ToString();
                    btnOkay.OnClientClick = String.Format("this.disabled=true; __doPostBack('{0}','');", btnOkay.UniqueID);                    
                }
                MultiView1.ActiveViewIndex = 0;
            }                                       
        }

        private void LoadData()
        {
            ContentController content_controller_obj = new ContentController();
            DataTable dt = content_controller_obj.GetDetails(_idx);           
            LoadContentTypeList2DDL(dt.Rows[0]["ContentTypeID"].ToString());        

            txtContents.Text = dt.Rows[0]["Content"].ToString();
            txtContentKey.Text = dt.Rows[0]["ContentKey"].ToString();
            bool indexed =Convert.ToBoolean(dt.Rows[0]["Indexed"].ToString());
            if (indexed == true){
                rdlIndexed.SelectedValue = "1";
            }
            else
            {
                rdlIndexed.SelectedValue = "2";
            }
            
        }

        #region Content Type List =======================================
        private void LoadContentTypeList2DDL()
        {
            ContentType content_types_obj = new ContentType();
            DataTable dt_content_types = content_types_obj.GetAll(); //select all the nodes from DB

            ddlContentTypes.Items.Clear();              
            ddlContentTypes.DataSource = dt_content_types;
            ddlContentTypes.DataTextField = "ContentType";
            ddlContentTypes.DataValueField = "ContentTypeID";
            ddlContentTypes.DataBind();
            ddlContentTypes.Items.Insert(0, new ListItem("- Chọn -", ""));
            ddlContentTypes.SelectedIndex = 0;
           
        }

        private void LoadContentTypeList2DDL(string selected_value)
        {
            ContentType content_types_obj = new ContentType();
            DataTable dt_content_types = content_types_obj.GetAll(); //select all the nodes from DB

            ddlContentTypes.Items.Clear(); //DROPDOWNLIST    
            ddlContentTypes.AutoPostBack = true;
            ddlContentTypes.DataSource = dt_content_types;
            ddlContentTypes.DataTextField = "ContentType";
            ddlContentTypes.DataValueField = "ContentTypeID";
            ddlContentTypes.DataBind();
            ddlContentTypes.Items.Insert(0, new ListItem("- Chọn -", ""));
            ddlContentTypes.SelectedValue = selected_value;            
        }
        #endregion
        
        protected void btnOkay_Click(object sender, EventArgs e)
        {            
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);

                if (_mode == UIMode.mode.add)
                {
                    int i = AddData();
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
                    int i = UpdateData();
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
            int ContentTypeID =Convert.ToInt32(ddlContentTypes.SelectedValue);            
            string Content = txtContents.Text;
            string ContentKey = txtContentKey.Text;
            string strIndexed = rdlIndexed.SelectedValue;
            bool Indexed = false;
            if (strIndexed == "1")
            {
                Indexed=true;
            }
            else
            {
                Indexed=false;
            }
            
            ContentController content_controller_obj = new ContentController();
            int i = content_controller_obj.Add(ContentTypeID, Content,ContentKey, Indexed);
            return i;
        }

        private int UpdateData()
        {
            int ContentTypeID = Convert.ToInt32(ddlContentTypes.SelectedValue);           
            string Content = txtContents.Text;
            string ContentKey = txtContentKey.Text;
            string strIndexed = rdlIndexed.SelectedValue;
            bool Indexed = false;
            if (strIndexed == "1")
            {
                Indexed = true;
            }
            else
            {
                Indexed = false;
            }
            
            ContentController content_controller_obj = new ContentController();
            int i = content_controller_obj.Edit(_idx, ContentTypeID, Content, ContentKey, Indexed);
            return i;
        }
    }
}