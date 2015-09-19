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
    public class ProductAttributeOptions
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductAttributeOptions()
        {
        }

        public DataTable GetListByAttributeId(int Attribute_Id)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_AttributeOptions_GetListByAttributeId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Attribute_Id", Attribute_Id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int Option_Id)
        {           
            SqlCommand cmd = new SqlCommand("[Production].[Product_AttributeOptions_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Option_Id", Option_Id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int Attribute_Id, string Option_Name, decimal Option_Value)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_AttributeOptions_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Attribute_Id", Attribute_Id);
            cmd.Parameters.AddWithValue("@Option_Name", Option_Name);
            cmd.Parameters.AddWithValue("@Option_Value", Option_Value);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int Option_Id, string Option_Name, decimal Option_Value)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_AttributeOptions_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Option_Id", Option_Id);
            cmd.Parameters.AddWithValue("@Option_Name", Option_Name);
            cmd.Parameters.AddWithValue("@Option_Value", Option_Value);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int Option_Id)
        {           
            SqlCommand cmd = new SqlCommand("[Production].[Product_AttributeOptions_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Option_Id", Option_Id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
