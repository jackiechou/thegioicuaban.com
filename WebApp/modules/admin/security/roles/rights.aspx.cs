using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Application;
using CommonLibrary.Security.Roles;

namespace WebApp.modules.admin.security.roles
{
    public partial class rights : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadApplicationList2DDL();
                LoadRoleList2DDL();
                CreateDataSource();
            }
        }
        #region Application =========================================================
        private void LoadApplicationList2DDL()
        {
            ddlApplicationList.Items.Clear();

            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_app = app_obj.GetApps();
            ddlApplicationList.AutoPostBack = true;
            ddlApplicationList.DataSource = dt_app;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
            //ddlApplicationList.Items.Insert(0, new ListItem("-Chọn ứng dụng-", ""));
            ddlApplicationList.SelectedIndex = 0;
        }
        protected void ddlApplicationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRoleList2DDL();            
        }
        #endregion ==================================================================

        #region Role =========================================================
        private void LoadRoleList2DDL()
        {
            ddlRoleList.Items.Clear();
            string ApplicationId = ddlApplicationList.SelectedValue;
            RoleController app_obj = new RoleController();
            DataTable dt_app = app_obj.GetRoleListByApplicationId(ApplicationId);
            ddlRoleList.AutoPostBack = true;
            ddlRoleList.DataSource = dt_app;
            ddlRoleList.DataTextField = "RoleName";
            ddlRoleList.DataValueField = "RoleId";
            ddlRoleList.DataBind();
            //ddlRoleList.Items.Insert(0, new ListItem("- Chọn -", ""));
            ddlRoleList.SelectedIndex = 0;
        }
        #endregion ==================================================================

        private void CreateDataSource()
        {
            //Create a data table users
            //DataTable dataTable = new DataTable();
            string RoleId = ddlRoleList.SelectedValue;
            TabPermissionController tab_permission_obj=new TabPermissionController();
            DataTable dataTable = tab_permission_obj.GetListByRoleId(RoleId);


            //Create a column TabId to store the tabId & View - Add - Edit - Delete - All to store the add permission access
            //DataColumn tabId = new DataColumn("TabId", Type.GetType("System.String"));
            //DataColumn tabName = new DataColumn("TabName", Type.GetType("System.String"));

            DataColumn view = new DataColumn("View", Type.GetType("System.Boolean"));
            DataColumn add = new DataColumn("Add", Type.GetType("System.Boolean"));
            DataColumn edit = new DataColumn("Edit", Type.GetType("System.Boolean"));
            DataColumn delete = new DataColumn("Delete", Type.GetType("System.Boolean"));
            DataColumn all = new DataColumn("All", Type.GetType("System.Boolean"));

            //Add the columns to the table
            //dataTable.Columns.Add(tabId);
            //dataTable.Columns.Add(tabName);

            dataTable.Columns.Add(view);
            dataTable.Columns.Add(add);
            dataTable.Columns.Add(edit);
            dataTable.Columns.Add(all);
            
            //Bind the grid view with the data source
            GridView1.DataSource = dataTable;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Check for the row type, which should be data row
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Find the check boxes and assign the values from the data source
                ((CheckBox)e.Row.FindControl("chkView")).Checked = Convert.ToBoolean(((DataRowView)e.Row.DataItem)[1]);
                ((CheckBox)e.Row.FindControl("chkAdd")).Checked = Convert.ToBoolean(((DataRowView)e.Row.DataItem)[2]);
                ((CheckBox)e.Row.FindControl("chkEdit")).Checked = Convert.ToBoolean(((DataRowView)e.Row.DataItem)[3]);
                ((CheckBox)e.Row.FindControl("chkDelete")).Checked = Convert.ToBoolean(((DataRowView)e.Row.DataItem)[4]);
                ((CheckBox)e.Row.FindControl("chkAll")).Checked = Convert.ToBoolean(((DataRowView)e.Row.DataItem)[5]);


                //Find the checkboxes and assign the javascript function which should be called when the user clicks the checkboxes.
                ((CheckBox)e.Row.FindControl("chkView")).Attributes.Add("onclick", "checkBoxClicked('" +
                    ((CheckBox)e.Row.FindControl("chkView")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAdd")).ClientID
                    + "','" + ((CheckBox)e.Row.FindControl("chkEdit")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAll")).ClientID + "'," + "'SELECT')");

                ((CheckBox)e.Row.FindControl("chkAdd")).Attributes.Add("onclick", "checkBoxClicked('" +
                    ((CheckBox)e.Row.FindControl("chkView")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAdd")).ClientID
                    + "','" + ((CheckBox)e.Row.FindControl("chkEdit")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAll")).ClientID + "'," + "'ADD')");


                ((CheckBox)e.Row.FindControl("chkEdit")).Attributes.Add("onclick", "checkBoxClicked('" +
                    ((CheckBox)e.Row.FindControl("chkView")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAdd")).ClientID
                    + "','" + ((CheckBox)e.Row.FindControl("chkEdit")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAll")).ClientID + "'," + "'EDIT')");


                ((CheckBox)e.Row.FindControl("chkDelete")).Attributes.Add("onclick", "checkBoxClicked('" +
                    ((CheckBox)e.Row.FindControl("chkView")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAdd")).ClientID
                    + "','" + ((CheckBox)e.Row.FindControl("chkDelete")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAll")).ClientID + "'," + "'DELETE')");


                ((CheckBox)e.Row.FindControl("chkAll")).Attributes.Add("onclick", "checkBoxClicked('" +
                    ((CheckBox)e.Row.FindControl("chkView")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAdd")).ClientID
                    + "','" + ((CheckBox)e.Row.FindControl("chkEdit")).ClientID + "','" + ((CheckBox)e.Row.FindControl("chkAll")).ClientID + "'," + "'ALL')");
            }
        }
    }
}