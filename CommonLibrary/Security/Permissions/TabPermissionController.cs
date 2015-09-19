using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Services.Log.EventLog;
using System.Data;
using System.Data.SqlClient;

namespace CommonLibrary.Security.Permissions
{
    public class TabPermissionController
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        private static PermissionProvider provider = PermissionProvider.Instance();
        private static void ClearPermissionCache(int tabId)
        {
            TabController objTabs = new TabController();
            TabInfo objTab = objTabs.GetTab(tabId, Null.NullInteger, false);
            DataCache.ClearTabPermissionsCache(objTab.PortalID);
        }
        public static bool CanAddContentToPage()
        {
            return CanAddContentToPage(TabController.CurrentPage);
        }
        public static bool CanAddContentToPage(TabInfo objTab)
        {
            return provider.CanAddContentToPage(objTab) || CanAdminPage(objTab);
        }
        public static bool CanAddPage()
        {
            return CanAddPage(TabController.CurrentPage);
        }
        public static bool CanAddPage(TabInfo objTab)
        {
            return provider.CanAddPage(objTab) || CanAdminPage(objTab);
        }
        public static bool CanAdminPage()
        {
            return CanAdminPage(TabController.CurrentPage);
        }
        public static bool CanAdminPage(TabInfo objTab)
        {
            return provider.CanAdminPage(objTab);
        }
        public static bool CanCopyPage()
        {
            return CanCopyPage(TabController.CurrentPage);
        }
        public static bool CanCopyPage(TabInfo objTab)
        {
            return provider.CanCopyPage(objTab) || CanAdminPage(objTab);
        }
        public static bool CanDeletePage()
        {
            return CanDeletePage(TabController.CurrentPage);
        }
        public static bool CanDeletePage(TabInfo objTab)
        {
            return provider.CanDeletePage(objTab) || CanAdminPage(objTab);
        }
        public static bool CanExportPage()
        {
            return CanExportPage(TabController.CurrentPage);
        }
        public static bool CanExportPage(TabInfo objTab)
        {
            return provider.CanExportPage(objTab) || CanAdminPage(objTab);
        }
        public static bool CanImportPage()
        {
            return CanImportPage(TabController.CurrentPage);
        }
        public static bool CanImportPage(TabInfo objTab)
        {
            return provider.CanImportPage(objTab) || CanAdminPage(objTab);
        }
        public static bool CanManagePage()
        {
            return CanManagePage(TabController.CurrentPage);
        }
        public static bool CanManagePage(TabInfo objTab)
        {
            return provider.CanManagePage(objTab) || CanAdminPage(objTab);
        }
        public static bool CanNavigateToPage()
        {
            return CanNavigateToPage(TabController.CurrentPage);
        }
        public static bool CanNavigateToPage(TabInfo objTab)
        {
            return provider.CanNavigateToPage(objTab) || CanAdminPage(objTab);
        }
        public static bool CanViewPage()
        {
            return CanViewPage(TabController.CurrentPage);
        }
        public static bool CanViewPage(TabInfo objTab)
        {
            return provider.CanViewPage(objTab) || CanAdminPage(objTab);
        }
        public static void DeleteTabPermissionsByUser(UserInfo objUser)
        {
            provider.DeleteTabPermissionsByUser(objUser);
            CommonLibrary.Services.Log.EventLog.EventLogController objEventLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABPERMISSION_DELETED);
            DataCache.ClearTabPermissionsCache(objUser.PortalID);
        }
        public static TabPermissionCollection GetTabPermissions(int tabID, int portalID)
        {
            return provider.GetTabPermissions(tabID, portalID);
        }
        public static bool HasTabPermission(string permissionKey)
        {
            return HasTabPermission(PortalController.GetCurrentPortalSettings().ActiveTab.TabPermissions, permissionKey);
        }
        public static bool HasTabPermission(TabPermissionCollection objTabPermissions, string permissionKey)
        {
            bool hasPermission = provider.HasTabPermission(objTabPermissions, "EDIT");
            if (!hasPermission)
            {
                if (permissionKey.Contains(","))
                {
                    foreach (string permission in permissionKey.Split(','))
                    {
                        if (provider.HasTabPermission(objTabPermissions, permission))
                        {
                            hasPermission = true;
                            break;
                        }
                    }
                }
                else
                {
                    hasPermission = provider.HasTabPermission(objTabPermissions, permissionKey);
                }
            }
            return hasPermission;
        }
        public static void SaveTabPermissions(TabInfo objTab)
        {
            provider.SaveTabPermissions(objTab);
            EventLogController objEventLog = new EventLogController();
            objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABPERMISSION_UPDATED);
            ClearPermissionCache(objTab.TabID);
        }

        #region Methods =========================================================================================
        public DataTable GetListByRoleId(string RoleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabPermission_GetListByRoleId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByRoleIdPermissionId(string RoleId, int PermissionId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabPermission_GetListByRoleIdPermissionId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int AddTabPermission(int TabId, int PermissionId, string RoleId, int AllowAccess, string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabPermission_AddTabPermission", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@AllowAccess", AllowAccess);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, int.MaxValue) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateTabPermission(int TabPermissionId, int TabId, int PermissionId, string RoleId, int AllowAccess, string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabPermission_UpdateTabPermission", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabPermissionId", TabId);
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@AllowAccess", AllowAccess);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, int.MaxValue) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = Convert.ToInt32(cmd.Parameters["@o_return"].Value);
            con.Close();
            return retunvalue;
        }

        public int DeleteTabPermission(int TabPermissionId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabPermission_DeleteTabPermission", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabPermissionId", TabPermissionId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = Convert.ToInt32(cmd.Parameters["@o_return"].Value);
            con.Close();
            return retunvalue;
        }

        public int DeleteTabPermissionsByTabId(int TabId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabPermission_DeleteTabPermissionsByTabId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = Convert.ToInt32(cmd.Parameters["@o_return"].Value);
            con.Close();
            return retunvalue;
        }

        public int DeleteTabPermissionsByUserId(string ApplicationId, string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabPermission_DeleteTabPermissionsByUserId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = Convert.ToInt32(cmd.Parameters["@o_return"].Value);
            con.Close();
            return retunvalue;
        }  
        #endregion ==============================================================================================
    }
}
