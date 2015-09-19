using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Security.Permissions
{
    internal class CompareTabPermissions : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return ((TabPermissionInfo)x).TabPermissionID.CompareTo(((TabPermissionInfo)y).TabPermissionID);
        }
    }
}
