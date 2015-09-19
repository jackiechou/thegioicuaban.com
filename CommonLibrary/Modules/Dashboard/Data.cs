using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLibrary.Data;

namespace CommonLibrary.Modules.Dashboard
{
    public class DataService
    {
        private static DataProvider provider = DataProvider.Instance();
        private static string moduleQualifier = "Dashboard_";
        private static string GetFullyQualifiedName(string name)
        {
            return String.Concat(moduleQualifier, name);
        }
        public static int AddDashboardControl(int packageId, string dashboardControlKey, bool isEnabled, string dashboardControlSrc, string dashboardControlLocalResources, string controllerClass, int viewOrder)
        {
            return provider.ExecuteScalar<int>(GetFullyQualifiedName("AddControl"), packageId, dashboardControlKey, isEnabled, dashboardControlSrc, dashboardControlLocalResources, controllerClass, viewOrder);
        }
        public static void DeleteDashboardControl(int dashboardControlId)
        {
            provider.ExecuteNonQuery(GetFullyQualifiedName("DeleteControl"), dashboardControlId);
        }
        public static IDataReader GetDashboardControlByKey(string dashboardControlKey)
        {
            return provider.ExecuteReader(GetFullyQualifiedName("GetDashboardControlByKey"), dashboardControlKey);
        }
        public static IDataReader GetDashboardControlByPackageId(int packageId)
        {
            return provider.ExecuteReader(GetFullyQualifiedName("GetDashboardControlByPackageId"), packageId);
        }
        public static IDataReader GetDashboardControls(bool isEnabled)
        {
            return provider.ExecuteReader(GetFullyQualifiedName("GetControls"), isEnabled);
        }
        public static IDataReader GetDbInfo()
        {
            return provider.ExecuteReader(GetFullyQualifiedName("GetDbInfo"));
        }
        public static IDataReader GetDbFileInfo()
        {
            return provider.ExecuteReader(GetFullyQualifiedName("GetDbFileInfo"));
        }
        public static IDataReader GetDbBackups()
        {
            return provider.ExecuteReader(GetFullyQualifiedName("GetDbBackups"));
        }
        public static IDataReader GetInstalledModules()
        {
            return provider.ExecuteReader(GetFullyQualifiedName("GetInstalledModules"));
        }
        public static IDataReader GetPortals()
        {
            return provider.GetPortals();
        }
        //public static IDataReader GetPortals(string CultureCode)
        //{
        //    return provider.GetPortals(CultureCode);
        //}
        public static IDataReader GetServerErrors()
        {
            return provider.ExecuteReader(GetFullyQualifiedName("GetServerErrors"));
        }
        public static void UpdateDashboardControl(int dashboardControlId, string dashboardControlKey, bool isEnabled, string dashboardControlSrc, string dashboardControlLocalResources, string controllerClass, int viewOrder)
        {
            provider.ExecuteNonQuery(GetFullyQualifiedName("UpdateControl"), dashboardControlId, dashboardControlKey, isEnabled, dashboardControlSrc, dashboardControlLocalResources, controllerClass, viewOrder);
        }
    }
}
