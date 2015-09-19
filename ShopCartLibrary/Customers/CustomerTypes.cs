using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;

namespace ShopCartLibrary.Customers
{
    public class CustomerTypes
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();    

        public CustomerTypes(){}

        public DataTable GetListByDiscontinued(int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[CustomerTypes_GetListByDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int CustomerType_ID)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[CustomerTypes_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CustomerType_ID", CustomerType_ID);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string CustomerType_BranchCode, string CustomerType_Name, int CustomerType_PromotionalRate, string CustomerType_Description, int Discontinued)
        {            
            SqlCommand cmd = new SqlCommand("[Sales].[CustomerTypes_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CustomerType_BranchCode", CustomerType_BranchCode);
            cmd.Parameters.AddWithValue("@CustomerType_Name", CustomerType_Name);
            cmd.Parameters.AddWithValue("@CustomerType_PromotionalRate", CustomerType_PromotionalRate);
            cmd.Parameters.AddWithValue("@CustomerType_Description", CustomerType_Description);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int CustomerType_ID, string CustomerType_BranchCode, string CustomerType_Name, int CustomerType_PromotionalRate, string CustomerType_Description, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[CustomerTypes_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CustomerType_ID", CustomerType_ID);
            cmd.Parameters.AddWithValue("@CustomerType_BranchCode", CustomerType_BranchCode);
            cmd.Parameters.AddWithValue("@CustomerType_Name", CustomerType_Name);
            cmd.Parameters.AddWithValue("@CustomerType_PromotionalRate", CustomerType_PromotionalRate);
            cmd.Parameters.AddWithValue("@CustomerType_Description", CustomerType_Description);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateDiscontinued(int CustomerType_ID, string Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Sales].[CustomerTypes_UpdateDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CustomerType_ID", CustomerType_ID);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
