using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonLibrary.Ajax.CallbackHandler;
using System.Drawing;
using ArticleLibrary;
using System.Data;
using CommonLibrary.Services.Banner;

namespace WebApp.handlers
{
    /// <summary>
    /// Summary description for RestService
    /// </summary>
    public class RestService : CallbackHandler
    {
        //[CallbackMethod(AllowedHttpVerbs = HttpVerbs.GET | HttpVerbs.POST)]
        [CallbackMethod]
        public string HelloWorld(string name)
        {
            return "Hello " + name + "! Time is: " + DateTime.Now;
        }

        [CallbackMethod]
        public string LoadBannerList()
        {
            BannerController banner_obj = new BannerController();            
            int num_rows = 3; int position = 1; string status = "1"; string result = string.Empty, title = string.Empty, FileName = string.Empty;
            DataTable dt = banner_obj.GetListByNumPosition(num_rows, position, status);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FileName = dt.Rows[i]["MainImage"].ToString();
                    Uri requestUri = Context.Request.Url;
                    string baseUrl = requestUri.Scheme + Uri.SchemeDelimiter + requestUri.Host + (requestUri.IsDefaultPort ? "" : ":" + requestUri.Port);
                    if (i < dt.Rows.Count - 1)
                        result += baseUrl + "/user_files/images/banner_images/main_images/" + FileName + ",";
                    else
                        result += baseUrl + "/user_files/images/banner_images/main_images/" + FileName;
                }
            }
            else
                result = "Không có dữ liệu";
            return result;
        }



        [CallbackMethod(RouteUrl = "rest-service/{message}")]
        public string GetData(string message)
        {
            string result = "Hello " + message;
            return message;
        }

        [CallbackMethod(ContentType = "image/png")]
        public Bitmap GetImage(string imageFile)
        {
            Bitmap img = new Bitmap(HttpContext.Current.Server.MapPath("~/images/" + imageFile));
            return img;
        }

      


        //[CallbackMethod(RouteUrl = "RestService/StockQuote/{symbol}")]
        //public StockQuote GetStockQuote(string symbol)
        //{
        //    StockServer server = new StockServer();
        //    return server.GetStockQuote(symbol);
        //}
        //[CallbackMethod(RouteUrl = "RestService/StockQuotes/{symbolList}")]
        //public StockQuote[] GetStockQuotes(string symbolList)
        //{
        //    StockServer server = new StockServer();
        //    string[] symbols = symbolList.Split(new char[2] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        //    return server.GetStockQuotes(symbols);
        //}

        //[CallbackMethod(ContentType = "image/png")]
        //public byte[] GetStockHistoryGraph(string symbol)
        //{
        //    StockServer server = new StockServer();
        //    byte[] img = server.GetStockHistoryGraph(new[] { symbol },
        //                                                "History for " + symbol.ToUpper(),
        //                                                450, 300, 2);

        //    return img;
        //}

        //[CallbackMethod(ContentType = "image/png")]
        //public Stream GetStockHistoryGraph(string symbol)
        //{
        //    StockServer server = new StockServer();
        //    byte[] img = server.GetStockHistoryGraph(new[] { symbol },
        //                                                "History for " + symbol.ToUpper(),
        //                                                450, 300, 2);

        //    MemoryStream ms = new MemoryStream(img);
        //    return ms;
        //}

        //[CallbackMethod(ReturnAsRawString = true, ContentType = "text/plain; charset=UTF-8")]
        //public string GetCities()
        //{
        //    var searchFor = HttpContext.Current.Request.QueryString["q"];

        //    busCustomer customer = new busCustomer();
        //    var res = (from cust in customer.Context.nw_Customers
        //               where cust.City.Contains(searchFor)
        //               orderby cust.City
        //               select new { City = cust.City, cust.CustomerID }).Distinct();

        //    StringBuilder sb = new StringBuilder();
        //    foreach (var city in res)
        //    {
        //        sb.AppendLine(city.City + "|" + city.CustomerID);
        //    }

        //    return sb.ToString();
        //}
    }
}