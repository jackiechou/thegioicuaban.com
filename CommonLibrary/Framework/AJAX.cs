using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using CommonLibrary.UI.WebControls;

namespace CommonLibrary.Framework
{
    public class AJAX
    {
        #region "Public Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddScriptManager is used internally by the framework to add a ScriptManager control to the page
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void AddScriptManager(Page objPage)
        {
            if (GetScriptManager(objPage) == null)
            {
                using (ScriptManager objScriptManager = new ScriptManager { ID = "ScriptManager", EnableScriptGlobalization = true })
                {
                    if (objPage.Form != null)
                    {
                        objPage.Form.Controls.AddAt(0, objScriptManager);
                        if (HttpContext.Current.Items["System.Web.UI.ScriptManager"] == null)
                        {
                            HttpContext.Current.Items.Add("System.Web.UI.ScriptManager", true);
                        }
                    }
                }
            }
        }

        public static ScriptManager GetScriptManager(Page objPage)
        {
            return objPage.FindControl("ScriptManager") as ScriptManager;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// IsEnabled can be used to determine if AJAX has been enabled already as we
        /// only need one Script Manager per page.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool IsEnabled()
        {
            if (HttpContext.Current.Items["System.Web.UI.ScriptManager"] == null)
            {
                return false;
            }
            else
            {
                return (bool)HttpContext.Current.Items["System.Web.UI.ScriptManager"];
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// IsInstalled can be used to determine if AJAX is installed on the server
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool IsInstalled()
        {
            return true;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Allows a control to be excluded from UpdatePanel async callback
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void RegisterPostBackControl(Control objControl)
        {
            ScriptManager objScriptManager = GetScriptManager(objControl.Page);
            if (objScriptManager != null)
            {
                objScriptManager.RegisterPostBackControl(objControl);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// RegisterScriptManager must be used by developers to instruct the framework that AJAX is required on the page
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void RegisterScriptManager()
        {
            if (!IsEnabled())
            {
                HttpContext.Current.Items.Add("System.Web.UI.ScriptManager", true);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// RemoveScriptManager will remove the ScriptManager control during Page Render if the RegisterScriptManager has not been called
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void RemoveScriptManager(Page objPage)
        {
            if (IsEnabled() == false)
            {
                Control objControl = objPage.FindControl("ScriptManager");
                if ((objControl != null))
                {
                    objPage.Form.Controls.Remove(objControl);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Wraps a control in an update panel
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static Control WrapUpdatePanelControl(Control objControl, bool blnIncludeProgress)
        {
            UpdatePanel updatePanel = new UpdatePanel();
            updatePanel.ID = objControl.ID + "_UP";
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

            Control objContentTemplateContainer = updatePanel.ContentTemplateContainer;

            for (int i = 0; i <= objControl.Parent.Controls.Count - 1; i++)
            {
                //find offset of original control
                if (objControl.Parent.Controls[i].ID == objControl.ID)
                {
                    //if ID matches
                    objControl.Parent.Controls.AddAt(i, updatePanel);
                    //insert update panel in that position
                    objContentTemplateContainer.Controls.Add(objControl);
                    //inject passed in control into update panel
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            if (blnIncludeProgress)
            {
                //create image for update progress control
                System.Web.UI.WebControls.Image objImage = new System.Web.UI.WebControls.Image();
                objImage.ImageUrl = "~/images/progressbar.gif";
                //hardcoded
                objImage.AlternateText = "ProgressBar";

                UpdateProgress updateProgress = new UpdateProgress();
                updateProgress.AssociatedUpdatePanelID = updatePanel.ID;
                updateProgress.ID = updatePanel.ID + "_Prog";
                updateProgress.ProgressTemplate = new LiteralTemplate(objImage);

                objContentTemplateContainer.Controls.Add(updateProgress);
            }

            return updatePanel;
        }

        #endregion
    }
}
