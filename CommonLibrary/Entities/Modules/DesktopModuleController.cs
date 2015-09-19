using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules.Definitions;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Data;
using System.Xml;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Services.EventQueue;
using CommonLibrary.Services.Installer.Packages;
using System.Collections;

namespace CommonLibrary.Entities.Modules
{
    public class DesktopModuleController
    {
        private static DataProvider dataProvider = DataProvider.Instance();
        private static object GetDesktopModulesByPortalCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalId = (int)cacheItemArgs.ParamList[0];
            return CBO.FillDictionary<int, DesktopModuleInfo>("DesktopModuleID", dataProvider.GetDesktopModulesByPortal(portalId), new Dictionary<int, DesktopModuleInfo>());
        }
        private static object GetPortalDesktopModulesByPortalIDCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalId = (int)cacheItemArgs.ParamList[0];
            return CBO.FillDictionary<int, PortalDesktopModuleInfo>("PortalDesktopModuleID", DataProvider.Instance().GetPortalDesktopModules(portalId, Null.NullInteger), new Dictionary<int, PortalDesktopModuleInfo>());
        }
        public static int AddDesktopModuleToPortal(int portalID, DesktopModuleInfo desktopModule, DesktopModulePermissionCollection permissions, bool clearCache)
        {
            int portalDesktopModuleID = AddDesktopModuleToPortal(portalID, desktopModule.DesktopModuleID, false, clearCache);
            if (portalDesktopModuleID > Null.NullInteger)
            {
                DesktopModulePermissionController.DeleteDesktopModulePermissionsByPortalDesktopModuleID(portalDesktopModuleID);
                foreach (DesktopModulePermissionInfo permission in permissions)
                {
                    permission.PortalDesktopModuleID = portalDesktopModuleID;
                    DesktopModulePermissionController.AddDesktopModulePermission(permission);
                }
            }
            return portalDesktopModuleID;
        }
        public static int AddDesktopModuleToPortal(int portalID, int desktopModuleID, bool addPermissions, bool clearCache)
        {
            int portalDesktopModuleID = Null.NullInteger;
            PortalDesktopModuleInfo portalDesktopModule = GetPortalDesktopModule(portalID, desktopModuleID);
            if (portalDesktopModule == null)
            {
                portalDesktopModuleID = DataProvider.Instance().AddPortalDesktopModule(portalID, desktopModuleID, UserController.GetCurrentUserInfo().UserID);
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog("PortalDesktopModuleID", portalDesktopModuleID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_CREATED);
                if (addPermissions)
                {
                    ArrayList permissions = PermissionController.GetPermissionsByPortalDesktopModule();
                    if (permissions.Count > 0)
                    {
                        PermissionInfo permission = permissions[0] as PermissionInfo;
                        PortalInfo objPortal = new PortalController().GetPortal(portalID);
                        if (permission != null && objPortal != null)
                        {
                            DesktopModulePermissionInfo desktopModulePermission = new DesktopModulePermissionInfo(permission);
                            desktopModulePermission.RoleID = objPortal.AdministratorRoleId;
                            desktopModulePermission.AllowAccess = true;
                            desktopModulePermission.PortalDesktopModuleID = portalDesktopModuleID;
                            DesktopModulePermissionController.AddDesktopModulePermission(desktopModulePermission);
                        }
                    }
                }
            }
            else
            {
                portalDesktopModuleID = portalDesktopModule.PortalDesktopModuleID;
            }
            if (clearCache)
            {
                DataCache.ClearPortalCache(portalID, true);
            }
            return portalDesktopModuleID;
        }
        public static void AddDesktopModuleToPortals(int desktopModuleID)
        {
            PortalController controller = new PortalController();
            foreach (PortalInfo portal in controller.GetPortals())
            {
                AddDesktopModuleToPortal(portal.PortalID, desktopModuleID, true, false);
            }
            DataCache.ClearHostCache(true);
        }
        public static void AddDesktopModulesToPortal(int portalID)
        {
            foreach (DesktopModuleInfo desktopModule in DesktopModuleController.GetDesktopModules(Null.NullInteger).Values)
            {
                if (!desktopModule.IsPremium)
                {
                    AddDesktopModuleToPortal(portalID, desktopModule.DesktopModuleID, !desktopModule.IsAdmin, false);
                }
            }
            DataCache.ClearPortalCache(portalID, true);
        }
        public static void DeleteDesktopModule(string moduleName)
        {
            DesktopModuleInfo desktopModule = GetDesktopModuleByModuleName(moduleName, Null.NullInteger);
            if (desktopModule != null)
            {
                DesktopModuleController controller = new DesktopModuleController();
                controller.DeleteDesktopModule(desktopModule.DesktopModuleID);
                PackageController.DeletePackage(desktopModule.PackageID);
            }
        }
        public static DesktopModuleInfo GetDesktopModule(int desktopModuleID, int portalID)
        {
            DesktopModuleInfo desktopModule = null;
            Dictionary<int, DesktopModuleInfo> desktopModules = GetDesktopModules(portalID);
            if (!desktopModules.TryGetValue(desktopModuleID, out desktopModule))
            {
                desktopModule = CBO.FillObject<DesktopModuleInfo>(dataProvider.GetDesktopModule(desktopModuleID));
            }
            return desktopModule;
        }
        public static DesktopModuleInfo GetDesktopModuleByPackageID(int packageID)
        {
            return CBO.FillObject<DesktopModuleInfo>(dataProvider.GetDesktopModuleByPackageID(packageID));
        }
        public static DesktopModuleInfo GetDesktopModuleByModuleName(string moduleName, int portalID)
        {
            DesktopModuleInfo desktopModule = null;
            foreach (KeyValuePair<int, DesktopModuleInfo> kvp in GetDesktopModules(portalID))
            {
                if (kvp.Value.ModuleName == moduleName)
                {
                    desktopModule = kvp.Value;
                    break;
                }
            }
            if (desktopModule == null)
            {
                desktopModule = CBO.FillObject<DesktopModuleInfo>(dataProvider.GetDesktopModuleByModuleName(moduleName));
            }
            return desktopModule;
        }
        public static Dictionary<int, DesktopModuleInfo> GetDesktopModules(int portalID)
        {
            Dictionary<int, DesktopModuleInfo> desktopModules = new Dictionary<int, DesktopModuleInfo>();
            if (portalID == Null.NullInteger)
            {
                desktopModules = CBO.FillDictionary<int, DesktopModuleInfo>("DesktopModuleID", dataProvider.GetDesktopModules(), new Dictionary<int, DesktopModuleInfo>());
            }
            else
            {
                string cacheKey = string.Format(DataCache.DesktopModuleCacheKey, portalID.ToString());
                desktopModules = CBO.GetCachedObject<Dictionary<int, DesktopModuleInfo>>(new CacheItemArgs(cacheKey, DataCache.DesktopModuleCacheTimeOut, DataCache.DesktopModuleCachePriority, portalID), GetDesktopModulesByPortalCallBack);
            }
            return desktopModules;
        }
        public static PortalDesktopModuleInfo GetPortalDesktopModule(int portalID, int desktopModuleID)
        {
            return CBO.FillObject<PortalDesktopModuleInfo>(DataProvider.Instance().GetPortalDesktopModules(portalID, desktopModuleID));
        }
        public static DesktopModuleInfo GetDesktopModuleByFriendlyName(string friendlyName)
        {
            DesktopModuleInfo desktopModule = null;
            foreach (KeyValuePair<int, DesktopModuleInfo> kvp in GetDesktopModules(Null.NullInteger))
            {
                if (kvp.Value.FriendlyName == friendlyName)
                {
                    desktopModule = kvp.Value;
                    break;
                }
            }
            return desktopModule;
        }
        public static Dictionary<int, PortalDesktopModuleInfo> GetPortalDesktopModulesByDesktopModuleID(int desktopModuleID)
        {
            return CBO.FillDictionary<int, PortalDesktopModuleInfo>("PortalDesktopModuleID", DataProvider.Instance().GetPortalDesktopModules(Null.NullInteger, desktopModuleID));
        }
        public static Dictionary<int, PortalDesktopModuleInfo> GetPortalDesktopModulesByPortalID(int portalID)
        {
            string cacheKey = string.Format(DataCache.PortalDesktopModuleCacheKey, portalID.ToString());
            return CBO.GetCachedObject<Dictionary<int, PortalDesktopModuleInfo>>(new CacheItemArgs(cacheKey, DataCache.PortalDesktopModuleCacheTimeOut, DataCache.PortalDesktopModuleCachePriority, portalID), GetPortalDesktopModulesByPortalIDCallBack);
        }
        public static SortedList<string, PortalDesktopModuleInfo> GetPortalDesktopModules(int portalID)
        {
            Dictionary<int, PortalDesktopModuleInfo> dicModules = GetPortalDesktopModulesByPortalID(portalID);
            SortedList<string, PortalDesktopModuleInfo> lstModules = new SortedList<string, PortalDesktopModuleInfo>();
            foreach (PortalDesktopModuleInfo desktopModule in dicModules.Values)
            {
                if (DesktopModulePermissionController.HasDesktopModulePermission(desktopModule.Permissions, "DEPLOY"))
                {
                    lstModules.Add(desktopModule.FriendlyName, desktopModule);
                }
            }
            return lstModules;
        }
        public static void RemoveDesktopModuleFromPortal(int portalID, int desktopModuleID, bool clearCache)
        {
            DataProvider.Instance().DeletePortalDesktopModules(portalID, desktopModuleID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("DesktopModuleID", desktopModuleID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_DELETED);
            if (clearCache)
            {
                DataCache.ClearPortalCache(portalID, false);
            }
        }
        public static void RemoveDesktopModuleFromPortals(int desktopModuleID)
        {
            DataProvider.Instance().DeletePortalDesktopModules(Null.NullInteger, desktopModuleID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("DesktopModuleID", desktopModuleID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_DELETED);
            DataCache.ClearHostCache(true);
        }
        public static void RemoveDesktopModulesFromPortal(int portalID)
        {
            DataProvider.Instance().DeletePortalDesktopModules(portalID, Null.NullInteger);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("PortalID", portalID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALDESKTOPMODULE_DELETED);
            DataCache.ClearPortalCache(portalID, true);
        }
        public static int SaveDesktopModule(DesktopModuleInfo desktopModule, bool saveChildren, bool clearCache)
        {
            int desktopModuleID = desktopModule.DesktopModuleID;
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            if (desktopModuleID == Null.NullInteger)
            {
                desktopModuleID = dataProvider.AddDesktopModule(desktopModule.PackageID, desktopModule.ModuleName, desktopModule.FolderName, desktopModule.FriendlyName, desktopModule.Description, desktopModule.Version, desktopModule.IsPremium, desktopModule.IsAdmin, desktopModule.BusinessControllerClass, desktopModule.SupportedFeatures,
                desktopModule.CompatibleVersions, desktopModule.Dependencies, desktopModule.Permissions, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(desktopModule, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_CREATED);
            }
            else
            {
                dataProvider.UpdateDesktopModule(desktopModule.DesktopModuleID, desktopModule.PackageID, desktopModule.ModuleName, desktopModule.FolderName, desktopModule.FriendlyName, desktopModule.Description, desktopModule.Version, desktopModule.IsPremium, desktopModule.IsAdmin, desktopModule.BusinessControllerClass,
                desktopModule.SupportedFeatures, desktopModule.CompatibleVersions, desktopModule.Dependencies, desktopModule.Permissions, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(desktopModule, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_UPDATED);
            }
            if (saveChildren)
            {
                foreach (ModuleDefinitionInfo definition in desktopModule.ModuleDefinitions.Values)
                {
                    definition.DesktopModuleID = desktopModuleID;
                    ModuleDefinitionInfo moduleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(definition.FriendlyName, desktopModuleID);
                    if (moduleDefinition != null)
                    {
                        definition.ModuleDefID = moduleDefinition.ModuleDefID;
                    }
                    ModuleDefinitionController.SaveModuleDefinition(definition, saveChildren, clearCache);
                }
            }
            if (clearCache)
            {
                DataCache.ClearHostCache(true);
            }
            return desktopModuleID;
        }
        public static void SerializePortalDesktopModules(XmlWriter writer, int portalID)
        {
            writer.WriteStartElement("portalDesktopModules");
            foreach (PortalDesktopModuleInfo portalDesktopModule in GetPortalDesktopModulesByPortalID(portalID).Values)
            {
                writer.WriteStartElement("portalDesktopModule");
                writer.WriteElementString("friendlyname", portalDesktopModule.FriendlyName);
                writer.WriteStartElement("portalDesktopModulePermissions");
                foreach (DesktopModulePermissionInfo permission in portalDesktopModule.Permissions)
                {
                    writer.WriteStartElement("portalDesktopModulePermission");
                    writer.WriteElementString("permissioncode", permission.PermissionCode);
                    writer.WriteElementString("permissionkey", permission.PermissionKey);
                    writer.WriteElementString("allowaccess", permission.AllowAccess.ToString().ToLower());
                    writer.WriteElementString("rolename", permission.RoleName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        public int AddDesktopModule(DesktopModuleInfo objDesktopModule)
        {
            return SaveDesktopModule(objDesktopModule, false, true);
        }
        public void DeleteDesktopModule(DesktopModuleInfo objDesktopModule)
        {
            dataProvider.DeleteDesktopModule(objDesktopModule.DesktopModuleID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objDesktopModule, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_DELETED);
            DataCache.ClearHostCache(true);
        }
        public void DeleteDesktopModule(int desktopModuleID)
        {
            dataProvider.DeleteDesktopModule(desktopModuleID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("DesktopModuleID", desktopModuleID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.DESKTOPMODULE_DELETED);
            DataCache.ClearHostCache(true);
        }
        public void UpdateDesktopModule(DesktopModuleInfo objDesktopModule)
        {
            SaveDesktopModule(objDesktopModule, false, true);
        }
        public void UpdateModuleInterfaces(ref DesktopModuleInfo desktopModuleInfo)
        {
            if ((UserController.GetCurrentUserInfo() == null))
            {
                UpdateModuleInterfaces(ref desktopModuleInfo, "", true);
            }
            else
            {
                UpdateModuleInterfaces(ref desktopModuleInfo, UserController.GetCurrentUserInfo().Username, true);
            }
        }
        public void UpdateModuleInterfaces(ref DesktopModuleInfo desktopModuleInfo, string sender, bool forceAppRestart)
        {
            EventMessage oAppStartMessage = new EventMessage();
            oAppStartMessage.Sender = sender;
            oAppStartMessage.Priority = MessagePriority.High;
            oAppStartMessage.ExpirationDate = DateTime.Now.AddYears(-1);
            oAppStartMessage.SentDate = System.DateTime.Now;
            oAppStartMessage.Body = "";
            oAppStartMessage.ProcessorType = "CommonLibrary.Entities.Modules.EventMessageProcessor, CommonLibrary";
            oAppStartMessage.ProcessorCommand = "UpdateSupportedFeatures";
            oAppStartMessage.Attributes.Add("BusinessControllerClass", desktopModuleInfo.BusinessControllerClass);
            oAppStartMessage.Attributes.Add("DesktopModuleId", desktopModuleInfo.DesktopModuleID.ToString());
            EventQueueController.SendMessage(oAppStartMessage, "Application_Start");
            if ((forceAppRestart))
            {
                Config.Touch();
            }
        }


    }
}
