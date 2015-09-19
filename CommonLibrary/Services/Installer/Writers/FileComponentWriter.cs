using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Installer.Log;
using CommonLibrary.Services.Installer.Packages;

namespace CommonLibrary.Services.Installer.Writers
{
    public class FileComponentWriter
    {
        private string _BasePath;
        private Dictionary<string, InstallFile> _Files;
        private int _InstallOrder = Null.NullInteger;
        private PackageInfo _Package;
        private int _UnInstallOrder = Null.NullInteger;
        public FileComponentWriter(string basePath, Dictionary<string, InstallFile> files, PackageInfo package)
        {
            _Files = files;
            _BasePath = basePath;
            _Package = package;
        }
        protected virtual string CollectionNodeName
        {
            get { return "files"; }
        }
        protected virtual string ComponentType
        {
            get { return "File"; }
        }
        protected virtual string ItemNodeName
        {
            get { return "file"; }
        }
        protected virtual Logger Log
        {
            get { return _Package.Log; }
        }
        protected virtual PackageInfo Package
        {
            get { return _Package; }
        }
        public int InstallOrder
        {
            get { return _InstallOrder; }
            set { _InstallOrder = value; }
        }
        public int UnInstallOrder
        {
            get { return _UnInstallOrder; }
            set { _UnInstallOrder = value; }
        }
        protected virtual void WriteCustomManifest(XmlWriter writer)
        {
        }
        protected virtual void WriteFileElement(XmlWriter writer, InstallFile file)
        {
            Log.AddInfo(string.Format(Util.WRITER_AddFileToManifest, file.Name));
            writer.WriteStartElement(ItemNodeName);
            if (!string.IsNullOrEmpty(file.Path))
            {
                string path = file.Path;
                if (!string.IsNullOrEmpty(_BasePath))
                {
                    if (file.Path.ToLowerInvariant().Contains(_BasePath.ToLowerInvariant()))
                    {
                        path = file.Path.ToLowerInvariant().Replace(_BasePath.ToLowerInvariant() + "\\", "");
                    }
                }
                writer.WriteElementString("path", path);
            }
            writer.WriteElementString("name", file.Name);
            if (!string.IsNullOrEmpty(file.SourceFileName))
            {
                writer.WriteElementString("sourceFileName", file.SourceFileName);
            }
            writer.WriteEndElement();
        }
        public virtual void WriteManifest(XmlWriter writer)
        {
            writer.WriteStartElement("component");
            writer.WriteAttributeString("type", ComponentType);
            if (InstallOrder > Null.NullInteger)
            {
                writer.WriteAttributeString("installOrder", InstallOrder.ToString());
            }
            if (UnInstallOrder > Null.NullInteger)
            {
                writer.WriteAttributeString("unInstallOrder", UnInstallOrder.ToString());
            }
            writer.WriteStartElement(CollectionNodeName);
            WriteCustomManifest(writer);
            if (!string.IsNullOrEmpty(_BasePath))
            {
                writer.WriteElementString("basePath", _BasePath);
            }
            foreach (InstallFile file in _Files.Values)
            {
                WriteFileElement(writer, file);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
