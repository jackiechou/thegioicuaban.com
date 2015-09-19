using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.IO;

namespace CommonLibrary.Modules
{
    public enum AnchorPosition
    {
        Top,
        Center,
        Bottom,
        Left,
        Right
    }

    public enum Dimensions
    {
        Width,
        Height
    }

    public class ImageHandleClass
    {
        public void DeleteImage(string imagePath)
        {
            FileInfo thisFile = new FileInfo(HttpContext.Current.Server.MapPath(imagePath));

            if (thisFile.Exists)
            {
                thisFile.Delete();
            }
        }

        public Image ConvertBase64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }

        public bool ConvertByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);               
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                _Exception.ToString();
            }

            // error occured, return false
            return false;
        }
        public byte[] GetImage(string filepath)
        {
            return File.ReadAllBytes(filepath);
        }        

        private byte[] GetEmptyImagePicture(string path, string filename)
        {
            //HttpContext.Current.Server.MapPath(path)
            string nopicFilename =Path.Combine(path,filename);
            System.Drawing.Bitmap bitMap = new System.Drawing.Bitmap(nopicFilename);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] byteArray = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));
            ms.Close();
            ms.Dispose();
            bitMap.Dispose();
            return byteArray;
        }

        public static byte[] ResizeFromByteArray(string fileName, int MaxSideSize, Byte[] byteArrayIn)
        {
            byte[] byteArray = null;  // really make this an error gif
            MemoryStream ms = new MemoryStream(byteArrayIn);
            byteArray = ResizeFromStream(fileName, MaxSideSize, ms);
            return byteArray;
        }

        public static byte[] ResizeFromStream(string fileName, int MaxSideSize, Stream Buffer)
        {
            byte[] byteArray = null;  // really make this an error gif

            try
            {
                Bitmap bitMap = new Bitmap(Buffer);
                int intOldWidth = bitMap.Width;
                int intOldHeight = bitMap.Height;

                int intNewWidth;
                int intNewHeight;

                int intMaxSide;

                if (intOldWidth >= intOldHeight)
                {
                    intMaxSide = intOldWidth;
                }
                else
                {
                    intMaxSide = intOldHeight;
                }

                if (intMaxSide > MaxSideSize)
                {
                    //set new width and height
                    double dblCoef = MaxSideSize / (double)intMaxSide;
                    intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                    intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
                }
                else
                {
                    intNewWidth = intOldWidth;
                    intNewHeight = intOldHeight;
                }

                Size ThumbNailSize = new Size(intNewWidth, intNewHeight);
                System.Drawing.Image oImg = System.Drawing.Image.FromStream(Buffer);
                System.Drawing.Image oThumbNail = new Bitmap
                    (ThumbNailSize.Width, ThumbNailSize.Height);
                Graphics oGraphic = Graphics.FromImage(oThumbNail);
                oGraphic.CompositingQuality = CompositingQuality.HighQuality;
                oGraphic.SmoothingMode = SmoothingMode.HighQuality;
                oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                Rectangle oRectangle = new Rectangle
                    (0, 0, ThumbNailSize.Width, ThumbNailSize.Height);

                oGraphic.DrawImage(oImg, oRectangle);

                //string fileName = Context.Server.MapPath("~/App_Data/") + "test1.jpg";
                //oThumbNail.Save(fileName, ImageFormat.Jpeg);
                MemoryStream ms = new MemoryStream();
                oThumbNail.Save(ms, ImageFormat.Jpeg);
                byteArray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));

                oGraphic.Dispose();
                oImg.Dispose();
                ms.Close();
                ms.Dispose();
            }
            catch (Exception)
            {
                int newSize = MaxSideSize - 20;
                Bitmap bitMap = new Bitmap(newSize, newSize);
                Graphics g = Graphics.FromImage(bitMap);
                g.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(0, 0, newSize, newSize));

                Font font = new Font("Courier", 8);
                SolidBrush solidBrush = new SolidBrush(Color.Red);
                g.DrawString("Failed File", font, solidBrush, 10, 5);
                g.DrawString(fileName, font, solidBrush, 10, 50);

                MemoryStream ms = new MemoryStream();
                bitMap.Save(ms, ImageFormat.Jpeg);
                byteArray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(byteArray, 0, Convert.ToInt32(ms.Length));

                ms.Close();
                ms.Dispose();
                bitMap.Dispose();
                solidBrush.Dispose();
                g.Dispose();
                font.Dispose();

            }
            return byteArray;
        }

        public byte[] ConvertObjectToByteArray(object _Object)
        {
            try
            {
                // create new memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream();

                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // Serializes an object, or graph of connected objects, to the given stream.
                _BinaryFormatter.Serialize(_MemoryStream, _Object);

                // convert stream to byte array and return
                return _MemoryStream.ToArray();
            }
            catch(Exception _Exception)
            {               
               _Exception.ToString();
            }

            // Error occured, return null
            return null;
        }

        public object ConvertByteArrayToObject(byte[] _ByteArray)
        {
            try
            {
                // convert byte array to memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_ByteArray);

                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // set memory stream position to starting point
                _MemoryStream.Position = 0;

                // Deserializes a stream into an object graph and return as a object.
                return _BinaryFormatter.Deserialize(_MemoryStream);
            }
            catch (Exception _Exception)
            {
                _Exception.ToString();
            }

            // Error occured, return null
            return null;
        }

        //read binary data from external file (image-object.dat) and convert byte array into object, in this case it's an image object, and show the image in picturebox
        //pictureBox2.Image = (Image)FileToObject("c:\\image-object.dat");
        public object ConvertFileToObject(string _FileName)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // attach filestream to binary reader
                System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);

                // get total byte length of the file
                long _TotalBytes = new System.IO.FileInfo(_FileName).Length;

                // read entire file into buffer
                byte[] _ByteArray = _BinaryReader.ReadBytes((Int32)_TotalBytes);

                // close file reader and do some cleanup
                _FileStream.Close();
                _FileStream.Dispose();
                _FileStream = null;
                _BinaryReader.Close();

                // convert byte array to memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_ByteArray);

                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // set memory stream position to starting point
                _MemoryStream.Position = 0;

                // Deserializes a stream into an object graph and return as a object.
                return _BinaryFormatter.Deserialize(_MemoryStream);
            }
            catch (Exception _Exception)
            {
                // Error
               _Exception.ToString();
            }

            // Error occured, return null
            return null;
        }

        public bool ConvertObjectToFile(object _Object, string _FileName)
        {
            try
            {
                // create new memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream();

                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // Serializes an object, or graph of connected objects, to the given stream.
                _BinaryFormatter.Serialize(_MemoryStream, _Object);

                // convert stream to byte array
                byte[] _ByteArray = _MemoryStream.ToArray();

                // Open file for writing
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray.ToArray(), 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                // cleanup
                _MemoryStream.Close();
                _MemoryStream.Dispose();
                _MemoryStream = null;
                _ByteArray = null;

                return true;
            }
            catch (Exception _Exception)
            {
                 _Exception.ToString();
            }

            // Error occured, return null
            return false;
        }

        public static System.Drawing.Imaging.ImageFormat GetImageExtension(string filename)
        {
            string sExtension = filename.Substring(filename.LastIndexOf(".") + 1);
            ImageFormat imgFormat = new ImageFormat(new Guid());

            switch (sExtension.ToLower())
            {
                case "bmp":
                    imgFormat = ImageFormat.Bmp;
                    break;
                case "png":
                    imgFormat = ImageFormat.Png;
                    break;
                case "gif":
                    imgFormat = ImageFormat.Gif;
                    break;
                case "jpg":
                case "jpeg":
                    imgFormat = ImageFormat.Jpeg;
                    break;
                default:
                    imgFormat = null;
                    break;
            }

            return imgFormat;
        }

        public static System.Drawing.Image MakeThumbnail(string filename)
        {
            System.Drawing.Image myThumbnail;
            object obj = new object();
            System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

            System.Drawing.Image imagesize = System.Drawing.Image.FromFile(filename);
            Bitmap bitmapNew = new Bitmap(imagesize);

            if (imagesize.Width < imagesize.Height)
            {
                myThumbnail = bitmapNew.GetThumbnailImage(128 * imagesize.Width / imagesize.Height, 128, myCallback, IntPtr.Zero);

            }
            else
            {
                myThumbnail = bitmapNew.GetThumbnailImage(105, imagesize.Height * 105 / imagesize.Width, myCallback, IntPtr.Zero);
            }

            return myThumbnail;
        }
        //Return thumbnail callback
        public static bool ThumbnailCallback()
        {
            return true;
        }
        
        public static byte[] ConvertImageToByteArray(System.Drawing.Image imageToConvert, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            byte[] img = { 0 };

            try
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    imageToConvert.Save(ms, imageFormat);
                    img = ms.ToArray();
                }
            }
            catch (Exception)
            { }

            return img;
        }
        //Open file in to a filestream and read data in a byte array.
        public static byte[] ReadFile(string sPath)
        {
            byte[] data = null;

            System.IO.FileInfo fInfo = new System.IO.FileInfo(sPath);
            long numBytes = fInfo.Length;

            System.IO.FileStream fStream = new System.IO.FileStream(sPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            System.IO.BinaryReader br = new System.IO.BinaryReader(fStream);

            data = br.ReadBytes((int)numBytes);
            return data;
        }


        #region CROP IMAGE ===============================================================================
        public static Image Crop(Image img, int width, int height, AnchorPosition Anchor)
        {
            if (img == null) return null;

            int sourceWidth = img.Width;
            int sourceHeight = img.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (Anchor)
                {
                    case AnchorPosition.Top:
                        destY = 0;
                        break;
                    case AnchorPosition.Bottom:
                        destY = (int)(height - (sourceHeight * nPercent));
                        break;
                    default:
                        destY = (int)((height - (sourceHeight * nPercent)) / 2);
                        break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (Anchor)
                {
                    case AnchorPosition.Left:
                        destX = 0;
                        break;
                    case AnchorPosition.Right:
                        destX = (int)(width - (sourceWidth * nPercent));
                        break;
                    default:
                        destX = (int)((width - (sourceWidth * nPercent)) / 2);
                        break;
                }
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            //grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.SmoothingMode = SmoothingMode.HighSpeed;
            grPhoto.CompositingQuality = CompositingQuality.HighQuality;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //grPhoto.TextRenderingHint = TextRenderingHint.AntiAlias;

            Rectangle destRec = new Rectangle(destX, destY, destWidth, destHeight);
            Rectangle sourceRec = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);

            grPhoto.DrawImage(img,
                              destRec,
                              sourceRec,
                              GraphicsUnit.Pixel);

            grPhoto.Dispose();

            //Quantizer
            //OctreeQuantizer quantizer = new OctreeQuantizer(255, 8);
            //bmPhoto = quantizer.Quantize(bmPhoto);

            return bmPhoto;
        }

        public static Image Crop(Image img, int width, int height)
        {
            return Crop(img, width, height, AnchorPosition.Center);
        }

        public static bool Crop(Image img, int width, int height, string fileName)
        {
            bool ret = false;
            Image crop = null;

            try
            {
                crop = Crop(img, width, height, AnchorPosition.Center);
                if (crop != null)
                {
                    crop.Save(fileName);
                    crop.Dispose();
                    if (img != null) img.Dispose();
                    ret = true;
                }
            }
            catch
            {
                ret = false;
            }
            finally
            {
                if (crop != null) crop.Dispose();
                if (img != null) img.Dispose();
            }
            return ret;
        }     
        #endregion =======================================================================================
    }
}
