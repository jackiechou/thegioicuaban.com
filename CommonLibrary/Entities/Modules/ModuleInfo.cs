using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Entities.Content;
using System.Collections;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Services.Tokens.PropertyAccess;
using CommonLibrary.Services.Tokens;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Modules.Definitions;
using CommonLibrary.ComponentModel;
using CommonLibrary.Services.ModuleCache;
using CommonLibrary.Services.Localization;

namespace CommonLibrary.Entities.Modules
{
    [XmlRoot("module", IsNullable = false)]
    [Serializable()]
    public class ModuleInfo : ContentItem, IHydratable, IPropertyAccess
    {
        private int _PortalID;
        private string _ModuleTitle;
        private bool _AllTabs;
        private bool _IsDeleted;
        private bool _InheritViewPermissions;
        private string _Header;
        private string _Footer;
        private System.DateTime _StartDate;
        private System.DateTime _EndDate;
        private int _TabModuleID;
        private string _PaneName;
        private int _ModuleOrder;
        private int _CacheTime;
        private string _CacheMethod;
        private string _Alignment;
        private string _Color;
        private string _Border;
        private string _IconFile;
        private VisibilityState _Visibility;
        private string _ContainerSrc;
        private bool _DisplayTitle;
        private bool _DisplayPrint;
        private bool _DisplaySyndicate;
        private bool _IsWebSlice;
        private string _WebSliceTitle;
        private DateTime _WebSliceExpiryDate;
        private int _WebSliceTTL;
        private int _DesktopModuleID;
        private DesktopModuleInfo _DesktopModule;
        private int _ModuleDefID;
        private ModuleDefinitionInfo _ModuleDefinition;
        private int _ModuleControlId;
        private ModuleControlInfo _ModuleControl;
        private string _AuthorizedEditRoles;
        private string _AuthorizedViewRoles;
        private string _AuthorizedRoles;
        private ModulePermissionCollection _ModulePermissions;
        private Hashtable _ModuleSettings;
        private Hashtable _TabModuleSettings;
        private TabPermissionCollection _TabPermissions;
        private string _ContainerPath;
        private int _PaneModuleIndex;
        private int _PaneModuleCount;
        private bool _IsDefaultModule;
        private bool _AllModules;

        public ModuleInfo()
        {
            _PortalID = Null.NullInteger;
            _TabModuleID = Null.NullInteger;
            _DesktopModuleID = Null.NullInteger;
            _ModuleDefID = Null.NullInteger;
            _ModuleTitle = Null.NullString;
            _AuthorizedEditRoles = Null.NullString;
            _AuthorizedViewRoles = Null.NullString;
            _Alignment = Null.NullString;
            _Color = Null.NullString;
            _Border = Null.NullString;
            _IconFile = Null.NullString;
            _Header = Null.NullString;
            _Footer = Null.NullString;
            _StartDate = Null.NullDate;
            _EndDate = Null.NullDate;
            _ContainerSrc = Null.NullString;
            _DisplayTitle = true;
            _DisplayPrint = true;
            _DisplaySyndicate = false;
        }

        [XmlElement("portalid")]
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        [XmlElement("title")]
        public string ModuleTitle
        {
            get { return _ModuleTitle; }
            set { _ModuleTitle = value; }
        }
        [XmlElement("alltabs")]
        public bool AllTabs
        {
            get { return _AllTabs; }
            set { _AllTabs = value; }
        }
        [XmlElement("isdeleted")]
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set { _IsDeleted = value; }
        }
        [XmlElement("inheritviewpermissions")]
        public bool InheritViewPermissions
        {
            get { return _InheritViewPermissions; }
            set { _InheritViewPermissions = value; }
        }
        [XmlElement("header")]
        public string Header
        {
            get { return _Header; }
            set { _Header = value; }
        }
        [XmlElement("footer")]
        public string Footer
        {
            get { return _Footer; }
            set { _Footer = value; }
        }
        [XmlElement("startdate")]
        public System.DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        [XmlElement("enddate")]
        public System.DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        [XmlElement("tabmoduleid")]
        public int TabModuleID
        {
            get { return _TabModuleID; }
            set { _TabModuleID = value; }
        }
        [XmlElement("panename")]
        public string PaneName
        {
            get { return _PaneName; }
            set { _PaneName = value; }
        }
        [XmlElement("moduleorder")]
        public int ModuleOrder
        {
            get { return _ModuleOrder; }
            set { _ModuleOrder = value; }
        }
        [XmlElement("cachetime")]
        public int CacheTime
        {
            get { return _CacheTime; }
            set { _CacheTime = value; }
        }
        [XmlElement("cachemethod")]
        public string CacheMethod
        {
            get { return _CacheMethod; }
            set { _CacheMethod = value; }
        }
        [XmlElement("alignment")]
        public string Alignment
        {
            get { return _Alignment; }
            set { _Alignment = value; }
        }
        [XmlElement("color")]
        public string Color
        {
            get { return _Color; }
            set { _Color = value; }
        }
        [XmlElement("border")]
        public string Border
        {
            get { return _Border; }
            set { _Border = value; }
        }
        [XmlElement("iconfile")]
        public string IconFile
        {
            get { return _IconFile; }
            set { _IconFile = value; }
        }
        [XmlElement("visibility")]
        public VisibilityState Visibility
        {
            get { return _Visibility; }
            set { _Visibility = value; }
        }
        [XmlElement("containersrc")]
        public string ContainerSrc
        {
            get { return _ContainerSrc; }
            set { _ContainerSrc = value; }
        }
        [XmlElement("displaytitle")]
        public bool DisplayTitle
        {
            get { return _DisplayTitle; }
            set { _DisplayTitle = value; }
        }
        [XmlElement("displayprint")]
        public bool DisplayPrint
        {
            get { return _DisplayPrint; }
            set { _DisplayPrint = value; }
        }
        [XmlElement("displaysyndicate")]
        public bool DisplaySyndicate
        {
            get { return _DisplaySyndicate; }
            set { _DisplaySyndicate = value; }
        }
        [XmlElement("iswebslice")]
        public bool IsWebSlice
        {
            get { return _IsWebSlice; }
            set { _IsWebSlice = value; }
        }
        [XmlElement("webslicetitle")]
        public string WebSliceTitle
        {
            get { return _WebSliceTitle; }
            set { _WebSliceTitle = value; }
        }
        [XmlElement("websliceexpirydate")]
        public DateTime WebSliceExpiryDate
        {
            get { return _WebSliceExpiryDate; }
            set { _WebSliceExpiryDate = value; }
        }
        [XmlElement("webslicettl")]
        public int WebSliceTTL
        {
            get { return _WebSliceTTL; }
            set { _WebSliceTTL = value; }
        }
        [XmlIgnore()]
        public bool HideAdminBorder
        {
            get
            {
                object setting = TabModuleSettings["hideadminborder"];
                if (setting == null || string.IsNullOrEmpty(setting.ToString()))
                {
                    return false;
                }

                bool val = false;
                bool.TryParse(setting.ToString(), out val);
                return val;
            }
        }
        [XmlIgnore()]
        public int DesktopModuleID
        {
            get { return _DesktopModuleID; }
            set { _DesktopModuleID = value; }
        }
        [XmlIgnore()]
        public DesktopModuleInfo DesktopModule
        {
            get
            {
                if (_DesktopModule == null)
                {
                    if (DesktopModuleID > Null.NullInteger)
                    {
                        _DesktopModule = DesktopModuleController.GetDesktopModule(DesktopModuleID, PortalID);
                    }
                    else
                    {
                        _DesktopModule = new DesktopModuleInfo();
                    }
                }
                return _DesktopModule;
            }
        }
        [XmlIgnore()]
        public int ModuleDefID
        {
            get { return _ModuleDefID; }
            set { _ModuleDefID = value; }
        }
        [XmlIgnore()]
        public ModuleDefinitionInfo ModuleDefinition
        {
            get
            {
                if (_ModuleDefinition == null)
                {
                    if (ModuleDefID > Null.NullInteger)
                    {
                        _ModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByID(ModuleDefID);
                    }
                    else
                    {
                        _ModuleDefinition = new ModuleDefinitionInfo();
                    }
                }
                return _ModuleDefinition;
            }
        }
        [XmlIgnore()]
        public int ModuleControlId
        {
            get { return _ModuleControlId; }
            set { _ModuleControlId = value; }
        }
        public ModuleControlInfo ModuleControl
        {
            get
            {
                if (_ModuleControl == null)
                {
                    if (ModuleControlId > Null.NullInteger)
                    {
                        _ModuleControl = ModuleControlController.GetModuleControl(ModuleControlId);
                    }
                    else
                    {
                        _ModuleControl = new ModuleControlInfo();
                    }
                }
                return _ModuleControl;
            }
        }
        [XmlArray("modulepermissions"), XmlArrayItem("permission")]
        public ModulePermissionCollection ModulePermissions
        {
            get
            {
                if (_ModulePermissions == null)
                {
                    _ModulePermissions = new ModulePermissionCollection(ModulePermissionController.GetModulePermissions(ModuleID, TabID));
                }
                return _ModulePermissions;
            }
            set { _ModulePermissions = value; }
        }
        [XmlIgnore()]
        public Hashtable ModuleSettings
        {
            get
            {
                if (_ModuleSettings == null)
                {
                    if (ModuleID == Null.NullInteger)
                    {
                        _ModuleSettings = new Hashtable();
                    }
                    else
                    {
                        ModuleController oModuleCtrl = new ModuleController();
                        _ModuleSettings = oModuleCtrl.GetModuleSettings(ModuleID);
                        oModuleCtrl = null;
                    }
                }
                return _ModuleSettings;
            }
        }
        [XmlIgnore()]
        public Hashtable TabModuleSettings
        {
            get
            {
                if (_TabModuleSettings == null)
                {
                    if (TabModuleID == Null.NullInteger)
                    {
                        _TabModuleSettings = new Hashtable();
                    }
                    else
                    {
                        ModuleController oModuleCtrl = new ModuleController();
                        _TabModuleSettings = oModuleCtrl.GetTabModuleSettings(TabModuleID);
                        oModuleCtrl = null;
                    }
                }
                return _TabModuleSettings;
            }
        }
        [XmlIgnore()]
        public string ContainerPath
        {
            get { return _ContainerPath; }
            set { _ContainerPath = value; }
        }
        [XmlIgnore()]
        public int PaneModuleIndex
        {
            get { return _PaneModuleIndex; }
            set { _PaneModuleIndex = value; }
        }
        [XmlIgnore()]
        public int PaneModuleCount
        {
            get { return _PaneModuleCount; }
            set { _PaneModuleCount = value; }
        }
        [XmlIgnore()]
        public bool IsDefaultModule
        {
            get { return _IsDefaultModule; }
            set { _IsDefaultModule = value; }
        }
        [XmlIgnore()]
        public bool AllModules
        {
            get { return _AllModules; }
            set { _AllModules = value; }
        }
        public ModuleInfo Clone()
        {
            ModuleInfo objModuleInfo = new ModuleInfo();
            objModuleInfo.PortalID = this.PortalID;
            objModuleInfo.TabID = this.TabID;
            objModuleInfo.TabModuleID = this.TabModuleID;
            objModuleInfo.ModuleID = this.ModuleID;
            objModuleInfo.ModuleOrder = this.ModuleOrder;
            objModuleInfo.PaneName = this.PaneName;
            objModuleInfo.ModuleTitle = this.ModuleTitle;
            objModuleInfo.CacheTime = this.CacheTime;
            objModuleInfo.CacheMethod = this.CacheMethod;
            objModuleInfo.Alignment = this.Alignment;
            objModuleInfo.Color = this.Color;
            objModuleInfo.Border = this.Border;
            objModuleInfo.IconFile = this.IconFile;
            objModuleInfo.AllTabs = this.AllTabs;
            objModuleInfo.Visibility = this.Visibility;
            objModuleInfo.IsDeleted = this.IsDeleted;
            objModuleInfo.Header = this.Header;
            objModuleInfo.Footer = this.Footer;
            objModuleInfo.StartDate = this.StartDate;
            objModuleInfo.EndDate = this.EndDate;
            objModuleInfo.ContainerSrc = this.ContainerSrc;
            objModuleInfo.DisplayTitle = this.DisplayTitle;
            objModuleInfo.DisplayPrint = this.DisplayPrint;
            objModuleInfo.DisplaySyndicate = this.DisplaySyndicate;
            objModuleInfo.IsWebSlice = this.IsWebSlice;
            objModuleInfo.WebSliceTitle = this.WebSliceTitle;
            objModuleInfo.WebSliceExpiryDate = this.WebSliceExpiryDate;
            objModuleInfo.WebSliceTTL = this.WebSliceTTL;
            objModuleInfo.InheritViewPermissions = this.InheritViewPermissions;
            objModuleInfo.DesktopModuleID = this.DesktopModuleID;
            objModuleInfo.ModuleDefID = this.ModuleDefID;
            objModuleInfo.ModuleControlId = this.ModuleControlId;
            objModuleInfo.ContainerPath = this.ContainerPath;
            objModuleInfo.PaneModuleIndex = this.PaneModuleIndex;
            objModuleInfo.PaneModuleCount = this.PaneModuleCount;
            objModuleInfo.IsDefaultModule = this.IsDefaultModule;
            objModuleInfo.AllModules = this.AllModules;
            objModuleInfo._ModulePermissions = this._ModulePermissions;
            objModuleInfo._TabPermissions = this._TabPermissions;
            objModuleInfo.ContentItemId = this.ContentItemId;
            return objModuleInfo;
        }
        public string GetEffectiveCacheMethod()
        {
            string effectiveCacheMethod;
            if (!string.IsNullOrEmpty(_CacheMethod))
            {
                effectiveCacheMethod = _CacheMethod;
            }
            else if (!string.IsNullOrEmpty(Entities.Host.Host.ModuleCachingMethod))
            {
                effectiveCacheMethod = Entities.Host.Host.ModuleCachingMethod;
            }
            else
            {
                ModuleCachingProvider defaultModuleCache = ComponentFactory.GetComponent<ModuleCachingProvider>();
                effectiveCacheMethod = (from provider in ModuleCachingProvider.GetProviderList()
                                        where provider.Value.Equals(defaultModuleCache)
                                        select provider.Key).SingleOrDefault();
            }
            if (string.IsNullOrEmpty(effectiveCacheMethod))
            {
                throw new InvalidOperationException(Localization.GetString("EXCEPTION_ModuleCacheMissing"));
            }
            return effectiveCacheMethod;
        }
        public void Initialize(int PortalId)
        {
            _PortalID = PortalId;
            //_TabID = -1;
            //_ModuleID = -1;
            _ModuleDefID = Null.NullInteger;
            _ModuleOrder = Null.NullInteger;
            _PaneName = Null.NullString;
            _ModuleTitle = Null.NullString;
            _CacheTime = 0;
            _CacheMethod = Null.NullString;
            _Alignment = Null.NullString;
            _Color = Null.NullString;
            _Border = Null.NullString;
            _IconFile = Null.NullString;
            _AllTabs = Null.NullBoolean;
            _Visibility = VisibilityState.Maximized;
            _IsDeleted = Null.NullBoolean;
            _Header = Null.NullString;
            _Footer = Null.NullString;
            _StartDate = Null.NullDate;
            _EndDate = Null.NullDate;
            _DisplayTitle = true;
            _DisplayPrint = true;
            _DisplaySyndicate = Null.NullBoolean;
            _IsWebSlice = Null.NullBoolean;
            _WebSliceTitle = "";
            _WebSliceExpiryDate = Null.NullDate;
            _WebSliceTTL = 0;
            _InheritViewPermissions = Null.NullBoolean;
            _ContainerSrc = Null.NullString;
            _DesktopModuleID = Null.NullInteger;
            _ModuleControlId = Null.NullInteger;
            _ContainerPath = Null.NullString;
            _PaneModuleIndex = 0;
            _PaneModuleCount = 0;
            _IsDefaultModule = Null.NullBoolean;
            _AllModules = Null.NullBoolean;
            if (PortalSettings.Current.DefaultModuleId > Null.NullInteger && PortalSettings.Current.DefaultTabId > Null.NullInteger)
            {
                ModuleController objModules = new ModuleController();
                ModuleInfo objModule = objModules.GetModule(PortalSettings.Current.DefaultModuleId, PortalSettings.Current.DefaultTabId, true);
                if (objModule != null)
                {
                    _Alignment = objModule.Alignment;
                    _Color = objModule.Color;
                    _Border = objModule.Border;
                    _IconFile = objModule.IconFile;
                    _Visibility = objModule.Visibility;
                    _ContainerSrc = objModule.ContainerSrc;
                    _DisplayTitle = objModule.DisplayTitle;
                    _DisplayPrint = objModule.DisplayPrint;
                    _DisplaySyndicate = objModule.DisplaySyndicate;
                }
            }
        }
        public override void Fill(System.Data.IDataReader dr)
        {
            //Call the base classes fill method to populate base class properties
            base.FillInternal(dr);

            PortalID = Null.SetNullInteger(dr["PortalID"]);
            ModuleDefID = Null.SetNullInteger(dr["ModuleDefID"]);
            ModuleTitle = Null.SetNullString(dr["ModuleTitle"]);
            AllTabs = Null.SetNullBoolean(dr["AllTabs"]);
            IsDeleted = Null.SetNullBoolean(dr["IsDeleted"]);
            InheritViewPermissions = Null.SetNullBoolean(dr["InheritViewPermissions"]);
            Header = Null.SetNullString(dr["Header"]);
            Footer = Null.SetNullString(dr["Footer"]);
            StartDate = Null.SetNullDateTime(dr["StartDate"]);
            EndDate = Null.SetNullDateTime(dr["EndDate"]);
            try
            {
                TabModuleID = Null.SetNullInteger(dr["TabModuleID"]);
                ModuleOrder = Null.SetNullInteger(dr["ModuleOrder"]);
                PaneName = Null.SetNullString(dr["PaneName"]);
                CacheTime = Null.SetNullInteger(dr["CacheTime"]);
                CacheMethod = Null.SetNullString(dr["CacheMethod"]);
                Alignment = Null.SetNullString(dr["Alignment"]);
                Color = Null.SetNullString(dr["Color"]);
                Border = Null.SetNullString(dr["Border"]);
                IconFile = Null.SetNullString(dr["IconFile"]);
                int visible = Null.SetNullInteger(dr["Visibility"]);
                if (visible == Null.NullInteger)
                {
                    Visibility = VisibilityState.Maximized;
                }
                else
                {
                    switch (visible)
                    {
                        case 0:
                            Visibility = VisibilityState.Maximized;
                            break;
                        case 1:
                            Visibility = VisibilityState.Minimized;
                            break;
                        case 2:
                            Visibility = VisibilityState.None;
                            break;

                    }
                }
                ContainerSrc = Null.SetNullString(dr["ContainerSrc"]);
                DisplayTitle = Null.SetNullBoolean(dr["DisplayTitle"]);
                DisplayPrint = Null.SetNullBoolean(dr["DisplayPrint"]);
                DisplaySyndicate = Null.SetNullBoolean(dr["DisplaySyndicate"]);
                IsWebSlice = Null.SetNullBoolean(dr["IsWebSlice"]);
                if (IsWebSlice)
                {
                    WebSliceTitle = Null.SetNullString(dr["WebSliceTitle"]);
                    WebSliceExpiryDate = Null.SetNullDateTime(dr["WebSliceExpiryDate"]);
                    WebSliceTTL = Null.SetNullInteger(dr["WebSliceTTL"]);
                }
                DesktopModuleID = Null.SetNullInteger(dr["DesktopModuleID"]);
                ModuleControlId = Null.SetNullInteger(dr["ModuleControlID"]);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        [XmlIgnore()]
        public override int KeyID
        {
            get { return ModuleID; }
            set { ModuleID = value; }
        }
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, Users.UserInfo AccessingUser, Scope CurrentScope, ref bool PropertyNotFound)
        {
            string OutputFormat = string.Empty;
            if (strFormat == string.Empty)
                OutputFormat = "g";
            if (CurrentScope == Scope.NoSettings) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
            PropertyNotFound = true;
            string result = string.Empty;
            bool PublicProperty = true;
            switch (strPropertyName.ToLower())
            {
                case "portalid":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = (this.PortalID.ToString(OutputFormat, formatProvider));
                    break;
                case "tabid":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = (this.TabID.ToString(OutputFormat, formatProvider));
                    break;
                case "tabmoduleid":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = (this.TabModuleID.ToString(OutputFormat, formatProvider));
                    break;
                case "moduleid":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = (this.ModuleID.ToString(OutputFormat, formatProvider));
                    break;
                case "moduledefid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.ModuleDefID.ToString(OutputFormat, formatProvider));
                    break;
                case "moduleorder":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.ModuleOrder.ToString(OutputFormat, formatProvider));
                    break;
                case "panename":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.PaneName, strFormat);
                    break;
                case "moduletitle":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.ModuleTitle, strFormat);
                    break;
                case "cachetime":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.CacheTime.ToString(OutputFormat, formatProvider));
                    break;
                case "cachemethod":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.CacheMethod, strFormat);
                    break;
                case "alignment":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Alignment, strFormat);
                    break;
                case "color":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Color, strFormat);
                    break;
                case "border":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Border, strFormat);
                    break;
                case "iconfile":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.IconFile, strFormat);
                    break;
                case "alltabs":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.AllTabs, formatProvider));
                    break;
                case "isdeleted":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.IsDeleted, formatProvider));
                    break;
                case "header":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Header, strFormat);
                    break;
                case "footer":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Footer, strFormat);
                    break;
                case "startdate":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.StartDate.ToString(OutputFormat, formatProvider));
                    break;
                case "enddate":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.EndDate.ToString(OutputFormat, formatProvider));
                    break;
                case "containersrc":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.ContainerSrc, strFormat);
                    break;
                case "displaytitle":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DisplayTitle, formatProvider));
                    break;
                case "displayprint":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DisplayPrint, formatProvider));
                    break;
                case "displaysyndicate":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DisplaySyndicate, formatProvider));
                    break;
                case "iswebslice":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.IsWebSlice, formatProvider));
                    break;
                case "webslicetitle":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.WebSliceTitle, strFormat);
                    break;
                case "websliceexpirydate":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.WebSliceExpiryDate.ToString(OutputFormat, formatProvider));
                    break;
                case "webslicettl":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.WebSliceTTL.ToString(OutputFormat, formatProvider));
                    break;
                case "inheritviewpermissions":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.InheritViewPermissions, formatProvider));
                    break;
                case "desktopmoduleid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.DesktopModuleID.ToString(OutputFormat, formatProvider));
                    break;
                case "friendlyname":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.FriendlyName, strFormat);
                    break;
                case "foldername":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.FolderName, strFormat);
                    break;
                case "description":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.Description, strFormat);
                    break;
                case "version":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.Version, strFormat);
                    break;
                case "ispremium":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DesktopModule.IsPremium, formatProvider));
                    break;
                case "isadmin":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DesktopModule.IsAdmin, formatProvider));
                    break;
                case "businesscontrollerclass":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.BusinessControllerClass, strFormat);
                    break;
                case "modulename":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.ModuleName, strFormat);
                    break;
                case "supportedfeatures":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.DesktopModule.SupportedFeatures.ToString(OutputFormat, formatProvider));
                    break;
                case "compatibleversions":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.CompatibleVersions, strFormat);
                    break;
                case "dependencies":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.Dependencies, strFormat);
                    break;
                case "permissions":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.DesktopModule.Permissions, strFormat);
                    break;
                case "defaultcachetime":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.ModuleDefinition.DefaultCacheTime.ToString(OutputFormat, formatProvider));
                    break;
                case "modulecontrolid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.ModuleControlId.ToString(OutputFormat, formatProvider));
                    break;
                case "controlsrc":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.ModuleControl.ControlSrc, strFormat);
                    break;
                case "controltitle":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.ModuleControl.ControlTitle, strFormat);
                    break;
                case "helpurl":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.ModuleControl.HelpURL, strFormat);
                    break;
                case "supportspartialrendering":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.ModuleControl.SupportsPartialRendering, formatProvider));
                    break;
                case "containerpath":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.ContainerPath, strFormat);
                    break;
                case "panemoduleindex":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.PaneModuleIndex.ToString(OutputFormat, formatProvider));
                    break;
                case "panemodulecount":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.PaneModuleCount.ToString(OutputFormat, formatProvider));
                    break;
                case "isdefaultmodule":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.IsDefaultModule, formatProvider));
                    break;
                case "allmodules":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.AllModules, formatProvider));
                    break;
                case "isportable":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DesktopModule.IsPortable, formatProvider));
                    break;
                case "issearchable":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DesktopModule.IsSearchable, formatProvider));
                    break;
                case "isupgradeable":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DesktopModule.IsUpgradeable, formatProvider));
                    break;
            }
            if (!PublicProperty && CurrentScope != Scope.Debug)
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
