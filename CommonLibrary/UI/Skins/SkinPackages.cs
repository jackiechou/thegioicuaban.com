using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using CommonLibrary;
using System.Data.SqlClient;

namespace CommonLibrary.UI.Skins
{
    public class SkinPackages
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public SkinPackages()
        {
        }

        public DataTable GetListByPortalIdSkinType(int PortalId, string SkinType)
        {
            SqlCommand cmd = new SqlCommand("SkinPackages_GetListByPortalIdSkinType", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@SkinType", SkinType);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int SkinId)
        {
            SqlCommand cmd = new SqlCommand("SkinPackages_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinId", SkinId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int PortalId, string SkinName, string SkinType, string SkinIcon, string CreatedByUserId)
        {
            SqlCommand cmd = new SqlCommand("SkinPackages_Insert", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@SkinName", SkinName);
            cmd.Parameters.AddWithValue("@SkinType", SkinType);
            cmd.Parameters.AddWithValue("@SkinIcon", SkinIcon);
            cmd.Parameters.AddWithValue("@CreatedByUserId", CreatedByUserId);            
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int SkinPackageId, int PortalId, string SkinName, string SkinType, string SkinIcon, string LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("SkinPackages_Update", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@SkinName", SkinName);
            cmd.Parameters.AddWithValue("@SkinType", SkinType);
            cmd.Parameters.AddWithValue("@SkinIcon", SkinIcon);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
