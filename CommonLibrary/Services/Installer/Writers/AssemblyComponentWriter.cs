using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Writers
{
    public class AssemblyComponentWriter : FileComponentWriter
    {
        public AssemblyComponentWriter(string basePath, Dictionary<string, InstallFile> assemblies, PackageInfo package)
            : base(basePath, assemblies, package)
        {

        }
        protected override string CollectionNodeName
        {
            get { return "assemblies"; }
        }
        protected override string ComponentType
        {
            get { return "Assembly"; }
        }
        protected override string ItemNodeName
        {
            get { return "assembly"; }
        }
    }
}
