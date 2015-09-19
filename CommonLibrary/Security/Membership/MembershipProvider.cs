using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CommonLibrary.Entities.Users;
using CommonLibrary.ComponentModel;

namespace CommonLibrary.Security.Membership
{
    public abstract class MembershipProvider
    {
        public static new MembershipProvider Instance()
        {
            return ComponentFactory.GetComponent<MembershipProvider>();
        }
        public abstract bool CanEditProviderProperties { get; }
        public abstract int MaxInvalidPasswordAttempts { get; set; }
        public abstract int MinPasswordLength { get; set; }
        public abstract int MinNonAlphanumericCharacters { get; set; }
        public abstract int PasswordAttemptWindow { get; set; }
        public abstract PasswordFormat PasswordFormat { get; set; }
        public abstract bool PasswordResetEnabled { get; set; }
        public abstract bool PasswordRetrievalEnabled { get; set; }
        public abstract string PasswordStrengthRegularExpression { get; set; }
        public abstract bool RequiresQuestionAndAnswer { get; set; }
        public abstract bool RequiresUniqueEmail { get; set; }
        public abstract bool ChangePassword(UserInfo user, string oldPassword, string newPassword);
        public abstract bool ChangePasswordQuestionAndAnswer(UserInfo user, string password, string passwordQuestion, string passwordAnswer);
        public abstract UserCreateStatus CreateUser(ref UserInfo user);
        public abstract bool DeleteUser(UserInfo user);
        public abstract string GeneratePassword();
        public abstract string GeneratePassword(int length);
        public abstract string GetPassword(UserInfo user, string passwordAnswer);
        public abstract int GetUserCountByPortal(int portalId);
        public abstract void GetUserMembership(ref UserInfo user);
        public abstract string ResetPassword(UserInfo user, string passwordAnswer);
        public abstract bool UnLockUser(UserInfo user);
        public abstract void UpdateUser(UserInfo user);
        public abstract UserInfo UserLogin(int portalId, string username, string password, string verificationCode, ref UserLoginStatus loginStatus);
        public abstract UserInfo UserLogin(int portalId, string username, string password, string authType, string verificationCode, ref UserLoginStatus loginStatus);
        public abstract void DeleteUsersOnline(int TimeWindow);
        public abstract ArrayList GetOnlineUsers(int PortalId);
        public abstract bool IsUserOnline(UserInfo user);
        public abstract void UpdateUsersOnline(Hashtable UserList);
        public virtual void TransferUsersToMembershipProvider()
        {
        }
        public abstract UserInfo GetUser(int portalId, int userId);
        public abstract UserInfo GetUserByUserName(int portalId, string username);
        public abstract ArrayList GetUnAuthorizedUsers(int portalId);
        public abstract ArrayList GetUsers(int portalId, int pageIndex, int pageSize, ref int totalRecords);
        public abstract ArrayList GetUsersByEmail(int portalId, string emailToMatch, int pageIndex, int pageSize, ref int totalRecords);
        public abstract ArrayList GetUsersByUserName(int portalId, string userNameToMatch, int pageIndex, int pageSize, ref int totalRecords);
        public abstract ArrayList GetUsersByProfileProperty(int portalId, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords);
        public abstract ArrayList GetUsersByProfileProperty(int portalId, bool isHydrated, string propertyName, string propertyValue, int pageIndex, int pageSize, ref int totalRecords);
    }
}
