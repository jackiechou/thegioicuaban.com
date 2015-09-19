using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Application
{
    public class ApplicationController
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();
        #region Methods ========================================================================
        public DataTable GetApps()
        {
            SqlCommand cmd = new SqlCommand("aspnet_Applications_GetApplication", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(string app_id)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Applications_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@app_id", app_id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetApplicationId(string ApplicationName, out string ApplicationId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Personalization_GetApplicationId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            ApplicationId = (string)cmd.ExecuteScalar();
            return ApplicationId;
        }

        public string Insert(string ApplicationName, string Description)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Applications_CreateApplication", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.Add(new SqlParameter("@ApplicationId", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            string ApplicationId = (string)cmd.Parameters["@ApplicationId"].Value;
            con.Close();
            return ApplicationId;
        }

        public int Update(string ApplicationId, string ApplicationName, string Description)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Applications_UpdateApplication", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(string ApplicationId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Applications_DeleteApplication", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        #endregion =============================================================================
    }
}
