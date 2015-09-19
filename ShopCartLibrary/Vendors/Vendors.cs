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
    partial class Vendors
    {
        ModuleClass module_obj = new ModuleClass();
        string IP = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        public Vendors(){ }

        public int CheckVendorEmail(string Email)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendors_CheckEmail]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable GetListByPortalId(int PortalId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendors_GetListByPortalId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int VendorId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendors_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        //INSERT- UPDATE - DELETE --------------------------------------------------------------------------
        public int Insert(string CreatedByUserId, int PortalId, string Category_Code, string VendorName, string AddressLine1, string AddressLine2,
                        int CountryId, string PostalCode, string Cell, string SupportOnline,
                        string Hotline, string Telephone, string Fax, string Email,
                        string Website, string KeyWords, int Authorized, string StoreName, string StoreCompanyName, string TaxCode,
                        string LogoFile, string Slogan, string CurrencyCode, string CurrencySymbol,
                        string Decimals, string DecimalSymbol, string ThousandSeparator, string PositiveFormat,
                        string NegativeFormat, string TermsOfService, string Description,
                        int ContactTypeId, string ContactEmail, string PasswordSalt, string Title, string FirstName, string MiddleName, string LastName,
                      string Phone)
        {
            MD5CryptEncrypt md5_obj = new MD5CryptEncrypt();
            string PasswordHash = md5_obj.getMd5Hash(PasswordSalt);

            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendors_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@VendorName", VendorName);
            cmd.Parameters.AddWithValue("@AddressLine1", AddressLine1);
            cmd.Parameters.AddWithValue("@AddressLine2", AddressLine2);
            cmd.Parameters.AddWithValue("@CountryId", CountryId);
            cmd.Parameters.AddWithValue("@PostalCode", PostalCode);
            cmd.Parameters.AddWithValue("@Cell", Cell);
            cmd.Parameters.AddWithValue("@SupportOnline", SupportOnline);
            cmd.Parameters.AddWithValue("@Hotline", Hotline);
            cmd.Parameters.AddWithValue("@Telephone", Telephone);
            cmd.Parameters.AddWithValue("@Fax", Fax);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Website", Website);
            cmd.Parameters.AddWithValue("@KeyWords", KeyWords);
            cmd.Parameters.AddWithValue("@Authorized", Authorized);
            cmd.Parameters.AddWithValue("@StoreName", StoreName);
            cmd.Parameters.AddWithValue("@StoreCompanyName", StoreCompanyName);
            cmd.Parameters.AddWithValue("@TaxCode", TaxCode);
            cmd.Parameters.AddWithValue("@LogoFile", LogoFile);
            cmd.Parameters.AddWithValue("@Slogan", Slogan);
            cmd.Parameters.AddWithValue("@CurrencyCode", CurrencyCode);
            cmd.Parameters.AddWithValue("@CurrencySymbol", CurrencySymbol);
            cmd.Parameters.AddWithValue("@Decimals", Decimals);
            cmd.Parameters.AddWithValue("@DecimalSymbol", DecimalSymbol);
            cmd.Parameters.AddWithValue("@ThousandSeparator", ThousandSeparator);
            cmd.Parameters.AddWithValue("@PositiveFormat", PositiveFormat);
            cmd.Parameters.AddWithValue("@NegativeFormat", NegativeFormat);
            cmd.Parameters.AddWithValue("@TermsOfService", TermsOfService);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@CreatedIP", IP);
            cmd.Parameters.AddWithValue("@ContactTypeId", ContactTypeId);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@FirstName", FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", MiddleName);
            cmd.Parameters.AddWithValue("@LastName", LastName);
            cmd.Parameters.AddWithValue("@EmailAddress", Email);
            cmd.Parameters.AddWithValue("@Phone", Phone);
            cmd.Parameters.AddWithValue("@PasswordHash", PasswordHash);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);
            cmd.Parameters.AddWithValue("@CreatedByUserId", CreatedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int PortalId, int VendorId, string VendorName, int CountryId, string PostalCode, 
                        string Cell, string SupportOnline, string Hotline, string Telephone, string Fax,
                        string Email, string Website, string KeyWords, int Authorized, string StoreName,
                        string StoreCompanyName, string TaxCode, string LogoFile, string Slogan, string TermsOfService, 
                        string Description, string LastModifiedByUserId)
        { 
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendors_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@VendorName", VendorName);
            cmd.Parameters.AddWithValue("@CountryId", CountryId);
            cmd.Parameters.AddWithValue("@PostalCode", PostalCode);
            cmd.Parameters.AddWithValue("@Cell", Cell);
            cmd.Parameters.AddWithValue("@SupportOnline", SupportOnline);
            cmd.Parameters.AddWithValue("@Hotline", Hotline);
            cmd.Parameters.AddWithValue("@Telephone", Telephone);
            cmd.Parameters.AddWithValue("@Fax", Fax);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Website", Website);
            cmd.Parameters.AddWithValue("@KeyWords", KeyWords);
            cmd.Parameters.AddWithValue("@Authorized", Authorized);
            cmd.Parameters.AddWithValue("@StoreName", StoreName);
            cmd.Parameters.AddWithValue("@StoreCompanyName", StoreCompanyName);
            cmd.Parameters.AddWithValue("@TaxCode", TaxCode);
            cmd.Parameters.AddWithValue("@LogoFile", LogoFile);
            cmd.Parameters.AddWithValue("@Slogan", Slogan);
            cmd.Parameters.AddWithValue("@TermsOfService", TermsOfService);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@LastModifiedIP", IP);
            cmd.Parameters.AddWithValue("@LastModifiedByUserId", LastModifiedByUserId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update_ClickThroughs(int VendorId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendors_Update_ClickThroughs]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        //==================================================================================================
        
        public int InsertProductType(int VendorId, int ProductTypeId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_ProductTypes_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@ProductTypeId", ProductTypeId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        //=================================================================================================

        #region ORDER =====================================================================================
        public int DeletePromotion(int VendorId, int PromotionId)
        {
            SqlCommand cmd = new SqlCommand("[Purchasing].[Vendor_OrderPromotions_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@PromotionId", PromotionId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        #endregion ========================================================================================
    }
}
