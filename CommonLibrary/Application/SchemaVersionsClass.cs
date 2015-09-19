using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Library.Entities.Users;
using CommonLibrary;

namespace CommonLibrary.Application
{
    public class SchemaVersionsClass
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();


        public SchemaVersionsClass()
        {
        }

        public void RegisterSchemaVersion(string Feature, string CompatibleSchemaVersion, bool IsCurrentVersion, bool RemoveIncompatibleSchema)
        {
            SqlCommand cmd = new SqlCommand("aspnet_RegisterSchemaVersion", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Feature", Feature);
            cmd.Parameters.AddWithValue("@CompatibleSchemaVersion", CompatibleSchemaVersion);
            cmd.Parameters.AddWithValue("@IsCurrentVersion", IsCurrentVersion);
            cmd.Parameters.AddWithValue("@RemoveIncompatibleSchema", RemoveIncompatibleSchema);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void CheckSchemaVersion(string Feature, string CompatibleSchemaVersion)
        {
            SqlCommand cmd = new SqlCommand("aspnet_CheckSchemaVersion", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Feature", Feature);
            cmd.Parameters.AddWithValue("@CompatibleSchemaVersion", CompatibleSchemaVersion);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UnRegisterSchemaVersion(string Feature, string CompatibleSchemaVersion)
        {
            SqlCommand cmd = new SqlCommand("aspnet_UnRegisterSchemaVersion", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Feature", Feature);
            cmd.Parameters.AddWithValue("@CompatibleSchemaVersion", CompatibleSchemaVersion);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}