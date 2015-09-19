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

public class UsersInRolesClass
{
    //ModuleClass module_obj = new ModuleClass();
    //string ip = IPNetworking.GetIP4Address();
    //SqlConnection con = new SqlConnection(Settings.ConnectionString);
    //DataTable dt = new DataTable();

    //public UsersInRolesClass()
    //{		
    //}

    //public void AddUsersToRoles(string ApplicationName, string UserName)
    //{
    //    DateTime CurrentTimeUtc = System.DateTime.Now;

    //    SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_AddUsersToRoles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
    //    cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
    //    cmd.Parameters.AddWithValue("@UserNames", UserName);
    //    cmd.Parameters.AddWithValue("@CurrentTimeUtc", CurrentTimeUtc);
    //    con.Open();
    //    cmd.ExecuteNonQuery();
    //    con.Close();
    //}

    //public DataTable FindUsersInRole(string ApplicationName, string RoleName, string UserNameToMatch)
    //{
    //    SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_FindUsersInRole", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
    //    cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
    //    cmd.Parameters.AddWithValue("@RoleName", RoleName);
    //    cmd.Parameters.AddWithValue("@UserNameToMatch", UserNameToMatch);
    //    con.Open();
    //    using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
    //    con.Close();
    //    return dt;
    //}

    //public DataTable GetRolesForUser(string ApplicationName, string RoleName)
    //{
    //    SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_GetRolesForUser", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
    //    cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
    //    cmd.Parameters.AddWithValue("@RoleName", RoleName);
    //    con.Open();
    //    using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
    //    con.Close();
    //    return dt;
    //}

    //public DataTable GetUsersInRoles(string ApplicationName, string RoleName)
    //{
    //    SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_GetUsersInRoles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
    //    cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
    //    cmd.Parameters.AddWithValue("@RoleName", RoleName);
    //    con.Open();
    //    using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
    //    con.Close();
    //    return dt;
    //}

    //public void IsUserInRole(string ApplicationName, string UserName, string RoleName)
    //{
    //    SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_IsUserInRole", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
    //    cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
    //    cmd.Parameters.AddWithValue("@UserNames", UserName);
    //    cmd.Parameters.AddWithValue("@RoleName", RoleName);
    //    con.Open();
    //    cmd.ExecuteNonQuery();
    //    con.Close();
    //}

    //public void RemoveUsersFromRoles(string ApplicationName, string UserName, string RoleName)
    //{
    //    SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_RemoveUsersFromRoles", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
    //    cmd.Parameters.AddWithValue("@ApplicationName", ApplicationName);
    //    cmd.Parameters.AddWithValue("@UserNames", UserName);
    //    cmd.Parameters.AddWithValue("@RoleName", RoleName);
    //    con.Open();
    //    cmd.ExecuteNonQuery();
    //    con.Close();
    //}
    
}