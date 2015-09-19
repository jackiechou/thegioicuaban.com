using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Modules;
using CommonLibrary.Framework;
using System.Web.UI;
using System.IO;
using System.ComponentModel;
using CommonLibrary.Entities.Modules.Actions;
using CommonLibrary.Common;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Portal;
using System.Collections;
using CommonLibrary.UI.Skins;

namespace CommonLibrary.Entities.Modules
{
    public class PortalModuleBase : UserControlBase, IModuleControl
    {
        private string _helpfile;
        private string _localResourceFile;
        private ModuleInstanceContext _moduleContext;
        public Control Control
        {
            get { return this; }
        }
        public string ControlPath
        {
            get { return this.TemplateSourceDirectory + "/"; }
        }
        public string ControlName
        {
            get { return this.GetType().Name.Replace("_", "."); }
        }
        public string LocalResourceFile
        {
            get
            {
                string fileRoot;
                if (string.IsNullOrEmpty(_localResourceFile))
                {
                    fileRoot = Path.Combine(this.ControlPath, Services.Localization.Localization.LocalResourceDirectory + "/" + this.ID);
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set { _localResourceFile = value; }
        }
        public ModuleInstanceContext ModuleContext
        {
            get
            {
                if (_moduleContext == null)
                {
                    _moduleContext = new ModuleInstanceContext(this);
                }
                return _moduleContext;
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModuleActionCollection Actions
        {
            get { return ModuleContext.Actions; }
            set { this.ModuleContext.Actions = value; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control ContainerControl
        {
            get { return Globals.FindControlRecursive(this, "ctr" + ModuleId.ToString()); }
        }
        public bool EditMode
        {
            get { return this.ModuleContext.EditMode; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl()
        {
            return this.ModuleContext.EditUrl();
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string ControlKey)
        {
            return this.ModuleContext.EditUrl(ControlKey);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue)
        {
            return this.ModuleContext.EditUrl(KeyName, KeyValue);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue, string ControlKey)
        {
            return this.ModuleContext.EditUrl(KeyName, KeyValue, ControlKey);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue, string ControlKey, params string[] AdditionalParameters)
        {
            return this.ModuleContext.EditUrl(KeyName, KeyValue, ControlKey, AdditionalParameters);
        }
        public string HelpURL
        {
            get { return this.ModuleContext.HelpURL; }
            set { this.ModuleContext.HelpURL = value; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditable
        {
            get { return this.ModuleContext.IsEditable; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModuleInfo ModuleConfiguration
        {
            get { return this.ModuleContext.Configuration; }
            set { this.ModuleContext.Configuration = value; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PortalId
        {
            get { return this.ModuleContext.PortalId; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TabId
        {
            get { return this.ModuleContext.TabId; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TabModuleId
        {
            get { return this.ModuleContext.TabModuleId; }
            set { this.ModuleContext.TabModuleId = value; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ModuleId
        {
            get { return this.ModuleContext.ModuleId; }
            set { this.ModuleContext.ModuleId = value; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UserInfo UserInfo
        {
            get { return PortalSettings.UserInfo; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int UserId
        {
            get { return PortalSettings.UserId; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PortalAliasInfo PortalAlias
        {
            get { return PortalSettings.PortalAlias; }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Hashtable Settings
        {
            get { return this.ModuleContext.Settings; }
        }
        protected void AddActionHandler(ActionEventHandler e)
        {
            Skin ParentSkin = Skin.GetParentSkin(this);
            if (ParentSkin != null)
            {
                ParentSkin.RegisterModuleActionEvent(this.ModuleId, e);
            }
        }
        public int GetNextActionID()
        {
            return this.ModuleContext.GetNextActionID();
        }
    }
}
