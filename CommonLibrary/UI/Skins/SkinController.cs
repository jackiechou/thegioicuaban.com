using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Data;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Host;
using System.Text.RegularExpressions;
using CommonLibrary.Common.Utilities;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Collections;
using CommonLibrary.Services.Localization;
using CommonLibrary.Common;


namespace CommonLibrary.UI.Skins
{
    public class SkinController
    {
        public static string RootSkin
        {
            get { return "Skins"; }
        }
        public static string RootContainer
        {
            get { return "Containers"; }
        }
        public static int AddSkin(int skinPackageID, string skinSrc)
        {
            return DataProvider.Instance().AddSkin(skinPackageID, skinSrc);
        }
        public static int AddSkinPackage(SkinPackageInfo skinPackage)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(skinPackage, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.SKINPACKAGE_CREATED);
            return DataProvider.Instance().AddSkinPackage(skinPackage.PackageID, skinPackage.PortalID, skinPackage.SkinName, skinPackage.SkinType, UserController.GetCurrentUserInfo().UserID);
        }
        public static bool CanDeleteSkin(string strFolderPath, string portalHomeDirMapPath)
        {
            string strSkinType;
            string strSkinFolder;
            bool canDelete = true;
            if (strFolderPath.ToLower().IndexOf(Common.Globals.HostMapPath.ToLower()) != -1)
            {
                strSkinType = "G";
                strSkinFolder = strFolderPath.ToLower().Replace(Common.Globals.HostMapPath.ToLower(), "").Replace("\\", "/");
            }
            else
            {
                strSkinType = "L";
                strSkinFolder = strFolderPath.ToLower().Replace(portalHomeDirMapPath.ToLower(), "").Replace("\\", "/");
            }
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
            string skin = "[" + strSkinType.ToLowerInvariant() + "]" + strSkinFolder.ToLowerInvariant();
            if (strSkinFolder.ToLowerInvariant().Contains("skins"))
            {
                if (Host.DefaultAdminSkin.ToLowerInvariant().StartsWith(skin) || Host.DefaultPortalSkin.ToLowerInvariant().StartsWith(skin) || portalSettings.DefaultAdminSkin.ToLowerInvariant().StartsWith(skin) || portalSettings.DefaultPortalSkin.ToLowerInvariant().StartsWith(skin))
                {
                    canDelete = false;
                }
            }
            else
            {
                if (Host.DefaultAdminContainer.ToLowerInvariant().StartsWith(skin) || Host.DefaultPortalContainer.ToLowerInvariant().StartsWith(skin) || portalSettings.DefaultAdminContainer.ToLowerInvariant().StartsWith(skin) || portalSettings.DefaultPortalContainer.ToLowerInvariant().StartsWith(skin))
                {
                    canDelete = false;
                }
            }
            if (canDelete)
            {
                canDelete = DataProvider.Instance().CanDeleteSkin(strSkinType, strSkinFolder);
            }
            return canDelete;
        }
        public static void DeleteSkin(int skinID)
        {
            DataProvider.Instance().DeleteSkin(skinID);
        }
        public static void DeleteSkinPackage(SkinPackageInfo skinPackage)
        {
            DataProvider.Instance().DeleteSkinPackage(skinPackage.SkinPackageID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(skinPackage, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.SKINPACKAGE_DELETED);
        }
        public static string FormatMessage(string Title, string Body, int Level, bool IsError)
        {
            string Message = Title;
            if (IsError)
            {
                Message = "<font class=\"NormalRed\">" + Title + "</font>";
            }
            switch (Level)
            {
                case -1:
                    Message = "<hr><br><b>" + Message + "</b>";
                    break;
                case 0:
                    Message = "<br><br><b>" + Message + "</b>";
                    break;
                case 1:
                    Message = "<br><b>" + Message + "</b>";
                    break;
                default:
                    Message = "<br><li>" + Message;
                    break;
            }
            return Message + ": " + Body + Environment.NewLine;
        }
        public static string FormatSkinPath(string SkinSrc)
        {
            string strSkinSrc = SkinSrc;
            if (!String.IsNullOrEmpty(strSkinSrc))
            {
                strSkinSrc = strSkinSrc.Substring(0, strSkinSrc.LastIndexOf("/") + 1);
            }
            return strSkinSrc;
        }
        public static string FormatSkinSrc(string SkinSrc, PortalSettings PortalSettings)
        {
            string strSkinSrc = SkinSrc;
            if (!String.IsNullOrEmpty(strSkinSrc))
            {
                switch (strSkinSrc.ToLowerInvariant().Substring(0, 3))
                {
                    case "[g]":
                        strSkinSrc = Regex.Replace(strSkinSrc, "\\[g]", Common.Globals.HostPath, RegexOptions.IgnoreCase);
                        break;
                    case "[l]":
                        strSkinSrc = Regex.Replace(strSkinSrc, "\\[l]", PortalSettings.HomeDirectory, RegexOptions.IgnoreCase);
                        break;
                }
            }
            return strSkinSrc;
        }
        public static string GetDefaultAdminContainer()
        {
            SkinDefaults defaultContainer = SkinDefaults.GetSkinDefaults(SkinDefaultType.ContainerInfo);
            return "[G]" + SkinController.RootContainer + defaultContainer.Folder + defaultContainer.AdminDefaultName;
        }
        public static string GetDefaultAdminSkin()
        {
            SkinDefaults defaultSkin = SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo);
            return "[G]" + SkinController.RootSkin + defaultSkin.Folder + defaultSkin.AdminDefaultName;
        }
        public static string GetDefaultPortalContainer()
        {
            SkinDefaults defaultContainer = SkinDefaults.GetSkinDefaults(SkinDefaultType.ContainerInfo);
            return "[G]" + SkinController.RootContainer + defaultContainer.Folder + defaultContainer.DefaultName;
        }
        public static string GetDefaultPortalSkin()
        {
            SkinDefaults defaultSkin = SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo);
            return "[G]" + SkinController.RootSkin + defaultSkin.Folder + defaultSkin.DefaultName;
        }
        public static SkinPackageInfo GetSkinByPackageID(int packageID)
        {
            return CBO.FillObject<SkinPackageInfo>(DataProvider.Instance().GetSkinByPackageID(packageID));
        }
        public static SkinPackageInfo GetSkinPackage(int portalId, string skinName, string skinType)
        {
            return CBO.FillObject<SkinPackageInfo>(DataProvider.Instance().GetSkinPackage(portalId, skinName, skinType));
        }
        public static bool IsGlobalSkin(string SkinSrc)
        {
            return SkinSrc.Contains(Common.Globals.HostPath);
        }
        public static void SetSkin(string SkinRoot, int PortalId, UI.Skins.SkinType SkinType, string SkinSrc)
        {
            Entities.Host.HostSettingsController objHostSettings = new Entities.Host.HostSettingsController();
            switch (SkinRoot)
            {
                case "Skins":
                    if (SkinType == Skins.SkinType.Admin)
                    {
                        if (PortalId == Null.NullInteger)
                        {
                            objHostSettings.UpdateHostSetting("DefaultAdminSkin", SkinSrc);
                        }
                        else
                        {
                            PortalController.UpdatePortalSetting(PortalId, "DefaultAdminSkin", SkinSrc);
                        }
                    }
                    else
                    {
                        if (PortalId == Null.NullInteger)
                        {
                            objHostSettings.UpdateHostSetting("DefaultPortalSkin", SkinSrc);
                        }
                        else
                        {
                            PortalController.UpdatePortalSetting(PortalId, "DefaultPortalSkin", SkinSrc);
                        }
                    }
                    break;
                case "Containers":
                    if (SkinType == Skins.SkinType.Admin)
                    {
                        if (PortalId == Null.NullInteger)
                        {
                            objHostSettings.UpdateHostSetting("DefaultAdminContainer", SkinSrc);
                        }
                        else
                        {
                            PortalController.UpdatePortalSetting(PortalId, "DefaultAdminContainer", SkinSrc);
                        }
                    }
                    else
                    {
                        if (PortalId == Null.NullInteger)
                        {
                            objHostSettings.UpdateHostSetting("DefaultPortalContainer", SkinSrc);
                        }
                        else
                        {
                            PortalController.UpdatePortalSetting(PortalId, "DefaultPortalContainer", SkinSrc);
                        }
                    }
                    break;
            }
        }
        public static void UpdateSkin(int skinID, string skinSrc)
        {
            DataProvider.Instance().UpdateSkin(skinID, skinSrc);
        }
        public static void UpdateSkinPackage(SkinPackageInfo skinPackage)
        {
            DataProvider.Instance().UpdateSkinPackage(skinPackage.SkinPackageID, skinPackage.PackageID, skinPackage.PortalID, skinPackage.SkinName, skinPackage.SkinType, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(skinPackage, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.SKINPACKAGE_UPDATED);
            foreach (KeyValuePair<int, string> kvp in skinPackage.Skins)
            {
                UpdateSkin(kvp.Key, kvp.Value);
            }
        }
        public static string UploadLegacySkin(string RootPath, string SkinRoot, string SkinName, Stream objInputStream)
        {
            ZipInputStream objZipInputStream = new ZipInputStream(objInputStream);
            ZipEntry objZipEntry;
            string strExtension;
            string strFileName;
            FileStream objFileStream;
            int intSize = 2048;
            byte[] arrData = new byte[2048];
            string strMessage = "";
            ArrayList arrSkinFiles = new ArrayList();
            PortalSettings ResourcePortalSettings = CommonLibrary.Common.Globals.GetPortalSettings();
            string BEGIN_MESSAGE = Localization.GetString("BeginZip", ResourcePortalSettings);
            string CREATE_DIR = Localization.GetString("CreateDir", ResourcePortalSettings);
            string WRITE_FILE = Localization.GetString("WriteFile", ResourcePortalSettings);
            string FILE_ERROR = Localization.GetString("FileError", ResourcePortalSettings);
            string END_MESSAGE = Localization.GetString("EndZip", ResourcePortalSettings);
            string FILE_RESTICTED = Localization.GetString("FileRestricted", ResourcePortalSettings);
            strMessage += FormatMessage(BEGIN_MESSAGE, SkinName, -1, false);
            objZipEntry = objZipInputStream.GetNextEntry();
            while (objZipEntry != null)
            {
                if (!objZipEntry.IsDirectory)
                {
                    strExtension = objZipEntry.Name.Substring(objZipEntry.Name.LastIndexOf(".") + 1);
                    if (("," + strExtension.ToUpper()).IndexOf(",ASCX,HTM,HTML,CSS,SWF,RESX,XAML,JS," + Host.FileExtensions.ToUpper()) != 0)
                    {
                        if (objZipEntry.Name.ToLower() == SkinController.RootSkin.ToLower() + ".zip")
                        {
                            MemoryStream objMemoryStream = new MemoryStream();
                            intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            while (intSize > 0)
                            {
                                objMemoryStream.Write(arrData, 0, intSize);
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            }
                            objMemoryStream.Seek(0, SeekOrigin.Begin);
                            strMessage += UploadLegacySkin(RootPath, SkinController.RootSkin, SkinName, (Stream)objMemoryStream);
                        }
                        else if (objZipEntry.Name.ToLower() == SkinController.RootContainer.ToLower() + ".zip")
                        {
                            MemoryStream objMemoryStream = new MemoryStream();
                            intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            while (intSize > 0)
                            {
                                objMemoryStream.Write(arrData, 0, intSize);
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            }
                            objMemoryStream.Seek(0, SeekOrigin.Begin);
                            strMessage += UploadLegacySkin(RootPath, SkinController.RootContainer, SkinName, (Stream)objMemoryStream);
                        }
                        else
                        {
                            strFileName = RootPath + SkinRoot + "\\" + SkinName + "\\" + objZipEntry.Name;
                            if (!Directory.Exists(Path.GetDirectoryName(strFileName)))
                            {
                                strMessage += FormatMessage(CREATE_DIR, Path.GetDirectoryName(strFileName), 2, false);
                                Directory.CreateDirectory(Path.GetDirectoryName(strFileName));
                            }
                            if (File.Exists(strFileName))
                            {
                                File.SetAttributes(strFileName, FileAttributes.Normal);
                                File.Delete(strFileName);
                            }
                            objFileStream = File.Create(strFileName);
                            strMessage += FormatMessage(WRITE_FILE, Path.GetFileName(strFileName), 2, false);
                            intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            while (intSize > 0)
                            {
                                objFileStream.Write(arrData, 0, intSize);
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length);
                            }
                            objFileStream.Close();
                            switch (Path.GetExtension(strFileName))
                            {
                                case ".htm":
                                case ".html":
                                case ".ascx":
                                case ".css":
                                    if (strFileName.ToLower().IndexOf(Globals.glbAboutPage.ToLower()) < 0)
                                    {
                                        arrSkinFiles.Add(strFileName);
                                    }
                                    break;
                            }
                            break;
                        }
                    }
                    else
                    {
                        strMessage += string.Format(FILE_RESTICTED, objZipEntry.Name, Host.FileExtensions.ToString(), ",", ", *.").Replace("2", "true");
                    }
                }
                objZipEntry = objZipInputStream.GetNextEntry();
            }
            strMessage += FormatMessage(END_MESSAGE, SkinName + ".zip", 1, false);
            objZipInputStream.Close();
            UI.Skins.SkinFileProcessor NewSkin = new UI.Skins.SkinFileProcessor(RootPath, SkinRoot, SkinName);
            strMessage += NewSkin.ProcessList(arrSkinFiles, SkinParser.Portable);
            try
            {
                Services.Log.EventLog.LogInfo objEventLogInfo = new Services.Log.EventLog.LogInfo();
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString();
                objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("Install Skin:", SkinName));
                Array arrMessage = strMessage.Split(new string[] { "<br>" }, StringSplitOptions.None);
                foreach (string strRow in arrMessage)
                {
                    objEventLogInfo.LogProperties.Add(new CommonLibrary.Services.Log.EventLog.LogDetailInfo("Info:", HtmlUtils.StripTags(strRow, true)));
                }
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog(objEventLogInfo);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return strMessage;
        }

    }
}
