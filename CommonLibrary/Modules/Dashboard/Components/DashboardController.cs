using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Modules.Dashboard.Components.Server;
using CommonLibrary.Framework;
using System.Xml;
using CommonLibrary.Common;
using System.IO;

namespace CommonLibrary.Modules.Dashboard.Components
{
    public class DashboardController
    {
        public static int AddDashboardControl(DashboardControl dashboardControl)
        {
            return DataService.AddDashboardControl(dashboardControl.PackageID, dashboardControl.DashboardControlKey, dashboardControl.IsEnabled, dashboardControl.DashboardControlSrc, dashboardControl.DashboardControlLocalResources, dashboardControl.ControllerClass, dashboardControl.ViewOrder);
        }
        public static void DeleteControl(DashboardControl dashboardControl)
        {
            DataService.DeleteDashboardControl(dashboardControl.DashboardControlID);
        }
        public static void Export(string filename)
        {
            string fullName = Path.Combine(Globals.HostMapPath, filename);
            XmlWriterSettings settings = new XmlWriterSettings();
            using (XmlWriter writer = XmlWriter.Create(fullName, settings))
            {
                writer.WriteStartElement("dashboard");
                foreach (DashboardControl dashboard in GetDashboardControls(true))
                {
                    IDashboardData controller = Activator.CreateInstance(Reflection.CreateType(dashboard.ControllerClass)) as IDashboardData;
                    if (controller != null)
                    {
                        controller.ExportData(writer);
                    }
                }
                writer.WriteEndElement();
                writer.Flush();
            }
        }
        public static DashboardControl GetDashboardControlByKey(string dashboardControlKey)
        {
            return CBO.FillObject<DashboardControl>(DataService.GetDashboardControlByKey(dashboardControlKey));
        }
        public static DashboardControl GetDashboardControlByPackageId(int packageId)
        {
            return CBO.FillObject<DashboardControl>(DataService.GetDashboardControlByPackageId(packageId));
        }
        public static List<DashboardControl> GetDashboardControls(bool isEnabled)
        {
            return CBO.FillCollection<DashboardControl>(DataService.GetDashboardControls(isEnabled));
        }
        public static void UpdateDashboardControl(DashboardControl dashboardControl)
        {
            DataService.UpdateDashboardControl(dashboardControl.DashboardControlID, dashboardControl.DashboardControlKey, dashboardControl.IsEnabled, dashboardControl.DashboardControlSrc, dashboardControl.DashboardControlLocalResources, dashboardControl.ControllerClass, dashboardControl.ViewOrder);
        }
    }
}
