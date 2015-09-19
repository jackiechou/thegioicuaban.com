using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using CommonLibrary.Entities.Modules;
using CommonLibrary.UI.Skins;
using System.Threading;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common;
using System.Web.UI.HtmlControls;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security;
using System.IO;
using CommonLibrary.Security.Permissions;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Framework;
using CommonLibrary.Services.ModuleCache;
using System.Web;

namespace CommonLibrary.UI.Modules
{
    public class ModuleHost : Panel
    {
        private Control _Control;
        private bool _IsCached;
        private ModuleInfo _ModuleConfiguration;
        private Skin _Skin;
        private static ReaderWriterLock objReaderWriterLock = new ReaderWriterLock();
        private int ReaderTimeOut = 10;
        private int WriterTimeOut = 100;
        public ModuleHost(ModuleInfo moduleConfiguration, Skin skin)
        {
            _ModuleConfiguration = moduleConfiguration;
            _Skin = skin;
            ID = "ModuleContent";
        }
        public IModuleControl ModuleControl
        {
            get
            {
                EnsureChildControls();
                return _Control as IModuleControl;
            }
        }
        public PortalSettings PortalSettings
        {
            get { return PortalController.GetCurrentPortalSettings(); }
        }
        private void InjectModuleContent(Control content)
        {
            if (_ModuleConfiguration.IsWebSlice && !Globals.IsAdminControl())
            {
                this.CssClass = "hslice";
                Label titleLabel = new Label();
                titleLabel.CssClass = "entry-title Hidden";
                if (!string.IsNullOrEmpty(_ModuleConfiguration.WebSliceTitle))
                {
                    titleLabel.Text = _ModuleConfiguration.WebSliceTitle;
                }
                else
                {
                    titleLabel.Text = _ModuleConfiguration.ModuleTitle;
                }
                this.Controls.Add(titleLabel);
                Panel websliceContainer = new Panel();
                websliceContainer.CssClass = "entry-content";
                websliceContainer.Controls.Add(content);
                HtmlGenericControl expiry = new HtmlGenericControl();
                expiry.TagName = "abbr";
                expiry.Attributes["class"] = "endtime";
                if (!Null.IsNull(_ModuleConfiguration.WebSliceExpiryDate))
                {
                    expiry.Attributes["title"] = _ModuleConfiguration.WebSliceExpiryDate.ToString("o");
                    websliceContainer.Controls.Add(expiry);
                }
                else if (_ModuleConfiguration.EndDate < System.DateTime.MaxValue)
                {
                    expiry.Attributes["title"] = _ModuleConfiguration.EndDate.ToString("o");
                    websliceContainer.Controls.Add(expiry);
                }
                HtmlGenericControl ttl = new HtmlGenericControl();
                ttl.TagName = "abbr";
                ttl.Attributes["class"] = "ttl";
                if (_ModuleConfiguration.WebSliceTTL > 0)
                {
                    ttl.Attributes["title"] = _ModuleConfiguration.WebSliceTTL.ToString();
                    websliceContainer.Controls.Add(ttl);
                }
                else if (_ModuleConfiguration.CacheTime > 0)
                {
                    ttl.Attributes["title"] = (_ModuleConfiguration.CacheTime / 60).ToString();
                    websliceContainer.Controls.Add(ttl);
                }
                this.Controls.Add(websliceContainer);
            }
            else
            {
                this.Controls.Add(content);
            }
        }
        private bool DisplayContent()
        {
            bool blnContent = PortalSettings.UserMode != PortalSettings.Mode.Layout;
            if (Page.Request.QueryString["content"] != null)
            {
                switch (Page.Request.QueryString["Content"].ToLower())
                {
                    case "1":
                    case "true":
                        blnContent = true;
                        break;
                    case "0":
                    case "false":
                        blnContent = false;
                        break;
                }
            }
            if (Globals.IsAdminControl() == true)
            {
                blnContent = true;
            }
            return blnContent;
        }

        private void InjectMessageControl(Control container)
        {
            PlaceHolder MessagePlaceholder = new PlaceHolder();
            MessagePlaceholder.ID = "MessagePlaceHolder";
            MessagePlaceholder.Visible = false;
            container.Controls.Add(MessagePlaceholder);
        }
        private bool IsViewMode()
        {
            return !(ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, Null.NullString, _ModuleConfiguration)) || PortalSettings.UserMode == PortalSettings.Mode.View;
        }
        private void LoadModuleControl()
        {
            try
            {
                if (DisplayContent())
                {
                    if (SupportsCaching() && IsViewMode())
                    {
                        _IsCached = TryLoadCached();
                    }
                    if (!_IsCached)
                    {
                        _Control = ControlUtilities.LoadControl<Control>(this.Page, _ModuleConfiguration.ModuleControl.ControlSrc);
                        _Control.ID = Path.GetFileNameWithoutExtension(_ModuleConfiguration.ModuleControl.ControlSrc);
                    }
                }
                else
                {
                    _Control = new ModuleControlBase();
                }
                _Skin.Communicator.LoadCommunicator(_Control);
                ModuleControl.ModuleContext.Configuration = _ModuleConfiguration;
            }
            catch (System.Threading.ThreadAbortException exc)
            {
                System.Threading.Thread.ResetAbort();
                exc.ToString();
            }
            catch (Exception exc)
            {
                _Control = new ModuleControlBase();
                ModuleControl.ModuleContext.Configuration = _ModuleConfiguration;
                if (TabPermissionController.CanAdminPage())
                {
                    Exceptions.ProcessModuleLoadException(_Control, exc);
                }
            }
        }
        private void LoadUpdatePanel()
        {
            AJAX.RegisterScriptManager();
            ScriptManager scriptManager = AJAX.GetScriptManager(this.Page);
            if (scriptManager != null)
            {
                scriptManager.EnablePartialRendering = true;
            }
            UpdatePanel updatePanel = new UpdatePanel();
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;
            updatePanel.ID = _Control.ID + "_UP";
            Control objContentTemplateContainer = updatePanel.ContentTemplateContainer;
            InjectMessageControl(objContentTemplateContainer);
            objContentTemplateContainer.Controls.Add(_Control);
            InjectModuleContent(updatePanel);
            System.Web.UI.WebControls.Image objImage = new System.Web.UI.WebControls.Image();
            objImage.ImageUrl = "~/images/progressbar.gif";
            objImage.AlternateText = "ProgressBar";

            UpdateProgress updateProgress = new UpdateProgress();
            updateProgress.AssociatedUpdatePanelID = updatePanel.ID;
            updateProgress.ID = updatePanel.ID + "_Prog";
            updateProgress.ProgressTemplate = new UI.WebControls.LiteralTemplate(objImage);
            this.Controls.Add(updateProgress);
        }
        private bool SupportsCaching()
        {
            return _ModuleConfiguration.CacheTime > 0;
        }
        private bool TryLoadCached()
        {
            bool bSuccess = false;
            string _cachedContent = string.Empty;
            try
            {
                ModuleCachingProvider cache = ModuleCachingProvider.Instance(_ModuleConfiguration.GetEffectiveCacheMethod());
                System.Collections.Generic.SortedDictionary<string, string> varyBy = new System.Collections.Generic.SortedDictionary<string, string>();
                varyBy.Add("locale", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                string cacheKey = cache.GenerateCacheKey(_ModuleConfiguration.TabModuleID, varyBy);
                byte[] cachedBytes =ModuleCachingProvider.Instance(_ModuleConfiguration.GetEffectiveCacheMethod()).GetModule(_ModuleConfiguration.TabModuleID, cacheKey);
                if (cachedBytes != null && cachedBytes.Length > 0)
                {
                    _cachedContent = System.Text.Encoding.UTF8.GetString(cachedBytes);
                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
                _cachedContent = string.Empty;
                bSuccess = false;
                ex.ToString();
            }
            if (bSuccess)
            {
                _Control = new CachedModuleControl(_cachedContent);
                this.Controls.Add(_Control);
            }
            return bSuccess;
        }
        protected override void CreateChildControls()
        {
            Controls.Clear();
            LoadModuleControl();
            if (ModuleControl != null)
            {
                if (!_IsCached && _ModuleConfiguration.ModuleControl.SupportsPartialRendering && AJAX.IsInstalled())
                {
                    LoadUpdatePanel();
                }
                else
                {
                    InjectMessageControl(this);
                    InjectModuleContent(_Control);
                }
            }
        }
        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);
            string strModuleName = this.ModuleControl.ModuleContext.Configuration.DesktopModule.ModuleName;
            if (strModuleName != null)
            {
                strModuleName = strModuleName.Replace(".", "_");
            }
            this.Attributes.Add("class", strModuleName + "Content");
        }
        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (_IsCached)
            {
                base.RenderContents(writer);
            }
            else
            {
                if (SupportsCaching() && IsViewMode() && !Common.Globals.IsAdminControl())
                {
                    string _cachedOutput = Null.NullString;
                    StringWriter tempWriter = new StringWriter();
                    _Control.RenderControl(new HtmlTextWriter(tempWriter));
                    _cachedOutput = tempWriter.ToString();
                    if (!string.IsNullOrEmpty(_cachedOutput) && (!HttpContext.Current.Request.Browser.Crawler))
                    {
                        byte[] moduleContent = System.Text.Encoding.UTF8.GetBytes(_cachedOutput);
                        ModuleCachingProvider cache = ModuleCachingProvider.Instance(_ModuleConfiguration.GetEffectiveCacheMethod());
                        System.Collections.Generic.SortedDictionary<string, string> varyBy = new System.Collections.Generic.SortedDictionary<string, string>();
                        varyBy.Add("locale", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                        string cacheKey = cache.GenerateCacheKey(_ModuleConfiguration.TabModuleID, varyBy);
                        cache.SetModule(_ModuleConfiguration.TabModuleID, cacheKey, new TimeSpan(0, 0, _ModuleConfiguration.CacheTime), moduleContent);
                    }
                    writer.Write(_cachedOutput);
                }
                else
                {
                    base.RenderContents(writer);
                }
            }
        }
    }
}
