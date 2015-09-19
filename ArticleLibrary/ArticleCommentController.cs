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
using Library.Common;
using CommonLibrary;
using CommonLibrary.Common;
using CommonLibrary.Modules;
using System.Collections.Generic;
using System.Linq;

namespace ArticleLibrary
{
    public class ArticleCommentController
    {
        ModuleClass module_obj = new ModuleClass();
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public ArticleCommentController() { }

        public DataTable GetDetailById(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleComments_GetDetailById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetList()
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleComments_GetList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };            
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public List<ArticleComment> GetListByArticleIdStatus(int ArticleId, string status)
        {
            using (ArticleEntities dbContext = new ArticleLibrary.ArticleEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                System.Data.Objects.ObjectResult<ArticleComment> obj_result = dbContext.sp_ArticleComments_GetList();
                IQueryable<ArticleComment> query = obj_result.AsQueryable();
                if (ArticleId>0)
                    query = query.Where(x => x.ArticleId == ArticleId);
                if (!string.IsNullOrEmpty(status))
                    query = query.Where(x => x.Publish == status);
                return query.ToList();
            }
        }

        public int Insert(int articleid, string name, string text, string email, int is_reply, string publish)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleComments_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@articleid", articleid);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@text", text);
            cmd.Parameters.AddWithValue("@is_reply", is_reply);
            cmd.Parameters.AddWithValue("@publish", publish);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;

        }

        public int UpdateStatus(string userid, int idx, string status)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleComments_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@publish", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(string userid, int idx)
        {           
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleComments_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }
    }
}
