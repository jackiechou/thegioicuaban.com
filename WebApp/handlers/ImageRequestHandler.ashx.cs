using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace WebApp.handlers
{    
    public class ImageRequestHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");

            context.Response.Clear();

            if (context.Request.QueryString.Count != 0)
            {
                var storedImage = context.Session["STORED_IMAGE"] as byte[];
                if (storedImage != null)
                {
                    Image image = GetImage(storedImage);
                    if (image != null)
                    {
                        context.Response.ContentType = "image/jpeg";
                        image.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                    }
                }
            }
        }

        private Image GetImage(byte[] storedImage)
        {
            var stream = new MemoryStream(storedImage);
            return Image.FromStream(stream);
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}