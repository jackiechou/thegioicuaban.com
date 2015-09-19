using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.handlers
{
    /// <summary>
    /// Summary description for KeepSessionAlive
    /// </summary>
    public class KeepSessionAlive : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Session["KeepSessionAlive"] = DateTime.Now;
            DateTime dateTimeBeginRequest = DateTime.Now;
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