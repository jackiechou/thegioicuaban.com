using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;


namespace CommonLibrary.Application
{
    public class PersonalizationAdministrationClass
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

	    public PersonalizationAdministrationClass()
	    {
	    }

        public int DeleteAllState(bool AllUsersScope, string ApplicationName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAdministration_DeleteAllState", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@AllUsersScope", AllUsersScope);
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@Count"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable FindState(bool AllUsersScope, string ApplicationName, int PageIndex, int PageSize, string Path, string UserName)
        {
            DateTime InactiveSinceDate = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAdministration_FindState", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@AllUsersScope", AllUsersScope);
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            cmd.Parameters.AddWithValue("@Path", Path);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@InactiveSinceDate", InactiveSinceDate);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int GetCountOfState(bool AllUsersScope, string ApplicationName, string Path, string UserName)
        {        
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAdministration_GetCountOfState", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@AllUsersScope", AllUsersScope);
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Path", Path);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@Count"].Value;
            con.Close();
            return retunvalue;
        }

        public int ResetSharedState(bool AllUsersScope, string ApplicationName, string Path, string UserName)
        {        
            DateTime InactiveSinceDate = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAdministration_ResetSharedState", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@AllUsersScope", AllUsersScope);
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Path", Path);                
            cmd.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@Count"].Value;
            con.Close();
            return retunvalue;
        }

        public int ResetUserState(bool AllUsersScope, string ApplicationName, string Path, string UserName)
        {
            DateTime InactiveSinceDate = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAdministration_ResetUserState", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@AllUsersScope", AllUsersScope);
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Path", Path);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@InactiveSinceDate", InactiveSinceDate);
            cmd.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@Count"].Value;
            con.Close();
            return retunvalue;
        }
    }      
}