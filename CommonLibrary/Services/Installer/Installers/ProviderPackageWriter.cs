using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Services.Installer.Packages;
using CommonLibrary.Common.Utilities;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Installers
{
    public class ProviderPackageWriter : PackageWriterBase
    {
        public ProviderPackageWriter(PackageInfo package)
            : base(package)
        {
            XmlDocument configDoc = Config.Load();
            XPathNavigator providerNavigator = configDoc.CreateNavigator().SelectSingleNode("/configuration/dotnetnuke/*/providers/add[@name='" + package.Name + "']");
            string providerPath = Null.NullString;
            if (providerNavigator != null)
            {
                providerPath = Util.ReadAttribute(providerNavigator, "providerPath");
            }
            if (!string.IsNullOrEmpty(providerPath))
            {
                BasePath = providerPath.Replace("~/", "").Replace("/", "\\");
            }
        }
        protected override void GetFiles(bool includeSource, bool includeAppCode)
        {
            base.GetFiles(includeSource, false);
        }
    }
}
