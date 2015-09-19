using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

namespace CommonLibrary.UI.Images
{
    /// <summary>
    /// Helper to create thumbnails
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Writes the image using the content type derived from file
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="filename">The filename.</param>
        public static void WriteImage(HttpContext context, string filename)
        {
            context.Response.ContentType = GetContentType(Path.GetExtension(filename));
            context.Response.TransmitFile(filename);
        }

        /// <summary>
        /// Writes the image with an overriden extension type (no extension of disc file?)
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="extension">The extension.</param>
        public static void WriteImage(HttpContext context, string filename, string extension)
        {
            context.Response.ContentType = GetContentType(extension);
            context.Response.TransmitFile(filename);
        }

        /// <summary>
        /// Writes a simple thumbnail. Suitable up to 150px (for larger, use <see cref="WriteThumbnail"/>)
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="width">The width.</param>
        public static void WriteThumbnailSimple(HttpContext context, string filename, int width)
        {
            using (Image imgThumb = CreateThumbnailSimple(filename, width))
            {
                HttpContext.Current.Response.ContentType = "images/jpeg";
                imgThumb.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            }
        }

        /// <summary>
        /// Writes the thumbnail. Suitable for larger sizes than <see cref="WriteThumbnailSimple"/>.
        /// This overload sets just a maximum width (eg 200px- rounding errors may make it +/-1px).
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="width">The (maximum) width.</param>
        public static void WriteThumbnail(HttpContext context, string filename, int width)
        {
            using (Image imgThumb = CreateThumbnail(filename, width))
            {
                HttpContext.Current.Response.ContentType = "images/jpeg";
                imgThumb.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            }
        }

        /// <summary>
        /// Writes the thumbnail. Suitable for larger sizes than <see cref="WriteThumbnailSimple"/>.
        /// This overload sets a maximum width or height depending on dimensions
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="width">The (maximum) width.</param>
        /// <param name="height">The (maximum) height.</param>
        public static void WriteThumbnail(HttpContext context, string filename, int width, int height)
        {
            using (Image imgThumb = CreateThumbnail(filename, width, height))
            {
                HttpContext.Current.Response.ContentType = "images/jpeg";
                imgThumb.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            }
        }

        public static Image CreateThumbnailSimple(string filename, int width)
        {
            Image imgThumb;
            //get bitmap from file (see also GetBitmapFromEmbeddedResource)
            using (Bitmap bitmap = GetBitmapFromFile(filename))
            {
                //create a callback delegate
                Image.GetThumbnailImageAbort myCallback = delegate { return false; };

                //preserve aspect ratio
                decimal aspectRatio = (decimal)width / bitmap.Width;
                if (aspectRatio < 0) aspectRatio = 1;
                int thumbnailHeight = Convert.ToInt32(bitmap.Height * aspectRatio);
                if (thumbnailHeight > (width * 2))
                {
                    thumbnailHeight = width * 2;
                    aspectRatio = (decimal)thumbnailHeight / bitmap.Height;
                    width = Convert.ToInt32(bitmap.Width * aspectRatio);
                }

                //images with thumbnails (from most digital cameras) have embedded thumbnails so scale badly
                //rotating 360 apparently kills the embedded thumbnail
                //source: http://aspnet.4guysfromrolla.com/articles/012203-1.2.aspx
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                //grab the thumbail from the image
                imgThumb = bitmap.GetThumbnailImage(width, thumbnailHeight, myCallback, IntPtr.Zero);
            }
            return imgThumb;
        }

        public static Image CreateThumbnail(string filename, int width)
        {
            //overload for just maximum width
            return CreateThumbnail(filename, width, 0);
        }

        public static Image CreateThumbnail(string filename, int width, int height)
        {
            //http://west-wind.com/weblog/posts/283.aspx
            Bitmap bmpOut;
            try
            {
                //get bitmap from file (see also GetBitmapFromEmbeddedResource)
                using (Bitmap bmp = GetBitmapFromFile(filename))
                {
                    decimal ratio;
                    int newWidth;
                    int newHeight;

                    //if the height isn't defined, get one with the right aspect ratio
                    if (height == 0)
                    {
                        ratio = (decimal)width / bmp.Width;
                        height = (int)(bmp.Height * ratio);
                    }

                    //If the image is smaller than a thumbnail just return it
                    if (bmp.Width < width && bmp.Height < height)
                        return new Bitmap(bmp); //return a copy as we dispose of the original

                    if (bmp.Width > bmp.Height)
                    {
                        //wide image
                        ratio = (decimal)width / bmp.Width;
                        newWidth = width;
                        newHeight = (int)(bmp.Height * ratio);
                    }
                    else
                    {
                        //tall image
                        ratio = (decimal)height / bmp.Height;
                        newHeight = height;
                        newWidth = (int)(bmp.Width * ratio);
                    }

                    // This code creates cleaner (though bigger) thumbnails and properly
                    // and handles GIF files better by generating a white background for
                    // transparent images (as opposed to black)
                    bmpOut = new Bitmap(newWidth, newHeight);

                    using (Graphics g = Graphics.FromImage(bmpOut))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                        g.DrawImage(bmp, 0, 0, newWidth, newHeight);
                    }
                }
            }
            catch
            {
                return null;
            }
            return bmpOut;
        }

        /// <summary>
        /// Writes image from database table
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="id">The id.</param>
        public static void WriteFromDatabase(HttpContext context, int id)
        {
            string cs = WebConfigurationManager.ConnectionStrings["MyCS"].ConnectionString;
            //replace this with your DAL
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand command = new SqlCommand("SELECT [Thumbnail] FROM [Images] WHERE Id=@Id", con);
                command.Parameters.Add("Id", SqlDbType.Int).Value = id;
                byte[] image = (byte[])command.ExecuteScalar();
                if (image != null)
                {
                    context.Response.ContentType = "image/jpeg";
                    context.Response.OutputStream.Write(image, 0, image.Length);
                }
            }
        }

        /// <summary>
        /// Gets the bitmap from file. Swap in <see cref="GetBitmapFromEmbeddedResource"/> if required
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static Bitmap GetBitmapFromFile(string filename)
        {
            return new Bitmap(filename);
        }

        /// <summary>
        /// Gets the bitmap from embedded resource. Swap in <see cref="GetBitmapFromFile"/> if required
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Bitmap GetBitmapFromEmbeddedResource(string name)
        {
            //or use an embedded resource
            Assembly executingAssembly =
                Assembly.GetExecutingAssembly();
            Stream stm = executingAssembly.GetManifestResourceStream(name);
            return new Bitmap(stm);
        }

        /// <summary>
        /// Gets the Response.ContentType from file extension (for images)
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".jpg":
                    return "images/jpeg";
                case ".jpeg":
                    return "images/jpeg";
                case ".gif":
                    return "images/gif";
                case ".bmp":
                    return "images/bmp";
                case ".tiff":
                    return "images/tiff";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
