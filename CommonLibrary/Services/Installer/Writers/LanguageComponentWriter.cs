using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Localization;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Writers
{
    public class LanguageComponentWriter : FileComponentWriter
    {
        private int _DependentPackageID;
        private Locale _Language;
        private LanguagePackType _PackageType;
        public LanguageComponentWriter(Locale language, string basePath, Dictionary<string, InstallFile> files, PackageInfo package)
            : base(basePath, files, package)
        {
            _Language = language;
            _PackageType = LanguagePackType.Core;
        }
        public LanguageComponentWriter(LanguagePackInfo languagePack, string basePath, Dictionary<string, InstallFile> files, PackageInfo package)
            : base(basePath, files, package)
        {
            _Language = Localization.Localization.GetLocaleByID(languagePack.LanguageID);
            _PackageType = languagePack.PackageType;
            _DependentPackageID = languagePack.DependentPackageID;
        }
        protected override string CollectionNodeName
        {
            get { return "languageFiles"; }
        }
        protected override string ComponentType
        {
            get
            {
                if (_PackageType == LanguagePackType.Core)
                {
                    return "CoreLanguage";
                }
                else
                {
                    return "ExtensionLanguage";
                }
            }
        }
        protected override string ItemNodeName
        {
            get { return "languageFile"; }
        }
        protected override void WriteCustomManifest(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("code", _Language.Code);
            if (_PackageType == LanguagePackType.Core)
            {
                writer.WriteElementString("displayName", _Language.Text);
                writer.WriteElementString("fallback", _Language.Fallback);
            }
            else
            {
                PackageInfo package = PackageController.GetPackage(_DependentPackageID);
                writer.WriteElementString("package", package.Name);
            }
        }
    }
}
