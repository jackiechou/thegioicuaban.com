using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using CommonLibrary.Common;
using ArticleLibrary;
using CommonLibrary.Entities.Portal;

public partial class admin_articles : System.Web.UI.UserControl
{
    ArticleController articles_obj = new ArticleController();
    DataTable dt = new DataTable();

    private string article_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_article_image_dir"];
    private string front_image_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_article_image_dir"] + "/front_images";
    private string main_image_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_article_image_dir"] + "/main_images";

    protected static string base_url = string.Format("{0}://{1}{2}",
                   HttpContext.Current.Request.Url.Scheme,
                   HttpContext.Current.Request.ServerVariables["HTTP_HOST"],
                   (HttpContext.Current.Request.ApplicationPath.Equals("/")) ? string.Empty : HttpContext.Current.Request.ApplicationPath
                   );

    protected static string add_url = "/modules/admin/articles/admin_articles_edit.aspx?mode=add";
    protected static string edit_url = "/modules/admin/articles/admin_articles_edit.aspx?mode=edit&idx=";
    protected static string loading_url = "/loading.aspx";   

    protected void Page_Load(object sender, EventArgs e)
    {
            AttachEvents();
            if (!Page.IsPostBack)
            {
                PopulatePortalList2DDL();
                PopulateCulture2DDL();
                ShowTreeNodes();                //load category into dropdownlist                               
                LoadStatus2DDL();               //Load status into dropdownlist  
                FillDataInGrid();               //load data into gridview     
            }
           // Page.DataBind();
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
    #endregion ============================================================   

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
        FillDataInGrid();
    }
    #endregion ============================================================================

    #region time periods ===================================================
    //protected void LoadPeriods2DDL(){
    //    ArrayList array_data = new ArrayList();
    //    DateTime dt = DateTime.Now;
    //    DateTime wkStDt = DateTime.MinValue;
    //    DateTime wkEndDt = DateTime.MinValue;
    //    DateTime lastDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

    //    wkStDt = dt.AddDays(1 - Convert.ToDouble(dt.DayOfWeek));
    //    wkEndDt = dt.AddDays(7 - Convert.ToDouble(dt.DayOfWeek));
    //    //String.Format("{0:yyyy/MM/dddd}",current_date);
    //    string format = "yyyy-MM-dd";

    //    string current_date = dt.ToString(format);
    //    string current_week = dt.ToString(format);
    //    string current_month = dt.Month.ToString(format);
    //    string last_day_of_month = lastDayOfMonth.ToString(format);
    //    string yesterday = DateTime.Today.AddDays(-1).ToString(format);
    //    string tommorrow = DateTime.Today.AddDays(1).ToString(format);

    //    array_data.Add(new ListItem("Ngày hiện tại", current_date));
    //    array_data.Add(new ListItem("Ngày hôm qua", yesterday));
    //    array_data.Add(new ListItem("Trong tuần này", current_week));
    //    array_data.Add(new ListItem("Trong tháng này", last_day_of_month)); 

    //    // Get an ArrayList with Periods, and bind to dropdownlist
    //    ddlPeriod.DataSource = array_data;
    //    ddlPeriod.DataValueField = "Value";
    //    ddlPeriod.DataTextField = "Text";
    //    ddlPeriod.DataBind();
    //    ddlPeriod.AutoPostBack = true;
    //    ddlPeriod.Items.Insert(0, new ListItem("Chọn thời điểm"));
    //}
    #endregion =======================================================

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
    
    #region events ========================================================
    protected void btnSearch_Click(object sender, ImageClickEventArgs e)
    {
        FillDataInGrid();
    }

    protected void btnInvokeArticleCrawler_Click1(object sender, EventArgs e)
    {
        string userid = Session["UserId"].ToString();
        int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
        string culturecode = ddlCultureList.SelectedValue;
        string code = ddlCategory.SelectedValue;
        int result = articles_obj.GetNewspaper(userid, portalid, culturecode, code, "vnexpress", "doi-song/am-thuc/", 9);
        if (result > 0)
        {
            Response.Write("<script>alert('Cập nhật thành công');</script>");
            Response.End();
        }
        else
        {
            Response.Write("<script>alert('Thông tin đã được cập nhật mới nhất rùi');</script>");
            Response.End();
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        //Get Ids
        string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');
        if (IDs.Length <= 0)
        {
            Response.Write("<script>alert('Vui lòng check chọn 1 dòng');</script>");
            Response.End();
        }
        if (IDs.Length == 1)
        {
            Response.Redirect("~/pages/administrators/index.aspx?type=1&page=articles/admin_articles_edit&id=" + hdnFldSelectedValues.Value.Trim());
        }
        else
        {
            Response.Write("<script>alert('Vui lòng chỉ chọn 1 dòng');</script>");
            Response.End();
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
                    int i = articles_obj.UpdateStatus(userid, id, status);
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
                    int i = articles_obj.UpdateStatus(userid, id, status);
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
        string front_image_dir = Server.MapPath(front_image_dir_path);
        string main_image_dir = Server.MapPath(main_image_dir_path);

        //Code for deleting items
        foreach (string Item in IDs)
        {
            try
            {               
                string userid = Session["UserId"].ToString();
                int id = int.Parse(Item.ToString());
                int i = articles_obj.Delete(userid, id, front_image_dir, main_image_dir);
                FillDataInGrid();
            }
            catch (InvalidOperationException ef) { ef.ToString(); }
        }
    }
    #endregion events =====================================================


    /// <summary>
    /// Initialize the grid view or put these as attributes
    /// </summary>  
    #region GRIDVIEW EVENTS ================================================  
    protected void AttachEvents()
    {
        GridView1.Sorting += GridView1_Sorting;
        GridView1.PageIndexChanging += GridView1_PageIndexChanging;
        GridView1.SelectedIndexChanging +=GridView1_SelectedIndexChanging;
        GridView1.SelectedIndexChanged +=GridView1_SelectedIndexChanged;
        
        GridView1.DataBound +=GridView1_DataBound;
        GridView1.RowCreated +=GridView1_RowCreated;
        GridView1.RowDataBound += GridView1_RowDataBound;    
        GridView1.RowDeleting +=GridView1_RowDeleting;
    }

    private static void AllowSortingAndPaging(GridView gv, bool IsAllowPaging, bool IsAllowSorting)
    {
        gv.AllowPaging = IsAllowPaging;
        gv.AllowSorting = IsAllowSorting;
        gv.PageSize = 13; 
    }
   
    private static int GetKeyFromDataKeys(GridView gv, int rowIndex)
    {
        //assumes key is an integer first in DataKeys. Adjust for strings or multiple IDs.
        if (rowIndex <= gv.DataKeys.Count)
            return (int)gv.DataKeys[rowIndex][0];
        return -1; //might throw an error here
    }

    private void FillDataInGrid()
    {
        int portalid = Convert.ToInt32(ddlPortalList.SelectedValue);
        string culturecode = ddlCultureList.SelectedValue;
        string code = ddlCategory.SelectedValue;
        string status = ddlStatus.SelectedValue;

        string begindate = "";
        string unformatted_date = txtStartDate.Text;
        if (unformatted_date != string.Empty)
        {
            System.Globalization.DateTimeFormatInfo MyDateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo();
            MyDateTimeFormatInfo.ShortDatePattern = "dd-MM-yyyy";
            DateTime dateTime = DateTime.Parse(unformatted_date, MyDateTimeFormatInfo);
            begindate = dateTime.ToString("yyyy-MM-dd");
        }

        string enddate = "";
        string unformatted_enddate = txtEndDate.Text;
        if (unformatted_enddate != string.Empty)
        {
            System.Globalization.DateTimeFormatInfo MyDateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo();
            MyDateTimeFormatInfo.ShortDatePattern = "dd-MM-yyyy";
            DateTime dateTime = DateTime.Parse(unformatted_enddate, MyDateTimeFormatInfo);
            enddate = dateTime.ToString("yyyy-MM-dd");
        }

        dt = articles_obj.GetDataByConditions(portalid, culturecode, begindate, enddate, code, status);  
        AllowSortingAndPaging(GridView1, true, true);
        GridView1.DataKeyNames = new string[] { "ArticleId" };
        GridView1.AutoGenerateSelectButton = false;
        GridView1.AutoGenerateEditButton = false;

        GridView1.DataSource = dt;
        GridView1.DataBind();   
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            if (lblStatus != null)
            {
                if (lblStatus.Text == "2")
                {
                    lblStatus.Text = "Publish";
                }
                else if (lblStatus.Text == "1")
                {
                    lblStatus.Text = "Active";
                }
                else if (lblStatus.Text == "0")
                {
                    lblStatus.Text = "InActive";
                }
                else
                {
                    lblStatus.Text = "Error";
                }
            }

            string selectedItemId = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
            e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + selectedItemId + "');";
            //e.Row.Attributes.Add("OnDblClick", "ShowEditModal('" + selectedItemId + "');");  //ok
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

                        HiddenField hdnFldAlias = (HiddenField)e.Row.Cells[0].FindControl("hdnFldAlias");
                        if (e.Row.Cells[3].Text != string.Empty)
                            strOptions += "selectedItemId=" + e.Row.Cells[3].Text;
                        if (hdnFldAlias.Value != string.Empty)
                            strOptions += "&selectedContent=" + hdnFldAlias.Value;

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

            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record?')");
            }
           
          
            CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
            CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
            HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

            chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());
        }
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
    protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
    {
        //GridView gv = (GridView)sender;
        //gv.PageIndex = 0; //always sort to page 1
        //GetSortDirection(e.SortExpression); //sort direction is always ascending, so we do this manually
        //GridViewSortBy = e.SortExpression; //sortBy is stored so we can change direction next time
        //FillDataInGrid();

        dt.DefaultView.Sort = e.SortExpression;
        GridView1.DataSource = dt.DefaultView.ToTable();
        GridView1.DataBind();
    }
    private void GetSortDirection(string sortBy)
    {
        //swap the sort direction if sorting same column
        if (GridViewSortBy != sortBy)
            GridViewSortAscending = true;
        else
            GridViewSortAscending = !GridViewSortAscending;
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
        //GridView gv = (GridView)sender;
        //gv.PageIndex = e.NewPageIndex;
        GridView1.PageIndex = e.NewPageIndex;
        FillDataInGrid();
    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
            {
                //e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#FFFFE1';");
                //e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#f7fff8';");
                //e.Row.Attributes.Add("onclick", "this.style.cursor='hand';this.style.textDecoration='none';this.style.backgroundColor='#A8CF45';");
                //string url = edit_path + "&id=" + (e.Row.RowIndex + 1);
                //e.Row.Attributes.Add("ondblclick", "Javascript:__doPostBack('myDblClick','" + url + "');");                                 
            }
            else
            {
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#FFFFE1';");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#eefef0';");
            }
        }
    }

    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int i = 0;
        string front_image_dir= Server.MapPath(front_image_dir_path);
        string main_image_dir = Server.MapPath(main_image_dir_path);

        string userid = Session["UserId"].ToString();
        // string id = GridView1.DataKeys[e.RowIndex].Values[0].ToString();
        int id = int.Parse(GridView1.Rows[e.RowIndex].Cells[1].Text);
        i = articles_obj.Delete(userid, id, front_image_dir, main_image_dir);
        FillDataInGrid();
        //}
        //catch (ArgumentOutOfRangeException ex) { ex.ToString(); }
    }

    #region Properties storing GridView data and settings
    private bool GridViewSortAscending
    {
        get { return (ViewState["SortDirection"] == null) ? true : (bool)ViewState["SortDirection"]; }
        set { ViewState["SortDirection"] = value; }
    }

    private string GridViewSortBy
    {
        get { return ViewState["SortBy"] as string ?? ""; }
        set { ViewState["SortBy"] = value; }
    }

    #endregion

    #endregion GRIDVIEW EVENTS ================================================
}
