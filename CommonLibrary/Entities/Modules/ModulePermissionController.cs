using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Users;
using CommonLibrary.Security;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Tabs;

namespace CommonLibrary.Entities.Modules
{
    public class ModulePermissionController
    {
        private static PermissionProvider provider = PermissionProvider.Instance();
        private static void ClearPermissionCache(int moduleId)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule(moduleId, Null.NullInteger, false);
            DataCache.ClearModulePermissionsCache(objModule.TabID);
        }
        private static bool CanAddContentToPage(ModuleInfo objModule)
        {
            bool canManage = Null.NullBoolean;
            TabInfo objTab = new TabController().GetTab(objModule.TabID, objModule.PortalID, false);
            canManage = TabPermissionController.CanAddContentToPage(objTab);
            return canManage;
        }
        public static bool CanAdminModule(ModuleInfo objModule)
        {
            return provider.CanAdminModule(objModule);
        }
        public static bool CanDeleteModule(ModuleInfo objModule)
        {
            return CanAddContentToPage(objModule) || provider.CanDeleteModule(objModule);
        }
        public static bool CanEditModuleContent(ModuleInfo objModule)
        {
            return CanAddContentToPage(objModule) || provider.CanEditModuleContent(objModule);
        }
        public static bool CanExportModule(ModuleInfo objModule)
        {
            return provider.CanExportModule(objModule);
        }
        public static bool CanImportModule(ModuleInfo objModule)
        {
            return provider.CanImportModule(objModule);
        }
        public static bool CanManageModule(ModuleInfo objModule)
        {
            return CanAddContentToPage(objModule) || provider.CanManageModule(objModule);
        }
        public static bool CanViewModule(ModuleInfo objModule)
        {
            bool canView = Null.NullBoolean;
            if (objModule.InheritViewPermissions)
            {
                TabInfo objTab = new TabController().GetTab(objModule.TabID, objModule.PortalID, false);
                canView = TabPermissionController.CanViewPage(objTab);
            }
            else
            {
                canView = provider.CanViewModule(objModule);
            }
            return canView;
        }
        public static void DeleteModulePermissionsByUser(UserInfo objUser)
        {
            provider.DeleteModulePermissionsByUser(objUser);
            DataCache.ClearModulePermissionsCachesByPortal(objUser.PortalID);
        }
        public static ModulePermissionCollection GetModulePermissions(int moduleID, int tabID)
        {
            return provider.GetModulePermissions(moduleID, tabID);
        }
        public static bool HasModulePermission(ModulePermissionCollection objModulePermissions, string permissionKey)
        {
            bool hasPermission = Null.NullBoolean;
            if (permissionKey.Contains(","))
            {
                foreach (string permission in permissionKey.Split(','))
                {
                    if (provider.HasModulePermission(objModulePermissions, permission))
                    {
                        hasPermission = true;
                        break;
                    }
                }
            }
            else
            {
                hasPermission = provider.HasModulePermission(objModulePermissions, permissionKey);
            }
            return hasPermission;
        }
        public static bool HasModuleAccess(SecurityAccessLevel AccessLevel, string permissionKey, ModuleInfo ModuleConfiguration)
        {
            bool blnAuthorized = false;
            UserInfo objUser = UserController.GetCurrentUserInfo();
            if (objUser != null && objUser.IsSuperUser)
            {
                blnAuthorized = true;
            }
            else
            {
                switch (AccessLevel)
                {
                    case SecurityAccessLevel.Anonymous:
                        blnAuthorized = true;
                        break;
                    case SecurityAccessLevel.View:
                        if (TabPermissionController.CanViewPage() || CanViewModule(ModuleConfiguration))
                        {
                            blnAuthorized = true;
                        }
                        break;
                    case SecurityAccessLevel.Edit:
                        if (TabPermissionController.CanAddContentToPage())
                        {
                            blnAuthorized = true;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(permissionKey))
                            {
                                permissionKey = "CONTENT,DELETE,EDIT,EXPORT,IMPORT,MANAGE";
                            }
                            if (ModuleConfiguration != null && CanViewModule(ModuleConfiguration) && (HasModulePermission(ModuleConfiguration.ModulePermissions, permissionKey) || HasModulePermission(ModuleConfiguration.ModulePermissions, "EDIT")))
                            {
                                blnAuthorized = true;
                            }
                        }
                        break;
                    case SecurityAccessLevel.Admin:
                        if (TabPermissionController.CanAddContentToPage())
                        {
                            blnAuthorized = true;
                        }
                        break;
                    case SecurityAccessLevel.Host:
                        break;
                }
            }
            return blnAuthorized;
        }
        public static void SaveModulePermissions(ModuleInfo objModule)
        {
            provider.SaveModulePermissions(objModule);
            DataCache.ClearModulePermissionsCache(objModule.TabID);
        }

    }
}
