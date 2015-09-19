using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Security.Permissions
{
    internal class CompareModulePermissions : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return ((ModulePermissionInfo)x).ModulePermissionID.CompareTo(((ModulePermissionInfo)y).ModulePermissionID);
        }
    }
}
