using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Application;
using CommonLibrary.Security.Roles;

namespace WebApp.modules.admin.dashboard.modules
{
    public partial class admin_module_permission_edit : System.Web.UI.Page
    {
        ModulePermissions module_permission_obj = new ModulePermissions();
        DataTable dt = new DataTable();

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
                LoadApplicationList2DDL();
                FillDataInGrid();
            }
        }

        #region Application =======================================
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
            ddlApplicationList.Items.Insert(0, new ListItem("-Chọn ứng dụng-", ""));
            ddlApplicationList.SelectedIndex = 0;
            ddlApplicationList.Enabled = false;
        }      
        #endregion ====================================================================

        private void FillDataInGrid()
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            RoleController role_obj = new RoleController();
            DataTable dt = role_obj.GetRoleListByApplicationId(ApplicationId);

            if (dt.Rows.Count > 0)
            {
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            //else
            //{
            //    dt.Rows.Add(dt.NewRow());
            //    GridView1.DataSource = dt;
            //    GridView1.DataBind();

            //    int TotalColumns = GridView1.Rows[0].Cells.Count;
            //    GridView1.Rows[0].Cells.Clear();
            //    GridView1.Rows[0].Cells.Add(new TableCell());
            //    GridView1.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            //    GridView1.Rows[0].Cells[0].Text = "No Record Found";
            //}
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            FillDataInGrid();
        }
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the currently selected row using the SelectedRow
            GridViewRow row = GridView1.SelectedRow;
            GridView1.SelectedIndex = -1;
        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;

            // Display the primary key value of the selected row.
            //string id = GridView1.DataKeys[e.NewEditIndex].Value.ToString();
            //HyperLink HyperLink_Edit = (HyperLink)GridView1.Rows[e.NewEditIndex].FindControl("HyperLink_Edit");
            //string url = "pages/administrators/index.aspx?type=1&page=articles/admin_article_categories_edit&id=" + id;
            //HyperLink_Edit.Attributes.Add("onclick", "location.href='" + url + "';");
        }
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Cancel the edit
            GridView1.EditIndex = -1;
            GridView1.DataBind();
        }
        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
                //{
                //    // Add javascript to highlight row
                //    e.Row.Attributes.Add("onclick", "ChangeRowColor(this);");           
                //}
            }
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            dt.DefaultView.Sort = e.SortExpression;
            GridView1.DataSource = dt.DefaultView.ToTable();
            GridView1.DataBind();
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            //Paging =========================================================
            GridViewRow gvrPager = GridView1.BottomPagerRow;

            if (gvrPager == null) return;
            gvrPager.Visible = true;

            // get your controls from the gridview
            DropDownList ddlPages =
                (DropDownList)gvrPager.Cells[0].FindControl("ddlPages");
            Label lblPageCount =
                (Label)gvrPager.Cells[0].FindControl("lblPageCount");

            if (ddlPages != null)
            {
                // populate pager
                for (int i = 0; i < GridView1.PageCount; i++)
                {
                    int intPageNumber = i + 1;
                    ListItem lstItem =
                        new ListItem(intPageNumber.ToString());

                    if (i == GridView1.PageIndex)
                        lstItem.Selected = true;

                    ddlPages.Items.Add(lstItem);
                }
            }

            // populate page count
            if (lblPageCount != null)
                lblPageCount.Text = GridView1.PageCount.ToString();
        }

        protected void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gvrPager = GridView1.BottomPagerRow;
            DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlPages");

            GridView1.PageIndex = ddlPages.SelectedIndex;
            // a method to populate your grid GridView1.DataBind(); 
            FillDataInGrid();
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GridView1.PageIndex = e.NewSelectedIndex;
            GridView1.DataBind();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Get the index of the row being changed
            int row_index = int.Parse(e.CommandArgument.ToString());
            int id = (int)GridView1.DataKeys[row_index].Value;
            string _commandName = e.CommandName;
            if (_commandName == "Delete")
            {
                int i = module_permission_obj.DeleteModulePermission(id);
                FillDataInGrid();
            }
            //switch (_commandName)
            //{
            //    case "Delete":
            //        {
            //            int i = module_controller_obj.Delete(id);
            //            FillDataInGrid();
            //            break;
            //        }
            //    case "Edit":
            //        {
            //            Response.Redirect("~/pages/administrators/index.aspx?type=1&page=banner_positions/admin_banner_position_edit&id=" + id);
            //            break;
            //        }
            //    case ("SingleClick"):
            //        {
            //            GridView1.SelectedIndex = selectedIndex;
            //            object send = new object();
            //            GridViewEditEventArgs ex = new GridViewEditEventArgs(selectedIndex);
            //            GridView1_RowEditing(send, ex);
            //            break;
            //        }
            //    default:
            //        break;             
            //}
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //getting the data using the Colum Name, and storing in a variable
                //string RoleId = ((DataRowView)e.Row.DataItem)["RoleId"].ToString(); ;
                //ImageButton btnAdd = (ImageButton)e.Row.FindControl("btnAdd");
                //if (btnAdd != null)
                //{
                //    btnAdd.Attributes.Add("onclick", "ShowAddModal(" + ModuleID + ")");
                //}

                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                e.Row.Attributes.Add("OnDblClick", "ShowEditModal(" + ID + ");");

                //ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
                //if (btnDelete != null)
                //{
                //    //btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record " + DataBinder.Eval(e.Row.DataItem, "CountryID") + "')");
                //    btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record?')");
                //}

            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            System.Collections.ArrayList array_list = new System.Collections.ArrayList();
            foreach (GridViewRow gvr in this.GridView1.Rows)
            {
                if (((CheckBox)gvr.FindControl("chkView")).Checked == true)
                {
                    array_list.Add(gvr.Cells[2].Text);
                }
            }
            string chkView_items=string.Empty;
            foreach(object item in chkView_items){
                chkView_items += " " + item.ToString();
            }
            Response.Write(chkView_items);

        }
    }
}