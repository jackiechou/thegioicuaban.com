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
    public class ProductAttributes
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductAttributes(){}

        public DataTable GetListByProductNo(string Product_No)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Attributes_GetListByProductNo]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int Attribute_Id)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Attributes_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Attribute_Id", Attribute_Id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string[] Insert(string Product_No, string Attribute_Name)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Attributes_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            cmd.Parameters.AddWithValue("@Attribute_Name", Attribute_Name);            
            cmd.Parameters.Add("@Attribute_Id", SqlDbType.Int, int.MaxValue).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_return", SqlDbType.Int, int.MaxValue).Direction = ParameterDirection.Output;
            con.Open();
            cmd.ExecuteNonQuery();            
            string[] arr_result = new string[2];
            arr_result[0] = cmd.Parameters["@o_return"].Value.ToString();
            arr_result[1] = cmd.Parameters["@Attribute_Id"].ToString();
            con.Close();
            return arr_result;
        }

        public int Update(int Attribute_Id, string Attribute_Name)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Attributes_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Attribute_Id", Attribute_Id);
            cmd.Parameters.AddWithValue("@Attribute_Name", Attribute_Name);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int Attribute_Id)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Attributes_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Attribute_Id", Attribute_Id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
