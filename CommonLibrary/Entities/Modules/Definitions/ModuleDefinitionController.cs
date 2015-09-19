using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Common.Utilities;
using System.Collections;
using CommonLibrary.Data;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Entities.Modules.Definitions
{
    public class ModuleDefinitionController
    {
        private static string key = "ModuleDefID";
        private static DataProvider dataProvider = DataProvider.Instance();
        private static object GetModuleDefinitionsCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillDictionary<int, ModuleDefinitionInfo>(key, dataProvider.GetModuleDefinitions(), new Dictionary<int, ModuleDefinitionInfo>());
        }
        public static ModuleDefinitionInfo GetModuleDefinitionByID(int moduleDefID)
        {
            ModuleDefinitionInfo moduleDefinition = null;
            Dictionary<int, ModuleDefinitionInfo> moduleDefinitions = GetModuleDefinitions();
            if (!moduleDefinitions.TryGetValue(moduleDefID, out moduleDefinition))
            {
                moduleDefinition = CBO.FillObject<ModuleDefinitionInfo>(dataProvider.GetModuleDefinition(moduleDefID));
            }
            return moduleDefinition;
        }
        public static ModuleDefinitionInfo GetModuleDefinitionByFriendlyName(string friendlyName)
        {
            ModuleDefinitionInfo moduleDefinition = null;
            foreach (KeyValuePair<int, ModuleDefinitionInfo> kvp in GetModuleDefinitions())
            {
                if (kvp.Value.FriendlyName == friendlyName)
                {
                    moduleDefinition = kvp.Value;
                    break;
                }
            }
            return moduleDefinition;
        }
        public static ModuleDefinitionInfo GetModuleDefinitionByFriendlyName(string friendlyName, int desktopModuleID)
        {
            ModuleDefinitionInfo moduleDefinition = null;
            Dictionary<string, ModuleDefinitionInfo> moduleDefinitions = GetModuleDefinitionsByDesktopModuleID(desktopModuleID);
            if (!moduleDefinitions.TryGetValue(friendlyName, out moduleDefinition))
            {
                moduleDefinition = CBO.FillObject<ModuleDefinitionInfo>(dataProvider.GetModuleDefinitionByName(desktopModuleID, friendlyName));
            }
            return moduleDefinition;
        }
        public static Dictionary<int, ModuleDefinitionInfo> GetModuleDefinitions()
        {
            return CBO.GetCachedObject<Dictionary<int, ModuleDefinitionInfo>>(new CacheItemArgs(DataCache.ModuleDefinitionCacheKey, DataCache.ModuleDefinitionCachePriority), GetModuleDefinitionsCallBack);
        }
        public static Dictionary<string, ModuleDefinitionInfo> GetModuleDefinitionsByDesktopModuleID(int desktopModuleID)
        {
            Dictionary<string, ModuleDefinitionInfo> moduleDefinitions = new Dictionary<string, ModuleDefinitionInfo>();
            foreach (KeyValuePair<int, ModuleDefinitionInfo> kvp in GetModuleDefinitions())
            {
                if (kvp.Value.DesktopModuleID == desktopModuleID)
                {
                    moduleDefinitions.Add(kvp.Value.FriendlyName, kvp.Value);
                }
            }
            return moduleDefinitions;
        }
        public static int SaveModuleDefinition(ModuleDefinitionInfo moduleDefinition, bool saveChildren, bool clearCache)
        {
            int moduleDefinitionID = moduleDefinition.ModuleDefID;
            if (moduleDefinitionID == Null.NullInteger)
            {
                moduleDefinitionID = dataProvider.AddModuleDefinition(moduleDefinition.DesktopModuleID, moduleDefinition.FriendlyName, moduleDefinition.DefaultCacheTime, UserController.GetCurrentUserInfo().UserID);
            }
            else
            {
                dataProvider.UpdateModuleDefinition(moduleDefinition.ModuleDefID, moduleDefinition.FriendlyName, moduleDefinition.DefaultCacheTime, UserController.GetCurrentUserInfo().UserID);
            }
            if (saveChildren)
            {
                foreach (KeyValuePair<string, PermissionInfo> kvp in moduleDefinition.Permissions)
                {
                    kvp.Value.ModuleDefID = moduleDefinitionID;
                    PermissionController permissionController = new PermissionController();
                    ArrayList permissions = permissionController.GetPermissionByCodeAndKey(kvp.Value.PermissionCode, kvp.Value.PermissionKey);
                    if (permissions != null && permissions.Count == 1)
                    {
                        PermissionInfo permission = (PermissionInfo)permissions[0];
                        kvp.Value.PermissionID = permission.PermissionID;
                        permissionController.UpdatePermission(kvp.Value);
                    }
                    else
                    {
                        permissionController.AddPermission(kvp.Value);
                    }
                }
                foreach (KeyValuePair<string, ModuleControlInfo> kvp in moduleDefinition.ModuleControls)
                {
                    kvp.Value.ModuleDefID = moduleDefinitionID;
                    ModuleControlInfo moduleControl = ModuleControlController.GetModuleControlByControlKey(kvp.Value.ControlKey, kvp.Value.ModuleDefID);
                    if (moduleControl != null)
                    {
                        kvp.Value.ModuleControlID = moduleControl.ModuleControlID;
                    }
                    ModuleControlController.SaveModuleControl(kvp.Value, clearCache);
                }
            }
            if (clearCache)
            {
                DataCache.ClearHostCache(true);
            }
            return moduleDefinitionID;
        }
        public int AddModuleDefinition(ModuleDefinitionInfo objModuleDefinition)
        {
            return SaveModuleDefinition(objModuleDefinition, false, true);
        }
        public void DeleteModuleDefinition(ModuleDefinitionInfo objModuleDefinition)
        {
            DeleteModuleDefinition(objModuleDefinition.ModuleDefID);
        }
        public void DeleteModuleDefinition(int moduleDefinitionId)
        {
            PermissionController permissionController = new PermissionController();
            foreach (PermissionInfo permission in permissionController.GetPermissionsByModuleDefID(moduleDefinitionId))
            {
                permissionController.DeletePermission(permission.PermissionID);
            }
            dataProvider.DeleteModuleDefinition(moduleDefinitionId);
            DataCache.ClearHostCache(true);
        }
        public void UpdateModuleDefinition(ModuleDefinitionInfo objModuleDefinition)
        {
            SaveModuleDefinition(objModuleDefinition, false, true);
        }
       
    }
}
