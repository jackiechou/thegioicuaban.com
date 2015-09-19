using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Services;
using System.Net;

namespace WebApp.modules.admin.domain
{
    public class Whois : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string strDomain = context.Request.QueryString["domain"];
            string strExt = context.Request.QueryString["ext"];
            string strCommand = context.Request.QueryString["cmd"];          
            //string strCommand = "getwhois";
            
            string strHost = "http://www.pavietnam.vn/vn/whois.php?domain=" + strDomain + strExt + "&cmd=" + strCommand;
            try
            {
                context.Response.Write(GetWebContent(strHost));
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }
        }

        public string GetWebContent(string strLink)
        {
            string strContent = "";
            try
            {
                WebRequest objWebRequest = WebRequest.Create(strLink);
                objWebRequest.Credentials = CredentialCache.DefaultCredentials;

                WebResponse objWebResponse = objWebRequest.GetResponse();
                Stream receiveStream = objWebResponse.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);

                strContent = readStream.ReadToEnd();
                objWebResponse.Close();
                readStream.Close();
            }
            catch (Exception ex)
            {
                strContent = ex.Message;
            }
            return strContent;
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