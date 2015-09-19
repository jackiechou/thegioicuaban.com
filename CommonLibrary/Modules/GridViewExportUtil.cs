using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// 
/// </summary>
public class GridViewExportUtil
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="gv"></param>
    public static void Export(string fileName, string title,  GridView gv)
    {
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        HttpContext.Current.Response.ContentType = "application/ms-excel";
        //HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
              
        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                //  Create a form to contain the grid
                Table table = new Table();
                table.GridLines = gv.GridLines;

                //  add the header row to the table
                if (gv.HeaderRow != null)
                {
                    gv.HeaderRow.Attributes.Add("class", "header_bg");                     
                    GridViewExportUtil.PrepareControlForExport(gv.HeaderRow); 
                    table.Rows.Add(gv.HeaderRow);
                }

                //  add each of the data rows to the table
                foreach (GridViewRow row in gv.Rows)
                {
                    row.Attributes.Add("class", "sborder"); 
                    GridViewExportUtil.PrepareControlForExport(row);
                    table.Rows.Add(row);
                   
                }

                //  add the footer row to the table
                if (gv.FooterRow != null)
                {
                    GridViewExportUtil.PrepareControlForExport(gv.FooterRow);
                    table.Rows.Add(gv.FooterRow);
                }

                //Color Setttings
                
                //gv.BackColor = System.Drawing.Color.AliceBlue;
                //gv.CellPadding = 50;
                //gv.CellSpacing = 10;
                //gv.CaptionAlign = TableCaptionAlign.Right;
                //gv.HeaderStyle.BackColor = System.Drawing.Color.Black;
                //gv.HeaderStyle.ForeColor = System.Drawing.Color.White;

                htw.WriteLine("<br/><center><span class='heading'>" + title + "<span></center><br/><br/><br/>");

                //htw.WriteLine("<br/><center><b><font size='5' color='#800000'>" + title + "</font></b></center><br/><br/><br/>");
                //  render the table into the htmlwriter
                table.RenderControl(htw);

                //  render the htmlwriter into the response  
                
                HttpContext.Current.Response.Write("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"  xmlns=\"http://www.w3.org/TR/REC-html40\">");
                HttpContext.Current.Response.Write("<head>");
                HttpContext.Current.Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                HttpContext.Current.Response.Write("<meta name=ProgId content=Excel.Sheet>");
                HttpContext.Current.Response.Write("<meta name=Generator content=\"Microsoft Excel 11\">");
                HttpContext.Current.Response.Write("<style>");               
                HttpContext.Current.Response.Write(".heading{background-color:#C0C0C0;font:bold 30px 'Times New Roman,Times,Helvetica Neue,Lucida Grande,Helvetica', Arial, Verdana, serif';text-transform:uppercase;color:#800000;}.header_bg{color:#800000;background-color:#C0C0C0;border:1px Solid #000000;} .sborder { color : #000000;border : 1px Solid #000000; background-color:lightyellow;}");
                HttpContext.Current.Response.Write("<!--table @page{} -->");
                HttpContext.Current.Response.Write("</style>");
                HttpContext.Current.Response.Write("<!--[if gte mso 9]><xml>");
                HttpContext.Current.Response.Write("<x:ExcelWorkbook>");
                HttpContext.Current.Response.Write("<x:ExcelWorksheets><x:ExcelWorksheet>");
                HttpContext.Current.Response.Write("<x:Name>"+title+"</x:Name>");
                HttpContext.Current.Response.Write("<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions>");
                HttpContext.Current.Response.Write("</x:ExcelWorksheet></x:ExcelWorksheets>");
                HttpContext.Current.Response.Write("</x:ExcelWorkbook>");
                HttpContext.Current.Response.Write("</xml>");
                HttpContext.Current.Response.Write("<![endif]-->");
                HttpContext.Current.Response.Write("</head>"); 
                HttpContext.Current.Response.Write("<BODY>");    
                HttpContext.Current.Response.Write(sw.ToString());
                HttpContext.Current.Response.Write("</BODY>");
                HttpContext.Current.Response.Write("</HTML>");
                HttpContext.Current.Response.End();
            }
        }
    }

    /// <summary>
    /// Replace any of the contained controls with literals
    /// </summary>
    /// <param name="control"></param>
    private static void PrepareControlForExport(Control control)
    {
        for (int i = 0; i < control.Controls.Count; i++)
        {
            Control current = control.Controls[i];
            if (current is LinkButton)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
            }
            else if (current is ImageButton)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
            }
            else if (current is HyperLink)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
            }
            else if (current is DropDownList)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
            }
            else if (current is CheckBox)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
            }

            if (current.HasControls())
            {
                GridViewExportUtil.PrepareControlForExport(current);
            }
        }
    }
}
