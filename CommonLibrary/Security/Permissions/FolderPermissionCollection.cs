using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class FolderPermissionCollection : CollectionBase
    {
        public FolderPermissionCollection()
            : base()
        {
        }
        public FolderPermissionCollection(ArrayList folderPermissions)
        {
            AddRange(folderPermissions);
        }
        public FolderPermissionCollection(FolderPermissionCollection folderPermissions)
        {
            AddRange(folderPermissions);
        }
        public FolderPermissionCollection(ArrayList folderPermissions, string FolderPath)
        {
            foreach (FolderPermissionInfo permission in folderPermissions)
            {
                if (permission.FolderPath == FolderPath)
                {
                    Add(permission);
                }
            }
        }
        public FolderPermissionInfo this[int index]
        {
            get { return (FolderPermissionInfo)List[index]; }
            set { List[index] = value; }
        }
        public int Add(FolderPermissionInfo value)
        {
            return List.Add(value);
        }
        public int Add(FolderPermissionInfo value, bool checkForDuplicates)
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
        public void AddRange(ArrayList folderPermissions)
        {
            foreach (FolderPermissionInfo permission in folderPermissions)
            {
                List.Add(permission);
            }
        }
        public void AddRange(FolderPermissionCollection folderPermissions)
        {
            foreach (FolderPermissionInfo permission in folderPermissions)
            {
                List.Add(permission);
            }
        }
        public int IndexOf(FolderPermissionInfo value)
        {
            return List.IndexOf(value);
        }
        public void Insert(int index, FolderPermissionInfo value)
        {
            List.Insert(index, value);
        }
        public void Remove(FolderPermissionInfo value)
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
        public bool Contains(FolderPermissionInfo value)
        {
            return List.Contains(value);
        }
        public bool CompareTo(FolderPermissionCollection objFolderPermissionCollection)
        {
            if (objFolderPermissionCollection.Count != this.Count)
            {
                return false;
            }
            InnerList.Sort(new CompareFolderPermissions());
            objFolderPermissionCollection.InnerList.Sort(new CompareFolderPermissions());
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (objFolderPermissionCollection[i].FolderPermissionID != this[i].FolderPermissionID || objFolderPermissionCollection[i].AllowAccess != this[i].AllowAccess)
                {
                    return false;
                }
            }
            return true;
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
