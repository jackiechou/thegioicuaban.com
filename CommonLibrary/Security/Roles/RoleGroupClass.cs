using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;

namespace CommonLibrary.Security.Roles
{
    public class RoleGroupClass
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();


        public RoleGroupClass()
        {            
        }
        
        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("aspnet_RoleGroups_GetAll", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };          
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByPortalID(int PortalID)
        {
            SqlCommand cmd = new SqlCommand("aspnet_RoleGroups_GetListByPortalID", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalID", PortalID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int RoleGroupID)
        {
            SqlCommand cmd = new SqlCommand("aspnet_RoleGroups_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@RoleGroupID", RoleGroupID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int CreateRoleGroup(int PortalID, string RoleGroupName, string Description, int CreatedByUserID)
        {
            SqlCommand cmd = new SqlCommand("aspnet_RoleGroups_CreateRoleGroup", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalID", PortalID);
            cmd.Parameters.AddWithValue("@RoleGroupName", RoleGroupName);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateRoleGroup(int RoleGroupID, int PortalID, string RoleGroupName, string Description, int LastModifiedByUserID)
        {
            SqlCommand cmd = new SqlCommand("aspnet_RoleGroups_UpdateRoleGroup", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@RoleGroupID", RoleGroupID);
            cmd.Parameters.AddWithValue("@PortalID", PortalID);
            cmd.Parameters.AddWithValue("@RoleGroupName", RoleGroupName);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@LastModifiedByUserID", LastModifiedByUserID);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
