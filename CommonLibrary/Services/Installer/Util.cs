using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.IO;
using CommonLibrary.Common;
using System.Xml.XPath;
using CommonLibrary.Services.Installer.Log;
using CommonLibrary.UI.Modules;
using CommonLibrary.Entities.Host;

namespace CommonLibrary.Services.Installer
{
    public class Util
    {
        public const string DEFAULT_MANIFESTEXT = ".manifest";
        public static string ASSEMBLY_Added = GetLocalizedString("ASSEMBLY_Added");
        public static string ASSEMBLY_InUse = GetLocalizedString("ASSEMBLY_InUse");
        public static string ASSEMBLY_Registered = GetLocalizedString("ASSEMBLY_Registered");
        public static string ASSEMBLY_UnRegistered = GetLocalizedString("ASSEMBLY_UnRegistered");
        public static string ASSEMBLY_Updated = GetLocalizedString("ASSEMBLY_Updated");
        public static string AUTHENTICATION_ReadSuccess = GetLocalizedString("AUTHENTICATION_ReadSuccess");
        public static string AUTHENTICATION_LoginSrcMissing = GetLocalizedString("AUTHENTICATION_LoginSrcMissing");
        public static string AUTHENTICATION_Registered = GetLocalizedString("AUTHENTICATION_Registered");
        public static string AUTHENTICATION_SettingsSrcMissing = GetLocalizedString("AUTHENTICATION_SettingsSrcMissing");
        public static string AUTHENTICATION_TypeMissing = GetLocalizedString("AUTHENTICATION_TypeMissing");
        public static string AUTHENTICATION_UnRegistered = GetLocalizedString("AUTHENTICATION_UnRegistered");
        public static string CLEANUP_Processing = GetLocalizedString("CLEANUP_Processing");
        public static string CLEANUP_ProcessComplete = GetLocalizedString("CLEANUP_ProcessComplete");
        public static string COMPONENT_Installed = GetLocalizedString("COMPONENT_Installed");
        public static string COMPONENT_RolledBack = GetLocalizedString("COMPONENT_RolledBack");
        public static string COMPONENT_RollingBack = GetLocalizedString("COMPONENT_RollingBack");
        public static string COMPONENT_UnInstalled = GetLocalizedString("COMPONENT_UnInstalled");
        public static string CONFIG_Committed = GetLocalizedString("CONFIG_Committed");
        public static string CONFIG_RolledBack = GetLocalizedString("CONFIG_RolledBack");
        public static string CONFIG_Updated = GetLocalizedString("CONFIG_Updated");
        public static string DASHBOARD_ReadSuccess = GetLocalizedString("DASHBOARD_ReadSuccess");
        public static string DASHBOARD_SrcMissing = GetLocalizedString("DASHBOARD_SrcMissing");
        public static string DASHBOARD_Registered = GetLocalizedString("DASHBOARD_Registered");
        public static string DASHBOARD_KeyMissing = GetLocalizedString("DASHBOARD_KeyMissing");
        public static string DASHBOARD_LocalResourcesMissing = GetLocalizedString("DASHBOARD_LocalResourcesMissing");
        public static string DASHBOARD_UnRegistered = GetLocalizedString("DASHBOARD_UnRegistered");
        public static string DNN_Reading = GetLocalizedString("DNN_Reading");
        public static string DNN_ReadingComponent = GetLocalizedString("DNN_ReadingComponent");
        public static string DNN_ReadingPackage = GetLocalizedString("DNN_ReadingPackage");
        public static string DNN_Success = GetLocalizedString("DNN_Success");
        public static string EVENTMESSAGE_CommandMissing = GetLocalizedString("EVENTMESSAGE_CommandMissing");
        public static string EVENTMESSAGE_TypeMissing = GetLocalizedString("EVENTMESSAGE_TypeMissing");
        public static string EXCEPTION = GetLocalizedString("EXCEPTION");
        public static string EXCEPTION_NameMissing = GetLocalizedString("EXCEPTION_NameMissing");
        public static string EXCEPTION_TypeMissing = GetLocalizedString("EXCEPTION_TypeMissing");
        public static string EXCEPTION_VersionMissing = GetLocalizedString("EXCEPTION_VersionMissing");
        public static string EXCEPTION_FileLoad = GetLocalizedString("EXCEPTION_FileLoad");
        public static string EXCEPTION_FileRead = GetLocalizedString("EXCEPTION_FileRead");
        public static string EXCEPTION_InstallerCreate = GetLocalizedString("EXCEPTION_InstallerCreate");
        public static string EXCEPTION_MissingDnn = GetLocalizedString("EXCEPTION_MissingDnn");
        public static string EXCEPTION_MultipleDnn = GetLocalizedString("EXCEPTION_MultipleDnn");
        public static string EXCEPTION_Type = GetLocalizedString("EXCEPTION_Type");
        public static string FILE_CreateBackup = GetLocalizedString("FILE_CreateBackup");
        public static string FILE_Created = GetLocalizedString("FILE_Created");
        public static string FILE_Deleted = GetLocalizedString("FILE_Deleted");
        public static string FILE_Found = GetLocalizedString("FILE_Found");
        public static string FILE_Loading = GetLocalizedString("FILE_Loading");
        public static string FILE_NotAllowed = GetLocalizedString("FILE_NotAllowed");
        public static string FILE_NotFound = GetLocalizedString("FILE_NotFound");
        public static string FILE_ReadSuccess = GetLocalizedString("FILE_ReadSuccess");
        public static string FILE_RestoreBackup = GetLocalizedString("FILE_RestoreBackup");
        public static string FILE_RolledBack = GetLocalizedString("FILE_RolledBack");
        public static string FILES_CreatedResources = GetLocalizedString("FILES_CreatedResources");
        public static string FILES_Expanding = GetLocalizedString("FILES_Expanding");
        public static string FILES_Loading = GetLocalizedString("FILES_Loading");
        public static string FILES_Reading = GetLocalizedString("FILES_Reading");
        public static string FILES_ReadingEnd = GetLocalizedString("FILES_ReadingEnd");
        public static string FOLDER_Created = GetLocalizedString("FOLDER_Created");
        public static string FOLDER_Deleted = GetLocalizedString("FOLDER_Deleted");
        public static string FOLDER_DeletedBackup = GetLocalizedString("FOLDER_DeletedBackup");
        public static string INSTALL_Compatibility = GetLocalizedString("INSTALL_Compatibility");
        public static string INSTALL_Dependencies = GetLocalizedString("INSTALL_Dependencies");
        public static string INSTALL_Aborted = GetLocalizedString("INSTALL_Aborted");
        public static string INSTALL_Failed = GetLocalizedString("INSTALL_Failed");
        public static string INSTALL_Committed = GetLocalizedString("INSTALL_Committed");
        public static string INSTALL_Namespace = GetLocalizedString("INSTALL_Namespace");
        public static string INSTALL_Package = GetLocalizedString("INSTALL_Package");
        public static string INSTALL_Permissions = GetLocalizedString("INSTALL_Permissions");
        public static string INSTALL_Start = GetLocalizedString("INSTALL_Start");
        public static string INSTALL_Success = GetLocalizedString("INSTALL_Success");
        public static string INSTALL_Version = GetLocalizedString("INSTALL_Version");
        public static string LANGUAGE_PortalsEnabled = GetLocalizedString("LANGUAGE_PortalsEnabled");
        public static string LANGUAGE_Registered = GetLocalizedString("LANGUAGE_Registered");
        public static string LANGUAGE_UnRegistered = GetLocalizedString("LANGUAGE_UnRegistered");
        public static string MODULE_ControlKeyMissing = GetLocalizedString("MODULE_ControlKeyMissing");
        public static string MODULE_ControlTypeMissing = GetLocalizedString("MODULE_ControlTypeMissing");
        public static string MODULE_FriendlyNameMissing = GetLocalizedString("MODULE_FriendlyNameMissing");
        public static string MODULE_ReadSuccess = GetLocalizedString("MODULE_ReadSuccess");
        public static string MODULE_Registered = GetLocalizedString("MODULE_Registered");
        public static string MODULE_UnRegistered = GetLocalizedString("MODULE_UnRegistered");
        public static string PACKAGE_NoLicense = GetLocalizedString("PACKAGE_NoLicense");
        public static string PACKAGE_NoReleaseNotes = GetLocalizedString("PACKAGE_NoReleaseNotes");
        public static string PACKAGE_UnRecognizable = GetLocalizedString("PACKAGE_UnRecognizable");
        public static string SECURITY_Installer = GetLocalizedString("SECURITY_Installer");
        public static string SECURITY_NotRegistered = GetLocalizedString("SECURITY_NotRegistered");
        public static string SKIN_BeginProcessing = GetLocalizedString("SKIN_BeginProcessing");
        public static string SKIN_Installed = GetLocalizedString("SKIN_Installed");
        public static string SKIN_EndProcessing = GetLocalizedString("SKIN_EndProcessing");
        public static string SKIN_Registered = GetLocalizedString("SKIN_Registered");
        public static string SKIN_UnRegistered = GetLocalizedString("SKIN_UnRegistered");
        public static string SQL_Begin = GetLocalizedString("SQL_Begin");
        public static string SQL_BeginFile = GetLocalizedString("SQL_BeginFile");
        public static string SQL_BeginUnInstall = GetLocalizedString("SQL_BeginUnInstall");
        public static string SQL_Committed = GetLocalizedString("SQL_Committed");
        public static string SQL_End = GetLocalizedString("SQL_End");
        public static string SQL_EndFile = GetLocalizedString("SQL_EndFile");
        public static string SQL_EndUnInstall = GetLocalizedString("SQL_EndUnInstall");
        public static string SQL_Exceptions = GetLocalizedString("SQL_Exceptions");
        public static string SQL_Executing = GetLocalizedString("SQL_Executing");
        public static string SQL_RolledBack = GetLocalizedString("SQL_RolledBack");
        public static string UNINSTALL_Start = GetLocalizedString("UNINSTALL_Start");
        public static string UNINSTALL_StartComp = GetLocalizedString("UNINSTALL_StartComp");
        public static string UNINSTALL_Failure = GetLocalizedString("UNINSTALL_Failure");
        public static string UNINSTALL_Success = GetLocalizedString("UNINSTALL_Success");
        public static string UNINSTALL_SuccessComp = GetLocalizedString("UNINSTALL_SuccessComp");
        public static string UNINSTALL_Warnings = GetLocalizedString("UNINSTALL_Warnings");
        public static string UNINSTALL_WarningsComp = GetLocalizedString("UNINSTALL_WarningsComp");
        public static string WRITER_AddFileToManifest = GetLocalizedString("WRITER_AddFileToManifest");
        public static string WRITER_CreateArchive = GetLocalizedString("WRITER_CreateArchive");
        public static string WRITER_CreatedManifest = GetLocalizedString("WRITER_CreatedManifest");
        public static string WRITER_CreatedPackage = GetLocalizedString("WRITER_CreatedPackage");
        public static string WRITER_CreatingManifest = GetLocalizedString("WRITER_CreatingManifest");
        public static string WRITER_CreatingPackage = GetLocalizedString("WRITER_CreatingPackage");
        public static string WRITER_SavedFile = GetLocalizedString("WRITER_SavedFile");
        public static string WRITER_SaveFileError = GetLocalizedString("WRITER_SaveFileError");
        public static string REGEX_Version = "\\d{2}.\\d{2}.\\d{2}";
        private static void StreamToStream(Stream SourceStream, Stream DestStream)
        {
            byte[] buf = new byte[1024];
            int count = 0;
            do
            {
                count = SourceStream.Read(buf, 0, 1024);
                DestStream.Write(buf, 0, count);
            } while (count > 0);
            DestStream.Flush();
        }
        private static void TryDeleteFolder(DirectoryInfo folder, Logger log)
        {
            if (folder.GetFiles().Length == 0 && folder.GetDirectories().Length == 0)
            {
                folder.Delete();
                log.AddInfo(string.Format(Util.FOLDER_Deleted, folder.Name));
                TryDeleteFolder(folder.Parent, log);
            }
        }
        private static string ValidateNode(string propValue, bool isRequired, Logger log, string logmessage, string defaultValue)
        {
            if (string.IsNullOrEmpty(propValue))
            {
                if (isRequired)
                {
                    log.AddFailure(logmessage);
                }
                else
                {
                    propValue = defaultValue;
                }
            }
            return propValue;
        }
        public static void BackupFile(InstallFile installFile, string basePath, Logger log)
        {
            string fullFileName = Path.Combine(basePath, installFile.FullName);
            string backupFileName = Path.Combine(installFile.BackupPath, installFile.Name + ".config");
            if (!Directory.Exists(installFile.BackupPath))
            {
                Directory.CreateDirectory(installFile.BackupPath);
            }
            FileSystemUtils.CopyFile(fullFileName, backupFileName);
            log.AddInfo(string.Format(Util.FILE_CreateBackup, installFile.FullName));
        }
        public static void CopyFile(InstallFile installFile, string basePath, Logger log)
        {
            string filePath = Path.Combine(basePath, installFile.Path);
            string fullFileName = Path.Combine(basePath, installFile.FullName);
            if (!Directory.Exists(filePath))
            {
                log.AddInfo(string.Format(Util.FOLDER_Created, filePath));
                Directory.CreateDirectory(filePath);
            }
            FileSystemUtils.CopyFile(installFile.TempFileName, fullFileName);
            log.AddInfo(string.Format(Util.FILE_Created, installFile.FullName));
        }
        public static void DeleteFile(InstallFile installFile, string basePath, Logger log)
        {
            DeleteFile(installFile.FullName, basePath, log);
        }
        public static void DeleteFile(string fileName, string basePath, Logger log)
        {
            string fullFileName = Path.Combine(basePath, fileName);
            if (File.Exists(fullFileName))
            {
                FileSystemUtils.DeleteFile(fullFileName);
                log.AddInfo(string.Format(Util.FILE_Deleted, fileName));
                string folderName = Path.GetDirectoryName(fullFileName);
                DirectoryInfo folder = new DirectoryInfo(folderName);
                TryDeleteFolder(folder, log);
            }
        }
        public static string GetLocalizedString(string key)
        {
            return Localization.Localization.GetString(key);
        }
        public static bool IsFileValid(InstallFile file, string packageWhiteList)
        {
            bool _IsFileValid = Null.NullBoolean;
            string systemWhiteList = Host.FileExtensions.ToLower();
            string strExtension = file.Extension.ToLowerInvariant();
            if ((strExtension == "dnn" || systemWhiteList.Contains(strExtension) || packageWhiteList.Contains(strExtension) || (packageWhiteList.Contains("*dataprovider") && strExtension.EndsWith("dataprovider"))))
            {
                _IsFileValid = true;
            }
            return _IsFileValid;
        }
        public static string InstallURL(int tabId, string type)
        {
            string[] parameters = new string[2];
            parameters[0] = "rtab=" + tabId.ToString();
            if (!string.IsNullOrEmpty(type))
            {
                parameters[1] = "ptype=" + type;
            }
            return Globals.NavigateURL(tabId, "Install", parameters);
        }
        public static string InstallURL(int tabId, string returnUrl, string type)
        {
            string[] parameters = new string[3];
            parameters[0] = "rtab=" + tabId.ToString();
            parameters[1] = "returnUrl=" + returnUrl;
            if (!string.IsNullOrEmpty(type))
            {
                parameters[2] = "ptype=" + type;
            }
            return Globals.NavigateURL(tabId, "Install", parameters);
        }
        public static string UnInstallURL(int tabId, int packageId, string returnUrl)
        {
            string[] parameters = new string[3];
            parameters[0] = "rtab=" + tabId.ToString();
            parameters[1] = "returnUrl=" + returnUrl;
            parameters[2] = "packageId=" + packageId.ToString();
            return Globals.NavigateURL(tabId, "UnInstall", parameters);
        }
        public static string PackageWriterURL(ModuleInstanceContext context, int packageId)
        {
            string[] parameters = new string[3];
            parameters[0] = "rtab=" + context.TabId.ToString();
            parameters[1] = "packageId=" + packageId.ToString();
            parameters[2] = "mid=" + context.ModuleId.ToString();
            return Globals.NavigateURL(context.TabId, "PackageWriter", parameters);
        }
        public static string ReadAttribute(XPathNavigator nav, string attributeName)
        {
            return ValidateNode(nav.GetAttribute(attributeName, ""), false, null, "", "");
        }
        public static string ReadAttribute(XPathNavigator nav, string attributeName, Logger log, string logmessage)
        {
            return ValidateNode(nav.GetAttribute(attributeName, ""), true, log, logmessage, "");
        }
        public static string ReadAttribute(XPathNavigator nav, string attributeName, bool isRequired, Logger log, string logmessage, string defaultValue)
        {
            return ValidateNode(nav.GetAttribute(attributeName, ""), isRequired, log, logmessage, defaultValue);
        }
        public static string ReadElement(XPathNavigator nav, string elementName)
        {
            return ValidateNode(XmlUtils.GetNodeValue(nav, elementName), false, null, "", "");
        }
        public static string ReadElement(XPathNavigator nav, string elementName, string defaultValue)
        {
            return ValidateNode(XmlUtils.GetNodeValue(nav, elementName), false, null, "", defaultValue);
        }
        public static string ReadElement(XPathNavigator nav, string elementName, Logger log, string logmessage)
        {
            return ValidateNode(XmlUtils.GetNodeValue(nav, elementName), true, log, logmessage, "");
        }
        public static string ReadElement(XPathNavigator nav, string elementName, bool isRequired, Logger log, string logmessage, string defaultValue)
        {
            return ValidateNode(XmlUtils.GetNodeValue(nav, elementName), isRequired, log, logmessage, defaultValue);
        }
        public static void RestoreFile(InstallFile installFile, string basePath, Logger log)
        {
            string fullFileName = Path.Combine(basePath, installFile.FullName);
            string backupFileName = Path.Combine(installFile.BackupPath, installFile.Name + ".config");
            FileSystemUtils.CopyFile(backupFileName, fullFileName);
            log.AddInfo(string.Format(Util.FILE_RestoreBackup, installFile.FullName));
        }
        public static string UnInstallURL(int tabId, int packageId)
        {
            string[] parameters = new string[2];
            parameters[0] = "rtab=" + tabId.ToString();
            parameters[1] = "packageId=" + packageId.ToString();
            return Globals.NavigateURL(tabId, "UnInstall", parameters);
        }
        public static void WriteStream(Stream SourceStream, string DestFileName)
        {
            FileSystemUtils.DeleteFile(DestFileName);
            FileInfo file = new FileInfo(DestFileName);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            Stream fileStrm = file.Create();
            StreamToStream(SourceStream, fileStrm);
            fileStrm.Close();
        }
    }
}
