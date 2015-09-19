using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Services.Installer.Packages;
using CommonLibrary.UI.Skins;

namespace CommonLibrary.Services.Installer.Installers
{
    public class ContainerPackageWriter : SkinPackageWriter
    {
        public ContainerPackageWriter(PackageInfo package)
            : base(package)
        {
            BasePath = "Portals\\_default\\Containers\\" + SkinPackage.SkinName;
        }
        public ContainerPackageWriter(SkinPackageInfo skinPackage, PackageInfo package)
            : base(skinPackage, package)
        {

            BasePath = "Portals\\_default\\Containers\\" + skinPackage.SkinName;
        }
        protected override void WriteFilesToManifest(System.Xml.XmlWriter writer)
        {
            ContainerComponentWriter containerFileWriter = new ContainerComponentWriter(SkinPackage.SkinName, BasePath, Files, Package);
            containerFileWriter.WriteManifest(writer);
        }
    }
}
