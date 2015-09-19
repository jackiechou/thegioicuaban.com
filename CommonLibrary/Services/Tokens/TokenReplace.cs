using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Tokens.PropertyAccess;
using System.Web;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;
using System.Collections;
using System.Data;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Host;

namespace CommonLibrary.Services.Tokens
{
    public class TokenReplace : BaseCustomTokenReplace
    {
        private PortalSettings _PortalSettings;
        private Dictionary<string, string> _Hostsettings;
        private Entities.Modules.ModuleInfo _ModuleInfo;
        private Entities.Users.UserInfo _User;
        private Entities.Tabs.TabInfo _Tab;
        private int _ModuleId = int.MinValue;
        private Dictionary<string, string> HostSettings
        {
            get
            {
                if (_Hostsettings == null)
                {
                    _Hostsettings = Host.GetSecureHostSettingsDictionary();
                }
                return _Hostsettings;
            }
        }
        public int ModuleId
        {
            get { return _ModuleId; }
            set { _ModuleId = value; }
        }
        public Entities.Modules.ModuleInfo ModuleInfo
        {
            get
            {
                if (ModuleId > int.MinValue && (_ModuleInfo == null || _ModuleInfo.ModuleID != ModuleId))
                {
                    ModuleController mc = new ModuleController();
                    if (PortalSettings != null && PortalSettings.ActiveTab != null)
                    {
                        _ModuleInfo = mc.GetModule(ModuleId, PortalSettings.ActiveTab.TabID, false);
                    }
                    else
                    {
                        _ModuleInfo = mc.GetModule(ModuleId);
                    }
                }
                return _ModuleInfo;
            }
            set { _ModuleInfo = value; }
        }
        public PortalSettings PortalSettings
        {
            get { return _PortalSettings; }
            set { _PortalSettings = value; }
        }
        public Entities.Users.UserInfo User
        {
            get { return _User; }
            set { _User = value; }
        }
        public TokenReplace() :
            this(Scope.DefaultSettings, null, null, null, Null.NullInteger)
        {

        }
        public TokenReplace(int ModuleID) :
            this(Scope.DefaultSettings, null, null, null, ModuleID)
        {

        }
        public TokenReplace(Scope AccessLevel) :
            this(AccessLevel, null, null, null, Null.NullInteger)
        {

        }
        public TokenReplace(Scope AccessLevel, int ModuleID) :
            this(AccessLevel, null, null, null, ModuleID)
        {

        }
        public TokenReplace(Scope AccessLevel, string Language, PortalSettings PortalSettings, UserInfo User) :
            this(AccessLevel, Language, PortalSettings, User, Null.NullInteger)
        {

        }
        public TokenReplace(Scope AccessLevel, string Language, PortalSettings PortalSettings, UserInfo User, int ModuleID)
        {
            this.CurrentAccessLevel = AccessLevel;
            if (AccessLevel != Scope.NoSettings)
            {
                if (PortalSettings == null)
                {
                    if (HttpContext.Current != null)
                        this.PortalSettings = PortalController.GetCurrentPortalSettings();
                }
                else
                {
                    this.PortalSettings = PortalSettings;
                }
                if (User == null)
                {
                    if (HttpContext.Current != null)
                    {
                        this.User = (UserInfo)HttpContext.Current.Items["UserInfo"];
                    }
                    else
                    {
                        this.User = new UserInfo();
                    }
                    this.AccessingUser = this.User;
                }
                else
                {
                    this.User = User;
                    if (HttpContext.Current != null)
                    {
                        this.AccessingUser = (UserInfo)HttpContext.Current.Items["UserInfo"];
                    }
                    else
                    {
                        this.AccessingUser = new UserInfo();
                    }
                }
                if (string.IsNullOrEmpty(Language))
                {
                    this.Language = new Localization.Localization().CurrentCulture;
                }
                else
                {
                    this.Language = Language;
                }
                if (ModuleID != Null.NullInteger)
                {
                    this.ModuleId = ModuleID;
                }
            }
            PropertySource["date"] = new DateTimePropertyAccess();
            PropertySource["datetime"] = new DateTimePropertyAccess();
            PropertySource["ticks"] = new TicksPropertyAccess();
            PropertySource["culture"] = new CulturePropertyAccess();
        }
        public string ReplaceEnvironmentTokens(string strSourceText)
        {
            return ReplaceTokens(strSourceText);
        }
        public string ReplaceEnvironmentTokens(string strSourceText, DataRow row)
        {
            DataRowPropertyAccess rowProperties = new DataRowPropertyAccess(row);
            PropertySource["field"] = rowProperties;
            PropertySource["row"] = rowProperties;
            return ReplaceTokens(strSourceText);
        }
        public string ReplaceEnvironmentTokens(string strSourceText, ArrayList Custom, string CustomCaption)
        {
            PropertySource[CustomCaption.ToLower()] = new ArrayListPropertyAccess(Custom);
            return ReplaceTokens(strSourceText);
        }
        public string ReplaceEnvironmentTokens(string strSourceText, IDictionary Custom, string CustomCaption)
        {
            PropertySource[CustomCaption.ToLower()] = new DictionaryPropertyAccess(Custom);
            return ReplaceTokens(strSourceText);
        }
        public string ReplaceEnvironmentTokens(string strSourceText, ArrayList Custom, string CustomCaption, System.Data.DataRow Row)
        {
            DataRowPropertyAccess rowProperties = new DataRowPropertyAccess(Row);
            PropertySource["field"] = rowProperties;
            PropertySource["row"] = rowProperties;
            PropertySource[CustomCaption.ToLower()] = new ArrayListPropertyAccess(Custom);
            return ReplaceTokens(strSourceText);
        }
        protected override string ReplaceTokens(string strSourceText)
        {
            InitializePropertySources();
            return base.ReplaceTokens(strSourceText);
        }
        private void InitializePropertySources()
        {
            IPropertyAccess DefaultPropertyAccess = new EmptyPropertyAccess();
            PropertySource["portal"] = DefaultPropertyAccess;
            PropertySource["tab"] = DefaultPropertyAccess;
            PropertySource["host"] = DefaultPropertyAccess;
            PropertySource["module"] = DefaultPropertyAccess;
            PropertySource["user"] = DefaultPropertyAccess;
            PropertySource["membership"] = DefaultPropertyAccess;
            PropertySource["profile"] = DefaultPropertyAccess;
            //if (CurrentAccessLevel >= Scope.Configuration)
            //{
            //    if (PortalSettings != null)
            //    {
            //        PropertySource["portal"] = PortalSettings;
            //        PropertySource["tab"] = PortalSettings.ActiveTab;
            //    }
            //    PropertySource["host"] = new HostPropertyAccess();
            //    if (ModuleInfo != null)
            //    {
            //        PropertySource["module"] = ModuleInfo;
            //    }
            //}
            //if (CurrentAccessLevel >= Scope.DefaultSettings && !(User == null || User.UserID == -1))
            //{
            //    PropertySource["user"] = User;
            //    PropertySource["membership"] = new MembershipPropertyAccess(User);
            //    PropertySource["profile"] = new ProfilePropertyAccess(User);
            //}
        }
    }
}
