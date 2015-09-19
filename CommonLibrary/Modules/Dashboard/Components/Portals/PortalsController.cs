using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Modules.Dashboard.Components.Portals
{
    public class PortalsController : IDashboardData
    {
        public PortalsController(){}

        public static List<PortalInfo> GetPortals()
        {
            return CBO.FillCollection<PortalInfo>(DataService.GetPortals());
        }
        public void ExportData(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("portals");
            foreach (PortalInfo portal in GetPortals())
            {
                portal.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}
