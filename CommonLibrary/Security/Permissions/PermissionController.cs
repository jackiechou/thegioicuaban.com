using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Data;
using CommonLibrary.Services.Log.EventLog;
using System.Data;
using System.Data.SqlClient;

namespace CommonLibrary.Security.Permissions
{
    public class PermissionController
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        private static DataProvider provider = DataProvider.Instance();
        public int AddPermission(PermissionInfo objPermission)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objPermission, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PERMISSION_CREATED);
            return Convert.ToInt32(provider.AddPermission(objPermission.PermissionCode, objPermission.ModuleDefID, objPermission.PermissionKey, objPermission.PermissionName, UserController.GetCurrentUserInfo().UserID));
        }
        public void DeletePermission(int permissionID)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("PermissionID", permissionID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PERMISSION_DELETED);
            provider.DeletePermission(permissionID);
        }
        public PermissionInfo GetPermission(int permissionID)
        {
            return CBO.FillObject<PermissionInfo>(provider.GetPermission(permissionID));
        }
        public ArrayList GetPermissionByCodeAndKey(string permissionCode, string permissionKey)
        {
            return CBO.FillCollection(provider.GetPermissionByCodeAndKey(permissionCode, permissionKey), typeof(PermissionInfo));
        }
        public ArrayList GetPermissionsByModuleDefID(int moduleDefID)
        {
            return CBO.FillCollection(provider.GetPermissionsByModuleDefID(moduleDefID), typeof(PermissionInfo));
        }
        public ArrayList GetPermissionsByModuleID(int moduleID)
        {
            return CBO.FillCollection(provider.GetPermissionsByModuleID(moduleID), typeof(PermissionInfo));
        }
        public void UpdatePermission(PermissionInfo objPermission)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objPermission, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PERMISSION_UPDATED);
            provider.UpdatePermission(objPermission.PermissionID, objPermission.PermissionCode, objPermission.ModuleDefID, objPermission.PermissionKey, objPermission.PermissionName, UserController.GetCurrentUserInfo().UserID);
        }
        public static string BuildPermissions(IList Permissions, string PermissionKey)
        {
            string strPrefix = "";
            string strPermission = "";
            StringBuilder objPermissions = new StringBuilder();
            foreach (PermissionInfoBase permission in Permissions)
            {
                if (permission.PermissionKey == PermissionKey)
                {
                    if (!permission.AllowAccess)
                    {
                        strPrefix = "!";
                    }
                    else
                    {
                        strPrefix = "";
                    }
                    if (Null.IsNull(permission.UserID))
                    {
                        strPermission = strPrefix + permission.RoleName + ";";
                    }
                    else
                    {
                        strPermission = strPrefix + "[" + permission.UserID.ToString() + "];";
                    }
                    if (strPrefix == "!")
                    {
                        objPermissions.Insert(0, strPermission);
                    }
                    else
                    {
                        objPermissions.Append(strPermission);
                    }
                }
            }
            string strPermissions = objPermissions.ToString();
            if (!strPermissions.StartsWith(";"))
            {
                strPermissions.Insert(0, ";");
            }
            return strPermissions;
        }
        public static ArrayList GetPermissionsByFolder()
        {
            return CBO.FillCollection(provider.GetPermissionsByFolder(), typeof(PermissionInfo));
        }
        public static ArrayList GetPermissionsByPortalDesktopModule()
        {
            return CBO.FillCollection(provider.GetPermissionsByPortalDesktopModule(), typeof(PermissionInfo));
        }
        public static ArrayList GetPermissionsByTab()
        {
            return CBO.FillCollection(provider.GetPermissionsByTab(), typeof(PermissionInfo));
        }

        #region Methods ========================================================================================
        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("aspnet_Permission_GetAll", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int PermissionId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Permission_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetPermissionsByCodeAndKey(string PermissionCode, string PermissionKey)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Permission_GetPermissionsByCodeAndKey", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PermissionCode", PermissionCode);
            cmd.Parameters.AddWithValue("@PermissionKey", PermissionKey);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetPermissionsByCode(string PermissionCode)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Permission_GetPermissionsByCode", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PermissionCode", PermissionCode);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetPermissionsByModuleId(int ModuleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Permission_GetPermissionsByModuleId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string PermissionCode, string PermissionKey, string PermissionName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Permission_Add", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PermissionCode", PermissionCode);
            cmd.Parameters.AddWithValue("@PermissionKey", PermissionKey);
            cmd.Parameters.AddWithValue("@PermissionName", PermissionName);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int PermissionId, string PermissionCode, string PermissionKey, string PermissionName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Permission_Edit", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            cmd.Parameters.AddWithValue("@PermissionCode", PermissionCode);
            cmd.Parameters.AddWithValue("@PermissionKey", PermissionKey);
            cmd.Parameters.AddWithValue("@PermissionName", PermissionName);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int PermissionId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Permission_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        #endregion =============================================================================================
    }
}
