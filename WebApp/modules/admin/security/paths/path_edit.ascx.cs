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
    public partial class path_edit : System.Web.UI.UserControl
    {
        private static string view_ctrl = "admin_paths.aspx";
        private static string edit_ctrl = "admin_paths_edit.aspx";
        private static string root_path = "~/pages/administrators/";
        private static string edit_path = root_path + "application/path/" + edit_ctrl;
        private static string login_path = root_path + "login.aspx";

        PathClass path_obj = new PathClass();
        DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadDataFromTable();
            }
        }


        protected void LoadDataFromTable()
        {
            if (Request.QueryString["id"] != null && Request.QueryString["id"] != string.Empty)
            {

                string id = Request.QueryString["id"];
                dt = path_obj.GetDetails(id);

                //Load data to controls               
                string app_id = dt.Rows[0]["ApplicationId"].ToString();
                ShowApp(app_id);
                txtPath.Text = dt.Rows[0]["Path"].ToString();
            }
        }

        #region Application =======================================
        private void ShowApp(string selected_value)
        {
            ddlApp.Items.Clear(); //DROPDOWNLIST        
            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_app = app_obj.GetApps(); //select all the nodes from DB
            ddlApp.AutoPostBack = true;
            ddlApp.DataSource = dt_app;
            ddlApp.DataTextField = "ApplicationName";
            ddlApp.DataValueField = "ApplicationId";
            ddlApp.SelectedValue = selected_value;
            ddlApp.Items.Insert(0, new ListItem("-Chọn ứng dụng-", " "));
            ddlApp.DataBind();
        }
        #endregion

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            //try
            //{
            string app_id = ddlApp.SelectedValue;
            string path_id = Request.QueryString["id"];
            string path = txtPath.Text;

            int i = path_obj.UpdatePath(app_id, path_id, path);

            switch (i)
            {
                case -3:
                    Response.Write("<script>alert('Không tồn tại ID này.');window.location.href='" + view_ctrl + "';</script>");
                    Response.End();
                    break;
                case -2:
                    Response.Write("<script>alert('Update không thành công.');window.location.href='" + edit_ctrl + "';</script>");
                    Response.End();
                    break;
                case -1:
                    Response.Write("<script>alert('Thông tin không đầy đủ');window.location.href='" + edit_ctrl + "';</script>");
                    Response.End();
                    break;
                case 1:
                    Response.Write("<script>alert('Update thành công');window.location.href='" + view_ctrl + "';</script>");
                    Response.End();
                    break;
                default:
                    Response.Write("<script>alert('Lỗi hệ thống');window.location.href='" + view_ctrl + "';</script>");
                    Response.End();
                    break;
            }

            //}
            //catch (Exception ex) { ex.ToString(); }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.location.href='" + view_ctrl + "';</script>");
            Response.End();
            //Server.Transfer(edit_path, false);
        }      
    }
}