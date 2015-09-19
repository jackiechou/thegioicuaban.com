using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;

namespace CommonLibrary.Services.Url.FriendlyUrl
{
    public class RouteHandler : IRouteHandler
    {
        private string _physicalFile;
        public RouteHandler(string physicalFile)
        {
            _physicalFile = physicalFile;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            HttpContext.Current.Items["RouteData"] = requestContext.RouteData;
            return BuildManager.CreateInstanceFromVirtualPath(_physicalFile, typeof(Page)) as Page;
        }
    }
 
}
