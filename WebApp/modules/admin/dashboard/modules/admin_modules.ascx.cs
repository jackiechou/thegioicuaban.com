using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Data;
using CommonLibrary.Application;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.dashboard.modules
{
    public partial class admin_modules : System.Web.UI.UserControl
    {
        protected static string add_url = "/modules/admin/dashboard/modules/admin_modules_add.aspx";
        protected static string edit_url = "/modules/admin/dashboard/modules/admin_modules_edit.aspx?mode=edit&idx=";
        protected static string loading_url = "/loading.aspx";

        protected static string module_controller_url = "/modules/admin/dashboard/modules/admin_module_controller.aspx?idx=";
        protected static string module_controller_add_url ="/modules/admin/dashboard/modules/admin_module_controller_add.aspx?idx=";

        Modules module_obj = new Modules();
        DataTable dt = new DataTable();       

        protected void Page_Load(object sender, EventArgs e)
        {
            //Insure that the __doPostBack() JavaScript method is created...
            Page.ClientScript.GetPostBackEventReference(this, string.Empty);

            if (!Page.IsPostBack)
            {
                LoadApplicationList2DDL();
                PopulatePortal2DDL();
                FillDataInGrid();
            }
        }

        #region tree node list - Application List =======================================
        private void LoadApplicationList2DDL()
        {
            ddlApplicationList.Items.Clear();

            ApplicationController application_obj = new ApplicationController();
            DataTable dtNodes = application_obj.GetApps();
            ddlApplicationList.DataSource = dtNodes;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
            //ddlApplicationList.Items.Insert(0, new ListItem("- Chọn -", "")); //DROPDOWNLIST
            ddlApplicationList.SelectedIndex = 0;
            ddlApplicationList.AutoPostBack = true;
        }

        protected void ddlApplicationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ===================================================================

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

        private void FillDataInGrid()
        {
            string PortalId = ddlPortalList.SelectedValue;
            Modules module_obj = new Modules();
            DataTable dt = module_obj.GetModuleListByPortalId(PortalId);          

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
            //int userid = int.Parse(Session["UserId"].ToString());
            //int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0].ToString());
            //int i = content_list_obj.Delete(userid, id);
            //FillDataInGrid();
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
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
                {
                    // Display the primary key value of the selected row.
                    // string ID = GridView1.DataKeys[e.Row.DataItemIndex].Values[0].ToString();
                    //string url = "Default.aspx?page=articles/admin_article_categories_edit&id=" + ID;
                    //e.Row.Attributes.Add("onDblclick", "location.href='" + url + "';");                   

                    // Add javascript to highlight row
                    // e.Row.Attributes["onmouseover"] = "ChangeBackColor(this, true, '#BAD5E8'); this.style.cursor = 'hand';this.style.textDecoration='underline';this.style.color='red'";
                    // e.Row.Attributes["onmouseout"] = "ChangeBackColor(this, false, '');this.style.cursor='hand';this.style.textDecoration='none';this.style.color='black';";
                    //e.Row.Attributes.Add("onclick", "this.style.background='#DFEEF3';");
                    //e.Row.Attributes["onclick"] = "StartSingleClick();";
                    //e.Row.Attributes["ondblclick"] = "ShowEditModal(\"" + e.Row.RowIndex + "\");";
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
            //Paging =========================================================
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
                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                //ImageButton btnShowModuleControllers = (ImageButton)e.Row.FindControl("btnShowModuleControllers");
                //if (btnShowModuleControllers != null)
                //{
                //    btnShowModuleControllers.Attributes.Add("onclick", "ShowModuleControllerModal('" + ID + "');");
                //}

                e.Row.Attributes.Add("onclick", "ChangeRowColor(this);");
                e.Row.Attributes.Add("OnDblClick", "ShowEditModal(" + ID + ");");  //ok


                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());

            }
        }

        protected void btnMultipleDelete_Click(object sender, EventArgs e)
        {
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            foreach (string Item in IDs)
            {               
                int id = int.Parse(Item.ToString());
                int i = module_obj.Delete(id);
            }
            FillDataInGrid();
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
    }
}