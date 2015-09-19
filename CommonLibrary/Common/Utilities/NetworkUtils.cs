using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace CommonLibrary.Common.Utilities
{
    public static class NetworkUtils
    {       
        /// <summary>
        /// Retrieves a base domain name from a full domain name.
        /// For example: www.west-wind.com produces west-wind.com
        /// </summary>
        /// <param name="domainName">Dns Domain name as a string</param>
        /// <returns></returns>
        public static string GetBaseDomain(string domainName)
        {
            var tokens = domainName.Split('.');

            // only split 3 urls like www.west-wind.com
            if (tokens == null || tokens.Length != 3)
                return domainName;

            var tok = new List<string>(tokens);
            var remove = tokens.Length - 2;
            tok.RemoveRange(0, remove);

            return tok[0] + "." + tok[1]; ;
        }

        /// <summary>
        /// Returns the base domain from a domain name
        /// Example: http://www.west-wind.com returns west-wind.com
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetBaseDomain(this Uri uri)
        {
            if (uri.HostNameType == UriHostNameType.Dns)
                return GetBaseDomain(uri.DnsSafeHost);

            return uri.Host;
        }

        //string strHostName = Dns.GetHostName();      
        //string scheme = Request.Url.Scheme; // will get http, https, etc.
        //string host = Request.Url.Host; // will get www.mywebsite.com
        //string port = Request.Url.Port; // will get the port
        //string path = Request.Url.AbsolutePath; // should get the /pages/page1.aspx part, can't remember if it only get pages/page1.aspx
        //public static string GetHost()
        //{
        //    return "http://" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
        //}


        public static string GetAddress(AddressType AddressFormat)
        {
            string Host = Dns.GetHostName();  
            string IPAddress = string.Empty;
            System.Net.Sockets.AddressFamily addrFamily = System.Net.Sockets.AddressFamily.InterNetwork;
            switch (AddressFormat)
            {
                case AddressType.IPv4:
                    addrFamily = System.Net.Sockets.AddressFamily.InterNetwork;
                    break;
                case AddressType.IPv6:
                    addrFamily = System.Net.Sockets.AddressFamily.InterNetworkV6;
                    break;
            }
            IPHostEntry IPE = Dns.GetHostEntry(Host);
            if (Host != IPE.HostName)
            {
                IPE = Dns.GetHostEntry(IPE.HostName);
            }
            foreach (IPAddress IPA in IPE.AddressList)
            {
                if (IPA.AddressFamily == addrFamily)
                {
                    return IPA.ToString();
                }
            }
            return string.Empty;
        }    

        public static string GetAddress(string Host, AddressType AddressFormat)
        {
            string IPAddress = string.Empty;
            System.Net.Sockets.AddressFamily addrFamily = System.Net.Sockets.AddressFamily.InterNetwork;
            switch (AddressFormat)
            {
                case AddressType.IPv4:
                    addrFamily = System.Net.Sockets.AddressFamily.InterNetwork;
                    break;
                case AddressType.IPv6:
                    addrFamily = System.Net.Sockets.AddressFamily.InterNetworkV6;
                    break;
            }
            IPHostEntry IPE = Dns.GetHostEntry(Host);
            if (Host != IPE.HostName)
            {
                IPE = Dns.GetHostEntry(IPE.HostName);
            }
            foreach (IPAddress IPA in IPE.AddressList)
            {
                if (IPA.AddressFamily == addrFamily)
                {
                    return IPA.ToString();
                }
            }
            return string.Empty;
        }
    }
    public enum AddressType
    {
        IPv4,
        IPv6
    }
}
