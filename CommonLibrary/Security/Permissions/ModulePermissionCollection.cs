using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Security.Permissions
{
    [Serializable()]
    public class ModulePermissionCollection : CollectionBase
    {
        public ModulePermissionCollection()
            : base()
        {
        }
        public ModulePermissionCollection(ArrayList modulePermissions)
        {
            AddRange(modulePermissions);
        }
        public ModulePermissionCollection(ModulePermissionCollection modulePermissions)
        {
            AddRange(modulePermissions);
        }
        public ModulePermissionCollection(ArrayList modulePermissions, int ModuleID)
        {
            foreach (ModulePermissionInfo permission in modulePermissions)
            {
                if (permission.ModuleID == ModuleID)
                {
                    Add(permission);
                }
            }
        }
        public ModulePermissionCollection(ModuleInfo objModule)
        {
            foreach (ModulePermissionInfo permission in objModule.ModulePermissions)
            {
                if (permission.ModuleID == objModule.ModuleID)
                {
                    Add(permission);
                }
            }
        }
        public ModulePermissionInfo this[int index]
        {
            get { return (ModulePermissionInfo)List[index]; }
            set { List[index] = value; }
        }
        public int Add(ModulePermissionInfo value)
        {
            return List.Add(value);
        }
        public int Add(ModulePermissionInfo value, bool checkForDuplicates)
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
        public void AddRange(ArrayList modulePermissions)
        {
            foreach (ModulePermissionInfo permission in modulePermissions)
            {
                Add(permission);
            }
        }
        public void AddRange(ModulePermissionCollection modulePermissions)
        {
            foreach (ModulePermissionInfo permission in modulePermissions)
            {
                Add(permission);
            }
        }
        public bool CompareTo(ModulePermissionCollection objModulePermissionCollection)
        {
            if (objModulePermissionCollection.Count != this.Count)
            {
                return false;
            }
            InnerList.Sort(new CompareModulePermissions());
            objModulePermissionCollection.InnerList.Sort(new CompareModulePermissions());
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (objModulePermissionCollection[i].ModulePermissionID != this[i].ModulePermissionID || objModulePermissionCollection[i].AllowAccess != this[i].AllowAccess)
                {
                    return false;
                }
            }
            return true;
        }
        public bool Contains(ModulePermissionInfo value)
        {
            return List.Contains(value);
        }
        public int IndexOf(ModulePermissionInfo value)
        {
            return List.IndexOf(value);
        }
        public void Insert(int index, ModulePermissionInfo value)
        {
            List.Insert(index, value);
        }
        public void Remove(ModulePermissionInfo value)
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
