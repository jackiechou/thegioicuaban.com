using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Skins;
using CommonLibrary.Common;
using System.IO;
using CommonLibrary.Common.Utilities;
using System.Xml.XPath;
using System.Collections;
using CommonLibrary.Entities.Portal;

namespace CommonLibrary.Services.Installer.Installers
{
    public class SkinInstaller : FileInstaller
    {
        private ArrayList _SkinFiles = new ArrayList();
        private string _SkinName = Null.NullString;
        private SkinPackageInfo SkinPackage;
        private SkinPackageInfo TempSkinPackage;
        protected override string CollectionNodeName
        {
            get { return "skinFiles"; }
        }
        protected override string ItemNodeName
        {
            get { return "skinFile"; }
        }
        protected override string PhysicalBasePath
        {
            get
            {
                string _PhysicalBasePath = RootPath + SkinRoot + "\\" + SkinPackage.SkinName;
                if (!_PhysicalBasePath.EndsWith("\\"))
                {
                    _PhysicalBasePath += "\\";
                }
                return _PhysicalBasePath.Replace("/", "\\");
            }
        }
        protected string RootPath
        {
            get
            {
                string _RootPath = Null.NullString;
                if (Package.InstallerInfo.PortalID == Null.NullInteger)
                {
                    _RootPath = Common.Globals.HostMapPath;
                }
                else
                {
                    _RootPath = PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath;
                }
                return _RootPath;
            }
        }
        protected ArrayList SkinFiles
        {
            get { return _SkinFiles; }
        }
        protected virtual string SkinNameNodeName
        {
            get { return "skinName"; }
        }
        protected virtual string SkinRoot
        {
            get { return SkinController.RootSkin; }
        }
        protected virtual string SkinType
        {
            get { return "Skin"; }
        }
        public override string AllowableFiles
        {
            get { return "ascx, html, htm, css, xml, js, resx, jpg, jpeg, gif, png"; }
        }
        private void DeleteSkinPackage()
        {
            try
            {
                SkinPackageInfo skinPackage = SkinController.GetSkinByPackageID(Package.PackageID);
                if (skinPackage != null)
                {
                    SkinController.DeleteSkinPackage(skinPackage);
                }
                Log.AddInfo(string.Format(Util.SKIN_UnRegistered, skinPackage.SkinName));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        protected override void ProcessFile(InstallFile file, System.Xml.XPath.XPathNavigator nav)
        {
            switch (file.Extension)
            {
                case "htm":
                case "html":
                case "ascx":
                case "css":
                    if (file.Path.ToLower().IndexOf(Globals.glbAboutPage.ToLower()) < 0)
                    {
                        SkinFiles.Add(PhysicalBasePath + file.FullName);
                    }
                    break;
            }
            base.ProcessFile(file, nav);
        }
        protected override void ReadCustomManifest(XPathNavigator nav)
        {
            SkinPackage = new SkinPackageInfo();
            SkinPackage.PortalID = Package.PortalID;
            SkinPackage.SkinName = Util.ReadElement(nav, SkinNameNodeName);
            base.ReadCustomManifest(nav);
        }
        protected override void UnInstallFile(InstallFile unInstallFile)
        {
            base.UnInstallFile(unInstallFile);
            if (unInstallFile.Extension == "htm" || unInstallFile.Extension == "html")
            {
                string fileName = unInstallFile.FullName;
                fileName = fileName.Replace(Path.GetExtension(fileName), ".ascx");
                Util.DeleteFile(fileName, PhysicalBasePath, Log);
            }
        }
        public override void Install()
        {
            bool bAdd = Null.NullBoolean;
            try
            {
                TempSkinPackage = SkinController.GetSkinPackage(SkinPackage.PortalID, SkinPackage.SkinName, SkinType);
                if (TempSkinPackage == null)
                {
                    bAdd = true;
                    SkinPackage.PackageID = Package.PackageID;
                }
                else
                {
                    SkinPackage.SkinPackageID = TempSkinPackage.SkinPackageID;
                    if (TempSkinPackage.PackageID != Package.PackageID)
                    {
                        Completed = false;
                        Log.AddFailure(Util.SKIN_Installed);
                        return;
                    }
                    else
                    {
                        SkinPackage.PackageID = TempSkinPackage.PackageID;
                    }
                }
                SkinPackage.SkinType = SkinType;
                if (bAdd)
                {
                    SkinPackage.SkinPackageID = SkinController.AddSkinPackage(SkinPackage);
                }
                else
                {
                    SkinController.UpdateSkinPackage(SkinPackage);
                }
                Log.AddInfo(string.Format(Util.SKIN_Registered, SkinPackage.SkinName));
                base.Install();
                if (SkinFiles.Count > 0)
                {
                    Log.StartJob(Util.SKIN_BeginProcessing);
                    string strMessage = Null.NullString;
                    UI.Skins.SkinFileProcessor NewSkin = new UI.Skins.SkinFileProcessor(RootPath, SkinRoot, SkinPackage.SkinName);
                    foreach (string skinFile in SkinFiles)
                    {
                        strMessage += NewSkin.ProcessFile(skinFile, SkinParser.Portable);
                        skinFile.Replace(Globals.HostMapPath + "\\", "[G]");
                        switch (Path.GetExtension(skinFile))
                        {
                            case ".htm":
                                SkinController.AddSkin(SkinPackage.SkinPackageID, skinFile.Replace("htm", "ascx"));
                                break;
                            case ".html":
                                SkinController.AddSkin(SkinPackage.SkinPackageID, skinFile.Replace("html", "ascx"));
                                break;
                            case ".ascx":
                                SkinController.AddSkin(SkinPackage.SkinPackageID, skinFile);
                                break;
                        }
                    }
                    Array arrMessage = strMessage.Split(new string[] { "<br>" }, StringSplitOptions.None);
                    foreach (string strRow in arrMessage)
                    {
                        Log.AddInfo(HtmlUtils.StripTags(strRow, true));
                    }
                    Log.EndJob(Util.SKIN_EndProcessing);
                }
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void Rollback()
        {
            if (TempSkinPackage == null)
            {
                DeleteSkinPackage();
            }
            else
            {
                SkinController.UpdateSkinPackage(TempSkinPackage);
            }
            base.Rollback();
        }
        public override void UnInstall()
        {
            DeleteSkinPackage();
            base.UnInstall();
        }
    }
}
