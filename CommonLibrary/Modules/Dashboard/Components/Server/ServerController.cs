using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules.Dashboard.Components.Server
{
    public class ServerController : IDashboardData
    {
        public void ExportData(System.Xml.XmlWriter writer)
        {
            ServerInfo host = new ServerInfo();
            writer.WriteStartElement("server");
            host.WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}
