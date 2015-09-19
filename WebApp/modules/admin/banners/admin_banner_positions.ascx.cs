using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Services.Banner;

public partial class admin_banner_positions : System.Web.UI.UserControl
{
    protected static string add_url = "/modules/admin/banners/admin_banner_position_add.aspx?mode=add";
    protected static string edit_url = "/modules/admin/banners/admin_banner_position_edit.aspx?mode=edit&idx=";
    protected static string loading_url = "/loading.aspx";


    BannerPosition banner_position_obj = new BannerPosition();
    public DataTable dt = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {       
        if (!Page.IsPostBack)       
            FillDataInGrid();           
    }

    protected void btnAdd_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/pages/administrators/index.aspx?type=1&page=banner_positions/admin_banner_position_add");       
    }
    
    private void FillDataInGrid()
    {                
        DataTable dt = banner_position_obj.GetTreeNodes(0);

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


    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //try
        //{
        int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0].ToString());
        string status = "0";
        int i = banner_position_obj.UpdateStatus(id, status);
        FillDataInGrid();
        //}
        //catch (ArgumentOutOfRangeException ef) { ef.ToString(); }
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
        string id = GridView1.DataKeys[e.NewEditIndex].Value.ToString();
        HyperLink HyperLink_Edit = (HyperLink)GridView1.Rows[e.NewEditIndex].FindControl("HyperLink_Edit");
        string url = "pages/administrators/index.aspx?type=1&page=banner_positions/admin_banner_position_edit&id=" + id;
        HyperLink_Edit.Attributes.Add("onclick", "location.href='" + url + "';");
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
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
            {
                e.Row.Attributes.Add("onclick", "this.style.backgroundColor='#cc6600';");

                // Display the primary key value of the selected row.
                // string ID = GridView1.DataKeys[e.Row.DataItemIndex].Values[0].ToString();
                //string url = "Default.aspx?page=banner_positions/admin_banner_position_edit&id=" + ID;
                //e.Row.Attributes.Add("onDblclick", "location.href='" + url + "';");
            }
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


    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            if (lblStatus != null)
            {
                if (lblStatus.Text == "0")
                {
                    lblStatus.Text = "InActive";
                }
                else
                {
                    lblStatus.Text = "Active";
                }
            }

            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                //btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record " + DataBinder.Eval(e.Row.DataItem, "CountryID") + "')");
                btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record?')");
            }           
           
            //==========================================================================               
            string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
            e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + ID + "');";
            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';this.style.color='#9C1E00'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#454545';";
            e.Row.Attributes["onmousedown"] = "this.style.background='#FACC2E';";
            //e.Row.Attributes["onmouseclick"] = "this.style.background='##228b22';this.style.color='#FFFFFF';";

            //==================================================================================================
            CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
            CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
            HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

            chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());


        }
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            // Get the index of the row being changed
            int row_index = Convert.ToInt32((string)e.CommandArgument);
            int id = (int)GridView1.DataKeys[row_index].Value;

            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/pages/administrators/index.aspx?type=1&page=banner_positions/admin_banner_position_edit&id=" + id);
            }

            if (e.CommandName == "UpdateSortKey")
            {
                TextBox txtSortKey = (TextBox)GridView1.Rows[row_index].FindControl("txtSortKey");
                if (txtSortKey != null)
                {
                    int sortkey = Convert.ToInt32(txtSortKey.Text);
                    banner_position_obj.UpdateSortKey(id, sortkey);
                    Response.Redirect("~/Pages/Administrators/Index.aspx?page=banner_positions/admin_banner_positions");
                }
            }
        }
        catch (Exception ex) { ex.ToString(); }
    }

    protected void btnLock_Click(object sender, EventArgs e)
    {
        try
        {
            //Get Ids
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            //Code for deleting items
            foreach (string Item in IDs)
            {
                try
                {
                    int id = int.Parse(Item.ToString());
                    string status = "0";
                    int i = banner_position_obj.UpdateStatus(id, status);
                    FillDataInGrid();
                }
                catch (InvalidOperationException ef) { ef.ToString(); }
            }
        }
        catch (FormatException ex)
        {
            ex.ToString();
        }
    }

    protected void btnUnLock_Click(object sender, EventArgs e)
    {
        try
        {
            //Get Ids
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            //Code for deleting items
            foreach (string Item in IDs)
            {
                try
                {
                    int id = int.Parse(Item.ToString());
                    string status = "1";
                    int i = banner_position_obj.UpdateStatus(id, status);
                    FillDataInGrid();
                }
                catch (InvalidOperationException ef) { ef.ToString(); }
            }
        }
        catch (FormatException ex)
        {
            ex.ToString();
        }
    }

    protected void btnMultipleDelete_Click(object sender, EventArgs e)
    {
        //Get Ids
        string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

        //Code for deleting items
        foreach (string Item in IDs)
        {
            try
            {
                int id = int.Parse(Item.ToString());
                int i = banner_position_obj.Delete(id);
                FillDataInGrid();
            }
            catch (InvalidOperationException ef) { ef.ToString(); }
        }
    }

   
   
}