using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Application;

namespace WebApp.modules.admin.security
{
    public partial class path_add : System.Web.UI.UserControl
    {
        private static string view_ctrl = "admin_paths.aspx";
        private static string add_ctrl = "admin_paths_add.aspx";
        private static string root_path = "~/pages/administrators/";
        private static string login_path = root_path + "login.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ShowApp();
            }
        }

        #region Application =======================================
        private void ShowApp()
        {
            ddlApp.Items.Clear(); //DROPDOWNLIST        

            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_app = app_obj.GetApps(); //select all the nodes from DB
            ddlApp.AutoPostBack = true;
            ddlApp.DataSource = dt_app;
            ddlApp.DataTextField = "ApplicationName";
            ddlApp.DataValueField = "ApplicationId";
            ddlApp.DataBind();
            ddlApp.Items.Insert(0, new ListItem("-Chọn ứng dụng-", " ")); //DROPDOWNLIST
            ddlApp.SelectedIndex = 0;
        }
        #endregion

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            //try
            //{
            string app_id = ddlApp.SelectedValue;
            string path = txtPath.Text;

            PathClass path_obj = new PathClass();
            string result = path_obj.CreatePath(app_id, path);

            if (result != "")
            {
                Response.Write("<script>alert('Insert thành công.');window.location.href='" + view_ctrl + "';</script>");
                Response.End();
            }
            else
            {
                Response.Write("<script>alert('Insert không thành công.');window.location.href='" + add_ctrl + "';</script>");
                Response.End();
            }

            //}
            //catch (Exception ex) { ex.ToString(); }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Server.Transfer(view_ctrl, false);
        }    
    }
}