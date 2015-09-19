using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.ComponentModel;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Entities.Portal;

namespace CommonLibrary.Services.Url.FriendlyUrl
{
    public abstract class FriendlyUrlProvider
    {
        public static FriendlyUrlProvider Instance()
        {
            return ComponentFactory.GetComponent<FriendlyUrlProvider>();
        }
        public abstract string FriendlyUrl(TabInfo tab, string path);
        public abstract string FriendlyUrl(TabInfo tab, string path, string pageName);
        public abstract string FriendlyUrl(TabInfo tab, string path, string pageName, PortalSettings settings);
        public abstract string FriendlyUrl(TabInfo tab, string path, string pageName, string portalAlias);
    }
}
