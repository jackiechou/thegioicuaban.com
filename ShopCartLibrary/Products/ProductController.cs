using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.IO;
using CommonLibrary;
using CommonLibrary.Modules;

namespace ShopCartLibrary.Products
{
    public partial class ProductController
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductController()
        {
        }

        public DataTable GetListByVendorIdTypeId(int VendorId, int TypeId)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByVendorIdTypeId]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@TypeId", TypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetCompletionList(string prefixText)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetCompletionList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@prefixText", prefixText);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }       

        public DataTable GetListByCodeDiscontinued(int VendorId, string CultureCode, string Category_Code, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByCodeDiscontinued]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        //public List<Product> GetListByPortalIdIsSecure(int VendorId, string CultureCode, string Category_Code, int Discontinued)
        //{
          
        //    //using (ProductEntities db = new ProductEntities(Settings.ConnectionString))
        //    //{                
        //    //    var query = from x in db.Products select x;
        //    //    if (Vendor_Id >= 0)
        //    //        query = query.Where(x => x.Vendor_Id == Vendor_Id);
        //    //    if (CultureCode != "")
        //    //        query = query.Where(x => x.CultureCode == CultureCode);
        //    //    if (Category_Code != "")
        //    //        query = query.Where(x => x.Category_Code == Category_Code);
        //    //    if (Discontinued >= 0)
        //    //        query = query.Where(x => x.Discontinued == Discontinued);
        //    //    List<Product> list = query.ToList();
        //    //    int count = list.Count;
        //    //    return list;
        //    //}
        //}

        public DataTable GetListByVendorIdCodeStatus(int VendorId,string Category_Code, string CultureCode, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByVendorIdCategoryCultureStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }


        public DataTable Products_GetListByVendor(int Num_Rows, int Vendor_Id)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByVendor]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Num_Rows", Num_Rows);
            cmd.Parameters.AddWithValue("@Vendor_Id", Vendor_Id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }
        
        public DataTable GetDetailByID(int Product_Id)
        {            
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetDetailByID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };            
            cmd.Parameters.AddWithValue("@Product_Id", Product_Id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByProductNo(string Product_No)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetDetailByProductNo]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByProductName(string Product_Name)
        {          
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetDetailByProductName]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_Name", Product_Name);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetQuantityById(int Product_Id)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetQuantity]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_Id", Product_Id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetPublishedListByCode(string Category_Code)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetPublishedListByCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }      

        public string[] GetFilesByID(int Product_Id)
        {
            string[] array_list = new string[3];
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetFileByID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_Id", Product_Id);
            cmd.Parameters.Add(new SqlParameter("@PhotoFileName", SqlDbType.NVarChar,int.MaxValue) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@ThumbnailPhotoFileName", SqlDbType.NVarChar, int.MaxValue) { Direction = ParameterDirection.Output });            
            con.Open();
            cmd.ExecuteNonQuery();
            array_list[0]  = cmd.Parameters["@PhotoFileName"].Value.ToString();
            array_list[1] = cmd.Parameters["@ThumbnailPhotoFileName"].Value.ToString();
            con.Close();
            return array_list;
        }       

        public DataSet Search_Product(string Keywords)
        {   
            SqlCommand cmd = new SqlCommand("[Production].[Products_Search]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Keywords", Keywords);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            con.Close();
            return ds;
        }

        public DataTable GetListByNumStatus(int Num_Rows, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByNumStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Num_Rows", Num_Rows);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByNumType(int Num_Rows, string Category_Code)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByNumType]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Num_Rows", Num_Rows);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByNumTypeStatus(int Num_Rows, int VendorId, string Category_Code, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByNumTypeStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Num_Rows", Num_Rows);
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByTypeStatus(string Category_Code, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByTypeStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByTime(string Category_Code, string SelectedDate)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByTime]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@SelectedDate", SelectedDate);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByConditions(string Begin_Date, string End_Date, int Discontinued, string Category_Code, int Product_TypeId)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_GetListByConditions]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Begin_Date", Begin_Date);
            cmd.Parameters.AddWithValue("@End_Date", End_Date);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Product_TypeId", Product_TypeId);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string[] Insert(string CultureCode, string Product_Code, string Category_Code, int Manufacturer_Id, int VendorId, 
                        int Product_TypeId, string Product_Name, string CurrencyCode, string NetPrice, string GrossPrice, int TaxRateId, 
                        int DiscountId, int UnitsInStock, int UnitsInAPackage, int UnitsInBox, string Unit,
                        decimal Weight, string UnitOfWeightMeasure, decimal Length, decimal Width, decimal Height,
                        string UnitOfDimensionMeasure, int MinPurchaseQty, int MaxPurchaseQty, string Availability,
                        string StartDate, string EndDate, string PhotoFileName, string ThumbnailPhotoFileName, string Url, 
                        string ShortDescription, string Specification,  int OnlineTransactionFlag, int Discontinued)
        {

            string Alias = ModuleClass.convertTitle2Link(Product_Name);
            NetPrice= StringHandleClass.RemoveExtraTextWithoutPointOrComma(NetPrice);
            GrossPrice = StringHandleClass.RemoveExtraTextWithoutPointOrComma(GrossPrice);

            SqlCommand cmd = new SqlCommand("[Production].[Products_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@Product_Code", Product_Code);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Manufacturer_Id", Manufacturer_Id);
            cmd.Parameters.AddWithValue("@Vendor_Id", VendorId);
            cmd.Parameters.AddWithValue("@Product_TypeId", Product_TypeId);
            cmd.Parameters.AddWithValue("@Product_Name", Product_Name);
            cmd.Parameters.AddWithValue("@Alias", Alias);
            cmd.Parameters.AddWithValue("@CurrencyCode", CurrencyCode);
            cmd.Parameters.AddWithValue("@NetPrice", NetPrice);
            cmd.Parameters.AddWithValue("@GrossPrice", GrossPrice);
            cmd.Parameters.AddWithValue("@TaxRateId", TaxRateId);
            cmd.Parameters.AddWithValue("@Discount_Id", DiscountId);
            cmd.Parameters.AddWithValue("@UnitsInStock", UnitsInStock);
            cmd.Parameters.AddWithValue("@UnitsInAPackage", UnitsInAPackage);
            cmd.Parameters.AddWithValue("@UnitsInBox", UnitsInBox);
            cmd.Parameters.AddWithValue("@Unit", Unit);
            cmd.Parameters.AddWithValue("@Weight", Weight);
            cmd.Parameters.AddWithValue("@UnitOfWeightMeasure", UnitOfWeightMeasure);
            cmd.Parameters.AddWithValue("@Length", Length);
            cmd.Parameters.AddWithValue("@Width", Width);
            cmd.Parameters.AddWithValue("@Height", Height);
            cmd.Parameters.AddWithValue("@UnitOfDimensionMeasure", UnitOfDimensionMeasure);
            cmd.Parameters.AddWithValue("@MinPurchaseQty", MinPurchaseQty);
            cmd.Parameters.AddWithValue("@MaxPurchaseQty", MaxPurchaseQty);
            cmd.Parameters.AddWithValue("@Availability", Availability);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@PhotoFileName", PhotoFileName);
            cmd.Parameters.AddWithValue("@ThumbnailPhotoFileName", ThumbnailPhotoFileName);
            cmd.Parameters.AddWithValue("@Url", Url);
            cmd.Parameters.AddWithValue("@ShortDescription", ShortDescription);
            cmd.Parameters.AddWithValue("@Specification", Specification);
            cmd.Parameters.AddWithValue("@OnlineTransactionFlag", OnlineTransactionFlag);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add("@o_return_Product_No", SqlDbType.NVarChar, int.MaxValue).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_return", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            con.Open();
            int i = cmd.ExecuteNonQuery();
            string[] arr_result = new string[2];
            arr_result[0] = cmd.Parameters["@o_return"].Value.ToString();
            arr_result[1] = cmd.Parameters["@o_return_Product_No"].Value.ToString();
            con.Close();
            return arr_result;
        }

        public int Update(int Product_Id, string CultureCode, string Product_Code, string Category_Code, int Manufacturer_Id, int VendorId,
                        int Product_TypeId, string Product_Name, string CurrencyCode, string NetPrice, string GrossPrice, int TaxRateId,
                        int DiscountId, int UnitsInStock, int UnitsInAPackage, int UnitsInBox, string Unit,
                        decimal Weight, string UnitOfWeightMeasure, decimal Length, decimal Width, decimal Height,
                        string UnitOfDimensionMeasure, int MinPurchaseQty, int MaxPurchaseQty, string Availability,
                        string StartDate, string EndDate, string PhotoFileName, string ThumbnailPhotoFileName, string Url,
                        string ShortDescription, string Specification, int OnlineTransactionFlag, int Discontinued)
        {
            string Alias = ModuleClass.convertTitle2Link(Product_Name);
            NetPrice = StringHandleClass.RemoveExtraTextWithoutPointOrComma(NetPrice);
            GrossPrice = StringHandleClass.RemoveExtraTextWithoutPointOrComma(GrossPrice);

            SqlCommand cmd = new SqlCommand("[Production].[Products_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_Id", Product_Id);
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@Product_Code", Product_Code);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Manufacturer_Id", Manufacturer_Id);
            cmd.Parameters.AddWithValue("@Vendor_Id", VendorId);
            cmd.Parameters.AddWithValue("@Product_TypeId", Product_TypeId);
            cmd.Parameters.AddWithValue("@Product_Name", Product_Name);
            cmd.Parameters.AddWithValue("@Alias", Alias);
            cmd.Parameters.AddWithValue("@CurrencyCode", CurrencyCode);
            cmd.Parameters.AddWithValue("@NetPrice", NetPrice);
            cmd.Parameters.AddWithValue("@GrossPrice", GrossPrice);
            cmd.Parameters.AddWithValue("@TaxRateId", TaxRateId);
            cmd.Parameters.AddWithValue("@Discount_Id", DiscountId);
            cmd.Parameters.AddWithValue("@UnitsInStock", UnitsInStock);
            cmd.Parameters.AddWithValue("@UnitsInAPackage", UnitsInAPackage);
            cmd.Parameters.AddWithValue("@UnitsInBox", UnitsInBox);
            cmd.Parameters.AddWithValue("@Unit", Unit);
            cmd.Parameters.AddWithValue("@Weight", Weight);
            cmd.Parameters.AddWithValue("@UnitOfWeightMeasure", UnitOfWeightMeasure);
            cmd.Parameters.AddWithValue("@Length", Length);
            cmd.Parameters.AddWithValue("@Width", Width);
            cmd.Parameters.AddWithValue("@Height", Height);
            cmd.Parameters.AddWithValue("@UnitOfDimensionMeasure", UnitOfDimensionMeasure);
            cmd.Parameters.AddWithValue("@MinPurchaseQty", MinPurchaseQty);
            cmd.Parameters.AddWithValue("@MaxPurchaseQty", MaxPurchaseQty);
            cmd.Parameters.AddWithValue("@Availability", Availability);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@PhotoFileName", PhotoFileName);
            cmd.Parameters.AddWithValue("@ThumbnailPhotoFileName", ThumbnailPhotoFileName);
            cmd.Parameters.AddWithValue("@Url", Url);
            cmd.Parameters.AddWithValue("@ShortDescription", ShortDescription);
            cmd.Parameters.AddWithValue("@Specification", Specification);
            cmd.Parameters.AddWithValue("@OnlineTransactionFlag", OnlineTransactionFlag);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);  
            cmd.Parameters.Add("@o_return", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int Product_Id, string front_dir_path,string main_dir_path)
        {
            string[] array_list = new string[3];
            array_list = GetFilesByID(Product_Id);
            string PhotoFileName =string.Empty, ThumbnailPhotoFileName =string.Empty;
            if (array_list.Length > 0)
            {
                PhotoFileName = array_list[0].ToString();
                ThumbnailPhotoFileName = array_list[1].ToString();
            }
            string front_file_path = HttpContext.Current.Server.MapPath(front_dir_path) + "/" + PhotoFileName;
            string main_file_path = HttpContext.Current.Server.MapPath(main_dir_path) + "/" + PhotoFileName;
            FileHandleClass file_handle = new FileHandleClass();
            if (System.IO.File.Exists(front_file_path))
                File.Delete(front_file_path);
            if (System.IO.File.Exists(main_file_path))
                File.Delete(main_file_path);

            SqlCommand cmd = new SqlCommand("[Production].[Products_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_Id", Product_Id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, 2) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateUnitsOnOrder(int Product_Id, int UnitsOnOrder)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_UpdateUnitsOnOrder]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_Id", Product_Id);
            cmd.Parameters.AddWithValue("@UnitsOnOrder", UnitsOnOrder);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, 2) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(int Product_Id, int Discontinued)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Products_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_Id", Product_Id);
            cmd.Parameters.AddWithValue("@Discontinued", Discontinued);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int, 2) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        private string RemoveExtraText(string value)
        {
            //var allowedChars = "01234567890.,";
            var allowedChars = "01234567890";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }
}
