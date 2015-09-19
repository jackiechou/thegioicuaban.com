using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CommonLibrary;
using CommonLibrary.Common;
using CommonLibrary.Modules;

namespace Library.Entities.Users.Membership
{
    public class Membership
    {
        ModuleClass module_obj = new ModuleClass();
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public Membership()
        {           
        }

      
        public int CreateUser(string ApplicationName, string UserName, string Password, string PasswordSalt, string Email, string PasswordQuestion, string PasswordAnswer, bool IsApproved, DateTime CurrentTimeUtc, DateTime CreateDate, int UniqueEmail, int PasswordFormat)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_CreateUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName",  UserName);
            cmd.Parameters.AddWithValue("@Password", Password);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@PasswordQuestion", PasswordQuestion);
            cmd.Parameters.AddWithValue("@PasswordAnswer", PasswordAnswer);
            cmd.Parameters.AddWithValue("@IsApproved",IsApproved);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc",CurrentTimeUtc);
            cmd.Parameters.AddWithValue("@CreateDate", CreateDate);
            cmd.Parameters.AddWithValue("@UniqueEmail",  UniqueEmail);
            cmd.Parameters.AddWithValue("@PasswordFormat",PasswordFormat);
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@UserId"].Value;
            con.Close();
            return retunvalue;
        }

        public void ChangePasswordQuestionAndAnswer(string app_name, string username, string new_pass_question, string new_pass_answer)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_ChangePasswordQuestionAndAnswer", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", app_name);
            cmd.Parameters.AddWithValue("@UserName", username);
            cmd.Parameters.AddWithValue("@NewPasswordQuestion", new_pass_question);
            cmd.Parameters.AddWithValue("@NewPasswordAnswer", new_pass_answer);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        
        public DataTable FindUsersByEmail(string ApplicationName, string EmailToMatch, int PageIndex, int PageSize)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_FindUsersByEmail", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@EmailToMatch", EmailToMatch);
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable FindUsersByName(string ApplicationName, string UserNameToMatch, int PageIndex, int PageSize)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_FindUsersByName", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserNameToMatch", UserNameToMatch);
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetAllUsers(string ApplicationName, int PageIndex, int PageSize)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_GetAllUsers", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetNumberOfUsersOnline(string ApplicationName, int MinutesSinceLastInActive, DateTime CurrentTimeUtc)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_GetNumberOfUsersOnline", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@MinutesSinceLastInActive", MinutesSinceLastInActive);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetPassword(string ApplicationName, string UserName, int MaxInvalidPasswordAttempts, DateTime CurrentTimeUtc, string PasswordAnswer)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_GetPassword", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@MaxInvalidPasswordAttempts", MaxInvalidPasswordAttempts);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            cmd.Parameters.AddWithValue("@PasswordAnswer", PasswordAnswer);
            con.Open();
            cmd.ExecuteNonQuery();
            string password = (string)cmd.ExecuteScalar();
            con.Close();
            return password;
        }

        public DataTable GetPasswordWithFormat(string ApplicationName, string UserName, bool UpdateLastLoginActivityDate, DateTime CurrentTimeUtc)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_GetPasswordWithFormat", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@UpdateLastLoginActivityDate", UpdateLastLoginActivityDate);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;            
        }

        public DataTable GetUserByEmail(string ApplicationName, string Email)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_GetUserByEmail", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Email", Email);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;            
        }

        public DataTable GetUserByName(string ApplicationName, string Email, DateTime CurrentTimeUtc, bool UpdateLastActivity)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_GetUserByName", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@Email", Email);            
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            cmd.Parameters.AddWithValue("@UpdateLastLoginActivityDate", UpdateLastActivity);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;            
        }

        public DataTable GetUserByUserId(string UserId, DateTime CurrentTimeUtc, bool UpdateLastActivity)
        {
            System.Guid user_id = new System.Guid(UserId);
            SqlCommand cmd = new SqlCommand("aspnet_Membership_GetUserByUserId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@user_id", user_id);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            cmd.Parameters.AddWithValue("@UpdateLastLoginActivityDate", UpdateLastActivity);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int ChangePassword(string UserId, string oldPassword, string newPassword)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_ChangePassword", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.AddWithValue("@oldPassword", oldPassword);
            cmd.Parameters.AddWithValue("@newPassword", newPassword);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        public int ResetPassword(string ApplicationName, string UserName, string NewPassword, int MaxInvalidPasswordAttempts, int PasswordAttemptWindow, string PasswordSalt, DateTime CurrentTimeUtc, int PasswordFormat, string PasswordAnswer)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_ResetPassword", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
            cmd.Parameters.AddWithValue("@MaxInvalidPasswordAttempts", MaxInvalidPasswordAttempts);
            cmd.Parameters.AddWithValue("@PasswordAttemptWindow", PasswordAttemptWindow);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            cmd.Parameters.AddWithValue("@PasswordFormat", PasswordFormat);
            cmd.Parameters.AddWithValue("@PasswordAnswer", PasswordAnswer);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;            
        }

        public int SetPassword(string ApplicationName, string UserName, string NewPassword, string PasswordSalt, DateTime CurrentTimeUtc, int PasswordFormat)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_SetPassword", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
            cmd.Parameters.AddWithValue("@PasswordSalt", PasswordSalt);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            cmd.Parameters.AddWithValue("@PasswordFormat", PasswordFormat);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        public int UnlockUser(string ApplicationName, string UserName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_UnlockUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;

        }

        public int UpdateUser(string ApplicationName, string UserName, string Email, string Comment, bool IsApproved, DateTime LastLoginDate, DateTime LastActivityDate, int UniqueEmail, DateTime CurrentTimeUtc)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_UpdateUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Comment", Comment);
            cmd.Parameters.AddWithValue("@IsApproved", IsApproved);
            cmd.Parameters.AddWithValue("@LastLoginDate", LastLoginDate);
            cmd.Parameters.AddWithValue("@LastActivityDate", LastActivityDate);
            cmd.Parameters.AddWithValue("@UniqueEmail", UniqueEmail);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        public int UpdateUserInfo(string ApplicationName, string UserName, string Email, bool IsPasswordCorrect, bool UpdateLastLoginActivityDate, int MaxInvalidPasswordAttempts, int PasswordAttemptWindow, DateTime CurrentTimeUtc, DateTime LastLoginDate, DateTime LastActivityDate)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Membership_UpdateUserInfo", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@IsPasswordCorrect", IsPasswordCorrect);
            cmd.Parameters.AddWithValue("@UpdateLastLoginActivityDate", UpdateLastLoginActivityDate);
            cmd.Parameters.AddWithValue("@MaxInvalidPasswordAttempts", MaxInvalidPasswordAttempts);
            cmd.Parameters.AddWithValue("@PasswordAttemptWindow", PasswordAttemptWindow);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            cmd.Parameters.AddWithValue("@LastLoginDate", LastLoginDate);
            cmd.Parameters.AddWithValue("@LastActivityDate", LastActivityDate);
            
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

    }
}
