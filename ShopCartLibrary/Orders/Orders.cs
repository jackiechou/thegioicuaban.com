using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Library.Common;
using CommonLibrary;
using CommonLibrary.Common;

namespace ShopCartLibrary.Orders
{
    public class Orders
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();
        string ip = IPNetworking.GetIP4Address();

        public Orders()
        {
        }

        public int GetMinYearInOrderList()
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetMinYearInOrderList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, 10) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetAll]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByConditions(string StartDate, string EndDate, string Status)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetListByConditions]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@Status", Status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByOrderType(string Code,string StartDate, string EndDate, string Status)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetListByOrderType]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Code", Code);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@Status", Status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByStatus(string Status)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetListByStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Status", Status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetOrderInfoByOrderID(string Order_ID)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetOrderInfoByOrderID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_ID", Order_ID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetOrderInfoByOrderNo(string Order_No)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetOrderInfoByOrderNo]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetOrderInfoByCustomerNo(string Customer_No)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetOrderInfoByCustomerNo]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Customer_No", Customer_No);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int GetNumOfUnReadList()
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_GetNumOfUnReadList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }    
     
        public int UpdateMarkAsRead(string Order_No)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_UpdateMarkAsRead]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, 2) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public string[] Insert(string Customer_No,int Payment_Method_Id, 
            decimal Shipping_Rate, string Coupon_Code, decimal Coupon_Discount, decimal SubTotal, decimal TotalFees,
            decimal Discount, decimal Tax, string CurrencyCode, string Comment, int OnlineOrderFlag)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Customer_No", Customer_No);
            cmd.Parameters.AddWithValue("@Payment_Method_Id", Payment_Method_Id);
            cmd.Parameters.AddWithValue("@Shipping_Rate", Shipping_Rate);
            cmd.Parameters.AddWithValue("@Coupon_Code", Coupon_Code);
            cmd.Parameters.AddWithValue("@Coupon_Discount", Coupon_Discount);
            cmd.Parameters.AddWithValue("@SubTotal", SubTotal);
            cmd.Parameters.AddWithValue("@TotalFees", TotalFees);
            cmd.Parameters.AddWithValue("@Discount", Discount);
            cmd.Parameters.AddWithValue("@Tax", Tax);
            cmd.Parameters.AddWithValue("@CurrencyCode", CurrencyCode);
            cmd.Parameters.AddWithValue("@Comment", Comment);
            cmd.Parameters.AddWithValue("@OnlineOrderFlag", OnlineOrderFlag);
            cmd.Parameters.AddWithValue("@IPLog", ip);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@o_OrderNo", SqlDbType.NVarChar, int.MaxValue) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            string[] arr_result = new string[2];
            arr_result[0] = cmd.Parameters["@o_return"].Value.ToString();
            arr_result[1] = cmd.Parameters["@o_OrderNo"].Value.ToString();
            con.Close();
            return arr_result;
        }


        public DataTable Search(string keywords)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_Search]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@keywords", keywords);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int UpdateShipStatus(string Order_No, string LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_Update_ShipStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, 2) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdatePaymentStatus(string Order_No, string LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_Update_PaymentStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, 2) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        
        public int UpdateOrderStatus(string Order_No, int OrderStatus, string LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Orders_UpdateOrderStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.AddWithValue("@OrderStatus", OrderStatus);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, 2) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
