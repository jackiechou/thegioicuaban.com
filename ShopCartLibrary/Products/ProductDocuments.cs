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
    public class ProductDocuments
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductDocuments()
        {
        }

        public DataTable GetAll()
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetAll]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetDetails(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetDetails]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_ID", idx);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetFile(int idx)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetFile]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_ID", idx);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        

        public DataTable GetListByCode(string Document_Code)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetListByCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_Code", Document_Code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }       

        public DataTable GetActiveList()
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetActiveList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetPublishListByCode(string Document_Code)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetPublishListByCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_Code", Document_Code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByStatus(string Document_Status)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetListByDocument_Status]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_Status", Document_Status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByCodeStatus(string Document_Code, string Document_Status)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetListByCodeStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_Code", Document_Code);
            cmd.Parameters.AddWithValue("@Document_Status", Document_Status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetActiveListByCode(string Document_Code)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_GetActiveListByCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_Code", Document_Code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

      
        public int Insert(string Document_Code, string Document_Name, string Document_File, string Document_Description, string Document_Status)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_Code", Document_Code);
            cmd.Parameters.AddWithValue("@Document_Name", Document_Name);
            cmd.Parameters.AddWithValue("@Document_File", Document_File);
            cmd.Parameters.AddWithValue("@Document_Description", Document_Description);
            cmd.Parameters.AddWithValue("@Document_Status", Document_Status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int Document_ID, string Document_Code, string Document_Name, string Document_File, string Document_Description, string Document_Status)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_ID", Document_ID);
            cmd.Parameters.AddWithValue("@Document_Code", Document_Code);
            cmd.Parameters.AddWithValue("@Document_Name", Document_File);
            cmd.Parameters.AddWithValue("@Document_File", Document_ID);
            cmd.Parameters.AddWithValue("@Document_Description", Document_Description);
            cmd.Parameters.AddWithValue("@Document_Status", Document_Status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(int Document_ID, string Document_Status)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_ID", Document_ID);
            cmd.Parameters.AddWithValue("@Document_Status", Document_Status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int Document_ID, string document_dir_path)
        {
            string dir_path = HttpContext.Current.Server.MapPath(document_dir_path);
            if (System.IO.Directory.Exists(dir_path))
            {
                string _file = GetFile(Document_ID);
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
            SqlCommand cmd = new SqlCommand("[Production].[Product_Documents_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Document_ID", Document_ID);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;         
        }    
    }
}
