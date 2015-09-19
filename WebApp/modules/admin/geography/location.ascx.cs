using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Net;
using System.Xml;

namespace WebApp.modules.admin.geography
{
    public partial class location : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string ipaddress;
                    string strHostName = System.Net.Dns.GetHostName();
                    string clientIPAddress = System.Net.Dns.GetHostAddresses(strHostName).GetValue(0).ToString();
                    string clientip = clientIPAddress.ToString();
                    System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://www.whatismyip.org");
                    request.UserAgent = "User-Agent: Mozilla/4.0 (compatible; MSIE" + "6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

                    System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

                    using (System.IO.StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        ipaddress = reader.ReadToEnd();
                        reader.Close();
                    }

                    lblip.Text = ipaddress.ToString();
                    DataTable dt = GetLocation(ipaddress.ToString());
                    lblcountry.Text = dt.Rows[0]["CountryName"].ToString();
                    lblregion.Text = dt.Rows[0]["RegionName"].ToString();
                    lblcity.Text = dt.Rows[0]["City"].ToString();
                    response.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable GetLocation(string ipaddress)
        {
            WebRequest rssReq = WebRequest.Create("http://freegeoip.appspot.com/xml/" + ipaddress); //Create a WebRequest
            WebProxy px = new WebProxy("http://freegeoip.appspot.com/xml/" + ipaddress, true);  //Create a Proxy
            rssReq.Proxy = px;      //Assign the proxy to the WebRequest
            rssReq.Timeout = 2000;  //Set the timeout in Seconds for the WebRequest

            try
            {
                WebResponse rep = rssReq.GetResponse(); //Get the WebResponse                
                XmlTextReader xtr = new XmlTextReader(rep.GetResponseStream()); //Read the Response in a XMLTextReader

                DataSet ds = new DataSet(); //Create a new DataSet                
                ds.ReadXml(xtr);            //Read the Response into the DataSet
                return ds.Tables[0];
            }
            catch
            {
                return null;
            }
        }
    }
}