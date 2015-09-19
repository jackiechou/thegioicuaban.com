using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CommonLibrary;
using CommonLibrary.ComponentModel;

namespace WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //Global Configs ==============================================================================            
            ComponentFactory.Container = new SimpleContainer();
            Settings.ConnectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            Application["OnlineUsers"] = 0;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineUsers"] = (int)Application["OnlineUsers"] + 1;
            Application.UnLock();

            Session["__ISREFRESH"] = null;
            Session["ASPNETShoppingCart"] = null;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            //get reference to the source of the exception chain
            Exception lastError = Server.GetLastError();
            Exception ex;
            if (lastError.InnerException != null)
                ex = lastError.InnerException;
            else
                ex = lastError.GetBaseException();
            // TODO: Log the error somehow.
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineUsers"] = (int)Application["OnlineUsers"] - 1;
            Application.UnLock();
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}