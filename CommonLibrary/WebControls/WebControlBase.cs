using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using CommonLibrary.Entities.Portal;
using System.ComponentModel;
using CommonLibrary.Common;
using System.Web.UI.WebControls;

namespace CommonLibrary.WebControls
{
    public abstract class WebControlBase : WebControl
    {
        private string _styleSheetUrl = "";
        private string _theme = "";
        public string Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }
        public string ResourcesFolderUrl
        {
            get { return Globals.ResolveUrl("~/Resources/"); }
        }
        public string StyleSheetUrl
        {
            get
            {
                if ((_styleSheetUrl.StartsWith("~")))
                {
                    return Globals.ResolveUrl(_styleSheetUrl);
                }
                else
                {
                    return _styleSheetUrl;
                }
            }
            set { _styleSheetUrl = value; }
        }
        public bool IsHostMenu
        {
            get
            {
                bool _IsHost = false;
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                {
                    _IsHost = true;
                }
                return _IsHost;
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PortalSettings PortalSettings
        {
            get { return PortalController.GetCurrentPortalSettings(); }
        }
        public abstract string HtmlOutput { get; }
        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write(HtmlOutput);
        }
       
    }
}
