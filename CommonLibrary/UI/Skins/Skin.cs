using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.UI;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Application;
using System.Web.UI.WebControls;
using CommonLibrary.Common.Utilities;
using CommonLibrary.UI.Skins.Controls;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Services.Exceptions;
using System.Web.UI.HtmlControls;
using CommonLibrary.Services.Localization;
using CommonLibrary.Entities.Modules.Actions;
using CommonLibrary.Framework;
using CommonLibrary.UI.Skins.EventListeners;
using CommonLibrary.Common;
using CommonLibrary.Entities.Modules.Communications;
using CommonLibrary.Services.ControlPanels;
using CommonLibrary.Entities.Host;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.UI.Skins
{
    public class Skin : Framework.UserControlBase
    {
        private ArrayList _actionEventListeners;
        private ModuleCommunicate _Communicator = new ModuleCommunicate();
        private Control _ControlPanel;
        private Dictionary<string, Pane> _panes;
        private string _skinSrc;
        public string PANE_LOAD_ERROR = Localization.GetString("PaneNotFound.Error");
        public string CONTRACTEXPIRED_ERROR = Localization.GetString("ContractExpired.Error");
        public string TABACCESS_ERROR = Localization.GetString("TabAccess.Error");
        public string MODULEACCESS_ERROR = Localization.GetString("ModuleAccess.Error");
        public string CRITICAL_ERROR = Localization.GetString("CriticalError.Error");
        public string MODULELOAD_WARNING = Localization.GetString("ModuleLoadWarning.Error");
        public string MODULELOAD_WARNINGTEXT = Localization.GetString("ModuleLoadWarning.Text");
        public static string MODULELOAD_ERROR = Localization.GetString("ModuleLoad.Error");
        public static string CONTAINERLOAD_ERROR = Localization.GetString("ContainerLoad.Error");
        public static string MODULEADD_ERROR = Localization.GetString("ModuleAdd.Error");

        private SqlConnection con = new SqlConnection(Settings.ConnectionString);
        private DataTable dt = new DataTable();

        internal Control ControlPanel
        {
            get
            {
                if (_ControlPanel == null)
                {
                    _ControlPanel = FindControl("ControlPanel");
                }
                return _ControlPanel;
            }
        }
        internal ModuleCommunicate Communicator
        {
            get { return _Communicator; }
        }
        internal Dictionary<string, Pane> Panes
        {
            get
            {
                if (_panes == null)
                {
                    _panes = new Dictionary<string, Pane>();
                }
                return _panes;
            }
        }
        public ArrayList ActionEventListeners
        {
            get
            {
                if (_actionEventListeners == null)
                {
                    _actionEventListeners = new ArrayList();
                }
                return _actionEventListeners;
            }
            set { _actionEventListeners = value; }
        }
        public string SkinPath
        {
            get { return this.TemplateSourceDirectory + "/"; }
        }
        public string SkinSrc
        {
            get { return _skinSrc; }
            set { _skinSrc = value; }
        }
        private bool CheckExpired()
        {
            bool blnExpired = false;
            if (PortalSettings.ExpiryDate != Null.NullDate)
            {
                if (Convert.ToDateTime(PortalSettings.ExpiryDate) < DateTime.Now && PortalSettings.ActiveTab.ParentId != PortalSettings.SuperTabId)
                {
                    blnExpired = true;
                }
            }
            return blnExpired;
        }
        private bool InjectControlPanel()
        {
            if (Request.QueryString["dnnprintmode"] != "true")
            {
                ControlPanelBase objControlPanel = ControlUtilities.LoadControl<ControlPanelBase>(this, Host.ControlPanel);
                if (ControlPanel == null)
                {
                    HtmlForm objForm = (HtmlForm)Parent.FindControl("Form");
                    if (objForm != null)
                    {
                        objForm.Controls.AddAt(0, objControlPanel);
                    }
                    else
                    {
                        Page.Controls.AddAt(0, objControlPanel);
                    }
                }
                else
                {
                    ControlPanel.Controls.Add(objControlPanel);
                }
            }
            return true;
        }
        private void LoadPanes()
        {
            HtmlContainerControl objPaneControl;
            foreach (Control ctlControl in Controls)
            {
                if (ctlControl is HtmlContainerControl)
                {
                    objPaneControl = ctlControl as HtmlContainerControl;
                    if (objPaneControl != null && !string.IsNullOrEmpty(objPaneControl.ID))
                    {
                        switch (objPaneControl.TagName.ToLowerInvariant())
                        {
                            case "td":
                            case "div":
                            case "span":
                            case "p":
                                if (objPaneControl.ID.ToLower() != "controlpanel")
                                {
                                    PortalSettings.ActiveTab.Panes.Add(objPaneControl.ID);
                                    Panes.Add(objPaneControl.ID.ToLowerInvariant(), new Pane(objPaneControl));
                                }
                                else
                                {
                                    _ControlPanel = objPaneControl;
                                }
                                break;
                        }
                    }
                }
            }
        }
        private bool ProcessMasterModules()
        {
            bool bSuccess = true;
            if (TabPermissionController.CanViewPage())
            {
                if (!CheckExpired())
                {
                    if ((PortalSettings.ActiveTab.StartDate < DateTime.Now && PortalSettings.ActiveTab.EndDate > DateTime.Now) || Globals.IsLayoutMode())
                    {
                        if (PortalSettings.ActiveTab.Modules.Count > 0)
                        {
                            foreach (ModuleInfo objModule in PortalSettings.ActiveTab.Modules)
                            {
                                if (ModulePermissionController.CanViewModule(objModule) && objModule.IsDeleted == false)
                                {
                                    if ((objModule.StartDate < DateTime.Now && objModule.EndDate > DateTime.Now) || Common.Globals.IsLayoutMode() || Common.Globals.IsEditMode())
                                    {
                                        Pane pane = null;
                                        bool bFound = Panes.TryGetValue(objModule.PaneName.ToLowerInvariant(), out pane);
                                        if (!bFound)
                                        {
                                            bFound = Panes.TryGetValue(Common.Globals.glbDefaultPane.ToLowerInvariant(), out pane);
                                        }
                                        if (bFound)
                                        {
                                            bSuccess = InjectModule(pane, objModule);
                                        }
                                        else
                                        {
                                            ModuleLoadException lex;
                                            lex = new ModuleLoadException(PANE_LOAD_ERROR);
                                            Controls.Add(new ErrorContainer(PortalSettings, MODULELOAD_ERROR, lex).Container);
                                            Exceptions.LogException(lex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        AddPageMessage(this, "", TABACCESS_ERROR, UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                    }
                }
                else
                {
                    AddPageMessage(this, "", string.Format(CONTRACTEXPIRED_ERROR, PortalSettings.PortalName, Common.Globals.GetMediumDate(PortalSettings.ExpiryDate.ToString()), PortalSettings.Email), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
            }
            else
            {
                Response.Redirect(Common.Globals.AccessDeniedURL(TABACCESS_ERROR), true);
            }
            return bSuccess;
        }
        private void ProcessPanes()
        {
            foreach (KeyValuePair<string, Pane> kvp in Panes)
            {
                kvp.Value.ProcessPane();
            }
        }
        private bool ProcessSlaveModule()
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule = null;
            ModuleInfo slaveModule = null;
            int moduleId = -1;
            string key = "";
            bool bSuccess = true;
            if (Request.QueryString["mid"] != null)
            {
                Int32.TryParse(Request.QueryString["mid"], out moduleId);
            }
            if (Request.QueryString["ctl"] != null)
            {
                key = Request.QueryString["ctl"];
            }
            if (Request.QueryString["moduleid"] != null && (key.ToLower() == "module" || key.ToLower() == "help"))
            {
                Int32.TryParse(Request.QueryString["moduleid"], out moduleId);
            }
            if (moduleId != -1)
            {
                objModule = objModules.GetModule(moduleId, PortalSettings.ActiveTab.TabID, false);
                if (objModule != null)
                {
                    slaveModule = objModule.Clone();
                }
            }
            if (slaveModule == null)
            {
                slaveModule = new ModuleInfo();
                slaveModule.ModuleID = moduleId;
                slaveModule.ModuleDefID = -1;
                slaveModule.TabID = PortalSettings.ActiveTab.TabID;
            }
            if (Request.QueryString["moduleid"] != null && (key.ToLower() == "module" || key.ToLower() == "help"))
            {
                slaveModule.ModuleDefID = -1;
            }
            if (Request.QueryString["dnnprintmode"] != "true")
            {
                slaveModule.ModuleTitle = "";
            }
            slaveModule.Header = "";
            slaveModule.Footer = "";
            slaveModule.StartDate = DateTime.MinValue;
            slaveModule.EndDate = DateTime.MaxValue;
            slaveModule.PaneName = Common.Globals.glbDefaultPane;
            slaveModule.Visibility = VisibilityState.None;
            slaveModule.Color = "";
            slaveModule.Border = "";
            slaveModule.DisplayTitle = true;
            slaveModule.DisplayPrint = false;
            slaveModule.DisplaySyndicate = false;
            slaveModule.ContainerSrc = PortalSettings.ActiveTab.ContainerSrc;
            if (string.IsNullOrEmpty(slaveModule.ContainerSrc))
            {
                slaveModule.ContainerSrc = PortalSettings.DefaultPortalContainer;
            }
            slaveModule.ContainerSrc = SkinController.FormatSkinSrc(slaveModule.ContainerSrc, PortalSettings);
            slaveModule.ContainerPath = SkinController.FormatSkinPath(slaveModule.ContainerSrc);
            Pane pane = null;
            bool bFound = Panes.TryGetValue(Common.Globals.glbDefaultPane.ToLowerInvariant(), out pane);
            ModuleControlInfo objModuleControl = ModuleControlController.GetModuleControlByControlKey(key, slaveModule.ModuleDefID);
            if (objModuleControl != null)
            {
                slaveModule.ModuleControlId = objModuleControl.ModuleControlID;
                slaveModule.IconFile = objModuleControl.IconFile;
                if (ModulePermissionController.HasModuleAccess(slaveModule.ModuleControl.ControlType, Null.NullString, slaveModule))
                {
                    bSuccess = InjectModule(pane, slaveModule);
                }
                else
                {
                    Response.Redirect(Common.Globals.AccessDeniedURL(MODULEACCESS_ERROR), true);
                }
            }
            return bSuccess;
        }
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            bool bSuccess = true;
            LoadPanes();
            if (!Common.Globals.IsAdminControl())
            {
                bSuccess = ProcessMasterModules();
            }
            else
            {
                bSuccess = ProcessSlaveModule();
            }
            InjectControlPanel();
            ProcessPanes();
            if (Request.QueryString["error"] != null)
            {
                AddPageMessage(this, CRITICAL_ERROR, Server.HtmlEncode(Request.QueryString["error"]), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
            if (!TabPermissionController.CanAdminPage())
            {
                if (!bSuccess)
                {
                    AddPageMessage(this, MODULELOAD_WARNING, string.Format(MODULELOAD_WARNINGTEXT, PortalSettings.Email), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                }
            }
            foreach (SkinEventListener listener in AppContext.Current.SkinEventListeners)
            {
                if (listener.EventType == SkinEventType.OnSkinInit)
                {
                    listener.SkinEvent.Invoke(this, new SkinEventArgs(this));
                }
            }
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            foreach (SkinEventListener listener in AppContext.Current.SkinEventListeners)
            {
                if (listener.EventType == SkinEventType.OnSkinLoad)
                {
                    listener.SkinEvent.Invoke(this, new SkinEventArgs(this));
                }
            }
        }
        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);
            foreach (SkinEventListener listener in AppContext.Current.SkinEventListeners)
            {
                if (listener.EventType == SkinEventType.OnSkinPreRender)
                {
                    listener.SkinEvent.Invoke(this, new SkinEventArgs(this));
                }
            }
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            foreach (SkinEventListener listener in AppContext.Current.SkinEventListeners)
            {
                if (listener.EventType == SkinEventType.OnSkinUnLoad)
                {
                    listener.SkinEvent.Invoke(this, new SkinEventArgs(this));
                }
            }
        }
        public bool InjectModule(Pane objPane, ModuleInfo objModule)
        {
            bool bSuccess = true;
            try
            {
                objPane.InjectModule(objModule);
            }
            catch (Exception ex)
            {
                ex.ToString();
                bSuccess = false;
            }
            return bSuccess;
        }
        public void RegisterModuleActionEvent(int ModuleID, ActionEventHandler e)
        {
            ActionEventListeners.Add(new ModuleActionEventListener(ModuleID, e));
        }
        private static Skin LoadSkin(PageBase Page, string SkinPath)
        {
            Skin ctlSkin = null;
            try
            {
                string SkinSrc = SkinPath;
                if (SkinPath.ToLower().IndexOf(Common.Globals.ApplicationPath) != -1)
                {
                    SkinPath = SkinPath.Remove(0, Common.Globals.ApplicationPath.Length);
                }
                ctlSkin = ControlUtilities.LoadControl<Skin>(Page, SkinPath);
                ctlSkin.SkinSrc = SkinSrc;
                ctlSkin.DataBind();
            }
            catch (Exception exc)
            {
                PageLoadException lex = new PageLoadException("Unhandled error loading page.", exc);
                if (TabPermissionController.CanAdminPage())
                {
                    Label SkinError = (Label)Page.FindControl("SkinError");
                    SkinError.Text = string.Format(Localization.GetString("SkinLoadError", Localization.GlobalResourceFile), SkinPath, Page.Server.HtmlEncode(exc.Message));
                    SkinError.Visible = true;
                }
                Exceptions.LogException(lex);
            }
            return ctlSkin;
        }
        private static void AddModuleMessage(Control objControl, string Heading, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType, string IconSrc)
        {
            if (objControl != null)
            {
                if (!String.IsNullOrEmpty(Message))
                {
                    PlaceHolder MessagePlaceHolder = (PlaceHolder)objControl.Parent.FindControl("MessagePlaceHolder");
                    if (MessagePlaceHolder != null)
                    {
                        MessagePlaceHolder.Visible = true;
                        UI.Skins.Controls.ModuleMessage objModuleMessage;
                        objModuleMessage = GetModuleMessageControl(Heading, Message, objModuleMessageType, IconSrc);
                        MessagePlaceHolder.Controls.Add(objModuleMessage);
                    }
                }
            }
        }
        private static void AddPageMessage(Control objControl, string Heading, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType, string IconSrc)
        {
            if (!String.IsNullOrEmpty(Message))
            {
                Control ContentPane = (Control)objControl.FindControl(Common.Globals.glbDefaultPane);
                if (ContentPane != null)
                {
                    UI.Skins.Controls.ModuleMessage objModuleMessage;
                    objModuleMessage = GetModuleMessageControl(Heading, Message, objModuleMessageType, IconSrc);
                    ContentPane.Controls.AddAt(0, objModuleMessage);
                }
            }
        }
        public static void AddModuleMessage(PortalModuleBase objControl, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType)
        {
            AddModuleMessage(objControl, "", Message, objModuleMessageType, Null.NullString);
        }
        public static void AddModuleMessage(PortalModuleBase objControl, string Heading, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType)
        {
            AddModuleMessage(objControl, Heading, Message, objModuleMessageType, Null.NullString);
        }
        public static void AddModuleMessage(Control objControl, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType)
        {
            AddModuleMessage(objControl, "", Message, objModuleMessageType, Null.NullString);
        }
        public static void AddModuleMessage(Control objControl, string Heading, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType)
        {
            AddModuleMessage(objControl, Heading, Message, objModuleMessageType, Null.NullString);
        }
        public static void AddPageMessage(Page objPage, string Heading, string Message, string IconSrc)
        {
            AddPageMessage(objPage, Heading, Message, ModuleMessage.ModuleMessageType.GreenSuccess, IconSrc);
        }
        public static void AddPageMessage(UI.Skins.Skin objSkin, string Heading, string Message, string IconSrc)
        {
            AddPageMessage(objSkin, Heading, Message, ModuleMessage.ModuleMessageType.GreenSuccess, IconSrc);
        }
        public static void AddPageMessage(UI.Skins.Skin objSkin, string Heading, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType)
        {
            AddPageMessage(objSkin, Heading, Message, objModuleMessageType, Null.NullString);
        }
        public static void AddPageMessage(Page objPage, string Heading, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType)
        {
            AddPageMessage(objPage, Heading, Message, objModuleMessageType, Null.NullString);
        }
        public static UI.Skins.Controls.ModuleMessage GetModuleMessageControl(string Heading, string Message, string IconImage)
        {
            return GetModuleMessageControl(Heading, Message, ModuleMessage.ModuleMessageType.GreenSuccess, IconImage);
        }
        public static UI.Skins.Controls.ModuleMessage GetModuleMessageControl(string Heading, string Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType objModuleMessageType)
        {
            return GetModuleMessageControl(Heading, Message, objModuleMessageType, Null.NullString);
        }
        public static UI.Skins.Controls.ModuleMessage GetModuleMessageControl(string Heading, string Message, Controls.ModuleMessage.ModuleMessageType objModuleMessageType, string IconImage)
        {
            UI.Skins.Controls.ModuleMessage objModuleMessage;
            UI.Skins.Skin s = new UI.Skins.Skin();
            objModuleMessage = (UI.Skins.Controls.ModuleMessage)s.LoadControl("~/admin/skins/ModuleMessage.ascx");
            objModuleMessage.Heading = Heading;
            objModuleMessage.Text = Message;
            objModuleMessage.IconImage = IconImage;
            objModuleMessage.IconType = objModuleMessageType;
            return objModuleMessage;
        }
        public static UI.Skins.Skin GetParentSkin(PortalModuleBase objModule)
        {
            return GetParentSkin(objModule as System.Web.UI.Control);
        }
        public static UI.Skins.Skin GetParentSkin(System.Web.UI.Control objControl)
        {
            if (objControl != null)
            {
                System.Web.UI.Control MyParent = objControl.Parent;
                bool FoundSkin = false;
                while (MyParent != null)
                {
                    if (MyParent is Skin)
                    {
                        FoundSkin = true;
                        break;
                    }
                    MyParent = MyParent.Parent;
                }
                if (FoundSkin)
                {
                    return (Skin)MyParent;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public static Skin GetSkin(PageBase Page)
        {
            Skin ctlSkin = null;
            string skinSource = Null.NullString;
            if ((Page.Request.QueryString["SkinSrc"] != null))
            {
                skinSource = SkinController.FormatSkinSrc(Common.Globals.QueryStringDecode(Page.Request.QueryString["SkinSrc"]) + ".ascx", Page.PortalSettings);
                ctlSkin = LoadSkin(Page, skinSource);
            }
            if (ctlSkin == null)
            {
                if (Page.Request.Cookies["_SkinSrc" + Page.PortalSettings.PortalId.ToString()] != null)
                {
                    if (!String.IsNullOrEmpty(Page.Request.Cookies["_SkinSrc" + Page.PortalSettings.PortalId.ToString()].Value))
                    {
                        skinSource = SkinController.FormatSkinSrc(Page.Request.Cookies["_SkinSrc" + Page.PortalSettings.PortalId.ToString()].Value + ".ascx", Page.PortalSettings);
                        ctlSkin = LoadSkin(Page, skinSource);
                    }
                }
            }
            if (ctlSkin == null)
            {
                if (Common.Globals.IsAdminSkin())
                {
                    skinSource = SkinController.FormatSkinSrc(Page.PortalSettings.DefaultAdminSkin, Page.PortalSettings);
                }
                else
                {
                    skinSource = Page.PortalSettings.ActiveTab.SkinSrc;
                }
                if (!String.IsNullOrEmpty(skinSource))
                {
                    skinSource = SkinController.FormatSkinSrc(skinSource, Page.PortalSettings);
                    ctlSkin = LoadSkin(Page, skinSource);
                }
            }
            if (ctlSkin == null)
            {
                skinSource = SkinController.FormatSkinSrc(SkinController.GetDefaultPortalSkin(), Page.PortalSettings);
                ctlSkin = LoadSkin(Page, skinSource);
            }
            Page.PortalSettings.ActiveTab.SkinPath = SkinController.FormatSkinPath(skinSource);
            ctlSkin.ID = "dnn";
            return ctlSkin;
        }


        #region Skin - Template-Related Method ============================================================================================
        public DataTable GetListByPortalIdSkinPackageId(int PortalId, int SkinPackageId)
        {
            SqlCommand cmd = new SqlCommand("Skins_GetListByPortalIdSkinPackageId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int SelectSkin(int SkinId)
        {
            SqlCommand cmd = new SqlCommand("Skins_Select", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinId", SkinId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable GetDetails(int SkinId)
        {
            SqlCommand cmd = new SqlCommand("Skins_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinId", SkinId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(int SkinPackageId, string SkinSrc)
        {
            SqlCommand cmd = new SqlCommand("Skins_Insert", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);
            cmd.Parameters.AddWithValue("@SkinSrc", SkinSrc);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int SkinId, int SkinPackageId, string SkinSrc)
        {
            SqlCommand cmd = new SqlCommand("Skins_Update", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@SkinId", SkinId);
            cmd.Parameters.AddWithValue("@SkinPackageId", SkinPackageId);
            cmd.Parameters.AddWithValue("@SkinSrc", SkinSrc);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }   
        //public static List<Template> GetTemplates()
        //{
        //    DirectoryInfo dInfo = new DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath("../../Templates/Front_Templates"));
        //    DirectoryInfo[] dArrInfo = dInfo.GetDirectories();
        //    List<Template> list = new List<Template>();
        //    foreach (DirectoryInfo sDirectory in dArrInfo)
        //    {
        //        Template temp = new Template(sDirectory.Name);

        //        list.Add(temp);
        //    }
        //    return list;
        //}

        //public static List<Template> GetTemplates_Admin()
        //{
        //    DirectoryInfo dInfo = new DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath("../../Templates/Admin_Templates"));
        //    DirectoryInfo[] dArrInfo = dInfo.GetDirectories();
        //    List<Template> list = new List<Template>();
        //    foreach (DirectoryInfo sDirectory in dArrInfo)
        //    {
        //        Template temp = new Template(sDirectory.Name);

        //        list.Add(temp);
        //    }
        //    return list;
        //}
        #endregion =====================================================================================================================
        
    }
}
