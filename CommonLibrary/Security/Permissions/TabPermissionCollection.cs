using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class TabPermissionCollection : CollectionBase
    {
        public TabPermissionCollection()
            : base()
        {
        }
        public TabPermissionCollection(ArrayList tabPermissions)
        {
            AddRange(tabPermissions);
        }
        public TabPermissionCollection(TabPermissionCollection tabPermissions)
        {
            AddRange(tabPermissions);
        }
        public TabPermissionCollection(ArrayList tabPermissions, int TabId)
        {
            foreach (TabPermissionInfo permission in tabPermissions)
            {
                if (permission.TabID == TabId)
                {
                    Add(permission);
                }
            }
        }
        public TabPermissionInfo this[int index]
        {
            get { return (TabPermissionInfo)List[index]; }
            set { List[index] = value; }
        }
        public int Add(TabPermissionInfo value)
        {
            return List.Add(value);
        }
        public int Add(TabPermissionInfo value, bool checkForDuplicates)
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
        public void AddRange(ArrayList tabPermissions)
        {
            foreach (TabPermissionInfo permission in tabPermissions)
            {
                Add(permission);
            }
        }
        public void AddRange(TabPermissionCollection tabPermissions)
        {
            foreach (TabPermissionInfo permission in tabPermissions)
            {
                Add(permission);
            }
        }
        public bool CompareTo(TabPermissionCollection objTabPermissionCollection)
        {
            if (objTabPermissionCollection.Count != this.Count)
            {
                return false;
            }
            InnerList.Sort(new CompareTabPermissions());
            objTabPermissionCollection.InnerList.Sort(new CompareTabPermissions());
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (objTabPermissionCollection[i].TabPermissionID != this[i].TabPermissionID || objTabPermissionCollection[i].AllowAccess != this[i].AllowAccess)
                {
                    return false;
                }
            }
            return true;
        }
        public bool Contains(TabPermissionInfo value)
        {
            return List.Contains(value);
        }
        public int IndexOf(TabPermissionInfo value)
        {
            return List.IndexOf(value);
        }
        public void Insert(int index, TabPermissionInfo value)
        {
            List.Insert(index, value);
        }
        public void Remove(TabPermissionInfo value)
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
