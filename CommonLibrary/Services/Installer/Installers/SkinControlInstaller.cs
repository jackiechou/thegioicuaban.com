using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Common.Utilities;
using System.Xml.XPath;
using System.IO;

namespace CommonLibrary.Services.Installer.Installers
{
    public class SkinControlInstaller : ComponentInstallerBase
    {
        private SkinControlInfo InstalledSkinControl;
        private SkinControlInfo SkinControl;
        public override string AllowableFiles
        {
            get { return "ascx, vb, cs, js, resx, xml, vbproj, csproj, sln"; }
        }
        private void DeleteSkinControl()
        {
            try
            {
                SkinControlInfo skinControl = SkinControlController.GetSkinControlByPackageID(Package.PackageID);
                if (skinControl != null)
                {
                    SkinControlController.DeleteSkinControl(skinControl);
                }
                Log.AddInfo(string.Format(Util.MODULE_UnRegistered, skinControl.ControlKey));
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
            try
            {
                InstalledSkinControl = SkinControlController.GetSkinControlByKey(SkinControl.ControlKey);
                if (InstalledSkinControl != null)
                {
                    SkinControl.SkinControlID = InstalledSkinControl.SkinControlID;
                }
                SkinControl.PackageID = Package.PackageID;
                SkinControl.SkinControlID = SkinControlController.SaveSkinControl(SkinControl);
                Completed = true;
                Log.AddInfo(string.Format(Util.MODULE_Registered, SkinControl.ControlKey));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void ReadManifest(XPathNavigator manifestNav)
        {
            SkinControl = CBO.DeserializeObject<SkinControlInfo>(new StringReader(manifestNav.InnerXml));
            if (Log.Valid)
            {
                Log.AddInfo(Util.MODULE_ReadSuccess);
            }
        }
        public override void Rollback()
        {
            if (InstalledSkinControl == null)
            {
                DeleteSkinControl();
            }
            else
            {
                SkinControlController.SaveSkinControl(InstalledSkinControl);
            }
        }
        public override void UnInstall()
        {
            DeleteSkinControl();
        }
    }
}
