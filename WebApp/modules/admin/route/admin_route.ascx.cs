using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Common;
using System.Data;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Url.FriendlyUrl;
using CommonLibrary.Data;
using CommonLibrary.Entities.Content;

namespace WebApp.modules.admin.route
{
    public partial class admin_route : System.Web.UI.UserControl
    {
        protected static string add_url = "/modules/admin/route/admin_route_edit.aspx?mode=add";
        protected static string edit_url = "/modules/admin/route/admin_route_edit.aspx?mode=edit&idx=";
        protected static string loading_url = "/loading.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadStatus2DDL();
                PopulatePortalList2DDL();
                PopulateContentItemList();
                PopulateCulture2DDL();               
                FillDataInGrid();
            }
        }

        #region Portal List ===============================================================
        private void PopulatePortalList2DDL()
        {
            PortalController portal_obj = new PortalController();
            DataTable dtNodes = portal_obj.GetList();

            ddlPortalList.Items.Clear();
            ddlPortalList.DataSource = dtNodes;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            ddlPortalList.AutoPostBack = true;
            //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));           
            if (Session["PortalId"] == null && Session["PortalId"].ToString() == string.Empty)
            {
                ddlPortalList.SelectedIndex = 0;
                ddlPortalList.Enabled = true;
            }
            else
            {
                ddlPortalList.SelectedValue = Session["PortalId"].ToString();
                ddlPortalList.Enabled = false;
            }
            ddlPortalList.AutoPostBack = true;
        }
        protected void ddlPortalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateCulture2DDL();  
        }
        #endregion ====================================================================

        #region ContentItems ===========================================================
        private void PopulateContentItemList()
        {
            ContentController content_controller_obj = new ContentController();
            int ContentTypeId = 1;
            DataTable dt = content_controller_obj.GetListByContentTypeId(ContentTypeId);

            ddlContentItem.Items.Clear();
            ddlContentItem.DataSource = dt;
            ddlContentItem.DataTextField = "ContentKey";
            ddlContentItem.DataValueField = "ContentItemID";
            ddlContentItem.DataBind();
            ddlContentItem.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlContentItem.SelectedIndex = 0;
            ddlContentItem.AutoPostBack = true;
        }
        protected void ddlContentItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion =======================================================================

        #region Culture ======================================================
        protected void PopulateCulture2DDL()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            CultureClass culture_obj = new CultureClass();
            DataTable dt = culture_obj.GetListByPortalId(PortalId);

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

        #region status ==================================================
        protected void LoadStatus2DDL()
        {
            //Load list item to dropdownlist
            ListItemCollection lstColl = new ListItemCollection();
            lstColl.Add(new ListItem("Active", "True"));
            lstColl.Add(new ListItem("InActive", "False"));
            

            ddlStatus.DataSource = lstColl;
            ddlStatus.DataTextField = "Text";
            ddlStatus.DataValueField = "Value";
            ddlStatus.DataBind();
            ddlStatus.SelectedIndex = 0;
            ddlStatus.AutoPostBack = true;
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ======================================================
        
        private void FillDataInGrid()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            int ContentItemId = Convert.ToInt32(ddlContentItem.SelectedValue);
            string CultureCode = ddlCultureList.SelectedValue;
            bool Discountinue = Convert.ToBoolean(ddlStatus.SelectedValue);
            List<aspnet_Routes> lst = RouteController.GetListByPortalIdCultureCodeStatus(PortalId,ContentItemId, CultureCode, Discountinue);
            GridView1.DataSource = lst;
            GridView1.DataBind();
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
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

        protected void btnMultipleDelete_Click(object sender, EventArgs e)
        {
            //try
            //{
                //Get Ids
                string[] IDs = hdnFldSelectedValues.Value.Trim().Split('|');

                //Code for deleting items
                foreach (string Item in IDs)
                {
                    int id = Convert.ToInt32(Item.ToString());
                    RouteController.Delete(id);

                    int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
                    int ContentItemId = Convert.ToInt32(ddlContentItem.SelectedValue);
                    string CultureCode = ddlCultureList.SelectedValue;
                    bool Discontinued = true;

                    string HomeDirectory = Session["HomeDirectory"].ToString();

                    string RouterPath = Server.MapPath("~/routers");

                    int result = RouteController.WriteFileRouter(PortalId,ContentItemId, CultureCode, Discontinued, HomeDirectory, RouterPath, "app_routers.cs");
                }
                Response.Redirect(Request.RawUrl);
            //}
            //catch (FormatException ex)
            //{
            //    ex.ToString();
            //}
        }

      

    }
}