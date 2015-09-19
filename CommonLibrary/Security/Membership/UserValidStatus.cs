using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Security.Membership
{
    public enum UserValidStatus
    {
        VALID = 0,
        PASSWORDEXPIRED = 1,
        PASSWORDEXPIRING = 2,
        UPDATEPROFILE = 3,
        UPDATEPASSWORD = 4
    }
}
