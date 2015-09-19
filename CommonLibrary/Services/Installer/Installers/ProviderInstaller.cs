using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace CommonLibrary.Services.Installer.Installers
{
    public class ProviderInstaller : ComponentInstallerBase
    {
        public override string AllowableFiles
        {
            get { return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, xml, htc, html, htm, text, vbproj, csproj, sln"; }
        }
        public override void Commit()
        {
            Completed = true;
        }
        public override void Install()
        {
            Completed = true;
        }
        public override void ReadManifest(XPathNavigator manifestNav)
        {
        }
        public override void Rollback()
        {
            Completed = true;
        }
        public override void UnInstall()
        {
            Completed = true;
        }
    }
}
