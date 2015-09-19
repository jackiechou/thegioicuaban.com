using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.FileSystem;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Security.Permissions
{
    public class FolderPermissionController
    {
        private static PermissionProvider provider = PermissionProvider.Instance();
        private static void ClearPermissionCache(int PortalID)
        {
            DataCache.ClearFolderPermissionsCache(PortalID);
        }
        public static bool CanAddFolder(FolderInfo folder)
        {
            return provider.CanAddFolder(folder) || CanAdminFolder(folder);
        }
        public static bool CanAdminFolder(FolderInfo folder)
        {
            return provider.CanAdminFolder(folder);
        }
        public static bool CanCopyFolder(FolderInfo folder)
        {
            return provider.CanCopyFolder(folder) || CanAdminFolder(folder);
        }
        public static bool CanDeleteFolder(FolderInfo folder)
        {
            return provider.CanDeleteFolder(folder) || CanAdminFolder(folder);
        }
        public static bool CanManageFolder(FolderInfo folder)
        {
            return provider.CanManageFolder(folder) || CanAdminFolder(folder);
        }
        public static bool CanViewFolder(FolderInfo folder)
        {
            return provider.CanViewFolder(folder) || CanAdminFolder(folder);
        }
        public static void DeleteFolderPermissionsByUser(UserInfo objUser)
        {
            provider.DeleteFolderPermissionsByUser(objUser);
            ClearPermissionCache(objUser.PortalID);
        }
        public static FolderPermissionCollection GetFolderPermissionsCollectionByFolder(int PortalID, string Folder)
        {
            return provider.GetFolderPermissionsCollectionByFolder(PortalID, Folder);
        }
        public static bool HasFolderPermission(int portalId, string folderPath, string permissionKey)
        {
            return HasFolderPermission(FolderPermissionController.GetFolderPermissionsCollectionByFolder(portalId, folderPath), permissionKey);
        }
        public static bool HasFolderPermission(FolderPermissionCollection objFolderPermissions, string PermissionKey)
        {
            bool hasPermission = provider.HasFolderPermission(objFolderPermissions, "WRITE");
            if (!hasPermission)
            {
                if (PermissionKey.Contains(","))
                {
                    foreach (string permission in PermissionKey.Split(','))
                    {
                        if (provider.HasFolderPermission(objFolderPermissions, permission))
                        {
                            hasPermission = true;
                            break;
                        }
                    }
                }
                else
                {
                    hasPermission = provider.HasFolderPermission(objFolderPermissions, PermissionKey);
                }
            }
            return hasPermission;
        }
        public static void SaveFolderPermissions(FolderInfo folder)
        {
            provider.SaveFolderPermissions(folder);
            DataCache.ClearFolderPermissionsCache(folder.PortalID);
        }

 
    }
}
