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
    public class ProductDiscounts
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductDiscounts(){}

        public DataTable GetList()
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Discounts_GetList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int DiscountId)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Discounts_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@DiscountId", DiscountId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int DiscountQty, decimal DiscountRate, int IsPercent, string Description)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Discounts_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@DiscountQty", DiscountQty);
            cmd.Parameters.AddWithValue("@DiscountRate", DiscountRate);
            cmd.Parameters.AddWithValue("@IsPercent", IsPercent);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }  

        public int Update(int DiscountId, int DiscountQty, decimal DiscountRate, int IsPercent, string Description)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Discounts_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@DiscountId", DiscountId);
            cmd.Parameters.AddWithValue("@DiscountQty", DiscountQty);
            cmd.Parameters.AddWithValue("@DiscountRate", DiscountRate);
            cmd.Parameters.AddWithValue("@IsPercent", IsPercent);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int DiscountId)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Discounts_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@DiscountId", DiscountId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
