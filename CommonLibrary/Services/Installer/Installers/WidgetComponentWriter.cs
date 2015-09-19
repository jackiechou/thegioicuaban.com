using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Installers
{
    public class WidgetComponentWriter : FileComponentWriter
    {
        public WidgetComponentWriter(string basePath, Dictionary<string, InstallFile> files, PackageInfo package)
            : base(basePath, files, package)
        {

        }
        protected override string CollectionNodeName
        {
            get { return "widgetFiles"; }
        }
        protected override string ItemNodeName
        {
            get { return "widgetFile"; }
        }
        protected override string ComponentType
        {
            get { return "Widget"; }
        }
    }
}
