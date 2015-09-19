using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CommonLibrary.UI.Modules;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Security.Permissions;

namespace CommonLibrary.UI.Skins
{
    public class SkinObjectBase : System.Web.UI.UserControl, ISkinControl
    {
        private IModuleControl _ModuleControl;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PortalSettings PortalSettings
        {
            get { return PortalController.GetCurrentPortalSettings(); }
        }
        public bool AdminMode
        {
            get { return TabPermissionController.CanAdminPage(); }
        }
        public Modules.IModuleControl ModuleControl
        {
            get { return _ModuleControl; }
            set { _ModuleControl = value; }
        }
    }
}
