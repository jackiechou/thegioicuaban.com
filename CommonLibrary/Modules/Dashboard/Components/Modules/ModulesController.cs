using System.Collections.Generic;
using System.Xml;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Modules.Dashboard.Components.Modules
{
	public class ModulesController : IDashboardData
	{
     
        public ModulesController()
        {
        }
        
        public static List<ModuleInfo> GetInstalledModules()
        {
            return CBO.FillCollection<ModuleInfo>(DataService.GetInstalledModules());
        }
        public void ExportData(XmlWriter writer)
        {
            writer.WriteStartElement("installedModules");
            foreach (ModuleInfo module in GetInstalledModules())
            {
                module.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
	}
}
