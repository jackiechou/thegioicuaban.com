using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using CommonLibrary;
using System.Data;
using System.Web;
using CommonLibrary.Common;
using CommonLibrary.Modules;

namespace MediaLibrary
{
    public class MediaFiles
    {
        ModuleClass module_obj = new ModuleClass();
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();

        public MediaFiles() { }

        //================================================================================================     

        public static List<Media_Files> GetActiveListByTypeIdFixedNum(int TypeId, int iTotalItemCount)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                var query = from x in dbContext.Media_Files join f in dbContext.Media_FileRefs 
                                                            on x.FileId equals f.FileId                            
                            where x.Status =="1" && f.TypeId == TypeId orderby x.FileId descending select x;
                int TotalItemCount = 0;
                if (query.Count() > 0)
                {
                    if (iTotalItemCount <= query.Count())
                        TotalItemCount = iTotalItemCount;
                    else
                        TotalItemCount = query.Count();
                }
                return query.Take(TotalItemCount).ToList();
            }
        }
        //================================================================================

        public string GetPhotoById(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_GetPhotoById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public string GetThumbnailById(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_GetThumbnailById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public string GetFileNameById(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_GetFileNameById]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable GetDetailById(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@idx", idx);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetSelectedItem(int TypeId)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_GetSelectedItem]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TypeId", TypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByTypeId(int TypeId)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_GetSelectedItem]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@TypeId", TypeId);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByVendorIdTopicIdTypeIdStatus(string vendorid, string topicid, string typeid, string status)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_GetListByVendorIdTopicIdTypeIdStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@vendorid", vendorid);
            cmd.Parameters.AddWithValue("@typeid", typeid);
            cmd.Parameters.AddWithValue("@topicid", topicid);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int SelectItem(string userid, int idx)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_Select]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
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

        //INSERT- UPDATE - DELETE 
        public int Insert(string userid, int vendorid, int typeid, int topicid, int playlistid, int albumid, int artistid, int composerid, string filename, string fileurl, string title, string description,
                           int autostart, int medialoop, string dimension, string source, string photo, string thumbnail, string status)
        {
            string alias = module_obj.createTags(title);
            SqlCommand cmd = new SqlCommand("[Media].[Files_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@vendorid", vendorid);
            cmd.Parameters.AddWithValue("@typeid", typeid);
            cmd.Parameters.AddWithValue("@topicid", topicid);
            cmd.Parameters.AddWithValue("@playlistid", playlistid);
            cmd.Parameters.AddWithValue("@albumid", albumid);
            cmd.Parameters.AddWithValue("@artistid", artistid);
            cmd.Parameters.AddWithValue("@composerid", composerid);
            cmd.Parameters.AddWithValue("@filename", filename);
            cmd.Parameters.AddWithValue("@alias", alias);
            cmd.Parameters.AddWithValue("@fileurl", fileurl);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@autostart", autostart);
            cmd.Parameters.AddWithValue("@medialoop", medialoop);
            cmd.Parameters.AddWithValue("@dimension", dimension);
            cmd.Parameters.AddWithValue("@source", source);
            cmd.Parameters.AddWithValue("@photo", photo);
            cmd.Parameters.AddWithValue("@thumbnail", thumbnail);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int idx, string userid, int typeid, int topicid, int playlistid, int albumid, int artistid, int composerid, string filename, string fileurl, string title, string description,
                           int autostart, int medialoop, string dimension, string source, string photo, string thumbnail, string status)
        {
            string alias = module_obj.createTags(title);
    
            SqlCommand cmd = new SqlCommand("[Media].[Files_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@typeid", typeid);
            cmd.Parameters.AddWithValue("@topicid", topicid);
            cmd.Parameters.AddWithValue("@playlistid", playlistid);
            cmd.Parameters.AddWithValue("@albumid", albumid);
            cmd.Parameters.AddWithValue("@artistid", artistid);
            cmd.Parameters.AddWithValue("@composerid", composerid);
            cmd.Parameters.AddWithValue("@filename", filename);
            cmd.Parameters.AddWithValue("@alias", alias);
            cmd.Parameters.AddWithValue("@fileurl", fileurl);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@autostart", autostart);
            cmd.Parameters.AddWithValue("@medialoop", medialoop);
            cmd.Parameters.AddWithValue("@dimension", dimension);
            cmd.Parameters.AddWithValue("@source", source);
            cmd.Parameters.AddWithValue("@photo", photo);
            cmd.Parameters.AddWithValue("@thumbnail", thumbnail);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(string userid, int idx, string status)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
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

        public int UpdateAllStatus(string userid, string status)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_UpdateAllStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip); 
            cmd.Parameters.AddWithValue("@status", status);      
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateSortKey(string userid, int idx, int sortkey)
        {
            SqlCommand cmd = new SqlCommand("[Media].[Files_UpdateSortKey]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@idx", idx);
            cmd.Parameters.AddWithValue("@sortkey", sortkey);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }


        public int Delete(string userid, int idx, string photo, string thumbnail, string filename)
        {
            // Photo =============================================================================================================================================================
            string photo_dir_path = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]) + "/media_images/photo";
            if (photo != string.Empty)
                module_obj.deleteFile(photo, photo_dir_path);

            // Thumbnail  ========================================================================================================================================================
            string thumbnail_dir_path = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["upload_image_dir"]) + "/media_images/thumbnails";
            if (thumbnail != string.Empty)
                module_obj.deleteFile(thumbnail, thumbnail_dir_path);

            //Media File  ========================================================================================================================================================
            string file_path = string.Empty;
            MediaFiles media_obj = new MediaFiles();
            DataTable dt = media_obj.GetDetailById(idx);
            if (dt.Rows.Count > 0)
            {
                int TypeId = Convert.ToInt32(dt.Rows[0]["TypeId"].ToString());
                MediaTypes media_type_obj = new MediaTypes();
                string TypePath = media_type_obj.GetTypePathByTypeId(TypeId);
                file_path = HttpContext.Current.Server.MapPath("~/" + TypePath);
            }
            module_obj.deleteFile(filename, file_path);
            //===================================================================================================================================================================

            SqlCommand cmd = new SqlCommand("[Media].[Files_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
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

        public int RemoveSelection(string userid, int idx)
        {
            string dir_path = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["upload_background_audio_dir"]);
            string filename = GetFileNameById(idx);
            if (filename != null && filename != string.Empty)
            {
                module_obj.deleteFile(filename, dir_path);
            }

            SqlCommand cmd = new SqlCommand("[Media].[Files_RemoveSelection]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
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
