using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Security.Permissions;
using System.Collections;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class DesktopModulePermissionCollection : CollectionBase
    {
        public DesktopModulePermissionCollection()
            : base()
        {
        }
        public DesktopModulePermissionCollection(ArrayList DesktopModulePermissions)
        {
            AddRange(DesktopModulePermissions);
        }
        public DesktopModulePermissionCollection(DesktopModulePermissionCollection DesktopModulePermissions)
        {
            AddRange(DesktopModulePermissions);
        }
        public DesktopModulePermissionCollection(ArrayList DesktopModulePermissions, int DesktopModulePermissionID)
        {
            foreach (DesktopModulePermissionInfo permission in DesktopModulePermissions)
            {
                if (permission.DesktopModulePermissionID == DesktopModulePermissionID)
                {
                    Add(permission);
                }
            }
        }
        public DesktopModulePermissionInfo this[int index]
        {
            get { return (DesktopModulePermissionInfo)List[index]; }
            set { List[index] = value; }
        }
        public int Add(DesktopModulePermissionInfo value)
        {
            return List.Add(value);
        }
        public int Add(DesktopModulePermissionInfo value, bool checkForDuplicates)
        {
            if (!checkForDuplicates)
            {
                return Add(value);
            }
            else
            {
                bool isMatch = false;
                foreach (PermissionInfoBase permission in this.List)
                {
                    if (permission.PermissionID == value.PermissionID && permission.UserID == value.UserID && permission.RoleID == value.RoleID)
                    {
                        isMatch = true;
                        break;
                    }
                }
                if (!isMatch)
                {
                    return Add(value);
                }
                else
                    return 0;
            }
        }
        public void AddRange(ArrayList DesktopModulePermissions)
        {
            foreach (DesktopModulePermissionInfo permission in DesktopModulePermissions)
            {
                Add(permission);
            }
        }
        public void AddRange(DesktopModulePermissionCollection DesktopModulePermissions)
        {
            foreach (DesktopModulePermissionInfo permission in DesktopModulePermissions)
            {
                Add(permission);
            }
        }
        public bool CompareTo(DesktopModulePermissionCollection objDesktopModulePermissionCollection)
        {
            if (objDesktopModulePermissionCollection.Count != this.Count)
            {
                return false;
            }
            InnerList.Sort(new CompareDesktopModulePermissions());
            objDesktopModulePermissionCollection.InnerList.Sort(new CompareDesktopModulePermissions());
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (objDesktopModulePermissionCollection[i].DesktopModulePermissionID != this[i].DesktopModulePermissionID || objDesktopModulePermissionCollection[i].AllowAccess != this[i].AllowAccess)
                {
                    return false;
                }
            }
            return true;
        }
        public bool Contains(DesktopModulePermissionInfo value)
        {
            return List.Contains(value);
        }
        public int IndexOf(DesktopModulePermissionInfo value)
        {
            return List.IndexOf(value);
        }
        public void Insert(int index, DesktopModulePermissionInfo value)
        {
            List.Insert(index, value);
        }
        public void Remove(DesktopModulePermissionInfo value)
        {
            List.Remove(value);
        }
        public void Remove(int permissionID, int roleID, int userID)
        {
            foreach (PermissionInfoBase permission in this.List)
            {
                if (permission.PermissionID == permissionID && permission.UserID == userID && permission.RoleID == roleID)
                {
                    List.Remove(permission);
                    break;
                }
            }
        }
        public List<PermissionInfoBase> ToList()
        {
            List<PermissionInfoBase> list = new List<PermissionInfoBase>();
            foreach (PermissionInfoBase permission in this.List)
            {
                list.Add(permission);
            }
            return list;
        }
        public string ToString(string key)
        {
            return PermissionController.BuildPermissions(List, key);
        }
    }
}
