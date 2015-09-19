using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Localization;
using System.Xml.XPath;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Installers
{
    public class LanguageInstaller : FileInstaller
    {
        private Locale Language;
        private Locale TempLanguage;
        private LanguagePackInfo InstalledLanguagePack;
        private LanguagePackInfo LanguagePack;
        private LanguagePackType LanguagePackType;
        public LanguageInstaller(LanguagePackType type)
        {
            LanguagePackType = type;
        }
        protected override string CollectionNodeName
        {
            get { return "languageFiles"; }
        }
        protected override string ItemNodeName
        {
            get { return "languageFile"; }
        }
        public override string AllowableFiles
        {
            get { return "resx, xml"; }
        }
        private void DeleteLanguage()
        {
            try
            {
                LanguagePackInfo tempLanguagePack = LanguagePackController.GetLanguagePackByPackage(Package.PackageID);
                Locale language = Localization.Localization.GetLocaleByID(tempLanguagePack.LanguageID);
                if (tempLanguagePack != null)
                {
                    LanguagePackController.DeleteLanguagePack(tempLanguagePack);
                }
                if (language != null && tempLanguagePack.PackageType == Localization.LanguagePackType.Core)
                {
                    Localization.Localization.DeleteLanguage(language);
                }
                Log.AddInfo(string.Format(Util.LANGUAGE_UnRegistered, language.Text));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        protected override void ProcessFile(InstallFile file, System.Xml.XPath.XPathNavigator nav)
        {
            base.ProcessFile(file, nav);
        }
        protected override void ReadCustomManifest(XPathNavigator nav)
        {
            Language = new Locale();
            LanguagePack = new LanguagePackInfo();
            Language.Code = Util.ReadElement(nav, "code");
            Language.Text = Util.ReadElement(nav, "displayName");
            Language.Fallback = Util.ReadElement(nav, "fallback");
            if (LanguagePackType == Localization.LanguagePackType.Core)
            {
                LanguagePack.DependentPackageID = -2;
            }
            else
            {
                string packageName = Util.ReadElement(nav, "package");
                PackageInfo package = PackageController.GetPackageByName(packageName);
                LanguagePack.DependentPackageID = package.PackageID;
            }
            base.ReadCustomManifest(nav);
        }
        public override void Commit()
        {
        }
        public override void Install()
        {
            try
            {
                InstalledLanguagePack = LanguagePackController.GetLanguagePackByPackage(Package.PackageID);
                if (InstalledLanguagePack != null)
                {
                    LanguagePack.LanguagePackID = InstalledLanguagePack.LanguagePackID;
                }
                TempLanguage = Localization.Localization.GetLocale(Language.Code);
                if (TempLanguage != null)
                {
                    Language.LanguageID = TempLanguage.LanguageID;
                }
                if (LanguagePack.PackageType == Localization.LanguagePackType.Core)
                {
                    Localization.Localization.SaveLanguage(Language);
                }
                LanguagePack.PackageID = Package.PackageID;
                LanguagePack.LanguageID = Language.LanguageID;
                LanguagePackController.SaveLanguagePack(LanguagePack);
                Log.AddInfo(string.Format(Util.LANGUAGE_Registered, Language.Text));
                base.Install();
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
        public override void Rollback()
        {
            if (TempLanguage == null)
            {
                DeleteLanguage();
            }
            else
            {
                Localization.Localization.SaveLanguage(TempLanguage);
            }
            base.Rollback();
        }
        public override void UnInstall()
        {
            DeleteLanguage();
            base.UnInstall();
        }
    }
}
