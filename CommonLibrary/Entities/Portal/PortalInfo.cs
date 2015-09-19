using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Security.Roles;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Entities.Portal
{
    [XmlRoot("settings", IsNullable = false)]
    [Serializable()]
    public class PortalInfo : BaseEntityInfo, IHydratable
    {
        private int _PortalID;
        private string _PortalName;
        private string _LogoFile;
        private string _FooterText;
        private System.DateTime _ExpiryDate;
        private int _UserRegistration;
        private int _BannerAdvertising;
        private int _AdministratorId;
        private string _Currency;
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
        private string _PaymentProcessor;
        private string _ProcessorUserId;
        private string _ProcessorPassword;
        private int _SiteLogHistory;
        private string _Email;
        private int _AdminTabId;
        private int _SuperTabId;
        private int _Users = Null.NullInteger;
        private int _Pages = Null.NullInteger;
        private int _SplashTabId;
        private int _HomeTabId;
        private int _LoginTabId;
        private int _RegisterTabId;
        private int _UserTabId;
        private string _DefaultLanguage;
        private int _TimeZoneOffset;
        private string _HomeDirectory;
        private string _Version;
        public PortalInfo()
        {
        }
        [XmlElement("footertext")]
        public string FooterText
        {
            get { return _FooterText; }
            set { _FooterText = value; }
        }
        [XmlElement("logofile")]
        public string LogoFile
        {
            get { return _LogoFile; }
            set { _LogoFile = value; }
        }
        [XmlElement("portalid")]
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        [XmlElement("portalname")]
        public string PortalName
        {
            get { return _PortalName; }
            set { _PortalName = value; }
        }
        [XmlElement("expirydate")]
        public System.DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set { _ExpiryDate = value; }
        }
        [XmlElement("userregistration")]
        public int UserRegistration
        {
            get { return _UserRegistration; }
            set { _UserRegistration = value; }
        }
        [XmlElement("banneradvertising")]
        public int BannerAdvertising
        {
            get { return _BannerAdvertising; }
            set { _BannerAdvertising = value; }
        }
        [XmlElement("administratorid")]
        public int AdministratorId
        {
            get { return _AdministratorId; }
            set { _AdministratorId = value; }
        }
        [XmlElement("currency")]
        public string Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }
        [XmlElement("hostfee")]
        public float HostFee
        {
            get { return _HostFee; }
            set { _HostFee = value; }
        }
        [XmlElement("hostspace")]
        public int HostSpace
        {
            get { return _HostSpace; }
            set { _HostSpace = value; }
        }
        [XmlElement("pagequota")]
        public int PageQuota
        {
            get { return _PageQuota; }
            set { _PageQuota = value; }
        }
        [XmlElement("userquota")]
        public int UserQuota
        {
            get { return _UserQuota; }
            set { _UserQuota = value; }
        }
        [XmlElement("administratorroleid")]
        public int AdministratorRoleId
        {
            get { return _AdministratorRoleId; }
            set { _AdministratorRoleId = value; }
        }
        [XmlElement("administratorrolename")]
        public string AdministratorRoleName
        {
            get
            {
                if (_AdministratorRoleName == Null.NullString && AdministratorRoleId > Null.NullInteger)
                {
                    RoleInfo adminRole = new RoleController().GetRole(AdministratorRoleId, PortalID);
                    if (adminRole != null)
                    {
                        _AdministratorRoleName = adminRole.RoleName;
                    }
                }
                return _AdministratorRoleName;
            }
            set { _AdministratorRoleName = value; }
        }
        [XmlElement("registeredroleid")]
        public int RegisteredRoleId
        {
            get { return _RegisteredRoleId; }
            set { _RegisteredRoleId = value; }
        }
        [XmlElement("registeredrolename")]
        public string RegisteredRoleName
        {
            get
            {
                if (_RegisteredRoleName == Null.NullString && RegisteredRoleId > Null.NullInteger)
                {
                    RoleInfo regUsersRole = new RoleController().GetRole(RegisteredRoleId, PortalID);
                    if (regUsersRole != null)
                    {
                        _RegisteredRoleName = regUsersRole.RoleName;
                    }
                }
                return _RegisteredRoleName;
            }
            set { _RegisteredRoleName = value; }
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
        [XmlElement("backgroundfile")]
        public string BackgroundFile
        {
            get { return _BackgroundFile; }
            set { _BackgroundFile = value; }
        }
        [XmlIgnore()]
        public Guid GUID
        {
            get { return _GUID; }
            set { _GUID = value; }
        }
        [XmlElement("paymentprocessor")]
        public string PaymentProcessor
        {
            get { return _PaymentProcessor; }
            set { _PaymentProcessor = value; }
        }
        [XmlElement("processorpassword")]
        public string ProcessorPassword
        {
            get { return _ProcessorPassword; }
            set { _ProcessorPassword = value; }
        }
        [XmlElement("processoruserid")]
        public string ProcessorUserId
        {
            get { return _ProcessorUserId; }
            set { _ProcessorUserId = value; }
        }
        [XmlElement("siteloghistory")]
        public int SiteLogHistory
        {
            get { return _SiteLogHistory; }
            set { _SiteLogHistory = value; }
        }
        [XmlElement("email")]
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        [XmlElement("admintabid")]
        public int AdminTabId
        {
            get { return _AdminTabId; }
            set { _AdminTabId = value; }
        }
        [XmlElement("supertabid")]
        public int SuperTabId
        {
            get { return _SuperTabId; }
            set { _SuperTabId = value; }
        }
        [XmlElement("users")]
        public int Users
        {
            get { return _Users; }
            set { _Users = value; }
        }
        [XmlElement("pages")]
        public int Pages
        {
            get
            {
                if (_Pages < 0)
                {
                    TabController objTabController = new TabController();
                    _Pages = objTabController.GetTabCount(PortalID);
                }
                return _Pages;
            }
            set { _Pages = value; }
        }
        [XmlElement("splashtabid")]
        public int SplashTabId
        {
            get { return _SplashTabId; }
            set { _SplashTabId = value; }
        }
        [XmlElement("hometabid")]
        public int HomeTabId
        {
            get { return _HomeTabId; }
            set { _HomeTabId = value; }
        }
        [XmlElement("logintabid")]
        public int LoginTabId
        {
            get { return _LoginTabId; }
            set { _LoginTabId = value; }
        }
        /// <summary>
        /// Tabid of the Registration page
        /// </summary>
        /// <value>TabId of the Registration page</value>
        /// <returns>TabId of the Registration page</returns>
        /// <remarks></remarks>
        [XmlElement("registertabid")]
        public int RegisterTabId
        {
            get { return _RegisterTabId; }
            set { _RegisterTabId = value; }
        }
        [XmlElement("usertabid")]
        public int UserTabId
        {
            get { return _UserTabId; }
            set { _UserTabId = value; }
        }
        [XmlElement("defaultlanguage")]
        public string DefaultLanguage
        {
            get { return _DefaultLanguage; }
            set { _DefaultLanguage = value; }
        }
        [XmlElement("timezoneoffset")]
        public int TimeZoneOffset
        {
            get { return _TimeZoneOffset; }
            set { _TimeZoneOffset = value; }
        }
        [XmlElement("homedirectory")]
        public string HomeDirectory
        {
            get { return _HomeDirectory; }
            set { _HomeDirectory = value; }
        }
        [XmlIgnore()]
        public string HomeDirectoryMapPath
        {
            get
            {
                Services.FileSystem.FolderController objFolderController = new Services.FileSystem.FolderController();
                return objFolderController.GetMappedDirectory(String.Format("{0}/{1}/", Common.Globals.ApplicationPath, HomeDirectory));
            }
        }
        [XmlElement("version")]
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            PortalName = Null.SetNullString(dr["PortalName"]);
            LogoFile = Null.SetNullString(dr["LogoFile"]);
            FooterText = Null.SetNullString(dr["FooterText"]);
            ExpiryDate = Null.SetNullDateTime(dr["ExpiryDate"]);
            UserRegistration = Null.SetNullInteger(dr["UserRegistration"]);
            BannerAdvertising = Null.SetNullInteger(dr["BannerAdvertising"]);
            AdministratorId = Null.SetNullInteger(dr["AdministratorID"]);
            Email = Null.SetNullString(dr["Email"]);
            Currency = Null.SetNullString(dr["Currency"]);
            HostFee = Null.SetNullInteger(dr["HostFee"]);
            HostSpace = Null.SetNullInteger(dr["HostSpace"]);
            PageQuota = Null.SetNullInteger(dr["PageQuota"]);
            UserQuota = Null.SetNullInteger(dr["UserQuota"]);
            AdministratorRoleId = Null.SetNullInteger(dr["AdministratorRoleID"]);
            RegisteredRoleId = Null.SetNullInteger(dr["RegisteredRoleID"]);
            Description = Null.SetNullString(dr["Description"]);
            KeyWords = Null.SetNullString(dr["KeyWords"]);
            BackgroundFile = Null.SetNullString(dr["BackGroundFile"]);
            GUID = new Guid(Null.SetNullString(dr["GUID"]));
            PaymentProcessor = Null.SetNullString(dr["PaymentProcessor"]);
            ProcessorUserId = Null.SetNullString(dr["ProcessorUserId"]);
            ProcessorPassword = Null.SetNullString(dr["ProcessorPassword"]);
            SiteLogHistory = Null.SetNullInteger(dr["SiteLogHistory"]);
            SplashTabId = Null.SetNullInteger(dr["SplashTabID"]);
            HomeTabId = Null.SetNullInteger(dr["HomeTabID"]);
            LoginTabId = Null.SetNullInteger(dr["LoginTabID"]);
            RegisterTabId = Null.SetNullInteger(dr["RegisterTabID"]);
            UserTabId = Null.SetNullInteger(dr["UserTabID"]);
            DefaultLanguage = Null.SetNullString(dr["DefaultLanguage"]);
            TimeZoneOffset = Null.SetNullInteger(dr["TimeZoneOffset"]);
            AdminTabId = Null.SetNullInteger(dr["AdminTabID"]);
            HomeDirectory = Null.SetNullString(dr["HomeDirectory"]);
            SuperTabId = Null.SetNullInteger(dr["SuperTabId"]);
            FillInternal(dr);
            AdministratorRoleName = Null.NullString;
            RegisteredRoleName = Null.NullString;
            Users = UserController.GetUserCountByPortal(PortalID);
            Pages = Null.NullInteger;
        }
        public int KeyID
        {
            get { return PortalID; }
            set { PortalID = value; }
        }
    }
}
