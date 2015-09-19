using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Application;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Dependencies
{
    public class CoreVersionDependency : DependencyBase
    {
        private System.Version minVersion;
        public override string ErrorMessage
        {
            get { return Util.INSTALL_Compatibility; }
        }
        public override bool IsValid
        {
            get
            {
                bool _IsValid = true;
                if (AppContext.Current.Application.Version < minVersion)
                {
                    _IsValid = false;
                }
                return _IsValid;
            }
        }
        public override void ReadManifest(XPathNavigator dependencyNav)
        {
            minVersion = new System.Version(dependencyNav.Value);
        }
    }
}
