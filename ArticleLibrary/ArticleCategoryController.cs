using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CommonLibrary;
using System.Web;
using CommonLibrary.Common;
using CommonLibrary.Modules;

namespace ArticleLibrary
{  
    public class ArticleCategoryController
    {
         string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public ArticleCategoryController() { }

        public static ArticleCategory GetDetailByCode(string code)
        {
            using (ArticleEntities dbContext = new ArticleEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                var query = (from x in dbContext.ArticleCategories
                             where x.CategoryCode.Trim() == code
                             select x).SingleOrDefault();
                return query;
            }
        }

        public static List<ArticleCategory> GetAllParentNodesBySelectedNodeStatus(string CategoryCode, string CultureCode, string Status)
        {
            using (ArticleEntities dbContext = new ArticleEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                System.Data.Objects.ObjectResult<ArticleCategory> obj_result = dbContext.ArticleCategories_GetAllParentNodesOfSelectedNode(CategoryCode);
                IQueryable<ArticleCategory> query = obj_result.AsQueryable();
                if (!string.IsNullOrEmpty(CultureCode))
                    query = query.Where(x => x.CultureCode.Trim() == CultureCode);
                if (!string.IsNullOrEmpty(Status))
                    query = query.Where(x => x.Status.Trim() == Status);
                return query.ToList();
            }
        }

        public static List<ArticleCategory> GetAllChildrenNodesOfSelectedNode(string CategoryCode, string CultureCode, string Status)
        {
            using (ArticleEntities dbContext = new ArticleEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                System.Data.Objects.ObjectResult<ArticleCategory> obj_result = dbContext.ArticleCategories_GetAllChildrenNodesOfSelectedNode(CategoryCode);
                var query = obj_result.AsQueryable();
                if (!string.IsNullOrEmpty(CultureCode))
                    query = query.Where(x => x.CultureCode.Trim() == CultureCode);
                if (!string.IsNullOrEmpty(Status))
                    query = query.Where(x => x.Status.Trim() == Status);
                return query.ToList();
            }
        }

        public DataSet GetActiveListMenu(int portalid, string culturecode)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetActiveList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@portalid", portalid);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            con.Close();
            return ds;
        }

        public DataTable GetListByParentID()
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetParentList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetAllTreeNodes(int portalid, string culturecode, int node)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetTreeNodes]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@portalid", portalid);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            cmd.Parameters.AddWithValue("@node", node);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByStatus(string status)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetListByStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetActiveList(int portalid, string culturecode)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetActiveList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@portalid", portalid);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetTreeNumListByCateIdStatus(int idx, int num_rows, string status)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetTreeNumListByCateIdStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@num_rows", num_rows);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetTreeListByParentIdStatus(int idx, string status)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetTreeListByParentIdStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);            
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetTreeNumListByParentIdStatus(int parentid, int num_rows, string status)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetTreeNumListByParentIdStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@parentid", parentid);
            cmd.Parameters.AddWithValue("@num_rows", num_rows);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetailByID(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetDetailById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);            
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetImageByID(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_GetImageById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Insert(string userid, int portalid, string culturecode, int parentid, string code, string name, string image, string description, string status)
        {
            string alias = ModuleClass.convertTitle2Link(name);
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@portalid", portalid);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);            
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@alias", alias);
            cmd.Parameters.AddWithValue("@parentid", parentid);
            cmd.Parameters.AddWithValue("@image", image).IsNullable = true;
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int,2) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(string userid, int portalid, string culturecode, int idx, int parentid, string code, string name, string image, string description, string status)
        {
            string alias = ModuleClass.convertTitle2Link(name);
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_Update]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = Settings.CommandTimeout;
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@portalid", portalid);
            cmd.Parameters.AddWithValue("@culturecode", culturecode);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@alias", alias);
            cmd.Parameters.AddWithValue("@parentid", parentid);
            cmd.Parameters.AddWithValue("@image", image).IsNullable = true;
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int,2) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(string userid, int idx, string status)
        {
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateSortKey(int option, int idx)
        {           
            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_UpdateSortKey]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@option", option);            
            cmd.Parameters.AddWithValue("@idx", idx);            
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(string userid, int idx)
        {
            // Write your own Delete statement blocks.        

            string dir_path = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]) + "/article_category_images";
            string file_name = GetImageByID(idx);
            if (file_name != null)
            {
                ModuleClass module_obj = new ModuleClass();
                module_obj.deleteFile(file_name, dir_path);
            }

            SqlCommand cmd = new SqlCommand("[Articles].[sp_ArticleCategories_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
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
