using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Writers
{
    public class ContainerComponentWriter : SkinComponentWriter
    {
        public ContainerComponentWriter(string containerName, string basePath, Dictionary<string, InstallFile> files, PackageInfo package)
            : base(containerName, basePath, files, package)
        {

        }
        protected override string CollectionNodeName
        {
            get { return "containerFiles"; }
        }
        protected override string ComponentType
        {
            get { return "Container"; }
        }
        protected override string ItemNodeName
        {
            get { return "containerFile"; }
        }
        protected override string SkinNameNodeName
        {
            get { return "containerName"; }
        }
    }
}
