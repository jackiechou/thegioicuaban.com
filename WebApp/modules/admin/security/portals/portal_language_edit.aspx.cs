using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CommonLibrary.Modules;
using CommonLibrary.Common;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Modules.Dashboard.Components.Portals;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.security.portals
{
    public partial class portal_language_add : System.Web.UI.Page
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
                        string portalid = Request.QueryString["portalid"];
                        PopulateLanguage2DDL();
                        PopulatePortalList2DDL(portalid);
                        hdnWindowUIMODE.Value = _mode.ToString();
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
            //btnOkay.OnClientClick = String.Format("this.disabled=true; __doPostBack('{0}','');", btnOkay.UniqueID);
        }

        #region Portal Language ==============================================
        protected void PopulateLanguage2DDL()
        {
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlLanguageList.Items.Clear();
            ddlLanguageList.DataSource = dt;
            ddlLanguageList.DataTextField = "CultureName";
            ddlLanguageList.DataValueField = "CultureCode";
            ddlLanguageList.DataBind();
            ddlLanguageList.SelectedValue = "vi-VN";
        }
        protected void PopulateLanguage2DDL(string selected_value)
        {
            CultureClass culture_obj = new CultureClass();
            string Discontinued = "1";
            DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

            ddlLanguageList.Items.Clear();
            ddlLanguageList.DataSource = dt;
            ddlLanguageList.DataTextField = "CultureName";
            ddlLanguageList.DataValueField = "CultureCode";
            ddlLanguageList.DataBind();
            ddlLanguageList.SelectedValue = selected_value;
        }
        #endregion ============================================================

        #region Portal List ==================================================  
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
            ddlPortalList.Enabled = false;
        }
        #endregion ===============================================================

        private void LoadData()
        {
            PortalLanguages portal_language_obj = new PortalLanguages();
            DataTable dt = portal_language_obj.GetDetails(_idx);

            string strPortalId = dt.Rows[0]["PortalId"].ToString();
            string CultureCode = dt.Rows[0]["CultureCode"].ToString();
            PopulatePortalList2DDL(strPortalId);
            PopulateLanguage2DDL(CultureCode);
        }

        private int AddData()
        {                    
            string CultureCode = ddlLanguageList.SelectedValue;
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);

            PortalLanguages portal_language_obj = new PortalLanguages();
            int i = portal_language_obj.Insert(PortalId, CultureCode);
            return i;
        }

        private int UpdateData()
        {           
            string CultureCode = ddlLanguageList.SelectedValue;    

            PortalLanguages portal_language_obj = new PortalLanguages();
            int i = portal_language_obj.Update(_idx, CultureCode);
            return i;
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                string result = string.Empty;

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