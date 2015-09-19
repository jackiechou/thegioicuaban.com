using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommonLibrary.Modules;
using System.Net;
using System.Data;
using CommonLibrary.Data;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Host;
using System.IO;


namespace CommonLibrary.Services.FileSystem
{
    public class FileController
    {
        internal bool FileChanged(DataRow drOriginalFile, string NewFileName, string NewExtension, long NewSize, int NewWidth, int NewHeight, string NewContentType, string NewFolder)
        {
            if (Convert.ToString(drOriginalFile["FileName"]) != NewFileName || Convert.ToString(drOriginalFile["Extension"]) != NewExtension || Convert.ToInt32(drOriginalFile["Size"]) != NewSize | Convert.ToInt32(drOriginalFile["Width"]) != NewWidth | Convert.ToInt32(drOriginalFile["Height"]) != NewHeight | Convert.ToString(drOriginalFile["ContentType"]) != NewContentType | Convert.ToString(drOriginalFile["Folder"]) != NewFolder)
            {
                return true;
            }
            return false;
        }
        public int AddFile(FileInfo file)
        {
            return AddFile(file.PortalId, file.FileName, file.Extension, file.Size, file.Width, file.Height, file.ContentType, file.Folder, file.FolderId, true);
        }
        public int AddFile(int PortalId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string FolderPath, int FolderID, bool ClearCache)
        {
            FolderPath = FileSystemUtils.FormatFolderPath(FolderPath);
            int FileId = DataProvider.Instance().AddFile(PortalId, FileName, Extension, Size, Width, Height, ContentType, FolderPath, FolderID);
            DataCache.RemoveCache("GetFileById" + FileId.ToString());
            if (ClearCache)
            {
                GetAllFilesRemoveCache();
            }
            return FileId;
        }
        public void ClearFileContent(int FileId)
        {
            DataProvider.Instance().UpdateFileContent(FileId, null);
        }
        public int ConvertFilePathToFileId(string FilePath, int PortalID)
        {
            string FileName = "";
            string FolderName = "";
            int FileId = -1;
            if (!String.IsNullOrEmpty(FilePath))
            {
                FileName = FilePath.Substring(FilePath.LastIndexOf("/") + 1);
                FolderName = FilePath.Replace(FileName, "");
            }
            FileController objFiles = new FileController();
            FolderController objFolders = new FolderController();
            FolderInfo objFolder = objFolders.GetFolder(PortalID, FolderName, false);
            if (objFolder != null)
            {
                FileInfo objFile = objFiles.GetFile(FileName, PortalID, objFolder.FolderID);
                if (objFile != null)
                {
                    FileId = objFile.FileId;
                }
            }
            return FileId;
        }
        public void DeleteFile(int PortalId, string FileName, int FolderID, bool ClearCache)
        {
            DataProvider.Instance().DeleteFile(PortalId, FileName, FolderID);
            if (ClearCache)
            {
                GetAllFilesRemoveCache();
            }
        }
        public void DeleteFiles(int PortalId)
        {
            DeleteFiles(PortalId, true);
        }
        public void DeleteFiles(int PortalId, bool ClearCache)
        {
            DataProvider.Instance().DeleteFiles(PortalId);
            if (ClearCache)
            {
                GetAllFilesRemoveCache();
            }
        }
        public DataTable GetAllFiles()
        {
            DataTable dt = (DataTable)DataCache.GetCache("GetAllFiles");
            if (dt == null)
            {
                dt = DataProvider.Instance().GetAllFiles();
                DataCache.SetCache("GetAllFiles", dt);
            }
            if (dt != null)
            {
                return dt.Copy();
            }
            else
            {
                return new DataTable();
            }
        }
        public void GetAllFilesRemoveCache()
        {
            DataCache.RemoveCache("GetAllFiles");
        }
        public FileInfo GetFile(string FileName, int PortalId, int FolderID)
        {
            return (FileInfo)CBO.FillObject(DataProvider.Instance().GetFile(FileName, PortalId, FolderID), typeof(FileInfo));
        }
        public FileInfo GetFileById(int FileId, int PortalId)
        {
            FileInfo objFile;
            string strCacheKey = "GetFileById" + FileId.ToString();
            objFile = (FileInfo)DataCache.GetCache(strCacheKey);
            if (objFile == null)
            {
                objFile = (FileInfo)CBO.FillObject(DataProvider.Instance().GetFileById(FileId, PortalId), typeof(FileInfo));
                if (objFile != null)
                {
                    int intCacheTimeout = 20 * Convert.ToInt32(Host.PerformanceSetting);
                    DataCache.SetCache(strCacheKey, objFile, TimeSpan.FromMinutes(intCacheTimeout));
                }
            }
            return objFile;
        }
        public byte[] GetFileContent(int FileId, int PortalId)
        {
            byte[] objContent = null;
            IDataReader dr = null;
            try
            {
                dr = DataProvider.Instance().GetFileContent(FileId, PortalId);
                if (dr.Read())
                {
                    objContent = System.Text.Encoding.Default.GetBytes((dr["Content"]).ToString());
                }
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return objContent;
        }
        public IDataReader GetFiles(int PortalId, int FolderID)
        {
            return DataProvider.Instance().GetFiles(PortalId, FolderID);
        }
        public void UpdateFile(int FileId, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string DestinationFolder, int FolderID)
        {
            DataProvider.Instance().UpdateFile(FileId, FileName, Extension, Size, Width, Height, ContentType, FileSystemUtils.FormatFolderPath(DestinationFolder), FolderID);
            DataCache.RemoveCache("GetFileById" + FileId.ToString());
        }
        public void UpdateFileContent(int FileId, Stream Content)
        {
            BinaryReader objBinaryReader = new BinaryReader(Content);
            byte[] objContent = objBinaryReader.ReadBytes(Convert.ToInt32(Content.Length));
            objBinaryReader.Close();
            Content.Close();
            DataProvider.Instance().UpdateFileContent(FileId, objContent);
        }
        public void UpdateFileContent(int FileId, byte[] Content)
        {
            DataProvider.Instance().UpdateFileContent(FileId, Content);
        }
    }
}
