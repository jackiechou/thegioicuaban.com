using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Common.Utilities;
using CommonLibrary.UI.Skins;
using CommonLibrary.Services.Tokens.PropertyAccess;
using CommonLibrary.Services.Tokens;
using System.Web;
using CommonLibrary.Entities.Users;
using CommonLibrary.Services.Localization;
using CommonLibrary.Common;
using System.Collections;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Services.Personalization;

namespace CommonLibrary.Entities.Portal
{
    public class PortalSettings
    {
        public enum Mode
        {
            View,
            Edit,
            Layout
        }
        public enum ControlPanelPermission
        {
            TabEditor,
            ModuleEditor
        }
        private int _PortalId;
        private string _PortalName;
        private string _HomeDirectory;
        private string _LogoFile;
        private string _FooterText;
        private System.DateTime _ExpiryDate;
        private int _UserRegistration;
        private int _BannerAdvertising;
        private string _Currency;
        private int _AdministratorId;
        private string _Email;
        private float _HostFee;
        private int _HostSpace;
        private int _PageQuota;
        private int _UserQuota;
        private int _AdministratorRoleId;
        private string _AdministratorRoleName;
        private int _RegisteredRoleId;
        private string _RegisteredRoleName;
        private string _Description;
        private string _KeyWords;
        private string _BackgroundFile;
        private Guid _GUID;
        private int _SiteLogHistory;
        private int _AdminTabId;
        private int _SuperTabId;
        private int _SplashTabId;
        private int _HomeTabId;
        private int _LoginTabId;
        private int _RegisterTabId;
        private int _UserTabId;
        private string _DefaultLanguage;
        private int _TimeZoneOffset;
        private string _Version;
        private TabInfo _ActiveTab;
        private PortalAliasInfo _PortalAlias;
        private SkinInfo _AdminContainer;
        private SkinInfo _AdminSkin;
        private SkinInfo _PortalContainer;
        private SkinInfo _PortalSkin;
        private int _Users;
        private int _Pages;
        public PortalSettings()
        {
        }
        public PortalSettings(int portalID): this(Null.NullInteger, portalID)
        {
        }
        public PortalSettings(int tabID, int portalID)
        {
            PortalController controller = new PortalController();
            PortalInfo portal = controller.GetPortal(portalID);
            GetPortalSettings(tabID, portal);
        }
        public PortalSettings(int tabID, PortalAliasInfo objPortalAliasInfo)
        {
            _ActiveTab = new TabInfo();
            PortalId = objPortalAliasInfo.PortalID;
            PortalAlias = objPortalAliasInfo;
            PortalController controller = new PortalController();
            PortalInfo portal = controller.GetPortal(PortalId);
            if (portal != null)
            {
                GetPortalSettings(tabID, portal);
            }
        }
        public PortalSettings(PortalInfo portal)
        {
            _ActiveTab = new TabInfo();
            GetPortalSettings(Null.NullInteger, portal);
        }
        public PortalSettings(int tabID, PortalInfo portal)
        {
            _ActiveTab = new TabInfo();
            GetPortalSettings(tabID, portal);
        }
        public string FooterText
        {
            get { return _FooterText; }
            set { _FooterText = value; }
        }
        public string HomeDirectory
        {
            get { return _HomeDirectory; }
            set { _HomeDirectory = value; }
        }
        public string HomeDirectoryMapPath
        {
            get
            {
                Services.FileSystem.FolderController objFolderController = new Services.FileSystem.FolderController();
                return objFolderController.GetMappedDirectory(HomeDirectory);
            }
        }
        public string LogoFile
        {
            get { return _LogoFile; }
            set { _LogoFile = value; }
        }
        public int PortalId
        {
            get { return _PortalId; }
            set { _PortalId = value; }
        }
        public string PortalName
        {
            get { return _PortalName; }
            set { _PortalName = value; }
        }
        public int UserId
        {
            get
            {
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    return UserInfo.UserID;
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }
        public UserInfo UserInfo
        {
            get { return UserController.GetCurrentUserInfo(); }
        }
        public System.DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set { _ExpiryDate = value; }
        }
        public int UserRegistration
        {
            get { return _UserRegistration; }
            set { _UserRegistration = value; }
        }
        public int BannerAdvertising
        {
            get { return _BannerAdvertising; }
            set { _BannerAdvertising = value; }
        }
        public string Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }
        public int AdministratorId
        {
            get { return _AdministratorId; }
            set { _AdministratorId = value; }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        public float HostFee
        {
            get { return _HostFee; }
            set { _HostFee = value; }
        }
        public int HostSpace
        {
            get { return _HostSpace; }
            set { _HostSpace = value; }
        }
        public int PageQuota
        {
            get { return _PageQuota; }
            set { _PageQuota = value; }
        }
        public int UserQuota
        {
            get { return _UserQuota; }
            set { _UserQuota = value; }
        }
        public int AdministratorRoleId
        {
            get { return _AdministratorRoleId; }
            set { _AdministratorRoleId = value; }
        }
        public string AdministratorRoleName
        {
            get { return _AdministratorRoleName; }
            set { _AdministratorRoleName = value; }
        }
        public int RegisteredRoleId
        {
            get { return _RegisteredRoleId; }
            set { _RegisteredRoleId = value; }
        }
        public string RegisteredRoleName
        {
            get { return _RegisteredRoleName; }
            set { _RegisteredRoleName = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public string KeyWords
        {
            get { return _KeyWords; }
            set { _KeyWords = value; }
        }
        public string BackgroundFile
        {
            get { return _BackgroundFile; }
            set { _BackgroundFile = value; }
        }
        public Guid GUID
        {
            get { return _GUID; }
            set { _GUID = value; }
        }
        public int SiteLogHistory
        {
            get { return _SiteLogHistory; }
            set { _SiteLogHistory = value; }
        }
        public int AdminTabId
        {
            get { return _AdminTabId; }
            set { _AdminTabId = value; }
        }
        public int SuperTabId
        {
            get { return _SuperTabId; }
            set { _SuperTabId = value; }
        }
        public int SplashTabId
        {
            get { return _SplashTabId; }
            set { _SplashTabId = value; }
        }
        public int HomeTabId
        {
            get { return _HomeTabId; }
            set { _HomeTabId = value; }
        }
        public int LoginTabId
        {
            get { return _LoginTabId; }
            set { _LoginTabId = value; }
        }
        public int RegisterTabId
        {
            get { return _RegisterTabId; }
            set { _RegisterTabId = value; }
        }
        public int UserTabId
        {
            get { return _UserTabId; }
            set { _UserTabId = value; }
        }
        public string DefaultLanguage
        {
            get { return _DefaultLanguage; }
            set { _DefaultLanguage = value; }
        }
        public int TimeZoneOffset
        {
            get { return _TimeZoneOffset; }
            set { _TimeZoneOffset = value; }
        }
        public int Users
        {
            get { return _Users; }
            set { _Users = value; }
        }
        public int Pages
        {
            get { return _Pages; }
            set { _Pages = value; }
        }
        public TabInfo ActiveTab
        {
            get { return _ActiveTab; }
            set { _ActiveTab = value; }
        }
        public Mode DefaultControlPanelMode
        {
            get
            {
                Mode mode = Mode.Edit;
                string setting = Null.NullString;
                if (PortalController.GetPortalSettingsDictionary(PortalId).TryGetValue("ControlPanelMode", out setting))
                {
                    if (setting.ToUpperInvariant() == "VIEW")
                    {
                        mode = PortalSettings.Mode.View;
                    }
                }
                return mode;
            }
        }
        public ControlPanelPermission ControlPanelSecurity
        {
            get
            {
                ControlPanelPermission security = ControlPanelPermission.ModuleEditor;
                string setting = Null.NullString;
                if (PortalController.GetPortalSettingsDictionary(PortalId).TryGetValue("ControlPanelSecurity", out setting))
                {
                    if (setting.ToUpperInvariant() == "TAB")
                    {
                        security = ControlPanelPermission.TabEditor;
                    }
                    else
                    {
                        security = ControlPanelPermission.ModuleEditor;
                    }
                }
                return security;
            }
        }
        public bool DefaultControlPanelVisibility
        {
            get
            {
                bool isVisible = true;
                string setting = "";
                if (PortalController.GetPortalSettingsDictionary(PortalId).TryGetValue("ControlPanelVisibility", out setting))
                {
                    isVisible = setting.ToUpperInvariant() != "MIN";
                }
                return isVisible;
            }
        }
        public bool ControlPanelVisible
        {
            get
            {
                bool isVisible = true;
                string setting = Convert.ToString(Personalization.GetProfile("Usability", "ControlPanelVisible" + PortalId.ToString()));
                if (String.IsNullOrEmpty(setting))
                {
                    isVisible = DefaultControlPanelVisibility;
                }
                else
                {
                    isVisible = Convert.ToBoolean(setting);
                }
                return isVisible;
            }
        }
        public string DefaultAdminContainer
        {
            get { return PortalController.GetPortalSetting("DefaultAdminContainer", PortalId, Host.Host.DefaultAdminContainer); }
        }
        public string DefaultAdminSkin
        {
            get { return PortalController.GetPortalSetting("DefaultAdminSkin", PortalId, Host.Host.DefaultAdminSkin); }
        }
        public int DefaultModuleId
        {
            get { return PortalController.GetPortalSettingAsInteger("defaultmoduleid", PortalId, Null.NullInteger); }
        }
        public int DefaultTabId
        {
            get { return PortalController.GetPortalSettingAsInteger("defaulttabid", PortalId, Null.NullInteger); }
        }
        public string DefaultPortalContainer
        {
            get { return PortalController.GetPortalSetting("DefaultPortalContainer", PortalId, Host.Host.DefaultPortalContainer); }
        }
        public string DefaultPortalSkin
        {
            get { return PortalController.GetPortalSetting("DefaultPortalSkin", PortalId, Host.Host.DefaultPortalSkin); }
        }
        public bool EnableBrowserLanguage
        {
            get { return PortalController.GetPortalSettingAsBoolean("EnableBrowserLanguage", PortalId, Host.Host.EnableBrowserLanguage); }
        }
        public bool EnableUrlLanguage
        {
            get { return PortalController.GetPortalSettingAsBoolean("EnableUrlLanguage", PortalId, Host.Host.EnableUrlLanguage); }
        }
        public bool EnableSkinWidgets
        {
            get { return PortalController.GetPortalSettingAsBoolean("EnableSkinWidgets", PortalId, true); }
        }
        public bool InlineEditorEnabled
        {
            get { return PortalController.GetPortalSettingAsBoolean("InlineEditorEnabled", PortalId, true); }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether folders which are hidden or whose name begins with underscore
        /// are included in folder synchronization.
        /// </summary>
        /// <remarks>Defaults to True</remarks>
        /// <history>
        /// [cnurse]	08/28/2008 Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool HideFoldersEnabled
        {
            get { return PortalController.GetPortalSettingAsBoolean("HideFoldersEnabled", PortalId, true); }
        }
        public PortalAliasInfo PortalAlias
        {
            get { return _PortalAlias; }
            set { _PortalAlias = value; }
        }
        public bool SearchIncludeCommon
        {
            get { return PortalController.GetPortalSettingAsBoolean("SearchIncludeCommon", PortalId, Host.Host.SearchIncludeNumeric); }
        }
        public bool SearchIncludeNumeric
        {
            get { return PortalController.GetPortalSettingAsBoolean("SearchIncludeNumeric", PortalId, Host.Host.SearchIncludeNumeric); }
        }
        public int SearchMaxWordlLength
        {
            get { return PortalController.GetPortalSettingAsInteger("MaxSearchWordLength", PortalId, Host.Host.SearchMaxWordlLength); }
        }
        public int SearchMinWordlLength
        {
            get { return PortalController.GetPortalSettingAsInteger("MinSearchWordLength", PortalId, Host.Host.SearchMinWordlLength); }
        }
        public bool SSLEnabled
        {
            get { return PortalController.GetPortalSettingAsBoolean("SSLEnabled", PortalId, false); }
        }
        public bool SSLEnforced
        {
            get { return PortalController.GetPortalSettingAsBoolean("SSLEnforced", PortalId, false); }
        }
        public string SSLURL
        {
            get { return PortalController.GetPortalSetting("SSLURL", PortalId, Null.NullString); }
        }
        public string STDURL
        {
            get { return PortalController.GetPortalSetting("STDURL", PortalId, Null.NullString); }
        }
        public Mode UserMode
        {
            get
            {
                Mode mode;
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    mode = DefaultControlPanelMode;
                    string setting = Convert.ToString(Personalization.GetProfile("Usability", "UserMode" + PortalId.ToString()));
                    switch (setting.ToUpper())
                    {
                        case "VIEW":
                            mode = PortalSettings.Mode.View;
                            break;
                        case "EDIT":
                            mode = PortalSettings.Mode.Edit;
                            break;
                        case "LAYOUT":
                            mode = PortalSettings.Mode.Layout;
                            break;
                    }
                }
                else
                {
                    mode = PortalSettings.Mode.View;
                }
                return mode;
            }
        }
        public static PortalSettings Current
        {
            get { return PortalController.GetCurrentPortalSettings(); }
        }
        private void GetBreadCrumbsRecursively(ref ArrayList objBreadCrumbs, int intTabId)
        {
            TabInfo objTab = null;
            TabController objTabController = new TabController();
            TabCollection portalTabs = objTabController.GetTabsByPortal(PortalId);
            TabCollection hostTabs = objTabController.GetTabsByPortal(Null.NullInteger);
            bool blnFound = portalTabs.TryGetValue(intTabId, out objTab);
            if (!blnFound)
            {
                blnFound = hostTabs.TryGetValue(intTabId, out objTab);
            }
            if (blnFound)
            {
                objBreadCrumbs.Insert(0, objTab.Clone());
                if (!Null.IsNull(objTab.ParentId))
                {
                    GetBreadCrumbsRecursively(ref objBreadCrumbs, objTab.ParentId);
                }
            }
        }
        private void GetPortalSettings(int tabID, PortalInfo portal)
        {
            ModuleController objModules = new ModuleController();
            ModuleInfo objModule;
            this.PortalId = portal.PortalID;
            this.PortalName = portal.PortalName;
            this.LogoFile = portal.LogoFile;
            this.FooterText = portal.FooterText;
            this.ExpiryDate = portal.ExpiryDate;
            this.UserRegistration = portal.UserRegistration;
            this.BannerAdvertising = portal.BannerAdvertising;
            this.Currency = portal.Currency;
            this.AdministratorId = portal.AdministratorId;
            this.Email = portal.Email;
            this.HostFee = portal.HostFee;
            this.HostSpace = portal.HostSpace;
            this.PageQuota = portal.PageQuota;
            this.UserQuota = portal.UserQuota;
            this.AdministratorRoleId = portal.AdministratorRoleId;
            this.AdministratorRoleName = portal.AdministratorRoleName;
            this.RegisteredRoleId = portal.RegisteredRoleId;
            this.RegisteredRoleName = portal.RegisteredRoleName;
            this.Description = portal.Description;
            this.KeyWords = portal.KeyWords;
            this.BackgroundFile = portal.BackgroundFile;
            this.GUID = portal.GUID;
            this.SiteLogHistory = portal.SiteLogHistory;
            this.AdminTabId = portal.AdminTabId;
            this.SuperTabId = portal.SuperTabId;
            this.SplashTabId = portal.SplashTabId;
            this.HomeTabId = portal.HomeTabId;
            this.LoginTabId = portal.LoginTabId;
            this.RegisterTabId = portal.RegisterTabId;
            this.UserTabId = portal.UserTabId;
            this.DefaultLanguage = portal.DefaultLanguage;
            this.TimeZoneOffset = portal.TimeZoneOffset;
            this.HomeDirectory = portal.HomeDirectory;
            this.Pages = portal.Pages;
            this.Users = portal.Users;
            if (Null.IsNull(this.HostSpace))
            {
                this.HostSpace = 0;
            }
            if (Null.IsNull(this.DefaultLanguage))
            {
                this.DefaultLanguage = Localization.SystemLocale;
            }
            if (Null.IsNull(this.TimeZoneOffset))
            {
                this.TimeZoneOffset = Localization.SystemTimeZoneOffset;
            }
            this.HomeDirectory = Common.Globals.ApplicationPath + "/" + portal.HomeDirectory + "/";
            if (VerifyPortalTab(PortalId, tabID))
            {
                if (this.ActiveTab != null)
                {
                    if (Globals.IsAdminSkin())
                    {
                        this.ActiveTab.SkinSrc = this.DefaultAdminSkin;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(this.ActiveTab.SkinSrc))
                        {
                            this.ActiveTab.SkinSrc = this.DefaultPortalSkin;
                        }
                    }
                    this.ActiveTab.SkinSrc = SkinController.FormatSkinSrc(this.ActiveTab.SkinSrc, this);
                    this.ActiveTab.SkinPath = SkinController.FormatSkinPath(this.ActiveTab.SkinSrc);
                    if (Globals.IsAdminSkin())
                    {
                        this.ActiveTab.ContainerSrc = this.DefaultAdminContainer;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(this.ActiveTab.ContainerSrc))
                        {
                            this.ActiveTab.ContainerSrc = this.DefaultPortalContainer;
                        }
                    }
                    this.ActiveTab.ContainerSrc = SkinController.FormatSkinSrc(this.ActiveTab.ContainerSrc, this);
                    this.ActiveTab.ContainerPath = SkinController.FormatSkinPath(this.ActiveTab.ContainerSrc);
                    this.ActiveTab.Panes = new ArrayList();
                    this.ActiveTab.Modules = new ArrayList();
                    ArrayList crumbs = new ArrayList();
                    GetBreadCrumbsRecursively(ref crumbs, this.ActiveTab.TabID);
                    this.ActiveTab.BreadCrumbs = crumbs;
                }
            }
            if (this.ActiveTab != null)
            {
                Dictionary<string, int> objPaneModules = new Dictionary<string, int>();
                foreach (KeyValuePair<int, ModuleInfo> kvp in objModules.GetTabModules(this.ActiveTab.TabID))
                {
                    ModuleInfo cloneModule = kvp.Value.Clone();
                    if (Null.IsNull(cloneModule.StartDate))
                    {
                        cloneModule.StartDate = System.DateTime.MinValue;
                    }
                    if (Null.IsNull(cloneModule.EndDate))
                    {
                        cloneModule.EndDate = System.DateTime.MaxValue;
                    }
                    if (String.IsNullOrEmpty(cloneModule.ContainerSrc))
                    {
                        cloneModule.ContainerSrc = this.ActiveTab.ContainerSrc;
                    }
                    cloneModule.ContainerSrc = SkinController.FormatSkinSrc(cloneModule.ContainerSrc, this);
                    cloneModule.ContainerPath = SkinController.FormatSkinPath(cloneModule.ContainerSrc);
                    if (objPaneModules.ContainsKey(cloneModule.PaneName) == false)
                    {
                        objPaneModules.Add(cloneModule.PaneName, 0);
                    }
                    cloneModule.PaneModuleCount = 0;
                    if (!cloneModule.IsDeleted)
                    {
                        objPaneModules[cloneModule.PaneName] = objPaneModules[cloneModule.PaneName] + 1;
                        cloneModule.PaneModuleIndex = objPaneModules[cloneModule.PaneName] - 1;
                    }
                    this.ActiveTab.Modules.Add(cloneModule);
                }
                foreach (ModuleInfo module in this.ActiveTab.Modules)
                {
                    module.PaneModuleCount = objPaneModules[module.PaneName];
                }
            }
        }
        private bool VerifyPortalTab(int PortalId, int TabId)
        {
            TabInfo objTab = null;
            TabInfo objSplashTab = null;
            TabInfo objHomeTab = null;
            bool isVerified = false;
            TabController objTabController = new TabController();
            TabCollection portalTabs = objTabController.GetTabsByPortal(PortalId);
            TabCollection hostTabs = objTabController.GetTabsByPortal(Null.NullInteger);
            if (TabId != Null.NullInteger)
            {
                if (portalTabs.TryGetValue(TabId, out objTab))
                {
                    if (!objTab.IsDeleted)
                    {
                        this.ActiveTab = objTab.Clone();
                        isVerified = true;
                    }
                }
            }
            if (!isVerified && TabId != Null.NullInteger)
            {
                if (hostTabs.TryGetValue(TabId, out objTab))
                {
                    if (!objTab.IsDeleted)
                    {
                        this.ActiveTab = objTab.Clone();
                        isVerified = true;
                    }
                }
            }
            if (!isVerified && this.SplashTabId > 0)
            {
                objSplashTab = objTabController.GetTab(this.SplashTabId, PortalId, false);
                this.ActiveTab = objSplashTab.Clone();
                isVerified = true;
            }
            if (!isVerified && this.HomeTabId > 0)
            {
                objHomeTab = objTabController.GetTab(this.HomeTabId, PortalId, false);
                this.ActiveTab = objHomeTab.Clone();
                isVerified = true;
            }
            if (!isVerified)
            {
                foreach (TabInfo tab in portalTabs.AsList())
                {
                    if (!tab.IsDeleted && tab.IsVisible)
                    {
                        this.ActiveTab = tab.Clone();
                        isVerified = true;
                        break;
                    }
                }
            }
            if (Null.IsNull(this.ActiveTab.StartDate))
            {
                this.ActiveTab.StartDate = System.DateTime.MinValue;
            }
            if (Null.IsNull(this.ActiveTab.EndDate))
            {
                this.ActiveTab.EndDate = System.DateTime.MaxValue;
            }
            return isVerified;
        }
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            string OutputFormat = string.Empty;
            if (strFormat == string.Empty)
                OutputFormat = "g";
            string lowerPropertyName = strPropertyName.ToLower();
            if (AccessLevel == Scope.NoSettings) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
            PropertyNotFound = true;
            string result = string.Empty;
            bool PublicProperty = true;
            switch (lowerPropertyName)
            {
                case "url":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.PortalAlias.HTTPAlias, strFormat);
                    break;
                case "portalid":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = (this.PortalId.ToString(OutputFormat, formatProvider));
                    break;
                case "portalname":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.PortalName, strFormat);
                    break;
                case "homedirectory":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.HomeDirectory, strFormat);
                    break;
                case "homedirectorymappath":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.HomeDirectoryMapPath, strFormat);
                    break;
                case "logofile":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.LogoFile, strFormat);
                    break;
                case "footertext":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.FooterText, strFormat);
                    break;
                case "expirydate":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.ExpiryDate.ToString(OutputFormat, formatProvider));
                    break;
                case "userregistration":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.UserRegistration.ToString(OutputFormat, formatProvider));
                    break;
                case "banneradvertising":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.BannerAdvertising.ToString(OutputFormat, formatProvider));
                    break;
                case "currency":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Currency, strFormat);
                    break;
                case "administratorid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.AdministratorId.ToString(OutputFormat, formatProvider));
                    break;
                case "email":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Email, strFormat);
                    break;
                case "hostfee":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.HostFee.ToString(OutputFormat, formatProvider));
                    break;
                case "hostspace":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.HostSpace.ToString(OutputFormat, formatProvider));
                    break;
                case "pagequota":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.PageQuota.ToString(OutputFormat, formatProvider));
                    break;
                case "userquota":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.UserQuota.ToString(OutputFormat, formatProvider));
                    break;
                case "administratorroleid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.AdministratorRoleId.ToString(OutputFormat, formatProvider));
                    break;
                case "administratorrolename":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.AdministratorRoleName, strFormat);
                    break;
                case "registeredroleid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.RegisteredRoleId.ToString(OutputFormat, formatProvider));
                    break;
                case "registeredrolename":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.RegisteredRoleName, strFormat);
                    break;
                case "description":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Description, strFormat);
                    break;
                case "keywords":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.KeyWords, strFormat);
                    break;
                case "backgroundfile":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.BackgroundFile, strFormat);
                    break;
                case "siteloghistory":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.SiteLogHistory.ToString(OutputFormat, formatProvider);
                    break;
                case "admintabid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.AdminTabId.ToString(OutputFormat, formatProvider);
                    break;
                case "supertabid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.SuperTabId.ToString(OutputFormat, formatProvider);
                    break;
                case "splashtabid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.SplashTabId.ToString(OutputFormat, formatProvider);
                    break;
                case "hometabid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.HomeTabId.ToString(OutputFormat, formatProvider);
                    break;
                case "logintabid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.LoginTabId.ToString(OutputFormat, formatProvider);
                    break;
                case "registertabid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.RegisterTabId.ToString(OutputFormat, formatProvider);
                    break;
                case "usertabid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.UserTabId.ToString(OutputFormat, formatProvider);
                    break;
                case "defaultlanguage":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DefaultLanguage, strFormat);
                    break;
                case "timezoneoffset":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = this.TimeZoneOffset.ToString(OutputFormat, formatProvider);
                    break;
                case "users":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.Users.ToString(OutputFormat, formatProvider);
                    break;
                case "pages":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = this.Pages.ToString(OutputFormat, formatProvider);
                    break;
                case "contentvisible":
                    PublicProperty = false;
                    PropertyNotFound = true;
                    break;
                case "controlpanelvisible":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.Boolean2LocalizedYesNo(this.ControlPanelVisible, formatProvider);
                    break;
            }
            if (!PublicProperty && AccessLevel != Scope.Debug)
            {
                PropertyNotFound = true;
                result = PropertyAccess.ContentLocked;
            }
            return result;
        }
        public CacheLevel Cacheability
        {
            get { return CacheLevel.fullyCacheable; }
        }       
    }
}
