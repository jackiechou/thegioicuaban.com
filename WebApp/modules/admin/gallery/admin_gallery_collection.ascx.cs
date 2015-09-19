using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GalleryLibrary;
using CommonLibrary.Modules;

namespace WebApp.modules.admin.gallery
{
    public partial class admin_gallery_collection : System.Web.UI.UserControl
    {
        protected static string add_url = "/modules/admin/gallery/admin_gallery_collection_edit.aspx?mode=add";
        protected static string edit_url = "/modules/admin/gallery/admin_gallery_collection_edit.aspx?mode=edit&idx=";
        protected static string loading_url = "/loading.aspx";   

        GalleryCollection collection_obj = new GalleryCollection();
        DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserId"] != null && Session["UserId"].ToString() != string.Empty)
                {
                    LoadStatus2DDL();
                    ShowTreeNodes_Topics();
                    FillDataInGrid();
                }
                else
                {
                    Server.Transfer("~/login.aspx", false);
                }
            }
        }

        #region tree node list - topic =======================================
        private void ShowTreeNodes_Topics()
        {
            ddlTreeNode_Topics.Items.Clear(); //DROPDOWNLIST             
            List<Gallery_Topic> list = GalleryTopic.GetList('1');
            DataTable dt = LinqHelper.ToDataTable(list);
            RecursiveFillTree(dt, 0);

           // ddlTreeNode_Topics.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlTreeNode_Topics.SelectedIndex = 0;
            ddlTreeNode_Topics.AutoPostBack = true;
        }

        int level = -1;
        private void RecursiveFillTree(DataTable dtParent, int ParentId)
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
            dv.RowFilter = string.Format("ParentId = {0}", ParentId);

            int i;

            if (dv.Count > 0)
            {
                for (i = 0; i < dv.Count; i++)
                {
                    //DROPDOWNLIST
                    ddlTreeNode_Topics.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["Gallery_TopicName"].ToString()), dv[i]["Gallery_TopicId"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["Gallery_TopicId"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }

        protected void ddlTreeNode_Topics_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }

        #endregion ==============================================================

        #region Status =====================================================================
        protected void LoadStatus2DDL()
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Hiện", "1"));
            lstColl.Add(new ListItem("Ẩn", "0"));

            ddlStatus.DataSource = lstColl;
            ddlStatus.DataTextField = "Text";
            ddlStatus.DataValueField = "Value";
            ddlStatus.DataBind();
            ddlStatus.SelectedValue = "1";
            ddlStatus.AutoPostBack = true;
        }
        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion =====================================================================================

        private void FillDataInGrid()
        {
            int topicid = Convert.ToInt32(ddlTreeNode_Topics.SelectedValue);
            char status = Convert.ToChar(ddlStatus.SelectedValue);
            List<Gallery_Collection> list = collection_obj.GetList(topicid, status);
            int count = list.Count;
            GridView1.DataSource = list;
            GridView1.DataBind();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //try
            //{
            string userid = Session["UserId"].ToString();
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0].ToString());
            collection_obj.Delete(id);
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
            GridView1.DataBind();
            // Display the primary key value of the selected row.
            //string id = GridView1.DataKeys[e.NewEditIndex].Value.ToString();
            //HyperLink HyperLink_Edit = (HyperLink)GridView1.Rows[e.NewEditIndex].FindControl("HyperLink_Edit");
            //string url = edit_path+"&id=" + id;
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
                if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
                {
                    e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.textDecoration='none';this.style.backgroundColor='#FFFFE1';");
                    e.Row.Attributes.Add("onmouseout", "this.style.cursor='hand';this.style.textDecoration='none';this.style.backgroundColor='#f7fff8';");
                    e.Row.Attributes.Add("onclick", "this.style.cursor='hand';this.style.textDecoration='none';this.style.backgroundColor='#A8CF45';");

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
                string selectedItemId = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                //=============================================================================================================================
                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + ID + "');";
                e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';this.style.color='#9C1E00'";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#454545';";
                e.Row.Attributes["onmouseclick"] = "this.style.background='#FFFFFF';";

                //=============================================================================================================================
                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());
                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());
                try
                {
                    HyperLink hplGetRowSelected = (HyperLink)e.Row.Cells[0].FindControl("hplGetRowSelected");
                    if (hplGetRowSelected != null)
                    {
                        if (Request.QueryString.Count > 0)
                        {
                            string strOptions = string.Empty, createdTabId = string.Empty;
                            HiddenField hdnFldAlias = (HiddenField)e.Row.Cells[0].FindControl("hdnFldAlias");

                            if (selectedItemId != string.Empty)
                                strOptions += "selectedItemId=" + selectedItemId;
                            if (hdnFldAlias.Value != string.Empty)
                                strOptions += "&selectedContent=" + ModuleClass.convertTitle2Link(hdnFldAlias.Value);
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
                        int id = int.Parse(Item.ToString());
                        string userid = Session["UserId"].ToString();
                        collection_obj.UpdateStatus(userid, id, '0');
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
                        string userid = Session["UserId"].ToString();
                        collection_obj.UpdateStatus(userid, id, '1');
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
    }
}