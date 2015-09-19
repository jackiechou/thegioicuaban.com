using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Security.Permissions
{
    internal class CompareFolderPermissions : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return ((FolderPermissionInfo)x).FolderPermissionID.CompareTo(((FolderPermissionInfo)y).FolderPermissionID);
        }
    }
}
