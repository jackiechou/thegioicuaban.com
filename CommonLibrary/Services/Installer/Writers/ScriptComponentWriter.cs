using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.IO;
using System.Xml;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Writers
{
    public class ScriptComponentWriter : FileComponentWriter
    {
        public ScriptComponentWriter(string basePath, Dictionary<string, InstallFile> scripts, PackageInfo package)
            : base(basePath, scripts, package)
        {
        }
        protected override string CollectionNodeName
        {
            get { return "scripts"; }
        }
        protected override string ComponentType
        {
            get { return "Script"; }
        }
        protected override string ItemNodeName
        {
            get { return "script"; }
        }
        protected override void WriteFileElement(XmlWriter writer, InstallFile file)
        {
            Log.AddInfo(string.Format(Util.WRITER_AddFileToManifest, file.Name));
            string type = "Install";
            string version = Null.NullString;
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            if (fileName.ToLower() == "uninstall")
            {
                type = "UnInstall";
                version = Package.Version.ToString(3);
            }
            else if (fileName.ToLower() == "install")
            {
                type = "Install";
                version = new Version(0, 0, 0).ToString(3);
            }
            else if (fileName.StartsWith("Install"))
            {
                type = "Install";
                version = fileName.Replace("Install.", "");
            }
            else
            {
                type = "Install";
                version = fileName;
            }
            writer.WriteStartElement(ItemNodeName);
            writer.WriteAttributeString("type", type);
            if (!string.IsNullOrEmpty(file.Path))
            {
                writer.WriteElementString("path", file.Path);
            }
            writer.WriteElementString("name", file.Name);
            if (!string.IsNullOrEmpty(file.SourceFileName))
            {
                writer.WriteElementString("sourceFileName", file.SourceFileName);
            }
            writer.WriteElementString("version", version);
            writer.WriteEndElement();
        }
    }
}
