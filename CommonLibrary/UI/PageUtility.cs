using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.UI
{
    public class PageUtility : System.Web.UI.Page
    {
        #region ============================================================
        //PageUtility.SetFocus(TextBox1);
        //Control c = PageUtility.GetPostBackControl(this.Page);
        //if (c != null){}
        // Render: <meta name="keywords" content="Some words listed here" />
        //HtmlMeta meta = new HtmlMeta();
        //meta.Name = "keywords";
        //meta.Content = "Some words listed here";
        //this.Header.Controls.Add(meta);

        //// Render: <meta name="robots" content="noindex" />
        //meta = new HtmlMeta();
        //meta.Name = "robots";
        //meta.Content = "noindex";
        //this.Header.Controls.Add(meta);

        //// Render: <meta name="date" content="2006-03-25" scheme="YYYY-MM-DD" />
        //meta = new HtmlMeta();
        //meta.Name = "date";
        //meta.Content = DateTime.Now.ToString("yyyy-MM-dd");
        //meta.Scheme = "YYYY-MM-DD";
        //this.Header.Controls.Add(meta);
        #endregion ========================================================


        public static void SetFocus(Control control)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\r\n<script language='JavaScript'>\r\n");
            sb.Append("<!--\r\n");
            sb.Append("function SetFocus()\r\n");
            sb.Append("{\r\n");
            sb.Append("\tdocument.");

            Control p = control.Parent;
            while (!(p is System.Web.UI.HtmlControls.HtmlForm)) p = p.Parent;

            sb.Append(p.ClientID);
            sb.Append("['");
            sb.Append(control.UniqueID);
            sb.Append("'].focus();\r\n");
            sb.Append("}\r\n");
            sb.Append("window.onload = SetFocus;\r\n");
            sb.Append("// -->\r\n");
            sb.Append("</script>");
            
            ScriptManager.RegisterClientScriptBlock(control, control.GetType(),"SetFocus", sb.ToString(),true);
        }

        public static Control GetPostBackControl(Page page)
        {
            Control control = null;

            string ctrlname = page.Request.Params.Get("__EVENTTARGET");
            if (ctrlname != null && ctrlname != string.Empty)
            {
                control = page.FindControl(ctrlname);
            }
            else
            {
                foreach (string ctl in page.Request.Form)
                {
                    Control c = page.FindControl(ctl);
                    if (c is System.Web.UI.WebControls.Button)
                    {
                        control = c;
                        break;
                    }
                }
            }
            return control;
        }
    }
}
