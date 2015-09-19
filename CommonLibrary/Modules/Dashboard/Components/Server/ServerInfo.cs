using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common;
using System.Web;
using System.Net;
using System.Xml.Serialization;

namespace CommonLibrary.Modules.Dashboard.Components.Server
{
    [XmlRoot("serverInfo")]
    public class ServerInfo : IXmlSerializable
    {
        public string Framework
        {
            get { return System.Environment.Version.ToString(); }
        }
        public string HostName
        {
            get { return Dns.GetHostName(); }
        }
        public string Identity
        {
            get { return System.Security.Principal.WindowsIdentity.GetCurrent().Name; }
        }
        public string IISVersion
        {
            get { return HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"]; }
        }
        public string OSVersion
        {
            get { return System.Environment.OSVersion.ToString(); }
        }
        public string PhysicalPath
        {
            get { return Globals.ApplicationMapPath; }
        }
        public string Url
        {
            get { return Globals.GetDomainName(HttpContext.Current.Request); }
        }
        public string RelativePath
        {
            get
            {
                string path;
                if (string.IsNullOrEmpty(Globals.ApplicationPath))
                {
                    path = "/";
                }
                else
                {
                    path = Globals.ApplicationPath;
                }
                return path;
            }
        }
        public string ServerTime
        {
            get { return System.DateTime.Now.ToString(); }
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("osVersion", OSVersion);
            writer.WriteElementString("iisVersion", IISVersion);
            writer.WriteElementString("framework", Framework);
            writer.WriteElementString("identity", Identity);
            writer.WriteElementString("hostName", HostName);
            writer.WriteElementString("physicalPath", PhysicalPath);
            writer.WriteElementString("url", Url);
            writer.WriteElementString("relativePath", RelativePath);
        }
    }
}
