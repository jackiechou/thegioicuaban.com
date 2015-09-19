using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace WebApp.handlers
{
    /// <summary>
    /// Summary description for CheckYahooHandler
    /// </summary>
    public class CheckYahooHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");

            
        }

        public string CheckYahooID(string yahoo_id)
        {
            string pattern = "http://opi.yahoo.com/online?u=" + yahoo_id + "&m=s&t=8";
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(pattern);
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            StreamReader objReader = new StreamReader(objResponse.GetResponseStream());
            string msgReturn = objReader.ReadToEnd();
            objReader.Close();
            objResponse.Close();
            msgReturn = msgReturn.ToLower().Trim();
            string result = string.Empty;

            if (msgReturn.EndsWith("is online"))
            {
                result = "<li><a href='ymsgr:sendim?" + yahoo_id + "'><img src='images/icons/online.gif' alt='" + yahoo_id + " is online' border='0' /></a></li>";
            }
            else
            {
                result = "<li><a href='ymsgr:sendim?" + yahoo_id + "'><img src='images/icons/offline.gif' alt='" + yahoo_id + " is offline' border='0' /></a></li>";
            }
            return result;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}