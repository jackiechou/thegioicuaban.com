using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web;
using CommonLibrary;
using System.Data.SqlClient;
using CommonLibrary.Modules;

namespace ShopCartLibrary.Products
{
    public class ProductFiles
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductFiles(){}

        public string GetFile(int File_Id)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Files_GetFile]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@File_Id", File_Id);
            cmd.Parameters.Add(new SqlParameter("@File_Name", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@File_Name"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable GetListByStatus(int File_Published)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Files_GetListByStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@File_Published", File_Published);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByProductNo(string Product_No, int File_Published)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Files_GetListByProductNo]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            cmd.Parameters.AddWithValue("@File_Published", File_Published);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int File_Id)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Files_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@File_Id", File_Id);            
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public int Insert(string Product_No, string File_Name, string File_Title, string File_Description,
            string File_Extension, string File_Url, int File_Published, int File_IsImage, int File_Image_Height,
            int File_Image_Width, int File_Image_Thumb_Height, int File_Image_Thumb_Width)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Files_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            cmd.Parameters.AddWithValue("@File_Name", File_Name);
            cmd.Parameters.AddWithValue("@File_Title", File_Title);
            cmd.Parameters.AddWithValue("@File_Description", File_Description);
            cmd.Parameters.AddWithValue("@File_Extension", File_Extension);
            cmd.Parameters.AddWithValue("@File_Url", File_Url);
            cmd.Parameters.AddWithValue("@File_Published", File_Published);
            cmd.Parameters.AddWithValue("@File_IsImage", File_IsImage);
            cmd.Parameters.AddWithValue("@File_Image_Height", File_Image_Height);
            cmd.Parameters.AddWithValue("@File_Image_Width", File_Image_Width);
            cmd.Parameters.AddWithValue("@File_Image_Thumb_Height", File_Image_Thumb_Height);
            cmd.Parameters.AddWithValue("@File_Image_Thumb_Width", File_Image_Thumb_Width);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int File_Id, string Product_No, string File_Name, string File_Title, string File_Description,
            string File_Extension, string File_Url, int File_Published, int File_IsImage, int File_Image_Height,
            int File_Image_Width, int File_Image_Thumb_Height, int File_Image_Thumb_Width)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Files_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Product_No", Product_No);
            cmd.Parameters.AddWithValue("@File_Id", File_Id);
            cmd.Parameters.AddWithValue("@File_Name", File_Name);
            cmd.Parameters.AddWithValue("@File_Title", File_Title);
            cmd.Parameters.AddWithValue("@File_Description", File_Description);
            cmd.Parameters.AddWithValue("@File_Extension", File_Extension);
            cmd.Parameters.AddWithValue("@File_Url", File_Url);
            cmd.Parameters.AddWithValue("@File_Published", File_Published);
            cmd.Parameters.AddWithValue("@File_IsImage", File_IsImage);
            cmd.Parameters.AddWithValue("@File_Image_Height", File_Image_Height);
            cmd.Parameters.AddWithValue("@File_Image_Width", File_Image_Width);
            cmd.Parameters.AddWithValue("@File_Image_Thumb_Height", File_Image_Thumb_Height);
            cmd.Parameters.AddWithValue("@File_Image_Thumb_Width", File_Image_Thumb_Width);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(int File_Id, int File_Published)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Files_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@File_Id", File_Id);
            cmd.Parameters.AddWithValue("@File_Published", File_Published);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int File_Id, string document_dir_path)
        {
            string dir_path = HttpContext.Current.Server.MapPath(document_dir_path);
            if (System.IO.Directory.Exists(dir_path))
            {
                string _file = GetFile(File_Id);
                if (_file != null || _file != string.Empty)
                {
                    string file_path = dir_path + "/" + _file;
                    if (System.IO.File.Exists(file_path))
                    {
                        FileHandleClass file_handle = new FileHandleClass();
                        file_handle.deleteFile(_file, dir_path);
                    }
                }
            }

            SqlCommand cmd = new SqlCommand("[Production].[Product_Files_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@File_Id", File_Id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        } 
    }
}
