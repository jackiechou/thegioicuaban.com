using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Common;
using ArticleLibrary;
using CommonLibrary.Entities.Portal;

public partial class user_controls_admin_controls_admin_article_categories : System.Web.UI.UserControl
{
    protected static string add_url = "/modules/admin/articles/admin_article_categories_edit.aspx?mode=add";
    protected static string edit_url = "/modules/admin/articles/admin_article_categories_edit.aspx?mode=edit&idx=";
    protected static string loading_url = "/loading.aspx";

    //variables to store sortexpression and direction
    private string sortExpression;
    private SortDirection sortDirection;

    ArticleCategoryController article_cate_obj = new ArticleCategoryController();
    DataTable dt = new DataTable();

    protected void Page_Load(object  sender, EventArgs e)
    {   
        if (Session["UserId"] != null && Session["UserId"].ToString() != string.Empty)
        {
            Page.ClientScript.GetPostBackEventReference(this, string.Empty);
            if (!Page.IsPostBack)
            {
                PopulatePortalList2DDL();
                PopulateCulture2DDL();
                FillDataInGrid();
            }
            else
            {
                //if it is a postback, fetch SortExpression and Direction from viewstate
                //and store it in local variables
                if (ViewState["SortExpression"] != null)
                    sortExpression = ViewState["SortExpression"].ToString();
                else
                    sortExpression = String.Empty;

                if (ViewState["SortDirection"] != null)
                {
                    if (Convert.ToInt32(ViewState["SortDirection"]) == (int)SortDirection.Ascending)
                    {
                        sortDirection = SortDirection.Ascending;
                    }
                    else
                    {
                        sortDirection = SortDirection.Descending;
                    }
                }
            }
        }
        else
            Response.RedirectToRoutePermanent("admin_login");
    }

    #region Portal List ==================================================
    private void PopulatePortalList2DDL()
    {
        PortalController portal_obj = new PortalController();
        DataTable dtNodes = portal_obj.GetList();

        ddlPortalList.Items.Clear();
        ddlPortalList.DataSource = dtNodes;
        ddlPortalList.DataTextField = "PortalName";
        ddlPortalList.DataValueField = "PortalId";
        ddlPortalList.DataBind();
        //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
        ddlPortalList.SelectedIndex = 0;
        ddlPortalList.AutoPostBack = true;
    }
    protected void ddlPortalList_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDataInGrid();
    }
    #endregion ===============================================================

    #region Culture ======================================================
    protected void PopulateCulture2DDL()
    {
        CultureClass culture_obj = new CultureClass();
        string Discontinued = "1";
        DataTable dt = culture_obj.GetListByDiscontinued(Discontinued);

        ddlCultureList.Items.Clear();
        ddlCultureList.DataSource = dt;
        ddlCultureList.DataTextField = "CultureName";
        ddlCultureList.DataValueField = "CultureCode";        
        ddlCultureList.DataBind();
        ddlCultureList.SelectedValue = "vi-VN";
        ddlCultureList.AutoPostBack = true;
    }
    protected void ddlCultureList_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDataInGrid();
    }
    #endregion ====================================================================    


    #region METHOD ================================================================
    private void FillDataInGrid()
    {
        int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
        string culturecode = ddlCultureList.SelectedValue;
        int node = 0;
        DataTable dt = new DataTable(); ;

        if (ViewState["ARTICLE_CATEGORY"] != null)        
            dt = (DataTable)ViewState["ARTICLE_CATEGORY"];
        else
        {
            //fetch data from database and store it in viewstate for subsequent postbacks
            dt = article_cate_obj.GetAllTreeNodes(portalid, culturecode, node);
            ViewState["ARTICLE_CATEGORY"] = dt;
        }

        //prepare the sort string based on the sortexpression and direction (such as ID DESC)
        String sort = String.Empty;
        if (null != sortExpression && String.Empty != sortExpression)
        {
            sort = String.Format("{0} {1}", sortExpression, (sortDirection == SortDirection.Descending) ? "DESC" : "ASC");
        }
        //Sort rows in dsProducts based on sort string and bind the view to the grid
        DataView dv = new DataView(dt, String.Empty, sort, DataViewRowState.CurrentRows);

        if (dt.Rows.Count > 0)
        {
            GridView1.DataSource = dv;
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
    #endregion ====================================================================

    
    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //try
        //{
        string userid = Session["UserId"].ToString();
        int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0].ToString());
        string status = "0";
        int i = article_cate_obj.UpdateStatus(userid, id, status);
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
        //check if it is a header row
        //since allowsorting is set to true, column names are added as command arguments to
        //the linkbuttons by DOTNET API
        if (e.Row.RowType == DataControlRowType.Header)
        {
            //LinkButton btnSort = new LinkButton();
            //Image image;
            ////iterate through all the header cells
            //foreach (TableCell cell in e.Row.Cells)
            //{
            //    //check if the header cell has any child controls
            //    if (cell.HasControls())
            //    {
            //        //get reference to the button column
            //        btnSort = (LinkButton)cell.Controls[0];
            //        image = new Image();
            //        if (ViewState["SortExpression"] != null)
            //        {
            //            //see if the button user clicked on and the sortexpression in the viewstate are same
            //            //this check is needed to figure out whether to add the image to this header column nor not
            //            if (btnSort.CommandArgument == ViewState["SortExpression"].ToString())
            //            {
            //                //following snippet figure out whether to add the up or down arrow
            //                //based on the sortdirection
            //                if (Convert.ToInt32(ViewState["SortDirection"]) == (int)SortDirection.Ascending)
            //                {
            //                    image.ImageUrl = "~/images/icons/up.gif";
            //                }
            //                else
            //                {
            //                    image.ImageUrl = "~/images/icons/down.gif";
            //                }
            //                cell.Controls.Add(image);
            //            }
            //        }
            //    }
            //}
        }
    }
    protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
    {
        // SortDirection is always set to "Ascending". So we have to store sortdirection in viewstate
        // which will then be retrieved and used while adding up or down arrows in rowcreated event           
        if (String.Empty != sortExpression)
        {
            if (String.Compare(e.SortExpression, sortExpression, true) == 0)
            {
                sortDirection =
                    (sortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
            }
            else
            {
                sortDirection = SortDirection.Ascending;
            }
        }
        sortExpression = e.SortExpression;
        ViewState["SortExpression"] = e.SortExpression;
        ViewState["SortDirection"] = (int)sortDirection;
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
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            if (lblStatus != null)
            {
                if (lblStatus.Text == "0")
                {
                    lblStatus.Text = "InActive";
                }
                else if (lblStatus.Text == "1")
                {
                    lblStatus.Text = "Active";
                }
                else if (lblStatus.Text == "2")
                {
                    lblStatus.Text = "Publish";
                }
            }

            //ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            //if (btnDelete != null)
            //{
            //    //btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record " + DataBinder.Eval(e.Row.DataItem, "CountryID") + "')");
            //    btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record?')");
            //}
            //==========================================================================               
            string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
            e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + ID + "');";

            e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';this.style.color='#9C1E00'";
            e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#454545';";
            e.Row.Attributes["onmousedown"] = "this.style.background='#FACC2E';";
            //e.Row.Attributes["onmouseclick"] = "this.style.background='##228b22';this.style.color='#FFFFFF';";

            try
            {
                HyperLink hplGetRowSelected = (HyperLink)e.Row.Cells[0].FindControl("hplGetRowSelected");
                if (hplGetRowSelected != null)
                {
                    if (Request.QueryString.Count > 0)
                    {
                        string strOptions = string.Empty, createdTabId = string.Empty;

                        if (e.Row.Cells[3].Text != string.Empty)
                            strOptions += "selectedItemId=" + e.Row.Cells[3].Text;
                        if (e.Row.Cells[5].Text != string.Empty)
                            strOptions += "&selectedContent=" + e.Row.Cells[5].Text;

                        if (Request.QueryString["portalId"].ToString() != "" && Request.QueryString["portalId"].ToString() != "")
                            strOptions += "&portalId=" + Request.QueryString["portalId"].ToString();
                        if (Request.QueryString["selectedTabId"].ToString() != "" && Request.QueryString["selectedTabId"].ToString() != "")
                            strOptions += "&selectedTabId=" + Request.QueryString["selectedTabId"].ToString();
                        if (Request.QueryString["contentItemKey"].ToString() != "" && Request.QueryString["contentItemKey"].ToString() != "")
                            strOptions += "&contentItemKey=" + Request.QueryString["contentItemKey"].ToString();
                        if (Request.QueryString["keyWords"].ToString() != "" && Request.QueryString["keyWords"].ToString() != "")
                            strOptions += "&keyWords=" + Request.QueryString["keyWords"].ToString();
                        if (Request.QueryString["createdTabId"] != null && Request.QueryString["createdTabId"].ToString() != string.Empty)
                            strOptions += "&createdTabId=" + Request.QueryString["CreatedTabId"].ToString();
                        if (Request.QueryString["createdTabPath"].ToString() != "" && Request.QueryString["createdTabPath"].ToString() != "")
                            strOptions += "&createdTabPath=" + Request.QueryString["createdTabPath"].ToString();
                        if (Request.QueryString["lang"].ToString() != "" && Request.QueryString["lang"].ToString() != "")
                            strOptions += "&lang=" + Request.QueryString["lang"].ToString();
                        if (Request.QueryString["router"].ToString() != "" && Request.QueryString["router"].ToString() != "")
                            strOptions += "&router=" + Request.QueryString["router"].ToString();
                        hplGetRowSelected.NavigateUrl = "/modules/admin/tabs/admin_front_tab_router_add.aspx?" + strOptions;
                    }
                }
            }
            catch (NoNullAllowedException ex) { ex.ToString(); }

            CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
            CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
            HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

            chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());

        }
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
                    string userid = Session["UserId"].ToString();
                    int id = int.Parse(Item.ToString());
                    string status = "0";
                    int i = article_cate_obj.UpdateStatus(userid, id, status);
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
                    string userid = Session["UserId"].ToString();
                    int id = int.Parse(Item.ToString());
                    string status = "1";
                    int i = article_cate_obj.UpdateStatus(userid, id, status);
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

    protected void btnReload_Click(object sender, EventArgs e)
    {
        FillDataInGrid();
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try{
        int index = Convert.ToInt32(e.CommandArgument.ToString());
        int id = Convert.ToInt32(GridView1.DataKeys[index]["CategoryId"].ToString());

        if (e.CommandName == "Up")
        {
            //int previous_id = Convert.ToInt32(GridView1.DataKeys[index - 1]["Id"].ToString());
            int option = 1;
            article_cate_obj.UpdateSortKey(option, id);
        }
        if (e.CommandName == "Down")
        {
            //int previous_id = Convert.ToInt32(GridView1.DataKeys[index + 1]["Id"].ToString());
            int option = 0;
            article_cate_obj.UpdateSortKey(option, id);
        }
        FillDataInGrid();
        }catch(FormatException aex){aex.ToString();}
    }

    
}