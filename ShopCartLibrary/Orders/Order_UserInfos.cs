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
    public class Order_UserInfos
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public Order_UserInfos()
        {
        }

        public int Insert(string Order_No, string Customer_No, string ReceiverName, string ReceiverEmail, string ReceiverPhone, string ReceiverAddress)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[Order_UserInfos_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Order_No", Order_No);
            cmd.Parameters.AddWithValue("@Customer_No", Customer_No);
            cmd.Parameters.AddWithValue("@ReceiverName", ReceiverName);
            cmd.Parameters.AddWithValue("@ReceiverEmail", ReceiverEmail);
            cmd.Parameters.AddWithValue("@ReceiverPhone", ReceiverPhone);
            cmd.Parameters.AddWithValue("@ReceiverAddress", ReceiverAddress);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
