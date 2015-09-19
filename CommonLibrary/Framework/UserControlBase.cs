using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using CommonLibrary.Entities.Portal;

namespace CommonLibrary.Framework
{
    public class UserControlBase : UserControl
    {
        public bool IsHostMenu
        {
            get
            {
                bool _IsHost = false;
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId || PortalSettings.ActiveTab.TabID == PortalSettings.SuperTabId)
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
    }
}
