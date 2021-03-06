﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Installer.Log;
using ICSharpCode.SharpZipLib.Zip;
using CommonLibrary.Common;
using System.IO;
using CommonLibrary.Common.Utilities;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using CommonLibrary.Services.Installer.Packages;


namespace CommonLibrary.Services.Installer.Writers
{
    public class PackageWriterBase
    {
        private Dictionary<string, InstallFile> _AppCodeFiles = new Dictionary<string, InstallFile>();
        private string _AppCodePath;
        private Dictionary<string, InstallFile> _Assemblies = new Dictionary<string, InstallFile>();
        private string _AssemblyPath;
        private string _BasePath = Null.NullString;
        private SortedList<string, InstallFile> _CleanUpFiles = new SortedList<string, InstallFile>();
        private Dictionary<string, InstallFile> _Files = new Dictionary<string, InstallFile>();
        private bool _HasProjectFile;
        private string _LegacyError;
        private PackageInfo _Package;
        private Dictionary<string, InstallFile> _Resources = new Dictionary<string, InstallFile>();
        private Dictionary<string, InstallFile> _Scripts = new Dictionary<string, InstallFile>();
        private List<string> _Versions = new List<string>();
        protected PackageWriterBase()
        {
        }
        public PackageWriterBase(PackageInfo package)
        {
            _Package = package;
            _Package.AttachInstallerInfo(new InstallerInfo());
        }
        protected virtual Dictionary<string, string> Dependencies
        {
            get { return new Dictionary<string, string>(); }
        }
        public Dictionary<string, InstallFile> AppCodeFiles
        {
            get { return _AppCodeFiles; }
        }
        public string AppCodePath
        {
            get { return _AppCodePath; }
            set { _AppCodePath = value; }
        }
        public Dictionary<string, InstallFile> Assemblies
        {
            get { return _Assemblies; }
        }
        public string AssemblyPath
        {
            get { return _AssemblyPath; }
            set { _AssemblyPath = value; }
        }
        public string BasePath
        {
            get { return _BasePath; }
            set { _BasePath = value; }
        }
        public SortedList<string, InstallFile> CleanUpFiles
        {
            get { return _CleanUpFiles; }
        }
        public Dictionary<string, InstallFile> Files
        {
            get { return _Files; }
        }
        public bool HasProjectFile
        {
            get { return _HasProjectFile; }
            set { _HasProjectFile = value; }
        }
        public virtual bool IncludeAssemblies
        {
            get { return true; }
        }
        public string LegacyError
        {
            get { return _LegacyError; }
            set { _LegacyError = value; }
        }
        public Logger Log
        {
            get { return Package.Log; }
        }
        public PackageInfo Package
        {
            get { return _Package; }
            set { _Package = value; }
        }
        public Dictionary<string, InstallFile> Resources
        {
            get { return _Resources; }
        }
        public Dictionary<string, InstallFile> Scripts
        {
            get { return _Scripts; }
        }
        public List<string> Versions
        {
            get { return _Versions; }
        }
        private void AddFilesToZip(ZipOutputStream stream, IDictionary<string, InstallFile> files, string basePath)
        {
            foreach (InstallFile packageFile in files.Values)
            {
                string filepath;
                if (string.IsNullOrEmpty(basePath))
                {
                    filepath = Path.Combine(Globals.ApplicationMapPath, packageFile.FullName);
                }
                else
                {
                    filepath = Path.Combine(Path.Combine(Globals.ApplicationMapPath, basePath), packageFile.FullName.Replace(basePath + "\\", ""));
                }
                if (File.Exists(filepath))
                {
                    string packageFilePath = packageFile.Path;
                    if (!string.IsNullOrEmpty(basePath))
                    {
                        packageFilePath = packageFilePath.Replace(basePath + "\\", "");
                    }
                    FileSystemUtils.AddToZip(ref stream, filepath, packageFile.Name, packageFilePath);
                    Log.AddInfo(string.Format(Util.WRITER_SavedFile, packageFile.FullName));
                }
            }
        }
        private void CreateZipFile(string zipFileName)
        {
            int CompressionLevel = 9;
            FileInfo zipFile = new FileInfo(zipFileName);
            string ZipFileShortName = zipFile.Name;
            FileStream strmZipFile = null;
            Log.StartJob(Util.WRITER_CreatingPackage);
            try
            {
                Log.AddInfo(string.Format(Util.WRITER_CreateArchive, ZipFileShortName));
                strmZipFile = File.Create(zipFileName);
                ZipOutputStream strmZipStream = null;
                try
                {
                    strmZipStream = new ZipOutputStream(strmZipFile);
                    strmZipStream.SetLevel(CompressionLevel);
                    AddFilesToZip(strmZipStream, _Assemblies, "");
                    AddFilesToZip(strmZipStream, _AppCodeFiles, AppCodePath);
                    AddFilesToZip(strmZipStream, _Files, BasePath);
                    AddFilesToZip(strmZipStream, _CleanUpFiles, BasePath);
                    AddFilesToZip(strmZipStream, _Resources, BasePath);
                    AddFilesToZip(strmZipStream, _Scripts, BasePath);
                }
                catch (Exception ex)
                {
                    Exceptions.Exceptions.LogException(ex);
                    Log.AddFailure(string.Format(Util.WRITER_SaveFileError, ex));
                }
                finally
                {
                    if (strmZipStream != null)
                    {
                        strmZipStream.Finish();
                        strmZipStream.Close();
                    }
                }
                Log.EndJob(Util.WRITER_CreatedPackage);
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
                Log.AddFailure(string.Format(Util.WRITER_SaveFileError, ex));
            }
            finally
            {
                if (strmZipFile != null)
                {
                    strmZipFile.Close();
                }
            }
        }
        private void WritePackageEndElement(XmlWriter writer)
        {
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        private void WritePackageStartElement(XmlWriter writer)
        {
            writer.WriteStartElement("package");
            writer.WriteAttributeString("name", Package.Name);
            writer.WriteAttributeString("type", Package.PackageType);
            writer.WriteAttributeString("version", Package.Version.ToString(3));
            writer.WriteElementString("friendlyName", Package.FriendlyName);
            writer.WriteElementString("description", Package.Description);
            writer.WriteStartElement("owner");
            writer.WriteElementString("name", Package.Owner);
            writer.WriteElementString("organization", Package.Organization);
            writer.WriteElementString("url", Package.Url);
            writer.WriteElementString("email", Package.Email);
            writer.WriteEndElement();
            writer.WriteElementString("license", Package.License);
            writer.WriteElementString("releaseNotes", Package.ReleaseNotes);
            if (Dependencies.Count > 0)
            {
                writer.WriteStartElement("dependencies");
                foreach (KeyValuePair<string, string> kvp in Dependencies)
                {
                    writer.WriteStartElement("dependency");
                    writer.WriteAttributeString("type", kvp.Key);
                    writer.WriteString(kvp.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteStartElement("components");
        }
        protected virtual void AddFile(string fileName)
        {
            AddFile(new InstallFile(fileName, Package.InstallerInfo));
        }
        protected virtual void AddFile(string fileName, string sourceFileName)
        {
            AddFile(new InstallFile(fileName, sourceFileName, Package.InstallerInfo));
        }
        protected virtual void ConvertLegacyManifest(XPathNavigator legacyManifest, XmlWriter writer)
        {
        }
        protected virtual void GetFiles(bool includeSource, bool includeAppCode)
        {
            string baseFolder = Path.Combine(Globals.ApplicationMapPath, BasePath);
            if (Directory.Exists(baseFolder))
            {
                DirectoryInfo folderInfo = new DirectoryInfo(baseFolder);
                FileInfo[] files = folderInfo.GetFiles("*.??proj");
                if (files.Length == 0)
                {
                    ParseFolder(baseFolder, baseFolder);
                    if (includeAppCode)
                    {
                        string appCodeFolder = Path.Combine(Globals.ApplicationMapPath, AppCodePath);
                        ParseFolder(appCodeFolder, appCodeFolder);
                    }
                }
                else
                {
                    HasProjectFile = true;
                    foreach (FileInfo projFile in files)
                    {
                        ParseProjectFile(projFile, includeSource);
                    }
                }
            }
        }
        protected virtual void ParseFiles(DirectoryInfo folder, string rootPath)
        {
            FileInfo[] files = folder.GetFiles();
            foreach (FileInfo file in files)
            {
                string filePath = folder.FullName.Replace(rootPath, "");
                if (filePath.StartsWith("\\"))
                {
                    filePath = filePath.Substring(1);
                }
                if (folder.FullName.ToLowerInvariant().Contains("app_code"))
                {
                    filePath = "[app_code]" + filePath;
                }
                if (file.Extension.ToLowerInvariant() != ".dnn" && (file.Attributes & FileAttributes.Hidden) == 0)
                {
                    AddFile(Path.Combine(filePath, file.Name));
                }
            }
        }
        protected virtual void ParseFolder(string folderName, string rootPath)
        {
            if (Directory.Exists(folderName))
            {
                DirectoryInfo folder = new DirectoryInfo(folderName);
                DirectoryInfo[] subFolders = folder.GetDirectories();
                foreach (DirectoryInfo subFolder in subFolders)
                {
                    if ((subFolder.Attributes & FileAttributes.Hidden) == 0)
                    {
                        ParseFolder(subFolder.FullName, rootPath);
                    }
                }
                ParseFiles(folder, rootPath);
            }
        }
        protected void ParseProjectFile(FileInfo projFile, bool includeSource)
        {
            string fileName = "";
            XPathDocument doc = new XPathDocument(new FileStream(projFile.FullName, FileMode.Open, FileAccess.Read));
            XPathNavigator rootNav = doc.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(rootNav.NameTable);
            manager.AddNamespace("proj", "http://schemas.microsoft.com/developer/msbuild/2003");
            rootNav.MoveToFirstChild();
            XPathNavigator assemblyNav = rootNav.SelectSingleNode("proj:PropertyGroup/proj:AssemblyName", manager);
            fileName = assemblyNav.Value;
            XPathNavigator buildPathNav = rootNav.SelectSingleNode("proj:PropertyGroup/proj:OutputPath", manager);
            string buildPath = buildPathNav.Value.Replace("..\\", "");
            buildPath = buildPath.Replace(AssemblyPath + "\\", "");
            AddFile(Path.Combine(buildPath, fileName + ".dll"));
            foreach (XPathNavigator itemNav in rootNav.Select("proj:ItemGroup/proj:Reference", manager))
            {
                fileName = Util.ReadAttribute(itemNav, "Include");
                if (fileName.IndexOf(",") > -1)
                {
                    fileName = fileName.Substring(0, fileName.IndexOf(","));
                }
                if (!(fileName.ToLowerInvariant().StartsWith("system") || fileName.ToLowerInvariant().StartsWith("microsoft") || fileName.ToLowerInvariant() == "dotnetnuke" || fileName.ToLowerInvariant() == "dotnetnuke.webutility" || fileName.ToLowerInvariant() == "dotnetnuke.webcontrols"))
                {
                    AddFile(fileName + ".dll");
                }
            }
            foreach (XPathNavigator itemNav in rootNav.Select("proj:ItemGroup/proj:None", manager))
            {
                fileName = Util.ReadAttribute(itemNav, "Include");
                AddFile(fileName);
            }
            foreach (XPathNavigator itemNav in rootNav.Select("proj:ItemGroup/proj:Content", manager))
            {
                fileName = Util.ReadAttribute(itemNav, "Include");
                AddFile(fileName);
            }
            if (includeSource)
            {
                foreach (XPathNavigator itemNav in rootNav.Select("proj:ItemGroup/proj:Compile", manager))
                {
                    fileName = Util.ReadAttribute(itemNav, "Include");
                    AddFile(fileName);
                }
            }
        }
        protected virtual void WriteFilesToManifest(XmlWriter writer)
        {
            FileComponentWriter fileWriter = new FileComponentWriter(BasePath, Files, Package);
            fileWriter.WriteManifest(writer);
        }
        protected virtual void WriteManifestComponent(XmlWriter writer)
        {
        }
        public virtual void AddFile(InstallFile file)
        {
            switch (file.Type)
            {
                case InstallFileType.AppCode:
                    _AppCodeFiles[file.FullName.ToLower()] = file;
                    break;
                case InstallFileType.Assembly:
                    _Assemblies[file.FullName.ToLower()] = file;
                    break;
                case InstallFileType.CleanUp:
                    _CleanUpFiles[file.FullName.ToLower()] = file;
                    break;
                case InstallFileType.Script:
                    _Scripts[file.FullName.ToLower()] = file;
                    break;
                default:
                    _Files[file.FullName.ToLower()] = file;
                    break;
            }
            if ((file.Type == InstallFileType.CleanUp || file.Type == InstallFileType.Script) && Regex.IsMatch(file.Name, Util.REGEX_Version))
            {
                string version = Path.GetFileNameWithoutExtension(file.Name);
                if (!_Versions.Contains(version))
                {
                    _Versions.Add(version);
                }
            }
        }
        public void AddResourceFile(InstallFile file)
        {
            _Resources[file.FullName.ToLower()] = file;
        }
        public void CreatePackage(string archiveName, string manifestName, string manifest, bool createManifest)
        {
            if (createManifest)
            {
                WriteManifest(manifestName, manifest);
            }
            AddFile(manifestName);
            CreateZipFile(archiveName);
        }
        public void GetFiles(bool includeSource)
        {
            GetFiles(includeSource, true);
        }
        public void WriteManifest(string manifestName, string manifest)
        {
            XmlWriter writer = XmlWriter.Create(Path.Combine(Globals.ApplicationMapPath, Path.Combine(BasePath, manifestName)), XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment));
            Log.StartJob(Util.WRITER_CreatingManifest);
            WriteManifest(writer, manifest);
            Log.EndJob(Util.WRITER_CreatedManifest);
        }
        public void WriteManifest(XmlWriter writer, string manifest)
        {
            WriteManifestStartElement(writer);
            writer.WriteRaw(manifest);
            WriteManifestEndElement(writer);
            writer.Close();
        }
        public string WriteManifest(bool packageFragment)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment));
            WriteManifest(writer, packageFragment);
            writer.Close();
            return sb.ToString();
        }
        public void WriteManifest(XmlWriter writer, bool packageFragment)
        {
            Log.StartJob(Util.WRITER_CreatingManifest);
            if (!packageFragment)
            {
                WriteManifestStartElement(writer);
            }
            WritePackageStartElement(writer);
            if (Scripts.Count > 0)
            {
                ScriptComponentWriter scriptWriter = new ScriptComponentWriter(BasePath, Scripts, Package);
                scriptWriter.WriteManifest(writer);
            }
            if (CleanUpFiles.Count > 0)
            {
                CleanupComponentWriter cleanupFileWriter = new CleanupComponentWriter(BasePath, CleanUpFiles);
                cleanupFileWriter.WriteManifest(writer);
            }
            WriteManifestComponent(writer);
            if (Assemblies.Count > 0)
            {
                AssemblyComponentWriter assemblyWriter = new AssemblyComponentWriter(AssemblyPath, Assemblies, Package);
                assemblyWriter.WriteManifest(writer);
            }
            if (AppCodeFiles.Count > 0)
            {
                FileComponentWriter fileWriter = new FileComponentWriter(AppCodePath, AppCodeFiles, Package);
                fileWriter.WriteManifest(writer);
            }
            if (Files.Count > 0)
            {
                WriteFilesToManifest(writer);
            }
            if (Resources.Count > 0)
            {
                ResourceFileComponentWriter fileWriter = new ResourceFileComponentWriter(BasePath, Resources, Package);
                fileWriter.WriteManifest(writer);
            }
            WritePackageEndElement(writer);
            if (!packageFragment)
            {
                WriteManifestEndElement(writer);
            }
            Log.EndJob(Util.WRITER_CreatedManifest);
        }
        public static void WriteManifestEndElement(XmlWriter writer)
        {
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        public static void WriteManifestStartElement(XmlWriter writer)
        {
            writer.WriteStartElement("dotnetnuke");
            writer.WriteAttributeString("type", "Package");
            writer.WriteAttributeString("version", "5.0");
            writer.WriteStartElement("packages");
        }
    }
}
