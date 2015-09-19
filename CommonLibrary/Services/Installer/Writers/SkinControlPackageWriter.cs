using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.Xml.XPath;
using CommonLibrary.Entities.Modules;
using System.IO;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Writers
{
    public class SkinControlPackageWriter : PackageWriterBase
    {
        private SkinControlInfo _SkinControl;
        public SkinControlPackageWriter(PackageInfo package)
            : base(package)
        {
            _SkinControl = SkinControlController.GetSkinControlByPackageID(package.PackageID);
            BasePath = Path.Combine("DesktopModules", package.Name.ToLower()).Replace("/", "\\");
            AppCodePath = Path.Combine("App_Code", package.Name.ToLower()).Replace("/", "\\");
        }
        public SkinControlPackageWriter(SkinControlInfo skinControl, PackageInfo package)
            : base(package)
        {
            _SkinControl = skinControl;
            BasePath = Path.Combine("DesktopModules", package.Name.ToLower()).Replace("/", "\\");
            AppCodePath = Path.Combine("App_Code", package.Name.ToLower()).Replace("/", "\\");
        }
        public SkinControlPackageWriter(XPathNavigator manifestNav, InstallerInfo installer)
        {
            _SkinControl = new SkinControlInfo();
            Package = new PackageInfo(installer);
            ReadLegacyManifest(manifestNav, true);
            Package.Description = Null.NullString;
            Package.Version = new Version(1, 0, 0);
            Package.PackageType = "SkinObject";
            Package.License = Util.PACKAGE_NoLicense;
            BasePath = Path.Combine("DesktopModules", Package.Name.ToLower()).Replace("/", "\\");
            AppCodePath = Path.Combine("App_Code", Package.Name.ToLower()).Replace("/", "\\");
        }
        public SkinControlInfo SkinControl
        {
            get { return _SkinControl; }
            set { _SkinControl = value; }
        }
        private void ReadLegacyManifest(XPathNavigator legacyManifest, bool processModule)
        {
            XPathNavigator folderNav = legacyManifest.SelectSingleNode("folders/folder");
            if (processModule)
            {
                Package.Name = Util.ReadElement(folderNav, "name");
                Package.FriendlyName = Package.Name;
                foreach (XPathNavigator controlNav in folderNav.Select("modules/module/controls/control"))
                {
                    SkinControl.ControlKey = Util.ReadElement(controlNav, "key");
                    SkinControl.ControlSrc = Path.Combine(Path.Combine("DesktopModules", Package.Name.ToLower()), Util.ReadElement(controlNav, "src")).Replace("\\", "/");
                    string supportsPartialRendering = Util.ReadElement(controlNav, "supportspartialrendering");
                    if (!string.IsNullOrEmpty(supportsPartialRendering))
                    {
                        SkinControl.SupportsPartialRendering = bool.Parse(supportsPartialRendering);
                    }
                }
            }
            foreach (XPathNavigator fileNav in folderNav.Select("files/file"))
            {
                string fileName = Util.ReadElement(fileNav, "name");
                string filePath = Util.ReadElement(fileNav, "path");
                AddFile(Path.Combine(filePath, fileName), fileName);
            }
            if (!string.IsNullOrEmpty(Util.ReadElement(folderNav, "resourcefile")))
            {
                AddFile(Util.ReadElement(folderNav, "resourcefile"));
            }
        }
        protected override void WriteManifestComponent(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("component");
            writer.WriteAttributeString("type", "SkinObject");
            CBO.SerializeObject(SkinControl, writer);
            writer.WriteEndElement();
        }
    }
}
