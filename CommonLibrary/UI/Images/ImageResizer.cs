using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CommonLibrary.UI.Images
{
    /// <summary>
    /// Resize images
    /// </summary>
    public class ImageResizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageResizer"/> class.
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toPath">To path.</param>
        public ImageResizer(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");
            if (!File.Exists(fromPath))
                throw new FileNotFoundException(fromPath + " does not exist");
            var toDirectory = Path.GetDirectoryName(toPath);
            if (!Directory.Exists(toDirectory))
                throw new DirectoryNotFoundException(toDirectory + " does not exist");
            FromPath = fromPath;
            ToPath = toPath;
            Width = 200;
            Compression = 50; //50%
        }

        /// <summary>
        /// Gets or sets from path.
        /// </summary>
        /// <value>From path.</value>
        public string FromPath { get; private set; }
        /// <summary>
        /// Gets or sets to path.
        /// </summary>
        /// <value>To path.</value>
        public string ToPath { get; private set; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }
        /// <summary>
        /// Gets or sets the height. 0 indicates height proportional to width.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }
        private int _compression;
        /// <summary>
        /// Gets or sets the compression. 0 corresponds to the greatest compression, 100 corresponds to the least compression.
        /// </summary>
        /// <value>The compression.</value>
        public int Compression
        {
            get { return _compression; }
            set
            {
                if (value > 100) throw new ArgumentOutOfRangeException("value", "value must be 100 or less");
                if (value < 0) throw new ArgumentOutOfRangeException("value", "value must be 0 or more");

                _compression = value;
            }
        }

        /// <summary>
        /// Resizes and saves the image.
        /// </summary>
        public void Resize()
        {
            using (Image original = GetBitmapFromFile(FromPath))
            {
                using (Image thumb = CreateThumbnail(original))
                {
                    //jpeg is b96b3cae-...
                    if (original.RawFormat.Guid == ImageFormat.Jpeg.Guid)
                    {
                        SaveAsJpeg(thumb);
                    }
                    else
                    {
                        thumb.Save(ToPath, original.RawFormat);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the thumbnail.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        private Image CreateThumbnail(Image original)
        {
            //http://west-wind.com/weblog/posts/283.aspx
            Bitmap bmpOut;
            try
            {
                decimal ratio;
                int newWidth;
                int newHeight;

                //if the height isn't defined, get one with the right aspect ratio
                if (Height == 0)
                {
                    ratio = (decimal)Width / original.Width;
                    Height = (int)(original.Height * ratio);
                }

                //If the image is smaller than a thumbnail just return it
                if (original.Width < Width && original.Height < Height)
                    return new Bitmap(original); //return a copy as we dispose of the original

                if (original.Width > original.Height)
                {
                    //wide image
                    ratio = (decimal)Width / original.Width;
                    newWidth = Width;
                    newHeight = (int)(original.Height * ratio);
                }
                else
                {
                    //tall image
                    ratio = (decimal)Height / original.Height;
                    newHeight = Height;
                    newWidth = (int)(original.Width * ratio);
                }

                // This code creates cleaner (though bigger) thumbnails and properly
                // and handles GIF files better by generating a white background for
                // transparent images (as opposed to black)
                bmpOut = new Bitmap(newWidth, newHeight);

                using (Graphics g = Graphics.FromImage(bmpOut))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                    g.DrawImage(original, 0, 0, newWidth, newHeight);
                }
            }
            catch
            {
                return null;
            }
            return bmpOut;
        }

        private static Bitmap GetBitmapFromFile(string filename)
        {
            return new Bitmap(filename);
        }

        private void SaveAsJpeg(Image img)
        {
            //http://msdn.microsoft.com/en-us/library/bb882583.aspx
            //Quality is 0 (most compression) to 100 (least compression)
            long compression = Compression;

            var jpgEncoder = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(x => x.FormatID == ImageFormat.Jpeg.Guid);

            using (var myEncoderParameters = new EncoderParameters(1))
            {
                using (var myEncoderParameter =
                    new EncoderParameter(Encoder.Quality, compression))
                {
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    img.Save(ToPath, jpgEncoder, myEncoderParameters);
                }
            }
        }

        /// <summary>
		/// Creates a resized bitmap from an existing image on disk. Resizes the image by 
		/// creating an aspect ratio safe image. Image is sized to the larger size of width
		/// height and then smaller size is adjusted by aspect ratio.
		/// 
		/// Image is returned as Bitmap - call Dispose() on the returned Bitmap object
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns>Bitmap or null</returns>
		public static Bitmap ResizeImage(string filename,int width, int height)
		{
			Bitmap bmpOut = null;

			try 
			{
				Bitmap bmp = new Bitmap(filename);
				ImageFormat format = bmp.RawFormat;

				decimal ratio;
				int newWidth = 0;
				int newHeight = 0;

				//*** If the image is smaller than a thumbnail just return it
				if (bmp.Width < width && bmp.Height < height) 
					return bmp;
			
				if (bmp.Width > bmp.Height)
				{
					ratio = (decimal) width / bmp.Width;
					newWidth = width;
					decimal lnTemp = bmp.Height * ratio;
					newHeight = (int)lnTemp;
				}
				else 
				{
					ratio = (decimal) height / bmp.Height;
					newHeight = height;
					decimal lnTemp = bmp.Width * ratio;
					newWidth = (int) lnTemp;
				}

				bmpOut = new Bitmap(newWidth, newHeight);
				Graphics g = Graphics.FromImage(bmpOut);
				g.InterpolationMode =InterpolationMode.HighQualityBicubic;
				g.FillRectangle( Brushes.White,0,0,newWidth,newHeight);
				g.DrawImage(bmp,0,0,newWidth,newHeight);

				//System.Drawing.Image imgOut = loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight,null,IntPtr.Zero);
				bmp.Dispose();
                bmpOut.Dispose();
			}
			catch 
			{
				return null;
			}
		
			return bmpOut;
		}

        /// <summary>
        /// Resizes an image from byte array and returns a Bitmap.
        /// Make sure you Dispose() the bitmap after you're done 
        /// with it!
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ResizeImage(byte[] data, int width, int height)
        {
            Bitmap bmpOut = null;

            try
            {
                Bitmap bmp = new Bitmap(new MemoryStream(data));
                ImageFormat format = bmp.RawFormat;

                decimal ratio;
                int newWidth = 0;
                int newHeight = 0;

                //*** If the image is smaller than a thumbnail just return it
                if (bmp.Width < width && bmp.Height < height)
                    return bmp;

                if (bmp.Width > bmp.Height)
                {
                    ratio = (decimal)width / bmp.Width;
                    newWidth = width;
                    decimal lnTemp = bmp.Height * ratio;
                    newHeight = (int)lnTemp;
                }
                else
                {
                    ratio = (decimal)height / bmp.Height;
                    newHeight = height;
                    decimal lnTemp = bmp.Width * ratio;
                    newWidth = (int)lnTemp;
                }

                bmpOut = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                g.DrawImage(bmp, 0, 0, newWidth, newHeight);

                //System.Drawing.Image imgOut = loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight,null,IntPtr.Zero);
                bmp.Dispose();
                //bmpOut.Dispose();
            }
            catch
            {
                return null;
            }

            return bmpOut;
        }

		public static bool ResizeImage(string filename, string outputFilename, int width, int height)
		{
			Bitmap bmpOut = null;

			try 
			{
				Bitmap bmp = new Bitmap(filename);
				ImageFormat format = bmp.RawFormat;

				decimal ratio;
				int newWidth = 0;
				int newHeight = 0;

				//*** If the image is smaller than a thumbnail just return it
				if (bmp.Width < width && bmp.Height < height) 
				{ 
                    if (outputFilename != filename)
					    bmp.Save(outputFilename);
                    bmp.Dispose();
					return true;
				}

				if (bmp.Width > bmp.Height)
				{
					ratio = (decimal) width / bmp.Width;
					newWidth = width;
					decimal temp = bmp.Height * ratio;
					newHeight = (int)temp;
				}
				else 
				{
					ratio = (decimal) height / bmp.Height;
					newHeight = height;
					decimal lnTemp = bmp.Width * ratio;
					newWidth = (int) lnTemp;
				}

				bmpOut = new Bitmap(newWidth, newHeight);
				Graphics g = Graphics.FromImage(bmpOut);
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.FillRectangle( Brushes.White,0,0,newWidth,newHeight);
				g.DrawImage(bmp,0,0,newWidth,newHeight);

				//System.Drawing.Image imgOut = loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight,null,IntPtr.Zero);
				bmp.Dispose();

				bmpOut.Save(outputFilename,format);
				bmpOut.Dispose();
			}
			catch(Exception ex) 
			{
                var msg = ex.GetBaseException();
				return false;
			}
		
			return true;
		}		
    }
}
