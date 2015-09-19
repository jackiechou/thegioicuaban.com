using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Application;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.tabs
{
    public partial class admin_tabs : System.Web.UI.UserControl
    {
       DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {                
                PopulateApplicationList2DDL();
                PopulatePortal2DDL();
                PopulateIsSecureList2DDL();
                FillDataInGrid();
            }
        }

        #region Application =========================================================
        private void PopulateApplicationList2DDL()
        {
            ddlApplicationList.Items.Clear();

            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_app = app_obj.GetApps();
            ddlApplicationList.AutoPostBack = true;
            ddlApplicationList.DataSource = dt_app;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();            
            if(Session["ApplicationId"] !=null && Session["ApplicationId"].ToString() !=string.Empty)
                ddlApplicationList.SelectedValue = Session["ApplicationId"].ToString();
            else
                ddlApplicationList.SelectedIndex = 0;
            ddlApplicationList.AutoPostBack = true;
        }

        protected void ddlApplicationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulatePortal2DDL();
        }      
        #endregion ====================================================================
        
        #region Portal List ===========================================================
        protected void PopulatePortal2DDL()
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            if (!string.IsNullOrEmpty(ApplicationId))
            {
                PortalController portal_obj = new PortalController();
                DataTable dt = portal_obj.GetListByApplicationId(ApplicationId);

                ddlPortalList.Items.Clear();
                ddlPortalList.DataSource = dt;
                ddlPortalList.DataTextField = "PortalName";
                ddlPortalList.DataValueField = "PortalId";
                ddlPortalList.DataBind();                
                ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", ""));
                if (Session["PortalId"] != null && Session["PortalId"].ToString() != string.Empty)
                    ddlPortalList.SelectedValue = Session["PortalId"].ToString();
                else
                    ddlPortalList.SelectedIndex = 0;
            }
            ddlPortalList.AutoPostBack = true;
        }
        protected void ddlPortalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ============================================================

        #region IsSecure List ==============================================
        public void PopulateIsSecureList2DDL()
        {
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Admin Menu", "True"));
            lstColl.Add(new ListItem("Front Menu", "False"));

            ddlIsSecure.DataSource = lstColl;
            ddlIsSecure.DataTextField = "Text";
            ddlIsSecure.DataValueField = "Value";
            ddlIsSecure.DataBind();
            ddlIsSecure.SelectedIndex = 0; // Select the first item 
            ddlIsSecure.AutoPostBack = true;
        }
        protected void ddlIsSecure_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ====================================================================

        private void FillDataInGrid()
        {
            string PortalId = ddlPortalList.SelectedValue;
            bool IsSecure = Convert.ToBoolean(ddlIsSecure.SelectedValue); 
            if (!string.IsNullOrEmpty(PortalId))
            {               
                TabController tab_obj = new TabController();
            List<aspnet_Tab> lstTabs = tab_obj.GetListByPortalIdIsSecure(Convert.ToInt32(PortalId), IsSecure);
                GridView1.ShowHeaderWhenEmpty = true;
                GridView1.PageSize = 15;
                GridView1.EmptyDataText = "No Record Found !!!";
                GridView1.DataSource = lstTabs;
                GridView1.DataBind();                   
            }
        }

        private void ShowNoResultFound(DataTable source, GridView gv)
        {
            source.Rows.Add(source.NewRow()); // create a new blank row to the DataTable
            // Bind the DataTable which contain a blank row to the GridView
            gv.DataSource = source;
            gv.DataBind();
            // Get the total number of columns in the GridView to know what the Column Span should be
            int columnsCount = gv.Columns.Count;
            gv.Rows[0].Cells.Clear();// clear all the cells in the row
            gv.Rows[0].Cells.Add(new TableCell()); //add a new blank cell
            gv.Rows[0].Cells[0].ColumnSpan = columnsCount; //set the column span to the new added cell

            //You can set the styles here
            gv.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            gv.Rows[0].Cells[0].ForeColor = System.Drawing.Color.Black;
            gv.Rows[0].Cells[0].Font.Bold = true;
            //set No Results found to the new added cell
            gv.Rows[0].Cells[0].Text = "No Record Found.";
        }

        protected void EmptyGridFix(GridView grdView)
        {           
            if(grdView.Rows.Count == 0 &&
                grdView.DataSource != null)
            {
                DataTable dt = null;
                if(grdView.DataSource is DataSet)
                {
                    dt = ((DataSet)grdView.DataSource).Tables[0].Clone();
                }
                else if (grdView.DataSource is DataTable)
                    dt = ((DataTable)grdView.DataSource).Clone();

                if(dt == null)
                    return;

                dt.Rows.Add(dt.NewRow()); // add empty row
                grdView.DataSource = dt;
                grdView.DataBind();

                // hide row
                grdView.Rows[0].Visible = false;
                grdView.Rows[0].Controls.Clear();
            }

            // normally executes at all postbacks
            if(grdView.Rows.Count == 1 &&
                grdView.DataSource == null)
            {
                bool bIsGridEmpty = true;

                // check first row that all cells empty
                for(int i = 0; i < grdView.Rows[0].Cells.Count; i++)
                {
                    if(grdView.Rows[0].Cells[i].Text != string.Empty)
                    {
                        bIsGridEmpty = false;
                    }
                }
                // hide row
                if(bIsGridEmpty)
                {
                    grdView.Rows[0].Visible = false;
                    grdView.Rows[0].Controls.Clear();
                }
            }           
        }

        protected void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gvrPager = GridView1.BottomPagerRow;
            DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlPages");
            GridView1.PageIndex = ddlPages.SelectedIndex;
            FillDataInGrid();
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {           
            GridView1.PageIndex = e.NewPageIndex;
            FillDataInGrid();           
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

            Label lblPagingInformation =
                (Label)gvrPager.Cells[0].FindControl("PagingInformation");
            lblPagingInformation.Text = string.Format("You are viewing page {0} of {1}...", GridView1.PageIndex + 1, GridView1.PageCount);



            if (ddlPages != null)
            {
                // populate pager
                for (int i = 0; i < GridView1.PageCount; i++)
                {
                    int intPageNumber = i + 1;
                    ListItem lstItem = new ListItem(intPageNumber.ToString());

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
            FillDataInGrid();
            //GridView1.DataBind();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            FillDataInGrid();
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GridView1.PageIndex = e.NewSelectedIndex;
            GridView1.DataBind();
        }

        protected void btnMultipleDelete_Click(object sender, EventArgs e)
        {
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            foreach (string Item in IDs)
            {
                TabController tab_obj = new TabController();
                int id = int.Parse(Item.ToString());
                int i = tab_obj.Delete(id);
            }
            hdnFldSelectedValues.Value = string.Empty;
            FillDataInGrid();
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            FillDataInGrid();
        }

        protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            // If we just deleted the last row in the GridView, decrement the PageIndex
            if (GridView1.Rows.Count == 1)
                // we just deleted the last row
                GridView1.PageIndex = Math.Max(0, GridView1.PageIndex - 1);
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            //dt.DefaultView.Sort = e.SortExpression;
            //GridView1.DataSource = dt.DefaultView.ToTable();
            //GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                //ImageButton btnShowModule = (ImageButton)e.Row.FindControl("btnShowModule");
                //if (btnShowModule != null)
                //{
                //    btnShowModule.Attributes.Add("onClick", "ShowModuleModal('" + ID + "');");
                //}                

                //e.Row.Attributes.Add("onclick", "ChangeRowColor(this);");
                //e.Row.Attributes.Add("OnDblClick", "ShowEditModal('" + ID + "');");
                e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + ID + "');";

                e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';this.style.color='#9C1E00'";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#454545';";
                e.Row.Attributes["onmousedown"] = "this.style.background='#FACC2E';";
                //e.Row.Attributes["onmouseclick"] = "this.style.background='##228b22';this.style.color='#FFFFFF';";

                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());
            }

            //if(e.Row.RowType != DataControlRowType.Header && e.Row.RowType != DataControlRowType.Footer && e.Row.RowType != DataControlRowType.Pager)
            //{
            //    e.Row.Cells[0].Text = "<input type=\"checkbox\" value=\"" + e.Row.Cells[1].Text + "\" name=\"chkRecordId\" id=\"chkRecordId\"/>";
            //}
        }

      

      

        //protected void btnShowModule_Click(object sender, ImageClickEventArgs e)
        //{
        //    ImageButton btnShowModule = sender as ImageButton;
        //    GridViewRow row = (GridViewRow)btnShowModule.NamingContainer;

        //    string ID = GridView1.DataKeys[row.RowIndex].Values[0].ToString();                
        //    btnShowModule.Attributes.Add("onClick", "ShowModuleModal('" + ID + "');");
             
        //}
    }
}