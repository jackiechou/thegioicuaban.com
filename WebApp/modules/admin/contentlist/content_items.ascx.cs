using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Entities.Content;
using System.Drawing;


namespace WebApp.modules.admin.contentlist
{
    public partial class content_items : System.Web.UI.UserControl
    {
        ContentController content_items_obj = new ContentController();
        DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            //Insure that the __doPostBack() JavaScript method is created...
            Page.ClientScript.GetPostBackEventReference(this, string.Empty);

            if (!Page.IsPostBack)
            {
                LoadContentTypeList2DDL();
                FillDataInGrid();
            }
        }

        #region ContentType =======================================================================
        private void LoadContentTypeList2DDL()
        {
            ContentType content_types_obj = new ContentType();
            DataTable dt_content_types = content_types_obj.GetAll(); //select all the nodes from DB

            ddlContentTypes.Items.Clear();
            ddlContentTypes.DataSource = dt_content_types;
            ddlContentTypes.DataTextField = "ContentType";
            ddlContentTypes.DataValueField = "ContentTypeID";
            ddlContentTypes.DataBind();
            //ddlContentTypes.Items.Insert(0, new ListItem("- Chọn -", ""));
            ddlContentTypes.SelectedIndex = 0;
            ddlContentTypes.AutoPostBack = true;

        }

        protected void ddlContentTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ContentType =======================================================================

        private void FillDataInGrid()
        {
            int ContentTypeID = Convert.ToInt32(ddlContentTypes.SelectedValue);
            DataTable dt = content_items_obj.GetListByContentTypeId(ContentTypeID);

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
                    // Add javascript to highlight row
                    e.Row.Attributes.Add("onclick", "ChangeRowColor(this);");
                }
            }
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            //dt.DefaultView.Sort = e.SortExpression;
            //GridView1.DataSource = dt.DefaultView.ToTable();
            //GridView1.DataBind();
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

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //string _commandName = e.CommandName;
            //switch (_commandName)
            //{
            //    case ("Add"):
            //        {
            //            int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
            //            GridView1.SelectedIndex = rowIndex;
            //            //object send = new object();
            //            //GridViewEditEventArgs ex = new GridViewEditEventArgs(selectedIndex);
            //            //GridView1_RowEditing(send, ex);
            //            break;
            //        }
            //}
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //gridViewRow.Parent.Parent or (GridView) gridViewRow.NamingContainer
            GridViewRow gridViewRow = e.Row;
            if (gridViewRow.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < GridView1.Columns.Count; i++)
                {
                    string sortExpression = GridView1.Columns[i].SortExpression;
                    TableCell tableCell = gridViewRow.Cells[i];

                    //Make sure the column we are working with has a sort expression
                    if (!string.IsNullOrEmpty(sortExpression))
                    {
                        System.Web.UI.WebControls.Image sortDirectionImageControl;

                        //Create an instance of a Image WebControl
                        sortDirectionImageControl = new System.Web.UI.WebControls.Image();

                        //Determine the image url based on the SortDirection
                        string imageUrl = "~/images/icons/sort_neutral.gif";
                        if (sortExpression == GridView1.SortExpression)
                        {
                            imageUrl = (GridView1.SortDirection == SortDirection.Ascending) ?
                                "~/images/icons/sort_asc.gif" : "~/images/icons/sort_desc.gif";
                        }

                        //Add the Image Web Control to the cell
                        sortDirectionImageControl.ImageUrl = imageUrl;
                        sortDirectionImageControl.Style.Add(HtmlTextWriterStyle.MarginLeft, "10px");
                        tableCell.Wrap = false;
                        tableCell.Controls.Add(sortDirectionImageControl);

                        //Enumerate the controls within the current cell and find the link button.
                        foreach (Control gridViewRowCellControl in gridViewRow.Cells[i].Controls)
                        {
                            LinkButton linkButton = gridViewRowCellControl as LinkButton;

                            if ((linkButton != null) && (linkButton.CommandName == "Sort"))
                            {
                                //Add an onclick attribute to the current cell
                                tableCell.Attributes.Add("onclick", "RequestData('" + linkButton.ClientID + "', this, event)");
                                tableCell.Style.Add(HtmlTextWriterStyle.Cursor, "hand");
                                tableCell.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                                break;
                            }
                        }
                    }
                }
            }


            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Enuerate all the rows and change the color of the column that is being sorted               
                for (int i = 0; i < gridViewRow.Cells.Count; i++)
                {
                    if ((!String.IsNullOrEmpty(GridView1.SortExpression)) &&
                        (GridView1.Columns[i].SortExpression == GridView1.SortExpression))
                    {
                        Color sortColumnBgColor;
                        sortColumnBgColor = (gridViewRow.RowState != DataControlRowState.Alternate) ?
                            ColorTranslator.FromHtml("#d7e3f1") : ColorTranslator.FromHtml("#f7fbff");
                        gridViewRow.Cells[i].BackColor = sortColumnBgColor;
                    }
                }
               //===============================================================================

                Label lblContentTypeID = (Label)e.Row.FindControl("lblContentTypeID");
                if (lblContentTypeID != null)
                {
                    if (lblContentTypeID.Text == "1")
                    {
                        lblContentTypeID.Text = "TAB";
                    }
                    else if (lblContentTypeID.Text == "2")
                    {
                        lblContentTypeID.Text = "MODULE";
                    }
                    else
                    {
                        lblContentTypeID.Text = "ERROR";
                    }
                }
                

                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                e.Row.Attributes.Add("OnDblClick", "ShowEditModal('" + ID + "');");  //ok

                //e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';";
                //e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';";
                //e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(this.GridView1, "Select$" + e.Row.RowIndex);
                
                //CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                //CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                //HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                //chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());


                
            }
        }

        protected void btnMultipleDelete_Click(object sender, EventArgs e)
        {
            string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

            //Code for deleting items
            foreach (string Item in IDs)
            {                
                int id = int.Parse(Item.ToString());
                int i = content_items_obj.Delete(id);              
            }
            FillDataInGrid();
        }

       
    }
}