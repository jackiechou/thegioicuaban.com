using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CommonLibrary.Security.Profile
{
    public class ProfileProviderConfig
    {
        private static ProfileProvider profileProvider = ProfileProvider.Instance();
        [Browsable(false)]
        public static bool CanEditProviderProperties
        {
            get { return profileProvider.CanEditProviderProperties; }
        }
    }
}
