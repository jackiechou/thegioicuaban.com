using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Writers
{
    public class ResourceFileComponentWriter : FileComponentWriter
    {
        public ResourceFileComponentWriter(string basePath, Dictionary<string, InstallFile> files, PackageInfo package)
            : base(basePath, files, package)
        {

        }
        protected override string CollectionNodeName
        {
            get { return "resourceFiles"; }
        }
        protected override string ComponentType
        {
            get { return "ResourceFile"; }
        }
        protected override string ItemNodeName
        {
            get { return "resourceFile"; }
        }
    }
}
