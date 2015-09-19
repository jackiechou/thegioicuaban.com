using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.IO;

namespace CommonLibrary.Services.Installer.Installers
{
    public class CleanupInstaller : FileInstaller
    {
        private string _FileName;
        public override string AllowableFiles
        {
            get { return "*"; }
        }
        private bool ProcessCleanupFile()
        {
            Log.AddInfo(string.Format(Util.CLEANUP_Processing, Version.ToString(3)));
            bool bSuccess = true;
            try
            {
                string strListFile = Path.Combine(this.Package.InstallerInfo.TempInstallFolder, _FileName);
                if (File.Exists(strListFile))
                {
                    FileSystemUtils.DeleteFiles(System.Text.RegularExpressions.Regex.Split(FileSystemUtils.ReadFile(strListFile), Environment.NewLine));
                }
                Log.AddInfo(string.Format(Util.CLEANUP_ProcessComplete, Version.ToString(3)));
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
                bSuccess = false;
            }
            return bSuccess;
        }
        protected bool CleanupFile(InstallFile insFile)
        {
            try
            {
                if (File.Exists(PhysicalBasePath + insFile.FullName))
                {
                    Util.BackupFile(insFile, PhysicalBasePath, Log);
                }
                Util.DeleteFile(insFile, PhysicalBasePath, Log);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }
        protected override void ProcessFile(InstallFile file, System.Xml.XPath.XPathNavigator nav)
        {
            if (file != null)
            {
                Files.Add(file);
            }
        }
        protected override InstallFile ReadManifestItem(System.Xml.XPath.XPathNavigator nav, bool checkFileExists)
        {
            return base.ReadManifestItem(nav, false);
        }
        protected override void RollbackFile(InstallFile installFile)
        {
            if (File.Exists(installFile.BackupFileName))
            {
                Util.RestoreFile(installFile, PhysicalBasePath, Log);
            }
        }
        public override void Commit()
        {
            base.Commit();
        }
        public override void Install()
        {
            try
            {
                bool bSuccess = true;
                if (string.IsNullOrEmpty(_FileName))
                {
                    foreach (InstallFile file in Files)
                    {
                        bSuccess = CleanupFile(file);
                        if (!bSuccess)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    bSuccess = ProcessCleanupFile();
                }
                Completed = bSuccess;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }
        public override void ReadManifest(System.Xml.XPath.XPathNavigator manifestNav)
        {
            _FileName = Util.ReadAttribute(manifestNav, "fileName");
            base.ReadManifest(manifestNav);
        }
        public override void UnInstall()
        {
        }
    }
}
