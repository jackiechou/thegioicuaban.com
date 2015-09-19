using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace CommonLibrary.Common.Controls
{
    public class Form : System.Web.UI.HtmlControls.HtmlForm
    {
        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            base.RenderAttributes(htmlWriter);
            string html = stringWriter.ToString();
            int StartPoint = html.IndexOf("action=\"");
            if (StartPoint >= 0)
            {
                int EndPoint = html.IndexOf("\"", StartPoint + 8) + 1;
                html = html.Remove(StartPoint, EndPoint - StartPoint);
                html = html.Insert(StartPoint, "action=\"" + HttpUtility.HtmlEncode(HttpContext.Current.Request.RawUrl) + "\"");
            }
            if (base.ID != null)
            {
                StartPoint = html.IndexOf("id=\"");
                if (StartPoint >= 0)
                {
                    int EndPoint = html.IndexOf("\"", StartPoint + 4) + 1;
                    html = html.Remove(StartPoint, EndPoint - StartPoint);
                    html = html.Insert(StartPoint, "id=\"" + base.ClientID + "\"");
                }
            }
            writer.Write(html);
        }
    }
}
