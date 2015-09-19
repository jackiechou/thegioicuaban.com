using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Installers
{
    public class LibraryPackageWriter : PackageWriterBase
    {
        public LibraryPackageWriter(PackageInfo package)
            : base(package)
        {
            BasePath = "DesktopModules\\Libraries";
            AssemblyPath = "bin";
        }
        protected override void GetFiles(bool includeSource, bool includeAppCode)
        {
            base.GetFiles(includeSource, false);
        }
    }
}
