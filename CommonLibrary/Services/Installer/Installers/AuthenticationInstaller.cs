using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Authentication;
using System.Xml.XPath;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Installer.Installers
{
    public class AuthenticationInstaller : ComponentInstallerBase
    {
        private AuthenticationInfo TempAuthSystem;
        private AuthenticationInfo AuthSystem;
        public override string AllowableFiles
        {
            get { return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html"; }
        }
        private void DeleteAuthentiation()
        {
            try
            {
                AuthenticationInfo authSystem = AuthenticationController.GetAuthenticationServiceByPackageID(Package.PackageID);
                if (authSystem != null)
                {
                    AuthenticationController.DeleteAuthentication(authSystem);
                }
                Log.AddInfo(string.Format(Util.AUTHENTICATION_UnRegistered, authSystem.AuthenticationType));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void Commit()
        {
        }
        public override void Install()
        {
            bool bAdd = Null.NullBoolean;
            try
            {
                TempAuthSystem = AuthenticationController.GetAuthenticationServiceByType(AuthSystem.AuthenticationType);
                if (TempAuthSystem == null)
                {
                    AuthSystem.IsEnabled = true;
                    bAdd = true;
                }
                else
                {
                    AuthSystem.AuthenticationID = TempAuthSystem.AuthenticationID;
                    AuthSystem.IsEnabled = TempAuthSystem.IsEnabled;
                }
                AuthSystem.PackageID = Package.PackageID;
                if (bAdd)
                {
                    AuthenticationController.AddAuthentication(AuthSystem);
                }
                else
                {
                    AuthenticationController.UpdateAuthentication(AuthSystem);
                }
                Completed = true;
                Log.AddInfo(string.Format(Util.AUTHENTICATION_Registered, AuthSystem.AuthenticationType));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void ReadManifest(XPathNavigator manifestNav)
        {
            AuthSystem = new AuthenticationInfo();
            AuthSystem.AuthenticationType = Util.ReadElement(manifestNav, "authenticationService/type", Log, Util.AUTHENTICATION_TypeMissing);
            AuthSystem.SettingsControlSrc = Util.ReadElement(manifestNav, "authenticationService/settingsControlSrc", Log, Util.AUTHENTICATION_SettingsSrcMissing);
            AuthSystem.LoginControlSrc = Util.ReadElement(manifestNav, "authenticationService/loginControlSrc", Log, Util.AUTHENTICATION_LoginSrcMissing);
            AuthSystem.LogoffControlSrc = Util.ReadElement(manifestNav, "authenticationService/logoffControlSrc");
            if (Log.Valid)
            {
                Log.AddInfo(Util.AUTHENTICATION_ReadSuccess);
            }
        }
        public override void Rollback()
        {
            if (TempAuthSystem == null)
            {
                DeleteAuthentiation();
            }
            else
            {
                AuthenticationController.UpdateAuthentication(TempAuthSystem);
            }
        }
        public override void UnInstall()
        {
            DeleteAuthentiation();
        }
    }
}
