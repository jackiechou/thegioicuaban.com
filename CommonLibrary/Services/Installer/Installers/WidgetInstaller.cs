using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLibrary.Common;

namespace CommonLibrary.Services.Installer.Installers
{
    public class WidgetInstaller : FileInstaller
    {
        protected override string CollectionNodeName
        {
            get { return "widgetFiles"; }
        }
        protected override string ItemNodeName
        {
            get { return "widgetFile"; }
        }
        protected override string PhysicalBasePath
        {
            get
            {
                string widgetPath = Path.Combine("Resources\\Widgets\\User", BasePath);
                return Path.Combine(Globals.ApplicationMapPath, widgetPath);
            }
        }
        public override string AllowableFiles
        {
            get { return "js"; }
        }
    }
}
