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

namespace ShopCartLibrary.Orders
{
    public class OrderShipment
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public OrderShipment(){}

        public int Insert(string Order_No, string Customer_No, string Ship_ReceiverName, string Ship_ReceiverEmail, string Ship_ReceiverPhone, string Ship_ReceiverAddress, decimal Ship_Weight, int Ship_IsInternational)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Order_Shipment_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.AddWithValue("@Customer_No", Customer_No);
            cmd.Parameters.AddWithValue("@Ship_ReceiverName", Ship_ReceiverName);
            cmd.Parameters.AddWithValue("@Ship_ReceiverEmail", Ship_ReceiverEmail);
            cmd.Parameters.AddWithValue("@Ship_ReceiverPhone", Ship_ReceiverPhone);
            cmd.Parameters.AddWithValue("@Ship_ReceiverAddress", Ship_ReceiverAddress);
            cmd.Parameters.AddWithValue("@Ship_Weight", Ship_Weight);
            cmd.Parameters.AddWithValue("@Ship_IsInternational", Ship_IsInternational);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
