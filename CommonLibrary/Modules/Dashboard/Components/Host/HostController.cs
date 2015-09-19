using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules.Dashboard.Components.Host
{
    public class HostController : IDashboardData
    {
        public void ExportData(System.Xml.XmlWriter writer)
        {
            HostInfo host = new HostInfo();
            writer.WriteStartElement("host");
            host.WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}
