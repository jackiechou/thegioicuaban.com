using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using CommonLibrary.Data;
using CommonLibrary.Common.Utilities;
using System.Web;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Services.FileSystem
{
    public class FolderController
    {
        public enum StorageLocationTypes
        {
            InsecureFileSystem = 0,
            SecureFileSystem = 1,
            DatabaseSecure = 2
        }
        private static object GetFoldersSortedCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalID = (int)cacheItemArgs.ParamList[0];
            return CBO.FillSortedList<string, FolderInfo>("FolderPath", DataProvider.Instance().GetFoldersByPortal(portalID));
        }
        private void UpdateParentFolder(int PortalID, string FolderPath)
        {
            if (!String.IsNullOrEmpty(FolderPath))
            {
                string parentFolderPath = FolderPath.Substring(0, FolderPath.Substring(0, FolderPath.Length - 1).LastIndexOf("/") + 1);
                FolderInfo objFolder = GetFolder(PortalID, parentFolderPath, false);
                if (objFolder != null)
                {
                    UpdateFolder(objFolder);
                }
            }
        }
        public int AddFolder(int PortalID, string FolderPath)
        {
            return AddFolder(PortalID, FolderPath, (int)StorageLocationTypes.InsecureFileSystem, false, false);
        }
        public int AddFolder(int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached)
        {
            return AddFolder(PortalID, FolderPath, StorageLocation, IsProtected, IsCached, Null.NullDate);
        }
        public int AddFolder(int PortalID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, System.DateTime LastUpdated)
        {
            int FolderId;
            FolderPath = FileSystemUtils.FormatFolderPath(FolderPath);
            FolderInfo folder = GetFolder(PortalID, FolderPath, true);
            if (folder == null)
            {
                FolderId = DataProvider.Instance().AddFolder(PortalID, FolderPath, StorageLocation, IsProtected, IsCached, LastUpdated, UserController.GetCurrentUserInfo().UserID);
                folder = GetFolder(PortalID, FolderPath, true);
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog(folder, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.FOLDER_CREATED);
                UpdateParentFolder(PortalID, FolderPath);
            }
            else
            {
                FolderId = folder.FolderID;
                DataProvider.Instance().UpdateFolder(PortalID, FolderId, FolderPath, StorageLocation, IsProtected, IsCached, LastUpdated, UserController.GetCurrentUserInfo().UserID);
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog("FolderPath", FolderPath, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.FOLDER_UPDATED);
            }
            DataCache.ClearFolderCache(PortalID);
            return FolderId;
        }
        public void DeleteFolder(int PortalID, string FolderPath)
        {
            DataProvider.Instance().DeleteFolder(PortalID, FileSystemUtils.FormatFolderPath(FolderPath));
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("FolderPath", FolderPath, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Log.EventLog.EventLogController.EventLogType.FOLDER_DELETED);
            UpdateParentFolder(PortalID, FolderPath);
            DataCache.ClearFolderCache(PortalID);
        }
        public FolderInfo GetFolder(int PortalID, string FolderPath, bool ignoreCache)
        {
            FolderPath = FileSystemUtils.FormatFolderPath(FolderPath);
            FolderInfo folder = null;
            bool bFound = false;
            if (!ignoreCache)
            {
                SortedList<string, FolderInfo> dicFolders;
                dicFolders = GetFoldersSorted(PortalID);
                bFound = dicFolders.TryGetValue(FolderPath, out folder);
            }
            if (ignoreCache || !bFound)
            {
                folder = CBO.FillObject<FolderInfo>(DataProvider.Instance().GetFolder(PortalID, FolderPath));
            }
            return folder;
        }
        public FolderInfo GetFolderInfo(int PortalID, int FolderID)
        {
            return CBO.FillObject<FolderInfo>(DataProvider.Instance().GetFolder(PortalID, FolderID));
        }
        public SortedList<string, FolderInfo> GetFoldersSorted(int PortalID)
        {
            string cacheKey = string.Format(DataCache.FolderCacheKey, PortalID.ToString());
            return CBO.GetCachedObject<SortedList<string, FolderInfo>>(new CacheItemArgs(cacheKey, DataCache.FolderCacheTimeOut, DataCache.FolderCachePriority, PortalID), GetFoldersSortedCallBack);
        }
        public string GetMappedDirectory(string VirtualDirectory)
        {
            string MappedDir = Convert.ToString(DataCache.GetCache("DirMap:" + VirtualDirectory));
            try
            {
                if (String.IsNullOrEmpty(MappedDir) && HttpContext.Current != null)
                {
                    MappedDir = FileSystemUtils.AddTrailingSlash(FileSystemUtils.MapPath(VirtualDirectory));
                    DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir);
                }
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
            return MappedDir;
        }
        public void SetMappedDirectory(string VirtualDirectory)
        {
            try
            {
                string MappedDir = FileSystemUtils.AddTrailingSlash(FileSystemUtils.MapPath(VirtualDirectory));
                DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir);
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
        }
        public void SetMappedDirectory(string VirtualDirectory, HttpContext context)
        {
            try
            {
                // The logic here was updated to use the new FileSystemUtils.MapPath so that we have consistent behavior with other Overloads
                string MappedDir = FileSystemUtils.AddTrailingSlash(FileSystemUtils.MapPath(VirtualDirectory));
                DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir);
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
        }
        public void SetMappedDirectory(PortalInfo portalInfo, HttpContext context)
        {
            try
            {
                string VirtualDirectory = Common.Globals.ApplicationPath + "/" + portalInfo.HomeDirectory + "/";
                SetMappedDirectory(VirtualDirectory, context);
            }
            catch (Exception exc)
            {
                Exceptions.Exceptions.LogException(exc);
            }
        }
        public void UpdateFolder(FolderInfo objFolderInfo)
        {
            DataProvider.Instance().UpdateFolder(objFolderInfo.PortalID, objFolderInfo.FolderID, FileSystemUtils.FormatFolderPath(objFolderInfo.FolderPath), objFolderInfo.StorageLocation, objFolderInfo.IsProtected, objFolderInfo.IsCached, objFolderInfo.LastUpdated, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objFolderInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.FOLDER_UPDATED);
            DataCache.ClearFolderCache(objFolderInfo.PortalID);
        }      
    }
}
