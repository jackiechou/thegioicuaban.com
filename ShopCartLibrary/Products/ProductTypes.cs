using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Library.Common;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;
using CommonLibrary.Common;

namespace ShopCartLibrary.Products
{
    public class ProductTypes
    {
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductTypes()
        {
        }

        public DataTable GetListByStatus(string Product_TypePublish)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Types_GetListByStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_TypePublish", Product_TypePublish);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }             

        public DataTable GetDetails(int Product_TypeId)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Types_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_TypeId", Product_TypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string Category_Code, string Product_TypeName, string Product_TypeDescription, string Product_TypePublish)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Types_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Product_TypeName", Product_TypeName);
            cmd.Parameters.AddWithValue("@Product_TypeDescription", Product_TypeDescription);
            cmd.Parameters.AddWithValue("@Product_TypePublish", Product_TypePublish);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int InsertVendorProductType(int VendorId, int ProductTypeId)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Vendor_ProductTypes_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);            
            cmd.Parameters.AddWithValue("@ProductTypeId", ProductTypeId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int Product_TypeId, string Category_Code, string Product_TypeName, string Product_TypeDescription, string Product_TypePublish)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Types_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Product_TypeId", Product_TypeId);
            cmd.Parameters.AddWithValue("@Product_TypeName", Product_TypeName);
            cmd.Parameters.AddWithValue("@Product_TypeDescription", Product_TypeDescription);
            cmd.Parameters.AddWithValue("@Product_TypePublish", Product_TypePublish);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

       
        public int UpdateSortKey(int option, int id, int previous_id)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Types_UpdateSortKey]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@option", option);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@previous_id", previous_id);        
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }  
    }
}
