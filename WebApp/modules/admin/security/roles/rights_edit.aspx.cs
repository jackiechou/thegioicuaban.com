using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Security.Permissions;
using System.Data;
using CommonLibrary.Modules.Dashboard.Components.Modules;
using System.Collections;
using CommonLibrary.Entities.Tabs;
namespace WebApp.modules.admin.security.roles
{
    public partial class rights_edit : System.Web.UI.Page
    {
        PermissionController permission_obj = new PermissionController();
        DataTable dt = new DataTable();

        public UIMode.mode _mode
        {
            get
            {
                if (ViewState["mode"] == null)
                    ViewState["mode"] = new UIMode.mode();
                return (UIMode.mode)ViewState["mode"];
            }
            set
            {
                ViewState["mode"] = value;
            }
        }

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
                //string qsuimode = Request.QueryString["mode"];

                //if (string.IsNullOrEmpty(qsuimode) == false)
                //{
                //    _mode = (UIMode.mode)Enum.Parse(typeof(UIMode.mode), qsuimode);
                //    if (_mode == UIMode.mode.add)
                //    {
                        loadPermissionToChkBoxListPermission();
                        ShowMenuSections();
                //    }
                //    if (_mode == UIMode.mode.edit)
                //    {
                //        _idx = Convert.ToInt32(Request.QueryString["idx"]);
                //        LoadData();
                //        hdnWindowUIMODE.Value = _mode.ToString();
                //    }
                        TreeView1.Attributes.Add("onclick", "OnCheckBoxCheckChanged(event)");
                //}
                MultiView1.ActiveViewIndex = 0;

            }

            PostBackOptions optionsSubmit = new PostBackOptions(btnOkay);
            btnOkay.OnClientClick = "disableButtonOnClick(this, 'Please wait...', 'disabled_button'); ";
            btnOkay.OnClientClick += ClientScript.GetPostBackEventReference(optionsSubmit);
            //btnOkay.OnClientClick = String.Format("this.disabled=true; __doPostBack('{0}','');", btnOkay.UniqueID);

        }
        #region load permission ======================================================
        protected void loadPermissionToChkBoxListPermission()
        {
            PermissionController permission_obj = new PermissionController();
            DataTable dt = permission_obj.GetPermissionsByCode("SYSTEM_TAB");

            CheckBoxList_Permission.DataSource = dt;
            CheckBoxList_Permission.DataTextField = "PermissionName";
            CheckBoxList_Permission.DataValueField = "PermissionId";
            CheckBoxList_Permission.DataBind();

            //ArrayList array_data = new ArrayList();
            //array_data.Add(new ListItem("View", "V"));
            //array_data.Add(new ListItem("Insert", "I"));
            //array_data.Add(new ListItem("Update", "U"));
            //array_data.Add(new ListItem("Security", "S"));
            //array_data.Add(new ListItem("History", "H"));

            //CheckBoxList_Permission.DataSource = array_data;
            //CheckBoxList_Permission.DataTextField = "Text";
            //CheckBoxList_Permission.DataValueField = "Value";
            //CheckBoxList_Permission.DataBind();
        }
        protected void loadPermissionToChkBoxListPermission(string permissions)
        {
            PermissionController permission_obj = new PermissionController();
            DataTable dt = permission_obj.GetPermissionsByCode("SYSTEM_TAB");

            CheckBoxList_Permission.DataSource = dt;
            CheckBoxList_Permission.DataTextField = "PermissionName";
            CheckBoxList_Permission.DataValueField = "PermissionId";
            CheckBoxList_Permission.DataBind();

            string[] arr_result = permissions.Split(new string[] { "," }, StringSplitOptions.None);
            for (int i = 0; i < CheckBoxList_Permission.Items.Count; i++)
            {
                for (int x = 0; x < arr_result.Length; x++)
                {
                    if (CheckBoxList_Permission.Items[i].Value == arr_result[x])
                    {
                        CheckBoxList_Permission.Items[i].Selected = true;
                    }
                }
            }           
        }
        #endregion ===================================================================

        #region tree node list - Menu Sections =======================================
        private void ShowMenuSections()
        {
            TreeNodeBinding tnb = new TreeNodeBinding();
            tnb.DataMember = "System.Data.DataRowView";
            tnb.TextField = "TabName";
            tnb.ValueField = "TabId";
            tnb.PopulateOnDemand = false;
            tnb.SelectAction = TreeNodeSelectAction.Select;
            TreeView1.DataBindings.Add(tnb);

            TabController tab_obj = new TabController();
            DataSet dataSet = tab_obj.GetActiveList();
            // You can use this:
            TreeView1.DataSource = new HierarchicalDataSet(dataSet, "TabId", "ParentId");

            // OR you can use the extensions for TreeView if you are using .NET 3.5
            //TreeView1.SetDataSourceFromDataSet(dataSet, "Idx", "ParentId");

            // OR This line, will load the tree starting from the parent record of value = 1
            //TreeView1.DataSource = new HierarchicalDataSet(dataSet, "Idx", "ParentId", 1);
            TreeView1.DataBind();
            TreeView1.CollapseAll();

        }
        private void loadMenuSections(int role_id)
        {
            TreeNodeBinding tnb = new TreeNodeBinding();
            tnb.DataMember = "System.Data.DataRowView";
            tnb.TextField = "TabName";
            tnb.ValueField = "TabId";
            tnb.PopulateOnDemand = false;
            tnb.SelectAction = TreeNodeSelectAction.Select;
            TreeView1.DataBindings.Add(tnb);

            TabController tab_obj = new TabController();
            DataSet dataSet = tab_obj.GetActiveList();
            // You can use this:
            TreeView1.DataSource = new HierarchicalDataSet(dataSet, "TabId", "ParentId");

            // OR you can use the extensions for TreeView if you are using .NET 3.5
            //TreeView1.SetDataSourceFromDataSet(dataSet, "Idx", "ParentId");

            // OR This line, will load the tree starting from the parent record of value = 1
            //TreeView1.DataSource = new HierarchicalDataSet(dataSet, "Idx", "ParentId", 1);
            TreeView1.DataBind();
            TreeView1.CollapseAll();

            //string menustring = tab_obj.GetMenuStringByRole(role_id);
            //string[] arr_result = menustring.Split(new string[] { "," }, StringSplitOptions.None);

            //for (int index = 0; index < arr_result.Length - 1; index++)
            //{
            //    //if(TreeView1.Nodes.Count>0){
            //    //    foreach (TreeNode nodes in TreeView1.Nodes)
            //    //    {
            //    //        if (arr_result[index].ToString() == nodes.Value)
            //    //        {
            //    //            nodes.Checked = true;
            //    //            nodes.Expand();

            //    //            if (nodes.ChildNodes.Count > 0)
            //    //            {
            //    //                foreach (TreeNode child_nodes in nodes.ChildNodes)
            //    //                {
            //    //                    if (arr_result[index].ToString() == child_nodes.Value)
            //    //                    {
            //    //                        child_nodes.Checked = true;
            //    //                        child_nodes.Expand();
            //    //                    }
            //    //                }
            //    //            }                
            //    //        }                   
            //    //    }
            //    //}           

            //    TreeNode nodes = SelectNode(arr_result[index].ToString(), TreeView1.Nodes);
            //    if (nodes != null)
            //    {
            //        nodes.Checked = true;
            //        nodes.Expand();
            //    }
            //}
        }
        //=======================================================================
        TreeNode node;
        private TreeNode SelectNode(string node_value, TreeNodeCollection parentCollection)
        {
            foreach (TreeNode childnode in parentCollection)
            { //iterate through the treeview nnode 
                if (childnode.Value == node_value)
                {
                    node = childnode;
                }
                else if (childnode.ChildNodes.Count > 0)
                {
                    // check for child item(level 2)
                    node = GetNode(node_value, childnode.ChildNodes);
                } //if Match found return node

                if ((node != null)) break;
            }
            return node;
        }

        public static TreeNode GetNode(string node_value, TreeNodeCollection nodeCollection)
        {
            // for all root nodes
            foreach (TreeNode childNode in nodeCollection)
            {
                // if child node matches
                if (childNode.Value.Equals(node_value))
                {
                    return childNode;
                }
                else
                {
                    // recursive for grandchilds
                    TreeNode result = GetNode(node_value, childNode.ChildNodes);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
        #endregion ==============================================================  

        private void LoadData()
        {
            DataTable dt = permission_obj.GetDetails(_idx);
            string RoleId = Request.QueryString["role_id"];

            //string status = dt.Rows[0]["AllowAccess"].ToString();
            //if (status == "1")
            //{
            //    chkAllowAccess.Checked = true;
            //}
            //else
            //{
            //    chkAllowAccess.Checked = false;
            //}
            //string permissions = dt.Rows[0]["PermissionGroup"].ToString();
            //loadPermissionToChkBoxListPermission(permissions);
            //loadMenuSections(role_id);           
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            //try
            //{
            System.Threading.Thread.Sleep(1000);
            if (_mode == UIMode.mode.add)
            {
                AddData();
            }
            else if (_mode == UIMode.mode.edit)
            {
                UpdateData();
            }
            MultiView1.ActiveViewIndex = 1;
            ClientScript.RegisterStartupScript(this.GetType(), "onload", "onSuccess();", true);

            //}
            //catch
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "onload", "onError();", true);
            //    MultiView1.ActiveViewIndex = 1;
            //}
        }

        private void AddData()
        {
            //string PermissionCode = txtPermissionCode.Text;
            //string PermissionKey = txtPermissionKey.Text;
            //string PermissionName = txtPermissionName.Text;
            //permission_obj.Insert(PermissionCode, PermissionKey, PermissionName);

            #region Get Menu Sections from LisBox ===============================
            //string selectedgroup = string.Empty;
            //if (lstSelectedGroup.Items.Count > 0)
            //{
            //    foreach (ListItem listItem in lstSelectedGroup.Items)
            //    {
            //        selectedgroup += "," + listItem.Value;
            //    }
            //    if (selectedgroup != "")
            //    {
            //        try
            //        {
            //            selectedgroup = selectedgroup.Remove(0, 1);
            //            Response.Write(selectedgroup);
            //        }
            //        catch (ArgumentOutOfRangeException ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //        }
            //    }
            //}
            //else
            //{
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "alertMsg", "<script>alert('Vui lòng chọn nhóm menu.');</script>", false);
            //}
            #endregion =========================================================

            #region Get Permission Values from CheckBoxList ============================
            string permissions = string.Empty;
            for (int index = 0; index < CheckBoxList_Permission.Items.Count; index++)
            {
                if (CheckBoxList_Permission.Items[index].Selected)
                {
                    permissions += "," + CheckBoxList_Permission.Items[index].Value;
                }
            }

            if (permissions != "0")
            {
                try
                {
                    permissions = permissions.Remove(0, 1);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Response.Write("<script>alert('Vui lòng chọn');window.location.href='Default.aspx?page=admin_users_add.ascx&type=2';</script>");
                Response.End();
            }
            #endregion ==================================================================

            #region Get Menu Sections - Selected Values from CheckBox in TreeView =======================
            //string menu_list = string.Empty;
            //foreach (TreeNode item in TreeView1.CheckedNodes)
            //{
            //    menu_list += "," + item.Value;
            //}
            //if (menu_list != string.Empty)
            //{
            //    try
            //    {
            //        menu_list = menu_list.Remove(0, 1);
            //    }
            //    catch (ArgumentOutOfRangeException ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}
            //else
            //{
            //    Response.Write("<script>alert('Vui lòng chọn nhóm menu');window.location.href='Index.aspx?page=admin_users_add.ascx&type=1';</script>");
            //    Response.End();
            //}
            #endregion ====================================================================

            string RoleId = ddlRoleList.SelectedValue;
            //string role_name = txtRoleName.Text;
            //string description = txtDescription.Text;
            //bool bStatus = ChkBoxNewStatus.Checked;
            //int status = 0;
            //if (bStatus == true)
            //{
            //    status = 1;
            //}
            //else
            //{
            //    status = 0;
            //}


            //int i = roles_obj.Insert(role_name, description, permissions, menu_list, status);
            //if (i == 1)
            //{
            //    Response.Write("<script>alert('Insert thành công');window.location.href='Index.aspx?type=1&page=accounts/admin_roles';</script>");
            //    Response.End();
            //}
            //else
            //{
            //    Response.Write("<script>alert('Insert không thành công');window.location.href='Index.aspx?type=1&page=accounts/admin_roles_add';</script>");
            //    Response.End();
            //}
        }

        private void UpdateData()
        {
            //string PermissionCode = txtPermissionCode.Text;
            //string PermissionKey = txtPermissionKey.Text;
            //string PermissionName = txtPermissionName.Text;
            //permission_obj.Update(_idx, PermissionCode, PermissionKey, PermissionName);

            #region Get Permission Values from CheckBoxList ==============================================
            string permissions = string.Empty;
            for (int index = 0; index < CheckBoxList_Permission.Items.Count; index++)
            {
                if (CheckBoxList_Permission.Items[index].Selected)
                {
                    permissions += "," + CheckBoxList_Permission.Items[index].Value;
                }
            }

            if (permissions != "0")
            {
                try
                {
                    permissions = permissions.Remove(0, 1);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Response.Write("<script>alert('Vui lòng chọn');window.location.href='Default.aspx?page=admin_roles_edit.ascx&type=1';</script>");
                Response.End();
            }
            #endregion ===================================================================================

            #region Get Selected Values from CheckBox in TreeView ==============================================
            string menu_list = string.Empty;
            foreach (TreeNode item in TreeView1.CheckedNodes)
            {
                menu_list += "," + item.Value;
            }
            if (menu_list != string.Empty)
            {
                try
                {
                    menu_list = menu_list.Remove(0, 1);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Response.Write("<script>alert('Vui lòng chọn nhóm menu');window.location.href='Default.aspx?page=admin_users_add.ascx&type=2';</script>");
                Response.End();
            }
            #endregion ===================================================================================
            string RoleId = ddlRoleList.SelectedValue;
            //int userid = Convert.ToInt32(Session["UserID"].ToString());
            //int id = Convert.ToInt32(Request.QueryString["id"]);
            //string role_name = txtRoleName.Text;
            //string description = txtDescription.Text;
            //bool bStatus = ChkBoxStatus.Checked;
            //int status = 0;
            //if (bStatus == true)
            //{
            //    status = 1;
            //}
            //else
            //{
            //    status = 0;
            //}

            //int i = roles_obj.Update(userid, id, role_name, description, permissions, menu_list, status);

            //if (i == -2)
            //{
            //    Response.Write("<script>alert('Cập nhật không thành công');window.history.back();</script>");
            //    Response.End();
            //}
            //else if (i == 1)
            //{
            //    Response.Write("<script>alert('Cập nhật thành công');window.location.href='Index.aspx?type=1&page=accounts/admin_roles';</script>");
            //    Response.End();
            //}
            //else
            //{
            //    Response.Write("<script>alert('Lỗi hệ thống');window.history.back();</script>");
            //    Response.End();
            //}
        
        }
    }
}