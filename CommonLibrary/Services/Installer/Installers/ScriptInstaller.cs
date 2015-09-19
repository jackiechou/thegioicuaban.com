using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.IO;
using CommonLibrary.Data;
using CommonLibrary.Common.Utilities;
using System.Data.Common;

namespace CommonLibrary.Services.Installer.Installers
{
    public class ScriptInstaller : FileInstaller
    {
        private InstallFile _InstallScript;
        private SortedList<System.Version, InstallFile> _InstallScripts = new SortedList<System.Version, InstallFile>();
        private DbTransaction _Transaction;
        private SortedList<System.Version, InstallFile> _UnInstallScripts = new SortedList<System.Version, InstallFile>();
        private InstallFile _UpgradeScript;
        protected InstallFile InstallScript
        {
            get { return _InstallScript; }
        }
        protected SortedList<System.Version, InstallFile> InstallScripts
        {
            get { return _InstallScripts; }
        }
        protected SortedList<System.Version, InstallFile> UnInstallScripts
        {
            get { return _UnInstallScripts; }
        }
        protected override string CollectionNodeName
        {
            get { return "scripts"; }
        }
        protected override string ItemNodeName
        {
            get { return "script"; }
        }
        protected Framework.Providers.ProviderConfiguration ProviderConfiguration
        {
            get { return Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data"); }
        }
        protected InstallFile UpgradeScript
        {
            get { return _UpgradeScript; }
        }
        public override string AllowableFiles
        {
            get { return "*dataprovider"; }
        }
        private bool ExecuteSql(InstallFile scriptFile, bool useTransaction)
        {
            bool bSuccess = true;
            Log.AddInfo(string.Format(Util.SQL_BeginFile, scriptFile.Name));
            string strScript = FileSystemUtils.ReadFile(PhysicalBasePath + scriptFile.FullName);
            if (strScript.StartsWith("?"))
            {
                strScript = strScript.Substring(1);
            }
            string strSQLExceptions = Null.NullString;
            strSQLExceptions = DataProvider.Instance().ExecuteScript(strScript);
            if (!String.IsNullOrEmpty(strSQLExceptions))
            {
                if (Package.InstallerInfo.IsLegacyMode)
                {
                    Log.AddWarning(string.Format(Util.SQL_Exceptions, Environment.NewLine, strSQLExceptions));
                }
                else
                {
                    Log.AddFailure(string.Format(Util.SQL_Exceptions, Environment.NewLine, strSQLExceptions));
                    bSuccess = false;
                }
            }
            Log.AddInfo(string.Format(Util.SQL_EndFile, scriptFile.Name));
            return bSuccess;
        }
        private bool InstallScriptFile(InstallFile scriptFile)
        {
            bool bSuccess = InstallFile(scriptFile);
            if (bSuccess && ProviderConfiguration.DefaultProvider.ToLower() == Path.GetExtension(scriptFile.Name.ToLower()).Substring(1))
            {
                Log.AddInfo(Util.SQL_Executing + scriptFile.Name);
                bSuccess = ExecuteSql(scriptFile, false);
            }
            return bSuccess;
        }
        protected override bool IsCorrectType(InstallFileType type)
        {
            return (type == InstallFileType.Script);
        }
        protected override void ProcessFile(InstallFile file, XPathNavigator nav)
        {
            string type = nav.GetAttribute("type", "");
            if (file != null && IsCorrectType(file.Type))
            {
                if (file.Name.ToLower().StartsWith("install."))
                {
                    _InstallScript = file;
                }
                else if (file.Name.ToLower().StartsWith("upgrade."))
                {
                    _UpgradeScript = file;
                }
                else if (type.ToLower() == "install")
                {
                    InstallScripts[file.Version] = file;
                }
                else
                {
                    UnInstallScripts[file.Version] = file;
                }
            }
            base.ProcessFile(file, nav);
        }
        protected override void UnInstallFile(InstallFile scriptFile)
        {
            if (UnInstallScripts.ContainsValue(scriptFile) && ProviderConfiguration.DefaultProvider.ToLower() == Path.GetExtension(scriptFile.Name.ToLower()).Substring(1))
            {
                if (scriptFile.Name.ToLower().StartsWith("uninstall."))
                {
                    Log.AddInfo(Util.SQL_Executing + scriptFile.Name);
                    ExecuteSql(scriptFile, false);
                }
            }
            base.UnInstallFile(scriptFile);
        }
        public override void Commit()
        {
            base.Commit();
        }
        public override void Install()
        {
            Log.AddInfo(Util.SQL_Begin);
            try
            {
                bool bSuccess = true;
                System.Version installedVersion = Package.InstalledVersion;
                if (installedVersion == new Version(0, 0, 0))
                {
                    if (InstallScript != null)
                    {
                        bSuccess = InstallScriptFile(InstallScript);
                        installedVersion = InstallScript.Version;
                    }
                }
                if (bSuccess)
                {
                    foreach (InstallFile file in InstallScripts.Values)
                    {
                        if (file.Version > installedVersion)
                        {
                            bSuccess = InstallScriptFile(file);
                            if (!bSuccess)
                            {
                                break;
                            }
                        }
                    }
                }
                if (UpgradeScript != null)
                {
                    bSuccess = InstallScriptFile(UpgradeScript);
                    installedVersion = UpgradeScript.Version;
                }
                if (bSuccess)
                {
                    foreach (InstallFile file in UnInstallScripts.Values)
                    {
                        bSuccess = InstallFile(file);
                        if (!bSuccess)
                        {
                            break;
                        }
                    }
                }
                Completed = bSuccess;
            }
            catch (Exception ex)
            {
                Log.AddFailure(Util.EXCEPTION + " - " + ex.Message);
            }
            Log.AddInfo(Util.SQL_End);
        }
        public override void Rollback()
        {
            base.Rollback();
        }
        public override void UnInstall()
        {
            Log.AddInfo(Util.SQL_BeginUnInstall);
            base.UnInstall();
            Log.AddInfo(Util.SQL_EndUnInstall);
        }
    }
}
