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
    public class ProductLifeCycles
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductLifeCycles(){}

        public DataTable GetListByCodeDiscontinued(string LifeCycle_Code, int LifeCycle_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Periods_GetListByDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@LifeCycle_Code", LifeCycle_Code);
            cmd.Parameters.AddWithValue("@LifeCycle_Discontinued", LifeCycle_Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }


        public DataTable GetDetails(int LifeCycle_ID)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_LifeCycles_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@LifeCycle_ID", LifeCycle_ID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;           
        }

        public int Insert(string LifeCycle_Code, string LifeCycle_Name, int LifeCycle_StartPoint, int LifeCycle_EndPoint, string LifeCycle_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_LifeCycles_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@LifeCycle_Code", LifeCycle_Code);
            cmd.Parameters.AddWithValue("@LifeCycle_Name", LifeCycle_Name);
            cmd.Parameters.AddWithValue("@LifeCycle_StartPoint", LifeCycle_StartPoint);
            cmd.Parameters.AddWithValue("@LifeCycle_EndPoint", LifeCycle_EndPoint);
            cmd.Parameters.AddWithValue("@LifeCycle_Discontinued", LifeCycle_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int LifeCycle_ID, string LifeCycle_Code, string LifeCycle_Name, int LifeCycle_StartPoint, int LifeCycle_EndPoint, string LifeCycle_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_LifeCycles_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@LifeCycle_ID", LifeCycle_ID);
            cmd.Parameters.AddWithValue("@LifeCycle_Code", LifeCycle_Code);
            cmd.Parameters.AddWithValue("@LifeCycle_Name", LifeCycle_Name);
            cmd.Parameters.AddWithValue("@LifeCycle_StartPoint", LifeCycle_StartPoint);
            cmd.Parameters.AddWithValue("@LifeCycle_EndPoint", LifeCycle_EndPoint);
            cmd.Parameters.AddWithValue("@LifeCycle_Discontinued", LifeCycle_Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateDiscontinued(int LifeCycle_ID, string LifeCycle_Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_LifeCycles_UpdateDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@LifeCycle_ID", LifeCycle_ID);
            cmd.Parameters.AddWithValue("@LifeCycle_Discontinued", LifeCycle_Discontinued);
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
