using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Authentication;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Services.Installer.Packages;
using System.IO;
using System.Xml;

namespace CommonLibrary.Services.Installer.Installers
{
    public class AuthenticationPackageWriter : PackageWriterBase
    {
        private AuthenticationInfo _AuthSystem;
        public AuthenticationPackageWriter(PackageInfo package)
            : base(package)
        {
            _AuthSystem = AuthenticationController.GetAuthenticationServiceByPackageID(package.PackageID);
            Initialize();
        }
        public AuthenticationPackageWriter(AuthenticationInfo authSystem, PackageInfo package)
            : base(package)
        {
            _AuthSystem = authSystem;
            Initialize();
        }
        public AuthenticationInfo AuthSystem
        {
            get { return _AuthSystem; }
            set { _AuthSystem = value; }
        }
        private void Initialize()
        {
            BasePath = Path.Combine("DesktopModules\\AuthenticationServices", AuthSystem.AuthenticationType);
            AppCodePath = Path.Combine("App_Code\\AuthenticationServices", AuthSystem.AuthenticationType);
            AssemblyPath = "bin";
        }
        private void WriteAuthenticationComponent(XmlWriter writer)
        {
            writer.WriteStartElement("component");
            writer.WriteAttributeString("type", "AuthenticationSystem");
            writer.WriteStartElement("authenticationService");
            writer.WriteElementString("type", AuthSystem.AuthenticationType);
            writer.WriteElementString("settingsControlSrc", AuthSystem.SettingsControlSrc);
            writer.WriteElementString("loginControlSrc", AuthSystem.LoginControlSrc);
            writer.WriteElementString("logoffControlSrc", AuthSystem.LogoffControlSrc);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        protected override void WriteManifestComponent(System.Xml.XmlWriter writer)
        {
            WriteAuthenticationComponent(writer);
        }
    }
}
