using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Data;
using System.Data;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class DesktopModulePermissionController
    {
        private static DataProvider provider = DataProvider.Instance();
        private static void ClearPermissionCache()
        {
            DataCache.ClearDesktopModulePermissionsCache();
        }
        private static Dictionary<int, DesktopModulePermissionCollection> FillDesktopModulePermissionDictionary(IDataReader dr)
        {
            Dictionary<int, DesktopModulePermissionCollection> dic = new Dictionary<int, DesktopModulePermissionCollection>();
            try
            {
                DesktopModulePermissionInfo obj;
                while (dr.Read())
                {
                    obj = CBO.FillObject<DesktopModulePermissionInfo>(dr, false);
                    if (dic.ContainsKey(obj.PortalDesktopModuleID))
                    {
                        dic[obj.PortalDesktopModuleID].Add(obj);
                    }
                    else
                    {
                        DesktopModulePermissionCollection collection = new DesktopModulePermissionCollection();
                        collection.Add(obj);
                        dic.Add(obj.PortalDesktopModuleID, collection);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return dic;
        }
        private static Dictionary<int, DesktopModulePermissionCollection> GetDesktopModulePermissions()
        {
            return CBO.GetCachedObject<Dictionary<int, DesktopModulePermissionCollection>>(new CacheItemArgs(DataCache.DesktopModulePermissionCacheKey, DataCache.DesktopModulePermissionCachePriority), GetDesktopModulePermissionsCallBack);
        }
        private static object GetDesktopModulePermissionsCallBack(CacheItemArgs cacheItemArgs)
        {
            return FillDesktopModulePermissionDictionary(provider.GetDesktopModulePermissions());
        }
        public static int AddDesktopModulePermission(DesktopModulePermissionInfo objDesktopModulePermission)
        {
            int Id = provider.AddDesktopModulePermission(objDesktopModulePermission.PortalDesktopModuleID, objDesktopModulePermission.PermissionID, objDesktopModulePermission.RoleID, objDesktopModulePermission.AllowAccess, objDesktopModulePermission.UserID, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objDesktopModulePermission, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULEPERMISSION_CREATED);
            ClearPermissionCache();
            return Id;
        }
        public static void DeleteDesktopModulePermission(int DesktopModulePermissionID)
        {
            provider.DeleteDesktopModulePermission(DesktopModulePermissionID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("DesktopModulePermissionID", DesktopModulePermissionID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULEPERMISSION_DELETED);
            ClearPermissionCache();
        }
        public static void DeleteDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID)
        {
            provider.DeleteDesktopModulePermissionsByPortalDesktopModuleID(portalDesktopModuleID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("PortalDesktopModuleID", portalDesktopModuleID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_DELETED);
            ClearPermissionCache();
        }
        public static void DeleteDesktopModulePermissionsByUserID(UserInfo objUser)
        {
            provider.DeleteDesktopModulePermissionsByUserID(objUser.UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("UserID", objUser.UserID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_DELETED);
            ClearPermissionCache();
        }
        public static DesktopModulePermissionInfo GetDesktopModulePermission(int DesktopModulePermissionID)
        {
            return CBO.FillObject<DesktopModulePermissionInfo>(provider.GetDesktopModulePermission(DesktopModulePermissionID), true);
        }
        public static DesktopModulePermissionCollection GetDesktopModulePermissions(int portalDesktopModuleID)
        {
            bool bFound = false;
            Dictionary<int, DesktopModulePermissionCollection> dicDesktopModulePermissions = GetDesktopModulePermissions();
            DesktopModulePermissionCollection DesktopModulePermissions = null;
            bFound = dicDesktopModulePermissions.TryGetValue(portalDesktopModuleID, out DesktopModulePermissions);
            if (!bFound)
            {
                DesktopModulePermissions = new DesktopModulePermissionCollection(CBO.FillCollection(provider.GetDesktopModulePermissionsByPortalDesktopModuleID(portalDesktopModuleID), typeof(DesktopModulePermissionInfo)), portalDesktopModuleID);
            }
            return DesktopModulePermissions;
        }
        public static bool HasDesktopModulePermission(DesktopModulePermissionCollection objDesktopModulePermissions, string permissionKey)
        {
            return PortalSecurity.IsInRoles(objDesktopModulePermissions.ToString(permissionKey));
        }
        public static void UpdateDesktopModulePermission(DesktopModulePermissionInfo objDesktopModulePermission)
        {
            provider.UpdateDesktopModulePermission(objDesktopModulePermission.DesktopModulePermissionID, objDesktopModulePermission.PortalDesktopModuleID, objDesktopModulePermission.PermissionID, objDesktopModulePermission.RoleID, objDesktopModulePermission.AllowAccess, objDesktopModulePermission.UserID, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objDesktopModulePermission, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULEPERMISSION_UPDATED);
            ClearPermissionCache();
        }
    }
}
