using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;

namespace CommonLibrary.Common
{
    public class CultureClass
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public CultureClass()  { }

        public DataTable GetListByPortalId(int PortalId)
        {
            SqlCommand cmd = new SqlCommand("[aspnet_Culture_GetListByPortalId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByDiscontinued(string Discontinued)
        {          
            SqlCommand cmd = new SqlCommand("[aspnet_Culture_GetListByDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int idx)
        {
            SqlCommand cmd = new SqlCommand("[aspnet_Culture_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@idx", idx);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string CultureCode, string CultureName, string Description, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[aspnet_Culture_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@CultureName", CultureName);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int CultureId, string CultureCode, string CultureName, string Description, int Discontinued)
        {           
            SqlCommand cmd = new SqlCommand("[aspnet_Culture_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@CultureId", CultureId);
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@CultureName", CultureName);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateDiscontinued(int CultureId, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[aspnet_Culture_UpdateDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@CultureId", CultureId);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);          
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
