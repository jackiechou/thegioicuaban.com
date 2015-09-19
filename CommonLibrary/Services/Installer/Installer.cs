using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLibrary.Services.Installer.Packages;
using System.Xml.XPath;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Installer.Installers;
using System.Xml;
using CommonLibrary.Services.Installer.Writers;
using CommonLibrary.Common;
using CommonLibrary.Services.Installer.Log;

namespace CommonLibrary.Services.Installer
{
    public class Installer
    {
        private InstallerInfo _InstallerInfo;
        private string _LegacyError;
        private SortedList<int, PackageInstaller> _Packages = new SortedList<int, PackageInstaller>();
        public Installer(string tempFolder, string manifest, string physicalSitePath, bool loadManifest)
        {
            _InstallerInfo = new InstallerInfo(tempFolder, manifest, physicalSitePath);
            _InstallerInfo.IgnoreWhiteList = false;
            if (loadManifest)
            {
                ReadManifest(true);
            }
        }
        public Installer(Stream inputStream, string physicalSitePath, bool loadManifest)
            : this(inputStream, physicalSitePath, loadManifest, true)
        {
        }
        public Installer(Stream inputStream, string physicalSitePath, bool loadManifest, bool deleteTemp)
        {
            _InstallerInfo = new InstallerInfo(inputStream, physicalSitePath);
            _InstallerInfo.IgnoreWhiteList = true;
            if (loadManifest)
            {
                ReadManifest(deleteTemp);
            }
        }
        //public Installer(PackageInfo package, string physicalSitePath)
        //{
        //    _InstallerInfo = new InstallerInfo(package, physicalSitePath);
        //    Packages.Add(Packages.Count, new PackageInstaller(package));
        //}
        public Installer(string manifest, string physicalSitePath, bool loadManifest)
        {
            _InstallerInfo = new InstallerInfo(physicalSitePath, InstallMode.ManifestOnly);
            if (loadManifest)
            {
                ReadManifest(new FileStream(manifest, FileMode.Open, FileAccess.Read));
            }
        }
        public InstallerInfo InstallerInfo
        {
            get { return _InstallerInfo; }
        }
        public bool IsValid
        {
            get { return InstallerInfo.IsValid; }
        }
        public string LegacyError
        {
            get { return _LegacyError; }
            set { _LegacyError = value; }
        }
        public SortedList<int, PackageInstaller> Packages
        {
            get { return _Packages; }
        }
        public string TempInstallFolder
        {
            get { return InstallerInfo.TempInstallFolder; }
        }
        private void InstallPackages()
        {
            for (int index = 0; index <= Packages.Count - 1; index++)
            {
                PackageInstaller installer = Packages.Values[index];
                if (installer.Package.IsValid)
                {
                    InstallerInfo.Log.AddInfo(Util.INSTALL_Start + " - " + installer.Package.Name);
                    installer.Install();
                    if (InstallerInfo.Log.Valid)
                    {
                        InstallerInfo.Log.AddInfo(Util.INSTALL_Success + " - " + installer.Package.Name);
                    }
                    else
                    {
                        InstallerInfo.Log.AddInfo(Util.INSTALL_Failed + " - " + installer.Package.Name);
                    }
                }
                else
                {
                    InstallerInfo.Log.AddFailure(Util.INSTALL_Aborted + " - " + installer.Package.Name);
                }
            }
        }
        private void LogInstallEvent(string package, string eventType)
        {
            try
            {
                Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo(eventType + " " + package + ":", InstallerInfo.ManifestFile.Name.Replace(".dnn", "")));
                foreach (LogEntry objLogEntry in InstallerInfo.Log.Logs)
                {
                    objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("Info:", objLogEntry.Description));
                }
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog(objEventLogInfo);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        private void ProcessPackages(XPathNavigator rootNav)
        {
            foreach (XPathNavigator nav in rootNav.Select("packages/package"))
            {
                int order = Packages.Count;
                string name = Util.ReadAttribute(nav, "name");
                string installOrder = Util.ReadAttribute(nav, "installOrder");
                if (!string.IsNullOrEmpty(installOrder))
                {
                    order = int.Parse(installOrder);
                }
                Packages.Add(order, new PackageInstaller(nav.OuterXml, InstallerInfo));
            }
        }
        private void ReadManifest(Stream stream)
        {
            XPathDocument doc = new XPathDocument(stream);
            XPathNavigator rootNav = doc.CreateNavigator();
            XPathDocument legacyDoc = null;
            rootNav.MoveToFirstChild();
            string packageType = Null.NullString;
            string legacyManifest = Null.NullString;
            if (rootNav.Name == "CommonLibrary")
            {
                packageType = Util.ReadAttribute(rootNav, "type");
            }
            else if (rootNav.Name.ToLower() == "languagepack")
            {
                packageType = "LanguagePack";
            }
            else
            {
                InstallerInfo.Log.AddFailure(Util.PACKAGE_UnRecognizable);
            }
            switch (packageType.ToLower())
            {
                case "package":
                    InstallerInfo.IsLegacyMode = false;
                    ProcessPackages(rootNav);
                    break;
                case "module":
                    InstallerInfo.IsLegacyMode = true;
                    StringBuilder sb = new StringBuilder();
                    XmlWriter writer = XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment));
                    PackageWriterBase.WriteManifestStartElement(writer);
                    foreach (XPathNavigator folderNav in rootNav.Select("folders/folder"))
                    {
                        ModulePackageWriter modulewriter = new ModulePackageWriter(folderNav, InstallerInfo);
                        modulewriter.WriteManifest(writer, true);
                    }

                    PackageWriterBase.WriteManifestEndElement(writer);
                    writer.Close();
                    legacyDoc = new XPathDocument(new StringReader(sb.ToString()));
                    ProcessPackages(legacyDoc.CreateNavigator().SelectSingleNode("CommonLibrary"));
                    break;
                case "languagepack":
                    InstallerInfo.IsLegacyMode = true;
                    LanguagePackWriter languageWriter = new LanguagePackWriter(rootNav, InstallerInfo);
                    LegacyError = languageWriter.LegacyError;
                    if (string.IsNullOrEmpty(LegacyError))
                    {
                        legacyManifest = languageWriter.WriteManifest(false);
                        legacyDoc = new XPathDocument(new StringReader(legacyManifest));
                        ProcessPackages(legacyDoc.CreateNavigator().SelectSingleNode("CommonLibrary"));
                    }
                    break;
                case "skinobject":
                    InstallerInfo.IsLegacyMode = true;
                    SkinControlPackageWriter skinControlwriter = new SkinControlPackageWriter(rootNav, InstallerInfo);
                    legacyManifest = skinControlwriter.WriteManifest(false);
                    legacyDoc = new XPathDocument(new StringReader(legacyManifest));
                    ProcessPackages(legacyDoc.CreateNavigator().SelectSingleNode("CommonLibrary"));
                    break;
            }
        }
        private void UnInstallPackages(bool deleteFiles)
        {
            for (int index = 0; index <= Packages.Count - 1; index++)
            {
                PackageInstaller installer = Packages.Values[index];
                InstallerInfo.Log.AddInfo(Util.UNINSTALL_Start + " - " + installer.Package.Name);
                installer.DeleteFiles = deleteFiles;
                installer.UnInstall();
                if (InstallerInfo.Log.HasWarnings)
                {
                    InstallerInfo.Log.AddWarning(Util.UNINSTALL_Warnings + " - " + installer.Package.Name);
                }
                else
                {
                    InstallerInfo.Log.AddInfo(Util.UNINSTALL_Success + " - " + installer.Package.Name);
                }
            }
        }
        public void DeleteTempFolder()
        {
            if (!string.IsNullOrEmpty(TempInstallFolder))
            {
                Directory.Delete(TempInstallFolder, true);
            }
        }
        public bool Install()
        {
            InstallerInfo.Log.StartJob(Util.INSTALL_Start);
            bool bStatus = true;
            try
            {
                InstallPackages();
            }
            catch (Exception ex)
            {
                InstallerInfo.Log.AddFailure(ex);
                bStatus = false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(TempInstallFolder))
                {
                    CommonLibrary.Common.Globals.DeleteFolderRecursive(TempInstallFolder);
                }
                InstallerInfo.Log.AddInfo(Util.FOLDER_DeletedBackup);
            }
            if (InstallerInfo.Log.Valid)
            {
                InstallerInfo.Log.EndJob(Util.INSTALL_Success);
            }
            else
            {
                InstallerInfo.Log.EndJob(Util.INSTALL_Failed);
                bStatus = false;
            }
            LogInstallEvent("Package", "Install");
            DataCache.ClearHostCache(true);
            return bStatus;
        }
        public void ReadManifest(bool deleteTemp)
        {
            InstallerInfo.Log.StartJob(Util.DNN_Reading);
            if (InstallerInfo.ManifestFile != null)
            {
                ReadManifest(new FileStream(InstallerInfo.ManifestFile.TempFileName, FileMode.Open, FileAccess.Read));
            }
            if (InstallerInfo.Log.Valid)
            {
                InstallerInfo.Log.EndJob(Util.DNN_Success);
            }
            else
            {
                if (deleteTemp)
                {
                    DeleteTempFolder();
                }
            }
        }
        public bool UnInstall(bool deleteFiles)
        {
            InstallerInfo.Log.StartJob(Util.UNINSTALL_Start);
            try
            {
                UnInstallPackages(deleteFiles);
            }
            catch (Exception ex)
            {
                InstallerInfo.Log.AddFailure(ex);
                return false;
            }
            if (InstallerInfo.Log.HasWarnings)
            {
                InstallerInfo.Log.EndJob(Util.UNINSTALL_Warnings);
            }
            else
            {
                InstallerInfo.Log.EndJob(Util.UNINSTALL_Success);
            }
            LogInstallEvent("Package", "UnInstall");
            return true;
        }
    }
}
