using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Framework;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Dependencies
{
    public class PermissionsDependency : DependencyBase
    {
        private string Permissions;
        private string Permission = Null.NullString;
        public override string ErrorMessage
        {
            get { return Util.INSTALL_Permissions + " - " + Permission; }
        }
        public override bool IsValid
        {
            get { return SecurityPolicy.HasPermissions(Permissions, ref Permission); }
        }
        public override void ReadManifest(XPathNavigator dependencyNav)
        {
            Permissions = dependencyNav.Value;
        }
    }
}
