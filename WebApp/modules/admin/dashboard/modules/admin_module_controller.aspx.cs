using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Entities.Modules;

namespace WebApp.modules.admin.dashboard.modules
{
    public partial class admin_module_controller : System.Web.UI.Page
    {
        Modules module_obj = new Modules();
        ModuleController module_controller_obj = new ModuleController();
        DataTable dt = new DataTable();

        private int _idx
        {
            get
            {
                if (ViewState["idx"] == null)
                    ViewState["idx"] = -1;
                return (int)ViewState["idx"];
            }
            set
            {
                ViewState["idx"] = value;
            }
        }


        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int ModuleID = Convert.ToInt32(Request.QueryString["idx"]);
                Literal_ModuleName.Text = module_obj.GetModuleTitleByModuleId(ModuleID);

                FillDataInGrid();
            }   
        }

        //protected void btnAdd_Click(object sender, EventArgs e)
        //{
        //    int ModuleID = Convert.ToInt32(Request.QueryString["idx"]);
        //    btnAdd.Attributes.Add("OnClick", "ShowAddModal(" + ModuleID + ");return false;");
        //}
      
        private void FillDataInGrid()
        {
            int ModuleID = Convert.ToInt32(Request.QueryString["idx"]);           

            DataTable dt = module_controller_obj.GetListByModuleId(ModuleID);

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
                //if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
                //{
                //    // Add javascript to highlight row
                //    e.Row.Attributes.Add("onclick", "ChangeRowColor(this);");           
                //}
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

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Get the index of the row being changed
            int row_index = int.Parse(e.CommandArgument.ToString());
            int id = (int)GridView1.DataKeys[row_index].Value;
            string _commandName = e.CommandName;
            if (_commandName == "Delete")
            {
                int i = module_controller_obj.Delete(id);
                FillDataInGrid();
            }
            //switch (_commandName)
            //{
            //    case "Delete":
            //        {
            //            int i = module_controller_obj.Delete(id);
            //            FillDataInGrid();
            //            break;
            //        }
            //    case "Edit":
            //        {
            //            Response.Redirect("~/pages/administrators/index.aspx?type=1&page=banner_positions/admin_banner_position_edit&id=" + id);
            //            break;
            //        }
            //    case ("SingleClick"):
            //        {
            //            GridView1.SelectedIndex = selectedIndex;
            //            object send = new object();
            //            GridViewEditEventArgs ex = new GridViewEditEventArgs(selectedIndex);
            //            GridView1_RowEditing(send, ex);
            //            break;
            //        }
            //    default:
            //        break;             
            //}
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //getting the data using the Colum Name, and storing in a variable
                string ModuleID = ((DataRowView)e.Row.DataItem)["ModuleID"].ToString();;
                ImageButton btnAdd = (ImageButton)e.Row.FindControl("btnAdd");
                if (btnAdd != null)
                {
                    btnAdd.Attributes.Add("onclick", "ShowAddModal(" + ModuleID + ")");
                }

                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                e.Row.Attributes.Add("OnDblClick", "ShowEditModal(" + ID + ");");

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
    }
}