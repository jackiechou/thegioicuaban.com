using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaLibrary;

namespace WebApp.modules.admin.media
{
    public partial class admin_media_composer : System.Web.UI.UserControl
    {
        protected static string add_url = "/modules/admin/media/admin_media_composer_edit.aspx?mode=add";
        protected static string edit_url = "/modules/admin/media/admin_media_composer_edit.aspx?mode=edit&idx=";
        protected static string loading_url = "/loading.aspx";
        
        private List<Media_Composers> StoredData
        {
            get { return ViewState["StoredData"] as List<Media_Composers> ?? null; }
            set { ViewState["StoredData"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MediaComposers dal = new MediaComposers(StoredData);
            if (!Page.IsPostBack)
            {
                LoadStatus2DDL();
                PageAndBind(GridView1);
            }
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            PageAndBind(GridView1);
        }

        #region status ==================================================
        protected void LoadStatus2DDL()
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Unpublish", "0"));
            lstColl.Add(new ListItem("Published", "1"));

            ddlStatus.DataSource = lstColl;
            ddlStatus.DataTextField = "Text";
            ddlStatus.DataValueField = "Value";
            ddlStatus.DataBind();
            ddlStatus.SelectedIndex = 1;
            ddlStatus.AutoPostBack = true;
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageAndBind(GridView1);
        }
        #endregion ======================================================

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

        private void GetSortDirection(string sortBy)
        {
            //swap the sort direction if sorting same column
            if (GridViewSortBy != sortBy)
                GridViewSortAscending = true;
            else
                GridViewSortAscending = !GridViewSortAscending;
        }
        #endregion

        private void PageAndBind(GridView gv)
        {
            IList<Object> list = new List<Object>();
            int pageSize = 3;
            int startRow = (gv.PageIndex * pageSize);
            string status = ddlStatus.SelectedValue;
            IList<Media_Composers> data =
                MediaComposers.LoadDatedItemsByPage(status, startRow, pageSize, GridViewSortBy, GridViewSortAscending);
            //copy data into generic list
            foreach (Media_Composers item in data)
                list.Add(item);
            int count = MediaComposers.LoadDatedItemsCount(); //you could cache this
            //here's the bit where we use create an ODS in code to fill the gridview with paged data
            CommonLibrary.UI.GridView.GridViewFiller.Fill(gv, list, count, pageSize);
        }

        #region GridView events =================================================================================
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.PageIndex = 0; //always sort to page 1

            GetSortDirection(e.SortExpression); //sort direction is always ascending, so we do this manually
            GridViewSortBy = e.SortExpression; //sortBy is stored so we can change direction next time

            PageAndBind(gv);
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.PageIndex = e.NewPageIndex;
            PageAndBind(gv);
        }
        protected void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gvrPager = GridView1.BottomPagerRow;
            DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlPages");
            GridView1.PageIndex = ddlPages.SelectedIndex;
            PageAndBind(GridView1);
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
                        lblStatus.Text = "UnPublished";
                    }
                    else if (lblStatus.Text == "1")
                    {
                        lblStatus.Text = "Published";
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
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridView gv = (GridView)sender;
            //to get index and row
            int rowIndex = e.RowIndex; //here we have the row directly
            //GridViewRow row = gv.Rows[rowIndex]; //the row
            int key = (int)gv.DataKeys[rowIndex][0]; //int key = Convert.ToInt32(gv.DataKeys[e.RowIndex].Values[0].ToString());
            //int key = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0].ToString());
            string UserId = Session["UserId"].ToString();
            string Status = "1";
            MediaComposers composer_obj = new MediaComposers();           
            composer_obj.UpdateStatus(UserId, key, Status);
            //composer_obj.Delete(key);
            if (rowIndex == 0 && gv.Rows.Count == 1)
                gv.PageIndex = gv.PageIndex - 1; //only 1 item on page- page back
            PageAndBind(gv); //load the new information
        }
        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.PageIndex = e.NewSelectedIndex;
            PageAndBind(gv);
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
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.EditIndex = e.NewEditIndex;
            PageAndBind(gv);
        }
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.EditIndex = -1;
            PageAndBind(gv);
        }
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the currently selected row using the SelectedRow
            GridViewRow row = GridView1.SelectedRow;
            GridView1.SelectedIndex = -1;
        }
        #endregion =========================================================================================       
    }
}