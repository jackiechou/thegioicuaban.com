using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Portal;
using System.Web.UI;

namespace CommonLibrary.Services.Exceptions
{
    public class ErrorContainer : Control
    {
        private UI.Skins.Controls.ModuleMessage _Container;
        public UI.Skins.Controls.ModuleMessage Container
        {
            get { return _Container; }
            set { _Container = value; }
        }
        public ErrorContainer(string strError)
        {
            Container = FormatException(strError);
        }
        public ErrorContainer(string strError, Exception exc)
        {
            Container = FormatException(strError, exc);
        }
        public ErrorContainer(PortalSettings _PortalSettings, string strError, Exception exc)
        {
            UserInfo objUserInfo = UserController.GetCurrentUserInfo();
            if (objUserInfo.IsSuperUser)
            {
                Container = FormatException(strError, exc);
            }
            else
            {
                Container = FormatException(strError);
            }
        }
        private UI.Skins.Controls.ModuleMessage FormatException(string strError)
        {
            UI.Skins.Controls.ModuleMessage m;
            m = UI.Skins.Skin.GetModuleMessageControl(Localization.Localization.GetString("ErrorOccurred"), strError, CommonLibrary.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            return m;
        }
        private UI.Skins.Controls.ModuleMessage FormatException(string strError, Exception exc)
        {
            UI.Skins.Controls.ModuleMessage m;
            if (exc != null)
            {
                m = UI.Skins.Skin.GetModuleMessageControl(strError, exc.ToString(), CommonLibrary.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
            else
            {
                m = UI.Skins.Skin.GetModuleMessageControl(Localization.Localization.GetString("ErrorOccurred"), strError, CommonLibrary.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
            return m;
        }
    }
}
