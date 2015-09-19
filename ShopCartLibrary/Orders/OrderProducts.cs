using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;


namespace ShopCartLibrary.Orders
{
    public class OrderProducts
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public OrderProducts(){}

        public DataTable GetOrderDetailsByOrderID(string Order_ID)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Order_Details_GetOrderDetailsByOrderID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_ID", Order_ID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetOrderProductsByOrderNo(string Order_No)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Order_Details_GetOrderDetailsByOrderID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetOrderProductsByOrderNoProductNo(string Order_No, string Product_No)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Order_Details_GetOrderDetailsByOrderNoProductNo]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert_OrderProducts(string Order_No, string Product_No, int OrderQty,decimal NetPrice, decimal GrossPrice, int VendorId)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Order_Products_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            cmd.Parameters.AddWithValue("@OrderQty", OrderQty);
            cmd.Parameters.AddWithValue("@NetPrice", NetPrice);
            cmd.Parameters.AddWithValue("@GrossPrice", GrossPrice);
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateProductQuantity_OrderDetails(string Order_No, int OrderQty)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Order_Details_UpdateProductQuantity]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);           
            cmd.Parameters.AddWithValue("@OrderQty", OrderQty);           
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update_OrderProducts(string Order_No, string Product_No, string Category_Code, int OrderQty, string UnitPrice,
            string VATAmount, string PromotionalAmount, string LastModifiedByUserId)
        {
            //SqlCommand cmd = new SqlCommand("[Sales].[Order_Details_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            //cmd.Parameters.AddWithValue("@Order_No", Order_No);
            //cmd.Parameters.AddWithValue("@Product_No", Product_No);
            //cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            //cmd.Parameters.AddWithValue("@OrderQty", OrderQty);
            //cmd.Parameters.AddWithValue("@NetPrice", NetPrice);
            //cmd.Parameters.AddWithValue("@GrossPrice", GrossPrice);
            //cmd.Parameters.AddWithValue("@VendorId", VendorId);
            //cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            //con.Open();
            //int i = cmd.ExecuteNonQuery();
            //int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            //con.Close();
            int retunvalue = 0;
            return retunvalue;
        }

        public int Delete_OrderProducts(string Order_No, string Product_No, string LastModifiedByUserId)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[OrderDetails_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
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
