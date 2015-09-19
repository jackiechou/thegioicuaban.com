using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MediaLibrary;
using System.IO;
using CommonLibrary.Modules;


namespace WebApp.modules.admin.media
{
    public partial class admin_video : System.Web.UI.UserControl
    {
        protected static string add_url = "/modules/admin/media/admin_video_add.aspx?mode=add";
        protected static string edit_url = "/modules/admin/media/admin_video_edit.aspx?mode=edit&idx=";
        protected static string loading_url = "/loading.aspx";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadStatus2DDL();
                PopulateMediaTypeList2DDL();
                PopulateMediaTopicList2DDL();
                FillDataInGrid();
            }
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            FillDataInGrid();
        }

        private void FillDataInGrid()
        {            
            string vendorid = Session["VendorId"].ToString();
            string topicid = ddlMediaTopicList.SelectedValue;
            string typeid = ddlMediaTypeList.SelectedValue;
            string status = ddlStatus.SelectedValue;

            MediaFiles media_obj = new MediaFiles();
            DataTable dt = media_obj.GetListByVendorIdTopicIdTypeIdStatus(vendorid, topicid, typeid, status);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        #region Media Types ======================================================
        protected void PopulateMediaTypeList2DDL()
        {
            MediaTypes media_type_obj = new MediaTypes();
            List<Media_Types> lst = media_type_obj.GetListByStatus("1");

            ddlMediaTypeList.Items.Clear();
            ddlMediaTypeList.DataSource = lst;
            ddlMediaTypeList.DataTextField = "TypeName";
            ddlMediaTypeList.DataValueField = "TypeId";
            ddlMediaTypeList.DataBind();
            ddlMediaTypeList.SelectedValue = "2";
            ddlMediaTypeList.Enabled = false;
            ddlMediaTypeList.AutoPostBack = true;
        }
        protected void ddlMediaTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ============================================================

        #region Media Topics ======================================================
        protected void PopulateMediaTopicList2DDL()
        {
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTopics media_topic_obj = new MediaTopics();
            List<Media_Topics> lst = media_topic_obj.GetListByTypeStatus(TypeId, "1");

            ddlMediaTopicList.Items.Clear();
            ddlMediaTopicList.DataSource = lst;
            ddlMediaTopicList.DataTextField = "Name";
            ddlMediaTopicList.DataValueField = "TopicId";
            ddlMediaTopicList.DataBind();
            ddlMediaTopicList.AutoPostBack = true;
        }
        protected void ddlMediaTopicList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ============================================================

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
            FillDataInGrid();
        }
        #endregion ======================================================

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
                        lblStatus.Text = "UnPublished";
                    }
                    else if (lblStatus.Text == "1")
                    {
                        lblStatus.Text = "Published";
                    }
                }

                ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
                if (btnDelete != null)
                {
                    btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record?')");
                }

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
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0].ToString());
            string userid = Session["UserId"].ToString();
            MediaFiles media_obj = new MediaFiles();
            DataTable dt = media_obj.GetDetailById(id);
            if (dt.Rows.Count > 0)
            {
                string filename = dt.Rows[0]["FileName"].ToString();
                int topicid = Convert.ToInt32(dt.Rows[0]["TopicId"].ToString());
                string photo = dt.Rows[0]["Photo"].ToString();
                string thumbnail = dt.Rows[0]["Thumbnail"].ToString();
                int i = media_obj.Delete(userid, id, photo, thumbnail, filename);
                if (i == 1)
                {
                    DeleteMediaFile(filename, topicid);
                }
            }
            FillDataInGrid();
        }

        private void DeleteMediaFile(string file_name, int topicid)
        {
            int TypeId = Convert.ToInt32(ddlMediaTypeList.SelectedValue);
            MediaTypes media_type_obj = new MediaTypes();
            Media_Types type_obj = media_type_obj.GetDetails(TypeId);
            string TypePath = type_obj.TypePath;
            string dir_path = Server.MapPath("~" + TypePath + "/" + topicid);
            if (Directory.Exists(dir_path))
            {
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_handle_obj.deleteFile(file_name, dir_path);
            }
        }
    }
}