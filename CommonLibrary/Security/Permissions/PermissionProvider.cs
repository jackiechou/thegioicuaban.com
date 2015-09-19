using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Services.FileSystem;
using System.Data;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Data;
using CommonLibrary.ComponentModel;
using System.Data.SqlClient;

namespace CommonLibrary.Security.Permissions
{
    public class PermissionProvider
    {
        private const string AdminFolderPermissionCode = "WRITE";
        private const string AddFolderPermissionCode = "WRITE";
        private const string CopyFolderPermissionCode = "WRITE";
        private const string DeleteFolderPermissionCode = "WRITE";
        private const string ManageFolderPermissionCode = "WRITE";
        private const string ViewFolderPermissionCode = "READ";
        private const string AdminModulePermissionCode = "EDIT";
        private const string ContentModulePermissionCode = "EDIT";
        private const string DeleteModulePermissionCode = "EDIT";
        private const string ExportModulePermissionCode = "EDIT";
        private const string ImportModulePermissionCode = "EDIT";
        private const string ManageModulePermissionCode = "EDIT";
        private const string ViewModulePermissionCode = "VIEW";
        private const string AddPagePermissionCode = "EDIT";
        private const string AdminPagePermissionCode = "EDIT";
        private const string ContentPagePermissionCode = "EDIT";
        private const string CopyPagePermissionCode = "EDIT";
        private const string DeletePagePermissionCode = "EDIT";
        private const string ExportPagePermissionCode = "EDIT";
        private const string ImportPagePermissionCode = "EDIT";
        private const string ManagePagePermissionCode = "EDIT";
        private const string NavigatePagePermissionCode = "VIEW";
        private const string ViewPagePermissionCode = "VIEW";
        private DataProvider dataProvider = Data.DataProvider.Instance();

       
        public static PermissionProvider Instance()
        {
            return ComponentFactory.GetComponent<PermissionProvider>();
        }
        public virtual string LocalResourceFile
        {
            get { return Services.Localization.Localization.GlobalResourceFile; }
        }
        private object GetFolderPermissionsCallBack(CacheItemArgs cacheItemArgs)
        {
            int PortalID = (int)cacheItemArgs.ParamList[0];
            IDataReader dr = dataProvider.GetFolderPermissionsByPortal(PortalID);
            Dictionary<string, FolderPermissionCollection> dic = new Dictionary<string, FolderPermissionCollection>();
            try
            {
                FolderPermissionInfo obj;
                while (dr.Read())
                {
                    obj = CBO.FillObject<FolderPermissionInfo>(dr, false);
                    string dictionaryKey = obj.FolderPath;
                    if (string.IsNullOrEmpty(dictionaryKey))
                    {
                        dictionaryKey = "[PortalRoot]";
                    }
                    if (dic.ContainsKey(dictionaryKey))
                    {
                        dic[dictionaryKey].Add(obj);
                    }
                    else
                    {
                        FolderPermissionCollection collection = new FolderPermissionCollection();
                        collection.Add(obj);
                        dic.Add(dictionaryKey, collection);
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
        private Dictionary<string, FolderPermissionCollection> GetFolderPermissions(int PortalID)
        {
            string cacheKey = string.Format(DataCache.FolderPermissionCacheKey, PortalID.ToString());
            return CBO.GetCachedObject<Dictionary<string, FolderPermissionCollection>>(new CacheItemArgs(cacheKey, DataCache.FolderPermissionCacheTimeOut, DataCache.FolderPermissionCachePriority, PortalID), GetFolderPermissionsCallBack);
        }
        private Dictionary<int, ModulePermissionCollection> GetModulePermissions(int tabID)
        {
            string cacheKey = string.Format(DataCache.ModulePermissionCacheKey, tabID.ToString());
            return CBO.GetCachedObject<Dictionary<int, ModulePermissionCollection>>(new CacheItemArgs(cacheKey, DataCache.ModulePermissionCacheTimeOut, DataCache.ModulePermissionCachePriority, tabID), GetModulePermissionsCallBack);
        }
        private object GetModulePermissionsCallBack(CacheItemArgs cacheItemArgs)
        {
            int tabID = (int)cacheItemArgs.ParamList[0];
            IDataReader dr = dataProvider.GetModulePermissionsByTabID(tabID);
            Dictionary<int, ModulePermissionCollection> dic = new Dictionary<int, ModulePermissionCollection>();
            try
            {
                ModulePermissionInfo obj;
                while (dr.Read())
                {
                    obj = CBO.FillObject<ModulePermissionInfo>(dr, false);
                    if (dic.ContainsKey(obj.ModuleID))
                    {
                        dic[obj.ModuleID].Add(obj);
                    }
                    else
                    {
                        ModulePermissionCollection collection = new ModulePermissionCollection();
                        collection.Add(obj);
                        dic.Add(obj.ModuleID, collection);
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
        private Dictionary<int, TabPermissionCollection> GetTabPermissions(int portalID)
        {
            string cacheKey = string.Format(DataCache.TabPermissionCacheKey, portalID.ToString());
            return CBO.GetCachedObject<Dictionary<int, TabPermissionCollection>>(new CacheItemArgs(cacheKey, DataCache.TabPermissionCacheTimeOut, DataCache.TabPermissionCachePriority, portalID), GetTabPermissionsCallBack);
        }
        private object GetTabPermissionsCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            IDataReader dr = dataProvider.GetTabPermissionsByPortal(portalID);
            Dictionary<int, TabPermissionCollection> dic = new Dictionary<int, TabPermissionCollection>();
            try
            {
                TabPermissionInfo obj;
                while (dr.Read())
                {
                    obj = CBO.FillObject<TabPermissionInfo>(dr, false);
                    if (dic.ContainsKey(obj.TabID))
                    {
                        dic[obj.TabID].Add(obj);
                    }
                    else
                    {
                        TabPermissionCollection collection = new TabPermissionCollection();
                        collection.Add(obj);
                        dic.Add(obj.TabID, collection);
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
        public virtual bool CanAdminFolder(FolderInfo folder)
        {
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(AdminFolderPermissionCode));
        }
        public virtual bool CanAddFolder(FolderInfo folder)
        {
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(AddFolderPermissionCode));
        }
        public virtual bool CanCopyFolder(FolderInfo folder)
        {
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(CopyFolderPermissionCode));
        }
        public virtual bool CanDeleteFolder(FolderInfo folder)
        {
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(DeleteFolderPermissionCode));
        }
        public virtual bool CanManageFolder(FolderInfo folder)
        {
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(ManageFolderPermissionCode));
        }
        public virtual bool CanViewFolder(FolderInfo folder)
        {
            return PortalSecurity.IsInRoles(folder.FolderPermissions.ToString(ViewFolderPermissionCode));
        }
        public virtual void DeleteFolderPermissionsByUser(UserInfo objUser)
        {
            dataProvider.DeleteFolderPermissionsByUserID(objUser.PortalID, objUser.UserID);
        }
        public virtual FolderPermissionCollection GetFolderPermissionsCollectionByFolder(int PortalID, string Folder)
        {
            bool bFound = false;
            string dictionaryKey = Folder;
            if (string.IsNullOrEmpty(dictionaryKey))
            {
                dictionaryKey = "[PortalRoot]";
            }
            Dictionary<string, FolderPermissionCollection> dicFolderPermissions = GetFolderPermissions(PortalID);
            FolderPermissionCollection folderPermissions = null;
            bFound = dicFolderPermissions.TryGetValue(dictionaryKey, out folderPermissions);
            if (!bFound)
            {
                folderPermissions = new FolderPermissionCollection(CBO.FillCollection(dataProvider.GetFolderPermissionsByFolderPath(PortalID, Folder, -1), typeof(FolderPermissionInfo)), Folder);
            }
            return folderPermissions;
        }
        public virtual bool HasFolderPermission(FolderPermissionCollection objFolderPermissions, string PermissionKey)
        {
            return PortalSecurity.IsInRoles(objFolderPermissions.ToString(PermissionKey));
        }
        public virtual void SaveFolderPermissions(FolderInfo folder)
        {
            if (folder.FolderPermissions != null)
            {
                FolderPermissionCollection folderPermissions = GetFolderPermissionsCollectionByFolder(folder.PortalID, folder.FolderPath);
                if (!folderPermissions.CompareTo(folder.FolderPermissions))
                {
                    dataProvider.DeleteFolderPermissionsByFolderPath(folder.PortalID, folder.FolderPath);
                    foreach (FolderPermissionInfo folderPermission in folder.FolderPermissions)
                    {
                        dataProvider.AddFolderPermission(folder.FolderID, folderPermission.PermissionID, folderPermission.RoleID, folderPermission.AllowAccess, folderPermission.UserID, UserController.GetCurrentUserInfo().UserID);
                    }
                }
            }
        }
        public virtual bool CanAdminModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(AdminModulePermissionCode));
        }
        public virtual bool CanDeleteModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(DeleteModulePermissionCode));
        }
        public virtual bool CanEditModuleContent(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ContentModulePermissionCode));
        }
        public virtual bool CanExportModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ExportModulePermissionCode));
        }
        public virtual bool CanImportModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ImportModulePermissionCode));
        }
        public virtual bool CanManageModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ManageModulePermissionCode));
        }
        public virtual bool CanViewModule(ModuleInfo objModule)
        {
            return PortalSecurity.IsInRoles(objModule.ModulePermissions.ToString(ViewModulePermissionCode));
        }
        public virtual void DeleteModulePermissionsByUser(UserInfo objUser)
        {
            dataProvider.DeleteModulePermissionsByUserID(objUser.PortalID, objUser.UserID);
            DataCache.ClearModulePermissionsCachesByPortal(objUser.PortalID);
        }
        public virtual ModulePermissionCollection GetModulePermissions(int moduleID, int tabID)
        {
            bool bFound = false;
            Dictionary<int, ModulePermissionCollection> dicModulePermissions = GetModulePermissions(tabID);
            ModulePermissionCollection modulePermissions = null;
            bFound = dicModulePermissions.TryGetValue(moduleID, out modulePermissions);
            if (!bFound)
            {
                modulePermissions = new ModulePermissionCollection(CBO.FillCollection(dataProvider.GetModulePermissionsByModuleID(moduleID, -1), typeof(ModulePermissionInfo)), moduleID);
            }
            return modulePermissions;
        }
        public virtual bool HasModulePermission(ModulePermissionCollection objModulePermissions, string permissionKey)
        {
            return PortalSecurity.IsInRoles(objModulePermissions.ToString(permissionKey));
        }
        public virtual void SaveModulePermissions(ModuleInfo objModule)
        {
            if (objModule.ModulePermissions != null)
            {
                ModulePermissionCollection modulePermissions = ModulePermissionController.GetModulePermissions(objModule.ModuleID, objModule.TabID);
                if (!modulePermissions.CompareTo(objModule.ModulePermissions))
                {
                    dataProvider.DeleteModulePermissionsByModuleID(objModule.ModuleID);
                    foreach (ModulePermissionInfo objModulePermission in objModule.ModulePermissions)
                    {
                        if (objModule.InheritViewPermissions && objModulePermission.PermissionKey == "VIEW")
                        {
                            dataProvider.DeleteModulePermission(objModulePermission.ModulePermissionID);
                        }
                        else
                        {
                            dataProvider.AddModulePermission(objModule.ModuleID, objModulePermission.PermissionID, objModulePermission.RoleID, objModulePermission.AllowAccess, objModulePermission.UserID, UserController.GetCurrentUserInfo().UserID);
                        }
                    }
                }
            }
        }
        public virtual bool CanAddContentToPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ContentPagePermissionCode));
        }
        public virtual bool CanAddPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(AddPagePermissionCode));
        }
        public virtual bool CanAdminPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(AdminPagePermissionCode));
        }
        public virtual bool CanCopyPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(CopyPagePermissionCode));
        }
        public virtual bool CanDeletePage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(DeletePagePermissionCode));
        }
        public virtual bool CanExportPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ExportPagePermissionCode));
        }
        public virtual bool CanImportPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ImportPagePermissionCode));
        }
        public virtual bool CanManagePage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ManagePagePermissionCode));
        }
        public virtual bool CanNavigateToPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(NavigatePagePermissionCode));
        }
        public virtual bool CanViewPage(TabInfo objTab)
        {
            return PortalSecurity.IsInRoles(objTab.TabPermissions.ToString(ViewPagePermissionCode));
        }
        public virtual void DeleteTabPermissionsByUser(UserInfo objUser)
        {
            dataProvider.DeleteTabPermissionsByUserID(objUser.PortalID, objUser.UserID);
            DataCache.ClearTabPermissionsCache(objUser.PortalID);
        }
        public virtual TabPermissionCollection GetTabPermissions(int tabID, int portalID)
        {
            bool bFound = false;
            Dictionary<int, TabPermissionCollection> dicTabPermissions = GetTabPermissions(portalID);
            TabPermissionCollection tabPermissions = null;
            bFound = dicTabPermissions.TryGetValue(tabID, out tabPermissions);
            if (!bFound)
            {
                tabPermissions = new TabPermissionCollection(CBO.FillCollection(dataProvider.GetTabPermissionsByTabID(tabID, -1), typeof(TabPermissionInfo)), tabID);
            }
            return tabPermissions;
        }
        public virtual bool HasTabPermission(Security.Permissions.TabPermissionCollection objTabPermissions, string permissionKey)
        {
            return PortalSecurity.IsInRoles(objTabPermissions.ToString(permissionKey));
        }
        public virtual void SaveTabPermissions(TabInfo objTab)
        {
            TabPermissionCollection objCurrentTabPermissions = GetTabPermissions(objTab.TabID, objTab.PortalID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            if (!objCurrentTabPermissions.CompareTo(objTab.TabPermissions))
            {
                dataProvider.DeleteTabPermissionsByTabID(objTab.TabID);
                objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABPERMISSION_DELETED);
                if (objTab.TabPermissions != null)
                {
                    foreach (TabPermissionInfo objTabPermission in objTab.TabPermissions)
                    {
                        dataProvider.AddTabPermission(objTab.TabID, objTabPermission.PermissionID, objTabPermission.RoleID, objTabPermission.AllowAccess, objTabPermission.UserID, UserController.GetCurrentUserInfo().UserID);
                        objEventLog.AddLog(objTab, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABPERMISSION_CREATED);
                    }
                }
            }
        }
    }
}
