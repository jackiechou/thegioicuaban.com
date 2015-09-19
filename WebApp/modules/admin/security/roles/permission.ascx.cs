using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Security.Permissions;

namespace WebApp.modules.admin.security.roles
{
    public partial class permission : System.Web.UI.UserControl
    {
        PermissionController permission_obj = new PermissionController();
        DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {               
                FillDataInGrid();
            }
        }

        

        private void FillDataInGrid()
        {
            dt = permission_obj.GetAll();

            if (dt.Rows.Count > 0)
            {
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else
            {
                dt.Rows.Add(dt.NewRow());
                GridView1.DataSource = dt;
                GridView1.DataBind();

                int TotalColumns = GridView1.Rows[0].Cells.Count;
                GridView1.Rows[0].Cells.Clear();
                GridView1.Rows[0].Cells.Add(new TableCell());
                GridView1.Rows[0].Cells[0].ColumnSpan = TotalColumns;
                GridView1.Rows[0].Cells[0].Text = "No Record Found";
            }
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
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Cancel the edit
            GridView1.EditIndex = -1;
            GridView1.DataBind();
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

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                //string ApplicationId = ((DataRowView)e.Row.DataItem)["ApplicationId"].ToString();                
                e.Row.Attributes.Add("OnDblClick", "ShowEditModal(" + ID + ");");        

                //ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
                //if (btnDelete != null)
                //{
                //    btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record " + DataBinder.Eval(e.Row.DataItem, "RoleId") + "')");
                //}

                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //if (e.CommandName == "Delete")
            //{
            //    //int index = Convert.ToInt32(e.CommandArgument);                         
            //    //GridView customersGridView = (GridView)e.CommandSource;
            //    //GridViewRow row = customersGridView.Rows[index];
            //    //Label lblID = (Label)row.FindControl("lblID");              

            //    string RoleId = e.CommandArgument.ToString();
            //    string ApplicationId = Server.HtmlDecode(e.CommandArgument.Cells[1].Text);

            //    bool DeleteOnlyIfRoleIsEmpty = false;                                
            //    role_obj.Delete(ApplicationId, RoleId, DeleteOnlyIfRoleIsEmpty);

            //}           
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //GridViewRow row = (GridViewRow)GridView1.Rows[e.RowIndex];
            int ID =Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value.ToString());
            permission_obj.Delete(ID);
            FillDataInGrid();
        }   
    }
}