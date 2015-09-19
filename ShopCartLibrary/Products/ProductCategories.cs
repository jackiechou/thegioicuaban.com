using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web;
using Library.Common;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using CommonLibrary;
using CommonLibrary.Common;
using CommonLibrary.Modules;

namespace ShopCartLibrary.Products
{
    public class ProductCategories
    {
        ModuleClass module_obj = new ModuleClass();
        string ip = IPNetworking.GetIP4Address();
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable(); 

        public ProductCategories(){}        

        public DataTable GetAll()
        {          
           SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetAll]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
           con.Open();
           using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
           con.Close();
           return dt;
        }

        public DataTable GetDetailByID(int Category_Id)
        {          
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetDetailByID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Id", Category_Id);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }
        
        public DataTable GetDetailByCode(string Category_Code)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetDetailByCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public string GetImageByID(int Category_Id)
        {          
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetImageByID]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Id", Category_Id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            string retunvalue = (string)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public DataTable GetList(int VendorId, string CultureCode)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetListByVendorIdCultureCode]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId", VendorId);
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }      

        public DataTable GetListByStatus(string Status)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetListByStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Status", Status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataSet GetActiveList()
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetActiveList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            con.Close();
            return ds;
        }

        public DataTable GetPublishedList()
        {         
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetPublishedList]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        public DataTable GetListByParentIDStatus(int VendorId, string CultureCode, int Parent_ID, string Status)
        {
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_GetListByParentIDStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@VendorId",VendorId);
            cmd.Parameters.AddWithValue("@CultureCode",CultureCode);
            cmd.Parameters.AddWithValue("@Parent_ID",Parent_ID);
            cmd.Parameters.AddWithValue("@Status",Status);
            con.Open();
            using (var dr = cmd.ExecuteReader()) { dt.Load(dr); }
            con.Close();
            return dt;
        }

        //INSERT- UPDATE - DELETE --------------------------------------------------------------------------       
        public int Insert(string CultureCode, int Parent_ID, string Category_Code, string Category_Name, string Category_Link, string CssClass, string Photo, string BriefDescription, string Description, string Status)
        {
            string Alias = ModuleClass.convertTitle2Link(Category_Name);
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_Insert]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Category_Name", Category_Name);
            cmd.Parameters.AddWithValue("@Category_Link", Category_Link);
            cmd.Parameters.AddWithValue("@CssClass", CssClass);
            cmd.Parameters.AddWithValue("@Alias", Alias);
            cmd.Parameters.AddWithValue("@Parent_ID", Parent_ID);
            cmd.Parameters.AddWithValue("@Photo", Photo);
            cmd.Parameters.AddWithValue("@BriefDescription", BriefDescription);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@Status", Status); 
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Update(int Category_Id, string CultureCode, int Parent_ID, string Category_Code, string Category_Name, string Category_Link, string CssClass, string Photo, string BriefDescription, string Description, string Status)
        {            
            string Alias = ModuleClass.convertTitle2Link(Category_Name);           

           SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_Update]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@CultureCode", CultureCode);
            cmd.Parameters.AddWithValue("@Category_Id", Category_Id);
            cmd.Parameters.AddWithValue("@Category_Code", Category_Code);
            cmd.Parameters.AddWithValue("@Category_Name", Category_Name);
            cmd.Parameters.AddWithValue("@Category_Link", Category_Link);
            cmd.Parameters.AddWithValue("@CssClass", CssClass);
            cmd.Parameters.AddWithValue("@Alias", Alias);
            cmd.Parameters.AddWithValue("@Parent_ID", Parent_ID);
            cmd.Parameters.AddWithValue("@Photo", Photo);
            cmd.Parameters.AddWithValue("@BriefDescription", BriefDescription);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@Status", Status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            int i = cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int UpdateStatus(int Category_Id, string Status)
        {          
            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_UpdateStatus]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Id", Category_Id);
            cmd.Parameters.AddWithValue("@Status", Status);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }

        public int Delete(int Category_Id, string ImgPath)
        {
            string dir_path = HttpContext.Current.Server.MapPath(ImgPath);
            if (System.IO.Directory.Exists(dir_path))
            {
                string img_file = GetImageByID(Category_Id);
                if (img_file != null || img_file !=string.Empty )
                {
                    string file_path=dir_path+"/"+img_file;
                    if(System.IO.File.Exists(file_path))
                    {
                        module_obj.deleteFile(img_file, dir_path);
                    }
                }
            }

            SqlCommand cmd = new SqlCommand("[Production].[Product_Categories_Delete]", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@Category_Id", Category_Id);
            cmd.Parameters.Add(new SqlParameter("@o_return", SqlDbType.Int) { Direction = ParameterDirection.Output });
            con.Open();
            cmd.ExecuteNonQuery();
            int retunvalue = (int)cmd.Parameters["@o_return"].Value;
            con.Close();
            return retunvalue;
        }    
  

        #region MENU PRODUCT CATEGORIES ==========================================================
        public string GenerateXmlFormat()
        {
            DataSet ds = GetActiveList();
            string result = string.Empty;
            if (ds.Tables[0].Rows.Count > 0)
            {
                ds.DataSetName = "Menus";
                ds.Tables[0].TableName = "Production.Product_Categories";

                //create Relation Parent and Child
                DataRelation relation = new DataRelation("ParentChild", ds.Tables[0].Columns["Category_Id"], ds.Tables[0].Columns["Parent_Id"], false);
                relation.Nested = true;
                ds.Relations.Add(relation);

                result = ds.GetXml();
            }
            return result;
        }


        public string ExecuteXSLTransformation(string xslt_filepath)
        {
            string ErrorMsg, HtmlTags = string.Empty, XsltPath;
            MemoryStream DataStream = default(MemoryStream);
            StreamReader streamReader = default(StreamReader);


            byte[] bytes = null;
            //Path of XSLT file
            XsltPath = HttpContext.Current.Server.MapPath(xslt_filepath);

            if (File.Exists(XsltPath))
            {

                //Encode all Xml format string to bytes
                string strXML = GenerateXmlFormat();
                if (strXML != string.Empty)
                {
                    try
                    {

                        bytes = Encoding.UTF8.GetBytes(strXML);

                        DataStream = new MemoryStream(bytes);

                        //Create Xmlreader from memory stream

                        XmlReader reader = XmlReader.Create(DataStream);

                        // Load the XML 
                        XPathDocument document = new XPathDocument(reader);


                        XslCompiledTransform XsltFormat = new XslCompiledTransform();

                        // Load the style sheet.
                        XsltFormat.Load(XsltPath);

                        DataStream = new MemoryStream();

                        XmlTextWriter writer = new XmlTextWriter(DataStream, Encoding.UTF8);


                        //Apply transformation from xml format to html format and save it in xmltextwriter
                        XsltFormat.Transform(document, writer);

                        streamReader = new StreamReader(DataStream);

                        DataStream.Position = 0;

                        HtmlTags = streamReader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg = ex.Message;
                        return ErrorMsg;
                    }
                    finally
                    {
                        //Release the resources 
                        streamReader.Close();
                        DataStream.Close();
                    }
                }
            }
            return HtmlTags;
        }
        #endregion ===============================================================================
    }
}
