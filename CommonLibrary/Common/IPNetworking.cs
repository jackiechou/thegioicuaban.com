using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Data;
using System.Xml;
using System.IO;

namespace CommonLibrary.Common
{
    public class IPNetworking
    {
        public IPNetworking() { }

        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            //foreach (System.Net.IPAddress IPA in System.Net.Dns.GetHostAddresses(System.Web.HttpContext.Current.Request.UserHostAddress))
            //{
            //    if (IPA.AddressFamily.ToString() == "InterNetwork")
            //    {
            //        IP4Address = IPA.ToString();
            //        break;
            //    }
            //}

            //if (IP4Address != String.Empty)
            //{
            //    return IP4Address;
            //}

            foreach (System.Net.IPAddress IPA in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        //Get Lan Connected IP address method
        public static string GetLanIPAddress()
        {
            string hostName = Dns.GetHostName(); //Get the Host Name
            IPHostEntry ipHostEntries = Dns.GetHostEntry(hostName); //Get The Ip Host Entry
            IPAddress[] arrIpAddress = ipHostEntries.AddressList;
            //string localIp = ipHostEntry.AddressList[1].ToString();
            string localIp = arrIpAddress[arrIpAddress.Length - 1].ToString();
            return localIp;
        }

        public static string GetRemoteIPAddress(){
            string remoteIp = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(remoteIp))
            {
                remoteIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return remoteIp;
        }

        public string GetIPFromDomainName(string domain_name)
        {
            System.Net.WebClient myWebClient = new System.Net.WebClient();
            System.IO.Stream myStream = myWebClient.OpenRead(domain_name);
            System.IO.StreamReader myStreamReader = new System.IO.StreamReader(myStream);
            string ip = myStreamReader.ReadToEnd();
            return ip;
        }
    }
}
