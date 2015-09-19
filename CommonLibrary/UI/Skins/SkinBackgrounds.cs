using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web;
using CommonLibrary;
using System.Data.SqlClient;

namespace CommonLibrary.UI.Skins
{
    public class SkinBackgrounds
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public SkinBackgrounds()
        {
        }

        public DataTable GetListBySkinTypeSkinPackageIdDiscontinued(string SkinType, int SkinPackageId, int SkinBackground_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_GetListBySkinTypeSkinPackageIdDiscontinued", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinType", SkinType);
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);            
            cmd.Parameters.AddWithValue("@SkinBackground_Discontinued", SkinBackground_Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListBySkinTypeSkinPackageIdDiscontinuedWithQty(string SkinType, int SkinPackageId, int SkinBackground_Discontinued, int Qty)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_GetListBySkinTypeSkinPackageIdDiscontinuedWithQty", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinType", SkinType);
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);
            cmd.Parameters.AddWithValue("@SkinBackground_Discontinued", SkinBackground_Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetActiveListByQty(int SkinPackageId, int Qty)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_GetActiveListByQty", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int SkinBackgroundId)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinBackgroundId", SkinBackgroundId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetImageById(int SkinBackgroundId)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_GetImageById", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinBackgroundId", SkinBackgroundId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Insert(int SkinPackageId, string SkinBackground_Name, string SkinBackground_FileName, string SkinBackground_Url, int SkinBackground_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_Insert", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);
            cmd.Parameters.AddWithValue("@SkinBackground_Name", SkinBackground_Name);
            cmd.Parameters.AddWithValue("@SkinBackground_FileName", SkinBackground_FileName);
            cmd.Parameters.AddWithValue("@SkinBackground_Url", SkinBackground_Url);
            cmd.Parameters.AddWithValue("@SkinBackground_Discontinued", SkinBackground_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int SkinBackgroundId, int SkinPackageId, string SkinBackground_Name, string SkinBackground_FileName, string SkinBackground_Url, int SkinBackground_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_Update", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinBackgroundId", SkinBackgroundId);
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);
            cmd.Parameters.AddWithValue("@SkinBackground_Name", SkinBackground_Name);
            cmd.Parameters.AddWithValue("@SkinBackground_FileName", SkinBackground_FileName);
            cmd.Parameters.AddWithValue("@SkinBackground_Url", SkinBackground_Url);
            cmd.Parameters.AddWithValue("@SkinBackground_Discontinued", SkinBackground_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(int SkinBackgroundId, int SkinBackground_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_UpdateStatus", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinBackgroundId", SkinBackgroundId);
            cmd.Parameters.AddWithValue("@SkinBackground_Discontinued", SkinBackground_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateSortKey(int SkinBackgroundId, int SkinBackground_SortKey)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_UpdateSortKey", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinBackgroundId", SkinBackgroundId);
            cmd.Parameters.AddWithValue("@SkinBackground_SortKey", SkinBackground_SortKey);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateSortKeyUpDown(int signal, int SkinBackgroundId)
        {
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_UpdateSortKeyUpDown", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@signal", signal);
            cmd.Parameters.AddWithValue("@current_idx", SkinBackgroundId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int SkinBackgroundId, string dir_path)
        {
            //string current_photo = GetImageById(SkinBackgroundId);
            SqlCommand cmd = new SqlCommand("SkinBackgrounds_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinBackgroundId", SkinBackgroundId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
