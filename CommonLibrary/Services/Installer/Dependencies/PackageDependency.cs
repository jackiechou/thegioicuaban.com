using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Packages;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Dependencies
{
    public class PackageDependency : DependencyBase
    {
        private string PackageName;
        public override string ErrorMessage
        {
            get { return Util.INSTALL_Package + " - " + PackageName; }
        }
        public override bool IsValid
        {
            get
            {
                bool _IsValid = true;
                PackageInfo package = PackageController.GetPackageByName(PackageName);
                if (package == null)
                {
                    _IsValid = false;
                }
                return _IsValid;
            }
        }
        public override void ReadManifest(XPathNavigator dependencyNav)
        {
            PackageName = dependencyNav.Value;
        }
    }
}
