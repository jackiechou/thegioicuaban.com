using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Library.Common;
using CommonLibrary;
using CommonLibrary.Common;
using CommonLibrary.Modules;
namespace Library.Common
{
    public class HitViewClass
    {
        ModuleClass module_obj = new ModuleClass();
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();
        
        public HitViewClass()  { }

        public int GetToTalViewsByPortal(int PortalId)
        {            
            SqlCommand cmd = new SqlCommand("[HitViews_GetTotalView]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            con.Open();
            int i = (int)cmd.ExecuteScalar();
            con.Close();
            return i;
        }

        public int GetToTalViewsInDayByPortal(int PortalId)
        {
            SqlCommand cmd = new SqlCommand("[HitViews_GetTotalViewInDay]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            con.Open();
            int i = (int)cmd.ExecuteScalar();
            con.Close();
            return i;
        }

        public int Update_Total_View(int PortalId)
        {
            SqlCommand cmd = new SqlCommand("[HitViews_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"].ToString()) };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;

        }
    }
}
