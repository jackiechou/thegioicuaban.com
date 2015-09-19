using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web;
using Library.Common;
using CommonLibrary;
using CommonLibrary.Common;

namespace ShopCartLibrary.Vendors
{
    public class VendorProductTypes
    {
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public VendorProductTypes()
        {
        }

        public DataTable GetListByVendorId(int VendorId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductTypes_GetListByVendorId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailVendorProductTypeId(int VendorProductTypeId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductTypes_GetDetailVendorProductTypeId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorProductTypeId", VendorProductTypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int Product_TypeId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Product_Types_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_TypeId", Product_TypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int VendorId, string Product_TypeName, string Product_TypeDescription, string Product_TypePublish)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductTypes_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
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

        public int Update(int VendorProductTypeId, string Product_TypeName, string Product_TypeDescription, string Product_TypePublish)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductTypes_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorProductTypeId", VendorProductTypeId);
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

        public int Delete(int VendorProductTypeId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductTypes_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorProductTypeId", VendorProductTypeId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }     
    }
}
