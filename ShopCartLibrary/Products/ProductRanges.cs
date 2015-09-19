using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;

namespace ShopCartLibrary.Products
{
    public class ProductRanges
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductRanges(){}

        public DataTable GetActiveList()
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Ranges_GetActiveList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByCodeDiscontinued(string Range_Code, string Range_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Ranges_GetListByCodeDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Range_Code", Range_Code);
            cmd.Parameters.AddWithValue("@Range_Discontinued", Range_Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int Range_ID)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Ranges_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Range_ID", Range_ID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string Range_Code, string Range_Name, string Range_StartPoint, string Range_EndPoint, int Range_Discontinued)
        {
            Range_StartPoint = RemoveExtraText(Range_StartPoint);
            Range_EndPoint = RemoveExtraText(Range_EndPoint);
            SqlCommand cmd = new SqlCommand("[Production].[Product_Ranges_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Range_Code", Range_Code);
            cmd.Parameters.AddWithValue("@Range_Name", Range_Name);
            cmd.Parameters.AddWithValue("@Range_StartPoint", Range_StartPoint);
            cmd.Parameters.AddWithValue("@Range_EndPoint", Range_EndPoint);
            cmd.Parameters.AddWithValue("@Range_Discontinued", Range_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int Range_ID, string Range_Code, string Range_Name, string Range_StartPoint, string Range_EndPoint, int Range_Discontinued)
        {
            Range_StartPoint = RemoveExtraText(Range_StartPoint);
            Range_EndPoint = RemoveExtraText(Range_EndPoint);
                        
            SqlCommand cmd = new SqlCommand("[Production].[Product_Ranges_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Range_ID", Range_ID);
            cmd.Parameters.AddWithValue("@Range_Code", Range_Code);
            cmd.Parameters.AddWithValue("@Range_Name", Range_Name);
            cmd.Parameters.AddWithValue("@Range_StartPoint", Range_StartPoint);
            cmd.Parameters.AddWithValue("@Range_EndPoint", Range_EndPoint);
            cmd.Parameters.AddWithValue("@Range_Discontinued", Range_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateDiscontinued(int Range_ID, string Range_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Ranges_UpdateDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Range_ID", Range_ID);
            cmd.Parameters.AddWithValue("@Range_Discontinued", Range_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int Range_ID)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Ranges_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Range_ID", Range_ID);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        private string RemoveExtraText(string value)
        {
            //var allowedChars = "01234567890.,";
            var allowedChars = "01234567890";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }
}
