using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AjaxControlToolkit;

namespace WebApp.handlers
{
    /// <summary>
    /// Summary description for CombineScriptsHandler
    /// </summary>
    public class CombineScriptsHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");

            //if (!ToolkitScriptManager.OutputCombinedScriptFile(context))
            //{
            //    throw new InvalidOperationException("Combined script file output failed unexpectedly.");
            //}
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