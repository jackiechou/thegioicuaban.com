using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using CommonLibrary;
using System.Data.SqlClient;

namespace ShopCartLibrary.Vendors
{
    public class VendorManufacturerCategories
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public VendorManufacturerCategories()
        {
            
        }

        public DataTable GetListByVendorIdDiscontinued(int VendorId, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_Manufacturer_Categories_GetListByVendorIdDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@Manufacturer_CategoryDiscontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int VendorManufacturerId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_Manufacturer_Categories_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorManufacturerId", VendorManufacturerId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int VendorId, string Manufacturer_CategoryName, string Manufacturer_CategoryDesc, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_Manufacturer_Categories_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@Manufacturer_CategoryName", Manufacturer_CategoryName);
            cmd.Parameters.AddWithValue("@Manufacturer_CategoryDesc", Manufacturer_CategoryDesc);
            cmd.Parameters.AddWithValue("@Manufacturer_CategoryDiscontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int VendorManufacturerId, string Manufacturer_CategoryName, string Manufacturer_CategoryDesc, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_Manufacturer_Categories_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorManufacturerId", VendorManufacturerId);
            cmd.Parameters.AddWithValue("@Manufacturer_CategoryName", Manufacturer_CategoryName);
            cmd.Parameters.AddWithValue("@Manufacturer_CategoryDesc", Manufacturer_CategoryDesc);
            cmd.Parameters.AddWithValue("@Manufacturer_CategoryDiscontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateDiscontinued(int Manufacturer_CategoryId, string Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_Manufacturer_Categories_UpdateDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorManufacturerId", Manufacturer_CategoryId);            
            cmd.Parameters.AddWithValue("@Manufacturer_CategoryDiscontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int VendorManufacturerId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_Manufacturer_Categories_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorManufacturerId", VendorManufacturerId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }      
    }
}
