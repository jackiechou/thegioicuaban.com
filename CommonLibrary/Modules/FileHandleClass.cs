using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Net;
using System.Web.UI;

namespace CommonLibrary.Modules
{
    public class FileHandleClass: System.Web.UI.Page
    {
        //====================== 24/12/2012===============================================
        public static string[] getDirectionFromFile(string file)
        {
            string[] result = new string[2];
            string filename = string.Empty, direction = string.Empty;

            if (file.IndexOf("/") > -1)
            {
                result[0] = file.Substring(0, file.LastIndexOf("/"));//direction 
                result[1] = file.Remove(0, file.LastIndexOf("/")).Trim('/');//filename
            }
            else
            {
                result[0] = "";//direction 
                result[1] = file;//filename
            }

            return result;
        }

        public void DeleteFile(string Orginal_FileName, string upload_dir_path)
        {
            string[] result = new string[2];
            result = getDirectionFromFile(Orginal_FileName);
            string dir_path = upload_dir_path + "/" + result[0];
            string filename = result[1].ToString();            
            if (Directory.Exists(upload_dir_path))
            {
                FileHandleClass file_handle_obj = new FileHandleClass();
                file_handle_obj.deleteFile(filename, upload_dir_path);
            }
        }
        //================================================================================
        //Save a Stream to a File
        // readStream is the stream you need to read
        // writeStream is the stream you want to write to
        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();

            //string saveTo = "some path to save";
            //// create a write stream
            //FileStream writeStream = new FileStream(saveTo, FileMode.Create, FileAccess.Write);
            //// write to the stream
            //ReadWriteStream(readStream, writeStream);
        }

        private bool CreateByteArrayToFile(string file_path, byte[] buffer)
        {
            bool result = false;
            try
            {
                System.IO.FileStream file_stream = new System.IO.FileStream(file_path, FileMode.Create, FileAccess.Write);
                file_stream.Write(buffer, 0, buffer.Length);
                file_stream.Close();
                result = true;
            }
            catch (Exception ex) { ex.ToString(); result = false; }
            return result;
        }

        public byte[] GetBytesFromFile(string filePath)
        {
            FileStream fileStream;

            byte[] fileByte;

            using (fileStream = File.OpenRead(filePath))
            {
                fileByte = new byte[fileStream.Length];
                fileStream.Read(fileByte, 0, Convert.ToInt32(fileStream.Length));
            }

            return fileByte;
        }

        public void CreateFileFromStream(string file_path, Stream stream)
        {
            int bufferSize = (int)stream.Length;
            if (bufferSize == 0) return;
            using (FileStream fileStream = System.IO.File.Create(file_path, bufferSize))
            {
                byte[] buffer = new byte[bufferSize];
                stream.Read(buffer, 0, Convert.ToInt32(buffer.Length));
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }

        public static byte[] GetByteFromFileUpload(FileUpload FileUpload1)
        {
            //C1
            //FileInfo file_info = new FileInfo(FileUpload1.PostedFile.FileName);
            //byte[] file_content = null;
            //if (file_info.Exists)
            //{
            //    file_content = new byte[file_info.Length];
            //    FileStream imagestream = file_info.OpenRead();
            //    imagestream.Read(file_content, 0, file_content.Length);
            //    imagestream.Close();
            //}

            //C2:
            byte[] file_content = FileUpload1.FileBytes;
            FileUpload1.PostedFile.InputStream.Read(file_content, 0, file_content.Length);
            //C3:
            //byte[] file_content = new byte[FileUpload1.PostedFile.InputStream.Length];
            //FileUpload1.PostedFile.InputStream.Seek(0, SeekOrigin.Begin);
            //FileUpload1.PostedFile.InputStream.Read(file_content, 0, file_content.Length);
            //FileUpload1.PostedFile.InputStream.Write(file_content,0, file_content.Length);   

            return file_content;
        }

        public static byte[] GetByteFromHtmlInputFile(HtmlInputFile InputFile)
        {
            HttpPostedFile myFile = InputFile.PostedFile;
            byte[] file_content = null;
            if (InputFile.PostedFile != null)
            {
                int nFileLen = myFile.ContentLength;
                file_content = new byte[nFileLen];
                myFile.InputStream.Read(file_content, 0, nFileLen);
            }
            return file_content;
        }

        public void GetFileContent(string filepath)
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)HttpWebRequest.Create(filepath);
            System.IO.StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            string n = sr.ReadLine();
            request.GetResponse().Close();
        }

        public static byte[] GetByteFromFilePath(string file_path)
        {
            //Get byte array of file
            byte[] byteArray = null;

            if (File.Exists(file_path))
            {
                byteArray = File.ReadAllBytes(file_path);
            }
            return byteArray;

            //Write byte array to file
            //C1: Response.BinaryWrite(byteArray);
            //C2: Response.OutputStream.Write(byteArray,0,byteArray.Length);
            //Set Content Type => Response.ContentTyoe="images/png";
        }

        //begin xu ly file upload========================================================================            
        public string uploadInputFile(HttpPostedFile myfile, string dir_path)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            string new_file_name = null;
            if (myfile.ContentLength > 0)
            {
                if (!System.IO.Directory.Exists(dir_path))
                    System.IO.Directory.CreateDirectory(dir_path);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month + "\\" + date);

                dir_path = dir_path + "\\" + year + "\\" + month + "\\" + date;
                string file_name = System.IO.Path.GetFileName(myfile.FileName);
                new_file_name = GetEncodeString(file_name);
                string file_path = Path.Combine(dir_path, HttpContext.Current.Server.HtmlEncode(new_file_name));
                myfile.SaveAs(file_path);
            }
            return year + "/" + month + "/" + date + "/" + new_file_name;
        }


        public string[] uploadFrontMainInputFile(HttpPostedFile myfile, string dir_front_image_path, string dir_main_image_path, int resized_img_width, int resized_img_height)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            //begin xu ly file upload=======================================================================================================            
            string[] result = new string[2];
            int file_size = myfile.ContentLength;
            if (file_size > 0)
            {
                if (!System.IO.Directory.Exists(dir_main_image_path))
                    System.IO.Directory.CreateDirectory(dir_main_image_path);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year + "\\" + month + "\\" + date);

                if (!System.IO.Directory.Exists(dir_front_image_path))
                    System.IO.Directory.CreateDirectory(dir_front_image_path);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year + "\\" + month + "\\" + date);

                dir_main_image_path = dir_main_image_path + "\\" + year + "\\" + month + "\\" + date;
                dir_front_image_path = dir_front_image_path + "\\" + year + "\\" + month + "\\" + date;

                string file_name = System.IO.Path.GetFileNameWithoutExtension(myfile.FileName);
                string file_ext = System.IO.Path.GetExtension(myfile.FileName).ToLower().Trim();
                string file_name_ext = file_name + file_ext;

                string new_file_name = GetEncodeString(file_name);
                result[0] = (year + "\\" + month + "\\" + date + "\\" + new_file_name + file_ext).Replace("\\", "/");
                result[1] = (year + "\\" + month + "\\" + date + "\\" + string.Format("{0}_thumb{1}", new_file_name, file_ext)).Replace("\\", "/");

                string main_image_path = Path.Combine(dir_main_image_path, HttpContext.Current.Server.HtmlEncode(new_file_name + file_ext));
                string front_image_path = Path.Combine(dir_front_image_path, HttpContext.Current.Server.HtmlEncode(string.Format("{0}_thumb{1}", new_file_name, file_ext)));

                if (System.IO.Directory.Exists(dir_main_image_path) && System.IO.Directory.Exists(dir_front_image_path))
                {
                    // Save main image================================================================================
                    System.Drawing.Image image = System.Drawing.Image.FromStream(myfile.InputStream);
                    image.Save(main_image_path);

                    System.Drawing.Image thumb = image.GetThumbnailImage((int)resized_img_width, (int)resized_img_height, null, IntPtr.Zero);
                    thumb.Save(front_image_path);
                    image.Dispose();
                    thumb.Dispose();
                }
            }
            return result;
        }

        public string uploadFixedInputFile(HttpPostedFile myfile, string dir_path, int width, int height)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            string new_file_name = null;
            if (myfile.ContentLength > 0)
            {
                if (!System.IO.Directory.Exists(dir_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month + "\\" + date);

                dir_path = dir_path + "\\" + year + "\\" + month + "\\" + date;

                if (System.IO.Directory.Exists(dir_path) == true)
                {
                    new_file_name = GetEncodeString(System.IO.Path.GetFileName(myfile.FileName));
                    string file_path = Path.Combine(dir_path, HttpContext.Current.Server.HtmlEncode(new_file_name));
                    System.Drawing.Image image = System.Drawing.Image.FromStream(myfile.InputStream);
                    System.Drawing.Image img = image.GetThumbnailImage((int)width, (int)height, delegate() { return false; }, (IntPtr)0);
                    img.Save(file_path);
                }
            }
            return year + "/" + month + "/" + date + "/" + new_file_name;
        }

        public string uploadFile(FileUpload FileUpload1, string dir_path)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();
            string new_file_name = null;

            if (FileUpload1.HasFile == true)
            {
                if (!System.IO.Directory.Exists(dir_path))
                    System.IO.Directory.CreateDirectory(dir_path);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month + "\\" + date);

                dir_path = dir_path + "\\" + year + "\\" + month + "\\" + date;
                new_file_name = GetEncodeString(FileUpload1.FileName);
                string file_path = Path.Combine(dir_path, HttpContext.Current.Server.HtmlEncode(new_file_name));

                if (File.Exists(file_path))
                    File.Delete(file_path);

                FileUpload1.SaveAs(file_path);
            }
            return year + "/" + month + "/" + date + "/" + new_file_name;
        }

        public string upload_video(FileUpload FileUpload1, string video_dir_path)
        {

            string result = string.Empty;
            if (FileUpload1.HasFile == true)
            {
                if (!System.IO.Directory.Exists(video_dir_path))
                    System.IO.Directory.CreateDirectory(video_dir_path);

                string file_name = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
                string file_ext = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower().Trim();
                string file_type = FileUpload1.PostedFile.ContentType;

                bool allowedType = false;

                switch (file_type)
                {
                    case ("video/avi"):
                        allowedType = true;
                        break;
                    case ("video/flv"):
                        allowedType = true;
                        file_ext = ".flv";
                        break;
                    case ("video/mp4"):
                        allowedType = true;
                        break;
                    case ("video/mpeg"):
                        allowedType = true;
                        break;
                    case ("image/wav"):
                        allowedType = true;
                        break;
                    case ("application/x-shockwave-flash"):
                        allowedType = true;
                        break;
                    default:
                        allowedType = false;
                        break;
                }

                if (allowedType)
                {
                    string new_file_name_ext = GetEncodeString(file_name) + file_ext;
                    string file_path = Path.Combine(video_dir_path, HttpContext.Current.Server.HtmlEncode(new_file_name_ext));

                    if (File.Exists(file_path))
                        result = "0";
                    else
                    {
                        FileUpload1.SaveAs(file_path);
                        result = new_file_name_ext;
                    }
                }
                else
                {
                    result = "-1";
                }
            }
            return result;
        }

        public string upload_front_main_images(FileUpload FileUpload1, string dir_front_image_path, string dir_main_image_path, int max_size, int num_scale)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            //begin xu ly file upload=======================================================================================================            
            string result = null;
            string new_main_image_name = null;
            string new_front_image_name = null;
            string error = null;

            if (FileUpload1.HasFile == true)
            {
                if (!System.IO.Directory.Exists(dir_main_image_path))
                    System.IO.Directory.CreateDirectory(dir_main_image_path);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year + "\\" + month + "\\" + date);

                if (!System.IO.Directory.Exists(dir_front_image_path))
                    System.IO.Directory.CreateDirectory(dir_front_image_path);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year + "\\" + month + "\\" + date);

                dir_main_image_path = dir_main_image_path + "\\" + year + "\\" + month + "\\" + date;
                dir_front_image_path = dir_front_image_path + "\\" + year + "\\" + month + "\\" + date;


                string file_name = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
                string file_ext = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower().Trim();
                string file_name_ext = file_name + file_ext;

                string file_type = FileUpload1.PostedFile.ContentType;
                bool allowedType = false;

                switch (file_type)
                {
                    case ("image/gif"):
                        allowedType = true;
                        file_ext = ".gif";
                        break;
                    case ("image/jpg"):
                        allowedType = true;
                        file_ext = ".jpeg";
                        break;
                    case ("image/jpeg"):
                        allowedType = true;
                        file_ext = ".jpeg";
                        break;
                    case ("image/bmp"):
                        allowedType = true;
                        file_ext = ".bmp";
                        break;
                    case ("image/png"):
                        allowedType = true;
                        file_ext = ".png";
                        break;
                    case ("image/tiff"):
                        allowedType = true;
                        file_ext = ".tiff";
                        break;
                    case ("application/x-shockwave-flash"):
                        allowedType = true;
                        file_ext = ".swf";
                        break;
                    default:
                        allowedType = false;
                        error = "InvalidExtension";
                        break;
                }

                if (allowedType)
                {
                    string new_file_name = GetEncodeString(file_name);
                    new_main_image_name = new_file_name + file_ext;
                    string main_image_path = Path.Combine(dir_main_image_path, HttpContext.Current.Server.HtmlEncode(new_main_image_name));

                    if (File.Exists(main_image_path))
                        //File.Delete(file_path);
                        error = "Existed";
                    else
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                        image.Save(main_image_path);

                        float imgWidth = image.PhysicalDimension.Width;
                        float imgHeight = image.PhysicalDimension.Height;
                        float imgSize = imgHeight > imgWidth ? imgHeight : imgWidth;
                        float imgResize = imgSize <= num_scale ? (float)1.0 : num_scale / imgSize;
                        imgWidth *= imgResize; imgHeight *= imgResize;

                        System.Drawing.Image thumb = image.GetThumbnailImage((int)imgWidth, (int)imgHeight, delegate() { return false; }, (IntPtr)0);
                        new_front_image_name = string.Format("{0}_thumb{1}", new_file_name, file_ext);
                        string front_image_path = Path.Combine(dir_front_image_path, HttpContext.Current.Server.HtmlEncode(new_front_image_name));
                        thumb.Save(front_image_path);

                        result = year + "/" + month + "/" + date + "/" + new_main_image_name + "," + year + "/" + month + "/" + date + "/" + new_front_image_name;
                    }
                }
                else
                    error = "InvalidExtension";
            }
            if (error != null)
                return error;
            else
                return result;
        }

        public string[] upload_front_main_image(FileUpload FileUpload1, string dir_front_image_path, string dir_main_image_path, int resized_img_width, int resized_img_height)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            //begin xu ly file upload=======================================================================================================            
            string[] result = new string[2];

            if (FileUpload1.HasFile == true)
            {
                // Create the directory if it does not exist.
                if (!System.IO.Directory.Exists(dir_main_image_path))
                    Directory.CreateDirectory(dir_main_image_path);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_main_image_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_main_image_path + "\\" + year + "\\" + month + "\\" + date);

                if (!System.IO.Directory.Exists(dir_front_image_path))
                    Directory.CreateDirectory(dir_front_image_path);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_front_image_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_front_image_path + "\\" + year + "\\" + month + "\\" + date);

                dir_main_image_path = dir_main_image_path + "\\" + year + "\\" + month + "\\" + date;
                dir_front_image_path = dir_front_image_path + "\\" + year + "\\" + month + "\\" + date;

                string file_name = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
                string file_ext = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower().Trim();
                string file_name_ext = file_name + file_ext;
                int file_size = FileUpload1.PostedFile.ContentLength;

                string new_file_name = GetEncodeString(file_name);
                result[0] = (year + "\\" + month + "\\" + date + "\\" + new_file_name + file_ext).Replace("\\", "/");
                string main_image_path = Path.Combine(dir_main_image_path, HttpContext.Current.Server.HtmlEncode(new_file_name + file_ext));

                if (File.Exists(main_image_path))
                    File.Delete(main_image_path);
                else
                {
                    // Save main image================================================================================
                    System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                    image.Save(main_image_path);

                    // Save front image ==============================================================================                   
                    result[1] = (year + "\\" + month + "\\" + date + "\\" + string.Format("{0}_thumb{1}", new_file_name, file_ext)).Replace("\\", "/");
                    string front_image_path = Path.Combine(dir_front_image_path, HttpContext.Current.Server.HtmlEncode(string.Format("{0}_thumb{1}", new_file_name, file_ext)));

                    if (File.Exists(front_image_path))
                        File.Delete(front_image_path);

                    System.Drawing.Image thumb = image.GetThumbnailImage((int)resized_img_width, (int)resized_img_height, null, IntPtr.Zero);
                    //thumb.Save(front_image_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    thumb.Save(front_image_path);
                    image.Dispose();
                    thumb.Dispose();
                }
            }
            return result;
        }

        public string uploadImage(FileUpload FileUpload1, string dir_path)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            //begin xu ly file upload========================================================================            
            string new_file_name = null;

            if (FileUpload1.HasFile == true)
            {
                string file_name = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
                string file_ext = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower().Trim();
                string file_name_ext = file_name + file_ext; // string file_name = FileUpload1.PostedFile.FileName;                

                if (!System.IO.Directory.Exists(dir_path))
                    System.IO.Directory.CreateDirectory(dir_path);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month + "\\" + date);

                dir_path = dir_path + "\\" + year + "\\" + month + "\\" + date;


                if (System.IO.Directory.Exists(dir_path) == true)
                {
                    new_file_name = GetEncodeString(file_name_ext);
                    string file_path = Path.Combine(dir_path, HttpContext.Current.Server.HtmlEncode(new_file_name));

                    if (File.Exists(file_path))
                        File.Delete(file_path);
                    else
                    {   //FileUpload1.PostedFile.SaveAs(file_path);
                        System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                        image.Save(file_path);
                    }
                }
                else
                {
                    // Create the directory if it does not exist.
                    Directory.CreateDirectory(dir_path);
                }

            }
            return year + "/" + month + "/" + date + "/" + new_file_name;
        }

        public string uploadImage(FileUpload FileUpload1, string dir_path, int width, int height)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
            if (!System.IO.Directory.Exists(dir_path + "\\" + year))
                System.IO.Directory.CreateDirectory(dir_path + "\\" + year);
            if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month))
                System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month);
            if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month + "\\" + date))
                System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month + "\\" + date);

            dir_path = dir_path + "\\" + year + "\\" + month + "\\" + date;


            string fileName = Path.Combine(HttpContext.Current.Server.MapPath(dir_path), FileUpload1.FileName);
            if (File.Exists(fileName))
                File.Delete(fileName);

            System.Drawing.Image img = image.GetThumbnailImage((int)width, (int)height, delegate() { return false; }, (IntPtr)0);
            string file_path = Path.Combine(dir_path, string.Format("{0}", GetEncodeString(Path.GetFileNameWithoutExtension(FileUpload1.FileName)), Path.GetExtension(FileUpload1.FileName)));
            img.Save(file_path);

            return year + "/" + month + "/" + date + "/" + file_path;
        }

        public string uploadImage(FileUpload FileUpload1, string dir_path, int max_size)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            //begin xu ly file upload========================================================================            
            string new_file_name = null;
            string error = null;

            if (FileUpload1.HasFile == true)
            {
                if (!System.IO.Directory.Exists(dir_path))
                    System.IO.Directory.CreateDirectory(dir_path);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month + "\\" + date);

                dir_path = dir_path + "\\" + year + "\\" + month + "\\" + date;

                string file_name = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
                string file_ext = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower().Trim();
                string file_name_ext = file_name + file_ext; // string file_name = FileUpload1.PostedFile.FileName;                
                int file_size = FileUpload1.PostedFile.ContentLength;

                if (file_size <= max_size)
                {

                    string file_type = FileUpload1.PostedFile.ContentType;
                    bool allowedType = false;

                    switch (file_type)
                    {
                        case ("image/gif"): allowedType = true; file_ext = ".gif"; break;
                        case ("image/jpg"): allowedType = true; file_ext = ".jpeg"; break;
                        case ("image/jpeg"): allowedType = true; file_ext = ".jpeg"; break;
                        case ("image/bmp"): allowedType = true; file_ext = ".bmp"; break;
                        case ("image/png"): allowedType = true; file_ext = ".png"; break;
                        case ("image/tiff"): allowedType = true; file_ext = ".tiff"; break;
                        case ("application/x-shockwave-flash"): allowedType = true; file_ext = ".swf"; break;
                        default: allowedType = false; error = "The file is not an allowed file type (ie: gif, jpg, jpeg,bmp, png, tiff, swf)"; break;
                    }

                    if (allowedType)
                    {
                        new_file_name = GetEncodeString(file_name_ext);
                        string file_path = Path.Combine(dir_path, HttpContext.Current.Server.HtmlEncode(new_file_name));

                        if (File.Exists(file_path))
                        {
                            //File.Delete(file_path);
                            error = new_file_name + " already exists on the server!";
                            return error;
                        }
                        else
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                            image.Save(file_path);
                            //FileUpload1.PostedFile.SaveAs(file_path);
                        }
                    }
                    else
                    {
                        error = "File Extension of is not allowed.";
                    }
                }
                else
                {
                    error = "Exceedance";
                    return error;
                }
            }
            return year + "/" + month + "/" + date + "/" + new_file_name;
        }

        public string uploadThumbImage(FileUpload FileUpload1, string dir_path, int num_scale)
        {
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();

            //begin xu ly file upload========================================================================            
            string new_file_name = null;

            if (FileUpload1.HasFile == true)
            {
                string file_name = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
                string file_ext = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower().Trim();
                string file_name_ext = file_name + file_ext; // string file_name = FileUpload1.PostedFile.FileName; 

                if (!System.IO.Directory.Exists(dir_path))
                    System.IO.Directory.CreateDirectory(dir_path);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month);
                if (!System.IO.Directory.Exists(dir_path + "\\" + year + "\\" + month + "\\" + date))
                    System.IO.Directory.CreateDirectory(dir_path + "\\" + year + "\\" + month + "\\" + date);

                dir_path = dir_path + "\\" + year + "\\" + month + "\\" + date;
                new_file_name = GetEncodeString(file_name_ext);
                string file_path = Path.Combine(dir_path, HttpContext.Current.Server.HtmlEncode(new_file_name));
                if (File.Exists(file_path))
                {
                    //File.Delete(file_path); 
                    new_file_name = new_file_name + "has existed on the server already";
                }
                else
                {
                    try
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                        float imgWidth = image.PhysicalDimension.Width;
                        float imgHeight = image.PhysicalDimension.Height;
                        float imgSize = imgHeight > imgWidth ? imgHeight : imgWidth;
                        float imgResize = imgSize <= num_scale ? (float)1.0 : num_scale / imgSize;
                        imgWidth *= imgResize; imgHeight *= imgResize;

                        System.Drawing.Image thumb = image.GetThumbnailImage((int)imgWidth, (int)imgHeight, delegate() { return false; }, (IntPtr)0);
                        thumb.Save(file_path);
                    }
                    catch (Exception ex) { ex.ToString(); }
                }
            }
            else
            {
                new_file_name = "FileUpload is empty";
            }
            return year + "/" + month + "/" + date + "/" + new_file_name;
        }

        public string uploadImageByPredefinedSize(FileUpload FileUpload1, string dir_path, int maxWidth, int maxHeigth)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);

            string fileName = Path.Combine(HttpContext.Current.Server.MapPath(dir_path), FileUpload1.FileName);
            if (File.Exists(fileName))
                File.Delete(fileName);


            float imgWidth = 0;
            float imgHeight = 0;

            if (image.PhysicalDimension.Width > 600)
            {
                imgWidth = maxWidth;
            }
            else
            {
                imgWidth = image.PhysicalDimension.Width;
            }

            if (image.PhysicalDimension.Height > maxHeigth)
            {
                imgHeight = 400;
            }
            else
            {
                imgHeight = image.PhysicalDimension.Height;
            }

            float imgSize = imgHeight > imgWidth ? imgHeight : imgWidth;
            float imgResize = imgSize <= 128 ? (float)1.0 : 128 / imgSize;
            imgWidth *= imgResize; imgHeight *= imgResize;

            System.Drawing.Image img = image.GetThumbnailImage((int)imgWidth, (int)imgHeight, delegate() { return false; }, (IntPtr)0);
            string file_path = Path.Combine(dir_path, string.Format("{0}", GetEncodeString(Path.GetFileNameWithoutExtension(FileUpload1.FileName)), Path.GetExtension(FileUpload1.FileName)));
            img.Save(file_path);

            return file_path;
        }

        public void CreateFile(string fileName, string strSTR)
        {
            try
            {
                StreamWriter write;
                StreamReader s;
                if (System.IO.File.Exists(fileName) == false)
                {
                    write = new StreamWriter(fileName);
                    write.WriteLine(strSTR);
                    write.Close();
                }
                else
                {
                    s = File.OpenText(fileName);
                    string line = null;
                    while ((line = s.ReadLine()) != null)
                    {
                        strSTR += line;
                    }
                    s.Close();
                    write = new StreamWriter(fileName);
                    write.WriteLine(strSTR);
                    write.Close();
                }
            }
            catch (Exception e) { e.Message.ToString(); }
        }

        public string ReadFile(string fileName)
        {
            string Content = "";
            StreamReader s;
            if (System.IO.File.Exists(fileName) == false)
            {
                return "";
            }
            else
            {
                s = File.OpenText(fileName);
                string line = null;
                while ((line = s.ReadLine()) != null)
                {
                    Content += line + System.Environment.NewLine;
                }
                s.Close();
                return Content;
            }
        }

        /// <summary>
        /// Cập nhật nhật nôi dung của file
        /// </summary>
        public void UpDateFile(string fileName, string newConTent)
        {
            StreamWriter write;
            if (System.IO.File.Exists(fileName) == false)
                System.Console.WriteLine("No Have fileName");
            else
            {
                write = new StreamWriter(fileName);
                write.WriteLine(newConTent);
                write.Close();
            }

        }


        public void deleteFile(string file_name, string dir_path)
        {
            if (file_name != null)
            {
                bool exists = System.IO.Directory.Exists(dir_path);
                if (exists == true)
                {
                    string file_path = System.IO.Path.Combine(dir_path, file_name);
                    if (System.IO.File.Exists(file_path))
                        System.IO.File.Delete(file_path);
                }
            }
        }

        public string GetEncodeString(string strInput)
        {
            //Random String 
            Random rnd = new Random();
            string strRandom = rnd.Next(10000, 99999).ToString() + GetRandomString();

            strInput = new Regex(@"[\s+\\\/:\*\?\&""\''<>|]").Replace(ConvertToUnSign(strInput), string.Empty);
            string encode_result = strRandom + "_" + strInput;
            return encode_result;
        }


        public string GetEncodeStringWithDate(string strInput)
        {
            //Current Date
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
            culture.DateTimeFormat.DateSeparator = string.Empty;
            culture.DateTimeFormat.ShortDatePattern = "yyyyMMdd";
            culture.DateTimeFormat.LongDatePattern = "yyyyMMdd";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            string current_date_yyyymmdd_hhmmss_mmm = DateTime.Now.ToString("yyyymmdd_hhmmss") + DateTime.Now.Millisecond.ToString();

            //Random String 
            Random rnd = new Random();
            string strRandom = rnd.Next(10000, 99999).ToString() + GetRandomString();

            strInput = new Regex(@"[\s+\\\/:\*\?\&""\''<>|]").Replace(ConvertToUnSign(strInput), string.Empty);
            string encode_result = current_date_yyyymmdd_hhmmss_mmm + "_" + strRandom + "_" + strInput;
            return encode_result;
        }

        public string GetRandomString()
        {
            //use the following string to control your set of alphabetic characters to choose from
            //for example, you could include uppercase too
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";

            // Random is not truly random, 
            // so we try to encourage better randomness by always changing the seed value
            Random rnd = new Random(DateTime.Now.Millisecond);

            // basic 5 digit random number
            string result = rnd.Next(10000, 99999).ToString();

            // single random character in ascii range a-z
            string alphaChar = alphabet.Substring(rnd.Next(0, alphabet.Length - 1), 1);

            // random position to put the alpha character
            int replacementIndex = rnd.Next(0, (result.Length - 1));
            result = result.Remove(replacementIndex, 1).Insert(replacementIndex, alphaChar);

            return result;
        }

        public string ConvertToUnSign(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');

            sb = sb.Replace('á', 'a');
            sb = sb.Replace('à', 'a');
            sb = sb.Replace('ả', 'a');
            sb = sb.Replace('ã', 'a');
            sb = sb.Replace('ạ', 'a');

            sb = sb.Replace('ă', 'a');
            sb = sb.Replace('ắ', 'a');
            sb = sb.Replace('ằ', 'a');
            sb = sb.Replace('ẳ', 'a');
            sb = sb.Replace('ẵ', 'a');
            sb = sb.Replace('ặ', 'a');

            sb = sb.Replace('é', 'e');
            sb = sb.Replace('è', 'e');
            sb = sb.Replace('ẻ', 'e');
            sb = sb.Replace('ẽ', 'e');
            sb = sb.Replace('ẹ', 'e');

            sb = sb.Replace('ê', 'e');
            sb = sb.Replace('ế', 'e');
            sb = sb.Replace('ề', 'e');
            sb = sb.Replace('ể', 'e');
            sb = sb.Replace('ễ', 'e');
            sb = sb.Replace('ệ', 'e');


            sb = sb.Replace('í', 'i');
            sb = sb.Replace('ì', 'i');
            sb = sb.Replace('ỉ', 'i');
            sb = sb.Replace('ĩ', 'i');
            sb = sb.Replace('ị', 'i');

            sb = sb.Replace('ó', 'o');
            sb = sb.Replace('ò', 'o');
            sb = sb.Replace('ỏ', 'o');
            sb = sb.Replace('õ', 'o');
            sb = sb.Replace('ọ', 'o');

            sb = sb.Replace('ô', 'o');
            sb = sb.Replace('ố', 'o');
            sb = sb.Replace('ồ', 'o');
            sb = sb.Replace('ổ', 'o');
            sb = sb.Replace('ỗ', 'o');
            sb = sb.Replace('ộ', 'o');

            sb = sb.Replace('ú', 'u');
            sb = sb.Replace('ù', 'u');
            sb = sb.Replace('ủ', 'u');
            sb = sb.Replace('ũ', 'u');
            sb = sb.Replace('ụ', 'u');

            sb = sb.Replace('ý', 'y');
            sb = sb.Replace('ỳ', 'y');
            sb = sb.Replace('ỷ', 'y');
            sb = sb.Replace('ỹ', 'y');
            sb = sb.Replace('ỵ', 'y');

            //Capital letter
            sb = sb.Replace('Á', 'A');
            sb = sb.Replace('À', 'A');
            sb = sb.Replace('Ả', 'A');
            sb = sb.Replace('Ã', 'A');
            sb = sb.Replace('Ạ', 'A');

            sb = sb.Replace('Ă', 'A');
            sb = sb.Replace('Ắ', 'A');
            sb = sb.Replace('Ằ', 'A');
            sb = sb.Replace('Ẳ', 'A');
            sb = sb.Replace('Ẵ', 'A');
            sb = sb.Replace('Ặ', 'A');

            sb = sb.Replace('É', 'E');
            sb = sb.Replace('È', 'E');
            sb = sb.Replace('Ẻ', 'E');
            sb = sb.Replace('Ẽ', 'E');
            sb = sb.Replace('Ẹ', 'E');

            sb = sb.Replace('Ê', 'E');
            sb = sb.Replace('Ế', 'E');
            sb = sb.Replace('Ề', 'E');
            sb = sb.Replace('Ể', 'E');
            sb = sb.Replace('Ễ', 'E');
            sb = sb.Replace('Ệ', 'E');

            sb = sb.Replace('Í', 'I');
            sb = sb.Replace('Ì', 'I');
            sb = sb.Replace('Ỉ', 'I');
            sb = sb.Replace('Ĩ', 'I');
            sb = sb.Replace('Ị', 'I');

            sb = sb.Replace('Ó', 'O');
            sb = sb.Replace('Ò', 'O');
            sb = sb.Replace('Ỏ', 'O');
            sb = sb.Replace('Õ', 'O');
            sb = sb.Replace('Ọ', 'O');

            sb = sb.Replace('Ô', 'O');
            sb = sb.Replace('Ố', 'O');
            sb = sb.Replace('Ồ', 'O');
            sb = sb.Replace('Ổ', 'O');
            sb = sb.Replace('Ỗ', 'O');
            sb = sb.Replace('Ộ', 'O');


            sb = sb.Replace('Ú', 'U');
            sb = sb.Replace('Ù', 'U');
            sb = sb.Replace('Ủ', 'U');
            sb = sb.Replace('Ũ', 'U');
            sb = sb.Replace('Ụ', 'U');

            sb = sb.Replace('Ý', 'Y');
            sb = sb.Replace('Ỳ', 'Y');
            sb = sb.Replace('Ỷ', 'Y');
            sb = sb.Replace('Ỹ', 'Y');
            sb = sb.Replace('Ỵ', 'Y');

            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

    }
}
