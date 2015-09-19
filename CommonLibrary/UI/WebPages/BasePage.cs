using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace CommonLibrary.UI.WebPages
{
    public class BasePage : System.Web.UI.Page
    {
        // Gets or sets the Meta Keywords tag for the page
        private string _keywords;
        public string Meta_Keywords
        {
            get { return _keywords; }
            set
            { // strip out any excessive white-space, newlines and linefeeds
                _keywords = Regex.Replace(value, "\\s+", " ");
            }
        }

        // Gets or sets the Meta Description tag for the page
        private string _description;
        public string Meta_Description
        {
            get { return _description; }
            set
            {// strip out any excessive white-space, newlines and linefeeds
                _description = Regex.Replace(value, "\\s+", " ");
            }
        }
        
        public string BaseUrl
        {
            get
            {
                string url = this.Request.ApplicationPath;
                if (url.EndsWith("/"))
                {
                    return url;
                }
                else
                {
                    return url + "/";
                }
            }
        }

        public BasePage()
        {
            Init += new EventHandler(BasePage_Init);
        }

        void BasePage_Init(object sender, EventArgs e)
        {

            if (!String.IsNullOrEmpty(Meta_Keywords))
            {
                HtmlMeta tag = new HtmlMeta();
                tag.Name = "keywords";
                tag.Content = Meta_Keywords;
                Header.Controls.Add(tag);
            }

            if (!String.IsNullOrEmpty(Meta_Description))
            {
                HtmlMeta tag = new HtmlMeta();
                tag.Name = "description";
                tag.Content = Meta_Description;
                Header.Controls.Add(tag);
            }
        }

       
        //protected override void OnPreInit(EventArgs e)
        //{
        //    base.OnInit(e);

        //   if (!String.IsNullOrEmpty(Meta_Keywords))
        //        {
        //        HtmlMeta tag = new HtmlMeta();
        //        tag.Name = "keywords";
        //        tag.Content = Meta_Keywords;
        //        Header.Controls.Add(tag);
        //        }

        //    if (!String.IsNullOrEmpty(Meta_Description))
        //        {
        //        HtmlMeta tag = new HtmlMeta();
        //        tag.Name = "description";
        //        tag.Content = Meta_Description;
        //        Header.Controls.Add(tag);
        //        }
        //    }
        //}  

        //protected override void OnLoad(EventArgs e)
        //{
        //    // If there is not an explicitly set page title, then set it automatically
        //    if (string.IsNullOrEmpty(Page.Title) || string.Compare(Page.Title, "Untitled Page", true) == 0)
        //    {
        //        // Set the page's Title based on the site map
        //        SiteMapNode currentNode = SiteMap.CurrentNode;

        //        string pageTitle = string.Empty;

        //        if (currentNode != null)
        //        {
        //            // The current page exists in the site map... build up the title based on the site map info
        //            pageTitle = currentNode.Title;

        //            // Walk up the site map to the root, prepending each node's title to the page title
        //            currentNode = currentNode.ParentNode;
        //            while (currentNode != null)
        //            {
        //                pageTitle = string.Concat(currentNode.Title, " : ", pageTitle);

        //                currentNode = currentNode.ParentNode;
        //            }
        //        }
        //        else
        //        {
        //            // This page is not in the site map, use the filename
        //            pageTitle = Path.GetFileNameWithoutExtension(Request.PhysicalPath);
        //        }

        //        // Set the page title
        //        Page.Title = pageTitle;
        //    }
        //    base.OnLoad(e);
        //}

     

      

        #region LOAD USER CONTROL ON WEB PAGE ========================================================
        public static string RenderView(string path)
        {
            return RenderView(path, null);
        }

        public static string RenderView(string path, object data)
        {
            Page pageHolder = new Page();
            UserControl viewControl = (UserControl)pageHolder.LoadControl(path);

            if (data != null)
            {
                Type viewControlType = viewControl.GetType();
                System.Reflection.FieldInfo field = viewControlType.GetField("Data");

                if (field != null)
                {
                    field.SetValue(viewControl, data);
                }
                else
                {
                    throw new Exception("View file: " + path + " does not have a public Data property");
                }
            }

            pageHolder.Controls.Add(viewControl);

            StringWriter output = new StringWriter();
            HttpContext.Current.Server.Execute(pageHolder, output, false);

            return output.ToString();
        }
        #endregion ================================================================================

        private static string[] aspNetFormElements = new string[] 
        { 
            "__EVENTTARGET",
            "__EVENTARGUMENT",
            "__VIEWSTATE",
            "__EVENTVALIDATION",
            "__VIEWSTATEENCRYPTED",
        };


        protected override void Render(HtmlTextWriter writer)
        {
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            base.Render(htmlWriter);
            string html = stringWriter.ToString();
            int formStart = html.IndexOf("<form");
            int endForm = -1;
            if (formStart >= 0)
                endForm = html.IndexOf(">", formStart);

            if (endForm >= 0)
            {
                StringBuilder viewStateBuilder = new StringBuilder();
                foreach (string element in aspNetFormElements)
                {
                    int startPoint = html.IndexOf("<input type=\"hidden\" name=\"" + element + "\"");
                    if (startPoint >= 0 && startPoint > endForm)
                    {
                        int endPoint = html.IndexOf("/>", startPoint);
                        if (endPoint >= 0)
                        {
                            endPoint += 2;
                            string viewStateInput = html.Substring(startPoint, endPoint - startPoint);
                            html = html.Remove(startPoint, endPoint - startPoint);
                            viewStateBuilder.Append(viewStateInput).Append("\r\n");
                        }
                    }
                }
                if (viewStateBuilder.Length > 0)
                {
                    viewStateBuilder.Insert(0, "\r\n");
                    html = html.Insert(endForm + 1, viewStateBuilder.ToString());
                }
            }
            writer.Write(html);
        }       

        //#region Public Properties
        //public new BaseMasterPage Master { get { return (BaseMasterPage)base.Master; } }

        //public string ErrorMessage
        //{
        //    set { Master.ErrorMessage = value; }
        //}

        //public string InformationMessage
        //{
        //    set { Master.InformationMessage = value; }
        //}

        //public string WarningMessage
        //{
        //    set { Master.WarningMessage = value; }
        //}

        //#endregion

    }
}