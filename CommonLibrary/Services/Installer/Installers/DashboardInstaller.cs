using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Modules.Dashboard.Components;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Installers
{
    public class DashboardInstaller : ComponentInstallerBase
    {
        private DashboardControl TempDashboardControl;
        private string Key;
        private string Src;
        private string LocalResources;
        private string ControllerClass;
        private bool IsEnabled;
        private int ViewOrder;
        public override string AllowableFiles
        {
            get { return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html"; }
        }
        private void DeleteDashboard()
        {
            try
            {
                DashboardControl dashboardControl = DashboardController.GetDashboardControlByPackageId(Package.PackageID);
                if (dashboardControl != null)
                {
                    DashboardController.DeleteControl(dashboardControl);
                }
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.AUTHENTICATION_UnRegistered);
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void Commit()
        {
        }
        public override void Install()
        {
            bool bAdd = Null.NullBoolean;
            try
            {
                TempDashboardControl = DashboardController.GetDashboardControlByKey(Key);
                DashboardControl dashboardControl = new DashboardControl();
                if (TempDashboardControl == null)
                {
                    dashboardControl.IsEnabled = true;
                    bAdd = true;
                }
                else
                {
                    dashboardControl.DashboardControlID = TempDashboardControl.DashboardControlID;
                    dashboardControl.IsEnabled = TempDashboardControl.IsEnabled;
                }
                dashboardControl.DashboardControlKey = Key;
                dashboardControl.PackageID = Package.PackageID;
                dashboardControl.DashboardControlSrc = Src;
                dashboardControl.DashboardControlLocalResources = LocalResources;
                dashboardControl.ControllerClass = ControllerClass;
                dashboardControl.ViewOrder = ViewOrder;
                if (bAdd)
                {
                    DashboardController.AddDashboardControl(dashboardControl);
                }
                else
                {
                    DashboardController.UpdateDashboardControl(dashboardControl);
                }
                Completed = true;
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.DASHBOARD_Registered);
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void ReadManifest(XPathNavigator manifestNav)
        {
            Key = Util.ReadElement(manifestNav, "dashboardControl/key", Log, Util.DASHBOARD_KeyMissing);
            Src = Util.ReadElement(manifestNav, "dashboardControl/src", Log, Util.DASHBOARD_SrcMissing);
            LocalResources = Util.ReadElement(manifestNav, "dashboardControl/localResources", Log, Util.DASHBOARD_LocalResourcesMissing);
            ControllerClass = Util.ReadElement(manifestNav, "dashboardControl/controllerClass");
            IsEnabled = bool.Parse(Util.ReadElement(manifestNav, "dashboardControl/isEnabled", "true"));
            ViewOrder = int.Parse(Util.ReadElement(manifestNav, "dashboardControl/viewOrder", "-1"));
            if (Log.Valid)
            {
                Log.AddInfo(Util.DASHBOARD_ReadSuccess);
            }
        }
        public override void Rollback()
        {
            if (TempDashboardControl == null)
            {
                DeleteDashboard();
            }
            else
            {
                DashboardController.UpdateDashboardControl(TempDashboardControl);
            }
        }
        public override void UnInstall()
        {
            try
            {
                DashboardControl dashboardControl = DashboardController.GetDashboardControlByPackageId(Package.PackageID);
                if (dashboardControl != null)
                {
                    DashboardController.DeleteControl(dashboardControl);
                }
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.DASHBOARD_UnRegistered);
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
    }
}
