using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace CommonLibrary.Entities.Tabs
{
    public class TabMenu
    {
        SqlConnection con = new SqlConnection(Settings.ConnectionString);
        DataTable dt = new DataTable();
        public TabMenu()
        {

        }

        //Load horizontal menu        
        public string GenerateXmlFormat(int PortalId, string RoleId, byte IsSecure)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetHierarchicalTreeByPortalIdRoleIdIsSecure", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@IsSecure", IsSecure);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            string result = string.Empty;
            if (ds.Tables.Count > 0)
            {
                ds.DataSetName = "Menus";
                ds.Tables[0].TableName = "aspnet_Tabs";
                //create Relation Parent and Child
                DataRelation relation = new DataRelation("ParentChild", ds.Tables[0].Columns["TabId"], ds.Tables[0].Columns["ParentId"], false);
                relation.Nested = true;
                ds.Relations.Add(relation);
                result = ds.GetXml();
            }
            con.Close();
            return result;
        }

        public string ExecuteXSLTransformation(int portalid, string role_id, string xslt_filepath, byte isadmin)
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
                string strXML = GenerateXmlFormat(portalid, role_id, isadmin);
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
                    //finally
                    //{
                    //    //Release the resources 
                    //    streamReader.Close();
                    //    DataStream.Close();
                    //}

                }

            }
            return HtmlTags;
        }

       
        #region FRONT PAGES ============================================================================
         //Load horizontal menu        
        public string GenerateXmlFormat_FrontPage(int PortalId, byte IsSecure)
        {
            SqlCommand cmd = new SqlCommand("aspnet_Tabs_GetListTabNameByPortalId", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = Settings.CommandTimeout };
            cmd.Parameters.AddWithValue("@PortalId", PortalId);
            cmd.Parameters.AddWithValue("@IsSecure", IsSecure);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            string result = string.Empty;
            if (ds.Tables.Count > 0)
            {
                ds.DataSetName = "Menus";
                ds.Tables[0].TableName = "aspnet_Tabs";
                //create Relation Parent and Child
                DataRelation relation = new DataRelation("ParentChild", ds.Tables[0].Columns["TabId"], ds.Tables[0].Columns["ParentId"], false);
                relation.Nested = true;
                ds.Relations.Add(relation);
                result = ds.GetXml();
            }
            con.Close();
            return result;
        }

        public string ExecuteXSLTransformation_FrontPage(int portalid, string xslt_filepath, byte isadmin)
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
                string strXML = GenerateXmlFormat_FrontPage(portalid, isadmin);
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
                    //finally
                    //{
                    //    //Release the resources 
                    //    streamReader.Close();
                    //    DataStream.Close();
                    //}

                }

            }
            return HtmlTags;
        }    
        #endregion =====================================================================================
    }
}
