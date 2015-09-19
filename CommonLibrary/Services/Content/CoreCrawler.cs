using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using CommonLibrary.Modules;

namespace CommonLibrary.Services.Content
{
    public class CoreCrawler
    {
        public string checkFileName(string filename)
        {
            filename = StringHandleClass.convertName2Link(filename);
            Regex regex = new Regex("[^[0-9]+$");

            if (regex.IsMatch(filename))
            {
                filename = Regex.Match(filename, @"\d+").Groups[0].ToString();
            }

            int newfile = int.Parse(filename);

            int filelength = filename.Length;
            switch (filelength)
            {
                case 1:
                    filename = "0000" + filename;
                    break;
                case 2:
                    filename = "000" + filename;
                    break;
                case 3:
                    filename = "00" + filename;
                    break;
                case 4:
                    filename = "0" + filename;
                    break;
                default:
                    filename = "0" + filename;
                    break;
            }

            return filename;
        }

        public void GetFolder(string directoryString)
        {
            string[] directories = Directory.GetDirectories(directoryString);

            foreach (string directory in directories)
            {
                Console.WriteLine("Subdirectory: \"{0}\"", directory);
                Console.WriteLine("=================================================================");
                GetFile(directory);
            }
        }

        public void GetFile(string dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] rgFiles = di.GetFiles("*");
            for (int i = 0; i < rgFiles.Length; i++)
            {
                //Console.WriteLine("{0}", rgFiles[i]);
                File.Move(dir + "\\" + rgFiles[i].Name, dir + "\\" + checkFileName(rgFiles[i].ToString()) + rgFiles[i].Extension);
                Console.WriteLine(checkFileName(rgFiles[i].ToString()));
            }
        }

        public static byte[] FileToByteArray(string _FileName)
        {
            byte[] _Buffer = null;

            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // attach filestream to binary reader
                System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);

                // get total byte length of the file
                long _TotalBytes = new System.IO.FileInfo(_FileName).Length;

                // read entire file into buffer
                _Buffer = _BinaryReader.ReadBytes((Int32)_TotalBytes);

                // close file reader
                _FileStream.Close();
                _FileStream.Dispose();
                _BinaryReader.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            return _Buffer;
        }

        public static int uploadFile(string FTPAddress, string filePath, string username, string password, string log_file_path)
        {
            try
            {
                string filename = Modules.ModuleClass.convertName2Link(Path.GetFileName(filePath));

                //Create FTP request
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FTPAddress + "/" + filename);

                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(username, password);

                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                //Load the file
                FileStream stream = File.OpenRead(filePath);
                byte[] buffer = new byte[stream.Length];

                stream.Read(buffer, 0, buffer.Length);
                stream.Close();

                //Upload file
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(buffer, 0, buffer.Length);
                reqStream.Close();

                return 1;
            }
            catch (Exception ex)
            {
                Log(log_file_path, "Khong the upload file " + filePath, "error:" + ex.ToString());
                return 0;
            }
        }

        public static void DownloadFiles(string namefile, string folder)
        {

            WebClient webClient = new WebClient();
            using (Stream webStream = webClient.OpenRead(namefile))
            using (FileStream fileStream = new FileStream(folder, FileMode.Create))
            {
                var buffer = new byte[32768];
                int bytesRead;
                Int64 bytesReadComplete = 0;  // Use Int64 for files larger than 2 gb

                // Get the size of the file to download
                Int64 bytesTotal = Convert.ToInt64(webClient.ResponseHeaders["Content-Length"]);

                // Start a new StartWatch for measuring download time
                // Stopwatch sw = Stopwatch.StartNew();

                // Download file in chunks
                while ((bytesRead = webStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bytesReadComplete += bytesRead;
                    fileStream.Write(buffer, 0, bytesRead);

                    // Output current progress to the "Output" editor
                    //StringBuilder sb = new StringBuilder();
                    //sb.AppendLine(String.Format("Progress: {0:0%}", (double)bytesReadComplete / bytesTotal));
                    //sb.AppendLine(String.Format("Downloaded: {0:0,0} Bytes", bytesReadComplete));
                    //sb.AppendLine(String.Format("Time Elapsed: {0:0,.00}s", sw.ElapsedMilliseconds));
                    //sb.AppendLine(String.Format("Average Speed: {0:0,0} KB/s", sw.ElapsedMilliseconds > 0 ? bytesReadComplete / sw.ElapsedMilliseconds / 1.024 : 0));
                    //Output.Text = sb.ToString();
                }

                //  sw.Stop();
            }
            // webClient.DownloadFile(namefile, folder);

        }

        public static int CreateFTPDirectory(string directory, string username, string password)
        {
            try
            {
                //create the directory
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(directory));
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(username, password);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return 1;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return 2;
                }
                else
                {
                    response.Close();
                    return 0;
                }
            }
        }

        public static string removeHTMLtab(string content, string replace)
        {
            string strSource = Regex.Replace(content, @"<(.|\n)*?>", replace);
            strSource = Regex.Replace(strSource, "\r|\t|\n", replace);
            return strSource;
        }

        public static string Demo(string Content)
        {
            string pattern = "^[a-z]{3}$";
            Regex obj = new Regex(pattern);
            return obj.IsMatch("shivkoirala").ToString();
        }

        public String[] GetString(String content, String Position1, String Position2)
        {
            String varParttern = Position1 + "[^" + Position2 + "]*" + Position2;
            Regex objRegex = new Regex(varParttern, RegexOptions.Multiline);
            MatchCollection objMatch = objRegex.Matches(content);
            String[] Ketqua = new String[objMatch.Count];
            for (int i = 0; i < objMatch.Count; i++)
            {
                Ketqua[i] = objMatch[i].Value.Replace(Position1, "").Replace(Position2, "");
            }
            return Ketqua;
        }

        public string GetNum(string s)
        {
            string kq = null;
            Regex objR = new Regex("[0-9]");
            MatchCollection objMatch = objR.Matches(s);
            foreach (Match Tmp in objMatch)
            {
                kq += Tmp.Value;
            }
            return kq;
        }

        // Lay Tieu De
        public static string GetTitle(string Content)
        {
            string pattern = "<a class=\"dxtxt\".*?>+[^<]+";
            Regex Title = new Regex(pattern);
            Match m = Title.Match(Content);
            if (m.Success)
                return m.Value.Substring(16, m.Value.Length - 16);
            return "";
        }

        public static void Log(string log_file_path, string stringlog, string state)
        {
            float N = 1; //Log File size in MB
            FileInfo logFile = new FileInfo(log_file_path);
            if (logFile.Length > 1024 * N)
            {
                logFile.Delete();
                logFile.Create();
            }

            using (StreamWriter logStream = logFile.AppendText())
                logStream.WriteLine(DateTime.Now + " ===> " + state + "==> " + stringlog);
        }


        public static string GetWebContentByWebRequest(string str_url)
        {
            string strContent = "";
            try
            {
                WebRequest objWebRequest = WebRequest.Create(str_url);
                objWebRequest.Credentials = CredentialCache.DefaultCredentials;
                WebResponse objWebResponse = objWebRequest.GetResponse();
                Stream receiveStream = objWebResponse.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                strContent = readStream.ReadToEnd();
                objWebResponse.Close();
                readStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return strContent;
        }


        public static string GetWebPageByHttpWebRequest(String uri)
        {
            const int bufSizeMax = 65536; // max read buffer size conserves memory
            const int bufSizeMin = 8192;  // min size prevents numerous small reads
            StringBuilder sb;

            // A WebException is thrown if HTTP request fails
            try
            {
                // Create an HttpWebRequest using WebRequest.Create (see .NET docs)!
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                // Execute the request and obtain the response stream
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                // Content-Length header is not trustable, but makes a good hint.
                // Responses longer than int size will throw an exception here!
                int length = (int)response.ContentLength;

                // Use Content-Length if between bufSizeMax and bufSizeMin
                int bufSize = bufSizeMin;
                if (length > bufSize)
                    bufSize = length > bufSizeMax ? bufSizeMax : length;

                // Allocate buffer and StringBuilder for reading response
                byte[] buf = new byte[bufSize];
                sb = new StringBuilder(bufSize);

                while ((length = responseStream.Read(buf, 0, buf.Length)) != 0)
                    sb.Append(Encoding.UTF8.GetString(buf, 0, length));
            }
            catch (Exception ex)
            {
                sb = new StringBuilder(ex.Message);
            }
            return sb.ToString();
        }
    }
}
