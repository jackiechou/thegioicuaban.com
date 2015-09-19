using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Modules.Dashboard.Components.Modules
{
    public class Modules
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("aspnet_Modules_GetAll", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetModuleListByPortalId(string PortalId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Modules_GetModuleListByPortalId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int ModuleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Modules_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetModuleTitleByModuleId(int ModuleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Modules_GetModuleTitleByModuleId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.NVarChar, int.MaxValue) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string ModuleTitle = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return ModuleTitle;
        }

        public int Insert(string ApplicationId, string ModuleTitle, bool AllTabs, bool IsAdmin, bool IsDeleted, bool InheritViewPermissions)
        {
            string FriendlyName = ModuleClass.convertTitle2Link(ModuleTitle);
            SqlCommand cmd = new SqlCommand("aspnet_Modules_Add", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@ModuleTitle", ModuleTitle);
            cmd.Parameters.AddWithValue("@FriendlyName", FriendlyName);
            cmd.Parameters.AddWithValue("@AllTabs", AllTabs);
            cmd.Parameters.AddWithValue("@IsAdmin", IsAdmin);
            cmd.Parameters.AddWithValue("@IsDeleted", IsDeleted);
            cmd.Parameters.AddWithValue("@InheritViewPermissions", InheritViewPermissions);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int ModuleId, string ApplicationId, string ModuleTitle, bool AllTabs, bool IsAdmin, bool IsDeleted, bool InheritViewPermissions)
        {
            string FriendlyName = ModuleClass.convertTitle2Link(ModuleTitle);
            SqlCommand cmd = new SqlCommand("aspnet_Modules_Edit", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.AddWithValue("@ModuleTitle", ModuleTitle);
            cmd.Parameters.AddWithValue("@FriendlyName", FriendlyName);
            cmd.Parameters.AddWithValue("@AllTabs", AllTabs);
            cmd.Parameters.AddWithValue("@IsAdmin", IsAdmin);
            cmd.Parameters.AddWithValue("@IsDeleted", IsDeleted);
            cmd.Parameters.AddWithValue("@InheritViewPermissions", InheritViewPermissions);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }


        public int Delete(int ModuleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Modules_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

      
    }
}
