using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Application
{
    public class PathClass
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public PathClass()
        {
        }

        public DataTable GetDetails(string PathId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Paths_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PathId", PathId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetPath(string app_id)
        {
            System.Guid ApplicationId = new System.Guid(app_id);
            SqlCommand cmd = new SqlCommand("aspnet_Paths_GetPath", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string CreatePath(string app_id, string path)
        {
            System.Guid ApplicationId = new System.Guid(app_id);
            SqlCommand cmd = new SqlCommand("aspnet_Paths_CreatePath", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@Path", path);
            cmd.Parameters.Add(new SqlParameter("@PathId", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            string PathId = (string)cmd.Parameters["@PathId"].Value;
            con.Close();
            return PathId;
        }

        public int UpdatePath(string app_id, string path_id, string path)
        {
            System.Guid ApplicationId = new System.Guid(app_id);
            System.Guid PathId = new System.Guid(path_id);
            SqlCommand cmd = new SqlCommand("aspnet_Paths_UpdatePath", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@PathId", PathId);
            cmd.Parameters.AddWithValue("@Path", path);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(string path_id)
        {
            System.Guid PathId = new System.Guid(path_id);
            SqlCommand cmd = new SqlCommand("aspnet_Paths_DeletePath", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PathId", PathId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
