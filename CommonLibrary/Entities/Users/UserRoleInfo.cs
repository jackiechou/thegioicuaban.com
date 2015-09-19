using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security.Roles;
using System.Data;
using System.Data.SqlClient;

namespace CommonLibrary.Entities.Users
{
    [Serializable()]
    public class UserRoleInfo : RoleInfo
    {
        private int _UserRoleID;
        private int _UserID;
        private string _FullName;
        private string _Email;
        private System.DateTime _EffectiveDate;
        private System.DateTime _ExpiryDate;
        private bool _IsTrialUsed;
        private bool _Subscribed;

        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();
        public UserRoleInfo()
	    {		
	    }


        public int UserRoleID
        {
            get { return _UserRoleID; }
            set { _UserRoleID = value; }
        }
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        public System.DateTime EffectiveDate
        {
            get { return _EffectiveDate; }
            set { _EffectiveDate = value; }
        }
        public System.DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set { _ExpiryDate = value; }
        }
        public bool IsTrialUsed
        {
            get { return _IsTrialUsed; }
            set { _IsTrialUsed = value; }
        }
        public bool Subscribed
        {
            get { return _Subscribed; }
            set { _Subscribed = value; }
        }
        public override void Fill(IDataReader dr)
        {
            base.Fill(dr);
            UserRoleID = Null.SetNullInteger(dr["UserRoleID"]);
            UserID = Null.SetNullInteger(dr["UserID"]);
            FullName = Null.SetNullString(dr["DisplayName"]);
            Email = Null.SetNullString(dr["Email"]);
            EffectiveDate = Null.SetNullDateTime(dr["EffectiveDate"]);
            ExpiryDate = Null.SetNullDateTime(dr["ExpiryDate"]);
            IsTrialUsed = Null.SetNullBoolean(dr["IsTrialUsed"]);
            if (UserRoleID > Null.NullInteger)
            {
                Subscribed = true;
            }
        }

        #region Methods ===============================================================
        public void AddUsersToRoles(string ApplicationName, string UserName)
        {
            DateTime CurrentTimeUtc = System.DateTime.Now;

            SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_AddUsersToRoles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserNames", UserName);
            cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public DataTable FindUsersInRole(string ApplicationName, string RoleName, string UserNameToMatch)
        {
            SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_FindUsersInRole", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            cmd.Parameters.AddWithValue("@UserNameToMatch", UserNameToMatch);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetRolesForUser(string ApplicationName, string RoleName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_GetRolesForUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetUsersInRoles(string ApplicationName, string RoleName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_GetUsersInRoles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public void IsUserInRole(string ApplicationName, string UserName, string RoleName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_IsUserInRole", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserNames", UserName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void RemoveUsersFromRoles(string ApplicationName, string UserName, string RoleName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_RemoveUsersFromRoles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            cmd.Parameters.AddWithValue("@UserNames", UserName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        #endregion ====================================================================
    }
}
