using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Services.Installer.Packages;
using CommonLibrary.Entities.Modules.Definitions;
using CommonLibrary.Security.Permissions;
using System.Collections;
using System.Xml;
using CommonLibrary.Services.FileSystem;
using System.IO;
using CommonLibrary.Services.Installer;
using CommonLibrary.Services.Localization;
using CommonLibrary.Entities.Users;
using System.Xml.XPath;
using CommonLibrary.Services.EventQueue.Config;
using CommonLibrary.Application;
using CommonLibrary.Data;
using System.Web;
using CommonLibrary.Security;
using System.Data;
using System.Data.SqlClient;
using CommonLibrary.Common.Lists;
using CommonLibrary.Entities.Profile;
using CommonLibrary.Entities.Host;

namespace CommonLibrary.Services.Upgrade
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Upgrade class provides Shared/Static methods to Upgrade/Install
    ///	a DotNetNuke Application
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// [cnurse]	11/6/2004	documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class Upgrade
    {

        #region "Private Shared Field"

        private static DateTime startTime;
        private static bool upgradeMemberShipProvider = true;

        #endregion

        #region "Public Property"

        public static string DefaultProvider
        {
            get { return Config.GetDefaultProvider("data").Name; }
        }

        public static TimeSpan RunTime
        {
            get
            {
                DateTime currentTime = DateTime.Now;
                return currentTime.Subtract(startTime);
            }
        }

        #endregion

        #region "Private Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddAdminPages adds an Admin Page and an associated Module to all configured Portals
        /// </summary>
        ///	<param name="TabName">The Name to give this new Tab</param>
        ///	<param name="TabIconFile">The Icon for this new Tab</param>
        ///	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        ///	<param name="ModuleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        ///	<param name="ModuleTitle">The Module's title</param>
        ///	<param name="ModuleIconFile">The Module's icon</param>
        /// <history>
        /// [cnurse]	11/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void AddAdminPages(string TabName, string Description, string TabIconFile, string TabIconFileLarge, bool IsVisible, int ModuleDefId, string ModuleTitle, string ModuleIconFile)
        {

            //Call overload with InheritPermisions=True
            AddAdminPages(TabName, Description, TabIconFile, TabIconFileLarge, IsVisible, ModuleDefId, ModuleTitle, ModuleIconFile, true);
        }

        private static void AddAdminRoleToPage(string tabPath)
        {
            PortalController portalCtrl = new PortalController();
            TabController tabCtrl = new TabController();
            int tabID = 0;
            TabInfo tab = default(TabInfo);

            foreach (PortalInfo portal in portalCtrl.GetPortals())
            {
                tabID = TabController.GetTabByTabPath(portal.PortalID, tabPath);
                if ((tabID != Null.NullInteger))
                {
                    tab = tabCtrl.GetTab(tabID, portal.PortalID, true);

                    if ((tab.TabPermissions.Count == 0))
                    {
                        AddPagePermission(tab.TabPermissions, "View", Convert.ToInt32(portal.AdministratorRoleId));
                        AddPagePermission(tab.TabPermissions, "Edit", Convert.ToInt32(portal.AdministratorRoleId));
                        TabPermissionController.SaveTabPermissions(tab);
                    }
                }
            }
        }

        private static void AddConsoleModuleSettings(int tabID, int moduleID)
        {
            ModuleController modCtrl = new ModuleController();

            modCtrl.UpdateModuleSetting(moduleID, "DefaultSize", "IconFileLarge");
            modCtrl.UpdateModuleSetting(moduleID, "AllowSizeChange", "False");
            modCtrl.UpdateModuleSetting(moduleID, "DefaultView", "Hide");
            modCtrl.UpdateModuleSetting(moduleID, "AllowViewChange", "False");
            modCtrl.UpdateModuleSetting(moduleID, "ShowTooltip", "True");
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddModuleControl adds a new Module Control to the system
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="ModuleDefId">The Module Definition Id</param>
        ///	<param name="ControlKey">The key for this control in the Definition</param>
        ///	<param name="ControlTitle">The title of this control</param>
        ///	<param name="ControlSrc">Te source of ths control</param>
        ///	<param name="IconFile">The icon file</param>
        ///	<param name="ControlType">The type of control</param>
        ///	<param name="ViewOrder">The vieworder for this module</param>
        ///	<param name="HelpURL">The Help Url</param>
        /// <history>
        /// [cnurse]	11/08/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void AddModuleControl(int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, SecurityAccessLevel ControlType, int ViewOrder, string HelpURL)
        {

            // check if module control exists
            ModuleControlInfo objModuleControl = ModuleControlController.GetModuleControlByControlKey(ControlKey, ModuleDefId);
            if (objModuleControl == null)
            {
                objModuleControl = new ModuleControlInfo();

                objModuleControl.ModuleControlID = Null.NullInteger;
                objModuleControl.ModuleDefID = ModuleDefId;
                objModuleControl.ControlKey = ControlKey;
                objModuleControl.ControlTitle = ControlTitle;
                objModuleControl.ControlSrc = ControlSrc;
                objModuleControl.ControlType = ControlType;
                objModuleControl.ViewOrder = ViewOrder;
                objModuleControl.IconFile = IconFile;
                objModuleControl.HelpURL = HelpURL;

                ModuleControlController.AddModuleControl(objModuleControl);
            }
        }

        private static void AddModuleControl(int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, SecurityAccessLevel ControlType, int ViewOrder, string HelpURL, bool SupportsPartialRendering)
        {

            // check if module control exists
            ModuleControlInfo objModuleControl = ModuleControlController.GetModuleControlByControlKey(ControlKey, ModuleDefId);
            if (objModuleControl == null)
            {
                objModuleControl = new ModuleControlInfo();

                objModuleControl.ModuleControlID = Null.NullInteger;
                objModuleControl.ModuleDefID = ModuleDefId;
                objModuleControl.ControlKey = ControlKey;
                objModuleControl.ControlTitle = ControlTitle;
                objModuleControl.ControlSrc = ControlSrc;
                objModuleControl.ControlType = ControlType;
                objModuleControl.ViewOrder = ViewOrder;
                objModuleControl.IconFile = IconFile;
                objModuleControl.SupportsPartialRendering = SupportsPartialRendering;

                ModuleControlController.AddModuleControl(objModuleControl);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddModuleDefinition adds a new Core Module Definition to the system
        /// </summary>
        /// <remarks>
        ///	This overload allows the caller to determine whether the module has a controller
        /// class
        /// </remarks>
        ///	<param name="DesktopModuleName">The Friendly Name of the Module to Add</param>
        ///	<param name="Description">Description of the Module</param>
        ///	<param name="ModuleDefinitionName">The Module Definition Name</param>
        ///	<param name="Premium">A flag representing whether the module is a Premium module</param>
        ///	<param name="Admin">A flag representing whether the module is an Admin module</param>
        ///	<returns>The Module Definition Id of the new Module</returns>
        /// <history>
        /// [cnurse]	10/14/2004	documented
        /// [cnurse] 11/11/2004 removed addition of Module Control (now in AddMOduleControl)
        /// </history>
        /// -----------------------------------------------------------------------------
        private static int AddModuleDefinition(string DesktopModuleName, string Description, string ModuleDefinitionName, bool Premium, bool Admin)
        {
            return AddModuleDefinition(DesktopModuleName, Description, ModuleDefinitionName, "", false, Premium, Admin);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddModuleDefinition adds a new Core Module Definition to the system
        /// </summary>
        /// <remarks>
        ///	This overload allows the caller to determine whether the module has a controller
        /// class
        /// </remarks>
        ///	<param name="DesktopModuleName">The Friendly Name of the Module to Add</param>
        ///	<param name="Description">Description of the Module</param>
        ///	<param name="ModuleDefinitionName">The Module Definition Name</param>
        ///	<param name="Premium">A flag representing whether the module is a Premium module</param>
        ///	<param name="Admin">A flag representing whether the module is an Admin module</param>
        ///	<returns>The Module Definition Id of the new Module</returns>
        /// <history>
        /// [cnurse]	10/14/2004	documented
        /// [cnurse] 11/11/2004 removed addition of Module Control (now in AddMOduleControl)
        /// </history>
        /// -----------------------------------------------------------------------------
        private static int AddModuleDefinition(string DesktopModuleName, string Description, string ModuleDefinitionName, string BusinessControllerClass, bool IsPortable, bool Premium, bool Admin)
        {
            // check if desktop module exists
            DesktopModuleInfo objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModuleName, Null.NullInteger);
            if (objDesktopModule == null)
            {
                PackageInfo package = new PackageInfo();
                package.Description = Description;
                package.FriendlyName = DesktopModuleName;
                package.Name = string.Concat("DotNetNuke.", DesktopModuleName);
                package.PackageType = "Module";
                package.Owner = "DotNetNuke";
                package.Organization = "DotNetNuke Corporation";
                package.Url = "www.dotnetnuke.com";
                package.Email = "support@dotnetnuke.com";
                if (DesktopModuleName == "Extensions" || DesktopModuleName == "Skin Designer" || DesktopModuleName == "Dashboard")
                {
                    package.IsSystemPackage = true;
                }
                package.Version = new System.Version(1, 0, 0);

                package.PackageID = PackageController.AddPackage(package, false);

                string moduleName = DesktopModuleName.Replace(" ", "");
                objDesktopModule = new DesktopModuleInfo();
                objDesktopModule.DesktopModuleID = Null.NullInteger;
                objDesktopModule.PackageID = package.PackageID;
                objDesktopModule.FriendlyName = DesktopModuleName;
                objDesktopModule.FolderName = "Admin/" + moduleName;
                objDesktopModule.ModuleName = moduleName;
                objDesktopModule.Description = Description;
                objDesktopModule.Version = "01.00.00";
                objDesktopModule.BusinessControllerClass = BusinessControllerClass;
                objDesktopModule.IsPortable = IsPortable;
                objDesktopModule.SupportedFeatures = 0;
                if ((IsPortable))
                {
                    objDesktopModule.SupportedFeatures = 1;
                }
                objDesktopModule.IsPremium = Premium;
                objDesktopModule.IsAdmin = Admin;

                objDesktopModule.DesktopModuleID = DesktopModuleController.SaveDesktopModule(objDesktopModule, false, false);

                if (!Premium)
                {
                    DesktopModuleController.AddDesktopModuleToPortals(objDesktopModule.DesktopModuleID);
                }
            }

            // check if module definition exists
            ModuleDefinitionInfo objModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(ModuleDefinitionName, objDesktopModule.DesktopModuleID);
            if (objModuleDefinition == null)
            {
                objModuleDefinition = new ModuleDefinitionInfo();

                objModuleDefinition.ModuleDefID = Null.NullInteger;
                objModuleDefinition.DesktopModuleID = objDesktopModule.DesktopModuleID;
                objModuleDefinition.FriendlyName = ModuleDefinitionName;

                objModuleDefinition.ModuleDefID = ModuleDefinitionController.SaveModuleDefinition(objModuleDefinition, false, false);
            }

            return objModuleDefinition.ModuleDefID;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddModuleToPage adds a module to a Page
        /// </summary>
        /// <remarks>
        /// This overload assumes ModulePermissions will be inherited
        /// </remarks>
        ///	<param name="page">The Page to add the Module to</param>
        ///	<param name="ModuleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        ///	<param name="ModuleTitle">The Module's title</param>
        ///	<param name="ModuleIconFile">The Module's icon</param>
        /// <history>
        /// [cnurse]	11/11/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static int AddModuleToPage(TabInfo page, int ModuleDefId, string ModuleTitle, string ModuleIconFile)
        {
            //Call overload with InheritPermisions=True
            return AddModuleToPage(page, ModuleDefId, ModuleTitle, ModuleIconFile, true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddPage adds a Tab Page
        /// </summary>
        /// <remarks>
        /// Adds a Tab to a parentTab
        /// </remarks>
        ///	<param name="parentTab">The Parent Tab</param>
        ///	<param name="TabName">The Name to give this new Tab</param>
        ///	<param name="TabIconFile">The Icon for this new Tab</param>
        ///	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        ///	<param name="permissions">Page Permissions Collection for this page</param>
        /// <param name="IsAdmin">Is an admin page</param>
        /// <history>
        /// [cnurse]	11/11/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static TabInfo AddPage(TabInfo parentTab, string TabName, string Description, string TabIconFile, string TabIconFileLarge, bool IsVisible, Security.Permissions.TabPermissionCollection permissions, bool IsAdmin)
        {

            int ParentId = Null.NullInteger;
            int PortalId = Null.NullInteger;

            if ((parentTab != null))
            {
                ParentId = parentTab.TabID;
                PortalId = parentTab.PortalID;
            }


            return AddPage(PortalId, ParentId, TabName, Description, TabIconFile, TabIconFileLarge, IsVisible, permissions, IsAdmin);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddPage adds a Tab Page
        /// </summary>
        ///	<param name="PortalId">The Id of the Portal</param>
        ///	<param name="ParentId">The Id of the Parent Tab</param>
        ///	<param name="TabName">The Name to give this new Tab</param>
        ///	<param name="TabIconFile">The Icon for this new Tab</param>
        ///	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        ///	<param name="permissions">Page Permissions Collection for this page</param>
        /// <param name="IsAdmin">Is and admin page</param>
        /// <history>
        /// [cnurse]	11/11/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static TabInfo AddPage(int PortalId, int ParentId, string TabName, string Description, string TabIconFile, string TabIconFileLarge, bool IsVisible, Security.Permissions.TabPermissionCollection permissions, bool IsAdmin)
        {
            TabController objTabs = new TabController();
            TabInfo objTab = default(TabInfo);

            objTab = objTabs.GetTabByName(TabName, PortalId, ParentId);

            if (objTab == null || objTab.ParentId != ParentId)
            {
                objTab = new TabInfo();
                objTab.TabID = Null.NullInteger;
                objTab.PortalID = PortalId;
                objTab.TabName = TabName;
                objTab.Title = "";
                objTab.Description = Description;
                objTab.KeyWords = "";
                objTab.IsVisible = IsVisible;
                objTab.DisableLink = false;
                objTab.ParentId = ParentId;
                objTab.IconFile = TabIconFile;
                objTab.IconFileLarge = TabIconFileLarge;
                objTab.IsDeleted = false;
                objTab.TabID = objTabs.AddTab(objTab, !IsAdmin);

                if (((permissions != null)))
                {
                    Security.Permissions.TabPermissionController tabPermissionCtrl = new Security.Permissions.TabPermissionController();
                    foreach (TabPermissionInfo tabPermission in permissions)
                    {
                        objTab.TabPermissions.Add(tabPermission, true);
                    }
                    TabPermissionController.SaveTabPermissions(objTab);
                }
            }

            return objTab;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddPagePermission adds a TabPermission to a TabPermission Collection
        /// </summary>
        ///	<param name="permissions">Page Permissions Collection for this page</param>
        ///	<param name="key">The Permission key</param>
        ///	<param name="roleId">The role given the permission</param>
        /// <history>
        /// [cnurse]	11/11/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void AddPagePermission(Security.Permissions.TabPermissionCollection permissions, string key, int roleId)
        {


            Security.Permissions.PermissionController objPermissionController = new Security.Permissions.PermissionController();
            Security.Permissions.PermissionInfo objPermission = (Security.Permissions.PermissionInfo)objPermissionController.GetPermissionByCodeAndKey("SYSTEM_TAB", key)[0];

            Security.Permissions.TabPermissionInfo objTabPermission = new Security.Permissions.TabPermissionInfo();
            objTabPermission.PermissionID = objPermission.PermissionID;
            objTabPermission.RoleID = roleId;
            objTabPermission.AllowAccess = true;

            permissions.Add(objTabPermission);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddSearchResults adds a top level Hidden Search Results Page
        /// </summary>
        ///	<param name="ModuleDefId">The Module Deinition Id for the Search Results Module</param>
        /// <history>
        /// [cnurse]	11/11/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void AddSearchResults(int ModuleDefId)
        {

            PortalController objPortals = new PortalController();
            PortalInfo objPortal = default(PortalInfo);
            ArrayList arrPortals = objPortals.GetPortals();
            int intPortal = 0;
            TabInfo newPage = default(TabInfo);

            //Add Page to Admin Menu of all configured Portals
            for (intPortal = 0; intPortal <= arrPortals.Count - 1; intPortal++)
            {
                Security.Permissions.TabPermissionCollection objTabPermissions = new Security.Permissions.TabPermissionCollection();

                objPortal = (PortalInfo)arrPortals[intPortal];

                AddPagePermission(objTabPermissions, "View", Convert.ToInt32(Common.Globals.glbRoleAllUsers));
                AddPagePermission(objTabPermissions, "View", Convert.ToInt32(objPortal.AdministratorRoleId));
                AddPagePermission(objTabPermissions, "Edit", Convert.ToInt32(objPortal.AdministratorRoleId));

                //Create New Page (or get existing one)
                newPage = AddPage(objPortal.PortalID, Null.NullInteger, "Search Results", "", "", "", false, objTabPermissions, false);

                //Add Module To Page
                AddModuleToPage(newPage, ModuleDefId, "Search Results", "");

            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddSkinControl adds a new Module Control to the system
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="ControlKey">The key for this control in the Definition</param>
        ///	<param name="ControlSrc">Te source of ths control</param>
        /// <history>
        /// [cnurse]	05/26/2008	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void AddSkinControl(string ControlKey, string PackageName, string ControlSrc)
        {

            // check if skin control exists
            SkinControlInfo skinControl = SkinControlController.GetSkinControlByKey(ControlKey);
            if (skinControl == null)
            {
                PackageInfo package = new PackageInfo();
                package.Name = PackageName;
                package.FriendlyName = string.Concat(ControlKey, "SkinObject");
                package.PackageType = "SkinObject";
                package.Version = new Version(1, 0, 0);
                LegacyUtil.ParsePackageName(package);

                int PackageId = PackageController.AddPackage(package, false);

                skinControl = new SkinControlInfo();

                skinControl.PackageID = PackageId;
                skinControl.ControlKey = ControlKey;
                skinControl.ControlSrc = ControlSrc;
                skinControl.SupportsPartialRendering = false;

                SkinControlController.SaveSkinControl(skinControl);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CoreModuleExists determines whether a Core Module exists on the system
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="DesktopModuleName">The Friendly Name of the Module</param>
        ///	<returns>True if the Module exists, otherwise False</returns>
        /// <history>
        /// [cnurse]	10/14/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private static bool CoreModuleExists(string DesktopModuleName)
        {
            bool blnExists = false;
            DesktopModuleInfo objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModuleName, Null.NullInteger);

            return ((objDesktopModule != null));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExecuteScript executes a SQl script file
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strScriptFile">The script to Execute</param>
        /// <history>
        /// [cnurse]	11/09/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static string ExecuteScript(string strScriptFile, bool writeFeedback)
        {
            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Executing Script: " + Path.GetFileName(strScriptFile));
            }

            string strExceptions = null;

            // read script file for installation
            string strScript = FileSystemUtils.ReadFile(strScriptFile);

            // execute SQL installation script
            strExceptions = DataProvider.Instance().ExecuteScript(strScript);

            // log the results
            try
            {
                StreamWriter objStream = default(StreamWriter);
                objStream = File.CreateText(strScriptFile.Replace("." + DefaultProvider, "") + ".log.resources");
                objStream.WriteLine(strExceptions);
                objStream.Close();
            }
            catch
            {
            }
            // does not have permission to create the log file

            if (writeFeedback)
            {
                HtmlUtils.WriteScriptSuccessError(HttpContext.Current.Response, (string.IsNullOrEmpty(strExceptions)), Path.GetFileName(strScriptFile).Replace("." + DefaultProvider, ".log.resources"));
            }

            return strExceptions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetModuleDefinition gets the Module Definition Id of a module
        /// </summary>
        ///	<param name="DesktopModuleName">The Friendly Name of the Module to Add</param>
        ///	<param name="ModuleDefinitionName">The Module Definition Name</param>
        ///	<returns>The Module Definition Id of the Module (-1 if no module definition)</returns>
        /// <history>
        /// [cnurse]	11/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static int GetModuleDefinition(string DesktopModuleName, string ModuleDefinitionName)
        {
            // get desktop module
            DesktopModuleInfo objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModuleName, Null.NullInteger);
            if (objDesktopModule == null)
            {
                return -1;
            }

            // get module definition
            ModuleDefinitionInfo objModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(ModuleDefinitionName, objDesktopModule.DesktopModuleID);
            if (objModuleDefinition == null)
            {
                return -1;
            }


            return objModuleDefinition.ModuleDefID;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// HostTabExists determines whether a tab of a given name exists under the Host tab
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="TabName">The Name of the Tab</param>
        ///	<returns>True if the Tab exists, otherwise False</returns>
        /// <history>
        /// [cnurse]	11/08/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private static bool HostTabExists(string TabName)
        {

            bool blnExists = false;

            TabController objTabController = new TabController();

            TabInfo objTabInfo = objTabController.GetTabByName(TabName, Null.NullInteger);
            if ((objTabInfo != null))
            {
                blnExists = true;
            }
            else
            {
                blnExists = false;
            }


            return blnExists;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// InstallMemberRoleProvider - Installs the MemberRole Provider Db objects
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strProviderPath">The Path to the Provider Directory</param>
        /// <history>
        /// [cnurse]	02/02/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static string InstallMemberRoleProvider(string strProviderPath, bool writeFeedback)
        {
            string strExceptions = "";

            bool installMemberRole = true;
            if ((Config.GetSetting("InstallMemberRole") != null))
            {
                installMemberRole = bool.Parse(Config.GetSetting("InstallMemberRole"));
            }

            if (installMemberRole)
            {
                if (writeFeedback)
                {
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Installing MemberRole Provider:<br>");
                }

                //Install Common
                strExceptions += InstallMemberRoleProviderScript(strProviderPath, "InstallCommon", writeFeedback);
                //Install Membership
                strExceptions += InstallMemberRoleProviderScript(strProviderPath, "InstallMembership", writeFeedback);
                //Install Profile
                strExceptions += InstallMemberRoleProviderScript(strProviderPath, "InstallProfile", writeFeedback);
                //Install Roles
                strExceptions += InstallMemberRoleProviderScript(strProviderPath, "InstallRoles", writeFeedback);

                //As we have just done an Install set the Upgrade Flag to false
                upgradeMemberShipProvider = false;
            }

            return strExceptions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// InstallMemberRoleProviderScript - Installs a specific MemberRole Provider script
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="providerPath">The Path to the Provider Directory</param>
        ///	<param name="scriptFile">The Name of the Script File</param>
        ///	<param name="writeFeedback">Whether or not to echo results</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        private static string InstallMemberRoleProviderScript(string providerPath, string scriptFile, bool writeFeedback)
        {
            string strExceptions = "";

            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Executing Script: " + scriptFile + "<br>");
            }

            strExceptions = DataProvider.Instance().ExecuteScript(FileSystemUtils.ReadFile(providerPath + scriptFile + ".sql"));

            // log the results
            try
            {
                StreamWriter objStream = default(StreamWriter);
                objStream = File.CreateText(providerPath + scriptFile + ".log.resources");
                objStream.WriteLine(strExceptions);
                objStream.Close();
            }
            catch
            {
            }
            // does not have permission to create the log file

            return strExceptions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ParseFiles parses the Host Template's Files node
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="node">The Files node</param>
        ///	<param name="portalId">The PortalId (-1 for Host Files)</param>
        /// <history>
        /// [cnurse]	11/08/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void ParseFiles(XmlNode node, int portalId)
        {

            FileSystem.FileController objController = new FileSystem.FileController();

            //Parse the File nodes
            foreach (XmlNode fileNode in node.SelectNodes("file"))
            {
                string strFileName = XmlUtils.GetNodeValue(fileNode.CreateNavigator(), "filename");
                string strExtenstion = XmlUtils.GetNodeValue(fileNode.CreateNavigator(), "extension");
                long fileSize = long.Parse(XmlUtils.GetNodeValue(fileNode.CreateNavigator(), "size"));
                int iWidth = XmlUtils.GetNodeValueInt(fileNode, "width");
                int iHeight = XmlUtils.GetNodeValueInt(fileNode, "height");
                string strType = XmlUtils.GetNodeValue(fileNode.CreateNavigator(), "contentType");
                string strFolder = XmlUtils.GetNodeValue(fileNode.CreateNavigator(), "folder");

                FolderController objFolders = new FolderController();
                FolderInfo objFolder = objFolders.GetFolder(portalId, strFolder, false);
                objController.AddFile(portalId, strFileName, strExtenstion, fileSize, iWidth, iHeight, strType, strFolder, objFolder.FolderID, true
                );

            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// RemoveCoreModule removes a Core Module from the system
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="DesktopModuleName">The Friendly Name of the Module to Remove</param>
        ///	<param name="ParentTabName">The Name of the parent Tab/Page for this module</param>
        ///	<param name="TabName">The Name to tab that contains the Module</param>
        ///	<param name="TabRemove">A flag to determine whether to remove the Tab if it has no
        ///	other modules</param>
        /// <history>
        /// [cnurse]	10/14/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void RemoveCoreModule(string DesktopModuleName, string ParentTabName, string TabName, bool TabRemove)
        {
            int ParentId = 0;
            int intModuleDefId = Null.NullInteger;
            int intDesktopModuleId = 0;

            //Find and remove the Module from the Tab
            switch (ParentTabName)
            {
                case "Host":
                    TabController objTabs = new TabController();
                    TabInfo objTab = objTabs.GetTabByName("Host", Null.NullInteger, Null.NullInteger);

                    if (objTab != null)
                    {
                        intModuleDefId = RemoveModule(DesktopModuleName, TabName, objTab.TabID, TabRemove);
                    }
                    break;
                case "Admin":
                    PortalController objPortals = new PortalController();
                    PortalInfo objPortal = default(PortalInfo);

                    ArrayList arrPortals = objPortals.GetPortals();
                    int intPortal = 0;

                    //Iterate through the Portals to remove the Module from the Tab
                    for (intPortal = 0; intPortal <= arrPortals.Count - 1; intPortal++)
                    {
                        objPortal = (PortalInfo)arrPortals[intPortal];
                        ParentId = objPortal.AdminTabId;
                        intModuleDefId = RemoveModule(DesktopModuleName, TabName, ParentId, TabRemove);
                    }
                    break;
            }

            DesktopModuleInfo objDesktopModule = null;
            if (intModuleDefId == Null.NullInteger)
            {
                objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModuleName, Null.NullInteger);
                intDesktopModuleId = objDesktopModule.DesktopModuleID;
            }
            else
            {
                //Get the Module Definition
                ModuleDefinitionController objModuleDefinitions = new ModuleDefinitionController();
                ModuleDefinitionInfo objModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByID(intModuleDefId);
                if (objModuleDefinition != null)
                {
                    intDesktopModuleId = objModuleDefinition.DesktopModuleID;
                    objDesktopModule = DesktopModuleController.GetDesktopModule(intDesktopModuleId, Null.NullInteger);
                }
            }

            if (objDesktopModule != null)
            {
                //Delete the Desktop Module
                DesktopModuleController objDesktopModules = new DesktopModuleController();
                objDesktopModules.DeleteDesktopModule(intDesktopModuleId);

                //Delete the Package
                PackageController.DeletePackage(objDesktopModule.PackageID);

            }
        }

        private static int RemoveModule(string DesktopModuleName, string TabName, int ParentId, bool TabRemove)
        {
            TabController objTabs = new TabController();
            ModuleController objModules = new ModuleController();
            TabInfo objTab = objTabs.GetTabByName(TabName, Null.NullInteger, ParentId);
            int intModuleDefId = 0;
            int intCount = 0;

            //Get the Modules on the Tab
            if (objTab != null)
            {
                foreach (KeyValuePair<int, ModuleInfo> kvp in objModules.GetTabModules(objTab.TabID))
                {
                    ModuleInfo objModule = kvp.Value;
                    if (objModule.DesktopModule.FriendlyName == DesktopModuleName)
                    {
                        //Delete the Module from the Modules list
                        objModules.DeleteTabModule(objModule.TabID, objModule.ModuleID, false);
                        intModuleDefId = objModule.ModuleDefID;
                    }
                    else
                    {
                        intCount += 1;
                    }
                }

                //If Tab has no modules optionally remove tab
                if (intCount == 0 & TabRemove)
                {
                    objTabs.DeleteTab(objTab.TabID, objTab.PortalID);
                }
            }

            return intModuleDefId;
        }

        private static void RemoveModuleControl(int ModuleDefId, string ControlKey)
        {
            // get Module Control
            ModuleControlInfo objModuleControl = ModuleControlController.GetModuleControlByControlKey(ControlKey, ModuleDefId);
            if (objModuleControl != null)
            {
                ModuleControlController.DeleteModuleControl(objModuleControl.ModuleControlID);
            }
        }

        private static void RemoveModuleFromPortals(string friendlyName)
        {
            DesktopModuleInfo objDesktopModule = DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName);
            if (objDesktopModule != null)
            {
                //Module was incorrectly assigned as "IsPremium=False"
                if (objDesktopModule.PackageID > Null.NullInteger)
                {
                    objDesktopModule.IsPremium = true;
                    DesktopModuleController.SaveDesktopModule(objDesktopModule, false, true);
                }

                //Remove the module from Portals
                DesktopModuleController.RemoveDesktopModuleFromPortals(objDesktopModule.DesktopModuleID);
            }
        }

        private static bool TabPermissionExists(TabPermissionInfo tabPermission, int PortalID)
        {
            bool blnExists = false;

            foreach (TabPermissionInfo permission in TabPermissionController.GetTabPermissions(tabPermission.TabID, PortalID))
            {
                if (permission.TabID == tabPermission.TabID && permission.RoleID == tabPermission.RoleID && permission.PermissionID == tabPermission.PermissionID)
                {
                    blnExists = true;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            return blnExists;
        }

        private static void UpgradeToVersion_323()
        {
            //add new SecurityException
            Log.EventLog.LogController objLogController = new Log.EventLog.LogController();
            string xmlConfigFile = Common.Globals.HostMapPath + "Logs\\LogConfig\\SecurityExceptionTemplate.xml.resources";
            objLogController.AddLogType(xmlConfigFile, Null.NullString);
        }

        private static void UpgradeToVersion_440()
        {
            // remove module cache files with *.htm extension ( they are now securely named *.resources )
            PortalController objPortals = new PortalController();
            ArrayList arrPortals = objPortals.GetPortals();
            foreach (PortalInfo objPortal in arrPortals)
            {
                if (Directory.Exists(Globals.ApplicationMapPath + "\\Portals\\" + objPortal.PortalID.ToString() + "\\Cache\\"))
                {
                    string[] arrFiles = Directory.GetFiles(Globals.ApplicationMapPath + "\\Portals\\" + objPortal.PortalID.ToString() + "\\Cache\\", "*.htm");
                    foreach (string strFile in arrFiles)
                    {
                        File.Delete(strFile);
                    }
                }
            }
        }

        private static void UpgradeToVersion_470()
        {
            string strHostTemplateFile = Globals.HostMapPath + "Templates\\Default.page.template";
            if (File.Exists(strHostTemplateFile))
            {
                PortalController objPortals = new PortalController();
                ArrayList arrPortals = objPortals.GetPortals();
                foreach (PortalInfo objPortal in arrPortals)
                {
                    string strPortalTemplateFolder = objPortal.HomeDirectoryMapPath + "Templates\\";

                    if (!Directory.Exists(strPortalTemplateFolder))
                    {
                        //Create Portal Templates folder
                        Directory.CreateDirectory(strPortalTemplateFolder);
                    }
                    string strPortalTemplateFile = strPortalTemplateFolder + "Default.page.template";
                    if (!File.Exists(strPortalTemplateFile))
                    {
                        File.Copy(strHostTemplateFile, strPortalTemplateFile);

                        //Synchronize the Templates folder to ensure the templates are accessible
                        FileSystemUtils.SynchronizeFolder(objPortal.PortalID, strPortalTemplateFolder, "Templates/", false, true, true, false);
                    }
                }
            }
        }

        private static void UpgradeToVersion_482()
        {
            //checks for the very rare case where the default validationkey prior to 4.08.02
            //is still being used and updates it
            Config.UpdateValidationKey();
        }

        private static void UpgradeToVersion_500()
        {
            PortalController objPortals = new PortalController();
            ArrayList arrPortals = objPortals.GetPortals();
            TabController controller = new TabController();

            //Add Edit Permissions for Admin Tabs to legacy portals
            PermissionController permissionControler = new PermissionController();
            TabPermissionController tabPermissionControler = new TabPermissionController();
            ArrayList permissions = permissionControler.GetPermissionByCodeAndKey("SYSTEM_TAB", "EDIT");
            int permissionID = Null.NullInteger;
            if (permissions.Count == 1)
            {
                PermissionInfo permission = permissions[0] as PermissionInfo;
                permissionID = permission.PermissionID;

                foreach (PortalInfo portal in arrPortals)
                {
                    TabInfo adminTab = controller.GetTab(portal.AdminTabId, portal.PortalID, true);
                    if (adminTab != null)
                    {
                        TabPermissionInfo tabPermission = new TabPermissionInfo();
                        tabPermission.TabID = adminTab.TabID;
                        tabPermission.PermissionID = permissionID;
                        tabPermission.AllowAccess = true;
                        tabPermission.RoleID = portal.AdministratorRoleId;
                        if (!TabPermissionExists(tabPermission, portal.PortalID))
                        {
                            adminTab.TabPermissions.Add(tabPermission);
                        }

                        //Save Tab Permissions to Data Base
                        TabPermissionController.SaveTabPermissions(adminTab);

                        foreach (TabInfo childTab in TabController.GetTabsByParent(portal.AdminTabId, portal.PortalID))
                        {
                            tabPermission = new TabPermissionInfo();
                            tabPermission.TabID = childTab.TabID;
                            tabPermission.PermissionID = permissionID;
                            tabPermission.AllowAccess = true;
                            tabPermission.RoleID = portal.AdministratorRoleId;
                            if (!TabPermissionExists(tabPermission, portal.PortalID))
                            {
                                childTab.TabPermissions.Add(tabPermission);
                            }
                            //Save Tab Permissions to Data Base
                            TabPermissionController.SaveTabPermissions(childTab);
                        }
                    }
                }
            }

            //Update Host/Admin modules Visibility setting
            bool superTabProcessed = Null.NullBoolean;
            ModuleController moduleController = new ModuleController();
            foreach (PortalInfo portal in arrPortals)
            {
                if (!superTabProcessed)
                {
                    //Process Host Tabs
                    foreach (TabInfo childTab in TabController.GetTabsByParent(portal.SuperTabId, Null.NullInteger))
                    {
                        foreach (ModuleInfo tabModule in moduleController.GetTabModules(childTab.TabID).Values)
                        {
                            tabModule.Visibility = VisibilityState.None;
                            moduleController.UpdateModule(tabModule);
                        }
                    }
                }

                //Process Portal Tabs
                foreach (TabInfo childTab in TabController.GetTabsByParent(portal.AdminTabId, portal.PortalID))
                {
                    foreach (ModuleInfo tabModule in moduleController.GetTabModules(childTab.TabID).Values)
                    {
                        tabModule.Visibility = VisibilityState.None;
                        moduleController.UpdateModule(tabModule);
                    }
                }
            }

            //Upgrade PortalDesktopModules to support new "model"
            permissions = permissionControler.GetPermissionByCodeAndKey("SYSTEM_DESKTOPMODULE", "DEPLOY");
            if (permissions.Count == 1)
            {
                PermissionInfo permission = permissions[0] as PermissionInfo;
                permissionID = permission.PermissionID;
                foreach (PortalInfo portal in arrPortals)
                {
                    foreach (DesktopModuleInfo desktopModule in DesktopModuleController.GetDesktopModules(Null.NullInteger).Values)
                    {
                        if (!desktopModule.IsPremium)
                        {
                            //Parse the permissions
                            DesktopModulePermissionCollection deployPermissions = new DesktopModulePermissionCollection();
                            DesktopModulePermissionInfo deployPermission = default(DesktopModulePermissionInfo);

                            // if Not IsAdmin add Registered Users
                            if (!desktopModule.IsAdmin)
                            {
                                deployPermission = new DesktopModulePermissionInfo();
                                deployPermission.PermissionID = permissionID;
                                deployPermission.AllowAccess = true;
                                deployPermission.RoleID = portal.RegisteredRoleId;
                                deployPermissions.Add(deployPermission);
                            }

                            // if Not a Host Module add Administrators
                            string hostModules = "Portals, SQL, HostSettings, Scheduler, SearchAdmin, Lists, SkinDesigner, Extensions";
                            if (!hostModules.Contains(desktopModule.ModuleName))
                            {
                                deployPermission = new DesktopModulePermissionInfo();
                                deployPermission.PermissionID = permissionID;
                                deployPermission.AllowAccess = true;
                                deployPermission.RoleID = portal.AdministratorRoleId;
                                deployPermissions.Add(deployPermission);
                            }

                            //Add Portal/Module to PortalDesktopModules
                            DesktopModuleController.AddDesktopModuleToPortal(portal.PortalID, desktopModule, deployPermissions, false);
                        }
                    }

                    DataCache.ClearPortalCache(portal.PortalID, true);
                }
            }

            LegacyUtil.ProcessLegacyModules();
            LegacyUtil.ProcessLegacyLanguages();
            LegacyUtil.ProcessLegacySkins();
            LegacyUtil.ProcessLegacySkinControls();
        }

        private static void UpgradeToVersion_501()
        {
            //add new Cache Error Event Type
            Log.EventLog.LogController objLogController = new Log.EventLog.LogController();
            string xmlConfigFile = string.Format("{0}Logs\\LogConfig\\CacheErrorTemplate.xml.resources", Common.Globals.HostMapPath);
            objLogController.AddLogType(xmlConfigFile, Null.NullString);
        }

        private static void UpgradeToVersion_510()
        {
            //Upgrade to .NET 3.5
            TryUpgradeNETFramework();

            PortalController objPortalController = new PortalController();
            TabController objTabController = new TabController();
            ModuleController objModuleController = new ModuleController();
            int ModuleDefID = 0;
            int ModuleID = 0;

            //add Dashboard module and tab
            if (HostTabExists("Dashboard") == false)
            {
                ModuleDefID = AddModuleDefinition("Dashboard", "Provides a snapshot of your DotNetNuke Application.", "Dashboard", true, true);
                AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Dashboard/Dashboard.ascx", "icon_dashboard_32px.gif", SecurityAccessLevel.Host, 0);
                AddModuleControl(ModuleDefID, "Export", "", "DesktopModules/Admin/Dashboard/Export.ascx", "", SecurityAccessLevel.Host, 0);
                AddModuleControl(ModuleDefID, "DashboardControls", "", "DesktopModules/Admin/Dashboard/DashboardControls.ascx", "", SecurityAccessLevel.Host, 0);

                //Create New Host Page (or get existing one)
                TabInfo dashboardPage = AddHostPage("Dashboard", "Summary view of application and site settings.", "~/images/icon_dashboard_16px.gif", "~/images/icon_dashboard_32px.gif", true);

                //Add Module To Page
                AddModuleToPage(dashboardPage, ModuleDefID, "Dashboard", "~/images/icon_dashboard_32px.gif");
            }
            else
            {
                //Module was incorrectly assigned as "IsPremium=False"
                RemoveModuleFromPortals("Dashboard");
                //fix path for dashboarcontrols
                ModuleDefID = GetModuleDefinition("Dashboard", "Dashboard");
                RemoveModuleControl(ModuleDefID, "DashboardControls");
                AddModuleControl(ModuleDefID, "DashboardControls", "", "DesktopModules/Admin/Dashboard/DashboardControls.ascx", "", SecurityAccessLevel.Host, 0);
            }

            //Add the Extensions Module
            if (CoreModuleExists("Extensions") == false)
            {
                ModuleDefID = AddModuleDefinition("Extensions", "", "Extensions");
                AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Extensions/Extensions.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.View, 0);
                AddModuleControl(ModuleDefID, "Edit", "Edit Feature", "DesktopModules/Admin/Extensions/EditExtension.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Edit, 0);
                AddModuleControl(ModuleDefID, "PackageWriter", "Package Writer", "DesktopModules/Admin/Extensions/PackageWriter.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0);
                AddModuleControl(ModuleDefID, "EditControl", "Edit Control", "DesktopModules/Admin/Extensions/Editors/EditModuleControl.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0);
                AddModuleControl(ModuleDefID, "ImportModuleDefinition", "Import Module Definition", "DesktopModules/Admin/Extensions/Editors/ImportModuleDefinition.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0);
                AddModuleControl(ModuleDefID, "BatchInstall", "Batch Install", "DesktopModules/Admin/Extensions/BatchInstall.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0);
                AddModuleControl(ModuleDefID, "NewExtension", "New Extension Wizard", "DesktopModules/Admin/Extensions/ExtensionWizard.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0);
                AddModuleControl(ModuleDefID, "UsageDetails", "Usage Information", "DesktopModules/Admin/Extensions/UsageDetails.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0, "", true);
            }
            else
            {
                ModuleDefID = GetModuleDefinition("Extensions", "Extensions");
                RemoveModuleControl(ModuleDefID, "EditLanguage");
                RemoveModuleControl(ModuleDefID, "TimeZone");
                RemoveModuleControl(ModuleDefID, "Verify");
                RemoveModuleControl(ModuleDefID, "LanguageSettings");
                RemoveModuleControl(ModuleDefID, "EditResourceKey");
                RemoveModuleControl(ModuleDefID, "EditSkins");
                AddModuleControl(ModuleDefID, "UsageDetails", "Usage Information", "DesktopModules/Admin/Extensions/UsageDetails.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0, "", true);

                //Module was incorrectly assigned as "IsPremium=False"
                RemoveModuleFromPortals("Extensions");
            }

            //Remove Module Definitions Module from Host Page (if present)
            RemoveCoreModule("Module Definitions", "Host", "Module Definitions", false);

            //Remove old Module Definition Validator module
            DesktopModuleController.DeleteDesktopModule("Module Definition Validator");

            //Get Module Definitions
            TabInfo definitionsPage = objTabController.GetTabByName("Module Definitions", Null.NullInteger);

            //Add Module To Page if not present
            ModuleID = AddModuleToPage(definitionsPage, ModuleDefID, "Module Definitions", "~/images/icon_moduledefinitions_32px.gif");
            objModuleController.UpdateModuleSetting(ModuleID, "Extensions_Mode", "Module");

            //Add Extensions Host Page
            TabInfo extensionsPage = AddHostPage("Extensions", "Install, add, modify and delete extensions, such as modules, skins and language packs.", "~/images/icon_extensions_16px.gif", "~/images/icon_extensions_32px.gif", true);

            ModuleID = AddModuleToPage(extensionsPage, ModuleDefID, "Extensions", "~/images/icon_extensions_32px.gif");
            objModuleController.UpdateModuleSetting(ModuleID, "Extensions_Mode", "All");

            //Add Extensions Module to Admin Page for all Portals
            AddAdminPages("Extensions", "Install, add, modify and delete extensions, such as modules, skins and language packs.", "~/images/icon_extensions_16px.gif", "~/images/icon_extensions_32px.gif", true, ModuleDefID, "Extensions", "~/images/icon_extensions_32px.gif");

            //Remove Host Languages Page
            RemoveHostPage("Languages");

            //Remove Admin > Authentication Pages
            RemoveAdminPages("//Admin//Authentication");

            //Remove old Languages module
            DesktopModuleController.DeleteDesktopModule("Languages");

            //Add new Languages module
            ModuleDefID = AddModuleDefinition("Languages", "", "Languages", false, false);
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Languages/languageeditor.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.View, 0);
            AddModuleControl(ModuleDefID, "Edit", "Edit Language", "DesktopModules/Admin/Languages/EditLanguage.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.Edit, 0);
            AddModuleControl(ModuleDefID, "EditResourceKey", "Full Language Editor", "DesktopModules/Admin/Languages/languageeditorext.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.Edit, 0);
            AddModuleControl(ModuleDefID, "LanguageSettings", "Language Settings", "DesktopModules/Admin/Languages/LanguageSettings.ascx", "", SecurityAccessLevel.Edit, 0);
            AddModuleControl(ModuleDefID, "TimeZone", "TimeZone Editor", "DesktopModules/Admin/Languages/timezoneeditor.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.Host, 0);
            AddModuleControl(ModuleDefID, "Verify", "Resource File Verifier", "DesktopModules/Admin/Languages/resourceverifier.ascx", "", SecurityAccessLevel.Host, 0);
            AddModuleControl(ModuleDefID, "PackageWriter", "Language Pack Writer", "DesktopModules/Admin/Languages/LanguagePackWriter.ascx", "", SecurityAccessLevel.Host, 0);

            //Add Module to Admin Page for all Portals
            AddAdminPages("Languages", "Manage Language Resources.", "~/images/icon_language_16px.gif", "~/images/icon_language_32px.gif", true, ModuleDefID, "Language Editor", "~/images/icon_language_32px.gif");

            //Remove Host Skins Page
            RemoveHostPage("Skins");

            //Remove old Skins module
            DesktopModuleController.DeleteDesktopModule("Skins");

            //Add new Skins module
            ModuleDefID = AddModuleDefinition("Skins", "", "Skins", false, false);
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Skins/editskins.ascx", "~/images/icon_skins_32px.gif", SecurityAccessLevel.View, 0);

            //Add Module to Admin Page for all Portals
            AddAdminPages("Skins", "Manage Skin Resources.", "~/images/icon_skins_16px.gif", "~/images/icon_skins_32px.gif", true, ModuleDefID, "Skin Editor", "~/images/icon_skins_32px.gif");

            //Remove old Skin Designer module
            DesktopModuleController.DeleteDesktopModule("Skin Designer");
            DesktopModuleController.DeleteDesktopModule("SkinDesigner");

            //Add new Skin Designer module
            ModuleDefID = AddModuleDefinition("Skin Designer", "Allows you to modify skin attributes.", "Skin Designer", true, true);
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SkinDesigner/Attributes.ascx", "~/images/icon_skins_32px.gif", SecurityAccessLevel.Host, 0);

            //Add new Skin Designer to every Admin Skins Tab
            AddModuleToPages("//Admin//Skins", ModuleDefID, "Skin Designer", "~/images/icon_skins_32px.gif", true);

            //Remove Admin Whats New Page
            RemoveAdminPages("//Admin//WhatsNew");

            //WhatsNew needs to be set to IsPremium and removed from all portals
            RemoveModuleFromPortals("WhatsNew");

            //Create New WhatsNew Host Page (or get existing one)
            TabInfo newPage = AddHostPage("What's New", "Provides a summary of the major features for each release.", "~/images/icon_whatsnew_16px.gif", "~/images/icon_whatsnew_32px.gif", true);

            //Add WhatsNew Module To Page
            ModuleDefID = GetModuleDefinition("WhatsNew", "WhatsNew");
            AddModuleToPage(newPage, ModuleDefID, "What's New", "~/images/icon_whatsnew_32px.gif");

            //add console module
            ModuleDefID = AddModuleDefinition("Console", "Display children pages as icon links for navigation.", "Console", "DotNetNuke.Modules.Console.Components.ConsoleController", true, false, false);
            AddModuleControl(ModuleDefID, "", "Console", "DesktopModules/Admin/Console/ViewConsole.ascx", "", SecurityAccessLevel.Anonymous, 0);
            AddModuleControl(ModuleDefID, "Settings", "Console Settings", "DesktopModules/Admin/Console/Settings.ascx", "", SecurityAccessLevel.Admin, 0);

            //add console module to host page
            ModuleID = AddModuleToPage("//Host", Null.NullInteger, ModuleDefID, "Basic Features", "", true);
            int tabID = TabController.GetTabByTabPath(Null.NullInteger, "//Host");
            TabInfo tab = null;

            //add console settings for host page
            if ((tabID != Null.NullInteger))
            {
                tab = objTabController.GetTab(tabID, Null.NullInteger, true);
                if (((tab != null)))
                {
                    AddConsoleModuleSettings(tabID, ModuleID);
                }
            }

            //add module to all admin pages
            foreach (PortalInfo portal in objPortalController.GetPortals())
            {
                tabID = TabController.GetTabByTabPath(portal.PortalID, "//Admin");
                if ((tabID != Null.NullInteger))
                {
                    tab = objTabController.GetTab(tabID, portal.PortalID, true);
                    if (((tab != null)))
                    {
                        ModuleID = AddModuleToPage(tab, ModuleDefID, "Basic Features", "", true);
                        AddConsoleModuleSettings(tabID, ModuleID);
                    }
                }
            }

            //Add Google Analytics module
            ModuleDefID = AddModuleDefinition("Google Analytics", "Configure portal Google Analytics settings.", "GoogleAnalytics", false, false);
            AddModuleControl(ModuleDefID, "", "Google Analytics", "DesktopModules/Admin/Analytics/GoogleAnalyticsSettings.ascx", "", SecurityAccessLevel.Admin, 0);
            AddAdminPages("Google Analytics", "Configure portal Google Analytics settings.", "~/images/icon_analytics_16px.gif", "~/images/icon_analytics_32px.gif", true, ModuleDefID, "Google Analytics", "~/images/icon_analytics_32px.gif");
        }

        private static void UpgradeToVersion_511()
        {
            //New Admin pages may not have administrator permission
            //Add Admin role if it does not exist for google analytics or extensions
            AddAdminRoleToPage("//Admin//Extensions");
            AddAdminRoleToPage("//Admin//GoogleAnalytics");
        }

        private static void UpgradeToVersion_513()
        {
            //Ensure that default language is present (not neccessarily enabled)
            Locale defaultLanguage = Localization.Localization.GetLocale("en-US");
            if (defaultLanguage == null)
            {
                defaultLanguage = new Locale();
            }
            defaultLanguage.Code = "en-US";
            defaultLanguage.Text = "English (United States)";
            Localization.Localization.SaveLanguage(defaultLanguage);

            //Ensure that there is a Default Authorization System
            PackageInfo package = PackageController.GetPackageByName("DefaultAuthentication");
            if (package == null)
            {
                package = new PackageInfo();
                package.Name = "DefaultAuthentication";
                package.FriendlyName = "Default Authentication";
                package.Description = "The Default UserName/Password Authentication System for DotNetNuke.";
                package.PackageType = "Auth_System";
                package.Version = new Version(1, 0, 0);
                package.Owner = "DotNetNuke";
                package.License = Localization.Localization.GetString("License", Localization.Localization.GlobalResourceFile);
                package.Organization = "DotNetNuke Corporation";
                package.Url = "www.dotnetnuke.com";
                package.Email = "support@dotnetnuke.com";
                package.ReleaseNotes = "There are no release notes for this version.";
                package.IsSystemPackage = true;
                PackageController.SavePackage(package);

                //Add Authentication System
                Authentication.AuthenticationInfo authSystem = Authentication.AuthenticationController.GetAuthenticationServiceByType("DNN");
                if (authSystem == null)
                {
                    authSystem = new Authentication.AuthenticationInfo();
                }
                authSystem.PackageID = package.PackageID;
                authSystem.AuthenticationType = "DNN";
                authSystem.SettingsControlSrc = "DesktopModules/AuthenticationServices/DNN/Settings.ascx";
                authSystem.LoginControlSrc = "DesktopModules/AuthenticationServices/DNN/Login.ascx";
                authSystem.IsEnabled = true;

                if (authSystem.AuthenticationID == Null.NullInteger)
                {
                    Authentication.AuthenticationController.AddAuthentication(authSystem);
                }
                else
                {
                    Authentication.AuthenticationController.UpdateAuthentication(authSystem);
                }
            }
        }

        private static void UpgradeToVersion_520()
        {
            int ModuleDefID = 0;

            //Add new ViewSource control
            AddModuleControl(Null.NullInteger, "ViewSource", "View Module Source", "Admin/Modules/ViewSource.ascx", "~/images/icon_source_32px.gif", SecurityAccessLevel.Host, 0, "", true);

            //Add Marketplace module definition
            ModuleDefID = AddModuleDefinition("Marketplace", "Search for DotNetNuke modules, extension and skins.", "Marketplace");
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Marketplace/Marketplace.ascx", "~/images/icon_marketplace_32px.gif", SecurityAccessLevel.Host, 0);

            //Add marketplace Module To Page
            TabInfo newPage = AddHostPage("Marketplace", "Search for DotNetNuke modules, extension and skins.", "~/images/icon_marketplace_16px.gif", "~/images/icon_marketplace_32px.gif", true);
            ModuleDefID = GetModuleDefinition("Marketplace", "Marketplace");
            AddModuleToPage(newPage, ModuleDefID, "Marketplace", "~/images/icon_marketplace_32px.gif");
        }

        private static void UpgradeToVersion_521()
        {
            // UpgradeDefaultLanguages is a temporary procedure containing code that
            // needed to execute after the 5.1.3 application upgrade code above
            DataProvider.Instance().ExecuteNonQuery("UpgradeDefaultLanguages");

            // This procedure is not intended to be part of the database schema
            // and is therefore dropped once it has been executed.
            DataProvider.Instance().ExecuteSQL("DROP PROCEDURE {databaseOwner}{objectQualifier}UpgradeDefaultLanguages");
        }

        private static void UpgradeToVersion_530()
        {
            int ModuleDefID = 0;

            //update languages module
            ModuleDefID = GetModuleDefinition("Languages", "Languages");
            RemoveModuleControl(ModuleDefID, "");
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Languages/languageEnabler.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.View, 0, "", true);
            AddModuleControl(ModuleDefID, "Editor", "", "DesktopModules/Admin/Languages/languageeditor.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.View, 0);

            //Add new View Profile module
            ModuleDefID = AddModuleDefinition("ViewProfile", "", "ViewProfile", false, false);
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/ViewProfile/ViewProfile.ascx", "~/images/icon_profile_32px.gif", SecurityAccessLevel.View, 0);
            AddModuleControl(ModuleDefID, "Settings", "Settings", "DesktopModules/Admin/ViewProfile/Settings.ascx", "~/images/icon_profile_32px.gif", SecurityAccessLevel.Edit, 0);

            //Add new Sitemap settings module
            ModuleDefID = AddModuleDefinition("Sitemap", "", "Sitemap", false, false);
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Sitemap/SitemapSettings.ascx", "~/images/icon_analytics_32px.gif", SecurityAccessLevel.View, 0);
            AddAdminPages("Search Engine Sitemap", "Configure the sitemap for submission to common search engines.", "~/images/icon_analytics_16px.gif", "~/images/icon_analytics_32px.gif", true, ModuleDefID, "Search Engine Sitemap", "~/images/icon_analytics_32px.gif");


            //Add new Photo Profile field to Host
            ListController objListController = new ListController();
            ListEntryInfoCollection dataTypes = objListController.GetListEntryInfoCollection("DataType");

            ProfilePropertyDefinitionCollection properties = ProfileController.GetPropertyDefinitionsByPortal(Null.NullInteger);
            ProfileController.AddDefaultDefinition(Null.NullInteger, "Preferences", "Photo", "Image", 0, properties.Count * 2 + 2, dataTypes);

            string strHostTemplateFile = string.Format("{0}Templates\\UserProfile.page.template", Globals.HostMapPath);
            if (File.Exists(strHostTemplateFile))
            {
                TabController tabController = new TabController();
                PortalController objPortals = new PortalController();
                ArrayList arrPortals = objPortals.GetPortals();
                foreach (PortalInfo objPortal in arrPortals)
                {
                    properties = ProfileController.GetPropertyDefinitionsByPortal(objPortal.PortalID);

                    //Add new Photo Profile field to Portal
                    ProfileController.AddDefaultDefinition(objPortal.PortalID, "Preferences", "Photo", "Image", 0, properties.Count * 2 + 2, dataTypes);

                    //Rename old Default Page template
                    File.Move(string.Format("{0}Templates\\Default.page.template", objPortal.HomeDirectoryMapPath), string.Format("{0}Templates\\Default_old.page.template", objPortal.HomeDirectoryMapPath));

                    //Update Default profile template in every portal
                    objPortals.CopyPageTemplate("Default.page.template", objPortal.HomeDirectoryMapPath);

                    //Add User profile template to every portal
                    objPortals.CopyPageTemplate("UserProfile.page.template", objPortal.HomeDirectoryMapPath);

                    //Synchronize the Templates folder to ensure the templates are accessible
                    FileSystemUtils.SynchronizeFolder(objPortal.PortalID, string.Format("{0}Templates\\", objPortal.HomeDirectoryMapPath), "Templates/", false, true, true, false);

                    XmlDocument xmlDoc = new XmlDocument();
                    try
                    {
                        // open the XML file
                        xmlDoc.Load(string.Format("{0}Templates\\UserProfile.page.template", objPortal.HomeDirectoryMapPath));
                    }
                    catch (Exception ex)
                    {
                        Exceptions.Exceptions.LogException(ex);
                    }

                    TabInfo newTab = new TabInfo();
                    newTab = TabController.DeserializeTab(xmlDoc.SelectSingleNode("//portal/tabs/tab"), null, objPortal.PortalID, PortalTemplateModuleAction.Merge);
                    TabController.DeserializePanes(xmlDoc.SelectSingleNode("//portal/tabs/tab/panes"), newTab.PortalID, newTab.TabID, PortalTemplateModuleAction.Ignore, new Hashtable());

                    //Update SiteSettings to point to the new page
                    objPortal.RegisterTabId = objPortal.UserTabId;
                    objPortal.UserTabId = newTab.TabID;
                    objPortals.UpdatePortalInfo(objPortal);

                    //Add Users folder to every portal
                    string strUsersFolder = string.Format("{0}Users\\", objPortal.HomeDirectoryMapPath);

                    if (!Directory.Exists(strUsersFolder))
                    {
                        //Create Users folder
                        Directory.CreateDirectory(strUsersFolder);

                        //Synchronize the Users folder to ensure the user folder is accessible
                        FileSystemUtils.SynchronizeFolder(objPortal.PortalID, strUsersFolder, "Users/", false, true, true, false);
                    }
                }
            }

            //Add new EventQueue Event
            CommonLibrary.Services.EventQueue.Config.EventQueueConfiguration config = CommonLibrary.Services.EventQueue.Config.EventQueueConfiguration.GetConfig();
            if (config != null)
            {
                if (!config.PublishedEvents.ContainsKey("Application_Start_FirstRequest"))
                {
                    foreach (CommonLibrary.Services.EventQueue.Config.SubscriberInfo subscriber in config.EventQueueSubscribers.Values)
                    {
                        CommonLibrary.Services.EventQueue.Config.EventQueueConfiguration.RegisterEventSubscription(config, "Application_Start_FirstRequest", subscriber);
                    }

                    EventQueueConfiguration.SaveConfig(config, string.Format("{0}EventQueue\\EventQueue.config", Globals.HostMapPath));
                }
            }
            //Change Key for Module Defintions
            ModuleDefID = GetModuleDefinition("Extensions", "Extensions");
            RemoveModuleControl(ModuleDefID, "ImportModuleDefinition");
            AddModuleControl(ModuleDefID, "EditModuleDefinition", "Edit Module Definition", "DesktopModules/Admin/Extensions/Editors/EditModuleDefinition.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0);

            //Module was incorrectly assigned as "IsPremium=False"
            RemoveModuleFromPortals("Users And Roles");
        }

        private static void UpgradeToVersion_540()
        {
            XmlDocument configDoc = Config.Load();
            XPathNavigator configNavigator = configDoc.CreateNavigator().SelectSingleNode("/configuration/system.web.extensions");
            if (configNavigator == null)
            {
                //attempt to remove "System.Web.Extensions" configuration section
                string upgradeFile = string.Format("{0}\\Config\\SystemWebExtensions.config", Globals.InstallMapPath);
                string strMessage = UpdateConfig(upgradeFile, AppContext.Current.Application.Version, "Remove System Web Extensions");
                if (string.IsNullOrEmpty(strMessage))
                {
                    //Log Upgrade
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog("UpgradeConfig", "Remove System Web Extensions", PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT);
                }
                else
                {
                    //Log Failed Upgrade
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog("UpgradeConfig", string.Format("Remove System Web Extensions failed. Error reported during attempt to update:{0}", strMessage), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT);
                }
            }

            //Add Styles Skin Object
            AddSkinControl("TAGS", "DotNetNuke.TagsSkinObject", "Admin/Skins/Tags.ascx");

            //Add Content List module definition
            int moduleDefID = AddModuleDefinition("ContentList", "This module displays a list of content by tag.", "Content List");
            AddModuleControl(moduleDefID, "", "", "DesktopModules/Admin/ContentList/ContentList.ascx", "", SecurityAccessLevel.View, 0);

            //Update registration page
            PortalController objPortals = new PortalController();
            ArrayList arrPortals = objPortals.GetPortals();
            foreach (PortalInfo objPortal in arrPortals)
            {
                objPortal.RegisterTabId = objPortal.UserTabId;
                objPortals.UpdatePortalInfo(objPortal);

                //Add ContentList to Search Results Page
                TabController tabController = new TabController();
                int tabId = TabController.GetTabByTabPath(objPortal.PortalID, "//SearchResults");
                TabInfo searchPage = tabController.GetTab(tabId, objPortal.PortalID, false);
                AddModuleToPage(searchPage, moduleDefID, "Results", "");

            }
        }

        private static void UpgradeToVersion_543()
        {
            // get log file path
            string LogFilePath = DataProvider.Instance().GetProviderPath();
            if (Directory.Exists(LogFilePath))
            {
                //get log files
                foreach (string fileName in Directory.GetFiles(LogFilePath, "*.log"))
                {
                    System.IO.FileInfo f = new System.IO.FileInfo(fileName);
                    //copy requires use of move
                    File.Move(fileName, fileName + ".resources");
                }
            }
        }


        #endregion

        #region "Public Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddAdminPages adds an Admin Page and an associated Module to all configured Portals
        /// </summary>
        ///	<param name="TabName">The Name to give this new Tab</param>
        ///	<param name="TabIconFile">The Icon for this new Tab</param>
        ///	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        ///	<param name="ModuleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        ///	<param name="ModuleTitle">The Module's title</param>
        ///	<param name="ModuleIconFile">The Module's icon</param>
        ///	<param name="InheritPermissions">Modules Inherit the Pages View Permisions</param>
        /// <history>
        /// [cnurse]	11/11/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void AddAdminPages(string TabName, string Description, string TabIconFile, string TabIconFileLarge, bool IsVisible, int ModuleDefId, string ModuleTitle, string ModuleIconFile, bool InheritPermissions)
        {

            PortalController objPortals = new PortalController();
            PortalInfo objPortal = default(PortalInfo);
            ArrayList arrPortals = objPortals.GetPortals();
            int intPortal = 0;
            TabInfo newPage = default(TabInfo);

            //Add Page to Admin Menu of all configured Portals
            for (intPortal = 0; intPortal <= arrPortals.Count - 1; intPortal++)
            {
                objPortal = (PortalInfo)arrPortals[intPortal];

                //Create New Admin Page (or get existing one)
                newPage = AddAdminPage(objPortal, TabName, Description, TabIconFile, TabIconFileLarge, IsVisible);

                //Add Module To Page
                AddModuleToPage(newPage, ModuleDefId, ModuleTitle, ModuleIconFile, InheritPermissions);

            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddAdminPage adds an Admin Tab Page
        /// </summary>
        ///	<param name="Portal">The Portal</param>
        ///	<param name="TabName">The Name to give this new Tab</param>
        ///	<param name="TabIconFile">The Icon for this new Tab</param>
        ///	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        /// <history>
        /// [cnurse]	11/11/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static TabInfo AddAdminPage(PortalInfo Portal, string TabName, string Description, string TabIconFile, string TabIconFileLarge, bool IsVisible)
        {

            TabController objTabController = new TabController();
            TabInfo AdminPage = objTabController.GetTab(Portal.AdminTabId, Portal.PortalID, false);

            if ((AdminPage != null))
            {
                Security.Permissions.TabPermissionCollection objTabPermissions = new Security.Permissions.TabPermissionCollection();
                AddPagePermission(objTabPermissions, "View", Convert.ToInt32(Portal.AdministratorRoleId));
                AddPagePermission(objTabPermissions, "Edit", Convert.ToInt32(Portal.AdministratorRoleId));
                return AddPage(AdminPage, TabName, Description, TabIconFile, TabIconFileLarge, IsVisible, objTabPermissions, true);
            }
            else
            {
                return null;

            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddHostPage adds a Host Tab Page
        /// </summary>
        ///	<param name="TabName">The Name to give this new Tab</param>
        ///	<param name="TabIconFile">The Icon for this new Tab</param>
        ///	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        /// <history>
        /// [cnurse]	11/11/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static TabInfo AddHostPage(string TabName, string Description, string TabIconFile, string TabIconFileLarge, bool IsVisible)
        {
            TabController objTabController = new TabController();
            TabInfo HostPage = objTabController.GetTabByName("Host", Null.NullInteger);

            if ((HostPage != null))
            {
                Security.Permissions.TabPermissionCollection objTabPermissions = new Security.Permissions.TabPermissionCollection();
                AddPagePermission(objTabPermissions, "View", Convert.ToInt32(Common.Globals.glbRoleSuperUser));
                AddPagePermission(objTabPermissions, "Edit", Convert.ToInt32(Common.Globals.glbRoleSuperUser));
                return AddPage(HostPage, TabName, Description, TabIconFile, TabIconFileLarge, IsVisible, objTabPermissions, true);
            }
            else
            {
                return null;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddModuleControl adds a new Module Control to the system
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="ModuleDefId">The Module Definition Id</param>
        ///	<param name="ControlKey">The key for this control in the Definition</param>
        ///	<param name="ControlTitle">The title of this control</param>
        ///	<param name="ControlSrc">Te source of ths control</param>
        ///	<param name="IconFile">The icon file</param>
        ///	<param name="ControlType">The type of control</param>
        ///	<param name="ViewOrder">The vieworder for this module</param>
        /// <history>
        /// [cnurse]	11/08/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void AddModuleControl(int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, SecurityAccessLevel ControlType, int ViewOrder)
        {

            //Call Overload with HelpUrl = Null.NullString
            AddModuleControl(ModuleDefId, ControlKey, ControlTitle, ControlSrc, IconFile, ControlType, ViewOrder, Null.NullString);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddModuleDefinition adds a new Core Module Definition to the system
        /// </summary>
        /// <remarks>
        ///	This overload asumes the module is an Admin module and not a Premium Module
        /// </remarks>
        ///	<param name="DesktopModuleName">The Friendly Name of the Module to Add</param>
        ///	<param name="Description">Description of the Module</param>
        ///	<param name="ModuleDefinitionName">The Module Definition Name</param>
        ///	<returns>The Module Definition Id of the new Module</returns>
        /// <history>
        /// [cnurse]	10/14/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int AddModuleDefinition(string DesktopModuleName, string Description, string ModuleDefinitionName)
        {
            //Call overload with Premium=False and Admin=True
            return AddModuleDefinition(DesktopModuleName, Description, ModuleDefinitionName, false, true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddModuleToPage adds a module to a Page
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="page">The Page to add the Module to</param>
        ///	<param name="ModuleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        ///	<param name="ModuleTitle">The Module's title</param>
        ///	<param name="ModuleIconFile">The Module's icon</param>
        ///	<param name="InheritPermissions">Inherit the Pages View Permisions</param>
        /// <history>
        /// [cnurse]	11/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int AddModuleToPage(TabInfo page, int ModuleDefId, string ModuleTitle, string ModuleIconFile, bool InheritPermissions)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = new ModuleInfo();
            bool blnDuplicate = false;
            int moduleId = Null.NullInteger;

            if ((page != null))
            {
                blnDuplicate = false;
                foreach (KeyValuePair<int, ModuleInfo> kvp in objModules.GetTabModules(page.TabID))
                {
                    objModule = kvp.Value;
                    if (objModule.ModuleDefID == ModuleDefId)
                    {
                        blnDuplicate = true;
                        moduleId = objModule.ModuleID;
                    }
                }

                if (!blnDuplicate)
                {
                    objModule = new ModuleInfo();
                    objModule.ModuleID = Null.NullInteger;
                    objModule.PortalID = page.PortalID;
                    objModule.TabID = page.TabID;
                    objModule.ModuleOrder = -1;
                    objModule.ModuleTitle = ModuleTitle;
                    objModule.PaneName = Globals.glbDefaultPane;
                    objModule.ModuleDefID = ModuleDefId;
                    objModule.CacheTime = 0;
                    objModule.IconFile = ModuleIconFile;
                    objModule.AllTabs = false;
                    objModule.Visibility = VisibilityState.None;
                    objModule.InheritViewPermissions = InheritPermissions;

                    try
                    {
                        moduleId = objModules.AddModule(objModule);
                    }
                    catch
                    {
                    }
                    // error adding module
                }
            }

            return moduleId;
        }

        public static int AddModuleToPage(string tabPath, int portalId, int ModuleDefId, string ModuleTitle, string ModuleIconFile, bool InheritPermissions)
        {
            TabController objTabController = new TabController();
            int moduleId = Null.NullInteger;

            int tabID = TabController.GetTabByTabPath(portalId, tabPath);
            if ((tabID != Null.NullInteger))
            {
                TabInfo tab = objTabController.GetTab(tabID, portalId, true);
                if ((tab != null))
                {
                    moduleId = AddModuleToPage(tab, ModuleDefId, ModuleTitle, ModuleIconFile, InheritPermissions);
                }
            }
            return moduleId;
        }

        public static void AddModuleToPages(string tabPath, int ModuleDefId, string ModuleTitle, string ModuleIconFile, bool InheritPermissions)
        {
            PortalController objPortalController = new PortalController();
            TabController objTabController = new TabController();

            ArrayList portals = objPortalController.GetPortals();
            foreach (PortalInfo portal in portals)
            {
                int tabID = TabController.GetTabByTabPath(portal.PortalID, tabPath);
                if ((tabID != Null.NullInteger))
                {
                    TabInfo tab = objTabController.GetTab(tabID, portal.PortalID, true);
                    if ((tab != null))
                    {
                        AddModuleToPage(tab, ModuleDefId, ModuleTitle, ModuleIconFile, InheritPermissions);
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddPortal manages the Installation of a new DotNetNuke Portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// [cnurse]	11/06/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int AddPortal(XmlNode node, bool status, int indent)
        {
            try
            {
                int intPortalId = 0;
                string strHostPath = Common.Globals.HostMapPath;
                string strChildPath = "";
                string strDomain = "";

                if ((HttpContext.Current != null))
                {
                    strDomain = Globals.GetDomainName(HttpContext.Current.Request, true).ToLowerInvariant().Replace("/install", "");
                }

                string strPortalName = XmlUtils.GetNodeValue(node.CreateNavigator(), "portalname");
                if (status)
                {
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Creating Portal: " + strPortalName + "<br>");
                }

                PortalController objPortalController = new PortalController();
                XmlNode adminNode = node.SelectSingleNode("administrator");
                string strFirstName = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "firstname");
                string strLastName = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "lastname");
                string strUserName = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "username");
                string strPassword = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "password");
                string strEmail = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "email");
                string strDescription = XmlUtils.GetNodeValue(node.CreateNavigator(), "description");
                string strKeyWords = XmlUtils.GetNodeValue(node.CreateNavigator(), "keywords");
                string strTemplate = XmlUtils.GetNodeValue(node.CreateNavigator(), "templatefile");
                string strServerPath = Globals.ApplicationMapPath + "\\";
                bool isChild = bool.Parse(XmlUtils.GetNodeValue(node.CreateNavigator(), "ischild"));
                string strHomeDirectory = XmlUtils.GetNodeValue(node.CreateNavigator(), "homedirectory");

                //Get the Portal Alias
                XmlNodeList portalAliases = node.SelectNodes("portalaliases/portalalias");
                string strPortalAlias = strDomain;
                if (portalAliases.Count > 0)
                {
                    if (!string.IsNullOrEmpty(portalAliases[0].InnerText))
                    {
                        strPortalAlias = portalAliases[0].InnerText;
                    }
                }

                //Create default email
                if (string.IsNullOrEmpty(strEmail))
                {
                    strEmail = "admin@" + strDomain.Replace("www.", "");
                    //Remove any domain subfolder information ( if it exists )
                    if (strEmail.IndexOf("/") != -1)
                    {
                        strEmail = strEmail.Substring(0, strEmail.IndexOf("/"));
                    }
                }

                if (isChild)
                {
                    strChildPath = strPortalAlias.Substring(strPortalAlias.LastIndexOf("/") + 1);
                }

                //Create Portal
                intPortalId = objPortalController.CreatePortal(strPortalName, strFirstName, strLastName, strUserName, strPassword, strEmail, strDescription, strKeyWords, strHostPath, strTemplate,
                strHomeDirectory, strPortalAlias, strServerPath, strServerPath + strChildPath, isChild);

                if (intPortalId > -1)
                {
                    //Add Extra Aliases
                    foreach (XmlNode portalAlias in portalAliases)
                    {
                        if (!string.IsNullOrEmpty(portalAlias.InnerText))
                        {
                            if (status)
                            {
                                HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Creating Portal Alias: " + portalAlias.InnerText + "<br>");
                            }
                            objPortalController.AddPortalAlias(intPortalId, portalAlias.InnerText);
                        }
                    }

                    //Force Administrator to Update Password on first log in
                    PortalInfo objPortal = objPortalController.GetPortal(intPortalId);
                    UserInfo objAdminUser = UserController.GetUserById(intPortalId, objPortal.AdministratorId);
                    objAdminUser.Membership.UpdatePassword = true;
                    UserController.UpdateUser(intPortalId, objAdminUser);
                }


                return intPortalId;
            }
            catch (Exception ex)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "<font color='red'>Error: " + ex.Message + "</font><br>");
                // failure
                return -1;
            }
        }

        public static string BuildUserTable(IDataReader dr, string header, string message)
        {

            string strWarnings = Null.NullString;
            StringBuilder sbWarnings = new StringBuilder();
            bool hasRows = false;

            sbWarnings.Append("<h3>" + header + "</h3>");
            sbWarnings.Append("<p>" + message + "</p>");
            sbWarnings.Append("<table cellspacing='4' cellpadding='4' border='0'>");
            sbWarnings.Append("<tr>");
            sbWarnings.Append("<td class='NormalBold'>ID</td>");
            sbWarnings.Append("<td class='NormalBold'>UserName</td>");
            sbWarnings.Append("<td class='NormalBold'>First Name</td>");
            sbWarnings.Append("<td class='NormalBold'>Last Name</td>");
            sbWarnings.Append("<td class='NormalBold'>Email</td>");
            sbWarnings.Append("</tr>");
            while (dr.Read())
            {
                hasRows = true;
                sbWarnings.Append("<tr>");
                sbWarnings.Append("<td class='Norma'>" + dr.GetInt32(0).ToString() + "</td>");
                sbWarnings.Append("<td class='Norma'>" + dr.GetString(1) + "</td>");
                sbWarnings.Append("<td class='Norma'>" + dr.GetString(2) + "</td>");
                sbWarnings.Append("<td class='Norma'>" + dr.GetString(3) + "</td>");
                sbWarnings.Append("<td class='Norma'>" + dr.GetString(4) + "</td>");
                sbWarnings.Append("</tr>");
            }

            sbWarnings.Append("</table>");

            if (hasRows)
            {
                strWarnings = sbWarnings.ToString();
            }


            return strWarnings;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CheckUpgrade checks whether there are any possible upgrade issues
        /// </summary>
        /// <history>
        /// [cnurse]	04/11/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string CheckUpgrade()
        {
            DataProvider dataProvider = Data.DataProvider.Instance();
            IDataReader dr = default(IDataReader);
            string strWarnings = Null.NullString;
            StringBuilder sbWarnings = new StringBuilder();
            bool hasRows = false;

            try
            {
                dr = dataProvider.ExecuteReader("CheckUpgrade");

                strWarnings = BuildUserTable(dr, "Duplicate SuperUsers", "We have detected that the following SuperUsers have duplicate entries as Portal Users. Although, no longer supported, these users may have been created in early Betas of DNN v3.0. You need to be aware that after the upgrade, these users will only be able to log in using the Super User Account's password.");

                if (dr.NextResult())
                {
                    strWarnings += BuildUserTable(dr, "Duplicate Portal Users", "We have detected that the following Users have duplicate entries (they exist in more than one portal). You need to be aware that after the upgrade, the password for some of these users may have been automatically changed (as the system now only uses one password per user, rather than one password per user per portal). It is important to remember that your Users can always retrieve their password using the Password Reminder feature, which will be sent to the Email addess shown in the table.");

                }
            }
            catch (SqlException ex)
            {
                strWarnings += ex.Message;
            }
            catch (Exception ex)
            {
                strWarnings += ex.Message;
            }

            try
            {
                dr = dataProvider.ExecuteReader("GetUserCount");
                dr.Read();
                int userCount = dr.GetInt32(0);
                double time = userCount / 10834;
                if (userCount > 1000)
                {
                    strWarnings += "<br/><h3>More than 1000 Users</h3><p>This DotNetNuke Database has " + userCount.ToString() + " users. As the users and their profiles are transferred to a new format, it is estimated that the script will take ~" + time.ToString("F2") + " minutes to execute.</p>";
                }
            }
            catch (Exception ex)
            {
                strWarnings += Environment.NewLine + Environment.NewLine + ex.Message;
            }


            return strWarnings;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// DeleteFiles - clean up deprecated files and folders
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="version">The Version being Upgraded</param>
        /// <history>
        /// [swalker]	11/09/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string DeleteFiles(System.Version version, bool writeFeedback)
        {
            string strExceptions = "";
            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Cleaning Up Files: " + Globals.FormatVersion(version));
            }

            try
            {
                string strListFile = Globals.InstallMapPath + "Cleanup\\" + GetStringVersion(version) + ".txt";

                if (File.Exists(strListFile))
                {
                    strExceptions = FileSystemUtils.DeleteFiles(FileSystemUtils.ReadFile(strListFile).Split('\r', '\n'));
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
                strExceptions += "Error: " + ex.Message + Environment.NewLine;
            }

            if (writeFeedback)
            {
                HtmlUtils.WriteSuccessError(HttpContext.Current.Response, (string.IsNullOrEmpty(strExceptions)));
            }

            return strExceptions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExecuteScripts manages the Execution of Scripts from the Install/Scripts folder.
        /// It is also triggered by InstallDNN and UpgradeDNN
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strProviderPath">The path to the Data Provider</param>
        /// <history>
        /// [cnurse]	05/04/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void ExecuteScripts(string strProviderPath)
        {
            string[] arrFiles = null;
            string ScriptPath = Globals.ApplicationMapPath + "\\Install\\Scripts\\";
            if (Directory.Exists(ScriptPath))
            {
                arrFiles = Directory.GetFiles(ScriptPath);
                foreach (string strFile in arrFiles)
                {
                    //Execute if script is a provider script
                    if (strFile.IndexOf("." + DefaultProvider) != -1)
                    {
                        ExecuteScript(strFile, true);
                        // delete the file
                        try
                        {
                            File.SetAttributes(strFile, FileAttributes.Normal);
                            File.Delete(strFile);
                        }
                        catch
                        {
                        }
                        // error removing the file
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExecuteScript executes a special script
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strFile">The script file to execute</param>
        /// <history>
        /// [cnurse]	04/11/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void ExecuteScript(string strFile)
        {
            //Execute if script is a provider script
            if (strFile.IndexOf("." + DefaultProvider) != -1)
            {
                ExecuteScript(strFile, true);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetInstallTemplate retrieves the Installation Template as specifeid in web.config
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="xmlDoc">The Xml Document to load</param>
        /// <returns>A string which contains the error message - if appropriate</returns>
        /// <history>
        /// [cnurse]	02/13/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetInstallTemplate(XmlDocument xmlDoc)
        {
            string strErrorMessage = Null.NullString;
            string installTemplate = Config.GetSetting("InstallTemplate");
            try
            {
                xmlDoc.Load(Common.Globals.ApplicationMapPath + "\\Install\\" + installTemplate);
            }
            catch
            {
                // error
                strErrorMessage = "Failed to load Install template.<br><br>";
            }

            return strErrorMessage;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetInstallVersion retrieves the Base Instal Version as specifeid in the install
        /// template
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="xmlDoc">The Install Template</param>
        /// <history>
        /// [cnurse]	02/13/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static System.Version GetInstallVersion(XmlDocument xmlDoc)
        {
            string strVersion = Null.NullString;
            XmlNode node = default(XmlNode);

            //get base version
            node = xmlDoc.SelectSingleNode("//dotnetnuke");
            if ((node != null))
            {
                strVersion = XmlUtils.GetNodeValue(node.CreateNavigator(), "version");
            }

            return new System.Version(strVersion);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetLogFile gets the filename for the version's log file
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strProviderPath">The path to the Data Provider</param>
        ///	<param name="version">The Version</param>
        /// <history>
        /// [cnurse]	02/16/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetLogFile(string strProviderPath, System.Version version)
        {
            return strProviderPath + GetStringVersion(version) + ".log.resources";
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetScriptFile gets the filename for the version
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strProviderPath">The path to the Data Provider</param>
        ///	<param name="version">The Version</param>
        /// <history>
        /// [cnurse]	02/16/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetScriptFile(string strProviderPath, System.Version version)
        {
            return strProviderPath + GetStringVersion(version) + "." + DefaultProvider;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetStringVersion gets the Version String (xx.xx.xx) from the Version
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="version">The Version</param>
        /// <history>
        /// [cnurse]	02/15/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string GetStringVersion(System.Version version)
        {
            int[] intVersion = new int[3];
            intVersion[0] = version.Major;
            intVersion[1] = version.Minor;
            intVersion[2] = version.Build;
            string strVersion = Null.NullString;
            for (int i = 0; i <= 2; i++)
            {
                if (intVersion[i] == 0)
                {
                    strVersion += "00";
                }
                else if (intVersion[i] >= 1 && intVersion[i] <= 9)
                {
                    strVersion += "0" + intVersion[i].ToString();
                }
                else
                {
                    strVersion += intVersion[i].ToString();
                }
                if (i < 2)
                {
                    strVersion += ".";
                }
            }
            return strVersion;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSuperUser gets the superuser from the Install Template
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="xmlTemplate">The install Templae</param>
        ///	<param name="writeFeedback">a flag to determine whether to output feedback</param>
        /// <history>
        /// [cnurse]	02/16/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static UserInfo GetSuperUser(XmlDocument xmlTemplate, bool writeFeedback)
        {
            XmlNode node = xmlTemplate.SelectSingleNode("//dotnetnuke/superuser");
            UserInfo objSuperUserInfo = null;
            if ((node != null))
            {
                if (writeFeedback)
                {
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Configuring SuperUser:<br>");
                }

                //Parse the SuperUsers nodes
                string strFirstName = XmlUtils.GetNodeValue(node.CreateNavigator(), "firstname");
                string strLastName = XmlUtils.GetNodeValue(node.CreateNavigator(), "lastname");
                string strUserName = XmlUtils.GetNodeValue(node.CreateNavigator(), "username");
                string strPassword = XmlUtils.GetNodeValue(node.CreateNavigator(), "password");
                string strEmail = XmlUtils.GetNodeValue(node.CreateNavigator(), "email");
                string strLocale = XmlUtils.GetNodeValue(node.CreateNavigator(), "locale");
                int timeZone = XmlUtils.GetNodeValueInt(node, "timezone");

                objSuperUserInfo = new UserInfo();
                objSuperUserInfo.PortalID = -1;
                objSuperUserInfo.FirstName = strFirstName;
                objSuperUserInfo.LastName = strLastName;
                objSuperUserInfo.Username = strUserName;
                objSuperUserInfo.DisplayName = strFirstName + " " + strLastName;
                objSuperUserInfo.Membership.Password = strPassword;
                objSuperUserInfo.Email = strEmail;
                objSuperUserInfo.IsSuperUser = true;
                objSuperUserInfo.Membership.Approved = true;

                objSuperUserInfo.Profile.FirstName = strFirstName;
                objSuperUserInfo.Profile.LastName = strLastName;
                objSuperUserInfo.Profile.PreferredLocale = strLocale;
                objSuperUserInfo.Profile.TimeZone = timeZone;
            }
            return objSuperUserInfo;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetUpgradeScripts gets an ArrayList of the Scripts required to Upgrade to the
        /// current Assembly Version
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strProviderPath">The path to the Data Provider</param>
        ///	<param name="databaseVersion">The current Database Version</param>
        /// <history>
        /// [cnurse]	02/14/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ArrayList GetUpgradeScripts(string strProviderPath, Version databaseVersion)
        {
            Version scriptVersion = default(Version);
            ArrayList arrScriptFiles = new ArrayList();
            string[] arrFiles = Directory.GetFiles(strProviderPath, "*." + DefaultProvider);
            foreach (string strFile in arrFiles)
            {
                // script file name must conform to ##.##.##.DefaultProviderName
                if (Path.GetFileName(strFile).Length == 9 + DefaultProvider.Length)
                {
                    scriptVersion = new System.Version(Path.GetFileNameWithoutExtension(strFile));
                    // check if script file is relevant for upgrade
                    if (scriptVersion > databaseVersion && scriptVersion <= AppContext.Current.Application.Version)
                    {
                        arrScriptFiles.Add(strFile);
                    }
                }
            }
            arrScriptFiles.Sort();

            return arrScriptFiles;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// InitialiseHostSettings gets the Host Settings from the Install Template
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="xmlTemplate">The install Templae</param>
        ///	<param name="writeFeedback">a flag to determine whether to output feedback</param>
        /// <history>
        /// [cnurse]	02/16/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void InitialiseHostSettings(XmlDocument xmlTemplate, bool writeFeedback)
        {
            XmlNode node = xmlTemplate.SelectSingleNode("//dotnetnuke/settings");
            if ((node != null))
            {
                if (writeFeedback)
                {
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Loading Host Settings:<br>");
                }

                HostSettingsController objController = new HostSettingsController();

                //Parse the Settings nodes
                foreach (XmlNode settingNode in node.ChildNodes)
                {
                    string strSettingName = settingNode.Name;
                    string strSettingValue = settingNode.InnerText;
                    XmlAttribute SecureAttrib = settingNode.Attributes["Secure"];
                    bool SettingIsSecure = false;
                    if ((SecureAttrib != null))
                    {
                        if (SecureAttrib.Value.ToLower() == "true")
                        {
                            SettingIsSecure = true;
                        }
                    }

                    string strDomainName = Globals.GetDomainName(HttpContext.Current.Request);

                    switch (strSettingName)
                    {
                        case "HostURL":
                            if (string.IsNullOrEmpty(strSettingValue))
                            {
                                strSettingValue = strDomainName;
                            }
                            break;
                        case "HostEmail":
                            if (string.IsNullOrEmpty(strSettingValue))
                            {
                                strSettingValue = "support@" + strDomainName;

                                //Remove any folders
                                strSettingValue = strSettingValue.Substring(0, strSettingValue.IndexOf("/"));

                                //Remove port number
                                if (strSettingValue.IndexOf(":") != -1)
                                {
                                    strSettingValue = strSettingValue.Substring(0, strSettingValue.IndexOf(":"));
                                }
                            }

                            break;
                    }


                    objController.UpdateHostSetting(strSettingName, strSettingValue, SettingIsSecure);
                }
                //Need to clear the cache to pick up new HostSettings from the SQLDataProvider script
                DataCache.RemoveCache("GetHostSettings");
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// InstallDatabase runs all the "scripts" identifed in the Install Template to
        /// install the base version
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="xmlDoc">The Xml Document to load</param>
        /// <param name="writeFeedback">A flag that determines whether to output feedback to the Response Stream</param>
        /// <returns>A string which contains the error message - if appropriate</returns>
        /// <history>
        /// [cnurse]	02/13/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string InstallDatabase(System.Version version, string strProviderPath, XmlDocument xmlDoc, bool writeFeedback)
        {
            XmlNode node = default(XmlNode);
            string strScript = Null.NullString;
            string strDefaultProvider = Config.GetDefaultProvider("data").Name;
            string strMessage = Null.NullString;

            //Output feedback line
            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Installing Version: " + Globals.FormatVersion(version) + "<br>");
            }

            //Parse the script nodes
            node = xmlDoc.SelectSingleNode("//dotnetnuke/scripts");
            if ((node != null))
            {
                // Loop through the available scripts
                foreach (XmlNode scriptNode in node.SelectNodes("script"))
                {
                    strScript = scriptNode.InnerText + "." + strDefaultProvider;
                    strMessage += ExecuteScript(strProviderPath + strScript, writeFeedback);
                }
            }

            // update the version
            Globals.UpdateDataBaseVersion(version);

            //Optionally Install the memberRoleProvider
            strMessage += InstallMemberRoleProvider(strProviderPath, writeFeedback);

            return strMessage;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// InstallDNN manages the Installation of a new DotNetNuke Application
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strProviderPath">The path to the Data Provider</param>
        /// <history>
        /// [cnurse]	11/06/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void InstallDNN(string strProviderPath)
        {
            int intPortalId = 0;
            string strHostPath = Common.Globals.HostMapPath;
            XmlDocument xmlDoc = new XmlDocument();
            XmlNodeList nodes = default(XmlNodeList);
            string strErrorMessage = "";

            // open the Install Template XML file
            strErrorMessage = GetInstallTemplate(xmlDoc);

            if (string.IsNullOrEmpty(strErrorMessage))
            {
                //get base version
                System.Version baseVersion = GetInstallVersion(xmlDoc);

                //Install Base Version
                strErrorMessage = InstallDatabase(baseVersion, strProviderPath, xmlDoc, true);

                //Call Upgrade with the current DB Version to carry out any incremental upgrades
                UpgradeDNN(strProviderPath, baseVersion);

                // parse Host Settings if available
                InitialiseHostSettings(xmlDoc, true);

                // parse SuperUser if Available
                UserInfo superUser = GetSuperUser(xmlDoc, true);
                if (superUser.Membership.Password.Contains("host"))
                {
                    superUser.Membership.UpdatePassword = true;
                }
                UserController.CreateUser(ref superUser);

                // parse File List if available
                InstallFiles(xmlDoc, true);

                //Run any addition scripts in the Scripts folder
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Executing Additional Scripts:<br>");
                ExecuteScripts(strProviderPath);

                //Install optional resources if present
                InstallPackages("Module", true);
                InstallPackages("Skin", true);
                InstallPackages("Container", true);
                InstallPackages("Language", true);
                InstallPackages("Provider", true);
                InstallPackages("AuthSystem", true);
                InstallPackages("Package", true);

                //Set Status to None
                Globals.SetStatus(Globals.UpgradeStatus.None);

                // parse portal(s) if available
                nodes = xmlDoc.SelectNodes("//dotnetnuke/portals/portal");
                foreach (XmlNode node in nodes)
                {
                    if ((node != null))
                    {
                        intPortalId = AddPortal(node, true, 2);
                        if (intPortalId > -1)
                        {
                            HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "<font color='green'>Successfully Installed Portal " + intPortalId + ":</font><br>");
                        }
                        else
                        {
                            HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "<font color='red'>Portal failed to install:Error!</font><br>");
                        }
                    }
                }
            }
            else
            {
                //500 Error - Redirect to ErrorPage
                if ((HttpContext.Current != null))
                {
                    string strURL = "~/ErrorPage.aspx?status=500&error=" + strErrorMessage;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Server.Transfer(strURL);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// InstallFiles intsalls any files listed in the Host Install Configuration file
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="xmlDoc">The Xml Document to load</param>
        /// <param name="writeFeedback">A flag that determines whether to output feedback to the Response Stream</param>
        /// <history>
        /// [cnurse]	02/19/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void InstallFiles(XmlDocument xmlDoc, bool writeFeedback)
        {
            XmlNode node = default(XmlNode);

            //Parse the file nodes
            node = xmlDoc.SelectSingleNode("//dotnetnuke/files");
            if ((node != null))
            {
                if (writeFeedback)
                {
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Loading Host Files:<br>");
                }
                ParseFiles(node, Null.NullInteger);
            }

            //Synchronise Host Folder
            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Synchronizing Host Files:<br>");
            }

            FileSystemUtils.SynchronizeFolder(Null.NullInteger, Common.Globals.HostMapPath, "", true, false, true, false);
        }

        public static bool InstallPackage(string strFile, string packageType, bool writeFeedback)
        {
            bool blnSuccess = Null.NullBoolean;
            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Installing Package File " + Path.GetFileNameWithoutExtension(strFile) + ": ");
            }

            bool deleteTempFolder = true;
            if (packageType == "Skin" || packageType == "Container")
            {
                deleteTempFolder = Null.NullBoolean;
            }

            Installer.Installer objInstaller = new Installer.Installer(new FileStream(strFile, FileMode.Open, FileAccess.Read), Common.Globals.ApplicationMapPath, true, deleteTempFolder);

            //Check if manifest is valid
            if (objInstaller.IsValid)
            {
                objInstaller.InstallerInfo.RepairInstall = true;
                blnSuccess = objInstaller.Install();
            }
            else
            {
                if (objInstaller.InstallerInfo.ManifestFile == null)
                {
                    //Missing manifest
                    if (packageType == "Skin" || packageType == "Container")
                    {
                        //Legacy Skin/Container
                        string TempInstallFolder = objInstaller.TempInstallFolder;
                        string ManifestFile = Path.Combine(TempInstallFolder, Path.GetFileNameWithoutExtension(strFile) + ".dnn");
                        StreamWriter manifestWriter = new StreamWriter(ManifestFile);
                        manifestWriter.Write(LegacyUtil.CreateSkinManifest(strFile, packageType, TempInstallFolder));
                        manifestWriter.Close();

                        objInstaller = new Installer.Installer(TempInstallFolder, ManifestFile, HttpContext.Current.Request.MapPath("."), true);

                        //Set the Repair flag to true for Batch Install
                        objInstaller.InstallerInfo.RepairInstall = true;

                        blnSuccess = objInstaller.Install();
                    }
                }
                else
                {
                    blnSuccess = false;
                }
            }

            if (writeFeedback)
            {
                HtmlUtils.WriteSuccessError(HttpContext.Current.Response, blnSuccess);
            }
            if (blnSuccess)
            {
                // delete file
                try
                {
                    File.SetAttributes(strFile, FileAttributes.Normal);
                    File.Delete(strFile);
                }
                catch
                {
                }
                // error removing the file
            }
            return blnSuccess;
        }

        public static void InstallPackages(string packageType, bool writeFeedback)
        {
            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Installing Optional " + packageType + "s:<br>");
            }
            string InstallPackagePath = Globals.ApplicationMapPath + "\\Install\\" + packageType;
            if (Directory.Exists(InstallPackagePath))
            {
                foreach (string strFile in Directory.GetFiles(InstallPackagePath))
                {
                    // check if valid custom module
                    if (Path.GetExtension(strFile.ToLower()) == ".zip")
                    {
                        InstallPackage(strFile, packageType, writeFeedback);
                    }
                }
            }
        }

        public static bool IsNETFrameworkCurrent(string version)
        {
            bool isCurrent = Null.NullBoolean;
            switch (version)
            {
                case "3.5":
                    //Try and instantiate a 3.5 Class
                    if (Framework.Reflection.CreateType("System.Data.Linq.DataContext", true) != null)
                    {
                        isCurrent = true;
                    }
                    break;
                case "4.0":
                    //Look for requestValidationMode attribute
                    XmlDocument configFile = Config.Load();
                    XPathNavigator configNavigator = configFile.CreateNavigator().SelectSingleNode("//configuration/system.web/httpRuntime");
                    if (configNavigator != null && !string.IsNullOrEmpty(configNavigator.GetAttribute("requestValidationMode", "")))
                    {
                        isCurrent = true;
                    }
                    break;
            }
            return isCurrent;
        }

        public static void RemoveAdminPages(string tabPath)
        {
            PortalController objPortalController = new PortalController();
            TabController objTabController = new TabController();

            ArrayList portals = objPortalController.GetPortals();
            foreach (PortalInfo portal in portals)
            {
                int tabID = TabController.GetTabByTabPath(portal.PortalID, tabPath);
                if ((tabID != Null.NullInteger))
                {
                    objTabController.DeleteTab(tabID, portal.PortalID);
                }
            }
        }

        public static void RemoveHostPage(string pageName)
        {
            TabController controller = new TabController();
            TabInfo skinsTab = controller.GetTabByName(pageName, Null.NullInteger);
            if (skinsTab != null)
            {
                controller.DeleteTab(skinsTab.TabID, Null.NullInteger);
            }
        }

        public static void StartTimer()
        {

            //Start Upgrade Timer

            startTime = DateTime.Now;
        }

        public static void TryUpgradeNETFramework()
        {
            switch (Common.Globals.NETFrameworkVersion.ToString(2))
            {
                case "3.5":
                    if (!IsNETFrameworkCurrent("3.5"))
                    {
                        //Upgrade to .NET 3.5
                        string upgradeFile = string.Format("{0}\\Config\\Net35.config", Globals.InstallMapPath);
                        string strMessage = UpdateConfig(upgradeFile, AppContext.Current.Application.Version, ".NET 3.5 Upgrade");
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            //Remove old AJAX file
                            FileSystemUtils.DeleteFile(Path.Combine(Globals.ApplicationMapPath, "bin\\System.Web.Extensions.dll"));

                            //Log Upgrade
                            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();

                            objEventLog.AddLog("UpgradeNet", "Upgraded Site to .NET 3.5", PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT);
                        }
                        else
                        {
                            //Log Failed Upgrade
                            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                            objEventLog.AddLog("UpgradeNet", string.Format("Upgrade to .NET 3.5 failed. Error reported during attempt to update:{0}", strMessage), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT);
                        }
                    }
                    break;
                case "4.0":
                    if (!IsNETFrameworkCurrent("4.0"))
                    {
                        //Upgrade to .NET 4.0
                        string upgradeFile = string.Format("{0}\\Config\\Net40.config", Globals.InstallMapPath);
                        string strMessage = UpdateConfig(upgradeFile, AppContext.Current.Application.Version, ".NET 4.0 Upgrade");
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            //Log Upgrade
                            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                            objEventLog.AddLog("UpgradeNet", "Upgraded Site to .NET 4.0", PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT);
                        }
                        else
                        {
                            //Log Failed Upgrade
                            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                            objEventLog.AddLog("UpgradeNet", string.Format("Upgrade to .NET 4.0 failed. Error reported during attempt to update:{0}", strMessage), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT);
                        }
                    }
                    break;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeApplication - This overload is used for general application upgrade operations.
        /// </summary>
        /// <remarks>
        ///	Since it is not version specific and is invoked whenever the application is
        ///	restarted, the operations must be re-executable.
        /// </remarks>
        /// <history>
        /// [cnurse]	11/6/2004	documented
        /// [cnurse] 02/27/2007 made public so it can be called from Wizard
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void UpgradeApplication()
        {

            TabController objTabController = new TabController();
            TabInfo HostPage = objTabController.GetTabByName("Host", Null.NullInteger);
            TabInfo newPage = default(TabInfo);

            int ModuleDefID = 0;

            try
            {
                // remove the system message module from the admin tab
                // System Messages are now managed through Localization
                if (CoreModuleExists("System Messages"))
                {
                    RemoveCoreModule("System Messages", "Admin", "Site Settings", false);
                }

                // remove portal alias module
                if (CoreModuleExists("PortalAliases"))
                {
                    RemoveCoreModule("PortalAliases", "Admin", "Site Settings", false);
                }

                // add the log viewer module to the admin tab
                if (CoreModuleExists("LogViewer") == false)
                {
                    ModuleDefID = AddModuleDefinition("LogViewer", "Allows you to view log entries for portal events.", "Log Viewer");
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/LogViewer/LogViewer.ascx", "", SecurityAccessLevel.Admin, 0);
                    AddModuleControl(ModuleDefID, "Edit", "Edit Log Settings", "DesktopModules/Admin/LogViewer/EditLogTypes.ascx", "", SecurityAccessLevel.Host, 0);

                    //Add the Module/Page to all configured portals
                    AddAdminPages("Log Viewer", "View a historical log of database events such as event schedules, exceptions, account logins, module and page changes, user account activities, security role activities, etc.", "icon_viewstats_16px.gif", "icon_viewstats_32px.gif", true, ModuleDefID, "Log Viewer", "icon_viewstats_16px.gif");
                }

                // add the schedule module to the host tab
                if (CoreModuleExists("Scheduler") == false)
                {
                    ModuleDefID = AddModuleDefinition("Scheduler", "Allows you to schedule tasks to be run at specified intervals.", "Scheduler");
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Scheduler/ViewSchedule.ascx", "", SecurityAccessLevel.Admin, 0);
                    AddModuleControl(ModuleDefID, "Edit", "Edit Schedule", "DesktopModules/Admin/Scheduler/EditSchedule.ascx", "", SecurityAccessLevel.Host, 0);
                    AddModuleControl(ModuleDefID, "History", "Schedule History", "DesktopModules/Admin/Scheduler/ViewScheduleHistory.ascx", "", SecurityAccessLevel.Host, 0);
                    AddModuleControl(ModuleDefID, "Status", "Schedule Status", "DesktopModules/Admin/Scheduler/ViewScheduleStatus.ascx", "", SecurityAccessLevel.Host, 0);

                    //Create New Host Page (or get existing one)
                    newPage = AddHostPage("Schedule", "Add, modify and delete scheduled tasks to be run at specified intervals.", "icon_scheduler_16px.gif", "icon_scheduler_32px.gif", true);

                    //Add Module To Page
                    AddModuleToPage(newPage, ModuleDefID, "Schedule", "icon_scheduler_16px.gif");
                }

                // add the Search Admin module to the host tab
                if (CoreModuleExists("SearchAdmin") == false)
                {
                    ModuleDefID = AddModuleDefinition("SearchAdmin", "The Search Admininstrator provides the ability to manage search settings.", "Search Admin");
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SearchAdmin/SearchAdmin.ascx", "", SecurityAccessLevel.Host, 0);

                    //Create New Host Page (or get existing one)
                    newPage = AddHostPage("Search Admin", "Manage search settings associated with DotNetNuke's search capability.", "icon_search_16px.gif", "icon_search_32px.gif", true);

                    //Add Module To Page
                    AddModuleToPage(newPage, ModuleDefID, "Search Admin", "icon_search_16px.gif");
                }

                // add the Search Input module
                if (CoreModuleExists("SearchInput") == false)
                {
                    ModuleDefID = AddModuleDefinition("SearchInput", "The Search Input module provides the ability to submit a search to a given search results module.", "Search Input", false, false);
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SearchInput/SearchInput.ascx", "", SecurityAccessLevel.Anonymous, 0);
                    AddModuleControl(ModuleDefID, "Settings", "Search Input Settings", "DesktopModules/Admin/SearchInput/Settings.ascx", "", SecurityAccessLevel.Edit, 0);
                }

                // add the Search Results module
                if (CoreModuleExists("SearchResults") == false)
                {
                    ModuleDefID = AddModuleDefinition("SearchResults", "The Search Reasults module provides the ability to display search results.", "Search Results", false, false);
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SearchResults/SearchResults.ascx", "", SecurityAccessLevel.Anonymous, 0);
                    AddModuleControl(ModuleDefID, "Settings", "Search Results Settings", "DesktopModules/Admin/SearchResults/Settings.ascx", "", SecurityAccessLevel.Edit, 0);

                    //Add the Search Module/Page to all configured portals
                    AddSearchResults(ModuleDefID);
                }

                // add the site wizard module to the admin tab
                if (CoreModuleExists("SiteWizard") == false)
                {
                    ModuleDefID = AddModuleDefinition("SiteWizard", "The Administrator can use this user-friendly wizard to set up the common Extensions of the Portal/Site.", "Site Wizard");
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SiteWizard/Sitewizard.ascx", "", SecurityAccessLevel.Admin, 0);
                    AddAdminPages("Site Wizard", "Configure portal settings, page design and apply a site template using a step-by-step wizard.", "icon_wizard_16px.gif", "icon_wizard_32px.gif", true, ModuleDefID, "Site Wizard", "icon_wizard_16px.gif");
                }

                //add Lists module and tab
                if (HostTabExists("Lists") == false)
                {
                    ModuleDefID = AddModuleDefinition("Lists", "Allows you to edit common lists.", "Lists");
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Lists/ListEditor.ascx", "", SecurityAccessLevel.Host, 0);

                    //Create New Host Page (or get existing one)
                    newPage = AddHostPage("Lists", "Manage common lists.", "icon_lists_16px.gif", "icon_lists_32px.gif", true);

                    //Add Module To Page
                    AddModuleToPage(newPage, ModuleDefID, "Lists", "icon_lists_16px.gif");
                }

                if (HostTabExists("Superuser Accounts") == false)
                {
                    //add SuperUser Accounts module and tab
                    DesktopModuleInfo objDesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Security", Null.NullInteger);
                    ModuleDefID = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("User Accounts", objDesktopModuleInfo.DesktopModuleID).ModuleDefID;

                    //Create New Host Page (or get existing one)
                    newPage = AddHostPage("Superuser Accounts", "Manage host user accounts.", "icon_users_16px.gif", "icon_users_32px.gif", true);

                    //Add Module To Page
                    AddModuleToPage(newPage, ModuleDefID, "Superuser Accounts", "icon_users_32px.gif");
                }

                //Add Edit Role Groups
                ModuleDefID = GetModuleDefinition("Security", "Security Roles");
                AddModuleControl(ModuleDefID, "EditGroup", "Edit Role Groups", "DesktopModules/Admin/Security/EditGroups.ascx", "icon_securityroles_32px.gif", SecurityAccessLevel.Edit, Null.NullInteger);
                AddModuleControl(ModuleDefID, "UserSettings", "Manage User Settings", "DesktopModules/Admin/Security/UserSettings.ascx", "~/images/settings.gif", SecurityAccessLevel.Edit, Null.NullInteger);

                //Add User Accounts Controls
                ModuleDefID = GetModuleDefinition("Security", "User Accounts");
                AddModuleControl(ModuleDefID, "ManageProfile", "Manage Profile Definition", "DesktopModules/Admin/Security/ProfileDefinitions.ascx", "icon_users_32px.gif", SecurityAccessLevel.Edit, Null.NullInteger);
                AddModuleControl(ModuleDefID, "EditProfileProperty", "Edit Profile Property Definition", "DesktopModules/Admin/Security/EditProfileDefinition.ascx", "icon_users_32px.gif", SecurityAccessLevel.Edit, Null.NullInteger);
                AddModuleControl(ModuleDefID, "UserSettings", "Manage User Settings", "DesktopModules/Admin/Security/UserSettings.ascx", "~/images/settings.gif", SecurityAccessLevel.Edit, Null.NullInteger);
                AddModuleControl(Null.NullInteger, "Profile", "Profile", "DesktopModules/Admin/Security/ManageUsers.ascx", "icon_users_32px.gif", SecurityAccessLevel.Anonymous, Null.NullInteger);
                AddModuleControl(Null.NullInteger, "SendPassword", "Send Password", "DesktopModules/Admin/Security/SendPassword.ascx", "", SecurityAccessLevel.Anonymous, Null.NullInteger);
                AddModuleControl(Null.NullInteger, "ViewProfile", "View Profile", "DesktopModules/Admin/Security/ViewProfile.ascx", "icon_users_32px.gif", SecurityAccessLevel.Anonymous, Null.NullInteger);

                //Update Child Portal subHost.aspx
                PortalAliasController objAliasController = new PortalAliasController();
                ArrayList arrAliases = objAliasController.GetPortalAliasArrayByPortalID(Null.NullInteger);
                string childPath = null;

                foreach (PortalAliasInfo objAlias in arrAliases)
                {
                    //For the alias to be for a child it must be of the form ...../child
                    int intChild = objAlias.HTTPAlias.IndexOf("/");
                    if (intChild != -1 & intChild != (objAlias.HTTPAlias.Length - 1))
                    {
                        childPath = Globals.ApplicationMapPath + "\\" + objAlias.HTTPAlias.Substring(intChild + 1);
                        // check if Folder exists
                        if (Directory.Exists(childPath))
                        {
                            System.IO.FileInfo objDefault = new System.IO.FileInfo(childPath + "\\" + Globals.glbDefaultPage);
                            System.IO.FileInfo objSubHost = new System.IO.FileInfo(Common.Globals.HostMapPath + "subhost.aspx");
                            // check if upgrade is necessary
                            if (objDefault.Length != objSubHost.Length)
                            {
                                //Rename existing file
                                System.IO.File.Copy(childPath + "\\" + Globals.glbDefaultPage, childPath + "\\old_" + Globals.glbDefaultPage, true);
                                // create the subhost default.aspx file
                                System.IO.File.Copy(Common.Globals.HostMapPath + "subhost.aspx", childPath + "\\" + Globals.glbDefaultPage, true);
                            }
                        }
                    }
                }

                // add the solutions explorer module to the admin tab
                if (CoreModuleExists("Solutions") == false)
                {
                    ModuleDefID = AddModuleDefinition("Solutions", "Browse additional solutions for your application.", "Solutions", false, false);
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Solutions/Solutions.ascx", "", SecurityAccessLevel.Admin, 0);
                    AddAdminPages("Solutions", "DotNetNuke Solutions Explorer page provides easy access to locate free and commercial DotNetNuke modules, skin and more.", "icon_solutions_16px.gif", "icon_solutions_32px.gif", true, ModuleDefID, "Solutions Explorer", "icon_solutions_32px.gif");
                }


                //Add Search Skin Object
                AddSkinControl("SEARCH", "DotNetNuke.SearchSkinObject", "Admin/Skins/Search.ascx");

                //Add TreeView Skin Object
                AddSkinControl("TREEVIEW", "DotNetNuke.TreeViewSkinObject", "Admin/Skins/TreeViewMenu.ascx");

                //Add Text Skin Object
                AddSkinControl("TEXT", "DotNetNuke.TextSkinObject", "Admin/Skins/Text.ascx");

                //Add Styles Skin Object

                AddSkinControl("STYLES", "DotNetNuke.StylesSkinObject", "Admin/Skins/Styles.ascx");
            }
            catch (Exception ex)
            {
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                objEventLogInfo.AddProperty("Upgraded DotNetNuke", "General");
                objEventLogInfo.AddProperty("Warnings", "Error: " + ex.Message + Environment.NewLine);
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.BypassBuffering = true;
                objEventLog.AddLog(objEventLogInfo);
                try
                {
                    Exceptions.Exceptions.LogException(ex);
                }
                catch
                {
                }

                // ignore
            }

            //Remove any .txt and .config files that may exist in the Install folder
            foreach (string strFile in Directory.GetFiles(Globals.InstallMapPath + "Cleanup\\", "??.??.??.txt"))
            {
                FileSystemUtils.DeleteFile(strFile);
            }
            foreach (string strFile in Directory.GetFiles(Globals.InstallMapPath + "Config\\", "??.??.??.config"))
            {
                FileSystemUtils.DeleteFile(strFile);

            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeApplication - This overload is used for version specific application upgrade operations.
        /// </summary>
        /// <remarks>
        ///	This should be used for file system modifications or upgrade operations which
        ///	should only happen once. Database references are not recommended because future
        ///	versions of the application may result in code incompatibilties.
        /// </remarks>
        ///	<param name="Version">The Version being Upgraded</param>
        /// <history>
        /// [cnurse]	11/6/2004	documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string UpgradeApplication(System.Version version, bool writeFeedback)
        {
            string strExceptions = "";
            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Executing Application Upgrades: " + Globals.FormatVersion(version));
            }
            try
            {
                switch (version.ToString(3))
                {
                    case "3.2.3":
                        UpgradeToVersion_323();
                        break;
                    case "4.4.0":
                        UpgradeToVersion_440();
                        break;
                    case "4.7.0":
                        UpgradeToVersion_470();
                        break;
                    case "4.8.2":
                        UpgradeToVersion_482();
                        break;
                    case "5.0.0":
                        UpgradeToVersion_500();
                        break;
                    case "5.0.1":
                        UpgradeToVersion_501();
                        break;
                    case "5.1.0":
                        UpgradeToVersion_510();
                        break;
                    case "5.1.1":
                        UpgradeToVersion_511();
                        break;
                    case "5.1.3":
                        UpgradeToVersion_513();
                        break;
                    case "5.2.0":
                        UpgradeToVersion_520();
                        break;
                    case "5.2.1":
                        UpgradeToVersion_521();
                        break;
                    case "5.3.0":
                        UpgradeToVersion_530();
                        break;
                    case "5.4.0":
                        UpgradeToVersion_540();
                        break;
                    case "5.4.3":
                        UpgradeToVersion_543();
                        break;

                }
            }
            catch (Exception ex)
            {
                strExceptions += string.Format("Error: {0}{1}", ex.Message, Environment.NewLine);
                try
                {
                    Exceptions.Exceptions.LogException(ex);
                }
                catch
                {
                }
                // ignore
            }

            if (writeFeedback)
            {
                HtmlUtils.WriteSuccessError(HttpContext.Current.Response, (string.IsNullOrEmpty(strExceptions)));
            }

            return strExceptions;
        }

        public static string UpdateConfig(System.Version version, bool writeFeedback)
        {
            if (writeFeedback)
            {
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, string.Format("Updating Config Files: {0}", Globals.FormatVersion(version)));
            }
            string strExceptions = UpdateConfig(Globals.InstallMapPath + "Config\\" + GetStringVersion(version) + ".config", version, "Core Upgrade");

            if (writeFeedback)
            {
                HtmlUtils.WriteSuccessError(HttpContext.Current.Response, (string.IsNullOrEmpty(strExceptions)));
            }

            return strExceptions;
        }

        public static string UpdateConfig(string strConfigFile, System.Version version, string strReason)
        {
            string strExceptions = "";
            if (File.Exists(strConfigFile))
            {
                //Create XmlMerge instance from config file source
                StreamReader stream = File.OpenText(strConfigFile);
                try
                {
                    XmlMerge merge = new XmlMerge(stream, version.ToString(3), strReason);

                    //Process merge
                    merge.UpdateConfigs();
                }
                catch (Exception ex)
                {
                    Exceptions.Exceptions.LogException(ex);
                    strExceptions += "Error: " + ex.Message + Environment.NewLine;
                }
                finally
                {
                    //Close stream
                    stream.Close();
                }
            }
            return strExceptions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeDNN manages the Upgrade of an exisiting DotNetNuke Application
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strProviderPath">The path to the Data Provider</param>
        ///	<param name="dataBaseVersion">The current Database Version</param>
        /// <history>
        /// [cnurse]	11/06/2004	created (Upgrade code extracted from AutoUpgrade)
        /// [cnurse] 11/10/2004 version specific upgrades extracted to ExecuteScript
        /// [cnurse] 01/20/2005 changed to Public so Upgrade can be manually controlled
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void UpgradeDNN(string strProviderPath, System.Version dataBaseVersion)
        {
            System.Version version = default(System.Version);

            HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Upgrading to Version: " + Globals.FormatVersion(AppContext.Current.Application.Version) + "<br/>");

            //Process the Upgrade Script files
            List<Version> versions = new List<Version>();
            foreach (string strScriptFile in GetUpgradeScripts(strProviderPath, dataBaseVersion))
            {
                version = new System.Version(Path.GetFileNameWithoutExtension(strScriptFile));
                versions.Add(new System.Version(Path.GetFileNameWithoutExtension(strScriptFile)));
                UpgradeVersion(strScriptFile, true);
            }

            foreach (Version ver in versions)
            {
                //' perform version specific application upgrades
                UpgradeApplication(ver, true);
            }

            foreach (Version ver in versions)
            {
                // delete files which are no longer used
                DeleteFiles(ver, true);
            }
            foreach (Version ver in versions)
            {
                //execute config file updates
                UpdateConfig(ver, true);
            }

            // perform general application upgrades
            HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Performing General Upgrades<br>");
            UpgradeApplication();

            DataCache.ClearHostCache(true);
        }

        public static string UpgradeIndicator(System.Version Version, bool IsLocal, bool IsSecureConnection)
        {
            return UpgradeIndicator(Version, AppContext.Current.Application.Type, AppContext.Current.Application.Name, "", IsLocal, IsSecureConnection);
        }

        public static string UpgradeIndicator(System.Version Version, string PackageType, string PackageName, string Culture, bool IsLocal, bool IsSecureConnection)
        {
            string strURL = "";
            if (Host.CheckUpgrade && Version != new System.Version(0, 0, 0) && (IsLocal == false | Config.GetSetting("ForceUpdateService") == "Y"))
            {
                strURL = AppContext.Current.Application.UpgradeUrl + "/update.aspx";
                if (IsSecureConnection)
                {
                    strURL = strURL.Replace("http://", "https://");
                }
                strURL += "?core=" + Globals.FormatVersion(AppContext.Current.Application.Version, "00", 3, "");
                strURL += "&version=" + Globals.FormatVersion(Version, "00", 3, "");
                strURL += "&type=" + PackageType;
                strURL += "&name=" + PackageName;
                if (!string.IsNullOrEmpty(Culture))
                {
                    strURL += "&culture=" + Culture;
                }
                if (PackageType.ToUpper() == AppContext.Current.Application.Type.ToUpper())
                {
                    strURL += "&os=" + Globals.FormatVersion(Common.Globals.OperatingSystemVersion, "00", 2, "");
                    strURL += "&net=" + Globals.FormatVersion(Common.Globals.NETFrameworkVersion, "00", 2, "");
                    strURL += "&db=" + Globals.FormatVersion(Common.Globals.DatabaseEngineVersion, "00", 2, "");
                }
            }
            return strURL;
        }

        public static string UpgradeRedirect()
        {
            return UpgradeRedirect(AppContext.Current.Application.Version, AppContext.Current.Application.Type, AppContext.Current.Application.Name, "");
        }

        public static string UpgradeRedirect(System.Version Version, string PackageType, string PackageName, string Culture)
        {
            string strURL = "";
            if (!string.IsNullOrEmpty(Config.GetSetting("UpdateServiceRedirect")))
            {
                strURL = Config.GetSetting("UpdateServiceRedirect");
            }
            else
            {
                strURL = AppContext.Current.Application.UpgradeUrl + "/redirect.aspx";
                strURL += "?core=" + Globals.FormatVersion(AppContext.Current.Application.Version, "00", 3, "");
                strURL += "&version=" + Globals.FormatVersion(Version, "00", 3, "");
                strURL += "&type=" + PackageType;
                strURL += "&name=" + PackageName;
                if (!string.IsNullOrEmpty(Culture))
                {
                    strURL += "&culture=" + Culture;
                }
            }
            return strURL;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeVersion upgrades a single version
        /// </summary>
        /// <remarks>
        /// </remarks>
        ///	<param name="strScriptFile">The upgrade script file</param>
        /// <history>
        /// [cnurse]	02/14/2007	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string UpgradeVersion(string strScriptFile, bool writeFeedback)
        {
            System.Version version = new System.Version(Path.GetFileNameWithoutExtension(strScriptFile));
            string strExceptions = Null.NullString;

            // verify script has not already been run
            if (!Globals.FindDatabaseVersion(version.Major, version.Minor, version.Build))
            {
                // upgrade database schema
                DataProvider.Instance().UpgradeDatabaseSchema(version.Major, version.Minor, version.Build);

                // execute script file (and version upgrades) for version
                strExceptions = ExecuteScript(strScriptFile, writeFeedback);

                // update the version
                Globals.UpdateDataBaseVersion(version);

                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                objEventLogInfo.AddProperty("Upgraded DotNetNuke", "Version: " + Globals.FormatVersion(version));
                if (strExceptions.Length > 0)
                {
                    objEventLogInfo.AddProperty("Warnings", strExceptions);
                }
                else
                {
                    objEventLogInfo.AddProperty("No Warnings", "");
                }
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.BypassBuffering = true;
                objEventLog.AddLog(objEventLogInfo);
            }

            return strExceptions;
        }

        #endregion

    }
}
