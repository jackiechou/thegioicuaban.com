using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Common;
using CommonLibrary.Entities.Modules.Definitions;
using CommonLibrary.Entities.Users;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Tabs;
using System.Collections;
using CommonLibrary.Security;

namespace CommonLibrary.Services.ControlPanels
{
    public class ControlPanelBase : System.Web.UI.UserControl
    {
        protected enum ViewPermissionType
        {
            View = 0,
            Edit = 1
        }
        private string _localResourceFile;
        protected bool IsModuleAdmin()
        {
            bool _IsModuleAdmin = Null.NullBoolean;
            foreach (ModuleInfo objModule in TabController.CurrentPage.Modules)
            {
                if (!objModule.IsDeleted)
                {
                    bool blnHasModuleEditPermissions = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, Null.NullString, objModule);
                    if (blnHasModuleEditPermissions == true && objModule.ModuleDefinition.DefaultCacheTime != -1)
                    {
                        _IsModuleAdmin = true;
                        break;
                    }
                }
            }
            return PortalSettings.ControlPanelSecurity == PortalSettings.ControlPanelPermission.ModuleEditor && _IsModuleAdmin;
        }
        protected bool IsPageAdmin()
        {
            bool _IsPageAdmin = Null.NullBoolean;
            if (TabPermissionController.CanAddContentToPage() || TabPermissionController.CanAddPage() || TabPermissionController.CanAdminPage() || TabPermissionController.CanCopyPage() || TabPermissionController.CanDeletePage() || TabPermissionController.CanExportPage() || TabPermissionController.CanImportPage() || TabPermissionController.CanManagePage())
            {
                _IsPageAdmin = true;
            }
            return _IsPageAdmin;
        }
        protected bool IsVisible
        {
            get { return PortalSettings.ControlPanelVisible; }
        }
        protected PortalSettings PortalSettings
        {
            get { return PortalController.GetCurrentPortalSettings(); }
        }
        protected PortalSettings.Mode UserMode
        {
            get { return PortalSettings.UserMode; }
        }
        private ModulePermissionInfo AddModulePermission(ModuleInfo objModule, PermissionInfo permission, int roleId, int userId, bool allowAccess)
        {
            ModulePermissionInfo objModulePermission = new ModulePermissionInfo();
            objModulePermission.ModuleID = objModule.ModuleID;
            objModulePermission.PermissionID = permission.PermissionID;
            objModulePermission.RoleID = roleId;
            objModulePermission.UserID = userId;
            objModulePermission.PermissionKey = permission.PermissionKey;
            objModulePermission.AllowAccess = allowAccess;
            if (!objModule.ModulePermissions.Contains(objModulePermission))
            {
                objModule.ModulePermissions.Add(objModulePermission);
            }
            return objModulePermission;
        }
        protected void AddExistingModule(int moduleId, int tabId, string paneName, int position, string align)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule;
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            int UserId = -1;
            if (Request.IsAuthenticated)
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                UserId = objUserInfo.UserID;
            }
            objModule = objModules.GetModule(moduleId, tabId, false);
            if (objModule != null)
            {
                ModuleInfo objClone = objModule.Clone();
                objClone.TabID = PortalSettings.ActiveTab.TabID;
                objClone.ModuleOrder = position;
                objClone.PaneName = paneName;
                objClone.Alignment = align;
                objModules.AddModule(objClone);
                objEventLog.AddLog(objClone, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_CREATED);
            }
        }
        protected void AddNewModule(string title, int desktopModuleId, string paneName, int position, ViewPermissionType permissionType, string align)
        {
            TabPermissionCollection objTabPermissions = PortalSettings.ActiveTab.TabPermissions;
            PermissionController objPermissionController = new PermissionController();
            ModuleController objModules = new ModuleController();
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            int j;
            try
            {
                DesktopModuleInfo desktopModule = null;
                if (!DesktopModuleController.GetDesktopModules(PortalSettings.PortalId).TryGetValue(desktopModuleId, out desktopModule))
                {
                    throw new ArgumentException("desktopModuleId");
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
            }
            int UserId = -1;
            if (Request.IsAuthenticated)
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                UserId = objUserInfo.UserID;
            }
            foreach (ModuleDefinitionInfo objModuleDefinition in ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(desktopModuleId).Values)
            {
                ModuleInfo objModule = new ModuleInfo();
                objModule.Initialize(PortalSettings.PortalId);
                objModule.PortalID = PortalSettings.PortalId;
                objModule.TabID = PortalSettings.ActiveTab.TabID;
                objModule.ModuleOrder = position;
                if (String.IsNullOrEmpty(title))
                {
                    objModule.ModuleTitle = objModuleDefinition.FriendlyName;
                }
                else
                {
                    objModule.ModuleTitle = title;
                }
                objModule.PaneName = paneName;
                objModule.ModuleDefID = objModuleDefinition.ModuleDefID;
                if (objModuleDefinition.DefaultCacheTime > 0)
                {
                    objModule.CacheTime = objModuleDefinition.DefaultCacheTime;
                    if (PortalSettings.Current.DefaultModuleId > Null.NullInteger && PortalSettings.Current.DefaultTabId > Null.NullInteger)
                    {
                        ModuleInfo defaultModule = objModules.GetModule(PortalSettings.Current.DefaultModuleId, PortalSettings.Current.DefaultTabId, true);
                        if (defaultModule != null)
                        {
                            objModule.CacheTime = defaultModule.CacheTime;
                        }
                    }
                }
                switch (permissionType)
                {
                    case ViewPermissionType.View:
                        objModule.InheritViewPermissions = true;
                        break;
                    case ViewPermissionType.Edit:
                        objModule.InheritViewPermissions = false;
                        break;
                }
                ArrayList arrSystemModuleViewPermissions = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW");
                foreach (TabPermissionInfo objTabPermission in objTabPermissions)
                {
                    if (objTabPermission.PermissionKey == "VIEW" && permissionType == ViewPermissionType.View)
                    {
                        continue;
                    }
                    ArrayList arrSystemModulePermissions = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", objTabPermission.PermissionKey);
                    for (j = 0; j <= arrSystemModulePermissions.Count - 1; j++)
                    {
                        PermissionInfo objSystemModulePermission;
                        objSystemModulePermission = (PermissionInfo)arrSystemModulePermissions[j];
                        if (objSystemModulePermission.PermissionKey == "VIEW" && permissionType == ViewPermissionType.Edit && objTabPermission.PermissionKey != "EDIT")
                        {
                            continue;
                        }
                        ModulePermissionInfo objModulePermission = AddModulePermission(objModule, objSystemModulePermission, objTabPermission.RoleID, objTabPermission.UserID, objTabPermission.AllowAccess);
                        if (objModulePermission.PermissionKey == "EDIT" && objModulePermission.AllowAccess)
                        {
                            ModulePermissionInfo objModuleViewperm = AddModulePermission(objModule, (PermissionInfo)arrSystemModuleViewPermissions[0], objModulePermission.RoleID, objModulePermission.UserID, true);
                        }
                    }
                    if (objTabPermission.PermissionKey == "EDIT")
                    {
                        ArrayList arrCustomModulePermissions = objPermissionController.GetPermissionsByModuleDefID(objModule.ModuleDefID);
                        for (j = 0; j <= arrCustomModulePermissions.Count - 1; j++)
                        {
                            PermissionInfo objCustomModulePermission;
                            objCustomModulePermission = (PermissionInfo)arrCustomModulePermissions[j];
                            AddModulePermission(objModule, objCustomModulePermission, objTabPermission.RoleID, objTabPermission.UserID, objTabPermission.AllowAccess);
                        }
                    }
                }
                objModule.AllTabs = false;
                objModule.Alignment = align;
                objModules.AddModule(objModule);
            }
        }
        protected string BuildURL(int PortalID, string FriendlyName)
        {
            string strURL = "~/" + Globals.glbDefaultPage;
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModuleByDefinition(PortalID, FriendlyName);
            if (objModule != null)
            {
                if (PortalID == Null.NullInteger)
                {
                    strURL = Globals.NavigateURL(objModule.TabID, true);
                }
                else
                {
                    strURL = Globals.NavigateURL(objModule.TabID);
                }
            }
            return strURL;
        }
        protected bool GetModulePermission(int PortalID, string FriendlyName)
        {
            bool AllowAccess = Null.NullBoolean;
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = objModules.GetModuleByDefinition(PortalID, FriendlyName);
            if (objModule != null)
            {
                AllowAccess = ModulePermissionController.CanViewModule(objModule);
            }
            return AllowAccess;
        }
        protected void SetUserMode(string userMode)
        {
            Personalization.Personalization.SetProfile("Usability", "UserMode" + PortalSettings.PortalId.ToString(), userMode.ToUpper());
        }
        protected void SetVisibleMode(bool isVisible)
        {
            Personalization.Personalization.SetProfile("Usability", "ControlPanelVisible" + PortalSettings.PortalId.ToString(), isVisible.ToString());
        }
        public string LocalResourceFile
        {
            get
            {
                string fileRoot;
                if (String.IsNullOrEmpty(_localResourceFile))
                {
                    fileRoot = this.TemplateSourceDirectory + "/" + Services.Localization.Localization.LocalResourceDirectory + "/" + this.ID;
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set { _localResourceFile = value; }
        }
  
    }
}
