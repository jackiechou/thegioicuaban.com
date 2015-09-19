using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace CommonLibrary.Services.Url.FriendlyUrl
{
    public static class RouteCollectionExtensions
    {
        public static Route MapPageRoute(this RouteCollection route, string routeName, string routeUrl, string physicalFile)
        {
            return MapPageRoute(route, routeName, routeUrl, physicalFile, null, null, null);
        }

        public static Route MapPageRoute(this RouteCollection route, string routeName, string routeUrl, string physicalFile, RouteValueDictionary defaults)
        {
            return MapPageRoute(route, routeName, routeUrl, physicalFile, defaults, null, null);
        }

        public static Route MapPageRoute(this RouteCollection route, string routeName, string routeUrl, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints)
        {
            return MapPageRoute(route, routeName, routeUrl, physicalFile, defaults, constraints, null);
        }

        public static Route MapPageRoute(this RouteCollection route, string routeName, string routeUrl, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
        {
            if (routeUrl == null)
            {
                throw new ArgumentNullException("routeUrl");
            }
            Route item = new Route(routeUrl, defaults, constraints, dataTokens, new RouteHandler(physicalFile));
            route.Add(routeName, item);
            return item;
        }
    }
}
