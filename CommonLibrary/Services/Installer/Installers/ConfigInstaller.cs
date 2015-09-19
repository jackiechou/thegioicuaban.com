using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Installer.Installers
{
    public class ConfigInstaller : ComponentInstallerBase
    {
        private string _FileName = Null.NullString;
        private string _InstallConfig = Null.NullString;
        private XmlDocument _TargetConfig;
        private InstallFile _TargetFile;
        private string _UnInstallConfig = Null.NullString;
        private string _UninstallFileName = Null.NullString;
        public string InstallConfig
        {
            get { return _InstallConfig; }
        }
        public XmlDocument TargetConfig
        {
            get { return _TargetConfig; }
        }
        public InstallFile TargetFile
        {
            get { return _TargetFile; }
        }
        public string UnInstallConfig
        {
            get { return _UnInstallConfig; }
        }
        public override void Commit()
        {
            try
            {
                Config.Save(TargetConfig, TargetFile.FullName);
                Log.AddInfo(Util.CONFIG_Committed + " - " + TargetFile.Name);
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
                if (string.IsNullOrEmpty(_FileName))
                {
                    Util.BackupFile(TargetFile, PhysicalSitePath, Log);
                    _TargetConfig = new XmlDocument();
                    TargetConfig.Load(Path.Combine(PhysicalSitePath, TargetFile.FullName));
                    XmlMerge merge = new XmlMerge(new StringReader(InstallConfig), this.Package.Version.ToString(), this.Package.Name);
                    merge.UpdateConfig(TargetConfig);
                    Completed = true;
                    Log.AddInfo(Util.CONFIG_Updated + " - " + TargetFile.Name);
                }
                else
                {
                    string strConfigFile = Path.Combine(this.Package.InstallerInfo.TempInstallFolder, _FileName);
                    if (File.Exists(strConfigFile))
                    {
                        StreamReader stream = File.OpenText(strConfigFile);
                        XmlMerge merge = new XmlMerge(stream, Package.Version.ToString(3), Package.Name + " Install");
                        merge.UpdateConfigs();
                        stream.Close();
                        Completed = true;
                        Log.AddInfo(Util.CONFIG_Updated);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
        }
        public override void ReadManifest(XPathNavigator manifestNav)
        {
            _FileName = Util.ReadAttribute(manifestNav, "fileName");
            _UninstallFileName = Util.ReadAttribute(manifestNav, "unInstallFileName");
            if (string.IsNullOrEmpty(_FileName))
            {
                XPathNavigator nav = manifestNav.SelectSingleNode("config");
                XPathNavigator nodeNav = nav.SelectSingleNode("configFile");
                string targetFileName = nodeNav.Value;
                if (!string.IsNullOrEmpty(targetFileName))
                {
                    _TargetFile = new InstallFile(targetFileName, "", this.Package.InstallerInfo);
                }
                nodeNav = nav.SelectSingleNode("install");
                _InstallConfig = nodeNav.InnerXml;
                nodeNav = nav.SelectSingleNode("uninstall");
                _UnInstallConfig = nodeNav.InnerXml;
            }
        }
        public override void Rollback()
        {
            Log.AddInfo(Util.CONFIG_RolledBack + " - " + TargetFile.Name);
        }
        public override void UnInstall()
        {
            if (string.IsNullOrEmpty(_UninstallFileName))
            {
                _TargetConfig = new XmlDocument();
                TargetConfig.Load(Path.Combine(PhysicalSitePath, TargetFile.FullName));
                XmlMerge merge = new XmlMerge(new StringReader(UnInstallConfig), this.Package.Version.ToString(), this.Package.Name);
                merge.UpdateConfig(TargetConfig, TargetFile.FullName);
            }
            else
            {
                string strConfigFile = Path.Combine(this.Package.InstallerInfo.TempInstallFolder, _UninstallFileName);
                if (File.Exists(strConfigFile))
                {
                    StreamReader stream = File.OpenText(strConfigFile);
                    XmlMerge merge = new XmlMerge(stream, Package.Version.ToString(3), Package.Name + " UnInstall");
                    merge.UpdateConfigs();
                    stream.Close();
                }
            }
        }
    }
}
