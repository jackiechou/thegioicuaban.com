using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Security
{
    public enum SecurityAccessLevel : int
    {
        ControlPanel = -3,
        SkinObject = -2,
        Anonymous = -1,
        View = 0,
        Edit = 1,
        Admin = 2,
        Host = 3
    }
}
