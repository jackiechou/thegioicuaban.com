using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Entities.Users;
using CommonLibrary.Application;

namespace WebApp.modules.admin.security.users
{
    public partial class aspnet_users : System.Web.UI.UserControl
    {
        UserController user_obj = new UserController();
        DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadApplicationList2DDL();
                //LoadBranchList2DDL();
                //LoadDepartmentList2DDL();
                FillDataInGrid();
            }
            this.btnLock.Click += new EventHandler(this.btnLock_Click);

        }


        #region Application List =======================================
        private void LoadApplicationList2DDL()
        {
            ddlApplicationList.Items.Clear();

            ApplicationController application_obj = new ApplicationController();
            DataTable dtNodes = application_obj.GetApps();
            ddlApplicationList.DataSource = dtNodes;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
            ddlApplicationList.Items.Insert(0, new ListItem("- Chọn -", "")); //DROPDOWNLIST
            ddlApplicationList.SelectedIndex = 1;
            ddlApplicationList.AutoPostBack = true;
        }

        protected void ddlApplicationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ===================================================================

        #region BRANCH LIST ==========================================
        //protected void LoadBranchList2DDL()
        //{
        //    BranchClass branch_obj = new BranchClass();
        //    DataTable dt = branch_obj.GetActiveList();

        //    ddlBranchList.DataSource = dt;
        //    ddlBranchList.DataTextField = "Branch_Name";
        //    ddlBranchList.DataValueField = "Branch_ID";
        //    ddlBranchList.DataBind();
        //    ddlBranchList.Items.Insert(0, new ListItem("- Chọn -", "")); // add the new item at the top of the list
        //    ddlBranchList.SelectedIndex = 0; // Select the first item  
        //    ddlBranchList.AutoPostBack = true;
        //}
        //protected void ddlBranchList_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    LoadDepartmentList2DDL();
        //}
        #endregion ==================================================

        #region Department LIST ==========================================
        //protected void LoadDepartmentList2DDL()
        //{
        //    if (ddlBranchList.SelectedValue != string.Empty)
        //    {
        //        int Branch_ID = Convert.ToInt32(ddlBranchList.SelectedValue);
        //        DepartmentClass department_obj = new DepartmentClass();
        //        DataTable dt = department_obj.GetDepartmentListByBranchID(Branch_ID);

        //        ddlDepartmentList.DataSource = dt;
        //        ddlDepartmentList.DataTextField = "Department_Name";
        //        ddlDepartmentList.DataValueField = "Department_ID";
        //        ddlDepartmentList.DataBind();                
        //    }
        //    ddlDepartmentList.Items.Insert(0, new ListItem("- Chọn -", "")); // add the new item at the top of the list
        //    ddlDepartmentList.SelectedIndex = 0; // Select the first item  
        //    ddlDepartmentList.AutoPostBack = true;
        //}
        //protected void ddlDepartmentList_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    FillDataInGrid();
        //}
        #endregion ==================================================


        private void FillDataInGrid()
        {
            //try
            //{
            string app_id = ddlApplicationList.SelectedValue;
            dt = user_obj.GetUsers(app_id);

            if (dt.Rows.Count > 0)
            {
                //string Branch_ID = ddlBranchList.SelectedValue;
                //string Department_ID = ddlDepartmentList.SelectedValue;
                //string query = string.Empty;
                //if (Branch_ID != string.Empty)
                //{
                //    if (Department_ID != string.Empty)
                //        query = "Branch_ID ='" + Branch_ID + "' AND Department_ID ='" + Department_ID + "'";
                //    else
                //        query = "Branch_ID ='" + Branch_ID + "'";
                //}
                //else
                //{
                //    if (Department_ID != string.Empty)
                //        query = "Department_ID ='" + Department_ID + "'";                   
                //}
                //dt.DefaultView.RowFilter = query;
                GridView1.DataSource = dt.DefaultView;
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

            //}catch (Exception ex) { ex.ToString(); }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {                
                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + ID + "');";
                //e.Row.Attributes.Add("OnDblClick", "ShowEditModal(" + ID + ");"); 
                e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';this.style.color='#9C1E00'";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#454545';";
                e.Row.Attributes["onmousedown"] = "this.style.background='#FACC2E';";
                //e.Row.Attributes["onmouseclick"] = "this.style.background='##228b22';this.style.color='#FFFFFF';";
                
                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridView1.PageIndex = e.NewPageIndex;
                FillDataInGrid();
            }
            catch (ArgumentOutOfRangeException ex) { ex.ToString(); }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the currently selected row using the SelectedRow
            GridViewRow row = GridView1.SelectedRow;
            GridView1.SelectedIndex = -1;
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GridView1.PageIndex = e.NewSelectedIndex;
            GridView1.DataBind();
        }
        protected void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gvrPager = GridView1.BottomPagerRow;
            DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlPages");
            GridView1.PageIndex = ddlPages.SelectedIndex;
            FillDataInGrid();
        }
        
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            //string app_id = ddlApplicationList.SelectedValue;
            //DataTable dt = user_obj.GetUsers(app_id);           
            //DataView dv = new DataView();
            //dv = dt.DefaultView;

            //if (ViewState["sortOrder"] == null)
            //{
            //    ViewState["sortOrder"] = "DESC";
            //}
            //else
            //{
            //    if (ViewState["sortOrder"].ToString() == "ASC")
            //    {
            //        ViewState["sortOrder"] = "DESC";
            //    }
            //    else
            //    {
            //        ViewState["sortOrder"] = "ASC";
            //    }
            //}     
            //dv.Sort = e.SortExpression + " " + ViewState["sortOrder"].ToString();
            ////dv.Sort = e.SortExpression + " " + e.SortDirection;
            //GridView1.DataSource = dv;
            //GridView1.DataBind();
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
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

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Cancel the edit
            GridView1.EditIndex = -1;
            GridView1.DataBind();
        }
       

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GridView1.DataBind();
        }

        protected void btnLock_Click(object sender, EventArgs e)
        {
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            foreach (string Item in IDs)
            {              
                string UserID = Item.ToString();               
                int i = user_obj.LockUser(UserID);
            }
            FillDataInGrid();
        }

        protected void btnUnLock_Click(object sender, EventArgs e)
        {
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            foreach (string Item in IDs)
            {
                string UserID = Item.ToString();
                int i = user_obj.UnlockUser(UserID);
            }
            FillDataInGrid();
        }

        protected void btnMultipleDelete_Click(object sender, EventArgs e)
        {
            // Delete from Membership table if (@TablesToDeleteFrom & 1) is set
            // Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
            //-- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
            //    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
            //    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set


            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            //Code for deleting items
            foreach (string Item in IDs)
            {
                string UserId = Item.ToString();
                user_obj.Delete(UserId, 15);   
            }
            hdnFldSelectedValues.Value = string.Empty;
            FillDataInGrid();
        }         
      
    }
}