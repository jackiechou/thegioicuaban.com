using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Reflection;

namespace CommonLibrary.Ajax.CallbackHandler
{
    public enum PostBackModes
    {
        /// No Form data is posted (but there may still be some post state)
        None,

        /// <summary>
        /// No POST data is posted back to the server
        /// </summary>
        Get,
        /// <summary>
        /// Only standard POST data is posted back - ASP.NET Post stuff left out
        /// </summary>
        Post,
        /// <summary>
        /// Posts back POST data but skips ViewState and EventTargets
        /// </summary>
        PostNoViewstate,
        /// <summary>
        /// Posts only the method parameters and nothing else
        /// </summary>
        PostMethodParametersOnly
    }

    public enum JavaScriptCodeLocationTypes
    {
        /// <summary>
        /// Causes the Javascript code to be embedded into the page on every 
        /// generation. Fully self-contained.
        /// <seealso>Enumeration JavaScriptCodeLocationTypes</seealso>
        /// </summary>
        EmbeddedInPage,
        /// <summary>
        /// Keeps the .js file as an external file in the Web application. If this is 
        /// set you should set the &lt;&lt;%= TopicLink([ScriptLocation],[_1Q01F9K4D]) 
        /// %&gt;&gt; Property to point at the location of the file.
        /// 
        /// This option requires that you deploy the .js file with your application.
        /// <seealso>Enumeration JavaScriptCodeLocationTypes</seealso>
        /// </summary>
        ExternalFile,
        /// <summary>
        /// ASP.NET 2.0 option to generate a WebResource.axd call that feeds the .js 
        /// file to the client.
        /// <seealso>Enumeration JavaScriptCodeLocationTypes</seealso>
        /// </summary>
        WebResource,
        /// <summary>
        /// Don't include any script - assume the page owner will handle it all
        /// </summary>
        None
    }

    public enum ProxyClassGenerationModes
    {
        /// <summary>
        /// The proxy is generated inline of the page.
        /// </summary>
        Inline,
        /// <summary>
        /// No proxy is generated at all
        /// </summary>
        None,
        /// <summary>
        /// Works only with CallbackHandler implementations
        /// that run as handlers at a distinct URL.
        /// JsonCallbacks.ashx/jsdebug
        /// </summary>
        jsdebug
    }
}
