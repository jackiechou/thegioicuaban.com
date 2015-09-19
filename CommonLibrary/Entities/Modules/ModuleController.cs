using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Web;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using CommonLibrary.Entities.Modules.Definitions;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Content;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Content.Taxonomy;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Services.OutputCache;
using CommonLibrary.Services.ModuleCache;
using CommonLibrary.Common;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Data;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Security.Roles;
using CommonLibrary.Services.EventQueue;

namespace CommonLibrary.Entities.Modules
{
    public class ModuleController
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();  

        private static DataProvider dataProvider = DataProvider.Instance();
        private void ClearCache(int TabId)
        {
            DataCache.ClearModuleCache(TabId);
        }
        private static void AddContent(XmlNode nodeModule, ModuleInfo objModule)
        {
            XmlAttribute xmlattr;
            if (!String.IsNullOrEmpty(objModule.DesktopModule.BusinessControllerClass) && objModule.DesktopModule.IsPortable)
            {
                try
                {
                    object objObject = Framework.Reflection.CreateObject(objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.BusinessControllerClass);
                    if (objObject is IPortable)
                    {
                        string Content = Convert.ToString(((IPortable)objObject).ExportModule(objModule.ModuleID));
                        if (!String.IsNullOrEmpty(Content))
                        {
                            XmlNode newnode = nodeModule.OwnerDocument.CreateElement("content");
                            xmlattr = nodeModule.OwnerDocument.CreateAttribute("type");
                            xmlattr.Value = Globals.CleanName(objModule.DesktopModule.ModuleName);
                            newnode.Attributes.Append(xmlattr);
                            xmlattr = nodeModule.OwnerDocument.CreateAttribute("version");
                            xmlattr.Value = objModule.DesktopModule.Version;
                            newnode.Attributes.Append(xmlattr);
                            Content = HttpContext.Current.Server.HtmlEncode(Content);
                            newnode.InnerXml = XmlUtils.XMLEncode(Content);
                            nodeModule.AppendChild(newnode);
                        }
                    }
                }
                catch
                {
                }
            }
        }
        private static bool CheckIsInstance(int templateModuleID, Hashtable hModules)
        {
            bool IsInstance = false;
            if (templateModuleID > 0)
            {
                if (hModules[templateModuleID] != null)
                {
                    IsInstance = true;
                }
            }
            return IsInstance;
        }
        private static void CreateEventQueueMessage(ModuleInfo objModule, string strContent, string strVersion, int userID)
        {
            EventMessage oAppStartMessage = new EventMessage();
            oAppStartMessage.Priority = MessagePriority.High;
            oAppStartMessage.ExpirationDate = DateTime.Now.AddYears(-1);
            oAppStartMessage.SentDate = System.DateTime.Now;
            oAppStartMessage.Body = "";
            oAppStartMessage.ProcessorType = "CommonLibrary.Entities.Modules.EventMessageProcessor";
            oAppStartMessage.ProcessorCommand = "ImportModule";
            oAppStartMessage.Attributes.Add("BusinessControllerClass", objModule.DesktopModule.BusinessControllerClass);
            oAppStartMessage.Attributes.Add("ModuleId", objModule.ModuleID.ToString());
            oAppStartMessage.Attributes.Add("Content", strContent);
            oAppStartMessage.Attributes.Add("Version", strVersion);
            oAppStartMessage.Attributes.Add("UserID", userID.ToString());
            EventQueueController.SendMessage(oAppStartMessage, "Application_Start_FirstRequest");
        }
        private static ModuleInfo DeserializeModule(XmlNode nodeModule, XmlNode nodePane, int PortalId, int TabId, int ModuleDefId)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = new ModuleInfo();
            objModule.PortalID = PortalId;
            objModule.TabID = TabId;
            objModule.ModuleOrder = -1;
            objModule.ModuleTitle = XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "title");
            objModule.PaneName = XmlUtils.GetNodeValue(nodePane.CreateNavigator(), "name");
            objModule.ModuleDefID = ModuleDefId;
            objModule.CacheTime = XmlUtils.GetNodeValueInt(nodeModule, "cachetime");
            objModule.CacheMethod = XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "cachemethod");
            objModule.Alignment = XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "alignment");
            objModule.IconFile = Globals.ImportFile(PortalId, XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "iconfile"));
            objModule.AllTabs = XmlUtils.GetNodeValueBoolean(nodeModule, "alltabs");
            switch (XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "visibility"))
            {
                case "Maximized":
                    objModule.Visibility = VisibilityState.Maximized;
                    break;
                case "Minimized":
                    objModule.Visibility = VisibilityState.Minimized;
                    break;
                case "None":
                    objModule.Visibility = VisibilityState.None;
                    break;
            }
            objModule.Color = XmlUtils.GetNodeValue(nodeModule, "color", "");
            objModule.Border = XmlUtils.GetNodeValue(nodeModule, "border", "");
            objModule.Header = XmlUtils.GetNodeValue(nodeModule, "header", "");
            objModule.Footer = XmlUtils.GetNodeValue(nodeModule, "footer", "");
            objModule.InheritViewPermissions = XmlUtils.GetNodeValueBoolean(nodeModule, "inheritviewpermissions", false);
            objModule.StartDate = XmlUtils.GetNodeValueDate(nodeModule, "startdate", Null.NullDate);
            objModule.EndDate = XmlUtils.GetNodeValueDate(nodeModule, "enddate", Null.NullDate);
            if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeModule, "containersrc", "")))
            {
                objModule.ContainerSrc = XmlUtils.GetNodeValue(nodeModule, "containersrc", "");
            }
            objModule.DisplayTitle = XmlUtils.GetNodeValueBoolean(nodeModule, "displaytitle", true);
            objModule.DisplayPrint = XmlUtils.GetNodeValueBoolean(nodeModule, "displayprint", true);
            objModule.DisplaySyndicate = XmlUtils.GetNodeValueBoolean(nodeModule, "displaysyndicate", false);
            objModule.IsWebSlice = XmlUtils.GetNodeValueBoolean(nodeModule, "iswebslice", false);
            if (objModule.IsWebSlice)
            {
                objModule.WebSliceTitle = XmlUtils.GetNodeValue(nodeModule, "webslicetitle", objModule.ModuleTitle);
                objModule.WebSliceExpiryDate = XmlUtils.GetNodeValueDate(nodeModule, "websliceexpirydate", objModule.EndDate);
                objModule.WebSliceTTL = XmlUtils.GetNodeValueInt(nodeModule, "webslicettl", objModule.CacheTime / 60);
            }
            return objModule;
        }
        private static void DeserializeModulePermissions(XmlNodeList nodeModulePermissions, int PortalId, int TabId, ModuleInfo objModule)
        {
            RoleController objRoleController = new RoleController();
            PermissionController objPermissionController = new PermissionController();
            PermissionInfo objPermission;
            int PermissionID;
            ArrayList arrPermissions;
            int i;
            foreach (XmlNode node in nodeModulePermissions)
            {
                string PermissionKey = XmlUtils.GetNodeValue(node.CreateNavigator(), "permissionkey");
                string PermissionCode = XmlUtils.GetNodeValue(node.CreateNavigator(), "permissioncode");
                string RoleName = XmlUtils.GetNodeValue(node.CreateNavigator(), "rolename");
                bool AllowAccess = XmlUtils.GetNodeValueBoolean(node, "allowaccess");
                int RoleID = int.MinValue;
                switch (RoleName)
                {
                    case Globals.glbRoleAllUsersName:
                        RoleID = Convert.ToInt32(Globals.glbRoleAllUsers);
                        break;
                    case Common.Globals.glbRoleUnauthUserName:
                        RoleID = Convert.ToInt32(Globals.glbRoleUnauthUser);
                        break;
                    default:
                        RoleInfo objRole = objRoleController.GetRoleByName(PortalId, RoleName);
                        if (objRole != null)
                        {
                            RoleID = objRole.RoleID;
                        }
                        break;
                }
                if (RoleID != int.MinValue)
                {
                    PermissionID = -1;
                    arrPermissions = objPermissionController.GetPermissionByCodeAndKey(PermissionCode, PermissionKey);
                    for (i = 0; i <= arrPermissions.Count - 1; i++)
                    {
                        objPermission = (PermissionInfo)arrPermissions[i];
                        PermissionID = objPermission.PermissionID;
                    }
                    if (PermissionID != -1)
                    {
                        ModulePermissionInfo objModulePermission = new ModulePermissionInfo();
                        objModulePermission.ModuleID = objModule.ModuleID;
                        objModulePermission.PermissionID = PermissionID;
                        objModulePermission.RoleID = RoleID;
                        objModulePermission.AllowAccess = Convert.ToBoolean(XmlUtils.GetNodeValue(node.CreateNavigator(), "allowaccess"));
                        objModule.ModulePermissions.Add(objModulePermission);
                    }
                }
            }
        }
        private static bool FindModule(XmlNode nodeModule, int TabId, PortalTemplateModuleAction mergeTabs)
        {
            ModuleController objModules = new ModuleController();
            Dictionary<int, ModuleInfo> dicModules = objModules.GetTabModules(TabId);
            ModuleInfo objModule;
            bool moduleFound = false;
            string modTitle = XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "title");
            if (mergeTabs == PortalTemplateModuleAction.Merge)
            {
                foreach (KeyValuePair<int, ModuleInfo> kvp in dicModules)
                {
                    objModule = kvp.Value;
                    if (modTitle == objModule.ModuleTitle)
                    {
                        moduleFound = true;
                        break;
                    }
                }
            }
            return moduleFound;
        }
        private static void GetModuleContent(XmlNode nodeModule, int ModuleId, int TabId, int PortalId)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModule(ModuleId, TabId, true);
            string strVersion = nodeModule.SelectSingleNode("content").Attributes["version"].Value;
            string strType = nodeModule.SelectSingleNode("content").Attributes["type"].Value;
            string strcontent = nodeModule.SelectSingleNode("content").InnerXml;
            strcontent = strcontent.Substring(9, strcontent.Length - 12);
            if (!String.IsNullOrEmpty(objModule.DesktopModule.BusinessControllerClass) && !String.IsNullOrEmpty(strcontent))
            {
                PortalInfo objportal;
                PortalController objportals = new PortalController();
                objportal = objportals.GetPortal(PortalId);
                if (objModule.DesktopModule.SupportedFeatures == Null.NullInteger)
                {
                    CreateEventQueueMessage(objModule, strcontent, strVersion, objportal.AdministratorId);
                }
                else
                {
                    strcontent = HttpContext.Current.Server.HtmlDecode(strcontent);
                    if (objModule.DesktopModule.IsPortable)
                    {
                        try
                        {
                            object objObject = Framework.Reflection.CreateObject(objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.BusinessControllerClass);
                            if (objObject is IPortable)
                            {
                                ((IPortable)objObject).ImportModule(objModule.ModuleID, strcontent, strVersion, objportal.AdministratorId);
                            }
                        }
                        catch
                        {
                            CreateEventQueueMessage(objModule, strcontent, strVersion, objportal.AdministratorId);
                        }
                    }
                }
            }
        }
        private static object GetTabModulesCallBack(CacheItemArgs cacheItemArgs)
        {
            int tabID = (int)cacheItemArgs.ParamList[0];
            return CBO.FillDictionary<int, ModuleInfo>("ModuleID", dataProvider.GetTabModules(tabID), new Dictionary<int, ModuleInfo>());
        }
        private static ModuleDefinitionInfo GetModuleDefinition(XmlNode nodeModule)
        {
            ModuleDefinitionInfo objModuleDefinition = null;
            DesktopModuleInfo objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "definition"), Null.NullInteger);
            if (objDesktopModule != null)
            {
                string friendlyName = XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "moduledefinition");
                if (string.IsNullOrEmpty(friendlyName))
                {
                    foreach (ModuleDefinitionInfo md in ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(objDesktopModule.DesktopModuleID).Values)
                    {
                        break;
                    }
                }
                else
                {
                    objModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(friendlyName, objDesktopModule.DesktopModuleID);
                }
            }
            return objModuleDefinition;
        }
        private static void DeserializeModuleSettings(XmlNodeList nodeModuleSettings, ModuleInfo objModule)
        {
            string sKey;
            string sValue;
            foreach (XmlNode oModuleSettingNode in nodeModuleSettings)
            {
                sKey = XmlUtils.GetNodeValue(oModuleSettingNode.CreateNavigator(), "settingname");
                sValue = XmlUtils.GetNodeValue(oModuleSettingNode.CreateNavigator(), "settingvalue");
                objModule.ModuleSettings[sKey] = sValue;
            }
        }
        private static void DeserializeTabModuleSettings(XmlNodeList nodeTabModuleSettings, ModuleInfo objModule)
        {
            string sKey;
            string sValue;
            ModuleController mc = new ModuleController();
            foreach (XmlNode oTabModuleSettingNode in nodeTabModuleSettings)
            {
                sKey = XmlUtils.GetNodeValue(oTabModuleSettingNode.CreateNavigator(), "settingname");
                sValue = XmlUtils.GetNodeValue(oTabModuleSettingNode.CreateNavigator(), "settingvalue");
                objModule.TabModuleSettings[sKey] = sValue;
            }
        }
        public static void DeserializeModule(XmlNode nodeModule, XmlNode nodePane, int PortalId, int TabId, PortalTemplateModuleAction mergeTabs, Hashtable hModules)
        {
            ModuleController objModules = new ModuleController();
            ModuleDefinitionInfo objModuleDefinition = GetModuleDefinition(nodeModule);
            ModuleInfo objModule;
            int intModuleId;
            int templateModuleID = XmlUtils.GetNodeValueInt(nodeModule, "moduleID");
            bool IsInstance = CheckIsInstance(templateModuleID, hModules);
            if (objModuleDefinition != null)
            {
                if (!FindModule(nodeModule, TabId, mergeTabs))
                {
                    objModule = DeserializeModule(nodeModule, nodePane, PortalId, TabId, objModuleDefinition.ModuleDefID);
                    XmlNodeList nodeModuleSettings = nodeModule.SelectNodes("modulesettings/modulesetting");
                    DeserializeModuleSettings(nodeModuleSettings, objModule);
                    XmlNodeList nodeTabModuleSettings = nodeModule.SelectNodes("tabmodulesettings/tabmodulesetting");
                    DeserializeTabModuleSettings(nodeTabModuleSettings, objModule);
                    if (!IsInstance)
                    {
                        intModuleId = objModules.AddModule(objModule);
                        if (templateModuleID > 0)
                        {
                            hModules.Add(templateModuleID, intModuleId);
                        }
                    }
                    else
                    {
                        objModule.ModuleID = Convert.ToInt32(hModules[templateModuleID]);
                        intModuleId = objModules.AddModule(objModule);
                    }
                    if (!String.IsNullOrEmpty(XmlUtils.GetNodeValue(nodeModule.CreateNavigator(), "content")) && !IsInstance)
                    {
                        GetModuleContent(nodeModule, intModuleId, TabId, PortalId);
                    }
                    if (!IsInstance)
                    {
                        XmlNodeList nodeModulePermissions = nodeModule.SelectNodes("modulepermissions/permission");
                        DeserializeModulePermissions(nodeModulePermissions, PortalId, TabId, objModule);
                        ModulePermissionController.SaveModulePermissions(objModule);
                    }
                }
            }
        }
        public static XmlNode SerializeModule(XmlDocument xmlModule, ModuleInfo objModule, bool includeContent)
        {
            XmlSerializer xserModules = new XmlSerializer(typeof(ModuleInfo));
            XmlNode nodeModule;
            XmlNode nodeDefinition;
            XmlNode newnode;
            ModuleController objmodules = new ModuleController();
            StringWriter sw = new StringWriter();
            xserModules.Serialize(sw, objModule);
            xmlModule.LoadXml(sw.GetStringBuilder().ToString());
            nodeModule = xmlModule.SelectSingleNode("module");
            nodeModule.Attributes.Remove(nodeModule.Attributes["xmlns:xsd"]);
            nodeModule.Attributes.Remove(nodeModule.Attributes["xmlns:xsi"]);
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("portalid"));
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("tabid"));
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("tabmoduleid"));
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("moduleorder"));
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("panename"));
            nodeModule.RemoveChild(nodeModule.SelectSingleNode("isdeleted"));
            foreach (XmlNode nodePermission in nodeModule.SelectNodes("modulepermissions/permission"))
            {
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("modulepermissionid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("permissionid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("moduleid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("roleid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("userid"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("username"));
                nodePermission.RemoveChild(nodePermission.SelectSingleNode("displayname"));
            }
            if (includeContent)
            {
                AddContent(nodeModule, objModule);
            }
            XmlUtils.SerializeHashtable(objModule.ModuleSettings, xmlModule, nodeModule, "modulesetting", "settingname", "settingvalue");
            XmlUtils.SerializeHashtable(objModule.TabModuleSettings, xmlModule, nodeModule, "tabmodulesetting", "settingname", "settingvalue");
            newnode = xmlModule.CreateElement("definition");
            ModuleDefinitionInfo objModuleDef = ModuleDefinitionController.GetModuleDefinitionByID(objModule.ModuleDefID);
            newnode.InnerText = DesktopModuleController.GetDesktopModule(objModuleDef.DesktopModuleID, objModule.PortalID).ModuleName;
            nodeModule.AppendChild(newnode);
            nodeDefinition = xmlModule.CreateElement("moduledefinition");
            nodeDefinition.InnerText = objModuleDef.FriendlyName;
            nodeModule.AppendChild(nodeDefinition);
            return nodeModule;
        }
        public static void SynchronizeModule(int moduleID)
        {
            ModuleController objModules = new ModuleController();
            ArrayList arrModules = objModules.GetModuleTabs(moduleID);
            TabController tabController = new TabController();
            Hashtable tabSettings;
            foreach (ModuleInfo objModule in arrModules)
            {
                tabSettings = tabController.GetTabSettings(objModule.TabID);
                if (tabSettings["CacheProvider"] != null && tabSettings["CacheProvider"].ToString().Length > 0)
                {
                    OutputCachingProvider provider = OutputCachingProvider.Instance(tabSettings["CacheProvider"].ToString());
                    if (provider != null)
                    {
                        provider.Remove(objModule.TabID);
                    }
                }
                if (HttpContext.Current != null)
                {
                    ModuleCachingProvider provider = ModuleCachingProvider.Instance(objModule.GetEffectiveCacheMethod());
                    if (provider != null)
                    {
                        provider.Remove(objModule.TabModuleID);
                    }
                }
            }
        }
        public int AddModule(ModuleInfo objModule)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            if (Null.IsNull(objModule.ModuleID))
            {
                IContentTypeController typeController = new ContentTypeController();
                ContentType contentType = (from t in typeController.GetContentTypes()
                                           where t.Type == "Module"
                                           select t)
                                                 .SingleOrDefault();

                IContentController contentController = new ContentController();
                objModule.Content = objModule.ModuleTitle;
                objModule.ContentTypeId = contentType.ContentTypeId;
                objModule.Indexed = false;
                int contentItemID = contentController.AddContentItem(objModule);

                objModule.ModuleID = dataProvider.AddModule(contentItemID, objModule.PortalID, objModule.ModuleDefID, objModule.ModuleTitle, objModule.AllTabs, objModule.Header, objModule.Footer, objModule.StartDate, objModule.EndDate, objModule.InheritViewPermissions, objModule.IsDeleted,
                UserController.GetCurrentUserInfo().UserID);
                //Now we have the ModuleID - update the contentItem
                contentController.UpdateContentItem(objModule);
                objEventLog.AddLog(objModule, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_CREATED);
                ModulePermissionController.SaveModulePermissions(objModule);
            }
            ModuleInfo tmpModule = GetModule(objModule.ModuleID, objModule.TabID);
            if (tmpModule != null)
            {
                if (tmpModule.IsDeleted)
                {
                    RestoreModule(objModule);
                }
            }
            else
            {
                dataProvider.AddTabModule(objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName, objModule.CacheTime, objModule.CacheMethod, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile,
                (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate, objModule.IsWebSlice, objModule.WebSliceTitle, objModule.WebSliceExpiryDate, objModule.WebSliceTTL, UserController.GetCurrentUserInfo().UserID);
                Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("TabId", objModule.TabID.ToString()));
                objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("ModuleID", objModule.ModuleID.ToString()));
                objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABMODULE_CREATED.ToString();
                objEventLog.AddLog(objEventLogInfo);
                if (objModule.ModuleOrder == -1)
                {
                    UpdateModuleOrder(objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName);
                }
                else
                {
                    UpdateTabModuleOrder(objModule.TabID);
                }
            }
            UpdateModuleSettings(objModule);
            if (objModule.TabModuleID == -1)
            {
                if (tmpModule == null)
                    tmpModule = GetModule(objModule.ModuleID, objModule.TabID);
                objModule.TabModuleID = tmpModule.TabModuleID;
            }
            UpdateTabModuleSettings(objModule);
            ClearCache(objModule.TabID);
            return objModule.ModuleID;
        }
        public void CopyModule(int moduleId, int fromTabId, int toTabId, string toPaneName, bool includeSettings)
        {
            ModuleInfo objModule = GetModule(moduleId, fromTabId, false);
            if (String.IsNullOrEmpty(toPaneName))
            {
                toPaneName = objModule.PaneName;
            }
            try
            {
                dataProvider.AddTabModule(toTabId, moduleId, -1, toPaneName, objModule.CacheTime, objModule.CacheMethod, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile,
                (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate, objModule.IsWebSlice, objModule.WebSliceTitle, objModule.WebSliceExpiryDate, objModule.WebSliceTTL, UserController.GetCurrentUserInfo().UserID);
                if (includeSettings)
                {
                    ModuleInfo toModule = GetModule(moduleId, toTabId, false);
                    CopyTabModuleSettings(objModule, toModule);
                }
            }
            catch
            {
            }
            ClearCache(fromTabId);
            ClearCache(toTabId);
        }
        public void CopyModule(int moduleId, int fromTabId, List<TabInfo> toTabs, bool includeSettings)
        {
            foreach (TabInfo objTab in toTabs)
            {
                if (objTab.TabID != fromTabId)
                {
                    CopyModule(moduleId, fromTabId, objTab.TabID, "", includeSettings);
                }
            }
        }
        public void CopyTabModuleSettings(ModuleInfo fromModule, ModuleInfo toModule)
        {
            Hashtable settings = GetTabModuleSettings(fromModule.TabModuleID);
            foreach (DictionaryEntry setting in settings)
            {
                UpdateTabModuleSetting(toModule.TabModuleID, Convert.ToString(setting.Key), Convert.ToString(setting.Value));
            }
        }
        public void CreateContentItem(ModuleInfo updatedModule)
        {
            IContentTypeController typeController = new ContentTypeController();
            ContentType contentType = (from t in typeController.GetContentTypes()
                                       where t.Type == "Module"
                                       select t).SingleOrDefault();
            //This module does not have a valid ContentItem
            //create ContentItem
            IContentController contentController = CommonLibrary.Entities.Content.Common.Util.GetContentController();
            updatedModule.Content = updatedModule.ModuleTitle;
            updatedModule.Indexed = false;
            updatedModule.ContentTypeId = contentType.ContentTypeId;
            updatedModule.ContentItemId = contentController.AddContentItem(updatedModule);
        }
        public void DeleteAllModules(int moduleId, int tabId, List<TabInfo> fromTabs)
        {
            DeleteAllModules(moduleId, tabId, fromTabs, true, false, false);
        }
        public void DeleteAllModules(int moduleId, int tabId, List<TabInfo> fromTabs, bool softDelete, bool includeCurrent, bool deleteBaseModule)
        {
            foreach (TabInfo objTab in fromTabs)
            {
                if (objTab.TabID != tabId || includeCurrent)
                {
                    DeleteTabModule(objTab.TabID, moduleId, softDelete);
                }
            }
            if (includeCurrent && deleteBaseModule && !softDelete)
            {
                DeleteModule(moduleId);
            }
            ClearCache(tabId);
        }
        public void DeleteModule(int ModuleId)
        {
            //Get the module
            ModuleInfo objModule = GetModule(ModuleId);
            dataProvider.DeleteModule(ModuleId);
            //Remove the Content Item
            if (objModule != null && objModule.ContentItemId > Null.NullInteger)
            {
                IContentController ctl = CommonLibrary.Entities.Content.Common.Util.GetContentController();
                ctl.DeleteContentItem(objModule);
            }

            //Log deletion
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("ModuleId", ModuleId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.MODULE_DELETED);
            dataProvider.DeleteSearchItems(ModuleId);
        }
        public void DeleteTabModule(int tabId, int moduleId, bool softDelete)
        {
            ModuleInfo objModule = GetModule(moduleId, tabId, false);
            if (objModule != null)
            {
                dataProvider.DeleteTabModule(tabId, moduleId, softDelete);
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("tabId", tabId.ToString()));
                objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("moduleId", moduleId.ToString()));
                objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABMODULE_DELETED.ToString();
                objEventLog.AddLog(objEventLogInfo);
                UpdateTabModuleOrder(tabId);
                if (GetModule(moduleId, Null.NullInteger, true).TabID == Null.NullInteger)
                {
                    DeleteModule(moduleId);
                }
            }
            ClearCache(tabId);
        }
        public ArrayList GetAllModules()
        {
            return CBO.FillCollection(dataProvider.GetAllModules(), typeof(ModuleInfo));
        }
        public ModuleInfo GetModule(int moduleID)
        {
            return GetModule(moduleID, Null.NullInteger, true);
        }
        public ModuleInfo GetModule(int moduleID, int tabID)
        {
            return GetModule(moduleID, tabID, false);
        }
        public ModuleInfo GetModule(int moduleID, int tabID, bool ignoreCache)
        {
            ModuleInfo modInfo = null;
            bool bFound = false;
            if (!ignoreCache)
            {
                Dictionary<int, ModuleInfo> dicModules = GetTabModules(tabID);
                bFound = dicModules.TryGetValue(moduleID, out modInfo);
            }
            if (ignoreCache || !bFound)
            {
                modInfo = CBO.FillObject<ModuleInfo>(dataProvider.GetModule(moduleID, tabID));
            }
            return modInfo;
        }
        public ArrayList GetModules(int portalID)
        {
            return CBO.FillCollection(dataProvider.GetModules(portalID), typeof(ModuleInfo));
        }
        public ArrayList GetAllTabsModules(int portalID, bool allTabs)
        {
            return CBO.FillCollection(dataProvider.GetAllTabsModules(portalID, allTabs), typeof(ModuleInfo));
        }
        public ModuleInfo GetModuleByDefinition(int portalID, string friendlyName)
        {
            ModuleInfo objModule = null;
            string key = string.Format(DataCache.ModuleCacheKey, portalID);
            Dictionary<string, ModuleInfo> modules = DataCache.GetCache<Dictionary<string, ModuleInfo>>(key);
            if (modules == null)
            {
                modules = new Dictionary<string, ModuleInfo>();
            }
            if (modules.ContainsKey(friendlyName))
            {
                objModule = modules[friendlyName];
            }
            else
            {
                Dictionary<string, ModuleInfo> clonemodules = new Dictionary<string, ModuleInfo>();
                foreach (ModuleInfo module in modules.Values)
                {
                    clonemodules[module.ModuleDefinition.FriendlyName] = module;
                }
                IDataReader dr = DataProvider.Instance().GetModuleByDefinition(portalID, friendlyName);
                try
                {
                    objModule = CBO.FillObject<ModuleInfo>(dr);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
                if (objModule != null)
                {
                    clonemodules[objModule.ModuleDefinition.FriendlyName] = objModule;
                    Int32 timeOut = DataCache.ModuleCacheTimeOut * Convert.ToInt32(Host.Host.PerformanceSetting);
                    if (timeOut > 0)
                    {
                        DataCache.SetCache(key, clonemodules, TimeSpan.FromMinutes(timeOut));
                    }
                }
            }
            return objModule;
        }
        public ArrayList GetModulesByDefinition(int portalID, string friendlyName)
        {
            return CBO.FillCollection(DataProvider.Instance().GetModuleByDefinition(portalID, friendlyName), typeof(ModuleInfo));
        }
        public ArrayList GetSearchModules(int portalID)
        {
            return CBO.FillCollection(dataProvider.GetSearchModules(portalID), typeof(ModuleInfo));
        }
        public Dictionary<int, ModuleInfo> GetTabModules(int tabID)
        {
            string cacheKey = string.Format(DataCache.TabModuleCacheKey, tabID.ToString());
            return CBO.GetCachedObject<Dictionary<int, ModuleInfo>>(new CacheItemArgs(cacheKey, DataCache.TabModuleCacheTimeOut, DataCache.TabModuleCachePriority, tabID), GetTabModulesCallBack);
        }
        public void MoveModule(int moduleId, int fromTabId, int toTabId, string toPaneName)
        {
            ModuleInfo objModule = GetModule(moduleId, fromTabId);
            dataProvider.MoveTabModule(fromTabId, moduleId, toTabId, toPaneName, UserController.GetCurrentUserInfo().UserID);
            UpdateTabModuleOrder(fromTabId);
            UpdateTabModuleOrder(toTabId);
        }
        public void RestoreModule(ModuleInfo objModule)
        {
            dataProvider.RestoreTabModule(objModule.TabID, objModule.ModuleID);
            ClearCache(objModule.TabID);
        }
        private void UpdateModuleSettings(ModuleInfo updatedModule)
        {
            foreach (string sKey in updatedModule.ModuleSettings.Keys)
            {
                UpdateModuleSetting(updatedModule.ModuleID, sKey, Convert.ToString(updatedModule.ModuleSettings[sKey]));
            }
        }
        private void UpdateTabModuleSettings(ModuleInfo updatedTabModule)
        {
            foreach (string sKey in updatedTabModule.TabModuleSettings.Keys)
            {
                UpdateTabModuleSetting(updatedTabModule.TabModuleID, sKey, Convert.ToString(updatedTabModule.TabModuleSettings[sKey]));
            }
        }
        public void UpdateModule(ModuleInfo objModule)
        {
            //Update ContentItem If neccessary
            if (objModule.ContentItemId == Null.NullInteger && objModule.ModuleID != Null.NullInteger)
            {
                CreateContentItem(objModule);
            }
            dataProvider.UpdateModule(objModule.ModuleID, objModule.ContentItemId, objModule.ModuleTitle, objModule.AllTabs, objModule.Header, objModule.Footer, objModule.StartDate, objModule.EndDate, objModule.InheritViewPermissions, objModule.IsDeleted, UserController.GetCurrentUserInfo().UserID);
            //Update Tags
            ITermController termController = CommonLibrary.Entities.Content.Common.Util.GetTermController();
            termController.RemoveTermsFromContent(objModule);
            foreach (Term _Term in objModule.Terms)
            {
                termController.AddTermToContent(_Term, objModule);
            }
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objModule, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_UPDATED);
            ModulePermissionController.SaveModulePermissions(objModule);
            UpdateModuleSettings(objModule);
            if (!Null.IsNull(objModule.TabID))
            {
                dataProvider.UpdateTabModule(objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName, objModule.CacheTime, objModule.CacheMethod, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile,
                (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate, objModule.IsWebSlice, objModule.WebSliceTitle, objModule.WebSliceExpiryDate, objModule.WebSliceTTL, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(objModule, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABMODULE_UPDATED);
                UpdateModuleOrder(objModule.TabID, objModule.ModuleID, objModule.ModuleOrder, objModule.PaneName);
                if (PortalSettings.Current != null)
                {
                    if (objModule.IsDefaultModule)
                    {
                        if (objModule.ModuleID != PortalSettings.Current.DefaultModuleId)
                        {
                            PortalController.UpdatePortalSetting(objModule.PortalID, "defaultmoduleid", objModule.ModuleID.ToString());
                        }
                        if (objModule.TabID != PortalSettings.Current.DefaultTabId)
                        {
                            PortalController.UpdatePortalSetting(objModule.PortalID, "defaulttabid", objModule.TabID.ToString());
                        }
                    }
                    else
                    {
                        if (objModule.ModuleID == PortalSettings.Current.DefaultModuleId && objModule.TabID == PortalSettings.Current.DefaultTabId)
                        {
                            PortalController.DeletePortalSetting(objModule.PortalID, "defaultmoduleid");
                            PortalController.DeletePortalSetting(objModule.PortalID, "defaulttabid");
                        }
                    }
                }
                if (objModule.AllModules)
                {
                    TabController objTabs = new TabController();
                    foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(objModule.PortalID))
                    {
                        TabInfo objTab = tabPair.Value;
                        foreach (KeyValuePair<int, ModuleInfo> modulePair in GetTabModules(objTab.TabID))
                        {
                            ModuleInfo objTargetModule = modulePair.Value;
                            dataProvider.UpdateTabModule(objTargetModule.TabID, objTargetModule.ModuleID, objTargetModule.ModuleOrder, objTargetModule.PaneName, objTargetModule.CacheTime, objModule.CacheMethod, objModule.Alignment, objModule.Color, objModule.Border, objModule.IconFile,
                            (int)objModule.Visibility, objModule.ContainerSrc, objModule.DisplayTitle, objModule.DisplayPrint, objModule.DisplaySyndicate, objModule.IsWebSlice, objModule.WebSliceTitle, objModule.WebSliceExpiryDate, objModule.WebSliceTTL, UserController.GetCurrentUserInfo().UserID);
                        }
                    }
                }
            }
            foreach (ModuleInfo tabModule in GetModuleTabs(objModule.ModuleID))
            {
                ClearCache(tabModule.TabID);
            }
        }
        public void UpdateModuleOrder(int TabId, int ModuleId, int ModuleOrder, string PaneName)
        {
            ModuleInfo objModule = GetModule(ModuleId, TabId, false);
            if (objModule != null)
            {
                if (ModuleOrder == -1)
                {
                    IDataReader dr = null;
                    try
                    {
                        dr = dataProvider.GetTabModuleOrder(TabId, PaneName);
                        while (dr.Read())
                        {
                            ModuleOrder = Convert.ToInt32(dr["ModuleOrder"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Exceptions.LogException(ex);
                    }
                    finally
                    {
                        CBO.CloseDataReader(dr, true);
                    }
                    ModuleOrder += 2;
                }
                dataProvider.UpdateModuleOrder(TabId, ModuleId, ModuleOrder, PaneName);
                if (objModule.AllTabs == false)
                {
                    ClearCache(TabId);
                }
                else
                {
                    TabController objTabs = new TabController();
                    foreach (KeyValuePair<int, TabInfo> tabPair in objTabs.GetTabsByPortal(objModule.PortalID))
                    {
                        TabInfo objTab = tabPair.Value;
                        ClearCache(objTab.TabID);
                    }
                }
            }
        }
        public void UpdateTabModuleOrder(int TabId)
        {
            int ModuleCounter;
            IDataReader dr = null;
            dr = dataProvider.GetTabPanes(TabId);
            try
            {
                while (dr.Read())
                {
                    ModuleCounter = 0;
                    IDataReader dr2 = null;
                    dr2 = dataProvider.GetTabModuleOrder(TabId, Convert.ToString(dr["PaneName"]));
                    try
                    {
                        while (dr2.Read())
                        {
                            ModuleCounter += 1;
                            dataProvider.UpdateModuleOrder(TabId, Convert.ToInt32(dr2["ModuleID"]), (ModuleCounter * 2) - 1, Convert.ToString(dr["PaneName"]));
                        }
                    }
                    catch (Exception ex2)
                    {
                       Exceptions.LogException(ex2);
                    }
                    finally
                    {
                        CBO.CloseDataReader(dr2, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            ClearCache(TabId);
        }
        public ArrayList GetModuleTabs(int moduleID)
        {
            return CBO.FillCollection(dataProvider.GetModule(moduleID, Null.NullInteger), typeof(ModuleInfo));
        }
        public Hashtable GetModuleSettings(int ModuleId)
        {
            Hashtable objSettings;
            string strCacheKey = "GetModuleSettings" + ModuleId.ToString();
            objSettings = (Hashtable)DataCache.GetCache(strCacheKey);
            if (objSettings == null)
            {
                objSettings = new Hashtable();
                IDataReader dr = null;
                try
                {
                    dr = dataProvider.GetModuleSettings(ModuleId);
                    while (dr.Read())
                    {
                        if (!dr.IsDBNull(1))
                        {
                            objSettings[dr.GetString(0)] = dr.GetString(1);
                        }
                        else
                        {
                            objSettings[dr.GetString(0)] = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonLibrary.Services.Exceptions.Exceptions.LogException(ex);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
                int intCacheTimeout = 20 * Convert.ToInt32(Host.Host.PerformanceSetting);
                DataCache.SetCache(strCacheKey, objSettings, TimeSpan.FromMinutes(intCacheTimeout));
            }
            return objSettings;
        }
        public void UpdateModuleSetting(int ModuleId, string SettingName, string SettingValue)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("ModuleId", ModuleId.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingValue", SettingValue.ToString()));
            IDataReader dr = null;
            try
            {
                dr = dataProvider.GetModuleSetting(ModuleId, SettingName);
                if (dr.Read())
                {
                    dataProvider.UpdateModuleSetting(ModuleId, SettingName, SettingValue, UserController.GetCurrentUserInfo().UserID);
                    objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.MODULE_SETTING_UPDATED.ToString();
                    objEventLog.AddLog(objEventLogInfo);
                }
                else
                {
                    dataProvider.AddModuleSetting(ModuleId, SettingName, SettingValue, UserController.GetCurrentUserInfo().UserID);
                    objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.MODULE_SETTING_CREATED.ToString();
                    objEventLog.AddLog(objEventLogInfo);
                }
            }
            catch (Exception ex)
            {
                CommonLibrary.Services.Exceptions.Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            DataCache.RemoveCache("GetModuleSettings" + ModuleId.ToString());
        }
        public void DeleteModuleSetting(int ModuleId, string SettingName)
        {
            dataProvider.DeleteModuleSetting(ModuleId, SettingName);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("ModuleId", ModuleId.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString()));
            objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.MODULE_SETTING_DELETED.ToString();
            objEventLog.AddLog(objEventLogInfo);
            DataCache.RemoveCache("GetModuleSettings" + ModuleId.ToString());
        }
        public void DeleteModuleSettings(int ModuleId)
        {
            dataProvider.DeleteModuleSettings(ModuleId);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("ModuleId", ModuleId.ToString()));
            objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.MODULE_SETTING_DELETED.ToString();
            objEventLog.AddLog(objEventLogInfo);
            DataCache.RemoveCache("GetModuleSettings" + ModuleId.ToString());
        }
        public Hashtable GetTabModuleSettings(int TabModuleId)
        {
            string strCacheKey = "GetTabModuleSettings" + TabModuleId.ToString();
            Hashtable objSettings = (Hashtable)DataCache.GetCache(strCacheKey);
            if (objSettings == null)
            {
                objSettings = new Hashtable();
                IDataReader dr = null;
                try
                {
                    dr = dataProvider.GetTabModuleSettings(TabModuleId);
                    while (dr.Read())
                    {
                        if (!dr.IsDBNull(1))
                        {
                            objSettings[dr.GetString(0)] = dr.GetString(1);
                        }
                        else
                        {
                            objSettings[dr.GetString(0)] = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonLibrary.Services.Exceptions.Exceptions.LogException(ex);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
                int intCacheTimeout = 20 * Convert.ToInt32(Host.Host.PerformanceSetting);
                DataCache.SetCache(strCacheKey, objSettings, TimeSpan.FromMinutes(intCacheTimeout));
            }
            return objSettings;
        }
        public void UpdateTabModuleSetting(int TabModuleId, string SettingName, string SettingValue)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("TabModuleId", TabModuleId.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingValue", SettingValue.ToString()));
            IDataReader dr = null;
            try
            {
                dr = dataProvider.GetTabModuleSetting(TabModuleId, SettingName);
                if (dr.Read())
                {
                    dataProvider.UpdateTabModuleSetting(TabModuleId, SettingName, SettingValue, UserController.GetCurrentUserInfo().UserID);
                    objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABMODULE_SETTING_UPDATED.ToString();
                    objEventLog.AddLog(objEventLogInfo);
                }
                else
                {
                    dataProvider.AddTabModuleSetting(TabModuleId, SettingName, SettingValue, UserController.GetCurrentUserInfo().UserID);
                    objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABMODULE_SETTING_CREATED.ToString();
                    objEventLog.AddLog(objEventLogInfo);
                }
            }
            catch (Exception ex)
            {
                CommonLibrary.Services.Exceptions.Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            DataCache.RemoveCache("GetTabModuleSettings" + TabModuleId.ToString());
        }
        public void DeleteTabModuleSetting(int TabModuleId, string SettingName)
        {
            dataProvider.DeleteTabModuleSetting(TabModuleId, SettingName);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("TabModuleId", TabModuleId.ToString()));
            objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("SettingName", SettingName.ToString()));
            objEventLogInfo.LogTypeKey = CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABMODULE_SETTING_DELETED.ToString();
            objEventLog.AddLog(objEventLogInfo);
            DataCache.RemoveCache("GetTabModuleSettings" + TabModuleId.ToString());
        }
        public void DeleteTabModuleSettings(int TabModuleId)
        {
            dataProvider.DeleteTabModuleSettings(TabModuleId);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("TabModuleID", TabModuleId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.TABMODULE_SETTING_DELETED);
            DataCache.RemoveCache("GetTabModuleSettings" + TabModuleId.ToString());
        }

        #region Methods ====================================================================================
        public DataTable GetListByPortalId(string PortalId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModuleControls_GetListByPortalId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByModuleId(int ModuleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModuleControls_GetListByModuleId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int ModuleControlId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModuleControls_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleControlId", ModuleControlId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int ModuleId, string ControlTitle, string ControlKey, string ControlSrc, int ControlType, string IconFile)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModuleControls_Add", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.AddWithValue("@ControlTitle", ControlTitle);
            cmd.Parameters.AddWithValue("@ControlKey", ControlKey);
            cmd.Parameters.AddWithValue("@ControlSrc", ControlSrc);
            cmd.Parameters.AddWithValue("@ControlType", ControlType);
            cmd.Parameters.AddWithValue("@IconFile", IconFile);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int ModuleId, int ModuleControlId, string ControlTitle, string ControlKey, string ControlSrc, int ControlType, string IconFile)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModuleControls_Edit", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleControlId", ModuleControlId);
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.AddWithValue("@ControlTitle", ControlTitle);
            cmd.Parameters.AddWithValue("@ControlKey", ControlKey);
            cmd.Parameters.AddWithValue("@ControlSrc", ControlSrc);
            cmd.Parameters.AddWithValue("@ControlType", ControlType);
            cmd.Parameters.AddWithValue("@IconFile", IconFile);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int ModuleControlId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_ModuleControls_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ModuleControlId", ModuleControlId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
        #endregion =========================================================================================
    }
}
