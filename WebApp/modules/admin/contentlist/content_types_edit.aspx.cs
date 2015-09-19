using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Entities.Content;

namespace WebApp.modules.admin.contentlist
{
    public enum mode
    {
        add,
        edit,
        view
    }

    public partial class content_types_edit : System.Web.UI.Page
    {
        public mode _mode
        {
            get
            {
                if (ViewState["mode"] == null)
                    ViewState["mode"] = new mode();
                return (mode)ViewState["mode"];
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
            if (!IsPostBack)
            {
                string qsuimode = Request.QueryString["mode"];
                if (string.IsNullOrEmpty(qsuimode) == false)
                {
                    _mode = (mode)Enum.Parse(typeof(mode), qsuimode);
                    if (_mode == mode.edit)
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
            ContentType content_types_obj = new ContentType();
            DataTable dt = content_types_obj.GetDetails(_idx);
            txtContentType.Text = dt.Rows[0]["ContentType"].ToString();         
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            try
            {
                if (_mode == mode.add)
                {
                    AddData();
                }
                else if (_mode == mode.edit)
                {
                    UpdateData();
                }
                MultiView1.ActiveViewIndex = 1;
                ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
            }
            catch
            {
                ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                MultiView1.ActiveViewIndex = 1;
            }
        }

        private void AddData()
        {           
            string ContentType = txtContentType.Text;
            ContentType content_types_obj = new ContentType();
            int i = content_types_obj.Insert(ContentType);
        }

        private void UpdateData()
        {
            string ContentType = txtContentType.Text;
            ContentType content_types_obj = new ContentType();
            int i = content_types_obj.Update(_idx, ContentType);
        }       
    }
}