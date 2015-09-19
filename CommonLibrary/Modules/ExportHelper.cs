using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;

namespace CommonLibrary.Modules
{
    public enum ExportType
    {
        CSV, Excel, Word
    }

    public class ExportHelper
    {
        private static void ClearControls(Control control)
        {
            for (int index = control.Controls.Count - 1; index >= 0; index--)
            {
                ClearControls(control.Controls[index]);
            }

            if (!(control is TableCell))
            {
                if (control.GetType().GetProperty("SelectedItem") != null)
                {
                    LiteralControl m_Literal = new LiteralControl();
                    control.Parent.Controls.Add(m_Literal);

                    m_Literal.Text = (string)control.GetType().GetProperty("SelectedItem").GetValue(control, null);
                    control.Parent.Controls.Remove(control);
                }
                else
                {
                    if (control.GetType().GetProperty("Text") != null)
                    {
                        LiteralControl m_Literal = new LiteralControl();
                        control.Parent.Controls.Add(m_Literal);
                        m_Literal.Text = (string)control.GetType().GetProperty("Text").GetValue(control, null);
                        control.Parent.Controls.Remove(control);
                    }
                }
            }
        }

        public static void ExportGridView(GridView gridView, string fileName, ExportType exportType)
        {
            const string m_Http_Attachment = "attachment;filename=";
            const string m_Http_Content = "content-disposition";

            HttpResponse m_Response = HttpContext.Current.Response;

            m_Response.Clear();
            m_Response.ClearContent();
            m_Response.ClearHeaders();
            m_Response.Buffer = true;

            m_Response.AddHeader(m_Http_Content, m_Http_Attachment + fileName);
            m_Response.ContentEncoding = Encoding.UTF8;
            m_Response.Cache.SetCacheability(HttpCacheability.NoCache);

            StringWriter m_StringWriter = new StringWriter();
            HtmlTextWriter m_HtmlWriter = new HtmlTextWriter(m_StringWriter);

            gridView.AllowPaging = false;
            gridView.HeaderStyle.Font.Bold = true;
            gridView.DataBind();

            ClearControls(gridView);
            try
            {
                gridView.RenderControl(m_HtmlWriter);
            }
            catch (Exception ex) { ex.ToString(); }

            string m_gridViewText = m_StringWriter.ToString();

            switch (exportType)
            {
                case ExportType.Excel:
                    ExportHelper.ExportToExcelWord(m_gridViewText, "application/vnd.ms-excel", m_Response);
                    break;

                case ExportType.CSV:
                    ExportHelper.ExportToCsv(m_gridViewText, "application/csv", m_Response);
                    break;

                case ExportType.Word:
                    ExportHelper.ExportToExcelWord(m_gridViewText, "application/vnd.ms-word", m_Response);
                    break;

                default:
                    ExportHelper.ExportToExcelWord(m_gridViewText, "application/vnd.ms-excel", m_Response);
                    break;
            }
        }

        public static void ExportToCsv(string gridViewText, string contentType, HttpResponse response)
        {
            const string m_Delimiter_Column = ",";
            string m_Delimiter_Row = Environment.NewLine;

            response.ContentType = contentType;

            Regex m_RegEx = new Regex(@"(>\s+<)", RegexOptions.IgnoreCase);
            gridViewText = m_RegEx.Replace(gridViewText, "><");

            gridViewText = gridViewText.Replace(m_Delimiter_Row, String.Empty);
            gridViewText = gridViewText.Replace("</td></tr>", m_Delimiter_Row);
            gridViewText = gridViewText.Replace("<tr><td>", String.Empty);
            gridViewText = gridViewText.Replace(m_Delimiter_Column, "\\" + m_Delimiter_Column);
            gridViewText = gridViewText.Replace("</td><td>", m_Delimiter_Column);

            m_RegEx = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase);
            gridViewText = m_RegEx.Replace(gridViewText, String.Empty);

            gridViewText = HttpUtility.HtmlDecode(gridViewText);
            response.Write(gridViewText);
            response.End();
        }

        public static void ExportToExcelWord(string gridViewText, string contentType, HttpResponse response)
        {
            response.ContentType = contentType;
            response.Write(gridViewText);
            response.End();
        }

    }
}
