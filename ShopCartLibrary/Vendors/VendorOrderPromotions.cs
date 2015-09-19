using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;
using CommonLibrary.Common;
using CommonLibrary.Modules;


namespace ShopCartLibrary.Vendors
{
    public class VendorOrderPromotions
    {
        ModuleClass module_obj = new ModuleClass();
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public VendorOrderPromotions()
        {
        }

        public DataTable GetListByVendorId(string VendorId, string Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_OrderPromotions_GetListByVendorIdDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByVendorOrderPromotionId(int VendorOrderPromotionId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_OrderPromotions_GetDetailByVendorOrderPromotionId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorOrderPromotionId", VendorOrderPromotionId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int VendorId, string OrderValue, string PromotionValue, int IsPercent, string Description, string StartDate, string EndDate, string Discontinued)
        {
            OrderValue = StringHandleClass.RemoveExtraTextWithoutPointOrComma(OrderValue);
            PromotionValue = StringHandleClass.RemoveExtraTextWithoutPointOrComma(PromotionValue);

            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_OrderPromotions_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@OrderValue", OrderValue);
            cmd.Parameters.AddWithValue("@PromotionValue", PromotionValue);
            cmd.Parameters.AddWithValue("@IsPercent", IsPercent);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int VendorOrderPromotionId, int Order_PromotionId, string OrderValue, string PromotionValue, int IsPercent, string Description, string StartDate, string EndDate, string Discontinued)
        {
            OrderValue = StringHandleClass.RemoveExtraTextWithoutPointOrComma(OrderValue);
            PromotionValue = StringHandleClass.RemoveExtraTextWithoutPointOrComma(PromotionValue);

            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_OrderPromotions_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorOrderPromotionId", VendorOrderPromotionId);
            cmd.Parameters.AddWithValue("@Order_PromotionId", Order_PromotionId);
            cmd.Parameters.AddWithValue("@OrderValue", OrderValue);
            cmd.Parameters.AddWithValue("@PromotionValue", PromotionValue);
            cmd.Parameters.AddWithValue("@IsPercent", IsPercent);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int VendorOrderPromotionId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_OrderPromotions_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorOrderPromotionId", VendorOrderPromotionId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}