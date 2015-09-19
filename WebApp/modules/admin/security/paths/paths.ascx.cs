using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Application;

namespace WebApp.modules.admin.security
{
    public partial class paths : System.Web.UI.UserControl
    {
        private static string view_ctrl = "admin_paths.aspx";
        private static string add_ctrl = "admin_paths_add.aspx";
        private static string edit_ctrl = "admin_paths_edit.aspx";
        private static string root_path = "~/pages/administrators/";
        private static string login_path = root_path + "login.aspx";

        PathClass path_obj = new PathClass();
        DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ShowApp();
                FillDataInGrid();
            }
        }

        #region Application =======================================
        private void ShowApp()
        {
            ddlApp.Items.Clear(); //DROPDOWNLIST        

            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_app = app_obj.GetApps(); //select all the nodes from DB
            ddlApp.AutoPostBack = true;
            ddlApp.DataSource = dt_app;
            ddlApp.DataTextField = "ApplicationName";
            ddlApp.DataValueField = "ApplicationId";
            ddlApp.DataBind();
            ddlApp.Items.Insert(0, new ListItem("-Chọn ứng dụng-", " ")); //DROPDOWNLIST
            ddlApp.SelectedIndex = 0;
        }
        protected void ddlApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion

        private void FillDataInGrid()
        {
            //try
            //{
            string app_id = ddlApp.SelectedValue;
            dt = path_obj.GetDetails(app_id);

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

            //}catch (Exception ex) { ex.ToString(); }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());
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
                }
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GridView1.DataBind();
        }


        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.location.href='" + add_ctrl + "';</script>");
            Response.End();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            //Get Ids
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');
            if (IDs.Length <= 0)
            {
                Response.Write("<script>alert('Vui lòng check chọn 1 dòng');window.location.href='" + view_ctrl + "';</script>");
                Response.End();

            }
            if (IDs.Length == 1)
            {
                Response.Write("<script>window.location.href='" + edit_ctrl + "?id=" + hdnFldSelectedValues.Value.Trim() + "';</script>");
                Response.End();
            }
            else
            {
                Response.Write("<script>alert('Vui lòng chỉ chọn 1 dòng');window.location.href='" + view_ctrl + "';</script>");
                Response.End();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //Get Ids
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            //Code for deleting items
            foreach (string Item in IDs)
            {
                try
                {
                    string id = Item.ToString();
                    int i = path_obj.Delete(id);
                    FillDataInGrid();
                }
                catch (InvalidOperationException ef) { ef.ToString(); }
            }
        }
    }
}