using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using CommonLibrary.Services.Banner;

namespace WebApp.modules.admin.banners
{
    public partial class admin_banner_position_edit : System.Web.UI.Page
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

        #region Status List ==============================================      
        public void LoadStatusList2DDL(string selected_value)
        {
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Active", "1"));
            lstColl.Add(new ListItem("InActive", "0"));

            rdlDiscontinued.DataSource = lstColl;
            rdlDiscontinued.DataTextField = "Text";
            rdlDiscontinued.DataValueField = "Value";
            rdlDiscontinued.DataBind();
            rdlDiscontinued.SelectedValue = selected_value;
         
        }
        #endregion =========================================================

        #region tree node list - category =======================================
        private void ShowTreeNodes(string ParentId)
        {
            lstTreeNode.Items.Clear(); //LISTBOX        

            BannerPosition objTree = new BannerPosition();
            DataTable dtNodes = objTree.GetAll(); //select all the nodes from DB
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
                    lstTreeNode.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["BannerPosition"].ToString()), dv[i]["Id"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["Id"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }
        #endregion ==============================================================

        private void LoadData()
        {
            BannerPosition banner_position_obj = new BannerPosition();
            DataTable dt = banner_position_obj.GetDetailById(_idx);

            txtBannerPosition.Text = dt.Rows[0]["BannerPosition"].ToString();
            txtDescription.Text = dt.Rows[0]["Description"].ToString();
            string sortkey = dt.Rows[0]["SortKey"].ToString();
            string status = dt.Rows[0]["Status"].ToString();
            LoadStatusList2DDL(status);

            /***** PARENT ID ***********************************************/
            ViewState["parentid"] = dt.Rows[0]["ParentId"].ToString();
            ShowTreeNodes(dt.Rows[0]["ParentId"].ToString());
            /******************************************************************/
            
        }

        private int UpdateData()
        {            
            string UserId = Session["UserId"].ToString();
            string strParentID = null;
            if (lstTreeNode.SelectedValue == _idx.ToString())
            {
                strParentID = ViewState["parentid"].ToString();
            }
            else
            {
                strParentID = lstTreeNode.SelectedValue;
            }
            int parent_id = Convert.ToInt32(strParentID);

            string banner_position = txtBannerPosition.Text;
            string description = txtDescription.Text;
            string status = rdlDiscontinued.SelectedValue;
            string Discontinued = rdlDiscontinued.SelectedValue;

            BannerPosition banner_position_obj = new BannerPosition();
            int i = banner_position_obj.Update(_idx, parent_id, banner_position, description, status);            
            return i;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int i = 0;
                if (_mode == UIMode.mode.edit)
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