using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace CommonLibrary.UI
{
    public class BaseMasterPage : MasterPage
    {
        public static string BaseURL
        {
            get
            {
                HttpContext context = HttpContext.Current;
                string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/');
                return baseUrl;
            }
        }
        /// <summary>
        /// Returns the name of the virtual folder where our project lives
        /// </summary>
        public static string BaseVirtualAppPath
        {
            get
            {                
                string url = HttpContext.Current.Request.ApplicationPath;
                if (url.EndsWith("/"))
                    return url;
                else
                    return url + "/";
            }
        }

    }
}
