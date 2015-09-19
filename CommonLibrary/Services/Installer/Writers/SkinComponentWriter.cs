using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Writers
{
    public class SkinComponentWriter : FileComponentWriter
    {
        private string _SkinName;
        public SkinComponentWriter(string skinName, string basePath, Dictionary<string, InstallFile> files, PackageInfo package)
            : base(basePath, files, package)
        {
            _SkinName = skinName;
        }
        protected override string CollectionNodeName
        {
            get { return "skinFiles"; }
        }
        protected override string ComponentType
        {
            get { return "Skin"; }
        }
        protected override string ItemNodeName
        {
            get { return "skinFile"; }
        }
        protected virtual string SkinNameNodeName
        {
            get { return "skinName"; }
        }
        protected override void WriteCustomManifest(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString(SkinNameNodeName, _SkinName);
        }
    }
}
