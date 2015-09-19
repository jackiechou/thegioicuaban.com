using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;
namespace CommonLibrary.Application
{
    public class PersonalizationPerUserClass
    {
        SqlConnection con = new SqlConnection(CommonLibrary.Settings.ConnectionString);
        DataTable dt = new DataTable();

        public PersonalizationPerUserClass() { }

        public void SetPageSettings(string ApplicationName, string UserName, string Path, byte[] PageSettings)
        {
            DateTime CurrentTimeUtc = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationPerUser_SetPageSettings", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Path", Path);
            cmd.Parameters.AddWithValue("@PageSettings", PageSettings);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ResetPageSettings(string ApplicationName, string UserName, string Path)
        {
            DateTime CurrentTimeUtc = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationPerUser_ResetPageSettings", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Path", Path);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void GetPageSettings(string ApplicationName, string UserName, string Path)
        {
            DateTime CurrentTimeUtc = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_PersonalizationAllUsers_GetPageSettings", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Path", Path);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}