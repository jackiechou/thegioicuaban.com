using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.ComponentModel;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Security.Profile
{
    public abstract class ProfileProvider
    {
        public static new ProfileProvider Instance()
        {
            return ComponentFactory.GetComponent<ProfileProvider>();
        }
        public abstract bool CanEditProviderProperties { get; }
        public abstract void GetUserProfile(ref UserInfo user);
        public abstract void UpdateUserProfile(UserInfo user);
    }
}
