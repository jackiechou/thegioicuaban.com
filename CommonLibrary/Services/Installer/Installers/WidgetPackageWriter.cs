using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Services.Installer.Packages;
using System.IO;

namespace CommonLibrary.Services.Installer.Installers
{
    public class WidgetPackageWriter : PackageWriterBase
    {
        public WidgetPackageWriter(PackageInfo package)
            : base(package)
        {
            string company = package.Name.Substring(0, package.Name.IndexOf("."));
            BasePath = Path.Combine("Resources\\Widgets\\User", company);
        }
        public override bool IncludeAssemblies
        {
            get { return false; }
        }
        protected override void GetFiles(bool includeSource, bool includeAppCode)
        {
            base.GetFiles(includeSource, false);
        }
        protected override void WriteFilesToManifest(System.Xml.XmlWriter writer)
        {
            string company = Package.Name.Substring(0, Package.Name.IndexOf("."));
            WidgetComponentWriter widgetFileWriter = new WidgetComponentWriter(company, Files, Package);
            widgetFileWriter.WriteManifest(writer);
        }
    }
}
