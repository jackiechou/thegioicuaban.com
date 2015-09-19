using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using CommonLibrary.Modules;
using CommonLibrary.Common;

namespace CommonLibrary.Entities.Tabs
{
    public class TabModules
    {
        ModuleClass module_obj = new ModuleClass();
        string ip = IPNetworking.GetIP4Address();
        DataSet ds = new DataSet();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();



        public TabModules()
        {

        }

        public DataTable GetModuleByTabId(int TabId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabModules_GetModuleByTabId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetTabNameByTabId(int TabId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetTabNameByTabId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string TabName = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return TabName;
        }

        //INSERT- UPDATE - DELETE                                                                        
        public int Insert(int TabId, int ModuleId, string PaneName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabModules_AddModule2Tab", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.AddWithValue("@PaneName", PaneName);
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int i = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return i;
        }

        public int Update(int TabId, int ModuleId, string PaneName)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabModules_UpdateTabModule", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.AddWithValue("@ModuleId", ModuleId);
            cmd.Parameters.AddWithValue("@PaneName", PaneName);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int i = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return i;

        }

        public int Delete(int TabId)
        {
            SqlCommand cmd = new SqlCommand("aspnet_TabModules_Delete", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TabId", TabId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int i = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return i;
        }

        ////lay menu theo role
        //public string GetMenuStringByRole(int groupId)
        //{
        //    string result = string.Empty;
        //    try
        //    {
        //        DbCommand dbCommand = db.GetStoredProcCommand("sp_Menu_Admin_GetMenuStringByRole");
        //        db.AddInParameter(dbCommand, "@i_GroupId", DbType.Int32, groupId);
        //        db.AddOutParameter(dbCommand, "@o_Return", DbType.String, 200);
        //        db.ExecuteNonQuery(dbCommand);
        //        result = db.GetParameterValue(dbCommand, "@o_return").ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //    }
        //    return result;
        //}

        ////lay menu list theo role
        //public DataSet GetMenuListByRole(int Id, int status)
        //{
        //    ds = db.ExecuteDataSet("sp_Menu_Admin_GetMenuListByRole", Id, status);
        //    return ds;
        //}

        ////Load horizontal menu
        //public DataSet GetHierarchicalTreeByUserId(int userId)
        //{
        //    ds = db.ExecuteDataSet("sp_Menu_Admin_GetMenuByUserId", userId);
        //    return ds;
        //}


        ////Load left menu
        //public DataSet GetHierarchicalTree(int userId)
        //{
        //    try
        //    {
        //        //select 1 as Status, Idx, ParentId, MenuName, LinkMenu, Target, GroupId, PermissionGroup
        //        //select -1 as Status, Idx, ParentId, MenuName, LinkMenu, Target, GroupId, PermissionGroup -- username has blocked            
        //        ds = db.ExecuteDataSet("sp_Menu_Admin_GetMenu", userId);
        //    }
        //    catch (ArgumentNullException ex) { ex.ToString(); }
        //    return ds;
        //}

        ////load all menu
        //public DataSet GetListByStatus(string status)
        //{
        //    return db.ExecuteDataSet("sp_Menu_Admin_GetListByStatus", status);
        //}

        ////load all menu
        //public DataTable GetAllMenuList()
        //{
        //    try
        //    {
        //        //Description: Lay danh sach menu theo trang thai ( @status = 0 : ẩn, @status = 1 : hiện, @status = -1 : lay tat ca )
        //        //Idx, MenuName, ParentId, Status
        //        int status = -1;
        //        dt = db.ExecuteDataSet("sp_Menu_Admin_GetMenuList", status).Tables[0];
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }
        //    return dt;
        //}

        ////load all valId menu
        //public DataSet LoadValIdMenuList()
        //{
        //    try
        //    {
        //        //Description: Lay danh sach menu theo trang thai ( @status = 0 : ẩn, @status = 1 : hiện, @status = -1 : lay tat ca )
        //        //Idx, MenuName, ParentId, Status
        //        int status = 1;
        //        ds = db.ExecuteDataSet("sp_Menu_Admin_GetMenuList", status);
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }
        //    return ds;
        //}


        ////load all valId menu
        //public DataTable GetValIdMenuList()
        //{
        //    try
        //    {
        //        //Description: Lay danh sach menu theo trang thai ( @status = 0 : ẩn, @status = 1 : hiện, @status = -1 : lay tat ca )
        //        //Idx, MenuName, ParentId, Status
        //        int status = 1;
        //        dt = db.ExecuteDataSet("sp_Menu_Admin_GetMenuList", status).Tables[0];
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }
        //    return dt;
        //}

        //public DataTable GetMenuListByMenuId(int Id)
        //{
        //    try
        //    {
        //        dt = db.ExecuteDataSet("sp_Menu_Admin_GetMenuListByMenuId", Id).Tables[0];
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }
        //    return dt;
        //}

        //public DataTable GetMenuListByIdStatus(int Id, int status)
        //{
        //    try
        //    {
        //        dt = db.ExecuteDataSet("sp_Menu_Admin_GetMenuListById", Id, status).Tables[0];
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }
        //    return dt;
        //}

        ////get detail menu
        //public DataTable GetMenuDetailById(int Id)
        //{
        //    try
        //    {
        //        //Idx, MenuName, ParentId, LinkMenu, TypeSite, Grade, Status, DateLog, DateUpdate  
        //        //Status = 0 : ẩn , = 1 : hiện 

        //        dt = db.ExecuteDataSet("sp_Menu_Admin_GetMenuDetail", Id).Tables[0];
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }
        //    return dt;
        //}


        ////get menu by role
        //public DataTable GetMenuByRole()
        //{
        //    try
        //    {
        //        //Idx, ParentId, MenuName, LinkMenu
        //        dt = db.ExecuteDataSet("sp_Menu_Admin_GetMenuByGroup").Tables[0];
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }
        //    return dt;
        //}



        ////update status
        //public int UpdateStatus(int userId, int Idx, string status)
        //{
        //    int i = 0;
        //    try
        //    {
        //        DbCommand dbCommand = db.GetStoredProcCommand("sp_Menu_Admin_UpdateStatus");
        //        db.AddInParameter(dbCommand, "@userId", DbType.Int32, userId);
        //        db.AddInParameter(dbCommand, "@ip", DbType.String, ip);
        //        db.AddInParameter(dbCommand, "@Idx", DbType.Int32, Idx);
        //        db.AddInParameter(dbCommand, "@status", DbType.String, status);
        //        db.AddOutParameter(dbCommand, "@o_return", DbType.Int32, 1);
        //        db.ExecuteNonQuery(dbCommand);
        //        i = (int)db.GetParameterValue(dbCommand, "@o_return");
        //    }
        //    catch (IndexOutOfRangeException ex) { ex.ToString(); }
        //    return i;
        //}

        //public int UpdateSortKey(int userId, int Idx, int sortkey)
        //{
        //    DbCommand dbCommand = db.GetStoredProcCommand("[AdminMenus].[sp_Menu_UpdateSortKey]");
        //    dbCommand.CommandType = CommandType.StoredProcedure;
        //    dbCommand.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString());
        //    db.AddInParameter(dbCommand, "@userId", DbType.Int32, userId);
        //    db.AddInParameter(dbCommand, "@ip", DbType.String, ip);
        //    db.AddInParameter(dbCommand, "@Idx", DbType.Int32, Idx);
        //    db.AddInParameter(dbCommand, "@sortkey", DbType.Int32, sortkey);
        //    db.AddOutParameter(dbCommand, "@o_return", DbType.Int32, 1);
        //    db.ExecuteNonQuery(dbCommand);

        //    int i = Convert.ToInt32(db.GetParameterValue(dbCommand, "o_return").ToString());
        //    return i;
        //}


    }
}
