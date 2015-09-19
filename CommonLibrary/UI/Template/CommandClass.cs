using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Devarchive_Net.Navigation;
using System.Reflection;
using System.IO;

namespace CommonLibrary.UI.Template
{
    public class CommandClass
    {	    
         #region Command functionality

        private string m_CommandName = "";

        public CommandClass(string commandName)
        {
            m_CommandName = commandName;
        }

        public static Command Create(string commandName)
        {
            return new Command(commandName);
        }

        public object Execute(object data)
        {
            Type type = this.GetType();
            MethodInfo method = type.GetMethod(m_CommandName);
            object[] args = new object[] { data };
            try
            {
                return method.Invoke(this, args);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion

        #region Public execution commands

        /// <summary>
        /// returns rendered control's string representation.
        /// object "data" should be passed from javascript method 
        /// as array of objects consisting of two objects,
        /// first - pageID - integer identificator by which we will
        /// lookup real control path; second object may be some data
        /// that the control needs.
        /// </summary>
        public object GetWizardPage(string controlPath)
        {
            bool errorLogged = false;
            try
            {            

                if (!String.IsNullOrEmpty(controlPath))
                {
                    if(
                        controlPath.ToLower()
                        .EndsWith(".htm") 
                        ||
                        controlPath.ToLower()
                        .EndsWith(".html") 
                        ||
                        controlPath.ToLower()
                        .EndsWith(".txt"))
                    {
                        string result = "";
                        using (
                                TextReader tr = 
                                    new StreamReader(
                                        HttpContext.Current.Server.MapPath(controlPath)
                                        )
                                    )
                        {
                            result = tr.ReadToEnd();
                        }
                        return new ContentsResponse(result, string.Empty, string.Empty);
                    }
                    else
                    {
                        return TemplateViewManager.RenderView(controlPath);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                // Log error
                errorLogged = true;
            }
            if (!errorLogged)
            {
                // Log custom error saying 
                // we did not find the page
            }
            return ContentsResponse.Empty;
        }

        #endregion      
    
    }
}