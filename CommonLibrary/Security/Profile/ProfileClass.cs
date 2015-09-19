using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;

namespace CommonLibrary.Security.Profile
{
    public class ProfileClass
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public ProfileClass() { }

        public int DeleteInactiveProfiles(string ApplicationName, int ProfileAuthOptions)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Profile_DeleteInactiveProfiles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@ProfileAuthOptions", ProfileAuthOptions);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int DeleteProfiles(string ApplicationName, string UserNames)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Profile_DeleteProfiles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserNames", UserNames);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable GetNumberOfInactiveProfiles(string ApplicationName, int ProfileAuthOptions)
        {
            DateTime InactiveSinceDate = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_Profile_GetNumberOfInactiveProfiles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@ProfileAuthOptions", ProfileAuthOptions);
            cmd.Parameters.AddWithValue("@InactiveSinceDate", InactiveSinceDate);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetProfiles(string ApplicationName, int ProfileAuthOptions, int PageIndex, int PageSize, string UserNameToMatch)
        {
            DateTime InactiveSinceDate = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_Profile_GetProfiles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@ProfileAuthOptions", ProfileAuthOptions);
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            cmd.Parameters.AddWithValue("@UserNameToMatch", UserNameToMatch);
            cmd.Parameters.AddWithValue("@InactiveSinceDate", InactiveSinceDate);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetProperties(string ApplicationName, string UserName)
        {
            DateTime CurrentTimeUtc = System.DateTime.Now;
            SqlCommand cmd = new SqlCommand("aspnet_Profile_GetProperties", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public void SetProperties(string ApplicationName, string PropertyNames, string PropertyValuesString, byte[] PropertyValuesBinary, string UserName, bool IsUserAnonymous)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Profile_SetProperties", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@PropertyNames", PropertyNames);
            cmd.Parameters.AddWithValue("@PropertyValuesString", PropertyValuesString);
            cmd.Parameters.AddWithValue("@PropertyValuesBinary", PropertyValuesBinary);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@IsUserAnonymous", IsUserAnonymous);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}