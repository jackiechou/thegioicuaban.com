using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Tokens;
using CommonLibrary.Services.Tokens.PropertyAccess;
using System.Web;
using System.IO;
using CommonLibrary.Common;
using CommonLibrary.Services.Localization;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Content;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Entities.Tabs
{
    [XmlRoot("tab", IsNullable = false)]
    [Serializable()]
    public class TabInfo : ContentItem, IHydratable, IPropertyAccess
    {
        private int _TabOrder;
        private int _PortalID;
        private string _TabName;
        private bool _IsVisible;
        private int _ParentId;
        private int _Level;
        private string _IconFile;
        private string _IconFileLarge;
        private bool _DisableLink;
        private string _Title;
        private string _Description;
        private string _KeyWords;
        private bool _IsDeleted;
        private string _Url;
        private string _SkinSrc;
        private string _SkinDoctype;
        private string _ContainerSrc;
        private string _TabPath;
        private System.DateTime _StartDate;
        private System.DateTime _EndDate;
        private bool _HasChildren;
        private int _RefreshInterval;
        private string _PageHeadText;
        private bool _IsSecure;
        private bool _PermanentRedirect;
        private float _SiteMapPriority = (float)0.5;
        private bool _SuperTabIdSet = Null.NullBoolean;
        private Security.Permissions.TabPermissionCollection _TabPermissions;
        private Hashtable _TabSettings;
        private string _AuthorizedRoles;
        private string _AdministratorRoles;
        private string _SkinPath;
        private string _ContainerPath;
        private ArrayList _BreadCrumbs;
        private ArrayList _Panes;
        private ArrayList _Modules;
        private bool _IsSuperTab;
        public TabInfo()
        {
            _PortalID = Null.NullInteger;
            _AuthorizedRoles = Null.NullString;
            _ParentId = Null.NullInteger;
            _IconFile = Null.NullString;
            _IconFileLarge = Null.NullString;
            _AdministratorRoles = Null.NullString;
            _Title = Null.NullString;
            _Description = Null.NullString;
            _KeyWords = Null.NullString;
            _Url = Null.NullString;
            _SkinSrc = Null.NullString;
            _SkinDoctype = Null.NullString;
            _ContainerSrc = Null.NullString;
            _TabPath = Null.NullString;
            _StartDate = Null.NullDate;
            _EndDate = Null.NullDate;
            _RefreshInterval = Null.NullInteger;
            _PageHeadText = Null.NullString;
            _SiteMapPriority = 0.5F;
            _IsVisible = true;
            _DisableLink = false;
        }

        [XmlElement("taborder")]
        public int TabOrder
        {
            get { return _TabOrder; }
            set { _TabOrder = value; }
        }
        [XmlElement("portalid")]
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        [XmlElement("name")]
        public string TabName
        {
            get { return _TabName; }
            set { _TabName = value; }
        }
        [XmlElement("visible")]
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; }
        }
        [XmlElement("parentid")]
        public int ParentId
        {
            get { return _ParentId; }
            set { _ParentId = value; }
        }
        [XmlIgnore()]
        public int Level
        {
            get { return _Level; }
            set { _Level = value; }
        }
        [XmlElement("iconfile")]
        public string IconFile
        {
            get { return _IconFile; }
            set { _IconFile = value; }
        }
        [XmlElement("iconfilelarge")]
        public string IconFileLarge
        {
            get { return _IconFileLarge; }
            set { _IconFileLarge = value; }
        }
        [XmlElement("disabled")]
        public bool DisableLink
        {
            get { return _DisableLink; }
            set { _DisableLink = value; }
        }
        [XmlElement("title")]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }
        [XmlElement("description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        [XmlElement("keywords")]
        public string KeyWords
        {
            get { return _KeyWords; }
            set { _KeyWords = value; }
        }
        [XmlElement("isdeleted")]
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set { _IsDeleted = value; }
        }
        [XmlElement("url")]
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }
        [XmlElement("skinsrc")]
        public string SkinSrc
        {
            get { return _SkinSrc; }
            set { _SkinSrc = value; }
        }
        [XmlElement("skindoctype")]
        public string SkinDoctype
        {
            get
            {
                if (string.IsNullOrEmpty(this.SkinSrc) == false && string.IsNullOrEmpty(_SkinDoctype))
                {
                    _SkinDoctype = GetDocTypeValue();
                }
                return _SkinDoctype;
            }
            set { _SkinDoctype = value; }
        }
        [XmlElement("containersrc")]
        public string ContainerSrc
        {
            get { return _ContainerSrc; }
            set { _ContainerSrc = value; }
        }
        [XmlElement("tabpath")]
        public string TabPath
        {
            get { return _TabPath; }
            set { _TabPath = value; }
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
        [XmlElement("haschildren")]
        public bool HasChildren
        {
            get { return _HasChildren; }
            set { _HasChildren = value; }
        }
        [XmlElement("refreshinterval")]
        public int RefreshInterval
        {
            get { return _RefreshInterval; }
            set { _RefreshInterval = value; }
        }
        [XmlElement("pageheadtext")]
        public string PageHeadText
        {
            get { return _PageHeadText; }
            set { _PageHeadText = value; }
        }
        [XmlElement("issecure")]
        public bool IsSecure
        {
            get { return _IsSecure; }
            set { _IsSecure = value; }
        }
        [XmlElement("permanentredirect")]
        public bool PermanentRedirect
        {
            get { return _PermanentRedirect; }
            set { _PermanentRedirect = value; }
        }
        [XmlElement("sitemappriority")]
        public float SiteMapPriority
        {
            get { return _SiteMapPriority; }
            set { _SiteMapPriority = value; }
        }
        [XmlArray("tabpermissions"), XmlArrayItem("permission")]
        public Security.Permissions.TabPermissionCollection TabPermissions
        {
            get
            {
                if (_TabPermissions == null)
                {
                    _TabPermissions = new TabPermissionCollection(TabPermissionController.GetTabPermissions(TabID, PortalID));
                }
                return _TabPermissions;
            }
        }
        [XmlIgnore()]
        public Hashtable TabSettings
        {
            get
            {
                if (_TabSettings == null)
                {
                    if (TabID == Null.NullInteger)
                    {
                        _TabSettings = new Hashtable();
                    }
                    else
                    {
                        TabController oTabCtrl = new TabController();
                        _TabSettings = oTabCtrl.GetTabSettings(TabID);
                        oTabCtrl = null;
                    }
                }
                return _TabSettings;
            }
        }
        [XmlIgnore()]
        public ArrayList BreadCrumbs
        {
            get { return _BreadCrumbs; }
            set { _BreadCrumbs = value; }
        }
        [XmlIgnore()]
        public string ContainerPath
        {
            get { return _ContainerPath; }
            set { _ContainerPath = value; }
        }
        [XmlIgnore()]
        public string FullUrl
        {
            get
            {
                string strUrl = "";
                switch (TabType)
                {
                    case TabType.Normal:
                        strUrl = Globals.NavigateURL(TabID, IsSuperTab);
                        break;
                    case TabType.Tab:
                        strUrl = Globals.NavigateURL(Convert.ToInt32(_Url));
                        break;
                    case TabType.File:
                        strUrl = Globals.LinkClick(_Url, TabID, Null.NullInteger);
                        break;
                    case TabType.Url:
                        strUrl = _Url;
                        break;
                }
                return strUrl;
            }
        }
        [XmlIgnore()]
        public bool IsSuperTab
        {
            get
            {
                if (_SuperTabIdSet)
                {
                    return _IsSuperTab;
                }
                else
                {
                    return (PortalID == Null.NullInteger);
                }
            }
            set
            {
                _IsSuperTab = value;
                _SuperTabIdSet = true;
            }
        }
        [XmlIgnore()]
        public string IndentedTabName
        {
            get
            {
                string _IndentedTabName = Null.NullString;
                for (int intCounter = 1; intCounter <= Level; intCounter++)
                {
                    _IndentedTabName += "...";
                }
                _IndentedTabName += LocalizedTabName;
                return _IndentedTabName;
            }
        }
        [XmlIgnore()]
        public string LocalizedTabName
        {
            get
            {
                string _LocalizedTabName = Localization.GetString(TabPath + ".String", Localization.GlobalResourceFile, true);
                if (string.IsNullOrEmpty(_LocalizedTabName))
                {
                    _LocalizedTabName = TabName;
                }
                return _LocalizedTabName;
            }
        }
        [XmlIgnore()]
        public ArrayList Modules
        {
            get { return _Modules; }
            set { _Modules = value; }
        }
        [XmlIgnore()]
        public ArrayList Panes
        {
            get { return _Panes; }
            set { _Panes = value; }
        }
        [XmlIgnore()]
        public string SkinPath
        {
            get { return _SkinPath; }
            set { _SkinPath = value; }
        }
        [XmlIgnore()]
        public TabType TabType
        {
            get { return Globals.GetURLType(_Url); }
        }
        private string GetDocTypeValue()
        {
            string doctype = string.Empty;
            doctype = CheckIfDoctypeConfigExists();
            if (string.IsNullOrEmpty(doctype) == true)
            {
                doctype = Host.Host.DefaultDocType;
            }
            return doctype;
        }
        private string CheckIfDoctypeConfigExists()
        {
            string doctypeConfig = string.Empty;
            if (!string.IsNullOrEmpty(SkinSrc))
            {
                string FileName = HttpContext.Current.Server.MapPath(SkinSrc.Replace(".ascx", ".doctype.xml"));
                string strLang = System.Globalization.CultureInfo.CurrentCulture.ToString();
                if (File.Exists(FileName))
                {
                    try
                    {
                        System.Xml.XmlDocument xmlSkinDocType = new System.Xml.XmlDocument();
                        xmlSkinDocType.Load(FileName);
                        string strDocType = xmlSkinDocType.FirstChild.InnerText.ToString();
                        doctypeConfig = strDocType;
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
            return doctypeConfig;
        }
        public TabInfo Clone()
        {
            TabInfo objTabInfo = new TabInfo();
            objTabInfo.TabID = this.TabID;
            objTabInfo.TabOrder = this.TabOrder;
            objTabInfo.PortalID = this.PortalID;
            objTabInfo.TabName = this.TabName;
            objTabInfo.IsVisible = this.IsVisible;
            objTabInfo.ParentId = this.ParentId;
            objTabInfo.Level = this.Level;
            objTabInfo.IconFile = this.IconFile;
            objTabInfo.IconFileLarge = this.IconFileLarge;
            objTabInfo.DisableLink = this.DisableLink;
            objTabInfo.Title = this.Title;
            objTabInfo.Description = this.Description;
            objTabInfo.KeyWords = this.KeyWords;
            objTabInfo.IsDeleted = this.IsDeleted;
            objTabInfo.Url = this.Url;
            objTabInfo.SkinSrc = this.SkinSrc;
            objTabInfo.ContainerSrc = this.ContainerSrc;
            objTabInfo.TabPath = this.TabPath;
            objTabInfo.StartDate = this.StartDate;
            objTabInfo.EndDate = this.EndDate;
            objTabInfo.HasChildren = this.HasChildren;
            objTabInfo.SkinPath = this.SkinPath;
            objTabInfo.ContainerPath = this.ContainerPath;
            objTabInfo.IsSuperTab = this.IsSuperTab;
            objTabInfo.RefreshInterval = this.RefreshInterval;
            objTabInfo.PageHeadText = this.PageHeadText;
            objTabInfo.IsSecure = this.IsSecure;
            objTabInfo.PermanentRedirect = this.PermanentRedirect;
            if (this.BreadCrumbs != null)
            {
                objTabInfo.BreadCrumbs = new ArrayList();
                foreach (TabInfo t in this.BreadCrumbs)
                {
                    objTabInfo.BreadCrumbs.Add(t.Clone());
                }
            }
            objTabInfo.ContentItemId = this.ContentItemId;
            objTabInfo.Panes = new ArrayList();
            objTabInfo.Modules = new ArrayList();
            return objTabInfo;
        }
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope CurrentScope, ref bool PropertyNotFound)
        {
            string OutputFormat = string.Empty;
            if (strFormat == string.Empty)
                OutputFormat = "g";
            string lowerPropertyName = strPropertyName.ToLower();
            if (CurrentScope == Scope.NoSettings) { PropertyNotFound = true; return PropertyAccess.ContentLocked; }
            PropertyNotFound = true;
            string result = string.Empty;
            bool PublicProperty = true;
            switch (lowerPropertyName)
            {
                case "tabid":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = (this.TabID.ToString(OutputFormat, formatProvider));
                    break;
                case "taborder":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.TabOrder.ToString(OutputFormat, formatProvider));
                    break;
                case "portalid":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = (this.PortalID.ToString(OutputFormat, formatProvider));
                    break;
                case "tabname":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.LocalizedTabName, strFormat);
                    break;
                case "isvisible":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.IsVisible, formatProvider));
                    break;
                case "parentid":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.ParentId.ToString(OutputFormat, formatProvider));
                    break;
                case "level":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.Level.ToString(OutputFormat, formatProvider));
                    break;
                case "iconfile":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.IconFile, strFormat);
                    break;
                case "iconfilelarge":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.IconFileLarge, strFormat);
                    break;
                case "disablelink":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.DisableLink, formatProvider));
                    break;
                case "title":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Title, strFormat);
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
                case "isdeleted":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.IsDeleted, formatProvider));
                    break;
                case "url":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.Url, strFormat);
                    break;
                case "skinsrc":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.SkinSrc, strFormat);
                    break;
                case "containersrc":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.ContainerSrc, strFormat);
                    break;
                case "tabpath":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.TabPath, strFormat);
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
                case "haschildren":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.HasChildren, formatProvider));
                    break;
                case "refreshinterval":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (this.RefreshInterval.ToString(OutputFormat, formatProvider));
                    break;
                case "pageheadtext":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.PageHeadText, strFormat);
                    break;
                case "skinpath":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.SkinPath, strFormat);
                    break;
                case "skindoctype":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.SkinDoctype, strFormat);
                    break;
                case "containerpath":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.ContainerPath, strFormat);
                    break;
                case "issupertab":
                    PublicProperty = false;
                    PropertyNotFound = false;
                    result = (PropertyAccess.Boolean2LocalizedYesNo(this.IsSuperTab, formatProvider));
                    break;
                case "fullurl":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.FullUrl, strFormat);
                    break;
                case "sitemappriority":
                    PublicProperty = true;
                    PropertyNotFound = false;
                    result = PropertyAccess.FormatString(this.SiteMapPriority.ToString(), strFormat);
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
        public override void Fill(System.Data.IDataReader dr)
        {
            //Call the base classes fill method to populate base class proeprties
            base.FillInternal(dr);
            TabOrder = Null.SetNullInteger(dr["TabOrder"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            TabName = Null.SetNullString(dr["TabName"]);
            IsVisible = Null.SetNullBoolean(dr["IsVisible"]);
            ParentId = Null.SetNullInteger(dr["ParentId"]);
            Level = Null.SetNullInteger(dr["Level"]);
            IconFile = Null.SetNullString(dr["IconFile"]);
            IconFileLarge = Null.SetNullString(dr["IconFileLarge"]);
            DisableLink = Null.SetNullBoolean(dr["DisableLink"]);
            Title = Null.SetNullString(dr["Title"]);
            Description = Null.SetNullString(dr["Description"]);
            KeyWords = Null.SetNullString(dr["KeyWords"]);
            IsDeleted = Null.SetNullBoolean(dr["IsDeleted"]);
            Url = Null.SetNullString(dr["Url"]);
            SkinSrc = Null.SetNullString(dr["SkinSrc"]);
            ContainerSrc = Null.SetNullString(dr["ContainerSrc"]);
            TabPath = Null.SetNullString(dr["TabPath"]);
            StartDate = Null.SetNullDateTime(dr["StartDate"]);
            EndDate = Null.SetNullDateTime(dr["EndDate"]);
            HasChildren = Null.SetNullBoolean(dr["HasChildren"]);
            RefreshInterval = Null.SetNullInteger(dr["RefreshInterval"]);
            PageHeadText = Null.SetNullString(dr["PageHeadText"]);
            IsSecure = Null.SetNullBoolean(dr["IsSecure"]);
            PermanentRedirect = Null.SetNullBoolean(dr["PermanentRedirect"]);
            SiteMapPriority = Null.SetNullSingle(dr["SiteMapPriority"]);
            BreadCrumbs = null;
            Panes = null;
            Modules = null;
        }
        [XmlIgnore()]
        public override int KeyID
        {
            get { return TabID; }
            set { TabID = value; }
        }       
    }
}
