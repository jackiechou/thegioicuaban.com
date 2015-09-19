using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AjaxControlToolkit;
using System.Data;
using System.Collections.Specialized;
using System.Web.Script.Services;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using CommonLibrary.UI.Skins;

namespace WebApp.services
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]

    public class vassws : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        //==============================================================================================================================
        //[WebMethod]
        //public string[] GetBranchAddressByBranchID(string Branch_ID)
        //{
        //    string[] array_list = new string[3];
        //    BranchClass branch_obj = new BranchClass();
        //    DataTable dt = branch_obj.GetDetails(Convert.ToInt32(Branch_ID));
        //    array_list[0] = dt.Rows[0]["Branch_Address"].ToString();
        //    array_list[1] = dt.Rows[0]["Branch_Place"].ToString();
        //    array_list[2] = dt.Rows[0]["Branch_HotLine"].ToString();           
        //    return array_list;
        //}


        //[WebMethod]
        //public string CheckYahoo()
        //{
        //    CompanyClass company_obj=new CompanyClass();
        //    string strYahooIDs = company_obj.GetSupportOnline(1);
        //    string result = string.Empty, yahoo_list = string.Empty;
        //    if(strYahooIDs!=null )
        //    {                
        //        string[] separator = new string[] { "," };
        //        string[] strSplitArr = strYahooIDs.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        //        foreach (string arrStr in strSplitArr)
        //        {
        //            result += "<li>" + CheckYahooID(arrStr) + "</li>";
        //        }
        //        yahoo_list = "<ul>" + result + "</ul>";
        //    }
        //    return yahoo_list;
        //}

        //public string CheckYahoo(string strYahooIDs)
        //{
        //    string result = string.Empty, yahoo_list = string.Empty;
        //    string[] separator = new string[] { "," };
        //    string[] strSplitArr = strYahooIDs.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        //    foreach (string arrStr in strSplitArr)
        //    {
        //        result += "<li>" + CheckYahooID(arrStr) + "</li>";
        //    }
        //    yahoo_list = "<ul>" + result + "</ul>";

        //    return yahoo_list;
        //}

        [WebMethod]
        public string CheckYahooID(string yahoo_id)
        {
            string result = string.Empty;
            try
            {
                string pattern = "http://opi.yahoo.com/online?u=" + yahoo_id + "&m=s&t=8";
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(pattern);
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                StreamReader objReader = new StreamReader(objResponse.GetResponseStream());
                string msgReturn = objReader.ReadToEnd();
                objReader.Close();
                objResponse.Close();
                msgReturn = msgReturn.ToLower().Trim();
                
                Uri requestUri = Context.Request.Url;
                string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);

                if (msgReturn.EndsWith("is online"))
                    result = "<a href='ymsgr:sendim?" + yahoo_id + "'><img src='" + baseUrl + "/images/icons/online.gif' alt='" + yahoo_id + " is online' border='0' /></a>";
                else
                    result = "<a href='ymsgr:sendim?" + yahoo_id + "'><img src='" + baseUrl + "/images/icons/offline.gif' alt='" + yahoo_id + " is offline' border='0' /></a>";
            }
            catch (WebException ex) { ex.ToString(); }
            return result;
        }



        //===========================================================================================================================================
         [WebMethod]
         public string[] GetBackgroundList()
         {
             SkinBackgrounds background_obj = new SkinBackgrounds();    
             string SkinType="0";
             int SkinPackageId=2;
             int SkinBackground_Discontinued =1;
             int Qty=6;

             DataTable dt = background_obj.GetListBySkinTypeSkinPackageIdDiscontinuedWithQty(SkinType, SkinPackageId, SkinBackground_Discontinued, Qty);

             System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();

             foreach (DataRow row in dt.Rows)
             {

                 list.Add(row["SkinBackground_FileName"].ToString());
             }

             return list.ToArray();

             //string[] strArray = new string[] { "s5_background1.jpg", "s5_background2.jpg", "s5_background3.jpg" };
             //return strArray;
         }

         //[WebMethod]
         [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
         public static string[] GetCompletionList(string prefixText, string contextKey)
         {
             // Create array of movies
             //string[] movies = { "Star Wars", "Star Trek", "Superman", "Memento", "Shrek", "Shrek II" };
             //// Return matching movies
             //return (from m in movies where m.StartsWith(prefixText, StringComparison.CurrentCultureIgnoreCase) select m).Take(count).ToArray();

             string sql;
             if (contextKey == "Sản phẩm")
                 sql = "Select ProductName from CMS_ProductDetail Where ProductName like @prefixText";
             else
                 sql = "Select Name from CMS_ProductParent Where Name like @prefixText";

             SqlDataAdapter da = new SqlDataAdapter(sql, ConfigurationManager.ConnectionStrings["ConString"].ConnectionString);
             da.SelectCommand.Parameters.Add("@prefixText", SqlDbType.VarChar, 50).Value = prefixText + "%";
             DataTable dt = new DataTable();
             da.Fill(dt);

             string[] items = new string[dt.Rows.Count];
             int i = 0;
             foreach (DataRow dr in dt.Rows)
             {
                 items.SetValue(dr[0].ToString(), i);
                 i++;
             }

             return items;
         }

         [System.Web.Services.WebMethod]
         [System.Web.Script.Services.ScriptMethod]
         public static string[] GetSuggestions(string prefixText)
         {
             return new string[] {
            prefixText + "@gmail.com",
            prefixText + "@yahoo.com",
            prefixText + "@yahoo.com.vn",
            prefixText + "@hotmail.com"            
        };
         }

        

        //===========================================================================================================================================
        private string RemoveExtraText(string value)
        {
            //var allowedChars = "01234567890.,";
            var allowedChars = "01234567890";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        private string FormatDecimal(string strInput)
        {
            System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
            nfi.CurrencyDecimalDigits = 0;
            //nfi.CurrencySymbol = "$";
            nfi.CurrencyDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalDigits = 2;
           
            string result = Decimal.Parse(strInput, nfi).ToString();
            //string result = String.Format(nfi, "{0:c}", strInput);
            return result;

            //const char point = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0];
            //string s = d.ToString();
            //// if there's no decimal point, there's nothing to trim
            //if (!s.Contains(point) == -1)
            //    return s;
            //// trim any trailing 0s, followed by the decimal point if necessary
            //return s.TrimEnd('0').TrimEnd(point);
        }
        
        #region reference =====================================================================================
        //[WebMethod]
        //public string[] GetInfoByVehicleModelID(string VehicleModel_ID)
        //{
        //    string[] array_list = new string[2];
        //    if (string.IsNullOrEmpty(VehicleModel_ID) == false)
        //    {
        //        int ModelID = Convert.ToInt32(VehicleModel_ID);
        //        VehicleModels vehicle_model_obj = new VehicleModels();
        //        array_list = vehicle_model_obj.GetInfoByVehicleModelID(ModelID);           
        //    }
        //    return array_list;
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public List<ProductDeduction> GetPublishedYearsByVehicleModelID(string VehicleModel_ID)
        //{
        //    List<VehicleModels> _list = new List<VehicleModels>();
        //    VehicleModels vehicle_model_obj = new VehicleModels();
        //    DataTable dt = vehicle_model_obj.GetPublishedListByCodeType(VehicleModel_ID);
        //    _list = (from DataRow row in dt.Rows
        //             select new ProductDeduction
        //             {
        //                 VehicleModel_PublishedYear = row["VehicleModel_PublishedYear"].ToString(),
        //                 VehicleModel_Price = row["VehicleModel_Price"].ToString()
        //             }).ToList();
        //    return _list;
        //}

        #endregion ============================================================================================

    }
}
