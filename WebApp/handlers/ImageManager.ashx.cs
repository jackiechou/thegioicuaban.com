using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebApp.handlers
{ 
    public class ImageManager : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //string application_path = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            string _uploadFolder = System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]+"/uploaded";

            HttpPostedFile postedFile = context.Request.Files["myfile"];
            string fileName = postedFile.FileName;            
            byte[] fileBytes = new byte[postedFile.ContentLength];
            string file_path =  HttpContext.Current.Server.MapPath("~/"+_uploadFolder) + "/" + fileName;
            if (File.Exists(file_path))
                File.Delete(file_path);

            using (Stream fileInputStream = postedFile.InputStream)
            {
                fileInputStream.Read(fileBytes, 0, postedFile.ContentLength);
            }
            File.WriteAllBytes(file_path, fileBytes);

            Uri uriSiteRoot = new Uri(context.Request.Url.GetLeftPart(UriPartial.Authority));
            var output = uriSiteRoot + _uploadFolder +"/"+  fileName;
            //var output = string.Format("{0}/{1}",_uploadFolder,fileName);
            context.Response.ContentType = "text/plain";
            context.Response.Write(output);
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