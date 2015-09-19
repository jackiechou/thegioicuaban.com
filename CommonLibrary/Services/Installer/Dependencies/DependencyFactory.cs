using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using CommonLibrary.Common.Lists;
using CommonLibrary.Framework;

namespace CommonLibrary.Services.Installer.Dependencies
{
    public class DependencyFactory
    {
        public static IDependency GetDependency(XPathNavigator dependencyNav)
        {
            IDependency dependency = null;
            string dependencyType = Util.ReadAttribute(dependencyNav, "type");
            switch (dependencyType.ToLowerInvariant())
            {
                case "coreversion":
                    dependency = new CoreVersionDependency();
                    break;
                case "package":
                    dependency = new PackageDependency();
                    break;
                case "permission":
                    dependency = new PermissionsDependency();
                    break;
                case "type":
                    dependency = new TypeDependency();
                    break;
                default:
                    ListController listController = new ListController();
                    ListEntryInfo entry = listController.GetListEntryInfo("Dependency", dependencyType);
                    if (entry != null && !string.IsNullOrEmpty(entry.Text))
                    {
                        dependency = (DependencyBase)Reflection.CreateObject(entry.Text, "Dependency_" + entry.Value);
                    }
                    break;
            }
            if (dependency == null)
            {
                dependency = new InvalidDependency(Util.INSTALL_Dependencies);
            }
            dependency.ReadManifest(dependencyNav);
            return dependency;
        }
    }
}
