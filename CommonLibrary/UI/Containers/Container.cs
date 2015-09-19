using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Modules;
using System.Web.UI.WebControls;
using System.Web.UI;
using CommonLibrary.Entities.Modules.Actions;
using System.Web.UI.HtmlControls;
using CommonLibrary.UI.Modules;
using CommonLibrary.Common;
using CommonLibrary.Entities.Portal;
using CommonLibrary.UI.Skins;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security.Permissions;
using System.Collections;
using CommonLibrary.Entities.Host;
using System.IO;
using CommonLibrary.Application;
using CommonLibrary.UI.Skins.EventListeners;
using CommonLibrary.Framework;

namespace CommonLibrary.UI.Containers
{
    public class Container : System.Web.UI.UserControl
    {
        private const string c_ContainerAdminBorder = "containerAdminBorder";
        private HtmlContainerControl _ContentPane;
        private string _ContainerSrc;
        private ModuleInfo _ModuleConfiguration;
        private ModuleHost _ModuleHost;
        protected HtmlContainerControl ContentPane
        {
            get
            {
                if (_ContentPane == null)
                {
                    _ContentPane = FindControl(Globals.glbDefaultPane) as HtmlContainerControl;
                }
                return _ContentPane;
            }
        }
        protected PortalSettings PortalSettings
        {
            get { return PortalController.GetCurrentPortalSettings(); }
        }
        public IModuleControl ModuleControl
        {
            get
            {
                IModuleControl _ModuleControl = null;
                if (ModuleHost != null)
                {
                    _ModuleControl = ModuleHost.ModuleControl;
                }
                return _ModuleControl;
            }
        }
        public ModuleInfo ModuleConfiguration
        {
            get { return _ModuleConfiguration; }
        }
        public ModuleHost ModuleHost
        {
            get { return _ModuleHost; }
        }
        public Skin ParentSkin
        {
            get { return Skin.GetParentSkin(this); }
        }
        public string ContainerPath
        {
            get { return this.TemplateSourceDirectory + "/"; }
        }
        public string ContainerSrc
        {
            get { return _ContainerSrc; }
            set { _ContainerSrc = value; }
        }
        private void ProcessChildControls(Control control)
        {
            IActionControl objActions;
            ISkinControl objSkinControl;
            foreach (Control objChildControl in control.Controls)
            {
                objActions = objChildControl as IActionControl;
                if (objActions != null)
                {
                    objActions.ModuleControl = ModuleControl;
                    objActions.Action += ModuleAction_Click;
                }
                objSkinControl = objChildControl as ISkinControl;
                if (objSkinControl != null)
                {
                    objSkinControl.ModuleControl = ModuleControl;
                }
                if (objChildControl.HasControls())
                {
                    ProcessChildControls(objChildControl);
                }
            }
        }
        private void ProcessContentPane()
        {
            if (!String.IsNullOrEmpty(ModuleConfiguration.Alignment))
            {
                if (ContentPane.Attributes["class"] != null)
                {
                    ContentPane.Attributes["class"] = ContentPane.Attributes["class"] + " DNNAlign" + ModuleConfiguration.Alignment.ToLower();
                }
                else
                {
                    ContentPane.Attributes["class"] = "DNNAlign" + ModuleConfiguration.Alignment.ToLower();
                }
            }
            if (!String.IsNullOrEmpty(ModuleConfiguration.Color))
            {
                ContentPane.Style["background-color"] = ModuleConfiguration.Color;
            }
            if (!String.IsNullOrEmpty(ModuleConfiguration.Border))
            {
                ContentPane.Style["border-top"] = String.Format("{0}px #000000 solid", ModuleConfiguration.Border);
                ContentPane.Style["border-bottom"] = String.Format("{0}px #000000 solid", ModuleConfiguration.Border);
                ContentPane.Style["border-right"] = String.Format("{0}px #000000 solid", ModuleConfiguration.Border);
                ContentPane.Style["border-left"] = String.Format("{0}px #000000 solid", ModuleConfiguration.Border);
            }
            string adminMessage = Null.NullString;
            string viewRoles = Null.NullString;
            if (ModuleConfiguration.InheritViewPermissions)
            {
                viewRoles = TabPermissionController.GetTabPermissions(ModuleConfiguration.TabID, ModuleConfiguration.PortalID).ToString("VIEW");
            }
            else
            {
                viewRoles = ModuleConfiguration.ModulePermissions.ToString("VIEW");
            }
            viewRoles = viewRoles.Replace(";", string.Empty).Trim().ToLowerInvariant();

            bool showMessage = false;

            if (viewRoles == PortalSettings.AdministratorRoleName.ToLowerInvariant())
            {
                adminMessage = CommonLibrary.Services.Localization.Localization.GetString("ModuleVisibleAdministrator.Text");
                showMessage = !ModuleConfiguration.HideAdminBorder;
            }
            if (ModuleConfiguration.StartDate >= DateTime.Now)
            {
                adminMessage = string.Format(CommonLibrary.Services.Localization.Localization.GetString("ModuleEffective.Text"), ModuleConfiguration.StartDate.ToShortDateString());
                showMessage = true;
            }
            if (ModuleConfiguration.EndDate <= DateTime.Now)
            {
                adminMessage = string.Format(CommonLibrary.Services.Localization.Localization.GetString("ModuleExpired.Text"), ModuleConfiguration.EndDate.ToShortDateString());
                showMessage = true;
            }
            if (showMessage)
            {
                AddAdministratorOnlyHighlighting(adminMessage);
            }
        }
        private void AddAdministratorOnlyHighlighting(string message)
        {
            string cssclass = ContentPane.Attributes["class"];
            if (string.IsNullOrEmpty(cssclass))
            {
                ContentPane.Attributes["class"] = c_ContainerAdminBorder;
            }
            else
            {
                ContentPane.Attributes["class"] = string.Format("{0} {1}", cssclass.Replace(c_ContainerAdminBorder, "").Trim().Replace(" ", " "), c_ContainerAdminBorder);
            }

            ContentPane.Controls.Add(new LiteralControl(string.Format("<div class=\"NormalRed DNNAligncenter\">{0}</div>", message)));
        }
        private void ProcessFooter()
        {
            if (!String.IsNullOrEmpty(ModuleConfiguration.Footer))
            {
                Label objLabel = new Label();
                objLabel.Text = ModuleConfiguration.Footer;
                objLabel.CssClass = "Normal";
                ContentPane.Controls.Add(objLabel);
            }
            if (!Globals.IsAdminControl())
            {
                ContentPane.Controls.Add(new LiteralControl("<!-- End_Module_" + ModuleConfiguration.ModuleID.ToString() + " -->"));
            }
        }
        private void ProcessHeader()
        {
            if (!Globals.IsAdminControl())
            {
                ContentPane.Controls.Add(new LiteralControl("<!-- Start_Module_" + ModuleConfiguration.ModuleID.ToString() + " -->"));
            }
            if (!String.IsNullOrEmpty(ModuleConfiguration.Header))
            {
                Label objLabel = new Label();
                objLabel.Text = ModuleConfiguration.Header;
                objLabel.CssClass = "Normal";
                ContentPane.Controls.Add(objLabel);
            }
        }
        private void ProcessModule()
        {
            if (ContentPane != null)
            {
                ProcessContentPane();
                ProcessHeader();
                _ModuleHost = new ModuleHost(ModuleConfiguration, ParentSkin);
                ContentPane.Controls.Add(ModuleHost);
                ProcessFooter();
                if (ModuleHost != null && ModuleControl != null)
                {
                    ProcessChildControls(this);
                }
                ProcessStylesheets(ModuleHost != null);
            }
        }
        private void ProcessStylesheets(bool includeModuleCss)
        {
            bool blnUpdateCache = false;
            CDefault DefaultPage = (CDefault)Page;
            string ID;
            Hashtable objCSSCache = null;
            if (Host.PerformanceSetting != Common.Globals.PerformanceSettings.NoCaching)
            {
                objCSSCache = (Hashtable)DataCache.GetCache("CSS");
            }
            if (objCSSCache == null)
            {
                objCSSCache = new Hashtable();
            }
            ID = Globals.CreateValidID(ContainerPath);
            if (objCSSCache.ContainsKey(ID) == false)
            {
                if (File.Exists(Server.MapPath(ContainerPath) + "container.css"))
                {
                    objCSSCache[ID] = ContainerPath + "container.css";
                }
                else
                {
                    objCSSCache[ID] = "";
                }
                blnUpdateCache = true;
            }
            if (objCSSCache[ID] != null && !String.IsNullOrEmpty(objCSSCache[ID].ToString()))
            {
                DefaultPage.AddStyleSheet(ID, objCSSCache[ID].ToString());
            }
            ID = Globals.CreateValidID(ContainerSrc.Replace(".ascx", ".css"));
            if (objCSSCache.ContainsKey(ID) == false)
            {
                if (File.Exists(Server.MapPath(ContainerSrc.Replace(".ascx", ".css"))))
                {
                    objCSSCache[ID] = ContainerSrc.Replace(".ascx", ".css");
                }
                else
                {
                    objCSSCache[ID] = "";
                }
                blnUpdateCache = true;
            }
            if (objCSSCache[ID] != null && !String.IsNullOrEmpty(objCSSCache[ID].ToString()))
            {
                DefaultPage.AddStyleSheet(ID, objCSSCache[ID].ToString());
            }
            if (includeModuleCss)
            {
                string controlSrc = ModuleConfiguration.ModuleControl.ControlSrc;
                string folderName = ModuleConfiguration.DesktopModule.FolderName;
                ID = Globals.CreateValidID(Common.Globals.ApplicationPath + "/DesktopModules/" + folderName);
                if (objCSSCache.ContainsKey(ID) == false)
                {
                    objCSSCache[ID] = "";
                    if (File.Exists(Server.MapPath(Common.Globals.ApplicationPath + "/DesktopModules/" + folderName + "/module.css")))
                    {
                        objCSSCache[ID] = Common.Globals.ApplicationPath + "/DesktopModules/" + folderName + "/module.css";
                    }
                    else
                    {
                        if (controlSrc.ToLower().EndsWith(".ascx"))
                        {
                            if (File.Exists(Server.MapPath(Common.Globals.ApplicationPath + "/" + controlSrc.Substring(0, controlSrc.LastIndexOf("/") + 1)) + "module.css"))
                            {
                                objCSSCache[ID] = Common.Globals.ApplicationPath + "/" + controlSrc.Substring(0, controlSrc.LastIndexOf("/") + 1) + "module.css";
                            }
                        }
                    }
                    blnUpdateCache = true;
                }
                if (objCSSCache.ContainsKey(ID) && !string.IsNullOrEmpty(objCSSCache[ID].ToString()))
                {
                    DefaultPage.AddStyleSheet(ID, objCSSCache[ID].ToString(), true);
                }
            }
            if (Host.PerformanceSetting != Common.Globals.PerformanceSettings.NoCaching)
            {
                if (blnUpdateCache)
                {
                    DataCache.SetCache("CSS", objCSSCache);
                }
            }
        }
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            foreach (ContainerEventListener listener in AppContext.Current.ContainerEventListeners)
            {
                if (listener.EventType == ContainerEventType.OnContainerInit)
                {
                    listener.ContainerEvent.Invoke(this, new ContainerEventArgs(this));
                }
            }
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            foreach (ContainerEventListener listener in AppContext.Current.ContainerEventListeners)
            {
                if (listener.EventType == ContainerEventType.OnContainerLoad)
                {
                    listener.ContainerEvent.Invoke(this, new ContainerEventArgs(this));
                }
            }
        }
        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);
            foreach (ContainerEventListener listener in AppContext.Current.ContainerEventListeners)
            {
                if (listener.EventType == ContainerEventType.OnContainerPreRender)
                {
                    listener.ContainerEvent.Invoke(this, new ContainerEventArgs(this));
                }
            }
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            foreach (ContainerEventListener listener in AppContext.Current.ContainerEventListeners)
            {
                if (listener.EventType == ContainerEventType.OnContainerUnLoad)
                {
                    listener.ContainerEvent.Invoke(this, new ContainerEventArgs(this));
                }
            }
        }

        public void SetModuleConfiguration(ModuleInfo configuration)
        {
            _ModuleConfiguration = configuration;
            ProcessModule();
        }
        private void ModuleAction_Click(object sender, ActionEventArgs e)
        {
            foreach (ModuleActionEventListener Listener in ParentSkin.ActionEventListeners)
            {
                if (e.ModuleConfiguration.ModuleID == Listener.ModuleID)
                {
                    Listener.ActionEvent.Invoke(sender, e);
                }
            }
        }
    }
}
