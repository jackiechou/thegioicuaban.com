using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;
using CommonLibrary.Modules;

namespace ShopCartLibrary.Vendors
{
    public class VendorDiscounts
    {       
        private static int Timeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString());
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public VendorDiscounts()
        {
        }

        public DataTable GetListByVendorId(int VendorId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductDiscounts_GetListByVendorId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByVendorProductDiscountId(int VendorProductDiscountId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductDiscounts_GetDetailByVendorProductDiscountId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorProductDiscountId", VendorProductDiscountId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByDiscountId(int DiscountId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductDiscounts_GetDetailByDiscountId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@DiscountId", DiscountId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int VendorId, int DiscountQty, string DiscountRate, int IsPercent, string Description, string StartDate, string EndDate)
        {
            DiscountRate = StringHandleClass.RemoveExtraTextWithoutPointOrComma(DiscountRate);
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductDiscounts_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@DiscountQty", DiscountQty);
            cmd.Parameters.AddWithValue("@DiscountRate", DiscountRate);
            cmd.Parameters.AddWithValue("@IsPercent", IsPercent);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int VendorProductDiscountId, int DiscountId, int DiscountQty, string DiscountRate, int IsPercent, string Description, string StartDate, string EndDate)
        {
            DiscountRate = StringHandleClass.RemoveExtraTextWithoutPointOrComma(DiscountRate);
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductDiscounts_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorProductDiscountId", VendorProductDiscountId);
            cmd.Parameters.AddWithValue("@DiscountQty", DiscountQty);
            cmd.Parameters.AddWithValue("@DiscountRate", DiscountRate);
            cmd.Parameters.AddWithValue("@IsPercent", IsPercent);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        
        public int Delete(int VendorProductDiscountId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductDiscounts_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorProductDiscountId", VendorProductDiscountId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }      
    }
}
