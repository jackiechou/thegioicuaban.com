using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Common;
using System.Web.UI.WebControls;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.UI.Utilities;
using CommonLibrary.Entities.Portal;
using System.Web.UI.HtmlControls;
using CommonLibrary.Entities.Modules;
using System.Web;

namespace CommonLibrary.UI.Skins
{
    public class Pane
    {
        private Dictionary<string, Containers.Container> _containers;
        private string _name;
        private HtmlContainerControl _paneControl;
        private const string c_PaneOutline = "paneOutline";
        public Pane(HtmlContainerControl pane)
        {
            _paneControl = pane;
            _name = pane.ID;
        }
        public Pane(string name, HtmlContainerControl pane)
        {
            _paneControl = pane;
            _name = name;
        }
        protected Dictionary<string, Containers.Container> Containers
        {
            get
            {
                if (_containers == null)
                {
                    _containers = new Dictionary<string, Containers.Container>();
                }
                return _containers;
            }
        }
        protected string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        protected HtmlContainerControl PaneControl
        {
            get { return _paneControl; }
            set { _paneControl = value; }
        }
        protected PortalSettings PortalSettings
        {
            get { return PortalController.GetCurrentPortalSettings(); }
        }
        private Containers.Container LoadContainerByPath(string ContainerPath)
        {
            if (ContainerPath.ToLower().IndexOf("/skins/") != -1 || ContainerPath.ToLower().IndexOf("/skins\\") != -1 || ContainerPath.ToLower().IndexOf("\\skins\\") != -1 || ContainerPath.ToLower().IndexOf("\\skins/") != -1)
            {
                throw new System.Exception();
            }
            Containers.Container ctlContainer = null;
            try
            {
                string ContainerSrc = ContainerPath;
                if (ContainerPath.IndexOf(Common.Globals.ApplicationPath, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    ContainerPath = ContainerPath.Remove(0, Common.Globals.ApplicationPath.Length);
                }
                ctlContainer = ControlUtilities.LoadControl<Containers.Container>(PaneControl.Page, ContainerPath);
                ctlContainer.ContainerSrc = ContainerSrc;
                ctlContainer.DataBind();
            }
            catch (Exception exc)
            {
                ModuleLoadException lex = new ModuleLoadException(Skin.MODULELOAD_ERROR, exc);
                if (TabPermissionController.CanAdminPage())
                {
                    PaneControl.Controls.Add(new ErrorContainer(PortalSettings, string.Format(Skin.CONTAINERLOAD_ERROR, ContainerPath), lex).Container);
                }
                Exceptions.LogException(lex);
            }
            return ctlContainer;
        }
        private Containers.Container LoadModuleContainer(ModuleInfo objModule)
        {
            Containers.Container ctlContainer = null;
            string containerSrc = Null.NullString;
            HttpRequest Request = PaneControl.Page.Request;
            int PreviewModuleId = -1;
            if (Request.QueryString["ModuleId"] != null)
            {
                Int32.TryParse(Request.QueryString["ModuleId"], out PreviewModuleId);
            }
            if ((Request.QueryString["ContainerSrc"] != null) && (objModule.ModuleID == PreviewModuleId || PreviewModuleId == -1))
            {
                containerSrc = SkinController.FormatSkinSrc(Globals.QueryStringDecode(Request.QueryString["ContainerSrc"]) + ".ascx", PortalSettings);
                ctlContainer = LoadContainerByPath(containerSrc);
            }
            if (ctlContainer == null)
            {
                if (Request.Cookies["_ContainerSrc" + PortalSettings.PortalId.ToString()] != null)
                {
                    if (!String.IsNullOrEmpty(Request.Cookies["_ContainerSrc" + PortalSettings.PortalId.ToString()].Value))
                    {
                        containerSrc = SkinController.FormatSkinSrc(Request.Cookies["_ContainerSrc" + PortalSettings.PortalId.ToString()].Value + ".ascx", PortalSettings);
                        ctlContainer = LoadContainerByPath(containerSrc);
                    }
                }
            }
            if (ctlContainer == null)
            {
                if (objModule.DisplayTitle == false)
                {
                    bool blnDisplayTitle = ModulePermissionController.CanEditModuleContent(objModule) || Globals.IsAdminSkin();
                    if (blnDisplayTitle == true)
                    {
                        blnDisplayTitle = (PortalSettings.UserMode != PortalSettings.Mode.View);
                    }
                    if (blnDisplayTitle == false)
                    {
                        containerSrc = SkinController.FormatSkinSrc("[G]" + SkinController.RootContainer + "/_default/No Container.ascx", PortalSettings);
                        ctlContainer = LoadContainerByPath(containerSrc);
                    }
                }
            }
            if (ctlContainer == null)
            {
                if (objModule.ContainerSrc == PortalSettings.ActiveTab.ContainerSrc)
                {
                    if (PaneControl is HtmlControl)
                    {
                        HtmlControl objHtmlControl = (HtmlControl)PaneControl;
                        if ((objHtmlControl.Attributes["ContainerSrc"] != null))
                        {
                            bool validSrc = false;
                            if ((objHtmlControl.Attributes["ContainerType"] != null) && (objHtmlControl.Attributes["ContainerName"] != null))
                            {
                                containerSrc = "[" + objHtmlControl.Attributes["ContainerType"] + "]" + SkinController.RootContainer + "/" + objHtmlControl.Attributes["ContainerName"] + "/" + objHtmlControl.Attributes["ContainerSrc"];
                                validSrc = true;
                            }
                            else
                            {
                                containerSrc = objHtmlControl.Attributes["ContainerSrc"];
                                if (containerSrc.Contains("/"))
                                {
                                    if (!(containerSrc.ToLower().StartsWith("[g]") || containerSrc.ToLower().StartsWith("[l]")))
                                    {
                                        if (SkinController.IsGlobalSkin(PortalSettings.ActiveTab.SkinSrc))
                                        {
                                            containerSrc = string.Format("[G]containers/{0}", containerSrc.TrimStart('/'));
                                        }
                                        else
                                        {
                                            containerSrc = string.Format("[L]containers/{0}", containerSrc.TrimStart('/'));
                                        }
                                        validSrc = true;
                                    }
                                }
                            }
                            if (validSrc)
                            {
                                containerSrc = SkinController.FormatSkinSrc(containerSrc, PortalSettings);
                                ctlContainer = LoadContainerByPath(containerSrc);
                            }
                        }
                    }
                }
            }
            if (ctlContainer == null)
            {
                containerSrc = objModule.ContainerSrc;
                if (!String.IsNullOrEmpty(containerSrc))
                {
                    containerSrc = SkinController.FormatSkinSrc(containerSrc, PortalSettings);
                    ctlContainer = LoadContainerByPath(containerSrc);
                }
            }
            if (ctlContainer == null)
            {
                containerSrc = SkinController.FormatSkinSrc(SkinController.GetDefaultPortalContainer(), PortalSettings);
                ctlContainer = LoadContainerByPath(containerSrc);
            }
            objModule.ContainerPath = SkinController.FormatSkinPath(containerSrc);
            ctlContainer.ID = "ctr";
            if (objModule.ModuleID > -1)
            {
                ctlContainer.ID += objModule.ModuleID.ToString();
            }
            return ctlContainer;
        }
        private void ModuleMoveToPanePostBack(Utilities.ClientAPIPostBackEventArgs args)
        {
            PortalSettings PortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            if (TabPermissionController.CanAdminPage())
            {
                int intModuleID = Convert.ToInt32(args.EventArguments["moduleid"]);
                string strPaneName = Convert.ToString(args.EventArguments["pane"]);
                int intOrder = Convert.ToInt32(args.EventArguments["order"]);
                ModuleController objModules = new ModuleController();
                objModules.UpdateModuleOrder(PortalSettings.ActiveTab.TabID, intModuleID, intOrder, strPaneName);
                objModules.UpdateTabModuleOrder(PortalSettings.ActiveTab.TabID);
                PaneControl.Page.Response.Redirect(PaneControl.Page.Request.RawUrl, true);
            }
        }
        public void InjectModule(ModuleInfo objModule)
        {
            bool bSuccess = true;
            try
            {
                if (!Common.Globals.IsAdminControl())
                {
                    PaneControl.Controls.Add(new LiteralControl("<a name=\"" + objModule.ModuleID.ToString() + "\"></a>"));
                }
                Containers.Container ctlContainer = LoadModuleContainer(objModule);
                Containers.Add(ctlContainer.ID, ctlContainer);
                if (Common.Globals.IsLayoutMode() && Common.Globals.IsAdminControl() == false)
                {
                    Panel ctlDragDropContainer = new Panel();
                    System.Web.UI.Control ctlTitle = ctlContainer.FindControl("dnnTitle");
                    ctlDragDropContainer.ID = ctlContainer.ID + "_DD";
                    PaneControl.Controls.Add(ctlDragDropContainer);
                    ctlDragDropContainer.Controls.Add(ctlContainer);
                    if (ctlTitle != null)
                    {
                        if (ctlTitle.Controls.Count > 0)
                        {
                            ctlTitle = ctlTitle.Controls[0];
                        }
                    }
                    //if (ctlDragDropContainer != null && ctlTitle != null)
                    //{
                    //    ClientAPI.EnableContainerDragAndDrop(ctlTitle, ctlDragDropContainer, objModule.ModuleID);
                    //    ClientAPI.RegisterPostBackEventHandler(PaneControl, "MoveToPane", ModuleMoveToPanePostBack, false);
                    //}
                }
                else
                {
                    PaneControl.Controls.Add(ctlContainer);
                }
                ctlContainer.SetModuleConfiguration(objModule);
                if (PaneControl.Visible == false)
                {
                    PaneControl.Visible = true;
                }
            }
            catch (Exception exc)
            {
                ModuleLoadException lex;
                lex = new ModuleLoadException(string.Format(Skin.MODULEADD_ERROR, PaneControl.ID.ToString()), exc);
                if (TabPermissionController.CanAdminPage())
                {
                    PaneControl.Controls.Add(new ErrorContainer(PortalSettings, Skin.MODULELOAD_ERROR, lex).Container);
                }
                Exceptions.LogException(exc);
                bSuccess = false;
                throw lex;
            }
            if (!bSuccess)
            {
                throw new ModuleLoadException();
            }
        }
        public void ProcessPane()
        {
            if (PaneControl != null)
            {
                PaneControl.Attributes.Remove("ContainerType");
                PaneControl.Attributes.Remove("ContainerName");
                PaneControl.Attributes.Remove("ContainerSrc");
                if (Globals.IsLayoutMode())
                {
                    PaneControl.Visible = true;
                    string cssclass = PaneControl.Attributes["class"];
                    if (string.IsNullOrEmpty(cssclass))
                    {
                        PaneControl.Attributes["class"] = c_PaneOutline;
                    }
                    else
                    {
                        PaneControl.Attributes["class"] = cssclass.Replace(c_PaneOutline, "").Trim().Replace("  ", " ") + " " + c_PaneOutline;
                    }
                    Label ctlLabel = new Label();
                    ctlLabel.Text = "<center>" + Name + "</center><br>";
                    ctlLabel.CssClass = "SubHead";
                    PaneControl.Controls.AddAt(0, ctlLabel);
                }
                else
                {
                    if (TabPermissionController.CanAddContentToPage() && PaneControl.Visible == false)
                    {
                        PaneControl.Visible = true;
                    }
                    bool collapsePanes = true;
                    if (Containers.Count > 0)
                    {
                        collapsePanes = false;
                    }
                    else if (PaneControl.Controls.Count == 0)
                    {
                        collapsePanes = true;
                    }
                    else if (PaneControl.Controls.Count == 1)
                    {
                        collapsePanes = false;
                        LiteralControl literal = PaneControl.Controls[0] as LiteralControl;
                        if (literal != null)
                        {
                            if (String.IsNullOrEmpty(HtmlUtils.StripWhiteSpace(literal.Text, false)))
                            {
                                collapsePanes = true;
                            }
                        }
                    }
                    else
                    {
                        collapsePanes = false;
                    }
                    if (collapsePanes)
                    {
                        if (PaneControl.Attributes["style"] != null)
                        {
                            PaneControl.Attributes.Remove("style");
                        }
                        if (PaneControl.Attributes["class"] != null)
                        {
                            PaneControl.Attributes["class"] = PaneControl.Attributes["class"] + " EmptyPane";
                        }
                        else
                        {
                            PaneControl.Attributes["class"] = "EmptyPane";
                        }
                    }
                }
            }
        }
    }
}
