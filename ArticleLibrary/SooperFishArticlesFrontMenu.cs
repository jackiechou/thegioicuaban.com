using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Web;
using System.Data;

namespace ArticleLibrary
{
    public class SooperFishArticlesFrontMenu
    {
        public SooperFishArticlesFrontMenu() { }

        public string GenerateXmlFormat()
        {
            ArticleCategoryController menu_obj = new ArticleCategoryController();
            DataSet ds = menu_obj.GetActiveListMenu(6, "vi-VN");

            string result = string.Empty;
            if (ds.Tables[0].Rows.Count > 0)
            {
                ds.DataSetName = "Menus";
                ds.Tables[0].TableName = "Articles.ArticleCategories";

                //create Relation Parent and Child
                DataRelation relation = new DataRelation("ParentChild", ds.Tables[0].Columns["CategoryId"], ds.Tables[0].Columns["ParentId"], false);
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
    }
}
