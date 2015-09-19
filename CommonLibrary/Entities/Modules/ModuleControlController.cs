using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Users;
using CommonLibrary.Data;

namespace CommonLibrary.Entities.Modules
{
    public class ModuleControlController
    {
        private static string key = "ModuleControlID";
        private static DataProvider dataProvider = DataProvider.Instance();
        private static Dictionary<int, ModuleControlInfo> GetModuleControls()
        {
            return CBO.GetCachedObject<Dictionary<int, ModuleControlInfo>>(new CacheItemArgs(DataCache.ModuleControlsCacheKey, DataCache.ModuleControlsCacheTimeOut, DataCache.ModuleControlsCachePriority), GetModuleControlsCallBack);
        }
        private static object GetModuleControlsCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillDictionary<int, ModuleControlInfo>(key, dataProvider.GetModuleControls(), new Dictionary<int, ModuleControlInfo>());
        }
        public static void AddModuleControl(ModuleControlInfo objModuleControl)
        {
            SaveModuleControl(objModuleControl, true);
        }
        public static void DeleteModuleControl(int moduleControlID)
        {
            dataProvider.DeleteModuleControl(moduleControlID);
            DataCache.ClearHostCache(true);
        }
        public static ModuleControlInfo GetModuleControl(int moduleControlID)
        {
            ModuleControlInfo moduleControl = null;
            Dictionary<int, ModuleControlInfo> moduleControls = GetModuleControls();
            if (!moduleControls.TryGetValue(moduleControlID, out moduleControl))
            {
                moduleControl = CBO.FillObject<ModuleControlInfo>(dataProvider.GetModuleControl(moduleControlID));
            }
            return moduleControl;
        }
        public static Dictionary<string, ModuleControlInfo> GetModuleControlsByModuleDefinitionID(int moduleDefID)
        {
            Dictionary<string, ModuleControlInfo> moduleControls = new Dictionary<string, ModuleControlInfo>();
            foreach (ModuleControlInfo moduleControl in GetModuleControls().Values)
            {
                if (moduleControl.ModuleDefID == moduleDefID)
                {
                    moduleControls[moduleControl.ControlKey] = moduleControl;
                }
            }
            return moduleControls;
        }
        public static ModuleControlInfo GetModuleControlByControlKey(string controlKey, int moduleDefId)
        {
            ModuleControlInfo moduleControl = null;
            foreach (KeyValuePair<int, ModuleControlInfo> kvp in GetModuleControls())
            {
                if (kvp.Value.ModuleDefID == moduleDefId && kvp.Value.ControlKey.ToLowerInvariant() == controlKey.ToLowerInvariant())
                {
                    moduleControl = kvp.Value;
                    break;
                }
            }
            if (moduleControl == null)
            {
                moduleControl = CBO.FillObject<ModuleControlInfo>(dataProvider.GetModuleControlsByKey(controlKey, moduleDefId));
            }
            return moduleControl;
        }
        public static int SaveModuleControl(ModuleControlInfo moduleControl, bool clearCache)
        {
            int moduleControlID = moduleControl.ModuleControlID;
            if (moduleControlID == Null.NullInteger)
            {
                moduleControlID = dataProvider.AddModuleControl(moduleControl.ModuleDefID, moduleControl.ControlKey, moduleControl.ControlTitle, moduleControl.ControlSrc, moduleControl.IconFile, Convert.ToInt32(moduleControl.ControlType), moduleControl.ViewOrder, moduleControl.HelpURL, moduleControl.SupportsPartialRendering, UserController.GetCurrentUserInfo().UserID);
            }
            else
            {
                dataProvider.UpdateModuleControl(moduleControl.ModuleControlID, moduleControl.ModuleDefID, moduleControl.ControlKey, moduleControl.ControlTitle, moduleControl.ControlSrc, moduleControl.IconFile, Convert.ToInt32(moduleControl.ControlType), moduleControl.ViewOrder, moduleControl.HelpURL, moduleControl.SupportsPartialRendering,
                UserController.GetCurrentUserInfo().UserID);
            }
            if (clearCache)
            {
                DataCache.ClearHostCache(true);
            }
            return moduleControlID;
        }
        public static void UpdateModuleControl(ModuleControlInfo objModuleControl)
        {
            SaveModuleControl(objModuleControl, true);
        }

    }
}
