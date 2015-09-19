using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLibrary.UI.Skins;
using CommonLibrary.Services.Installer.Packages;


namespace CommonLibrary.Services.Installer.Writers
{
    public class SkinPackageWriter : PackageWriterBase
    {
        private SkinPackageInfo _SkinPackage;
        private string _SubFolder;
        public SkinPackageWriter(PackageInfo package)
            : base(package)
        {
            _SkinPackage = SkinController.GetSkinByPackageID(package.PackageID);
            SetBasePath();
        }
        public SkinPackageWriter(SkinPackageInfo skinPackage, PackageInfo package)
            : base(package)
        {
            _SkinPackage = skinPackage;
            SetBasePath();
        }
        public SkinPackageWriter(SkinPackageInfo skinPackage, PackageInfo package, string basePath)
            : base(package)
        {
            _SkinPackage = skinPackage;
            this.BasePath = basePath;
        }
        public SkinPackageWriter(SkinPackageInfo skinPackage, PackageInfo package, string basePath, string subFolder)
            : base(package)
        {
            _SkinPackage = skinPackage;
            _SubFolder = subFolder;
            this.BasePath = Path.Combine(basePath, subFolder);
        }
        public override bool IncludeAssemblies
        {
            get { return false; }
        }
        protected SkinPackageInfo SkinPackage
        {
            get { return _SkinPackage; }
        }
        public void SetBasePath()
        {
            if (_SkinPackage.SkinType == "Skin")
            {
                BasePath = Path.Combine("Portals\\_default\\Skins", SkinPackage.SkinName);
            }
            else
            {
                BasePath = Path.Combine("Portals\\_default\\Containers", SkinPackage.SkinName);
            }
        }
        protected override void GetFiles(bool includeSource, bool includeAppCode)
        {
            base.GetFiles(includeSource, false);
        }
        protected override void ParseFiles(System.IO.DirectoryInfo folder, string rootPath)
        {
            FileInfo[] files = folder.GetFiles();
            foreach (FileInfo file in files)
            {
                string filePath = folder.FullName.Replace(rootPath, "");
                if (filePath.StartsWith("\\"))
                {
                    filePath = filePath.Substring(1);
                }
                if (file.Extension.ToLowerInvariant() != ".dnn")
                {
                    if (string.IsNullOrEmpty(_SubFolder))
                    {
                        AddFile(Path.Combine(filePath, file.Name));
                    }
                    else
                    {
                        filePath = Path.Combine(filePath, file.Name);
                        AddFile(filePath, Path.Combine(_SubFolder, filePath));
                    }
                }
            }
        }
        protected override void WriteFilesToManifest(System.Xml.XmlWriter writer)
        {
            SkinComponentWriter skinFileWriter = new SkinComponentWriter(SkinPackage.SkinName, BasePath, Files, Package);
            if (SkinPackage.SkinType == "Skin")
            {
                skinFileWriter = new SkinComponentWriter(SkinPackage.SkinName, BasePath, Files, Package);
            }
            else
            {
                skinFileWriter = new ContainerComponentWriter(SkinPackage.SkinName, BasePath, Files, Package);
            }
            skinFileWriter.WriteManifest(writer);
        }
    }
}
