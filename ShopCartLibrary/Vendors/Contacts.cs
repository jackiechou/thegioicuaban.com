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
using CommonLibrary.Modules;

namespace ShopCartLibrary.Vendors
{
    public class Contacts
    {
        ModuleClass module_obj = new ModuleClass();
        string IP = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();    

        public Contacts(){}

        public int CheckContactEmail(string Email)
        {
            SqlCommand cmd = new SqlCommand("[Person].[Contact_CheckEmail]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Email", Email);
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("[Production].[Contact_GetAll]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int ContactId)
        {
            SqlCommand cmd = new SqlCommand("[Person].[Contact_GetDetailByID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ContactId", ContactId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByVendorId(int VendorId)
        {
            SqlCommand cmd = new SqlCommand("[Person].[Contact_GetListByVendorId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }


        public int Insert(int VendorId, int ContactTypeId, string Email, string PasswordSalt, string Title, string FirstName, string MiddleName, string LastName,
                      string Phone)
        {
            MD5CryptEncrypt md5_obj = new MD5CryptEncrypt();
            string PasswordHash = md5_obj.getMd5Hash(PasswordSalt);

            SqlCommand cmd = new SqlCommand("[Person].[Contact_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@ContactTypeId", ContactTypeId);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@FirstName", FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", MiddleName);
            cmd.Parameters.AddWithValue("@LastName", LastName);
            cmd.Parameters.AddWithValue("@EmailAddress", Email);
            cmd.Parameters.AddWithValue("@Phone", Phone);
            cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);            
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int ContactTypeId, int ContactId, string Email, string Title, string FirstName, string MiddleName, string LastName,
                      string Phone, string PasswordSalt)
        {
            MD5CryptEncrypt md5_obj = new MD5CryptEncrypt();
            string PasswordHash = md5_obj.getMd5Hash(PasswordSalt);

            SqlCommand cmd = new SqlCommand("[Person].[Contact_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };            
            cmd.Parameters.AddWithValue("@ContactTypeId", ContactTypeId);
            cmd.Parameters.AddWithValue("@ContactId", ContactId);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@FirstName", FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", MiddleName);
            cmd.Parameters.AddWithValue("@LastName", LastName);
            cmd.Parameters.AddWithValue("@EmailAddress", Email);
            cmd.Parameters.AddWithValue("@Phone", Phone);
            cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
