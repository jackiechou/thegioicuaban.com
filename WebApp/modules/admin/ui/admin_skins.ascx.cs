using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using CommonLibrary.UI.Skins;
using CommonLibrary.Application;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.ui
{
    public partial class admin_skins : System.Web.UI.UserControl
    {
        Skin skin_obj = new Skin();
        DataTable dt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadApplicationList2DDL();
                PopulatePortalList2DDL();
                LoadSkinTypeList2DDL();
                LoadPackageList2DDL();
                FillDataInGrid();
            }   
        }

        #region Application List =================================================================
        private void LoadApplicationList2DDL()
        {
            ApplicationController obj_data = new ApplicationController();
            DataTable dtNodes = obj_data.GetApps();

            ddlApplicationList.Items.Clear();
            ddlApplicationList.DataSource = dtNodes;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
            ddlApplicationList.Items.Insert(0, new ListItem("- Chọn -", ""));
            ddlApplicationList.SelectedIndex = 0;
        }
        protected void ddlApplicationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulatePortalList2DDL();
            FillDataInGrid();
        }
        #endregion =====================================================================

        #region Portal List ==================================================
        private void PopulatePortalList2DDL()
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            if (!string.IsNullOrEmpty(ApplicationId))
            {
                PortalController portal_obj = new PortalController();
                DataTable dtNodes = portal_obj.GetListByApplicationId(ApplicationId);

                ddlPortalList.Items.Clear();
                ddlPortalList.DataSource = dtNodes;
                ddlPortalList.DataTextField = "PortalName";
                ddlPortalList.DataValueField = "PortalId";
                ddlPortalList.DataBind();
                //ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
                ddlPortalList.SelectedIndex = 0;
                ddlPortalList.AutoPostBack = true;
            }
        }
        protected void ddlPortalList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ===============================================================

        #region Skin Type List =================================================================
        //public static void BindToEnum(Type enumType, ListControl lc)
        //{
        //    // get the names from the enumeration
        //    string[] names = Enum.GetNames(enumType);
        //    // get the values from the enumeration
        //    Array values = Enum.GetValues(enumType);
        //    // turn it into a hash table
        //    Hashtable ht = new Hashtable();
        //    for (int i = 0; i < names.Length; i++)
        //        // note the cast to integer here is important
        //        // otherwise we'll just get the enum string back again
        //        ht.Add(names[i], (int)values.GetValue(i));
        //    // return the dictionary to be bound to
        //    lc.DataSource = ht;
        //    lc.DataTextField = "Key";
        //    lc.DataValueField = "Value";
        //    lc.DataBind();
        //}
        //BindToEnum(typeof(NewsType), DropDownList1);
        //BindToEnum(typeof(NewsType), CheckBoxList1);
        //BindToEnum(typeof(NewsType), RadoButtonList1);

        protected void LoadSkinTypeList2DDL()
        {
            Hashtable ht = GetEnumForBind(typeof(SkinType));
            ddlSkinTypeList.DataSource = ht;
            ddlSkinTypeList.DataTextField = "value";
            ddlSkinTypeList.DataValueField = "key";
            ddlSkinTypeList.DataBind();
            ddlSkinTypeList.SelectedIndex = 0;
            ddlSkinTypeList.AutoPostBack = true;
        }
        public Hashtable GetEnumForBind(Type enumeration)
        {
            string[] names = Enum.GetNames(enumeration);
            Array values = Enum.GetValues(enumeration);
            Hashtable ht = new Hashtable();
            for (int i = 0; i < names.Length; i++)
            {
                ht.Add(Convert.ToInt32(values.GetValue(i)).ToString(), names[i]);
            }
            return ht;
        }

        protected void ddlSkinTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion =====================================================================

        #region PackageList =================================================================
        private void LoadPackageList2DDL()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            string SkinType = ddlSkinTypeList.SelectedValue;
            SkinPackages obj_data = new SkinPackages();
            DataTable dtNodes = obj_data.GetListByPortalIdSkinType(PortalId, SkinType);

            ddlPackageList.Items.Clear();
            ddlPackageList.DataSource = dtNodes;
            ddlPackageList.DataTextField = "SkinName";
            ddlPackageList.DataValueField = "SkinPackage_ID";
            ddlPackageList.DataBind();
            ddlPackageList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPackageList.SelectedIndex = 0;
        }
        protected void ddlPackageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion =====================================================================

       
        private void FillDataInGrid()
        {
            int PortalId = Convert.ToInt32(ddlPortalList.SelectedValue);
            int SkinPackage_ID = Convert.ToInt32(ddlPackageList.SelectedValue);
            DataTable dt = skin_obj.GetListByPortalIdSkinPackageId(PortalId, SkinPackage_ID);

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
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowState == DataControlRowState.Alternate)
                {
                    e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#FFFFE1';");
                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#f7fff8';");
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

                string ID = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
                e.Row.Attributes["ondblclick"] = "this.style.background='#BFFF00';ShowEditModal('" + ID + "');";

                e.Row.Attributes["onmouseover"] = "this.style.cursor='hand';this.style.textDecoration='underline';this.style.color='#9C1E00'";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.color='#454545';";
                e.Row.Attributes["onmousedown"] = "this.style.background='#FACC2E';";
                //e.Row.Attributes["onmouseclick"] = "this.style.background='##228b22';this.style.color='#FFFFFF';";

                //ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
                //if (btnDelete != null)
                //{
                //    //btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record " + DataBinder.Eval(e.Row.DataItem, "CountryID") + "')");
                //    btnDelete.Attributes.Add("onclick", "javascript:return " + "confirm('Are you sure you want to delete this record?')");
                //}

                CheckBox chkBxSelect = (CheckBox)e.Row.Cells[0].FindControl("chkBxSelect");
                CheckBox chkBxHeader = (CheckBox)this.GridView1.HeaderRow.FindControl("chkBxHeader");
                HiddenField hdnFldId = (HiddenField)e.Row.Cells[0].FindControl("hdnFldId");

                chkBxSelect.Attributes["onclick"] = string.Format("javascript:ChildClick(this,document.getElementById('{0}'),'{1}');", chkBxHeader.ClientID, hdnFldId.Value.Trim());

            }
        }

      
        protected void btnReload_Click(object sender, EventArgs e)
        {
            FillDataInGrid();
        }             
    }
}