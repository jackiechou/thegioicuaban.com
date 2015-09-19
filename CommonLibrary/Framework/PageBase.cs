using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Web;
using CommonLibrary.Common;
using System.Reflection;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using CommonLibrary.UI.Modules;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using CommonLibrary.Services.Localization;
using System.Globalization;
using System.Collections.Specialized;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Entities.Host;


namespace CommonLibrary.Framework
{
    public abstract class PageBase : System.Web.UI.Page
    {
        private ArrayList _localizedControls;
        private string _localResourceFile;
        private CultureInfo _PageCulture;
        private NameValueCollection _htmlAttributes = new NameValueCollection();
        public PageBase()
        {
            _localizedControls = new ArrayList();
        }
        protected override System.Web.UI.PageStatePersister PageStatePersister
        {
            get
            {
                PageStatePersister _persister = base.PageStatePersister;
                switch (Host.PageStatePersister)
                {
                    case "M":
                        _persister = new CachePageStatePersister(this);
                        break;
                    case "D":
                        _persister = new DiskPageStatePersister(this);
                        break;
                }
                return _persister;
            }
        }
        public PortalSettings PortalSettings
        {
            get { return PortalController.GetCurrentPortalSettings(); }
        }
        public NameValueCollection HtmlAttributes
        {
            get { return _htmlAttributes; }
        }
        public CultureInfo PageCulture
        {
            get
            {
                if (_PageCulture == null)
                {
                    _PageCulture = Localization.GetPageLocale(PortalSettings);
                }
                return _PageCulture;
            }
        }
        public string LocalResourceFile
        {
            get
            {
                string fileRoot;
                string[] page = Request.ServerVariables["SCRIPT_NAME"].Split('/');
                if (String.IsNullOrEmpty(_localResourceFile))
                {
                    fileRoot = this.TemplateSourceDirectory + "/" + Services.Localization.Localization.LocalResourceDirectory + "/" + page[page.GetUpperBound(0)] + ".resx";
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set { _localResourceFile = value; }
        }
        internal void ProcessControl(Control c, ArrayList affectedControls, bool includeChildren, string ResourceFileRoot)
        {
            string key = GetControlAttribute(c, affectedControls);
            if (!string.IsNullOrEmpty(key))
            {
                string value;
                value = Services.Localization.Localization.GetString(key, ResourceFileRoot);
                if (c is Label)
                {
                    Label ctrl;
                    ctrl = (Label)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is LinkButton)
                {
                    LinkButton ctrl;
                    ctrl = (LinkButton)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        MatchCollection imgMatches = Regex.Matches(value, "<(a|link|img|script|input|form).[^>]*(href|src|action)=(\\\"|'|)(.[^\\\"']*)(\\\"|'|)[^>]*>", RegexOptions.IgnoreCase);
                        foreach (Match _match in imgMatches)
                        {
                            if ((_match.Groups[_match.Groups.Count - 2].Value.IndexOf("~") != -1))
                            {
                                string resolvedUrl = Page.ResolveUrl(_match.Groups[_match.Groups.Count - 2].Value);
                                value = value.Replace(_match.Groups[_match.Groups.Count - 2].Value, resolvedUrl);
                            }
                        }
                        ctrl.Text = value;
                        if (string.IsNullOrEmpty(ctrl.ToolTip))
                        {
                            ctrl.ToolTip = value;
                        }
                    }
                }
                if (c is HyperLink)
                {
                    HyperLink ctrl;
                    ctrl = (HyperLink)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is ImageButton)
                {
                    ImageButton ctrl;
                    ctrl = (ImageButton)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.AlternateText = value;
                    }
                }
                if (c is Button)
                {
                    Button ctrl;
                    ctrl = (Button)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is System.Web.UI.HtmlControls.HtmlImage)
                {
                    System.Web.UI.HtmlControls.HtmlImage ctrl;
                    ctrl = (System.Web.UI.HtmlControls.HtmlImage)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Alt = value;
                    }
                }
                if (c is CheckBox)
                {
                    CheckBox ctrl;
                    ctrl = (CheckBox)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.Text = value;
                    }
                }
                if (c is BaseValidator)
                {
                    BaseValidator ctrl;
                    ctrl = (BaseValidator)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.ErrorMessage = value;
                    }
                }
                if (c is System.Web.UI.WebControls.Image)
                {
                    System.Web.UI.WebControls.Image ctrl;
                    ctrl = (System.Web.UI.WebControls.Image)c;
                    if (!String.IsNullOrEmpty(value))
                    {
                        ctrl.AlternateText = value;
                        ctrl.ToolTip = value;
                    }
                }
            }
            if (c is RadioButtonList)
            {
                RadioButtonList ctrl;
                ctrl = (RadioButtonList)c;
                int i;
                for (i = 0; i <= ctrl.Items.Count - 1; i++)
                {
                    System.Web.UI.AttributeCollection ac = null;
                    ac = ctrl.Items[i].Attributes;
                    key = ac[Services.Localization.Localization.KeyName];
                    if (key != null)
                    {
                        string value = Services.Localization.Localization.GetString(key, ResourceFileRoot);
                        if (!String.IsNullOrEmpty(value))
                        {
                            ctrl.Items[i].Text = value;
                        }
                    }
                    if (key != null && affectedControls != null)
                    {
                        affectedControls.Add(ac);
                    }
                }
            }
            if (c is DropDownList)
            {
                DropDownList ctrl;
                ctrl = (DropDownList)c;
                int i;
                for (i = 0; i <= ctrl.Items.Count - 1; i++)
                {
                    System.Web.UI.AttributeCollection ac = null;
                    ac = ctrl.Items[i].Attributes;
                    key = ac[Services.Localization.Localization.KeyName];
                    if (key != null)
                    {
                        string value = Services.Localization.Localization.GetString(key, ResourceFileRoot);
                        if (!String.IsNullOrEmpty(value))
                        {
                            ctrl.Items[i].Text = value;
                        }
                    }
                    if (key != null && affectedControls != null)
                    {
                        affectedControls.Add(ac);
                    }
                }
            }
            if (c is System.Web.UI.WebControls.Image)
            {
                System.Web.UI.WebControls.Image ctrl;
                ctrl = (System.Web.UI.WebControls.Image)c;
                if ((ctrl.ImageUrl.IndexOf("~") != -1))
                {
                    ctrl.ImageUrl = Page.ResolveUrl(ctrl.ImageUrl);
                }
            }
            if (c is System.Web.UI.HtmlControls.HtmlImage)
            {
                System.Web.UI.HtmlControls.HtmlImage ctrl;
                ctrl = (System.Web.UI.HtmlControls.HtmlImage)c;
                if ((ctrl.Src.IndexOf("~") != -1))
                {
                    ctrl.Src = Page.ResolveUrl(ctrl.Src);
                }
            }
            if (c is System.Web.UI.WebControls.HyperLink)
            {
                System.Web.UI.WebControls.HyperLink ctrl;
                ctrl = (System.Web.UI.WebControls.HyperLink)c;
                if ((ctrl.NavigateUrl.IndexOf("~") != -1))
                {
                    ctrl.NavigateUrl = Page.ResolveUrl(ctrl.NavigateUrl);
                }
                if ((ctrl.ImageUrl.IndexOf("~") != -1))
                {
                    ctrl.ImageUrl = Page.ResolveUrl(ctrl.ImageUrl);
                }
            }
            if (includeChildren == true && c.HasControls())
            {
                IModuleControl objModuleControl = c as IModuleControl;
                if (objModuleControl == null)
                {
                    PropertyInfo pi = c.GetType().GetProperty("LocalResourceFile");
                    if (pi != null && pi.GetValue(c, null) != null)
                    {
                        IterateControls(c.Controls, affectedControls, pi.GetValue(c, null).ToString());
                    }
                    else
                    {
                        IterateControls(c.Controls, affectedControls, ResourceFileRoot);
                    }
                }
                else
                {
                    IterateControls(c.Controls, affectedControls, objModuleControl.LocalResourceFile);
                }
            }
        }
        static internal string GetControlAttribute(Control c, ArrayList affectedControls)
        {
            System.Web.UI.AttributeCollection ac = null;
            string key = null;
            if (c is LiteralControl)
            {
                key = null;
                ac = null;
            }
            else
            {
                if (c is WebControl)
                {
                    WebControl w = (WebControl)c;
                    ac = w.Attributes;
                    key = ac[Services.Localization.Localization.KeyName];
                }
                else
                {
                    if (c is HtmlControl)
                    {
                        HtmlControl h = (HtmlControl)c;
                        ac = h.Attributes;
                        key = ac[Services.Localization.Localization.KeyName];
                    }
                    else
                    {
                        if (c is UserControl)
                        {
                            UserControl u = (UserControl)c;
                            ac = u.Attributes;
                            key = ac[Services.Localization.Localization.KeyName];
                        }
                        else
                        {
                            Type controlType = c.GetType();
                            PropertyInfo attributeProperty = controlType.GetProperty("Attributes", typeof(System.Web.UI.AttributeCollection));
                            if (attributeProperty != null)
                            {
                                ac = (System.Web.UI.AttributeCollection)attributeProperty.GetValue(c, null);
                                key = ac[Services.Localization.Localization.KeyName];
                            }
                        }
                    }
                }
            }
            if (key != null && affectedControls != null)
            {
                affectedControls.Add(ac);
            }
            return key;
        }
        public static void RemoveKeyAttribute(ArrayList affectedControls)
        {
            if (affectedControls == null)
            {
                return;
            }
            int i;
            for (i = 0; i <= affectedControls.Count - 1; i++)
            {
                System.Web.UI.AttributeCollection ac = (System.Web.UI.AttributeCollection)affectedControls[i];
                ac.Remove(Services.Localization.Localization.KeyName);
            }
        }
        protected override void OnError(System.EventArgs e)
        {
            base.OnError(e);
            Exception exc = Server.GetLastError();
            string strURL = Globals.ApplicationURL();
            if (object.ReferenceEquals(exc.GetType(), typeof(HttpException)))
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Server.Transfer("~/ErrorPage.aspx");
            }
            BasePortalException objBasePortalException = new BasePortalException(exc.ToString(), exc);
            if (Request.QueryString["error"] != null)
            {
                strURL += (strURL.IndexOf("?") == -1 ? "?" : "&").ToString() + "error=terminate";
            }
            else
            {
                strURL += (strURL.IndexOf("?") == -1 ? "?" : "&").ToString() + "error=" + Server.UrlEncode(exc.Message);
                if (!Globals.IsAdminControl())
                {
                    strURL += "&content=0";
                }
            }
            Exceptions.ProcessPageLoadException(exc, strURL);
        }
        protected override void OnInit(System.EventArgs e)
        {
            if (HttpContext.Current.Request != null && !HttpContext.Current.Request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx"))
            {
                Thread.CurrentThread.CurrentUICulture = PageCulture;
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
            AJAX.AddScriptManager(this);
            Page.ClientScript.RegisterClientScriptInclude("dnncore", ResolveUrl("~/js/dnncore.js"));
            base.OnInit(e);
        }
        protected override void OnPreRender(EventArgs evt)
        {
            base.OnPreRender(evt);
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            IterateControls(Controls, _localizedControls, LocalResourceFile);
            RemoveKeyAttribute(_localizedControls);
            AJAX.RemoveScriptManager(this);
            base.Render(writer);
        }
        private void IterateControls(ArrayList affectedControls)
        {
            IterateControls(Controls, affectedControls, null);
        }
        private void IterateControls(ControlCollection controls, ArrayList affectedControls, string ResourceFileRoot)
        {
            foreach (Control c in controls)
            {
                ProcessControl(c, affectedControls, true, ResourceFileRoot);
                //if (c.Controls.Count > 0)
                //{
                //    IterateControls(c.Controls, affectedControls, ResourceFileRoot);
                //}
            }
        }
    }
}
