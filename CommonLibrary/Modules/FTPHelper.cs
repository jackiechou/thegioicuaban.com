using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CommonLibrary.Modules
{
   public class FTPHelper
    {
        private static string _FTPServerIP = string.Empty;
        /// <summary>
        /// FTP Server IP
        /// </summary>
        public string FTPServerIP
        {
            get
            {
                if (_FTPServerIP.Length > 0) return _FTPServerIP;
                else
                {
                    _FTPServerIP = System.Configuration.ConfigurationManager.AppSettings["FTPServerIP"];
                    return _FTPServerIP;
                }
            }
            set
            {
                _FTPassword = value;
            }
        }

        private static string _FTPUserID = string.Empty;
        /// <summary>
        /// FTP User IP
        /// </summary>
        public string FTPUserID
        {
            get
            {
                if (_FTPUserID.Length > 0) return _FTPUserID;
                else
                {
                    _FTPUserID = System.Configuration.ConfigurationManager.AppSettings["FTPUserID"];
                    return _FTPUserID;
                }
            }
            set
            {
                _FTPassword = value;
            }
        }

        private static string _FTPassword = string.Empty;
        /// <summary>
        /// FTP User IP
        /// </summary>
        public string FTPassword
        {
            get
            {
                if (_FTPassword.Length > 0) return _FTPassword;
                else
                {
                    _FTPassword = System.Configuration.ConfigurationManager.AppSettings["FTPassword"];
                    return _FTPassword;
                }
            }
            set
            {
                _FTPassword = value;
            }
        }

            FtpWebRequest reqFTP;

            private void Connect(String path)
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(FTPUserID, FTPassword);
            }

            public FTPHelper()
            {
            }
            public FTPHelper(string ftpServerIP, string ftpUserID, string ftpPassword)
            {
                this.FTPServerIP = ftpServerIP;
                this.FTPUserID = ftpUserID;
                this.FTPassword = ftpPassword;
            }

            private string[] GetFileList(string path, string WRMethods)
            {
                string[] downloadFiles;
                StringBuilder result = new StringBuilder();
                try
                {
                    Connect(path);
                    reqFTP.Method = WRMethods;
                    WebResponse response = reqFTP.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append(" ");
                        line = reader.ReadLine();
                    }
                    // to remove the trailing ' '
                    result.Remove(result.ToString().LastIndexOf(' '), 1);
                    reader.Close();
                    response.Close();
                    return result.ToString().Split(' ');
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    downloadFiles = null;
                    return downloadFiles;
                }
            }

            public List<string> GetDirectoryList()
            {
                List<string> result = new List<string>();

                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + FTPServerIP + "/"));
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(FTPUserID, FTPassword);
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                    WebResponse response = reqFTP.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Add(line);

                        line = reader.ReadLine();
                    }
                    reader.Close();
                    response.Close();
                }
                catch (Exception)
                {
                }


                return result;
            }
            private bool IsExist(string name)
            {
                List<string> files = GetDirectoryList();

                return files.Contains(name);
            }

            /// <summary>
            /// Lấy danh sách file từ folder
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public string[] GetFileList(string path)
            {
                return GetFileList("ftp://" + FTPServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectory);
            }

            /// <summary>
            /// Lấy danh sách file từ folder root
            /// </summary>
            /// <returns></returns>
            public string[] GetFileList()
            {
                return GetFileList("ftp://" + FTPServerIP + "/", WebRequestMethods.Ftp.ListDirectory);
            }

            /// <summary>
            /// Hàm upload file
            /// </summary>
            /// <param name="filename"></param>
            public void Upload(string filename)
            {

                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + FTPServerIP + "/" + fileInf.Name;
                Connect(uri);        

                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.ContentLength = fileInf.Length;

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];

                int contentLen;
                FileStream fs = fileInf.OpenRead();

                try
                {
                    Stream strm = reqFTP.GetRequestStream();
                    contentLen = fs.Read(buff, 0, buffLength);
                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }
                    strm.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            /// <summary>
            /// Hàm upload file
            /// </summary>
            public void UpLoad(string filename, string dir)
            {
                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + FTPServerIP + "/" + fileInf.Name;
                FtpWebRequest reqFTP;
                string[] dirs = dir.Split('/');
                string des = "";

                bool needCheck = true;
                foreach (string d in dirs)
                {
                    if (needCheck)
                    {
                        if (!IsExist(d))
                        {
                            MakeDir(d);
                            needCheck = false;
                        }
                    }
                    else
                    {
                        MakeDir(d);
                    }
                    des += "/" + d;

                }

                // Create FtpWebRequest object from the Uri provided
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + FTPServerIP + "/" + dir + "/" + fileInf.Name));

                // Provide the WebPermission Credintials
                reqFTP.Credentials = new NetworkCredential(FTPUserID, FTPassword);

                // By default KeepAlive is true, where the control connection is not closed
                // after a command is executed.
                reqFTP.KeepAlive = false;

                // Specify the command to be executed.
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

                // Specify the data transfer type.
                reqFTP.UseBinary = true;

                // Notify the server about the size of the uploaded file
                reqFTP.ContentLength = fileInf.Length;

                // The buffer size is set to 2kb
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;

                // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
                FileStream fs = fileInf.OpenRead();

                try
                {
                    // Stream to which the file to be upload is written
                    Stream strm = reqFTP.GetRequestStream();

                    // Read from the file stream 2kb at a time
                    contentLen = fs.Read(buff, 0, buffLength);

                    // Till Stream content ends
                    while (contentLen != 0)
                    {
                        // Write Content from the file stream to the FTP Upload Stream
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }

                    // Close the file stream and the Request Stream
                    strm.Close();
                    fs.Close();
                }
                catch (Exception)
                {

                }

            }

            /// <summary>
            /// Hàm download file
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="fileName"></param>
            /// <param name="errorinfo"></param>
            /// <returns></returns>
            public bool Download(string filePath, string fileName, out string errorinfo)
            {
                try
                {
                    String onlyFileName = Path.GetFileName(fileName);

                    string newFileName = filePath + @"\" + onlyFileName;

                    if (File.Exists(newFileName))
                    {
                        errorinfo = string.Format("file không tồn tại", newFileName);
                        return false;
                    }

                    string url = "ftp://" + FTPServerIP + "/" + fileName;

                    Connect(url);

                    reqFTP.Credentials = new NetworkCredential(FTPUserID, FTPassword);

                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                    Stream ftpStream = response.GetResponseStream();

                    long cl = response.ContentLength;

                    int bufferSize = 2048;

                    int readCount;

                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);

                    FileStream outputStream = new FileStream(newFileName, FileMode.Create);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);

                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                    ftpStream.Close();

                    outputStream.Close();

                    response.Close();

                    errorinfo = "";

                    return true;
                }
                catch (Exception ex)
                {
                    errorinfo = string.Format("Lỗi", ex.Message);
                    return false;
                }
            }

            /// <summary>
            /// Hàm xóa file
            /// </summary>
            /// <param name="fileName">Tên file muốn xóa</param>
            public void DeleteFileName(string fileName)
            {
                try
                {
                    FileInfo fileInf = new FileInfo(fileName);
                    string uri = "ftp://" + FTPServerIP + "/" + fileInf.Name;
                    Connect(uri);   

                    reqFTP.KeepAlive = false;
                    reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    response.Close();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            /// <summary>
            /// Hàm tạo folder
            /// </summary>
            /// <param name="dirName">Tên filder</param>
            public void MakeDir(string dirName)
            {
                try
                {
                    string uri = "ftp://" + FTPServerIP + "/" + dirName;
                    Connect(uri);     

                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                    response.Close();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            /// <summary>
            /// Hàm xóa folder
            /// </summary>
            /// <param name="dirName">Tên filder muốn xóa</param>
            public void delDir(string dirName)
            {
                try
                {
                    string uri = "ftp://" + FTPServerIP + "/" + dirName;
                    Connect(uri);
                    reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    response.Close();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            /// <summary>
            /// Xem kích thước file
            /// </summary>
            /// <param name="filename">Tên file</param>
            /// <returns></returns>
            public long GetFileSize(string filename)
            {
                long fileSize = 0;
                try
                {
                    FileInfo fileInf = new FileInfo(filename);
                    string uri = "ftp://" + FTPServerIP + "/" + fileInf.Name;
                    Connect(uri);     
                    reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    fileSize = response.ContentLength;
                    response.Close();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                return fileSize;
            }

            /// <summary>
            /// Đổi tên file
            /// </summary>
            /// <param name="currentFilename">Tên cũ</param>
            /// <param name="newFilename">Tên file mới</param>
            public void Rename(string currentFilename, string newFilename)
            {
                try
                {
                    FileInfo fileInf = new FileInfo(currentFilename);
                    string uri = "ftp://" + FTPServerIP + "/" + fileInf.Name;
                    Connect(uri);
                    reqFTP.Method = WebRequestMethods.Ftp.Rename;
                    reqFTP.RenameTo = newFilename;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    //Stream ftpStream = response.GetResponseStream();
                    //ftpStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            public string[] GetFilesDetailList()
            {
                return GetFileList("ftp://" + FTPServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
            }

            public string[] GetFilesDetailList(string path)
            {
                return GetFileList("ftp://" + FTPServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
            }


        //Sử dụng như sau:
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    FtpHelper ftpHelper = new FtpHelper(ConvertData.Properties.Settings.Default.ftpServerIP,
        //                                        ConvertData.Properties.Settings.Default.ftpUserID,
        //                                        ConvertData.Properties.Settings.Default.ftpPassword);

        //    string[] str = ftpHelper.GetFileList("2005");
        //    richTextBox1.Lines = str;
        //}


        //private void button2_Click(object sender, EventArgs e)
        //{
        //    FtpHelper ftpHelper = new FtpHelper(ConvertData.Properties.Settings.Default.ftpServerIP,
        //                                        ConvertData.Properties.Settings.Default.ftpUserID,
        //                                        ConvertData.Properties.Settings.Default.ftpPassword);

        //    ftpHelper.Download(@"c:\", "2007/11/01/57070.pdf");
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    FtpHelper ftpHelper = new FtpHelper(ConvertData.Properties.Settings.Default.ftpServerIP,
        //                                        ConvertData.Properties.Settings.Default.ftpUserID,
        //                                        ConvertData.Properties.Settings.Default.ftpPassword);

        //    ftpHelper.Upload(@"c:\57070.pdf");
        //}

    }
 }



