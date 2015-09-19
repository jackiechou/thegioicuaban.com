using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;

namespace CommonLibrary.Application
{
    public class PersonalizationAllUsersClass
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public PersonalizationAllUsersClass()
        {
        }

        public void SetPageSettings(string ApplicationName, string Path, byte[] PageSettings)
        {
            DateTime CurrentTimeUtc = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAllUsers_SetPageSettings", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Path", Path);
            cmd.Parameters.AddWithValue("@PageSettings", PageSettings);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

        }

        public void ResetPageSettings(string ApplicationName, string Path)
        {
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAllUsers_ResetPageSettings", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Path", Path);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void GetPageSettings(string ApplicationName, string Path)
        {

            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAllUsers_GetPageSettings", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Path", Path);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}