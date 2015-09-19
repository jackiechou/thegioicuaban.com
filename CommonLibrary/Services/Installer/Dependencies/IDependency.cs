using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Dependencies
{
    public interface IDependency
    {
        string ErrorMessage { get; }
        bool IsValid { get; }
        void ReadManifest(XPathNavigator dependencyNav);
    }
}
