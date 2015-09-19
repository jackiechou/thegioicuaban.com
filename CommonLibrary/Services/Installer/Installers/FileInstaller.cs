using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using CommonLibrary.Common.Utilities;
using System.IO;

namespace CommonLibrary.Services.Installer.Installers
{
    public class FileInstaller : ComponentInstallerBase
    {
        private string _BasePath;
        private bool _DeleteFiles = Null.NullBoolean;
        private List<InstallFile> _Files = new List<InstallFile>();
        protected string BasePath
        {
            get { return _BasePath; }
        }
        protected virtual string CollectionNodeName
        {
            get { return "files"; }
        }
        protected List<InstallFile> Files
        {
            get { return _Files; }
        }
        protected virtual string DefaultPath
        {
            get { return Null.NullString; }
        }
        protected virtual string ItemNodeName
        {
            get { return "file"; }
        }
        protected virtual string PhysicalBasePath
        {
            get
            {
                string _PhysicalBasePath = PhysicalSitePath + "\\" + BasePath;
                if (!_PhysicalBasePath.EndsWith("\\"))
                {
                    _PhysicalBasePath += "\\";
                }
                return _PhysicalBasePath.Replace("/", "\\");
            }
        }
        public bool DeleteFiles
        {
            get { return _DeleteFiles; }
            set { _DeleteFiles = value; }
        }
        public override bool SupportsManifestOnlyInstall
        {
            get { return Null.NullBoolean; }
        }
        protected virtual void CommitFile(InstallFile insFile)
        {
        }
        protected virtual void DeleteFile(InstallFile insFile)
        {
            if (DeleteFiles)
            {
                Util.DeleteFile(insFile, PhysicalBasePath, Log);
            }
        }
        protected virtual bool InstallFile(InstallFile insFile)
        {
            try
            {
                if ((this.Package.InstallerInfo.IgnoreWhiteList || Util.IsFileValid(insFile, Package.InstallerInfo.AllowableFiles)))
                {
                    if (File.Exists(PhysicalBasePath + insFile.FullName))
                    {
                        Util.BackupFile(insFile, PhysicalBasePath, Log);
                    }
                    Util.CopyFile(insFile, PhysicalBasePath, Log);
                    return true;
                }
                else
                {
                    Log.AddFailure(string.Format(Util.FILE_NotAllowed, insFile.FullName));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
                return false;
            }
        }
        protected virtual bool IsCorrectType(InstallFileType type)
        {
            return true;
        }
        protected virtual void ProcessFile(InstallFile file, XPathNavigator nav)
        {
            if (file != null && IsCorrectType(file.Type))
            {
                Files.Add(file);
                this.Package.InstallerInfo.Files[file.FullName.ToLower()] = file;
            }
        }
        protected virtual void ReadCustomManifest(XPathNavigator nav)
        {
        }
        protected virtual InstallFile ReadManifestItem(XPathNavigator nav, bool checkFileExists)
        {
            string fileName = Null.NullString;
            XPathNavigator pathNav = nav.SelectSingleNode("path");
            if (pathNav == null)
            {
                fileName = DefaultPath;
            }
            else
            {
                fileName = pathNav.Value + "\\";
            }
            XPathNavigator nameNav = nav.SelectSingleNode("name");
            if (nameNav != null)
            {
                fileName += nameNav.Value;
            }
            string sourceFileName = Util.ReadElement(nav, "sourceFileName");
            InstallFile file = new InstallFile(fileName, sourceFileName, Package.InstallerInfo);
            if ((!string.IsNullOrEmpty(BasePath)) && (BasePath.ToLowerInvariant().StartsWith("app_code") && file.Type == InstallFileType.Other))
            {
                file.Type = InstallFileType.AppCode;
            }
            if (file != null)
            {
                string strVersion = XmlUtils.GetNodeValue(nav, "version");
                if (!string.IsNullOrEmpty(strVersion))
                {
                    file.SetVersion(new System.Version(strVersion));
                }
                else
                {
                    file.SetVersion(Package.Version);
                }
                string strAction = XmlUtils.GetAttributeValue(nav, "action");
                if (!string.IsNullOrEmpty(strAction))
                {
                    file.Action = strAction;
                }
                if (InstallMode == InstallMode.Install && checkFileExists && file.Action != "UnRegister")
                {
                    if (System.IO.File.Exists(file.TempFileName))
                    {
                        Log.AddInfo(string.Format(Util.FILE_Found, file.Path, file.Name));
                    }
                    else
                    {
                        Log.AddFailure(Util.FILE_NotFound + " - " + file.TempFileName);
                    }
                }
            }
            return file;
        }
        protected virtual void RollbackFile(InstallFile installFile)
        {
            if (File.Exists(installFile.BackupFileName))
            {
                Util.RestoreFile(installFile, PhysicalBasePath, Log);
            }
            else
            {
                DeleteFile(installFile);
            }
        }
        protected virtual void UnInstallFile(InstallFile unInstallFile)
        {
            DeleteFile(unInstallFile);
        }
        public override void Commit()
        {
            try
            {
                foreach (InstallFile file in Files)
                {
                    CommitFile(file);
                }
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }
        public override void Install()
        {
            try
            {
                bool bSuccess = true;
                foreach (InstallFile file in Files)
                {
                    bSuccess = InstallFile(file);
                    if (!bSuccess)
                    {
                        break;
                    }
                }
                Completed = bSuccess;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }
        public override void ReadManifest(XPathNavigator manifestNav)
        {
            XPathNavigator rootNav = manifestNav.SelectSingleNode(CollectionNodeName);
            if (rootNav != null)
            {
                XPathNavigator baseNav = rootNav.SelectSingleNode("basePath");
                if (baseNav != null)
                {
                    _BasePath = baseNav.Value;
                }
                ReadCustomManifest(rootNav);
                foreach (XPathNavigator nav in rootNav.Select(ItemNodeName))
                {
                    ProcessFile(ReadManifestItem(nav, true), nav);
                }
            }
        }
        public override void Rollback()
        {
            try
            {
                foreach (InstallFile file in Files)
                {
                    RollbackFile(file);
                }
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }
        public override void UnInstall()
        {
            try
            {
                foreach (InstallFile file in Files)
                {
                    UnInstallFile(file);
                }
                Completed = true;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }
    }
}
