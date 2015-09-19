using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommonLibrary.Common.Utilities;
using System.Xml.XPath;
using System.IO;
using CommonLibrary.Entities.Modules.Definitions;
using CommonLibrary.Security;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Services.Installer.Packages;
using System.ComponentModel;
using CommonLibrary.Common;

namespace CommonLibrary.Services.Installer.Writers
{
    public class ModulePackageWriter : PackageWriterBase
    {
        private DesktopModuleInfo _DesktopModule;
        public ModulePackageWriter(XPathNavigator manifestNav, InstallerInfo installer)
        {
            _DesktopModule = new DesktopModuleInfo();
            Package = new PackageInfo(installer);
            ReadLegacyManifest(manifestNav, true);
            Package.Name = DesktopModule.ModuleName;
            Package.FriendlyName = DesktopModule.FriendlyName;
            Package.Description = DesktopModule.Description;
            if (!string.IsNullOrEmpty(DesktopModule.Version))
            {
                Package.Version = new Version(DesktopModule.Version);
            }
            Package.PackageType = "Module";
            LegacyUtil.ParsePackageName(Package);
            Initialize(DesktopModule.FolderName);
        }
        public ModulePackageWriter(DesktopModuleInfo desktopModule, XPathNavigator manifestNav, PackageInfo package)
            : base(package)
        {

            _DesktopModule = desktopModule;
            Initialize(desktopModule.FolderName);
            if (manifestNav != null)
            {
                ReadLegacyManifest(manifestNav.SelectSingleNode("folders/folder"), false);
            }
            string physicalFolderPath = Path.Combine(Globals.ApplicationMapPath, BasePath);
            ProcessModuleFolders(physicalFolderPath, physicalFolderPath);
        }
        public ModulePackageWriter(PackageInfo package)
            : base(package)
        {
            _DesktopModule = DesktopModuleController.GetDesktopModuleByPackageID(package.PackageID);
            Initialize(DesktopModule.FolderName);
        }
        public ModulePackageWriter(DesktopModuleInfo desktopModule, PackageInfo package)
            : base(package)
        {
            _DesktopModule = desktopModule;
            Initialize(desktopModule.FolderName);
        }
        protected override Dictionary<string, string> Dependencies
        {
            get
            {
                Dictionary<string, string> _Dependencies = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(DesktopModule.Dependencies))
                {
                    _Dependencies["type"] = DesktopModule.Dependencies;
                }
                if (!string.IsNullOrEmpty(DesktopModule.Permissions))
                {
                    _Dependencies["permission"] = DesktopModule.Permissions;
                }
                return _Dependencies;
            }
        }
        public DesktopModuleInfo DesktopModule
        {
            get { return _DesktopModule; }
            set { _DesktopModule = value; }
        }
        private void Initialize(string folder)
        {
            BasePath = Path.Combine("DesktopModules", folder).Replace("/", "\\");
            AppCodePath = Path.Combine("App_Code", folder).Replace("/", "\\");
            AssemblyPath = "bin";
        }
        private static void ProcessControls(XPathNavigator controlNav, string moduleFolder, ModuleDefinitionInfo definition)
        {
            ModuleControlInfo moduleControl = new ModuleControlInfo();
            moduleControl.ControlKey = Util.ReadElement(controlNav, "key");
            moduleControl.ControlTitle = Util.ReadElement(controlNav, "title");
            string ControlSrc = Util.ReadElement(controlNav, "src");
            if (!(ControlSrc.ToLower().StartsWith("desktopmodules") || !ControlSrc.ToLower().EndsWith(".ascx")))
            {
                ControlSrc = Path.Combine("DesktopModules", Path.Combine(moduleFolder, ControlSrc));
            }
            ControlSrc = ControlSrc.Replace('\\', '/');
            moduleControl.ControlSrc = ControlSrc;
            moduleControl.IconFile = Util.ReadElement(controlNav, "iconfile");
            string controlType = Util.ReadElement(controlNav, "type");
            if (!string.IsNullOrEmpty(controlType))
            {
                try
                {
                    moduleControl.ControlType = (SecurityAccessLevel)TypeDescriptor.GetConverter(typeof(SecurityAccessLevel)).ConvertFromString(controlType);
                }
                catch (Exception ex)
                {
                    throw new Exception(Util.EXCEPTION_Type + ex.ToString());
                }
            }
            string viewOrder = Util.ReadElement(controlNav, "vieworder");
            if (!string.IsNullOrEmpty(viewOrder))
            {
                moduleControl.ViewOrder = int.Parse(viewOrder);
            }
            moduleControl.HelpURL = Util.ReadElement(controlNav, "helpurl");
            string supportsPartialRendering = Util.ReadElement(controlNav, "supportspartialrendering");
            if (!string.IsNullOrEmpty(supportsPartialRendering))
            {
                moduleControl.SupportsPartialRendering = bool.Parse(supportsPartialRendering);
            }
            definition.ModuleControls[moduleControl.ControlKey] = moduleControl;
        }
        private void ProcessModuleFiles(string folder, string basePath)
        {
            foreach (string fileName in Directory.GetFiles(folder))
            {
                string name = fileName.Replace(basePath + "\\", "");
                AddFile(name, name);
            }
        }
        private void ProcessModuleFolders(string folder, string basePath)
        {
            foreach (string directoryName in Directory.GetDirectories(folder))
            {
                ProcessModuleFolders(directoryName, basePath);
            }
            ProcessModuleFiles(folder, basePath);
        }
        private void ProcessModules(XPathNavigator moduleNav, string moduleFolder)
        {
            ModuleDefinitionInfo definition = new ModuleDefinitionInfo();
            definition.FriendlyName = Util.ReadElement(moduleNav, "friendlyname");
            string cacheTime = Util.ReadElement(moduleNav, "cachetime");
            if (!string.IsNullOrEmpty(cacheTime))
            {
                definition.DefaultCacheTime = int.Parse(cacheTime);
            }
            foreach (XPathNavigator controlNav in moduleNav.Select("controls/control"))
            {
                ProcessControls(controlNav, moduleFolder, definition);
            }
            DesktopModule.ModuleDefinitions[definition.FriendlyName] = definition;
        }
        private void ReadLegacyManifest(XPathNavigator folderNav, bool processModule)
        {
            if (processModule)
            {
                string name = Util.ReadElement(folderNav, "name");
                DesktopModule.FolderName = name;
                DesktopModule.ModuleName = name;
                DesktopModule.FriendlyName = name;
                string folderName = Util.ReadElement(folderNav, "foldername");
                if (!string.IsNullOrEmpty(folderName))
                {
                    DesktopModule.FolderName = folderName;
                }
                if (string.IsNullOrEmpty(DesktopModule.FolderName))
                {
                    DesktopModule.FolderName = "MyModule";
                }
                string friendlyname = Util.ReadElement(folderNav, "friendlyname");
                if (!string.IsNullOrEmpty(friendlyname))
                {
                    DesktopModule.FriendlyName = friendlyname;
                    DesktopModule.ModuleName = friendlyname;
                }
                string modulename = Util.ReadElement(folderNav, "modulename");
                if (!string.IsNullOrEmpty(modulename))
                {
                    DesktopModule.ModuleName = modulename;
                }
                string permissions = Util.ReadElement(folderNav, "permissions");
                if (!string.IsNullOrEmpty(permissions))
                {
                    DesktopModule.Permissions = permissions;
                }
                string dependencies = Util.ReadElement(folderNav, "dependencies");
                if (!string.IsNullOrEmpty(dependencies))
                {
                    DesktopModule.Dependencies = dependencies;
                }
                DesktopModule.Version = Util.ReadElement(folderNav, "version", "01.00.00");
                DesktopModule.Description = Util.ReadElement(folderNav, "description");
                DesktopModule.BusinessControllerClass = Util.ReadElement(folderNav, "businesscontrollerclass");
                foreach (XPathNavigator moduleNav in folderNav.Select("modules/module"))
                {
                    ProcessModules(moduleNav, DesktopModule.FolderName);
                }
            }
            foreach (XPathNavigator fileNav in folderNav.Select("files/file"))
            {
                string fileName = Util.ReadElement(fileNav, "name");
                string filePath = Util.ReadElement(fileNav, "path");
                string sourceFileName;
                if (filePath.Contains("[app_code]"))
                {
                    sourceFileName = Path.Combine(filePath, fileName).Replace("[app_code]", "App_Code\\" + DesktopModule.FolderName);
                }
                else
                {
                    sourceFileName = Path.Combine(filePath, fileName);
                }
                string tempFolder = Package.InstallerInfo.TempInstallFolder;
                if (!File.Exists(Path.Combine(tempFolder, sourceFileName)))
                {
                    sourceFileName = fileName;
                }
                if (fileName.ToLower().EndsWith(".dll"))
                {
                    AddFile("bin/" + fileName, sourceFileName);
                }
                else
                {
                    AddFile(Path.Combine(filePath, fileName), sourceFileName);
                }
            }
            if (!string.IsNullOrEmpty(Util.ReadElement(folderNav, "resourcefile")))
            {
                AddResourceFile(new InstallFile(Util.ReadElement(folderNav, "resourcefile"), Package.InstallerInfo));
            }
        }
        private void WriteEventMessage(XmlWriter writer)
        {
            writer.WriteStartElement("eventMessage");
            writer.WriteElementString("processorType", "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke");
            writer.WriteElementString("processorCommand", "UpgradeModule");
            writer.WriteStartElement("attributes");
            writer.WriteElementString("businessControllerClass", DesktopModule.BusinessControllerClass);
            writer.WriteElementString("desktopModuleID", "[DESKTOPMODULEID]");
            string upgradeVersions = Null.NullString;
            Versions.Sort();
            foreach (string version in Versions)
            {
                upgradeVersions += version + ",";
            }
            if (upgradeVersions.Length > 1)
            {
                upgradeVersions = upgradeVersions.Remove(upgradeVersions.Length - 1, 1);
            }
            writer.WriteElementString("upgradeVersionsList", upgradeVersions);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        private void WriteModuleComponent(XmlWriter writer)
        {
            writer.WriteStartElement("component");
            writer.WriteAttributeString("type", "Module");
            if (AppCodeFiles.Count > 0)
            {
                DesktopModule.CodeSubDirectory = DesktopModule.FolderName;
            }
            CBO.SerializeObject(DesktopModule, writer);
            if (!string.IsNullOrEmpty(DesktopModule.BusinessControllerClass))
            {
                WriteEventMessage(writer);
            }
            writer.WriteEndElement();
        }
        protected override void WriteManifestComponent(System.Xml.XmlWriter writer)
        {
            WriteModuleComponent(writer);
        }
    }
}
