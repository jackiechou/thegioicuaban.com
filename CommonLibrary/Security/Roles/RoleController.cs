using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Security.Roles;
using CommonLibrary.Services.Messaging.Data;
using CommonLibrary.Common.Utilities;
using System.Globalization;
using System.Collections;
using CommonLibrary.Services.Messaging;
using CommonLibrary.Services.Localization;
using System.Xml;
using System.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Security.Roles
{
    public class RoleController
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        private enum UserRoleActions
        {
            add = 0,
            update = 1,
            delete = 2
        }
        private static string[] UserRoleActionsCaption = {
			"ASSIGNMENT",
			"UPDATE",
			"UNASSIGNMENT"
		};
        private static RoleProvider provider = RoleProvider.Instance();
        private static MessagingController _messagingController = new MessagingController();
        private void AutoAssignUsers(RoleInfo objRoleInfo)
        {
            if (objRoleInfo.AutoAssignment)
            {
                ArrayList arrUsers = UserController.GetUsers(objRoleInfo.PortalID);
                foreach (UserInfo objUser in arrUsers)
                {
                    try
                    {
                        AddUserRole(objRoleInfo.PortalID, objUser.UserID, objRoleInfo.RoleID, Null.NullDate, Null.NullDate);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
        }
        public int AddRole(RoleInfo objRoleInfo)
        {
            int roleId = -1;
            bool success = provider.CreateRole(objRoleInfo.PortalID, ref objRoleInfo);
            if (success)
            {
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog(objRoleInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.ROLE_CREATED);
                AutoAssignUsers(objRoleInfo);
                roleId = objRoleInfo.RoleID;
            }
            return roleId;
        }
        public void DeleteRole(int RoleId, int PortalId)
        {
            RoleInfo objRole = GetRole(RoleId, PortalId);
            if (objRole != null)
            {
                provider.DeleteRole(PortalId, ref objRole);
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                objEventLog.AddLog("RoleID", RoleId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Services.Log.EventLog.EventLogController.EventLogType.ROLE_DELETED);
            }
        }
        public ArrayList GetPortalRoles(int PortalId)
        {
            return provider.GetRoles(PortalId);
        }
        public RoleInfo GetRole(int RoleID, int PortalID)
        {
            return provider.GetRole(PortalID, RoleID);
        }
        public RoleInfo GetRoleByName(int PortalId, string RoleName)
        {
            return provider.GetRole(PortalId, RoleName);
        }
        public string[] GetRoleNames(int PortalID)
        {
            return provider.GetRoleNames(PortalID);
        }
        public ArrayList GetRoles()
        {
            return provider.GetRoles(Null.NullInteger);
        }
        public ArrayList GetRolesByGroup(int portalId, int roleGroupId)
        {
            return provider.GetRolesByGroup(portalId, roleGroupId);
        }
        public string[] GetRolesByUser(int UserId, int PortalId)
        {
            return provider.GetRoleNames(PortalId, UserId);
        }
        public void UpdateRole(RoleInfo objRoleInfo)
        {
            provider.UpdateRole(objRoleInfo);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objRoleInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.ROLE_UPDATED);
            AutoAssignUsers(objRoleInfo);
        }
        public void AddUserRole(int PortalID, int UserId, int RoleId, System.DateTime ExpiryDate)
        {
            AddUserRole(PortalID, UserId, RoleId, Null.NullDate, ExpiryDate);
        }
        public void AddUserRole(int PortalID, int UserId, int RoleId, System.DateTime EffectiveDate, System.DateTime ExpiryDate)
        {
            UserInfo objUser = UserController.GetUserById(PortalID, UserId);
            UserRoleInfo objUserRole = GetUserRole(PortalID, UserId, RoleId);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            if (objUserRole == null)
            {
                objUserRole = new UserRoleInfo();
                objUserRole.UserID = UserId;
                objUserRole.RoleID = RoleId;
                objUserRole.PortalID = PortalID;
                objUserRole.EffectiveDate = EffectiveDate;
                objUserRole.ExpiryDate = ExpiryDate;
                provider.AddUserToRole(PortalID, objUser, objUserRole);
                objEventLog.AddLog(objUserRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_CREATED);
            }
            else
            {
                objUserRole.EffectiveDate = EffectiveDate;
                objUserRole.ExpiryDate = ExpiryDate;
                provider.UpdateUserRole(objUserRole);
                objEventLog.AddLog(objUserRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED);
            }
        }
        public bool DeleteUserRole(int PortalId, int UserId, int RoleId)
        {
            UserInfo objUser = UserController.GetUserById(PortalId, UserId);
            UserRoleInfo objUserRole = GetUserRole(PortalId, UserId, RoleId);
            PortalController objPortals = new PortalController();
            bool blnDelete = true;
            PortalInfo objPortal = objPortals.GetPortal(PortalId);
            if (objPortal != null && objUserRole != null)
            {
                if (CanRemoveUserFromRole(objPortal, UserId, RoleId))
                {
                    provider.RemoveUserFromRole(PortalId, objUser, objUserRole);
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog(objUserRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.ROLE_UPDATED);
                }
                else
                {
                    blnDelete = false;
                }
            }
            return blnDelete;
        }
        public UserRoleInfo GetUserRole(int PortalID, int UserId, int RoleId)
        {
            return provider.GetUserRole(PortalID, UserId, RoleId);
        }
        public ArrayList GetUserRoles(int PortalId)
        {
            return GetUserRoles(PortalId, -1);
        }
        public ArrayList GetUserRoles(int PortalId, int UserId)
        {
            return provider.GetUserRoles(PortalId, UserId, true);
        }
        public ArrayList GetUserRoles(int PortalId, int UserId, bool includePrivate)
        {
            return provider.GetUserRoles(PortalId, UserId, includePrivate);
        }
        public ArrayList GetUserRolesByUsername(int PortalID, string Username, string Rolename)
        {
            return provider.GetUserRoles(PortalID, Username, Rolename);
        }
        public ArrayList GetUserRolesByRoleName(int portalId, string roleName)
        {
            return provider.GetUserRolesByRoleName(portalId, roleName);
        }
        public ArrayList GetUsersByRoleName(int PortalID, string RoleName)
        {
            return provider.GetUsersByRoleName(PortalID, RoleName);
        }
        public void UpdateUserRole(int PortalId, int UserId, int RoleId)
        {
            UpdateUserRole(PortalId, UserId, RoleId, false);
        }
        public void UpdateUserRole(int PortalId, int UserId, int RoleId, bool Cancel)
        {
            UserRoleInfo userRole;
            userRole = GetUserRole(PortalId, UserId, RoleId);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            if (Cancel)
            {
                if (userRole != null && userRole.ServiceFee > 0.0 && userRole.IsTrialUsed)
                {
                    userRole.ExpiryDate = DateTime.Now.AddDays(-1);
                    provider.UpdateUserRole(userRole);
                    objEventLog.AddLog(userRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED);
                }
                else
                {
                    DeleteUserRole(PortalId, UserId, RoleId);
                    objEventLog.AddLog("UserId", UserId.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_DELETED);
                }
            }
            else
            {
                int UserRoleId = -1;
                RoleInfo role;
                System.DateTime ExpiryDate = DateTime.Now;
                System.DateTime EffectiveDate = Null.NullDate;
                bool IsTrialUsed = false;
                int Period = 0;
                string Frequency = "";
                if (userRole != null)
                {
                    UserRoleId = userRole.UserRoleID;
                    EffectiveDate = userRole.EffectiveDate;
                    ExpiryDate = userRole.ExpiryDate;
                    IsTrialUsed = userRole.IsTrialUsed;
                }
                role = GetRole(RoleId, PortalId);
                if (role != null)
                {
                    if (IsTrialUsed == false && role.TrialFrequency.ToString() != "N")
                    {
                        Period = role.TrialPeriod;
                        Frequency = role.TrialFrequency;
                    }
                    else
                    {
                        Period = role.BillingPeriod;
                        Frequency = role.BillingFrequency;
                    }
                }
                if (EffectiveDate < DateTime.Now)
                {
                    EffectiveDate = Null.NullDate;
                }
                if (ExpiryDate < DateTime.Now)
                {
                    ExpiryDate = DateTime.Now;
                }
                if (Period == Null.NullInteger)
                {
                    ExpiryDate = Null.NullDate;
                }
                else
                {
                    switch (Frequency)
                    {
                        case "N":
                            ExpiryDate = Null.NullDate;
                            break;
                        case "O":
                            ExpiryDate = new System.DateTime(9999, 12, 31);
                            break;
                        case "D":
                            ExpiryDate = ExpiryDate.AddDays(Period);
                            break;
                        case "W":
                            ExpiryDate = ExpiryDate.AddDays(Period * 7);
                            break;
                        case "M":
                            ExpiryDate = ExpiryDate.AddMonths(Period);
                            break;
                        case "Y":
                            ExpiryDate = ExpiryDate.AddYears(Period);
                            break;
                    }
                }
                if (UserRoleId != -1)
                {
                    userRole.ExpiryDate = ExpiryDate;
                    provider.UpdateUserRole(userRole);
                    objEventLog.AddLog(userRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED);
                }
                else
                {
                    AddUserRole(PortalId, UserId, RoleId, EffectiveDate, ExpiryDate);
                }
            }
        }
        private static void SendNotification(UserInfo objUser, RoleInfo objRole, PortalSettings PortalSettings, UserRoleActions Action)
        {
            RoleController objRoles = new RoleController();
            ArrayList Custom = new ArrayList();
            Custom.Add(objRole.RoleName);
            Custom.Add(objRole.Description);
            switch (Action)
            {
                case UserRoleActions.add:
                case UserRoleActions.update:
                    string preferredLocale = objUser.Profile.PreferredLocale;
                    if (string.IsNullOrEmpty(preferredLocale))
                    {
                        preferredLocale = PortalSettings.DefaultLanguage;
                    }
                    CultureInfo ci = new CultureInfo(preferredLocale);
                    UserRoleInfo objUserRole = objRoles.GetUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID);
                    if (Null.IsNull(objUserRole.EffectiveDate))
                    {
                        Custom.Add(DateTime.Today.ToString("g", ci));
                    }
                    else
                    {
                        Custom.Add(objUserRole.EffectiveDate.ToString("g", ci));
                    }
                    if (Null.IsNull(objUserRole.ExpiryDate))
                    {
                        Custom.Add("-");
                    }
                    else
                    {
                        Custom.Add(objUserRole.ExpiryDate.ToString("g", ci));
                    }
                    break;
                case UserRoleActions.delete:
                    Custom.Add("");
                    break;
            }
            Message _message = new Message();
            _message.FromUserID = PortalSettings.AdministratorId;
            _message.ToUserID = objUser.UserID;
            _message.Subject = Services.Localization.Localization.GetSystemMessage(objUser.Profile.PreferredLocale, PortalSettings, "EMAIL_ROLE_" + RoleController.UserRoleActionsCaption[(int)Action] + "_SUBJECT", objUser);
            _message.Body = Localization.GetSystemMessage(objUser.Profile.PreferredLocale, PortalSettings, "EMAIL_ROLE_" + RoleController.UserRoleActionsCaption[(int)Action] + "_BODY", objUser, Services.Localization.Localization.GlobalResourceFile, Custom);
            _message.Status = MessageStatusType.Unread;

            //_messagingController.SaveMessage(_message);
            //Services.Mail.Mail.SendEmail(PortalSettings.Email, objUser.Email, _message.Subject, _message.Body);
        }
        public static int AddRoleGroup(RoleGroupInfo objRoleGroupInfo)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objRoleGroupInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_CREATED);
            return provider.CreateRoleGroup(objRoleGroupInfo);
        }
        public static void AddUserRole(UserInfo objUser, RoleInfo objRole, PortalSettings PortalSettings, System.DateTime effDate, System.DateTime expDate, int userId, bool notifyUser)
        {
            RoleController objRoleController = new RoleController();
            UserRoleInfo objUserRole = objRoleController.GetUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objRoleController.AddUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID, effDate, expDate);
            objUser.RefreshRoles = true;
            UserController.UpdateUser(PortalSettings.PortalId, objUser);
            if (objUserRole == null)
            {
                objEventLog.AddLog("Role", objRole.RoleName, PortalSettings, userId, Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_CREATED);
                if (notifyUser)
                {
                    SendNotification(objUser, objRole, PortalSettings, UserRoleActions.@add);
                }
            }
            else
            {
                objEventLog.AddLog("Role", objRole.RoleName, PortalSettings, userId, Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED);
                if (notifyUser)
                {
                    objUserRole = objRoleController.GetUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID);
                    SendNotification(objUser, objRole, PortalSettings, UserRoleActions.update);
                }
            }
        }
        public static bool DeleteUserRole(int userId, RoleInfo objRole, PortalSettings PortalSettings, bool notifyUser)
        {
            UserInfo objUser = UserController.GetUserById(PortalSettings.PortalId, userId);
            return DeleteUserRole(objUser, objRole, PortalSettings, notifyUser);
        }
        public static bool DeleteUserRole(int roleId, UserInfo objUser, PortalSettings PortalSettings, bool notifyUser)
        {
            RoleController objRoleController = new RoleController();
            RoleInfo objRole = objRoleController.GetRole(roleId, PortalSettings.PortalId);
            return DeleteUserRole(objUser, objRole, PortalSettings, notifyUser);
        }
        public static bool DeleteUserRole(UserInfo objUser, RoleInfo objRole, PortalSettings PortalSettings, bool notifyUser)
        {
            RoleController objRoleController = new RoleController();
            bool canDelete = objRoleController.DeleteUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID);
            if (canDelete)
            {
                if (notifyUser)
                {
                    SendNotification(objUser, objRole, PortalSettings, UserRoleActions.delete);
                }
            }
            return canDelete;
        }
        public static bool CanRemoveUserFromRole(PortalSettings PortalSettings, int UserId, int RoleId)
        {
            return !((PortalSettings.AdministratorId == UserId && PortalSettings.AdministratorRoleId == RoleId) || PortalSettings.RegisteredRoleId == RoleId);
        }
        public static bool CanRemoveUserFromRole(PortalInfo PortalInfo, int UserId, int RoleId)
        {
            return !((PortalInfo.AdministratorId == UserId && PortalInfo.AdministratorRoleId == RoleId) || PortalInfo.RegisteredRoleId == RoleId);
        }
        public static void DeleteRoleGroup(int PortalID, int RoleGroupId)
        {
            DeleteRoleGroup(GetRoleGroup(PortalID, RoleGroupId));
        }
        public static void DeleteRoleGroup(RoleGroupInfo objRoleGroupInfo)
        {
            provider.DeleteRoleGroup(objRoleGroupInfo);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objRoleGroupInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_DELETED);
        }
        public static RoleGroupInfo GetRoleGroup(int PortalID, int RoleGroupID)
        {
            return provider.GetRoleGroup(PortalID, RoleGroupID);
        }
        public static RoleGroupInfo GetRoleGroupByName(int PortalID, string RoleGroupName)
        {
            return provider.GetRoleGroupByName(PortalID, RoleGroupName);
        }
        public static ArrayList GetRoleGroups(int PortalID)
        {
            return provider.GetRoleGroups(PortalID);
        }
        public static void SerializeRoleGroups(XmlWriter writer, int portalID)
        {
            writer.WriteStartElement("rolegroups");
            foreach (RoleGroupInfo objRoleGroup in GetRoleGroups(portalID))
            {
                CBO.SerializeObject(objRoleGroup, writer);
            }
            RoleGroupInfo globalRoleGroup = new RoleGroupInfo(Null.NullInteger, portalID, true);
            globalRoleGroup.RoleGroupName = "GlobalRoles";
            globalRoleGroup.Description = "A dummy role group that represents the Global roles";
            CBO.SerializeObject(globalRoleGroup, writer);
            writer.WriteEndElement();
        }
        public static void SerializeRoles(XmlWriter writer, int portalID)
        {
            writer.WriteStartElement("roles");
            foreach (RoleInfo objRole in new RoleController().GetRolesByGroup(portalID, Null.NullInteger))
            {
                CBO.SerializeObject(objRole, writer);
            }
            writer.WriteEndElement();
        }
        public static void UpdateRoleGroup(RoleGroupInfo roleGroup)
        {
            UpdateRoleGroup(roleGroup, false);
        }
        public static void UpdateRoleGroup(RoleGroupInfo roleGroup, bool includeRoles)
        {
            provider.UpdateRoleGroup(roleGroup);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(roleGroup, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_ROLE_UPDATED);
            if (includeRoles)
            {
                RoleController controller = new RoleController();
                foreach (RoleInfo role in roleGroup.Roles.Values)
                {
                    controller.UpdateRole(role);
                    objEventLog.AddLog(role, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Services.Log.EventLog.EventLogController.EventLogType.ROLE_UPDATED);
                }
            }
        }

        #region ======================================================================================================
        public DataTable GetAllRoles(string ApplicationName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Roles_GetAllRoles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetRoleListByApplicationId(string ApplicationId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Roles_GetRoleListByApplicationId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(string RoleId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Roles_GetDetails", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string CreateRole(string ApplicationName, string RoleName, string Description)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Roles_CreateRole", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.Add(new SqlParameter("@ErrorCode", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@ErrorCode"].Value;
            con.Close();
            return retunvalue;
        }

        public string UpdateRole(string ApplicationName, string RoleId, string RoleName, string Description)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Roles_UpdateRole", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.Add(new SqlParameter("@ErrorCode", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@ErrorCode"].Value;
            con.Close();
            return retunvalue;
        }

        public void Delete(string ApplicationId, string RoleId, bool DeleteOnlyIfRoleIsEmpty)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Roles_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@DeleteOnlyIfRoleIsEmpty", DeleteOnlyIfRoleIsEmpty);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void DeleteRole(string ApplicationName, string RoleName, bool DeleteOnlyIfRoleIsEmpty)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Roles_DeleteRole", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            cmd.Parameters.AddWithValue("@DeleteOnlyIfRoleIsEmpty", DeleteOnlyIfRoleIsEmpty);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }


        public string IsRoleExists(string ApplicationName, string RoleName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Roles_RoleExists", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public string RemoveAllRoleMembers(string ApplicationName, string rolename)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Setup_RemoveAllRoleMembers", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleName", rolename);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public string RestorePermissions(string Name)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Setup_RestorePermissions", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };

            cmd.Parameters.AddWithValue("@name", Name);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;

        }
        #endregion ===================================================================================================
    }
}
