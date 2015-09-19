using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Modules;
using System.Data;
using MediaLibrary;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.media
{
    public partial class admin_background_audio : System.Web.UI.UserControl
    {
        protected static string add_url = "/modules/admin/media/admin_background_audio_edit.aspx?mode=add";
        protected static string edit_url = "/modules/admin/media/admin_background_audio_edit.aspx?mode=edit&idx=";
        protected static string loading_url = "/loading.aspx";
        protected static string select_page_url = "/modules/admin/media/admin_background_audio_select.aspx";

        ModuleClass modules_obj = new ModuleClass();
        MediaFiles media_obj = new MediaFiles();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PopulatePortalList2DDL();
                PopulateMediaTypeList2DDL();
                FillDataInGrid();
            }
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            FillDataInGrid();
        }

        protected void btnSoundOff_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] != null && Session["UserId"].ToString() != string.Empty)
            {
                string userid = Session["UserId"].ToString();
                media_obj.UpdateAllStatus(userid, "0");
                FillDataInGrid();
            }
        }

        protected void btnMultipleRowDelete_Click(object sender, EventArgs e)
        {       
            string upload_background_audio_dir = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["upload_background_audio_dir"]);
            //Get Ids
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            //Code for deleting items
            foreach (string Item in IDs)
            {              
                string userid = Session["UserId"].ToString();
                int id = int.Parse(Item.ToString());
                //int i = media_obj.Delete(userid, id, upload_background_audio_dir);                             
            }
            FillDataInGrid();  
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

        #region Media Types ======================================================
        protected void PopulateMediaTypeList2DDL()
        {
            //MediaTypes media_type_obj = new MediaTypes();            
            //DataTable dt = media_type_obj.GetList();

            //ddlMediaTypeList.Items.Clear();
            //ddlMediaTypeList.DataSource = dt;
            //ddlMediaTypeList.DataTextField = "TypeName";
            //ddlMediaTypeList.DataValueField = "TypeId";
            //ddlMediaTypeList.DataBind();
            //ddlMediaTypeList.SelectedIndex = 0;
            //ddlMediaTypeList.AutoPostBack = true;
        }
        protected void ddlMediaTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ============================================================  

        private void FillDataInGrid()
        {            
            //int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            //int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            //DataTable dt = media_obj.GetListByPortalIdTypeId(PortalId, TypeId);

            //if (dt.Rows.Count > 0)
            //{
            //    GridView1.DataSource = dt;
            //    GridView1.DataBind();

            //}
            //else
            //{
            //    dt.Rows.Add(dt.NewRow());
            //    GridView1.DataSource = dt;
            //    GridView1.DataBind();

            //    int TotalColumns = GridView1.Rows[0].Cells.Count;
            //    GridView1.Rows[0].Cells.Clear();
            //    GridView1.Rows[0].Cells.Add(new TableCell());
            //    GridView1.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            //    GridView1.Rows[0].Cells[0].Text = "No Record Found";
            //}            
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
        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
            //    {
            //        e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#FFFFE1';");
            //        e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#f7fff8';");
            //        e.Row.Attributes.Add("onclick", "this.style.backgroundColor='#cc6600';");
            //    }
            //}
        }
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = new DataTable();
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

                e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';this.style.color='#9C1E00'";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#454545';";
                e.Row.Attributes["onmousedown"] = "this.style.background='#FACC2E';";
                //e.Row.Attributes["onmouseclick"] = "this.style.background='##228b22';this.style.color='#FFFFFF';";


                //HyperLink Hyperlink_Edit = (HyperLink)e.Row.FindControl("Hyperlink_Edit");
                //if (Hyperlink_Edit != null)
                //{
                //    Hyperlink_Edit.NavigateUrl = "~/Pages/Administrators/Index.aspx?type=1&page=audio/admin_background_audio_edit.ascx&id=" + DataBinder.Eval(e.Row.DataItem, "Id");
                //}

                ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
                if (btnDelete != null)
                {
                    //btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record " + DataBinder.Eval(e.Row.DataItem, "CountryID") + "')");
                    btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record?')");
                }

                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());

            }
        }
       
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //try
            //{
            // Get the index of the row being changed
            int row_index = Convert.ToInt32((string)e.CommandArgument);
            int id = (int)GridView1.DataKeys[row_index].Value;
            string userid = Session["UserId"].ToString();

            //if (e.CommandName == "Edit")
            //{
            //    Response.Redirect("~/Pages/Administrators/Index.aspx?page=audio/admin_background_audio_edit.ascx&id=" + id);
            //}

            if (e.CommandName == "UpdateSortKey")
            {
                TextBox txtSortKey = (TextBox)GridView1.Rows[row_index].FindControl("txtSortKey");
                if (txtSortKey != null)
                {
                    int sortkey = Convert.ToInt32(txtSortKey.Text);
                    media_obj.UpdateSortKey(userid, id, sortkey);
                    Response.Redirect("~/Pages/Administrators/Index.aspx?page=audio/admin_background_audio.ascx");
                }
            }

            if (e.CommandName == "UpdateStatus")
            {
                Control control = GridView1.Rows[row_index].Cells[3].Controls[0];
                if (((ButtonField)GridView1.Columns[3]).ButtonType == ButtonType.Image)
                {
                    ImageButton btnUpdateStatus = control as ImageButton;
                    Label lblStatus = (Label)GridView1.Rows[row_index].FindControl("lblStatus");
                    string new_status = string.Empty;

                    if (lblStatus != null)
                    {
                        string status = lblStatus.Text;
                        int result = -1;
                        if (status == "Active")
                        {
                            new_status = "0";

                            result = media_obj.UpdateStatus(userid, id, new_status);
                            btnUpdateStatus.ImageUrl = "~/images/icons/unlock.gif";
                        }
                        if (status == "InActive")
                        {
                            new_status = "1";
                            media_obj.UpdateStatus(userid, id, new_status);
                            btnUpdateStatus.ImageUrl = "~/images/icons/lock.gif";
                        }


                        switch (result)
                        {
                            case -1:
                                Response.Write("<script>alert('Thông tin không đầy đủ.');window.location.href='Index.aspx?type=1&page=audio/admin_background_audio.ascx';</script>");
                                Response.End();
                                break;
                            case -2:
                                Response.Write("<script>alert('Cập nhật không thành công');window.location.href='Index.aspx?type=1&page=audio/admin_background_audio.ascx';</script>");
                                Response.End();
                                break;
                            case -3:
                                Response.Write("<script>alert('Không tồn tại id này');window.location.href='Index.aspx?type=1&page=audio/admin_background_audio.ascx';</script>");
                                Response.End();
                                break;
                            case 1:
                                Response.Write("<script>alert('Cập nhật thành công');window.location.href='Index.aspx?type=1&page=audio/admin_background_audio.ascx';</script>");
                                Response.End();
                                break;
                            default:
                                Response.Write("<script>alert('Lỗi hệ thống');window.location.href='Index.aspx?type=1&page=audio/admin_background_audio.ascx';</script>");
                                Response.End();
                                break;
                        }
                    }
                }
            }
        }
    }
}