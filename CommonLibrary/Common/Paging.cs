using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Library.Common
{
    public class Paging
    {
        public int PageFirst, PageLast, TotalPage, TotalRecord, Total, First, Last;
        public int RecordPerPage = 10;
        public int PageCount = 6;
        public int Pages = -1;

        public int FirstOfPage(int Pages)
        {
            if (Pages <= 0)
            {
                Pages = 1;
            }
            First = (Pages - 1) * RecordPerPage + 1;
            return First;
        }

        public int FirstOfPage()
        {
            if (Pages <= 0)
            {
                Pages = 1;
            }
            First = (Pages - 1) * RecordPerPage + 1;
            return First;
        }

        public string setPage(string sParam, int Language)
        {
            string str = "";
            int MidPage;
            string sURL = HttpContext.Current.Request.Url.AbsolutePath + sParam;
            string[] text = { "Xem tiếp", "Next Page", "" };
            string[] f = { "Trang đầu", "First", "<img src='WebMedia/Images/resultset_first.png' alt='Trang đầu' width='10' height='10' />" };
            string[] l = { "Trang cuối", "Last", "<img src='WebMedia/Images/resultset_last.png' alt='Trang cuối' width='10' height='10' />" };

            if (Language > text.Length - 1)
                Language = 0;

            int TotalPage;
            if ((TotalRecord % RecordPerPage) > 0)
            {
                TotalPage = (TotalRecord / RecordPerPage) + 1;
            }
            else
            {
                TotalPage = TotalRecord / RecordPerPage;
            }
            MidPage = (PageCount / 2);
            int i = 0;
            str = "<span style='font-weight:bold;padding:5px 2px 5px 2px;'>" + text[Language] + "</span>";

            if (Pages < MidPage + 1)
            {
                if (TotalPage > PageCount)
                {
                    for (i = 1; i <= PageCount; i++)
                    {
                        if (Pages == i)
                        {
                            str = str + "<span class='currentpage'>" + Pages.ToString() + "</span>";
                        }
                        else
                        {
                            str = str + "<a class='paging' href='" + sURL + "&pages=" + i.ToString() + "'>" + i.ToString() + "</a>";
                        }
                    }
                    str = str + "<span class='dotpage'>...</span><a class='paging' href='" + sURL + "&pages=" + TotalPage.ToString() + "'>" + l[Language] + "</a>";
                }
                else
                {
                    for (i = 1; i <= TotalPage; i++)
                    {
                        if (Pages == i)
                        {
                            str = str + "<span class='currentpage'>" + Pages.ToString() + "</span>";
                        }
                        else
                        {
                            str = str + "<a class='paging' href='" + sURL + "&pages=" + i.ToString() + "'>" + i.ToString() + "</a>";
                        }
                    }
                }
            }
            else if (Pages > TotalPage - (MidPage + 1))
            {
                if (TotalPage > PageCount)
                {
                    str = str + "<a class='paging' href='" + sURL + "&pages=1'>" + f[Language] + "</a><span class='dotpage'>...</span>";
                    for (i = TotalPage - (PageCount - 1); i <= TotalPage; i++)
                    {
                        if (Pages == i)
                        {
                            str = str + "<span class='currentpage'>" + Pages.ToString() + "</span>";
                        }
                        else
                        {
                            str = str + "<a class='paging' href='" + sURL + "&pages=" + i.ToString() + "'>" + i.ToString() + "</a>";
                        }
                    }
                }
                else
                {
                    for (i = 1; i <= TotalPage; i++)
                    {
                        if (Pages == i)
                        {
                            str = str + "<span class='currentpage'>" + Pages.ToString() + "</span>";
                        }
                        else
                        {
                            str = str + "<a class='paging' href='" + sURL + "&pages=" + i.ToString() + "'>" + i.ToString() + "</a>";
                        }
                    }
                }

            }
            else
            {
                str = str + "<a class='paging' href='" + sURL + "&pages=1'>" + f[Language] + "</a><span class='dotpage'>...</span>";
                for (i = Pages - (MidPage - 1); i <= Pages + MidPage; i++)
                {
                    if (Pages == i)
                    {
                        str = str + "<span class='currentpage'>" + Pages.ToString() + "</span>";
                    }
                    else
                    {
                        str = str + "<a class='paging' href='" + sURL + "&pages=" + i.ToString() + "'>" + i.ToString() + "</a>";
                    }
                }
                str = str + "<span class='dotpage'>...</span><a class='paging' href='" + sURL + "&pages=" + TotalPage.ToString() + "'>" + l[Language] + "</a>";
            }
            return str;
        }
    }
}
