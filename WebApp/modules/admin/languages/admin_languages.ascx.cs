using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Common;

namespace WebApp.modules.admin.languages
{
    public partial class admin_languages : System.Web.UI.UserControl
    {
        CultureClass culture_obj = new CultureClass();
        DataTable dt = new DataTable();

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Page.EnableEventValidation = false;
            Page.MaintainScrollPositionOnPostBack = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadDiscontinued2DDL();
                FillDataInGrid();
            }
       }

        #region status ==================================================
        protected void LoadDiscontinued2DDL()
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Hiện", "1"));
            lstColl.Add(new ListItem("Ẩn", "0"));

            ddlDiscontinued.DataSource = lstColl;
            ddlDiscontinued.DataTextField = "Text";
            ddlDiscontinued.DataValueField = "Value";
            ddlDiscontinued.DataBind();
            ddlDiscontinued.SelectedIndex = 0; // Select the first item
            ddlDiscontinued.AutoPostBack = true;
        }
        protected void ddlDiscontinued_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ======================================================

        private void FillDataInGrid()
        {
            string Discontinued = ddlDiscontinued.SelectedValue;
            dt = culture_obj.GetListByDiscontinued(Discontinued);

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

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
            //{
            //    FillDataInGrid();
            //}
            if (GridView1.Rows.Count > 0)
            {
                //This replaces <td> with <th> and adds the scope attribute
                GridView1.UseAccessibleHeader = true;

                //This will add the <thead> and <tbody> elements
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;

                //This adds the <tfoot> element. Remove if you don't have a footer row
                //GridView1.FooterRow.TableSection = TableRowSection.TableFooter;                
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate))
            { 
                //==========================================================================               
                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + ID + "');";

                e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';this.style.color='#9C1E00'";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#454545';";
                e.Row.Attributes["onmousedown"] = "this.style.background='#FACC2E';";
                //e.Row.Attributes["onmouseclick"] = "this.style.background='##228b22';this.style.color='#FFFFFF';";

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
            FillDataInGrid();
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
            // a method to populate your grid GridView1.DataBind(); 
            FillDataInGrid();
        }

        protected void btnLock_Click(object sender, EventArgs e)
        {         
            int status = 0;

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                CheckBox chkBxSelect = GridView1.Rows[i].FindControl("chkBxSelect") as CheckBox;
                if (chkBxSelect.Checked)
                {
                    int id = Convert.ToInt32(GridView1.Rows[i].Cells[2].Text);
                    int result = culture_obj.UpdateDiscontinued(id, status);
                }
            }
            FillDataInGrid();
            //Page.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }

        protected void btnUnLock_Click(object sender, EventArgs e)
        {          
            int status = 1;

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                CheckBox chkBxSelect = GridView1.Rows[i].FindControl("chkBxSelect") as CheckBox;
                if (chkBxSelect.Checked)
                {
                    int id = Convert.ToInt32(GridView1.Rows[i].Cells[2].Text);
                    int result = culture_obj.UpdateDiscontinued(id, status);
                }
            }
            FillDataInGrid();
            //Page.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            FillDataInGrid();
        }       
    }
}