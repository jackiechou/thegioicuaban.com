using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Services.EventQueue;
using CommonLibrary.Common.Utilities;
using System.Xml.XPath;
using System.IO;
using CommonLibrary.Common;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Modules.Definitions;

namespace CommonLibrary.Services.Installer.Installers
{
    public class ModuleInstaller : ComponentInstallerBase
    {
        private DesktopModuleInfo InstalledDesktopModule;
        private DesktopModuleInfo DesktopModule;
        private EventMessage EventMessage;
        public override string AllowableFiles
        {
            get { return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html, xml, psd"; }
        }
        private void DeleteModule()
        {
            try
            {
                DesktopModuleInfo tempDesktopModule = DesktopModuleController.GetDesktopModuleByPackageID(Package.PackageID);
                if (tempDesktopModule != null)
                {
                    if ((DesktopModule != null) && (!string.IsNullOrEmpty(DesktopModule.CodeSubDirectory)))
                    {
                        Config.RemoveCodeSubDirectory(DesktopModule.CodeSubDirectory);
                    }
                    DesktopModuleController controller = new DesktopModuleController();
                    controller.DeleteDesktopModule(tempDesktopModule);
                }
                Log.AddInfo(string.Format(Util.MODULE_UnRegistered, tempDesktopModule.ModuleName));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void Commit()
        {
            if (!string.IsNullOrEmpty(DesktopModule.CodeSubDirectory))
            {
                Config.AddCodeSubDirectory(DesktopModule.CodeSubDirectory);
            }
            if (DesktopModule.SupportedFeatures == Null.NullInteger)
            {
                //Set an Event Message so the features are loaded by reflection on restart
                EventQueue.EventMessage oAppStartMessage = new EventQueue.EventMessage();
                oAppStartMessage.Priority = MessagePriority.High;
                oAppStartMessage.ExpirationDate = DateTime.Now.AddYears(-1);
                oAppStartMessage.SentDate = System.DateTime.Now;
                oAppStartMessage.Body = "";
                oAppStartMessage.ProcessorType = "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke";
                oAppStartMessage.ProcessorCommand = "UpdateSupportedFeatures";

                //Add custom Attributes for this message
                oAppStartMessage.Attributes.Add("BusinessControllerClass", DesktopModule.BusinessControllerClass);
                oAppStartMessage.Attributes.Add("desktopModuleID", DesktopModule.DesktopModuleID.ToString());

                //send it to occur on next App_Start Event
                EventQueueController.SendMessage(oAppStartMessage, "Application_Start_FirstRequest");
            }
            if (EventMessage != null)
            {
                EventMessage.Attributes.Set("desktopModuleID", DesktopModule.DesktopModuleID.ToString());
                EventQueueController.SendMessage(EventMessage, "Application_Start");
            }
            if (!DesktopModule.IsPremium)
            {
                DesktopModuleController.AddDesktopModuleToPortals(DesktopModule.DesktopModuleID);
            }
        }
        public override void Install()
        {
            try
            {
                InstalledDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModule.ModuleName, Package.InstallerInfo.PortalID);
                if (InstalledDesktopModule != null)
                {
                    DesktopModule.DesktopModuleID = InstalledDesktopModule.DesktopModuleID;
                }
                DataCache.RemoveCache(DataCache.ModuleDefinitionCacheKey);
                DataCache.RemoveCache(DataCache.ModuleControlsCacheKey);
                DesktopModule.PackageID = Package.PackageID;
                DesktopModule.DesktopModuleID = DesktopModuleController.SaveDesktopModule(DesktopModule, true, false);
                Completed = true;
                Log.AddInfo(string.Format(Util.MODULE_Registered, DesktopModule.ModuleName));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void ReadManifest(XPathNavigator manifestNav)
        {
            DesktopModule = CBO.DeserializeObject<DesktopModuleInfo>(new StringReader(manifestNav.InnerXml));
            DesktopModule.FriendlyName = Package.FriendlyName;
            DesktopModule.Description = Package.Description;
            DesktopModule.Version = Globals.FormatVersion(Package.Version);
            //DesktopModule.IsPremium = false;
            //DesktopModule.IsAdmin = false;
            DesktopModule.CompatibleVersions = Null.NullString;
            DesktopModule.Dependencies = Null.NullString;
            DesktopModule.Permissions = Null.NullString;
            if (string.IsNullOrEmpty(DesktopModule.BusinessControllerClass))
            {
                DesktopModule.SupportedFeatures = 0;
            }
            XPathNavigator eventMessageNav = manifestNav.SelectSingleNode("eventMessage");
            if (eventMessageNav != null)
            {
                EventMessage = new EventQueue.EventMessage();
                EventMessage.Priority = MessagePriority.High;
                EventMessage.ExpirationDate = DateTime.Now.AddYears(-1);
                EventMessage.SentDate = System.DateTime.Now;
                EventMessage.Body = "";
                EventMessage.ProcessorType = Util.ReadElement(eventMessageNav, "processorType", Log, Util.EVENTMESSAGE_TypeMissing);
                EventMessage.ProcessorCommand = Util.ReadElement(eventMessageNav, "processorCommand", Log, Util.EVENTMESSAGE_CommandMissing);
                foreach (XPathNavigator attributeNav in eventMessageNav.Select("attributes/*"))
                {
                    EventMessage.Attributes.Add(attributeNav.Name, attributeNav.Value);
                }
            }
            foreach (XPathNavigator moduleDefinitionNav in manifestNav.Select("desktopModule/moduleDefinitions/moduleDefinition"))
            {
                string friendlyName = Util.ReadElement(moduleDefinitionNav, "friendlyName");
                foreach (XPathNavigator permissionNav in moduleDefinitionNav.Select("permissions/permission"))
                {
                    PermissionInfo permission = new PermissionInfo();
                    permission.PermissionCode = Util.ReadAttribute(permissionNav, "code");
                    permission.PermissionKey = Util.ReadAttribute(permissionNav, "key");
                    permission.PermissionName = Util.ReadAttribute(permissionNav, "name");
                    ModuleDefinitionInfo moduleDefinition = DesktopModule.ModuleDefinitions[friendlyName];
                    if (moduleDefinition != null)
                    {
                        moduleDefinition.Permissions.Add(permission.PermissionKey, permission);
                    }
                }
            }
            if (Log.Valid)
            {
                Log.AddInfo(Util.MODULE_ReadSuccess);
            }
        }
        public override void Rollback()
        {
            if (InstalledDesktopModule == null)
            {
                DeleteModule();
            }
            else
            {
                DesktopModuleController.SaveDesktopModule(InstalledDesktopModule, true, false);
            }
        }
        public override void UnInstall()
        {
            DeleteModule();
        }
    }
}
