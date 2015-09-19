using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ArticleLibrary;
using System.Data;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common;

public partial class admin_article_comments : System.Web.UI.UserControl
{
    protected static string edit_url = "/modules/admin/articles/admin_article_comments_edit.aspx?mode=edit&idx=";
    protected static string loading_url = "/loading.aspx";

    ArticleCommentController article_comment_obj = new ArticleCommentController();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulatePortalList2DDL();
            PopulateCulture2DDL();
            ShowTreeNodes();
            PopulateArticle2DDL();
            LoadStatus2DDL();
            FillDataInGrid();
        }
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

    #region tree node list - category =======================================
    private void ShowTreeNodes()
    {
        ddlCategory.Items.Clear(); //DROPDOWNLIST        
        int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
        string culturecode = ddlCultureList.SelectedValue;
        ArticleCategoryController objTree = new ArticleCategoryController();
        DataTable dtNodes = objTree.GetActiveList(portalid, culturecode); //select all the nodes from DB
        RecursiveFillTree(dtNodes, 0);

        ddlCategory.Items.Insert(0, new ListItem("-Chọn nhóm tin tức-", "")); //DROPDOWNLIST
        ddlCategory.SelectedIndex = 0;
        ddlCategory.EnableViewState = true;
        ddlCategory.AutoPostBack = true;
    }

    int level = -1;
    private void RecursiveFillTree(DataTable dtParent, int parentID)
    {
        level++; //on the each call level increment 1
        System.Text.StringBuilder appender = new System.Text.StringBuilder();

        for (int j = 0; j < level; j++)
        {
            appender.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
        }
        if (level > 0)
        {
            appender.Append("|__");
        }

        DataView dv = new DataView(dtParent);
        dv.RowFilter = string.Format("ParentID = {0}", parentID);

        int i;

        if (dv.Count > 0)
        {
            for (i = 0; i < dv.Count; i++)
            {
                //DROPDOWNLIST
                ddlCategory.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["CategoryName"].ToString()), dv[i]["CategoryCode"].ToString()));
                RecursiveFillTree(dtParent, int.Parse(dv[i]["CategoryId"].ToString()));
            }
        }

        level--; //on the each function end level will decrement by 1
    }
    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateArticle2DDL();
        FillDataInGrid();
    }
    #endregion ============================================================================

    #region Articles ======================================================
    protected void PopulateArticle2DDL()
    {
        string code = ddlCategory.SelectedValue;
        ArticleController article_obj = new ArticleController();
        DataTable dt = article_obj.GetListByCode(code);
        ddlArticles.Items.Clear();
        if (dt.Rows.Count > 0)
        {
            ddlArticles.DataSource = dt;
            ddlArticles.DataTextField = "Title";
            ddlArticles.DataValueField = "ArticleId";
            ddlArticles.DataBind();
        }
        else
        {
            ddlArticles.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlArticles.SelectedIndex = 0;
            ddlArticles.DataBind();
        }
        ddlArticles.AutoPostBack = true;
    }
    protected void ddlArticles_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDataInGrid();
    }
    #endregion ====================================================================

    #region status ==================================================
    protected void LoadStatus2DDL()
    {
        //Load list item to dropdownlist
        ListItemCollection lstColl = new ListItemCollection();
        lstColl.Add(new ListItem("Publish", "2"));
        lstColl.Add(new ListItem("Active", "1"));
        lstColl.Add(new ListItem("InActive", "0"));

        ddlStatus.DataSource = lstColl;
        ddlStatus.DataTextField = "Text";
        ddlStatus.DataValueField = "Value";
        ddlStatus.DataBind();
        ddlStatus.Items.Insert(0, new ListItem("Chọn trạng thái", "")); // add the new item at the top of the list
        ddlStatus.SelectedIndex = 0; // Select the first item
        ddlStatus.AutoPostBack = true;
    }
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillDataInGrid();
    }
    #endregion ======================================================

    private void FillDataInGrid()
    {
        int articleid = Convert.ToInt32(ddlArticles.SelectedValue);
        string status = ddlStatus.SelectedValue;
        List<ArticleComment> lst = article_comment_obj.GetListByArticleIdStatus(articleid, status);

        GridView1.DataSource = lst;
        GridView1.DataBind();
    }

    protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        GridView1.PageIndex = e.NewSelectedIndex;
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
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        FillDataInGrid();
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
        FillDataInGrid();
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Get the currently selected row using the SelectedRow
        GridViewRow row = GridView1.SelectedRow;
        GridView1.SelectedIndex = -1;
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
            //    btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record?')");
            //}

            string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
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
                    int id = Convert.ToInt32(Item.ToString());
                    string status = "0";
                    int i = article_comment_obj.UpdateStatus(userid, id, status);
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
                    int id = Convert.ToInt32(Item.ToString());
                    string status = "1";
                    int i = article_comment_obj.UpdateStatus(userid, id, status);
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
                    int id = Convert.ToInt32(Item.ToString());
                    int i = article_comment_obj.Delete(userid, id);
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
}