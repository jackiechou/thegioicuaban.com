using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Host;

namespace CommonLibrary.Security.Membership
{
    public class PasswordConfig
    {
        //[SortOrder(0), Category("Password")]
        public static int PasswordExpiry
        {
            get { return Host.PasswordExpiry; }
            set
            {
                Entities.Host.HostSettingsController objHostSettings = new Entities.Host.HostSettingsController();
                objHostSettings.UpdateHostSetting("PasswordExpiry", value.ToString());
            }
        }
        //[SortOrder(1), Category("Password")]
        public static int PasswordExpiryReminder
        {
            get { return Host.PasswordExpiryReminder; }
            set
            {
                Entities.Host.HostSettingsController objHostSettings = new Entities.Host.HostSettingsController();
                objHostSettings.UpdateHostSetting("PasswordExpiryReminder ", value.ToString());
            }
        }
    }
}
