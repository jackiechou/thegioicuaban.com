using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using Devarchive_Net.Navigation;

namespace CommonLibrary.UI.Template
{
    public class Command
    {
        #region Command functionality

        private string m_CommandName = "";

        public Command(string commandName)
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
        public object GetWizardPage(object data)
        {
            bool errorLogged = false;
            try
            {
                Dictionary<string, object> param =
                    (Dictionary<string, object>)data;
                int pageID = (int)param["pageID"];
                object customData = param["data"];

                string controlPath = 
                    m_NavigationData.Find(x => x.Key == pageID).Value;

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
                        return TemplateViewManager.RenderView(controlPath, customData);
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

        #region Wizard Navigation

        private static List<KeyValuePair<int, string>>
            m_NavigationData = new List<KeyValuePair<int, string>>()
            {
                new KeyValuePair<int, string>(1,Pages.Controls.Welcome),
                new KeyValuePair<int, string>(2,Pages.Controls.AcceptLicenceAgreement),
                new KeyValuePair<int, string>(3,Pages.Controls.PleaseClickProceedButton),
                new KeyValuePair<int, string>(4,Pages.Controls.ConfirmationMessage),
                new KeyValuePair<int, string>(5,Pages.Controls.Installing),
                new KeyValuePair<int, string>(6,Pages.Controls.ThankYouMessage)
            };

        #endregion
    }
}