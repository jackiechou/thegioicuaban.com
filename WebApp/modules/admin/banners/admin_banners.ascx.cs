using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Services.Banner;

namespace WebApp.modules.admin.banners
{
    public partial class admin_banners : System.Web.UI.UserControl
    {
        private string banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"];
        private string thumb_banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"] + "/thumb_images";
        private string main_banner_dir_path = "~/" + System.Configuration.ConfigurationManager.AppSettings["upload_banner_image_dir"] + "/main_images";
        protected static string add_url = "/modules/admin/banners/admin_banners_add.aspx?mode=add";
        protected static string edit_url = "/modules/admin/banners/admin_banners_edit.aspx?mode=edit&idx=";
        protected static string loading_url = "/loading.aspx";

        BannerController banner_obj = new BannerController();
        DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadPosition2DDL();
                LoadStatus2DDL();
                FillDataInGrid();
            }
        }

        #region BANNER POSITION ==================================================
        protected void LoadPosition2DDL()
        {
            ddlPosition.Items.Clear(); //DROPDOWNLIST        
            string status = "1";
            BannerPosition objTree = new BannerPosition();
            DataTable dtNodes = objTree.GetListByStatus(status); //select all the nodes from DB
            RecursiveFillTree(dtNodes, 0);

           // ddlPosition.Items.Insert(0, new ListItem("- Root -", "0")); //DROPDOWNLIST
            ddlPosition.SelectedIndex = 0;
            ddlPosition.AutoPostBack = true;
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
            dv.RowFilter = string.Format("ParentId = {0}", parentID);

            int i;

            if (dv.Count > 0)
            {
                for (i = 0; i < dv.Count; i++)
                {
                    //DROPDOWNLIST
                    ddlPosition.Items.Add(new ListItem(Server.HtmlDecode(appender.ToString() + dv[i]["BannerPosition"].ToString()), dv[i]["Id"].ToString()));
                    RecursiveFillTree(dtParent, int.Parse(dv[i]["Id"].ToString()));
                }
            }

            level--; //on the each function end level will decrement by 1
        }
        #endregion ======================================================

        #region status ==================================================
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
           // ddlStatus.Items.Insert(0, new ListItem("Chọn trạng thái", "")); // add the new item at the top of the list
            ddlStatus.SelectedIndex = 0; // Select the first item
            ddlStatus.AutoPostBack = true;
        }
        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ======================================================


        #region METHODS ===============================================================================
        private void FillDataInGrid()
        {
            int position = Convert.ToInt32(ddlPosition.SelectedValue);
            string status = ddlStatus.SelectedValue;
            dt = banner_obj.GetListByPositionStatus(position, status);

            GridView1.DataSource = dt;
            GridView1.DataBind();
            //GridViewRow FirstRow = GridView1.Rows[0];
            //Button btnUp = (Button)FirstRow.FindControl("btnUp");
            //btnUp.Enabled = false;
            //GridViewRow LastRow = GridView1.Rows[GridView1.Rows.Count - 1];
            //Button btnDown = (Button)LastRow.FindControl("btnDown");
            //btnDown.Enabled = false;    
        }
        #endregion ====================================================================================
        

        #region GRID - EVENTS ========================================================================
        protected void ddlPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
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

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the currently selected row using the SelectedRow
            GridViewRow row = GridView1.SelectedRow;
            GridView1.SelectedIndex = -1;
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            dt.DefaultView.Sort = e.SortExpression;
            GridView1.DataSource = dt.DefaultView.ToTable();
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
                    else
                    {
                        lblStatus.Text = "Active";
                    }
                }

                //==========================================================================               
                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + ID + "');";             

                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());

            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int id = Convert.ToInt32(GridView1.DataKeys[index]["Id"].ToString());

            if (e.CommandName == "Up")
            {
                int previous_id = Convert.ToInt32(GridView1.DataKeys[index - 1]["Id"].ToString());
                int option = 1;
                banner_obj.UpdateSortKey(option, id, previous_id);
            }
            if (e.CommandName == "Down")
            {
                int previous_id = Convert.ToInt32(GridView1.DataKeys[index + 1]["Id"].ToString());
                int option = 0;
                banner_obj.UpdateSortKey(option, id, previous_id);
            }
            FillDataInGrid();

            //int index = Convert.ToInt32(e.CommandArgument);
            //GridViewRow gvrow = GridView1.Rows[index];
            //GridViewRow previousRow;

            //if (e.CommandName == "Up")
            //{      
            //    previousRow = GridView1.Rows[index - 1];
            //    int current_sortkey = Convert.ToInt32(GridView1.DataKeys[gvrow.RowIndex].Value.ToString());
            //    int id = Convert.ToInt32(gvrow.Cells[0].Text);
            //    int previous_id = Convert.ToInt32(previousRow.Cells[0].Text);                
            //    int option=1;
            //    banner_obj.UpdateSortKey(option, id, previous_id, current_sortkey);
            //}
            //if (e.CommandName == "Down")
            //{                
            //    previousRow = GridView1.Rows[index + 1];
            //    int current_sortkey = Convert.ToInt32(GridView1.DataKeys[gvrow.RowIndex].Value.ToString());
            //    int id = Convert.ToInt32(gvrow.Cells[0].Text);
            //    int previous_id = Convert.ToInt32(previousRow.Cells[0].Text);
            //    int option = 1;
            //    banner_obj.UpdateSortKey(option, id, previous_id, current_sortkey);                       
            //}
            //FillDataInGrid();
        }
        #endregion ==============================================================================================

        protected void btnReload_Click(object sender, EventArgs e)
        {
            FillDataInGrid();
        }

        protected void btnLock_Click(object sender, EventArgs e)
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
                    int i = banner_obj.UpdateStatus(id, status);
                    FillDataInGrid();
                }
                catch (InvalidOperationException ef) { ef.ToString(); }
            }            
        }

        protected void btnUnLock_Click(object sender, EventArgs e)
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
                    int i = banner_obj.UpdateStatus(id, status);
                    FillDataInGrid();
                }
                catch (InvalidOperationException ef) { ef.ToString(); }
            }
        }

        protected void btnMultipleDelete_Click(object sender, EventArgs e)
        {
            //Get Ids
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');
            string front_banner_dir = Server.MapPath(thumb_banner_dir_path);
            string main_banner_dir = Server.MapPath(main_banner_dir_path);
            //Code for deleting items
            foreach (string Item in IDs)
            {
                try
                {
                    int id = int.Parse(Item.ToString());
                    int i = banner_obj.Delete(id, front_banner_dir, main_banner_dir);
                    FillDataInGrid();
                }
                catch (InvalidOperationException ef) { ef.ToString(); }
            }
        }
    }
}