using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.ComponentModel;
using CommonLibrary.Security.Roles;
using System.Collections;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Security.Roles
{
    public abstract class RoleProvider
    {
        public static new RoleProvider Instance()
        {
            return ComponentFactory.GetComponent<RoleProvider>();
        }
        public abstract bool CreateRole(int portalId, ref RoleInfo role);
        public abstract void DeleteRole(int portalId, ref RoleInfo role);
        public abstract RoleInfo GetRole(int portalId, int roleId);
        public abstract RoleInfo GetRole(int portalId, string roleName);
        public abstract string[] GetRoleNames(int portalId);
        public abstract string[] GetRoleNames(int portalId, int userId);
        public abstract ArrayList GetRoles(int portalId);
        public abstract ArrayList GetRolesByGroup(int portalId, int roleGroupId);
        public abstract void UpdateRole(RoleInfo role);
        public abstract int CreateRoleGroup(RoleGroupInfo roleGroup);
        public abstract void DeleteRoleGroup(RoleGroupInfo roleGroup);
        public abstract RoleGroupInfo GetRoleGroup(int portalId, int roleGroupId);
        public abstract ArrayList GetRoleGroups(int portalId);
        public abstract void UpdateRoleGroup(RoleGroupInfo roleGroup);
        public abstract bool AddUserToRole(int portalId, UserInfo user, UserRoleInfo userRole);
        public abstract UserRoleInfo GetUserRole(int PortalId, int UserId, int RoleId);
        public abstract ArrayList GetUserRoles(int PortalId, int UserId, bool includePrivate);
        public abstract ArrayList GetUserRoles(int PortalId, string Username, string Rolename);
        public abstract ArrayList GetUsersByRoleName(int portalId, string roleName);
        public abstract ArrayList GetUserRolesByRoleName(int portalId, string roleName);
        public abstract void RemoveUserFromRole(int portalId, UserInfo user, UserRoleInfo userRole);
        public abstract void UpdateUserRole(UserRoleInfo userRole);
        public virtual RoleGroupInfo GetRoleGroupByName(int PortalID, string RoleGroupName)
        {
            return null;
        }
    }
}
