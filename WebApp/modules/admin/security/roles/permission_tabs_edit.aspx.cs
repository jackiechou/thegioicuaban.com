using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Users;
using CommonLibrary.Security.Roles;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Portal;

namespace WebApp.modules.admin.security.roles
{
    public partial class permission_tabs_edit : System.Web.UI.Page
    {
        TabPermissionController tab_permission_obj = new TabPermissionController();
        DataTable dt = new DataTable();      

        public void Page_PreInit(Object sender, EventArgs e)
        {
            Page.Title = "5EAGLES";
            Page.Theme = "default";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
                FillDataInGrid();
                MultiView1.ActiveViewIndex = 0;
            }
            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
        }

        private void LoadData()
        {
            string ApplicationId = string.Empty, RoleId = string.Empty;
            if (Request.QueryString["app_id"] != null && Request.QueryString["app_id"] != string.Empty)
            {
                ApplicationId = Request.QueryString["app_id"].ToString();
                LoadApplicationList2DDL(ApplicationId);                
            }
            if (Request.QueryString["role_id"] != null && Request.QueryString["role_id"] != string.Empty)
            {
                RoleId = Request.QueryString["role_id"].ToString();
                LoadRoleList2DDL(RoleId); 
            }
            LoadPortalionList2DDL();
            LoadPermissionList2DDL();
        }

        #region Portal List ================================================================
        private void LoadPortalionList2DDL()
        {
            ddlPortalList.Items.Clear();
            string ApplicationId = ddlApplicationList.SelectedValue;
            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetListByApplicationId(ApplicationId); //select all the nodes from DB
            ddlPortalList.DataSource = dt;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            ddlPortalList.AutoPostBack = true;
            if(Session["PortalId"] !=null && Session["PortalId"].ToString() != string.Empty)
                ddlPortalList.SelectedValue = Session["PortalId"].ToString();
            else
                ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));    
        }
        private void LoadPortalionList2DDL(string selected_value)
        {
            ddlPortalList.Items.Clear();

            PortalController portal_obj = new PortalController();
            DataTable dt = portal_obj.GetList(); //select all the nodes from DB
            ddlPortalList.DataSource = dt;
            ddlPortalList.DataTextField = "PortalName";
            ddlPortalList.DataValueField = "PortalId";
            ddlPortalList.DataBind();
            ddlPortalList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPortalList.SelectedValue = selected_value;
            ddlPortalList.Enabled = false;
        }
        #endregion

        #region Application =========================================================
        private void LoadApplicationList2DDL(string selected_value)
        {
            ddlApplicationList.Items.Clear();

            CommonLibrary.Application.ApplicationController app_obj = new CommonLibrary.Application.ApplicationController();
            DataTable dt_app = app_obj.GetApps();
            ddlApplicationList.DataSource = dt_app;
            ddlApplicationList.DataTextField = "ApplicationName";
            ddlApplicationList.DataValueField = "ApplicationId";
            ddlApplicationList.DataBind();
            ddlApplicationList.SelectedValue = selected_value;
            ddlApplicationList.Enabled = false;
        }
        #endregion ==================================================================

        #region Role =========================================================
        private void LoadRoleList2DDL(string selected_value)
        {
            ddlRoleList.Items.Clear();
            string ApplicationId = ddlApplicationList.SelectedValue;
            RoleController app_obj = new RoleController();
            DataTable dt_app = app_obj.GetRoleListByApplicationId(ApplicationId);
            ddlRoleList.DataSource = dt_app;
            ddlRoleList.DataTextField = "RoleName";
            ddlRoleList.DataValueField = "RoleId";
            ddlRoleList.DataBind();            
            ddlRoleList.SelectedValue = selected_value;
            ddlRoleList.Enabled = false;
        }
        #endregion ==================================================================
       
        #region Permission =========================================================
        private void LoadPermissionList2DDL()
        {
            ddlPermissionList.Items.Clear();
            PermissionController permission_obj = new PermissionController();
            DataTable dt_permission = permission_obj.GetPermissionsByCode("SYSTEM_TAB");
            ddlPermissionList.DataSource = dt_permission;
            ddlPermissionList.DataTextField = "PermissionName";
            ddlPermissionList.DataValueField = "PermissionId";
            ddlPermissionList.DataBind();
            // ddlPermissionList.Items.Insert(0, new ListItem("- Chọn -", "0"));
            ddlPermissionList.SelectedIndex = 0;
            ddlPermissionList.AutoPostBack = true;
        }
        protected void ddlPermissionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataInGrid();
        }
        #endregion ==================================================================

        //private DataTable PopulateSelectedRights(int PermissionId)
        //{
        //    string RoleId = string.Empty;
        //    if (Request.QueryString["role_id"] != null && Request.QueryString["role_id"].ToString() != string.Empty)
        //    {
        //        RoleId = Request.QueryString["role_id"].ToString();
        //    }
        //    TabPermissionController tab_permission_obj = new TabPermissionController();
        //    DataTable dt = tab_permission_obj.GetListByRoleIdPermissionId(RoleId, PermissionId);
        //    return dt;
        //}

        private void FillDataInGrid()
        {
            string PortalId = ddlPortalList.SelectedValue;
            int isadmin = Convert.ToInt32(Session["IsSuperUser"].ToString());
            if (PortalId != string.Empty)
            {
                TabController tab_obj = new TabController();
                dt = tab_obj.GetListByPortalId(PortalId, isadmin);

                if (dt.Rows.Count > 0)
                {
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                    //GridView1.HeaderRow.Attributes["style"] = "display:block";
                    //GridView1.UseAccessibleHeader = true;
                    //GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
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
        }        

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationCheck");
            if (Page.IsValid)
            {
                System.Threading.Thread.Sleep(2000);
                int i = SaveAllSelectedRights();
                if (i == -1)
                {
                    lblErrorMsg.Text = "Thông tin không đầy đủ";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 0;
                }
                else if (i == -2)
                {
                    lblErrorMsg.Text = "Tiến trình xử lý bị lỗi";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 2;
                }
                else if (i == -3)
                {
                    lblErrorMsg.Text = "Dữ liệu đã tồn tại";
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 1;
                }
                else if (i == 1)
                {
                    lblResult.Text = "Cập nhật thành công";
                    MultiView1.ActiveViewIndex = 1;
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
                    MultiView1.ActiveViewIndex = 2;
                }
            }    
        }

        private int SaveAllSelectedRights()
        {
            int i = 0, result = 0;
            i = DeleteOldData();
            //switch (i)
            //{
            //    case -3:
            //        lblErrorMsg.Text = "Không tồn tại Id này";
            //        ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
            //        MultiView1.ActiveViewIndex = 2;
            //        break; 
            //    case -2:
            //        lblErrorMsg.Text = "Tiến trình xử lý bị lỗi";
            //        ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
            //        MultiView1.ActiveViewIndex = 2;
            //        break; 
            //    case 1:
                    string UserId = null;
                    int TabId = 0, PermissionId = 0, AllowAccess = 0;
                    string RoleId = ddlRoleList.SelectedValue;
                    string strPermissionId = ddlPermissionList.SelectedValue;

                    if (strPermissionId != string.Empty)
                        PermissionId = Convert.ToInt32(strPermissionId);

                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        // Selected Permission Right ===============================================================================================
                        bool resultSelectedPermissionRight = ((CheckBox)row.FindControl("chkSelectedPermissionRight")).Checked;
                        if (resultSelectedPermissionRight)
                            TabId = (int)GridView1.DataKeys[row.RowIndex].Value;

                        //UserId ======================================================================================================                
                        DropDownList ddlUserList = (DropDownList)row.FindControl("ddlUserList");
                        if (ddlUserList != null)
                            UserId = ddlUserList.SelectedValue;
                        else
                            UserId = Session["UserId"].ToString();

                        // AllowAccess Right ========================================================================================
                        CheckBox chkSelectedAllowAccessRight = (CheckBox)row.FindControl("chkSelectedAllowAccessRight");
                        if (chkSelectedAllowAccessRight != null)
                        {
                            bool resultAllowAccessRight = chkSelectedAllowAccessRight.Checked;
                            if (resultAllowAccessRight)
                            {
                                AllowAccess = 1;
                                //==============================================================================================================
                                result = tab_permission_obj.AddTabPermission(TabId, PermissionId, RoleId, AllowAccess, UserId);
                                //==============================================================================================================
                            }
                            else
                                AllowAccess = 0;
                        }
                    }
            //        break;

            //}
            return result;
        }

        private int DeleteOldData()
        {
            string RoleId = ddlRoleList.SelectedValue;                             
            int PermissionId = Convert.ToInt32(ddlPermissionList.SelectedValue);                          
            TabPermissionController tab_permission_obj = new TabPermissionController();
            DataTable dt = tab_permission_obj.GetListByRoleIdPermissionId(RoleId, PermissionId);
            int TabPermissionId = -1, result=-1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TabPermissionId = Convert.ToInt32(dt.Rows[i]["TabPermissionId"].ToString());
                result = tab_permission_obj.DeleteTabPermission(TabPermissionId);
            }
            return result;
        }

        private void RememberSelectedPermissionRights()
        {
            ArrayList _list = new ArrayList();
            int index = -1;
            foreach (GridViewRow row in GridView1.Rows)
            {
                index = (int)GridView1.DataKeys[row.RowIndex].Value;
                //var id = Convert.ToInt32(GridView1.DataKeys[row.RowIndex]["ID"]);
                CheckBox chkSelectedViewRight = (CheckBox)row.FindControl("chkSelectedPermissionRight");
                if (chkSelectedViewRight != null)
                {
                    bool result = ((CheckBox)row.FindControl("chkSelectedPermissionRight")).Checked;

                    // Check in the Session
                    if (Session["CHECKED_PERMISSION_RIGHT_ITEMS"] != null)
                        _list = (ArrayList)Session["CHECKED_PERMISSION_RIGHT_ITEMS"];

                    if (result)
                    {
                        if (!_list.Contains(index))
                            _list.Add(index);
                    }
                    else
                        _list.Remove(index);
                }
            }
            if (_list != null && _list.Count > 0)
                Session["CHECKED_PERMISSION_RIGHT_ITEMS"] = _list;
        }

        private void RememberSelectedAllowAccessRights()
        {
            ArrayList _list = new ArrayList();
            int index = -1;
            foreach (GridViewRow row in GridView1.Rows)
            {
                index = (int)GridView1.DataKeys[row.RowIndex].Value;
                CheckBox chkSelectedkAllowAccessRight = (CheckBox)row.FindControl("chkSelectedkAllowAccessRight");
                if (chkSelectedkAllowAccessRight != null)
                {
                    bool result = ((CheckBox)row.FindControl("chkSelectedkAllowAccessRight")).Checked;

                    // Check in the Session
                    if (Session["CHECKED_ALLOW_ACCESS_RIGHT_ITEMS"] != null)
                        _list = (ArrayList)Session["CHECKED_ALLOW_ACCESS_RIGHT_ITEMS"];

                    if (result)
                    {
                        if (!_list.Contains(index))
                            _list.Add(index);
                    }
                    else
                        _list.Remove(index);
                }
            }
            if (_list != null && _list.Count > 0)
                Session["CHECKED_ALLOW_ACCESS_RIGHT_ITEMS"] = _list;
        }

        private void RememberOldValues()
        {
            RememberSelectedPermissionRights();
            RememberSelectedAllowAccessRights();
        }

        private void RePopulateSelectedPermissionRights()
        {
            ArrayList _list = (ArrayList)Session["CHECKED_PERMISSION_RIGHT_ITEMS"];
            if (_list != null && _list.Count > 0)
            {
                foreach (GridViewRow row in GridView1.Rows)
                {
                    int index = (int)GridView1.DataKeys[row.RowIndex].Value;
                    if (_list.Contains(index))
                    {
                        CheckBox myCheckBox = (CheckBox)row.FindControl("chkSelectedPermissionRight");
                        myCheckBox.Checked = true;
                    }
                }
            }
        }

        private void RePopulateSelectedAllowAccessRights()
        {
            ArrayList _list = (ArrayList)Session["CHECKED_ALLOW_ACCESS_RIGHT_ITEMS"];
            if (_list != null && _list.Count > 0)
            {
                foreach (GridViewRow row in GridView1.Rows)
                {
                    int index = (int)GridView1.DataKeys[row.RowIndex].Value;
                    if (_list.Contains(index))
                    {
                        CheckBox myCheckBox = (CheckBox)row.FindControl("chkSelectedkAllowAccessRight");
                        myCheckBox.Checked = true;
                    }
                }
            }
        }

        private void RePopulateValues()
        {
            RePopulateSelectedPermissionRights();
            RePopulateSelectedAllowAccessRights();
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            RePopulateValues();
        }       

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                RememberOldValues();
                GridView1.PageIndex = e.NewPageIndex;
                FillDataInGrid();
                RePopulateValues();
            }
            catch (ArgumentOutOfRangeException ex) { ex.ToString(); }
        }

        protected void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gvrPager = GridView1.BottomPagerRow;
            DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlPages");
            GridView1.PageIndex = ddlPages.SelectedIndex;
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

            Label lblPagingInformation =
                (Label)gvrPager.Cells[0].FindControl("PagingInformation");
            lblPagingInformation.Text = string.Format("You are viewing page {0} of {1}...", GridView1.PageIndex + 1, GridView1.PageCount);
                        
            if (ddlPages != null)
            {
                // populate pager
                for (int i = 0; i < GridView1.PageCount; i++)
                {
                    int intPageNumber = i + 1;
                    ListItem lstItem = new ListItem(intPageNumber.ToString());

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
            FillDataInGrid();
            //GridView1.DataBind();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            FillDataInGrid();
        }
               
        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            GridView1.PageIndex = e.NewSelectedIndex;
            GridView1.DataBind();
        }

        protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            // If we just deleted the last row in the GridView, decrement the PageIndex
            if (GridView1.Rows.Count == 1)
                // we just deleted the last row
                GridView1.PageIndex = Math.Max(0, GridView1.PageIndex - 1);
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            dt.DefaultView.Sort = e.SortExpression;
            GridView1.DataSource = dt.DefaultView.ToTable();
            GridView1.DataBind();
        }

        private void PopulateUserList(DropDownList ddlUserList, string selected_value)
        {
            string ApplicationId = ddlApplicationList.SelectedValue;
            UserController user_obj = new UserController();
            DataTable dt_app = user_obj.GetUsers(ApplicationId);            
            ddlUserList.Items.Clear();
            ddlUserList.DataSource = dt_app;
            ddlUserList.DataTextField = "UserName";
            ddlUserList.DataValueField = "UserId";
            ddlUserList.DataBind();
            ddlUserList.Items.Insert(0, new ListItem("- Chọn -", ""));
            if (selected_value != string.Empty)
                ddlUserList.SelectedValue = selected_value;
            else
                ddlUserList.SelectedIndex = 0;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string TabId = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();   
                string RoleId = ddlRoleList.SelectedValue;                             
                int PermissionId = Convert.ToInt32(ddlPermissionList.SelectedValue);
                string SelectedTabId = null, UserId=null;                
                TabPermissionController tab_permission_obj = new TabPermissionController();
                DataTable dt = tab_permission_obj.GetListByRoleIdPermissionId(RoleId, PermissionId);                           

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SelectedTabId = dt.Rows[i]["TabId"].ToString();
                    if(dt.Rows[i]["UserId"].ToString() != string.Empty)
                        UserId = dt.Rows[i]["UserId"].ToString();     
                    else
                    {
                        if (Session["UserId"] != null && Session["UserId"].ToString() != string.Empty)
                            UserId = Session["UserId"].ToString();
                    }

                    if (TabId == SelectedTabId)
                    {
                        CheckBox chkSelectedPermissionRight = (CheckBox)e.Row.Cells[2].FindControl("chkSelectedPermissionRight");
                        if (chkSelectedPermissionRight != null)
                            chkSelectedPermissionRight.Checked = true;

                        CheckBox chkSelectedAllowAccessRight = (CheckBox)e.Row.Cells[3].FindControl("chkSelectedAllowAccessRight");
                        if (chkSelectedAllowAccessRight != null)
                            chkSelectedAllowAccessRight.Checked = true;

                        //TextBox txtUserId = (TextBox)e.Row.Cells[4].FindControl("txtUserId");
                        //txtUserId.Text = UserId;
                        //txtUserId.Attributes.Add("value", txtUserId.Text);

                        DropDownList ddlUserList = (DropDownList)e.Row.FindControl("ddlUserList");
                        PopulateUserList(ddlUserList, UserId);
                    }
                }               
            }                   
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //ArrayList selectedItems;
            //if (Session["selectedItems"] == null)          
            //    selectedItems = new ArrayList();           
            //else          
            //    selectedItems = (ArrayList)Session["selectedItems"];
          

            //if (e.CommandName == "SaveSelectedViewRights")
            //{        
            //    foreach (GridViewRow row in GridView1.Rows)
            //    {
            //        string index = GridView1.DataKeys[row.RowIndex]["TabId"].ToString();               
            //        selectedItems.Add(index);
            //        Session["selectedItems"] = selectedItems;
            //}
            
            //if (e.CommandName == "RemoveSelectedViewRights")
            //{        
            //    foreach (GridViewRow row in GridView1.Rows)
            //    {
            //        string index = GridView1.DataKeys[row.RowIndex]["TabId"].ToString();               
            //        selectedItems.Remove(index);
            //        Session["selectedItems"] = selectedItems;
            //}
        }

    
    }
}