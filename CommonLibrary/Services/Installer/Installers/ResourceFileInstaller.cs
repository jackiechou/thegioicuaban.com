using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Installer.Installers
{
    public class ResourceFileInstaller : FileInstaller
    {
        private string _Manifest;
        public const string DEFAULT_MANIFESTEXT = ".manifest";
        protected override string CollectionNodeName
        {
            get { return "resourceFiles"; }
        }
        protected override string ItemNodeName
        {
            get { return "resourceFile"; }
        }
        protected string Manifest
        {
            get { return _Manifest; }
        }
        public override string AllowableFiles
        {
            get { return "resources, zip"; }
        }
        protected override void CommitFile(InstallFile insFile)
        {
        }
        protected override void DeleteFile(InstallFile file)
        {
        }
        protected override bool InstallFile(InstallFile insFile)
        {
            FileStream fs = null;
            ZipInputStream unzip = null;
            XmlWriter writer = null;
            bool retValue = true;
            try
            {
                Log.AddInfo(Util.FILES_Expanding);
                unzip = new ZipInputStream(new FileStream(insFile.TempFileName, FileMode.Open));
                if (string.IsNullOrEmpty(Manifest))
                {
                    _Manifest = insFile.Name + ".manifest";
                }
                if (!Directory.Exists(PhysicalBasePath))
                {
                    Directory.CreateDirectory(PhysicalBasePath);
                }
                fs = new FileStream(Path.Combine(PhysicalBasePath, Manifest), FileMode.Create, FileAccess.Write);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                writer = XmlWriter.Create(fs, settings);
                writer.WriteStartElement("dotnetnuke");
                writer.WriteAttributeString("type", "ResourceFile");
                writer.WriteAttributeString("version", "5.0");
                writer.WriteStartElement("files");
                ZipEntry entry = unzip.GetNextEntry();
                while (entry != null)
                {
                    if (!entry.IsDirectory)
                    {
                        string fileName = Path.GetFileName(entry.Name);
                        writer.WriteStartElement("file");
                        writer.WriteElementString("path", entry.Name.Substring(0, entry.Name.IndexOf(fileName)));
                        writer.WriteElementString("name", fileName);
                        string physicalPath = Path.Combine(PhysicalBasePath, entry.Name);
                        if (File.Exists(physicalPath))
                        {
                            Util.BackupFile(new InstallFile(entry.Name, this.Package.InstallerInfo), PhysicalBasePath, Log);
                        }
                        Util.WriteStream(unzip, physicalPath);
                        writer.WriteEndElement();
                        Log.AddInfo(string.Format(Util.FILE_Created, entry.Name));
                    }
                    entry = unzip.GetNextEntry();
                }
                writer.WriteEndElement();
                Log.AddInfo(Util.FILES_CreatedResources);
            }
            catch (Exception ex)
            {
                ex.ToString();
                retValue = false;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
                if (fs != null)
                    fs.Close();
                if (unzip != null)
                    unzip.Close();
            }
            return retValue;
        }
        protected override bool IsCorrectType(InstallFileType type)
        {
            return (type == InstallFileType.Resources);
        }
        protected override InstallFile ReadManifestItem(System.Xml.XPath.XPathNavigator nav, bool checkFileExists)
        {
            InstallFile insFile = base.ReadManifestItem(nav, checkFileExists);
            _Manifest = Util.ReadElement(nav, "manifest");
            if (string.IsNullOrEmpty(_Manifest))
            {
                _Manifest = insFile.FullName + DEFAULT_MANIFESTEXT;
            }
            return base.ReadManifestItem(nav, checkFileExists);
        }
        protected override void RollbackFile(InstallFile insFile)
        {
            ZipInputStream unzip = new ZipInputStream(new FileStream(insFile.InstallerInfo.TempInstallFolder + insFile.FullName, FileMode.Open));
            ZipEntry entry = unzip.GetNextEntry();
            while (entry != null)
            {
                if (!entry.IsDirectory)
                {
                    if (File.Exists(insFile.BackupPath + entry.Name))
                    {
                        Util.RestoreFile(new InstallFile(unzip, entry, this.Package.InstallerInfo), PhysicalBasePath, Log);
                    }
                    else
                    {
                        Util.DeleteFile(entry.Name, PhysicalBasePath, Log);
                    }
                }
                entry = unzip.GetNextEntry();
            }
        }
        protected override void UnInstallFile(InstallFile unInstallFile)
        {
            _Manifest = unInstallFile.Name + ".manifest";
            XPathDocument doc = new XPathDocument(Path.Combine(PhysicalBasePath, Manifest));
            foreach (XPathNavigator fileNavigator in doc.CreateNavigator().Select("dotnetnuke/files/file"))
            {
                string path = XmlUtils.GetNodeValue(fileNavigator, "path");
                string fileName = XmlUtils.GetNodeValue(fileNavigator, "name");
                string filePath = System.IO.Path.Combine(path, fileName);
                try
                {
                    if (DeleteFiles)
                    {
                        Util.DeleteFile(filePath, PhysicalBasePath, Log);
                    }
                }
                catch (Exception ex)
                {
                    Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
                }
            }
            if (DeleteFiles)
            {
                Util.DeleteFile(Manifest, PhysicalBasePath, Log);
            }
        }
    }
}
