using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Host;
using System.Web;
using CommonLibrary.Services.Localization;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Security.Permissions;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Services.FileSystem;

namespace CommonLibrary.Common.Utilities
{
    public static class FileSystemUtils
    {
        #region "Enums"
        private enum EnumUserFolderElement
        {
            Root = 0,
            SubFolder = 1
        }
        #endregion

        private static string AddFile(int PortalId, Stream inStream, string fileName, string contentType, long length, string folderName, bool closeInputStream, bool clearCache)
        {
            return AddFile(PortalId, inStream, fileName, contentType, length, folderName, closeInputStream, clearCache, false);
        }
        private static string AddFile(int PortalId, Stream inStream, string fileName, string contentType, long length, string folderName, bool closeInputStream, bool clearCache, bool synchronize)
        {
            CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
            CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
            string sourceFolderName = Globals.GetSubFolderPath(fileName, PortalId);
            CommonLibrary.Services.FileSystem.FolderInfo folder = objFolderController.GetFolder(PortalId, sourceFolderName, false);
            string sourceFileName = GetFileName(fileName);
            int intFileID;
            string retValue = "";
            retValue += CheckValidFileName(fileName);
            if (!String.IsNullOrEmpty(retValue))
            {
                return retValue;
            }
            string extension = Path.GetExtension(fileName).Replace(".", "");
            if (String.IsNullOrEmpty(contentType))
            {
                contentType = GetContentType(extension);
            }
            intFileID = objFileController.AddFile(PortalId, sourceFileName, extension, length, Null.NullInteger, Null.NullInteger, contentType, folderName, folder.FolderID, clearCache);
            if (folder.StorageLocation != (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem || synchronize == false)
            {
                WriteStream(intFileID, inStream, fileName, folder.StorageLocation, closeInputStream);
            }
            retValue += UpdateFileData(intFileID, folder.FolderID, PortalId, sourceFileName, extension, contentType, length, folderName);
            if (folder.StorageLocation != (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem)
            {
                DeleteFile(fileName);
            }
            if (folder.StorageLocation != (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.SecureFileSystem)
            {
                if (!fileName.EndsWith(".template"))
                {
                    DeleteFile(fileName + Globals.glbProtectedExtension);
                }
            }
            return retValue;
        }
        private static int AddFolder(int PortalId, string relativePath, int StorageLocation)
        {
            CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
            bool isProtected = FileSystemUtils.DefaultProtectedFolders(relativePath);
            int FolderID = objFolderController.AddFolder(PortalId, relativePath, StorageLocation, isProtected, false);
            if (PortalId != Null.NullInteger)
            {
                SetFolderPermissions(PortalId, FolderID, relativePath);
            }
            return FolderID;
        }
        public static string CheckValidFileName(string strFileName)
        {
            string retValue = Null.NullString;
            string strExtension = Path.GetExtension(strFileName).Replace(".", "");
            string strWhiteList = Host.FileExtensions.ToLowerInvariant();
            if (string.IsNullOrEmpty(strExtension) || ("," + strWhiteList + ",").IndexOf("," + strExtension.ToLowerInvariant() + ",") == -1)
            {
                if (HttpContext.Current != null)
                {
                    retValue = "<br>" + string.Format(Localization.GetString("RestrictedFileType"), strFileName, strWhiteList.Replace(",", ", *."));
                }
                else
                {
                    retValue = "RestrictedFileType";
                }
            }
            return retValue;
        }
        private static string CreateFile(string RootPath, string FileName, long ContentLength, string ContentType, Stream InputStream, string NewFileName, bool Unzip)
        {
            // Obtain PortalSettings from Current Context
            PortalSettings settings = PortalController.GetCurrentPortalSettings();
            int PortalId = GetFolderPortalId(settings);
            bool isHost = (bool)(settings.ActiveTab.ParentId == settings.SuperTabId ? true : false);

            PortalController objPortalController = new PortalController();
            string strMessage = "";
            string strFileName = FileName;
            if (NewFileName != Null.NullString) strFileName = NewFileName;
            strFileName = RootPath + Path.GetFileName(strFileName);
            string strExtension = Path.GetExtension(strFileName).Replace(".", "");
            string strFolderpath = Globals.GetSubFolderPath(strFileName, PortalId);

            CommonLibrary.Services.FileSystem.FolderController objFolders = new CommonLibrary.Services.FileSystem.FolderController();
            CommonLibrary.Services.FileSystem.FolderInfo objFolder = objFolders.GetFolder(PortalId, strFolderpath, false);

            if (FolderPermissionController.CanAdminFolder(objFolder))
            {
                if (objPortalController.HasSpaceAvailable(PortalId, ContentLength))
                {
                    if (("," + Host.FileExtensions.ToUpper()).IndexOf("," + strExtension.ToUpper()) != -1 || isHost)
                    {
                        //Save Uploaded file to server
                        try
                        {
                            strMessage += AddFile(PortalId, InputStream, strFileName, ContentType, ContentLength, strFolderpath, true, true);

                            //Optionally Unzip File?
                            if (Path.GetExtension(strFileName).ToLower() == ".zip" & Unzip == true)
                            {
                                strMessage += UnzipFile(strFileName, RootPath, settings);
                            }
                        }
                        catch (Exception exc)
                        {
                            // save error - can happen if the security settings are incorrect on the disk
                            strMessage += "<br>" + string.Format(Localization.GetString("SaveFileError"), strFileName);
                            exc.ToString();
                        }
                    }
                    else
                    {
                        // restricted file type
                        strMessage += "<br>" + string.Format(Localization.GetString("RestrictedFileType"), strFileName, Host.FileExtensions.Replace(",", ", *."));
                    }
                }
                else
                {
                    // file too large
                    strMessage += "<br>" + string.Format(Localization.GetString("DiskSpaceExceeded"), strFileName);
                }
            }
            else
            {
                // insufficient folder permission in the application
                strMessage += "<br>" + string.Format(Localization.GetString("InsufficientFolderPermission"), strFolderpath);
            }

            return strMessage;
        }
        private static string GetFileName(string filePath)
        {
            return System.IO.Path.GetFileName(filePath).Replace(Globals.glbProtectedExtension, "");
        }
        private static void RemoveOrphanedFiles(CommonLibrary.Services.FileSystem.FolderInfo folder, int PortalId)
        {
            CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
            if (folder.StorageLocation != (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.DatabaseSecure)
            {
                foreach (CommonLibrary.Services.FileSystem.FileInfo objFile in GetFilesByFolder(PortalId, folder.FolderID))
                {
                    RemoveOrphanedFile(objFile, PortalId);
                }
            }
        }
        private static bool ShouldSyncFolder(bool hideSystemFolders, DirectoryInfo dirinfo)
        {
            if (hideSystemFolders && (((dirinfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) || dirinfo.Name.StartsWith("_", StringComparison.CurrentCultureIgnoreCase)))
            {

                return false;
            }

            return true;
        }
        private static void RemoveOrphanedFile(CommonLibrary.Services.FileSystem.FileInfo objFile, int PortalId)
        {
            CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
            string strFile = "";
            switch (objFile.StorageLocation)
            {
                case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem:
                    strFile = objFile.PhysicalPath;
                    break;
                case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.SecureFileSystem:
                    strFile = objFile.PhysicalPath + Globals.glbProtectedExtension;
                    break;
            }
            if (!String.IsNullOrEmpty(strFile))
            {
                if (!File.Exists(strFile))
                {
                    objFileController.DeleteFile(PortalId, objFile.FileName, objFile.FolderId, true);
                }
            }
        }
        private static string UpdateFile(string strSourceFile, string strDestFile, int PortalId, bool isCopy, bool isNew, bool ClearCache)
        {
            string retValue = "";
            retValue += CheckValidFileName(strSourceFile) + " ";
            retValue += CheckValidFileName(strDestFile);
            if (retValue.Length > 1)
            {
                return retValue;
            }
            retValue = "";
            Stream sourceStream = null;
            try
            {
                CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
                CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
                string sourceFolderName = Globals.GetSubFolderPath(strSourceFile, PortalId);
                string sourceFileName = GetFileName(strSourceFile);
                CommonLibrary.Services.FileSystem.FolderInfo sourceFolder = objFolderController.GetFolder(PortalId, sourceFolderName, false);
                string destFileName = GetFileName(strDestFile);
                string destFolderName = Globals.GetSubFolderPath(strDestFile, PortalId);
                CommonLibrary.Services.FileSystem.FileInfo file;
                if (sourceFolder != null)
                {
                    file = objFileController.GetFile(sourceFileName, PortalId, sourceFolder.FolderID);
                    if (file != null)
                    {
                        sourceStream = (Stream)GetFileStream(file);
                        if (isCopy)
                        {
                            AddFile(PortalId, sourceStream, strDestFile, "", file.Size, destFolderName, true, ClearCache);
                        }
                        else
                        {
                            CommonLibrary.Services.FileSystem.FolderInfo destinationFolder = objFolderController.GetFolder(PortalId, destFolderName, false);
                            if (destinationFolder != null)
                            {
                                objFileController.UpdateFile(file.FileId, destFileName, file.Extension, file.Size, file.Width, file.Height, file.ContentType, destFolderName, destinationFolder.FolderID);
                                WriteStream(file.FileId, sourceStream, strDestFile, destinationFolder.StorageLocation, true);
                                if (sourceFolder.StorageLocation == (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem)
                                {
                                    DeleteFile(strSourceFile);
                                }
                                if (sourceFolder.StorageLocation == (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.SecureFileSystem)
                                {
                                    DeleteFile(strSourceFile + Globals.glbProtectedExtension);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = ex.Message;
            }
            finally
            {
                if (sourceStream != null)
                {
                    sourceStream.Close();
                    sourceStream.Dispose();
                }
            }
            return retValue;
        }
        private static string UpdateFileData(int fileID, int folderID, int PortalId, string fileName, string extension, string contentType, long length, string folderName)
        {
            string retvalue = "";
            try
            {
                CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
                int imageWidth = 0;
                int imageHeight = 0;
                if ((Globals.glbImageFileTypes + ",").IndexOf(extension.ToLower() + ",") > -1)
                {
                    System.Drawing.Image imgImage = null;
                    Stream imageStream = null;
                    try
                    {
                        CommonLibrary.Services.FileSystem.FileInfo objFile = objFileController.GetFileById(fileID, PortalId);
                        imageStream = GetFileStream(objFile);
                        imgImage = System.Drawing.Image.FromStream(imageStream);
                        imageHeight = imgImage.Height;
                        imageWidth = imgImage.Width;
                    }
                    catch
                    {
                        contentType = "application/octet-stream";
                    }
                    finally
                    {
                        if (imgImage != null)
                        {
                            imgImage.Dispose();
                        }
                        if (imageStream != null)
                        {
                            imageStream.Close();
                            imageStream.Dispose();
                        }
                        objFileController.UpdateFile(fileID, fileName, extension, length, imageWidth, imageHeight, contentType, folderName, folderID);
                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message;
            }
            return retvalue;
        }
        private static void WriteStream(int fileId, Stream inStream, string fileName, int storageLocation, bool closeInputStream)
        {
            CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
            byte[] arrData = new byte[2048];
            Stream outStream = null;
            try
            {
                switch (storageLocation)
                {
                    case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.DatabaseSecure:
                        objFileController.ClearFileContent(fileId);
                        outStream = new MemoryStream();
                        break;
                    case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.SecureFileSystem:
                        if (File.Exists(fileName + Globals.glbProtectedExtension) == true)
                        {
                            File.Delete(fileName + Globals.glbProtectedExtension);
                        }
                        outStream = new FileStream(fileName + Globals.glbProtectedExtension, FileMode.Create);
                        break;
                    case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem:
                        if (File.Exists(fileName) == true)
                        {
                            File.Delete(fileName);
                        }
                        outStream = new FileStream(fileName, FileMode.Create);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (inStream != null && closeInputStream)
                {
                    inStream.Close();
                    inStream.Dispose();
                }
                if (outStream != null)
                {
                    outStream.Close();
                    outStream.Dispose();
                }
                throw ex;
            }
            try
            {
                int intLength;
                intLength = inStream.Read(arrData, 0, arrData.Length);
                while (intLength > 0)
                {
                    outStream.Write(arrData, 0, intLength);
                    intLength = inStream.Read(arrData, 0, arrData.Length);
                }
                if (storageLocation == (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.DatabaseSecure)
                {
                    outStream.Seek(0, SeekOrigin.Begin);
                    objFileController.UpdateFileContent(fileId, outStream);
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                if (inStream != null == false && closeInputStream)
                {
                    inStream.Close();
                    inStream.Dispose();
                }
                if (outStream != null)
                {
                    outStream.Close();
                    outStream.Dispose();
                }
            }
        }
        private static void WriteStream(HttpResponse objResponse, Stream objStream)
        {
            byte[] bytBuffer = new byte[10000];
            int intLength;
            long lngDataToRead;
            try
            {
                lngDataToRead = objStream.Length;
                while (lngDataToRead > 0)
                {
                    if (objResponse.IsClientConnected)
                    {
                        intLength = objStream.Read(bytBuffer, 0, 10000);
                        objResponse.OutputStream.Write(bytBuffer, 0, intLength);
                        objResponse.Flush();

                        lngDataToRead = lngDataToRead - intLength;
                    }
                    else
                    {
                        lngDataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.Write("Error : " + ex.Message);
            }
            finally
            {
                if (objStream != null)
                {
                    objStream.Close();
                    objStream.Dispose();
                }
            }
        }
        public static string AddTrailingSlash(string strSource)
        {
            if (!strSource.EndsWith("\\"))
                strSource = strSource + "\\";
            return strSource;
        }
        public static string RemoveTrailingSlash(string strSource)
        {
            if (String.IsNullOrEmpty(strSource))
                return "";
            if (strSource.Substring(strSource.Length - 1, 1) == "\\" || strSource.Substring(strSource.Length - 1, 1) == "/")
            {
                return strSource.Substring(0, strSource.Length - 1);
            }
            else
            {
                return strSource;
            }
        }
        public static string StripFolderPath(string strOrigPath)
        {
            if (strOrigPath.IndexOf("\\") != -1)
            {
                return Regex.Replace(strOrigPath, "^0\\\\", "");
            }
            else
            {
                return strOrigPath.StartsWith("0") ? strOrigPath.Substring(1) : strOrigPath;
            }
        }
        public static string FormatFolderPath(string strFolderPath)
        {
            if (String.IsNullOrEmpty(strFolderPath))
            {
                return "";
            }
            else
            {
                if (strFolderPath.EndsWith("/"))
                {
                    return strFolderPath;
                }
                else
                {
                    return strFolderPath + "/";
                }
            }
        }

         //<summary>
         //The MapPath method maps the specified relative or virtual path to the corresponding physical directory on the server.
         //</summary>
         //<param name="path">Specifies the relative or virtual path to map to a physical directory. If Path starts with either 
         //a forward (/) or backward slash (\), the MapPath method returns a path as if Path were a full, virtual path. If Path 
         //doesn't start with a slash, the MapPath method returns a path relative to the directory of the .asp file being processed</param>
         //<returns></returns>
         //<remarks>If path is a null reference (Nothing in Visual Basic), then the MapPath method returns the full physical path 
         //of the directory that contains the current application</remarks>
        public static string MapPath(string path)
        {
            string convertedPath = path;
            if (Globals.ApplicationPath.Length > 1 && convertedPath.StartsWith(Globals.ApplicationPath))
            {
                convertedPath = convertedPath.Substring(Globals.ApplicationPath.Length);
            }
            convertedPath = convertedPath.Replace("/", "\\");

            if (path.StartsWith("~") | path.StartsWith(".") | path.StartsWith("/"))
            {
                string appMapPath = Globals.ApplicationMapPath;
                if (convertedPath.Length > 1)
                {
                    convertedPath = string.Concat(AddTrailingSlash(Globals.ApplicationMapPath), convertedPath.Substring(1));
                }
                else
                {
                    convertedPath = Globals.ApplicationMapPath;
                }
            }
            convertedPath = System.IO.Path.GetFullPath(convertedPath);

            if (!convertedPath.StartsWith(Globals.ApplicationMapPath))
            {
                throw new System.Web.HttpException();
            }

            return convertedPath;
        }

        public static void AddFile(string FileName, int PortalId, string Folder, string HomeDirectoryMapPath, string contentType)
        {
            string strFile = HomeDirectoryMapPath + Folder + FileName;
            CommonLibrary.Services.FileSystem.FileController objFiles = new CommonLibrary.Services.FileSystem.FileController();
            System.IO.FileInfo finfo = new System.IO.FileInfo(strFile);
            CommonLibrary.Services.FileSystem.FolderController objFolders = new CommonLibrary.Services.FileSystem.FolderController();
            CommonLibrary.Services.FileSystem.FolderInfo objFolder = objFolders.GetFolder(PortalId, Folder, false);
            CommonLibrary.Services.FileSystem.FileInfo objFile;
            objFile = objFiles.GetFile(FileName, PortalId, objFolder.FolderID);
            if (objFile == null)
            {
                objFiles.AddFile(PortalId, FileName, finfo.Extension, finfo.Length, 0, 0, contentType, "", objFolder.FolderID, true);
            }
            else
            {
                objFiles.UpdateFile(objFile.FileId, objFile.FileName, finfo.Extension, finfo.Length, 0, 0, contentType, "", objFolder.FolderID);
            }
        }
        public static string AddFile(string strFile, int PortalId, bool ClearCache, CommonLibrary.Services.FileSystem.FolderInfo folder)
        {
            string retValue = "";
            try
            {
                CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
                System.IO.FileInfo fInfo = new System.IO.FileInfo(strFile);
                string sourceFolderName = Globals.GetSubFolderPath(strFile, PortalId);
                string sourceFileName;
                if (folder.StorageLocation == (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.SecureFileSystem)
                {
                    sourceFileName = GetFileName(strFile);
                }
                else
                {
                    sourceFileName = strFile;
                }
                CommonLibrary.Services.FileSystem.FileInfo file = objFileController.GetFile(sourceFileName, PortalId, folder.FolderID);
                if (file == null)
                {
                    FileStream fileStrm = null;
                    try
                    {
                        fileStrm = fInfo.OpenRead();
                        AddFile(PortalId, fileStrm, strFile, "", fInfo.Length, sourceFolderName, true, ClearCache, true);
                    }
                    finally
                    {
                        if (fileStrm != null)
                        {
                            fileStrm.Close();
                            fileStrm.Dispose();
                        }
                    }
                }
                else
                {
                    if (file.Size != fInfo.Length)
                    {
                        string extension = Path.GetExtension(strFile).Replace(".", "");
                        UpdateFileData(file.FileId, folder.FolderID, PortalId, sourceFileName, extension, GetContentType(extension), fInfo.Length, sourceFolderName);
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = ex.Message;
            }
            return retValue;
        }
        public static void AddFolder(PortalSettings _PortalSettings, string parentFolder, string newFolder)
        {
            AddFolder(_PortalSettings, parentFolder, newFolder, (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem);
        }
        public static void AddFolder(PortalSettings _PortalSettings, string parentFolder, string newFolder, int StorageLocation)
        {
            int PortalId;
            string ParentFolderName;
            System.IO.DirectoryInfo dinfo = new System.IO.DirectoryInfo(parentFolder);
            System.IO.DirectoryInfo dinfoNew;
            if (_PortalSettings.ActiveTab.ParentId == _PortalSettings.SuperTabId)
            {
                PortalId = Null.NullInteger;
                ParentFolderName = Common.Globals.HostMapPath;
            }
            else
            {
                PortalId = _PortalSettings.PortalId;
                ParentFolderName = _PortalSettings.HomeDirectoryMapPath;
            }
            dinfoNew = new System.IO.DirectoryInfo(parentFolder + newFolder);
            if (!dinfoNew.Exists)
            {
                dinfoNew = dinfo.CreateSubdirectory(newFolder);
            }
            string FolderPath = dinfoNew.FullName.Substring(ParentFolderName.Length).Replace("\\", "/");
            AddFolder(PortalId, FolderPath, StorageLocation);
        }
         //-----------------------------------------------------------------------------
         //<summary>
         //Creates a User Folder
         //</summary>
         //<param name="_PortalSettings">Portal Settings for the Portal</param>
         //<param name="parentFolder">The Parent Folder Name</param>
         //<param name="UserID">The UserID, in order to generate the path/foldername</param>
         //<param name="StorageLocation">The Storage Location</param>
         //<remarks>
         //</remarks>
         //<history>
         //   [jlucarino]	02/26/2010	Created
         //</history>
         //-----------------------------------------------------------------------------
        public static void AddUserFolder(PortalSettings _PortalSettings, string parentFolder, int StorageLocation, int UserID)
        {
            int PortalId = 0;
            string ParentFolderName = null;
            System.IO.DirectoryInfo dinfoNew = default(System.IO.DirectoryInfo);
            string RootFolder = "";
            string SubFolder = "";

            PortalId = _PortalSettings.PortalId;
            ParentFolderName = _PortalSettings.HomeDirectoryMapPath;

            RootFolder = GetUserFolderPathElement(UserID, EnumUserFolderElement.Root);
            SubFolder = GetUserFolderPathElement(UserID, EnumUserFolderElement.SubFolder);

            //create root folder
            string folderPath = "";
            folderPath = System.IO.Path.Combine(Path.Combine(ParentFolderName, "Users"), RootFolder);
            dinfoNew = new System.IO.DirectoryInfo(folderPath);
            if (!dinfoNew.Exists)
            {
                dinfoNew.Create();
                AddFolder(PortalId, folderPath.Substring(ParentFolderName.Length).Replace("\\", "/"), StorageLocation);
            }

            //create two-digit subfolder
            folderPath = System.IO.Path.Combine(folderPath, SubFolder);
            dinfoNew = new System.IO.DirectoryInfo(folderPath);
            if (!dinfoNew.Exists)
            {
                dinfoNew.Create();
                AddFolder(PortalId, folderPath.Substring(ParentFolderName.Length).Replace("\\", "/"), StorageLocation);
            }

            //create folder from UserID
            folderPath = System.IO.Path.Combine(folderPath, UserID.ToString());
            dinfoNew = new System.IO.DirectoryInfo(folderPath);
            if (!dinfoNew.Exists)
            {
                dinfoNew.Create();
                int folderID = AddFolder(PortalId, folderPath.Substring(ParentFolderName.Length).Replace("\\", "/"), StorageLocation);

                //Give user Read Access to this folder
                CommonLibrary.Services.FileSystem.FolderInfo folder = new CommonLibrary.Services.FileSystem.FolderController().GetFolderInfo(PortalId, folderID);
                foreach (PermissionInfo permission in PermissionController.GetPermissionsByFolder())
                {
                    if (permission.PermissionKey.ToUpper() == "READ" || permission.PermissionKey.ToUpper() == "WRITE")
                    {
                        FolderPermissionInfo folderPermission = new FolderPermissionInfo(permission);
                        folderPermission.FolderID = folder.FolderID;
                        folderPermission.UserID = UserID;
                        folderPermission.RoleID = Null.NullInteger;
                        folderPermission.AllowAccess = true;

                        folder.FolderPermissions.Add(folderPermission);
                    }
                }

                FolderPermissionController.SaveFolderPermissions(folder);

            }
        }

         //-----------------------------------------------------------------------------
         //<summary>
         //Returns path to a User Folder 
         //</summary>
         //<history>
         //   [jlucarino]	03/01/2010	Created
         //</history>
         //-----------------------------------------------------------------------------
        public static string GetUserFolderPath(int UserID)
        {
            string RootFolder = null;
            string SubFolder = null;
            string FullPath = null;

            RootFolder = GetUserFolderPathElement(UserID, EnumUserFolderElement.Root);
            SubFolder = GetUserFolderPathElement(UserID, EnumUserFolderElement.SubFolder);

            FullPath = System.IO.Path.Combine(RootFolder, SubFolder);
            FullPath = System.IO.Path.Combine(FullPath, UserID.ToString());


            return FullPath;
        }

         //-----------------------------------------------------------------------------
         //<summary>
         //Returns Root and SubFolder elements of User Folder path
         //</summary>
         //<history>
         //   [jlucarino]	03/01/2010	Created
         //</history>
         //-----------------------------------------------------------------------------
        private static string GetUserFolderPathElement(int UserID, EnumUserFolderElement Mode)
        {
            const int SUBFOLDER_SEED_LENGTH = 2;
            const int BYTE_OFFSET = 255;
            string Element = "";

            switch (Mode)
            {
                case EnumUserFolderElement.Root:
                    Element = (Convert.ToInt32(UserID) & BYTE_OFFSET).ToString("000");

                    break;
                case EnumUserFolderElement.SubFolder:
                    Element = UserID.ToString("00").Substring(UserID.ToString("00").Length - SUBFOLDER_SEED_LENGTH, SUBFOLDER_SEED_LENGTH);

                    break;
            }


            return Element;
        }

        public static void AddToZip(ref ZipOutputStream ZipFile, string filePath, string fileName, string folder)
        {
            Crc32 crc = new Crc32();
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(filePath);
                byte[] buffer = new byte[fs.Length];

                fs.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(Path.Combine(folder, fileName));
                entry.DateTime = DateTime.Now;
                entry.Size = fs.Length;
                fs.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                ZipFile.PutNextEntry(entry);
                ZipFile.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        public static void CopyFile(string sourceFileName, string destFileName)
        {
            if (File.Exists(destFileName))
            {
                File.SetAttributes(destFileName, FileAttributes.Normal);
            }
            File.Copy(sourceFileName, destFileName, true);
        }
        public static string CopyFile(string strSourceFile, string strDestFile, PortalSettings settings)
        {
            return UpdateFile(strSourceFile, strDestFile, GetFolderPortalId(settings), true, false, true);
        }
         //-----------------------------------------------------------------------------
         //<summary>
         //UploadFile pocesses a single file 
         //</summary>
         //<param name="RootPath">The folder where the file will be put</param>
         //<param name="FileName">The file name</param>
         //<param name="FileData">Content of the file</param>
         //<param name="ContentType">Type of content, ie: text/html</param>
         //<param name="NewFileName"></param>
         //<param name="Unzip"></param> 
         //<remarks>
         //</remarks>
         //<history>
         //    [cnurse]        16/9/2004   Updated for localization, Help and 508
         //    [Philip Beadle] 10/06/2004  Moved to Globals from WebUpload.ascx.vb so can be accessed by URLControl.ascx
         //    [cnurse]        04/26/2006  Updated for Secure Storage
         //    [sleupold]      08/14/2007  Added NewFileName
         //    [sdarkis]       10/19/2009  Creates a file from a string
         //</history>
        // -----------------------------------------------------------------------------
        public static string CreateFileFromString(string RootPath, string FileName, string FileData, string ContentType, string NewFileName, bool Unzip)
        {
            string returnValue = string.Empty;
            MemoryStream memStream = null;

            try
            {
                memStream = new MemoryStream();
                byte[] fileDataBytes = System.Text.Encoding.ASCII.GetBytes(FileData);
                memStream.Write(fileDataBytes, 0, fileDataBytes.Length);
                memStream.Flush();
                memStream.Position = 0;

                returnValue = CreateFile(RootPath, FileName, memStream.Length, ContentType, memStream, NewFileName, Unzip);
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            finally
            {
                if (((memStream != null)))
                {
                    memStream.Close();
                    memStream.Dispose();
                }
            }

            return returnValue;
        }

        public static bool DefaultProtectedFolders(string folderPath)
        {
            if (String.IsNullOrEmpty(folderPath) || folderPath.ToLower() == "skins" || folderPath.ToLower() == "containers" || folderPath.ToLower().StartsWith("skins/") == true || folderPath.ToLower().StartsWith("containers/") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool DeleteFileWithWait(string filename, Int16 waitInMilliseconds, Int16 maxAttempts)
        {
            if (!File.Exists(filename))
            {
                return true;
            }
            bool fileDeleted = false;
            int i = 0;
            while (fileDeleted != true)
            {
                if (i > maxAttempts)
                    break;
                i = i + 1;
                try
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    fileDeleted = true;
                }
                catch (Exception ex)
                {
                    fileDeleted = false;
                    ex.ToString();
                }
                if (fileDeleted == false)
                {
                    System.Threading.Thread.Sleep(waitInMilliseconds);
                }
            }
            return fileDeleted;
        }
        public static void DeleteFile(string strFileName)
        {
            if (File.Exists(strFileName))
            {
                File.SetAttributes(strFileName, FileAttributes.Normal);
                File.Delete(strFileName);
            }
        }
        public static string DeleteFile(string strSourceFile, PortalSettings settings)
        {
            return DeleteFile(strSourceFile, settings, true);
        }
        public static string DeleteFile(string strSourceFile, PortalSettings settings, bool ClearCache)
        {
            string retValue = "";
            int PortalId = GetFolderPortalId(settings);
            string folderName = Globals.GetSubFolderPath(strSourceFile, PortalId);
            string fileName = GetFileName(strSourceFile);
            CommonLibrary.Services.FileSystem.FolderController objFolders = new CommonLibrary.Services.FileSystem.FolderController();
            CommonLibrary.Services.FileSystem.FolderInfo objFolder = objFolders.GetFolder(PortalId, folderName, false);
            if (FolderPermissionController.CanAdminFolder(objFolder))
            {
                try
                {
                    DeleteFile(strSourceFile);
                    DeleteFile(strSourceFile + Globals.glbProtectedExtension);
                    CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
                    objFileController.DeleteFile(PortalId, fileName, objFolder.FolderID, ClearCache);
                }
                catch (IOException ioEx)
                {
                    retValue += "<br>" + string.Format(Localization.GetString("FileInUse"), strSourceFile);
                    ioEx.ToString();
                }
                catch (Exception ex)
                {
                    retValue = ex.Message;
                }
            }
            else
            {
                retValue += "<br>" + string.Format(Localization.GetString("InsufficientFolderPermission"), folderName);
            }
            return retValue;
        }
        public static void DeleteFolder(int PortalId, System.IO.DirectoryInfo folder, string folderName)
        {
            folder.Delete(false);
            CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
            objFolderController.DeleteFolder(PortalId, folderName.Replace("\\", "/"));
        }
        public static void DownloadFile(string FileLoc)
        {
            System.IO.FileInfo objFile = new System.IO.FileInfo(FileLoc);
            System.Web.HttpResponse objResponse = System.Web.HttpContext.Current.Response;
            string filename = objFile.Name;
            if (HttpContext.Current.Request.UserAgent.IndexOf("; MSIE ") > 0)
            {
                filename = HttpUtility.UrlEncode(filename);
            }
            if (objFile.Exists)
            {
                objResponse.ClearContent();
                objResponse.ClearHeaders();
                objResponse.AppendHeader("content-disposition", "attachment; filename=\"" + filename + "\"");
                objResponse.AppendHeader("Content-Length", objFile.Length.ToString());
                objResponse.ContentType = GetContentType(objFile.Extension.Replace(".", ""));
                WriteFile(objFile.FullName);
                objResponse.Flush();
                objResponse.End();
            }
        }
        public static bool DownloadFile(PortalSettings settings, int FileId, bool ClientCache, bool ForceDownload)
        {
            return DownloadFile(GetFolderPortalId(settings), FileId, ClientCache, ForceDownload);
        }
        public static bool DownloadFile(int PortalId, int FileId, bool ClientCache, bool ForceDownload)
        {
            bool blnDownload = false;
            CommonLibrary.Services.FileSystem.FileController objFiles = new CommonLibrary.Services.FileSystem.FileController();
            CommonLibrary.Services.FileSystem.FileInfo objFile = objFiles.GetFileById(FileId, PortalId);
            if (objFile != null)
            {
                string filename = objFile.FileName;
                if (HttpContext.Current.Request.UserAgent.IndexOf("; MSIE ") > 0)
                {
                    filename = HttpUtility.UrlEncode(filename);
                }
                CommonLibrary.Services.FileSystem.FolderController objFolders = new CommonLibrary.Services.FileSystem.FolderController();
                CommonLibrary.Services.FileSystem.FolderInfo objFolder = objFolders.GetFolder(PortalId, objFile.Folder, false);
                if (FolderPermissionController.CanViewFolder(objFolder))
                {
                    bool blnFileExists = true;
                    if (Host.EnableFileAutoSync)
                    {
                        string strFile = "";
                        switch (objFile.StorageLocation)
                        {
                            case (int)FolderController.StorageLocationTypes.InsecureFileSystem:
                                strFile = objFile.PhysicalPath;
                                break;
                            case (int)FolderController.StorageLocationTypes.SecureFileSystem:
                                strFile = objFile.PhysicalPath + Globals.glbProtectedExtension;
                                break;
                        }
                        if (!String.IsNullOrEmpty(strFile))
                        {
                            System.IO.FileInfo objFileInfo = new System.IO.FileInfo(strFile);
                            if (objFileInfo.Exists)
                            {
                                if (objFile.Size != objFileInfo.Length)
                                {
                                    objFile.Size = Convert.ToInt32(objFileInfo.Length);
                                    UpdateFileData(FileId, objFile.FolderId, PortalId, objFile.FileName, objFile.Extension, GetContentType(objFile.Extension), objFileInfo.Length, objFile.Folder);
                                }
                            }
                            else
                            {
                                RemoveOrphanedFile(objFile, PortalId);
                                blnFileExists = false;
                            }
                        }
                    }
                    if (blnFileExists)
                    {
                        int scriptTimeOut = HttpContext.Current.Server.ScriptTimeout;
                        HttpContext.Current.Server.ScriptTimeout = int.MaxValue;
                        HttpResponse objResponse = HttpContext.Current.Response;
                        objResponse.ClearContent();
                        objResponse.ClearHeaders();
                        if (ForceDownload)
                        {
                            objResponse.AppendHeader("content-disposition", "attachment; filename=\"" + filename + "\"");
                        }
                        else
                        {
                            objResponse.AppendHeader("content-disposition", "inline; filename=\"" + filename + "\"");
                        }
                        objResponse.AppendHeader("Content-Length", objFile.Size.ToString());
                        objResponse.ContentType = GetContentType(objFile.Extension.Replace(".", ""));
                        System.IO.Stream objStream = null;
                        try
                        {
                            objStream = FileSystemUtils.GetFileStream(objFile);
                            WriteStream(objResponse, objStream);
                        }
                        catch (Exception ex)
                        {
                            objResponse.Write("Error : " + ex.Message);
                        }
                        finally
                        {
                            if (objStream != null)
                            {
                                objStream.Close();
                                objStream.Dispose();
                            }
                        }
                        objResponse.Flush();
                        objResponse.End();
                        HttpContext.Current.Server.ScriptTimeout = scriptTimeOut;
                        blnDownload = true;
                    }
                }
            }
            return blnDownload;
        }
        public static string GetContentType(string extension)
        {
            string contentType;
            switch (extension.ToLower())
            {
                case "txt":
                    contentType = "text/plain";
                    break;
                case "htm":
                case "html":
                    contentType = "text/html";
                    break;
                case "rtf":
                    contentType = "text/richtext";
                    break;
                case "jpg":
                case "jpeg":
                    contentType = "image/jpeg";
                    break;
                case "gif":
                    contentType = "image/gif";
                    break;
                case "bmp":
                    contentType = "image/bmp";
                    break;
                case "mpg":
                case "mpeg":
                    contentType = "video/mpeg";
                    break;
                case "avi":
                    contentType = "video/avi";
                    break;
                case "pdf":
                    contentType = "application/pdf";
                    break;
                case "doc":
                case "dot":
                    contentType = "application/msword";
                    break;
                case "csv":
                    contentType = "text/csv";
                    break;
                case "xls":
                case "xlt":
                    contentType = "application/x-msexcel";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            return contentType;
        }
        public static byte[] GetFileContent(CommonLibrary.Services.FileSystem.FileInfo objFile)
        {
            Stream objStream = null;
            byte[] objContent = null;
            try
            {
                objStream = FileSystemUtils.GetFileStream(objFile);
                BinaryReader objBinaryReader = new BinaryReader(objStream);
                objContent = objBinaryReader.ReadBytes(Convert.ToInt32(objStream.Length));
                objBinaryReader.Close();
            }
            finally
            {
                if (objStream != null)
                {
                    objStream.Close();
                    objStream.Dispose();
                }
            }
            return objContent;
        }
        public static Stream GetFileStream(CommonLibrary.Services.FileSystem.FileInfo objFile)
        {
            CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
            Stream fileStream = null;
            switch (objFile.StorageLocation)
            {
                case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem:
                    fileStream = new FileStream(objFile.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    break;
                case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.SecureFileSystem:
                    fileStream = new FileStream(objFile.PhysicalPath + Globals.glbProtectedExtension, FileMode.Open, FileAccess.Read, FileShare.Read);
                    break;
                case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.DatabaseSecure:
                    fileStream = new MemoryStream(objFileController.GetFileContent(objFile.FileId, objFile.PortalId));
                    break;
            }
            return fileStream;
        }
        public static ArrayList GetFilesByFolder(int PortalId, int folderId)
        {
            CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
            return CBO.FillCollection(objFileController.GetFiles(PortalId, folderId), typeof(CommonLibrary.Services.FileSystem.FileInfo));
        }
        public static int GetFolderPortalId(PortalSettings settings)
        {
            int FolderPortalId = Null.NullInteger;
            bool isHost = settings.ActiveTab.ParentId == settings.SuperTabId;
            if (!isHost)
            {
                FolderPortalId = settings.PortalId;
            }
            return FolderPortalId;
        }
        public static ArrayList GetFolders(int PortalID)
        {
            CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
            ArrayList arrFolders = new ArrayList();
            foreach (KeyValuePair<string, CommonLibrary.Services.FileSystem.FolderInfo> folderPair in objFolderController.GetFoldersSorted(PortalID))
            {
                arrFolders.Add(folderPair.Value);
            }
            return arrFolders;
        }
        public static CommonLibrary.Services.FileSystem.FolderInfo GetFolder(int PortalID, string FolderPath)
        {
            CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
            CommonLibrary.Services.FileSystem.FolderInfo objFolder = objFolderController.GetFolder(PortalID, FolderPath, false);
            if (Host.EnableFileAutoSync)
            {
                if (objFolder != null)
                {
                    SynchronizeFolder(objFolder.PortalID, objFolder.PhysicalPath, objFolder.FolderPath, false, true, false, false);
                }
            }
            return objFolder;
        }
        public static ArrayList GetFoldersByParentFolder(int PortalId, string ParentFolder)
        {
            ArrayList folders = GetFolders(PortalId);
            ArrayList subFolders = new ArrayList();
            foreach (CommonLibrary.Services.FileSystem.FolderInfo folder in folders)
            {
                string strfolderPath = folder.FolderPath;
                if (folder.FolderPath.IndexOf(ParentFolder) > -1 && folder.FolderPath != Null.NullString && folder.FolderPath != ParentFolder)
                {
                    if (ParentFolder == Null.NullString)
                    {
                        if (strfolderPath.IndexOf("/") == strfolderPath.LastIndexOf("/"))
                        {
                            subFolders.Add(folder);
                        }
                    }
                    else if (strfolderPath.StartsWith(ParentFolder))
                    {
                        strfolderPath = strfolderPath.Substring(ParentFolder.Length + 1);
                        if (strfolderPath.IndexOf("/") == strfolderPath.LastIndexOf("/"))
                        {
                            subFolders.Add(folder);
                        }
                    }
                }
            }
            return subFolders;
        }
        public static ArrayList GetFoldersByUser(int PortalID, bool IncludeSecure, bool IncludeDatabase, string Permissions)
        {
            CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
            ArrayList arrFolders = new ArrayList();
            foreach (CommonLibrary.Services.FileSystem.FolderInfo folder in objFolderController.GetFoldersSorted(PortalID).Values)
            {
                bool canAdd = true;
                switch (folder.StorageLocation)
                {
                    case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.DatabaseSecure:
                        canAdd = IncludeDatabase;
                        break;
                    case (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.SecureFileSystem:
                        canAdd = IncludeSecure;
                        break;
                }
                if (canAdd && PortalID > Null.NullInteger)
                {
                    canAdd = FolderPermissionController.HasFolderPermission(folder.FolderPermissions, Permissions);
                }
                if (canAdd)
                {
                    arrFolders.Add(folder);
                }
            }
            return arrFolders;
        }
        public static string MoveFile(string strSourceFile, string strDestFile, PortalSettings settings)
        {
            return UpdateFile(strSourceFile, strDestFile, GetFolderPortalId(settings), false, false, true);
        }
        public static string ReadFile(string filePath)
        {
            StreamReader objStreamReader = null;
            string fileContent = string.Empty;
            try
            {
                objStreamReader = File.OpenText(filePath);
                fileContent = objStreamReader.ReadToEnd();
            }
            finally
            {
                if (objStreamReader != null)
                {
                    objStreamReader.Close();
                    objStreamReader.Dispose();
                }
            }
            return fileContent;
        }
        public static void RemoveOrphanedFolders(int PortalId)
        {
            CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
            ArrayList arrFolders = GetFolders(PortalId);
            foreach (CommonLibrary.Services.FileSystem.FolderInfo objFolder in arrFolders)
            {
                if (objFolder.StorageLocation != (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.DatabaseSecure)
                {
                    if (Directory.Exists(objFolder.PhysicalPath) == false)
                    {
                        RemoveOrphanedFiles(objFolder, PortalId);
                        objFolderController.DeleteFolder(PortalId, objFolder.FolderPath);
                    }
                }
            }
        }
        public static void SaveFile(string FullFileName, byte[] Buffer)
        {
            if (System.IO.File.Exists(FullFileName))
            {
                System.IO.File.SetAttributes(FullFileName, FileAttributes.Normal);
            }
            FileStream fs = null;
            try
            {
                fs = new FileStream(FullFileName, FileMode.Create, FileAccess.Write);
                fs.Write(Buffer, 0, Buffer.Length);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
        public static void SetFileAttributes(string FileLoc, int FileAttributesOn)
        {
            System.IO.File.SetAttributes(FileLoc, (FileAttributes)FileAttributesOn);
        }
        public static void SetFolderPermissions(int PortalId, int FolderId, int AdministratorRoleId, string relativePath)
        {
            CommonLibrary.Services.FileSystem.FolderInfo folder = new CommonLibrary.Services.FileSystem.FolderController().GetFolderInfo(PortalId, FolderId);
            foreach (PermissionInfo objPermission in PermissionController.GetPermissionsByFolder())
            {
                FolderPermissionInfo folderPermission = new FolderPermissionInfo(objPermission);
                folderPermission.FolderID = FolderId;
                folderPermission.RoleID = AdministratorRoleId;
                folder.FolderPermissions.Add(folderPermission, true);
            }
            FolderPermissionController.SaveFolderPermissions(folder);
        }
        public static void SetFolderPermissions(int PortalId, int FolderId, string relativePath)
        {
            if (!String.IsNullOrEmpty(relativePath))
            {
                CommonLibrary.Services.FileSystem.FolderInfo folder = new CommonLibrary.Services.FileSystem.FolderController().GetFolderInfo(PortalId, FolderId);
                string parentFolderPath = relativePath.Substring(0, relativePath.Substring(0, relativePath.Length - 1).LastIndexOf("/") + 1);
                foreach (FolderPermissionInfo objPermission in FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalId, parentFolderPath))
                {
                    FolderPermissionInfo folderPermission = new FolderPermissionInfo(objPermission);
                    folderPermission.FolderID = FolderId;
                    folderPermission.RoleID = objPermission.RoleID;
                    folderPermission.UserID = objPermission.UserID;
                    folderPermission.AllowAccess = objPermission.AllowAccess;
                    folder.FolderPermissions.Add(folderPermission, true);
                }
                FolderPermissionController.SaveFolderPermissions(folder);
            }
        }
        public static void SetFolderPermission(int PortalId, int FolderId, int PermissionId, int RoleId, string relativePath)
        {
            SetFolderPermission(PortalId, FolderId, PermissionId, RoleId, Null.NullInteger, relativePath);
        }
        public static void SetFolderPermission(int PortalId, int FolderId, int PermissionId, int RoleId, int UserId, string relativePath)
        {
            FolderPermissionInfo objFolderPermissionInfo;
            CommonLibrary.Services.FileSystem.FolderController objController = new CommonLibrary.Services.FileSystem.FolderController();
            CommonLibrary.Services.FileSystem.FolderInfo folder = objController.GetFolderInfo(PortalId, FolderId);
            foreach (FolderPermissionInfo fpi in folder.FolderPermissions)
            {
                if (fpi.FolderID == FolderId
                    && fpi.PermissionID == PermissionId
                    && fpi.RoleID == RoleId
                    && fpi.UserID == UserId
                    && fpi.AllowAccess == true)
                {
                    return;
                }
            }
            objFolderPermissionInfo = new FolderPermissionInfo();
            objFolderPermissionInfo.FolderID = FolderId;
            objFolderPermissionInfo.PermissionID = PermissionId;
            objFolderPermissionInfo.RoleID = RoleId;
            objFolderPermissionInfo.UserID = UserId;
            objFolderPermissionInfo.AllowAccess = true;
            folder.FolderPermissions.Add(objFolderPermissionInfo, true);
            FolderPermissionController.SaveFolderPermissions(folder);
        }
        public static void Synchronize(int PortalId, int AdministratorRoleId, string HomeDirectory, bool hideSystemFolders)
        {
            string PhysicalRoot = HomeDirectory;
            string VirtualRoot = "";
            SynchronizeFolder(PortalId, PhysicalRoot, VirtualRoot, true, true, true, hideSystemFolders);
            DataCache.ClearFolderCache(PortalId);
        }
        public static void SynchronizeFolder(int PortalId, string physicalPath, string relativePath, bool isRecursive, bool hideSystemFolders)
        {
            SynchronizeFolder(PortalId, physicalPath, relativePath, isRecursive, true, true, hideSystemFolders);
        }
        public static void SynchronizeFolder(int PortalId, string physicalPath, string relativePath, bool isRecursive, bool syncFiles, bool forceFolderSync, bool hideSystemFolders)
        {
            CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
            int FolderId;
            bool isInSync = true;
            if (forceFolderSync == true && String.IsNullOrEmpty(relativePath))
            {
                RemoveOrphanedFolders(PortalId);
            }
            CommonLibrary.Services.FileSystem.FolderInfo folder = objFolderController.GetFolder(PortalId, relativePath, false);
            DirectoryInfo dirInfo = new DirectoryInfo(physicalPath);
            if (dirInfo.Exists)
            {
                if (folder == null)
                {
                    if (ShouldSyncFolder(hideSystemFolders, dirInfo))
                    {
                        FolderId = AddFolder(PortalId, relativePath, (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem);
                        folder = objFolderController.GetFolder(PortalId, relativePath, true);
                        isInSync = false;
                    }
                    else
                    {
                        //Prevent further processing of this folder
                        return;
                    }
                }
                else
                {
                    isInSync = (dirInfo.LastWriteTime.ToString("yyyyMMddhhmmss") == folder.LastUpdated.ToString("yyyyMMddhhmmss"));
                }
                if (folder != null)
                {
                    if (syncFiles == true && (isInSync == false || forceFolderSync == true))
                    {
                        string[] strFiles = Directory.GetFiles(physicalPath);
                        foreach (string strFileName in strFiles)
                        {
                            AddFile(strFileName, PortalId, false, folder);
                        }
                        RemoveOrphanedFiles(folder, PortalId);
                        folder.LastUpdated = dirInfo.LastWriteTime;
                        objFolderController.UpdateFolder(folder);
                    }
                    if (isRecursive)
                    {
                        string[] strFolders = Directory.GetDirectories(physicalPath);
                        foreach (string strFolder in strFolders)
                        {
                            DirectoryInfo dir = new DirectoryInfo(strFolder);
                            string relPath = Null.NullString;
                            if (String.IsNullOrEmpty(relativePath))
                            {
                                relPath = dir.Name + "/";
                            }
                            else
                            {
                                relPath = relativePath;
                                if (!relativePath.EndsWith("/"))
                                {
                                    relPath = relPath + "/";
                                }
                                relPath = relPath + dir.Name + "/";
                            }
                            SynchronizeFolder(PortalId, strFolder, relPath, true, syncFiles, forceFolderSync, hideSystemFolders);
                        }
                    }
                }
            }
            else
            {
                if (folder != null)
                {
                    if (folder.StorageLocation != (int)CommonLibrary.Services.FileSystem.FolderController.StorageLocationTypes.DatabaseSecure)
                    {
                        RemoveOrphanedFiles(folder, PortalId);
                        objFolderController.DeleteFolder(PortalId, relativePath.Replace("\\", "/"));
                    }
                }
            }
        }
        public static string UnzipFile(string fileName, string DestFolder, PortalSettings settings)
        {
            ZipInputStream objZipInputStream = null;
            string strMessage = "";
            try
            {
                int FolderPortalId = GetFolderPortalId(settings);
                bool isHost = settings.ActiveTab.ParentId == settings.SuperTabId;
                PortalController objPortalController = new PortalController();
                CommonLibrary.Services.FileSystem.FolderController objFolderController = new CommonLibrary.Services.FileSystem.FolderController();
                CommonLibrary.Services.FileSystem.FileController objFileController = new CommonLibrary.Services.FileSystem.FileController();
                string sourceFolderName = Globals.GetSubFolderPath(fileName, FolderPortalId);
                string sourceFileName = GetFileName(fileName);
                CommonLibrary.Services.FileSystem.FolderInfo folder = objFolderController.GetFolder(FolderPortalId, sourceFolderName, false);
                CommonLibrary.Services.FileSystem.FileInfo file = objFileController.GetFile(sourceFileName, FolderPortalId, folder.FolderID);
                int storageLocation = folder.StorageLocation;
                ZipEntry objZipEntry;
                string strFileName = "";
                string strExtension;
                try
                {
                    objZipInputStream = new ZipInputStream(GetFileStream(file));
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                ArrayList sortedFolders = new ArrayList();
                objZipEntry = objZipInputStream.GetNextEntry();
                while (objZipEntry != null)
                {
                    if (objZipEntry.IsDirectory)
                    {
                        try
                        {
                            sortedFolders.Add(objZipEntry.Name.ToString());
                        }
                        catch (Exception ex)
                        {
                            objZipInputStream.Close();
                            return ex.Message;
                        }
                    }
                    objZipEntry = objZipInputStream.GetNextEntry();
                }
                sortedFolders.Sort();
                foreach (string s in sortedFolders)
                {
                    try
                    {
                        AddFolder(settings, DestFolder, s.ToString(), storageLocation);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                objZipInputStream = new ZipInputStream(GetFileStream(file));
                objZipEntry = objZipInputStream.GetNextEntry();
                while (objZipEntry != null)
                {
                    if (!objZipEntry.IsDirectory)
                    {
                        if (objPortalController.HasSpaceAvailable(FolderPortalId, objZipEntry.Size))
                        {
                            strFileName = Path.GetFileName(objZipEntry.Name);
                            if (!String.IsNullOrEmpty(strFileName))
                            {
                                strExtension = Path.GetExtension(strFileName).Replace(".", "");
                                if (("," + Host.FileExtensions.ToLower()).IndexOf("," + strExtension.ToLower()) != 0 || isHost)
                                {
                                    try
                                    {
                                        string folderPath = System.IO.Path.GetDirectoryName(DestFolder + objZipEntry.Name.Replace("/", "\\"));
                                        DirectoryInfo Dinfo = new DirectoryInfo(folderPath);
                                        if (!Dinfo.Exists)
                                        {
                                            AddFolder(settings, DestFolder, objZipEntry.Name.Substring(0, objZipEntry.Name.Replace("/", "\\").LastIndexOf("\\")));
                                        }
                                        string zipEntryFileName = DestFolder + objZipEntry.Name.Replace("/", "\\");
                                        strMessage += AddFile(FolderPortalId, objZipInputStream, zipEntryFileName, "", objZipEntry.Size, Globals.GetSubFolderPath(zipEntryFileName, settings.PortalId), false, false);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (objZipInputStream != null)
                                        {
                                            objZipInputStream.Close();
                                        }
                                        return ex.Message;
                                    }
                                }
                                else
                                {
                                    strMessage += "<br>" + string.Format(Localization.GetString("RestrictedFileType"), strFileName, Host.FileExtensions.Replace(",", ", *."));
                                }
                            }
                        }
                        else
                        {
                            strMessage += "<br>" + string.Format(Localization.GetString("DiskSpaceExceeded"), strFileName);
                        }
                    }
                    objZipEntry = objZipInputStream.GetNextEntry();
                }
            }
            finally
            {
                if (objZipInputStream != null)
                {
                    objZipInputStream.Close();
                    objZipInputStream.Dispose();
                }
            }
            return strMessage;
        }
        public static void UnzipResources(ZipInputStream zipStream, string destPath)
        {
            try
            {
                ZipEntry objZipEntry;
                string LocalFileName;
                string RelativeDir;
                string FileNamePath;
                objZipEntry = zipStream.GetNextEntry();
                while (objZipEntry != null)
                {
                    LocalFileName = objZipEntry.Name;
                    RelativeDir = Path.GetDirectoryName(objZipEntry.Name);
                    if ((RelativeDir != string.Empty) && (!Directory.Exists(Path.Combine(destPath, RelativeDir))))
                    {
                        Directory.CreateDirectory(Path.Combine(destPath, RelativeDir));
                    }
                    if ((!objZipEntry.IsDirectory) && (!String.IsNullOrEmpty(LocalFileName)))
                    {
                        FileNamePath = Path.Combine(destPath, LocalFileName).Replace("/", "\\");
                        try
                        {
                            if (File.Exists(FileNamePath))
                            {
                                File.SetAttributes(FileNamePath, FileAttributes.Normal);
                                File.Delete(FileNamePath);
                            }
                            FileStream objFileStream = null;
                            try
                            {
                                objFileStream = File.Create(FileNamePath);
                                int intSize = 2048;
                                byte[] arrData = new byte[2048];
                                intSize = zipStream.Read(arrData, 0, arrData.Length);
                                while (intSize > 0)
                                {
                                    objFileStream.Write(arrData, 0, intSize);
                                    intSize = zipStream.Read(arrData, 0, arrData.Length);
                                }
                            }
                            finally
                            {
                                if (objFileStream != null)
                                {
                                    objFileStream.Close();
                                    objFileStream.Dispose();
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    objZipEntry = zipStream.GetNextEntry();
                }
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
            }
        }
        public static string UploadFile(string RootPath, HttpPostedFile objHtmlInputFile, bool Unzip)
        {
            return UploadFile(RootPath, objHtmlInputFile, Null.NullString, Unzip);
        }
        public static string UploadFile(string RootPath, HttpPostedFile objHtmlInputFile, string NewFileName, bool Unzip)
        {
            return CreateFile(RootPath, objHtmlInputFile.FileName, objHtmlInputFile.ContentLength, objHtmlInputFile.ContentType, objHtmlInputFile.InputStream, NewFileName, Unzip);
        }
        public static void WriteFile(string strFileName)
        {
            System.Web.HttpResponse objResponse = System.Web.HttpContext.Current.Response;
            System.IO.Stream objStream = null;
            try
            {
                objStream = new System.IO.FileStream(strFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                WriteStream(objResponse, objStream);
            }
            catch (Exception ex)
            {
                objResponse.Write("Error : " + ex.Message);
            }
            finally
            {
                if (objStream != null)
                {
                    objStream.Close();
                    objStream.Dispose();
                }
            }
        }
        public static string DeleteFiles(Array arrPaths)
        {
            string strExceptions = "";
            for (int i = 0; i < arrPaths.Length; i++)
            {
                string strPath = arrPaths.GetValue(i).ToString();
                if (strPath.IndexOf("'") != -1)
                {
                    strPath = strPath.Substring(0, strPath.IndexOf("'"));
                }
                if (!String.IsNullOrEmpty(strPath.Trim()))
                {
                    strPath = Common.Globals.ApplicationMapPath + "\\" + strPath;
                    if (strPath.EndsWith("\\"))
                    {
                        if (Directory.Exists(strPath))
                        {
                            try
                            {
                                Globals.DeleteFolderRecursive(strPath);
                            }
                            catch (Exception ex)
                            {
                                strExceptions += "Error: " + ex.Message + Environment.NewLine;
                            }
                        }
                    }
                    else
                    {
                        if (File.Exists(strPath))
                        {
                            try
                            {
                                File.SetAttributes(strPath, FileAttributes.Normal);
                                File.Delete(strPath);
                            }
                            catch (Exception ex)
                            {
                                strExceptions += "Error: " + ex.Message + Environment.NewLine;
                            }
                        }
                    }
                }
            }
            return strExceptions;
        }
        public static string SendFile(string URL, string FilePath)
        {
            string strMessage = "";
            try
            {
                WebClient objWebClient = new WebClient();
                byte[] responseArray = objWebClient.UploadFile(URL, "POST", FilePath);
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return strMessage;
        }
        public static string ReceiveFile(HttpRequest Request, string FolderPath)
        {
            string strMessage = "";
            try
            {
                if (Request.Files.AllKeys.Length != 0)
                {
                    string strKey = Request.Files.AllKeys[0];
                    HttpPostedFile objFile = Request.Files[strKey];
                    objFile.SaveAs(FolderPath + objFile.FileName);
                }
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return strMessage;
        }
        public static string PullFile(string URL, string FilePath)
        {
            string strMessage = "";
            try
            {
                WebClient objWebClient = new WebClient();
                objWebClient.DownloadFile(URL, FilePath);
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return strMessage;
        }

        public static IEnumerable<System.IO.FileInfo> GetFilesByExtensions(this DirectoryInfo dirInfo, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
            return dirInfo.EnumerateFiles().Where(f => allowedExtensions.Contains(f.Extension));
        }
    }
}
