using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Security.Permissions
{
    public class ModulePermissions
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();
        public ModulePermissions()
        {
        }

        public DataTable GetModulePermission()
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_GetModulePermission", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetModulePermissionByModulePermissionId(int ModulePermissionId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_GetModulePermissionByModulePermissionId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModulePermissionId", ModulePermissionId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetModulePermissionsByModuleId(int ModuleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_GetModulePermissionsByModuleId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByRoleIdPermissionId(string RoleId, int PermissionId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_GetListByRoleIdPermissionId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int AddModulePermission(int ModuleId, int PermissionId, string RoleId, int AllowAccess, string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_AddModulePermission", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@AllowAccess", AllowAccess);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateModulePermission(int ModulePermissionId, int ModuleId, int PermissionId, string RoleId, bool AllowAccess, string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_UpdateModulePermission", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModulePermissionId", ModulePermissionId);
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.AddWithValue("@PermissionId", PermissionId);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@AllowAccess", AllowAccess);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int DeleteModulePermission(int ModulePermissionId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_DeleteModulePermission", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModulePermissionId", ModulePermissionId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int DeleteModulePermissionsByModuleId(int ModuleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_DeleteModulePermissionsByModuleId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int DeleteModulePermissionsByUserId(string ApplicationId, string UserId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModulePermission_DeleteModulePermissionsByUserId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
