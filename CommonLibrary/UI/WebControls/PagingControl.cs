using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonLibrary.Common;

namespace CommonLibrary.UI.WebControls //IPostBackEventHandler
{
    public enum PagingControlMode
    {
        PostBack,
        URL
    }

    [ToolboxData("<{0}:PagingControl runat=server></{0}:PagingControl>")]
    public class PagingControl : System.Web.UI.WebControls.WebControl
    {
        protected System.Web.UI.WebControls.Table tablePageNumbers;
        protected System.Web.UI.WebControls.Repeater PageNumbers;
        protected System.Web.UI.WebControls.TableCell cellDisplayStatus;
        protected System.Web.UI.WebControls.TableCell cellDisplayLinks;
        private int TotalPages = -1;
        private int _TotalRecords;
        private PagingControlMode _Mode = PagingControlMode.URL;
        private int _PageSize;
        private int _CurrentPage;
        private string _QuerystringParams;
        private int _TabID;
        private string _CSSClassLinkActive;
        private string _CSSClassLinkInactive;
        private string _CSSClassPagingStatus;
        public event EventHandler PageChanged;


        [Bindable(true), Category("Behavior"), DefaultValue("Normal")]
        public string CSSClassLinkActive
        {
            get
            {
                if (String.IsNullOrEmpty(_CSSClassLinkActive))
                {
                    return "CommandButton";
                }
                else
                {
                    return _CSSClassLinkActive;
                }
            }
            set { _CSSClassLinkActive = value; }
        }

        [Bindable(true), Category("Behavior"), DefaultValue("CommandButton")]
        public string CSSClassLinkInactive
        {
            get
            {
                if (String.IsNullOrEmpty(_CSSClassLinkInactive))
                {
                    return "NormalDisabled";
                }
                else
                {
                    return _CSSClassLinkInactive;
                }
            }
            set { _CSSClassLinkInactive = value; }
        }

        [Bindable(true), Category("Behavior"), DefaultValue("Normal")]
        public string CSSClassPagingStatus
        {
            get
            {
                if (String.IsNullOrEmpty(_CSSClassPagingStatus))
                {
                    return "Normal";
                }
                else
                {
                    return _CSSClassPagingStatus;
                }
            }
            set { _CSSClassPagingStatus = value; }
        }

        [Bindable(true), Category("Behavior"), DefaultValue("1")]
        public int CurrentPage
        {
            get { return _CurrentPage; }
            set { _CurrentPage = value; }
        }
        public PagingControlMode Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        [Bindable(true), Category("Behavior"), DefaultValue("10")]
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("")]
        public string QuerystringParams
        {
            get { return _QuerystringParams; }
            set { _QuerystringParams = value; }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("-1")]
        public int TabID
        {
            get { return _TabID; }
            set { _TabID = value; }
        }
        [Bindable(true), Category("Behavior"), DefaultValue("0")]
        public int TotalRecords
        {
            get { return _TotalRecords; }
            set { _TotalRecords = value; }
        }

        private void BindPageNumbers(int TotalRecords, int RecordsPerPage)
        {
            int PageLinksPerPage = 10;
            if (TotalRecords < 1 || RecordsPerPage < 1)
                return;
            if (TotalRecords / RecordsPerPage >= 1)
            {
                TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(TotalRecords / RecordsPerPage)));
            }
            else
            {
                TotalPages = 0;
            }
            if (TotalPages > 0)
            {
                DataTable ht = new DataTable();
                ht.Columns.Add("PageNum");
                DataRow tmpRow;
                int LowNum = 1;
                int HighNum = Convert.ToInt32(TotalPages);
                double tmpNum;
                tmpNum = CurrentPage - PageLinksPerPage / 2;
                if (tmpNum < 1)
                    tmpNum = 1;
                if (CurrentPage > (PageLinksPerPage / 2))
                {
                    LowNum = Convert.ToInt32(Math.Floor(tmpNum));
                }
                if (Convert.ToInt32(TotalPages) <= PageLinksPerPage)
                {
                    HighNum = Convert.ToInt32(TotalPages);
                }
                else
                {
                    HighNum = LowNum + PageLinksPerPage - 1;
                }
                if (HighNum > Convert.ToInt32(TotalPages))
                {
                    HighNum = Convert.ToInt32(TotalPages);
                    if (HighNum - LowNum < PageLinksPerPage)
                    {
                        LowNum = HighNum - PageLinksPerPage + 1;
                    }
                }
                if (HighNum > Convert.ToInt32(TotalPages))
                    HighNum = Convert.ToInt32(TotalPages);
                if (LowNum < 1)
                    LowNum = 1;
                int i;
                for (i = LowNum; i <= HighNum; i++)
                {
                    tmpRow = ht.NewRow();
                    tmpRow["PageNum"] = i;
                    ht.Rows.Add(tmpRow);
                }
                PageNumbers.DataSource = ht;
                PageNumbers.DataBind();
            }
        }

        private string CreateURL(string CurrentPage)
        {
            if (Mode == PagingControlMode.URL)
            {
                //if (!String.IsNullOrEmpty(QuerystringParams))
                //{
                //    if (!String.IsNullOrEmpty(CurrentPage))
                //    {
                //        return Globals.NavigateURL(TabID, "", QuerystringParams, "currentpage=" + CurrentPage);
                //    }
                //    else
                //    {
                //        return Globals.NavigateURL(TabID, "", QuerystringParams);
                //    }
                //}
                //else
                //{
                //    if (!String.IsNullOrEmpty(CurrentPage))
                //    {
                //        return Globals.NavigateURL(TabID, "", "currentpage=" + CurrentPage);
                //    }
                //    else
                //    {
                //        return Globals.NavigateURL(TabID);
                //    }
                //}
            }
            else
            {
                return this.Page.ClientScript.GetPostBackClientHyperlink(this, "Page_" + CurrentPage.ToString(), false);
            }

            return "aaaaaaaaaaaaaaa";
        }
        private string GetLink(int PageNum)
        {
            if (PageNum == CurrentPage)
            {
                return "<span class=\"" + CSSClassLinkInactive + "\">[" + PageNum.ToString() + "]</span>";
            }
            else
            {
                return "<a href=\"" + CreateURL(PageNum.ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + PageNum.ToString() + "</a>";
            }
        }
        //private string GetPreviousLink()
        //{
        //    if (CurrentPage > 1 && TotalPages > 0) {
        //        return "<a href=\"" + CreateURL((CurrentPage - 1).ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + Services.Localization.Localization.GetString("Previous", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</a>";
        //    } else {
        //        return "<span class=\"" + CSSClassLinkInactive + "\">" + Services.Localization.Localization.GetString("Previous", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</span>";
        //    }
        //}
        //private string GetNextLink()
        //{
        //    if (CurrentPage != TotalPages && TotalPages > 0) {
        //        return "<a href=\"" + CreateURL((CurrentPage + 1).ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + Services.Localization.Localization.GetString("Next", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</a>";
        //    } else {
        //        return "<span class=\"" + CSSClassLinkInactive + "\">" + Services.Localization.Localization.GetString("Next", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</span>";
        //    }
        //}
        //private string GetFirstLink()
        //{
        //    if (CurrentPage > 1 && TotalPages > 0) {
        //        return "<a href=\"" + CreateURL("1") + "\" class=\"" + CSSClassLinkActive + "\">" + Services.Localization.Localization.GetString("First", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</a>";
        //    } else {
        //        return "<span class=\"" + CSSClassLinkInactive + "\">" + Services.Localization.Localization.GetString("First", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</span>";
        //    }
        //}
        //private string GetLastLink()
        //{
        //    if (CurrentPage != TotalPages && TotalPages > 0) {
        //        return "<a href=\"" + CreateURL(TotalPages.ToString()) + "\" class=\"" + CSSClassLinkActive + "\">" + Services.Localization.Localization.GetString("Last", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</a>";
        //    } else {
        //        return "<span class=\"" + CSSClassLinkInactive + "\">" + Services.Localization.Localization.GetString("Last", DotNetNuke.Services.Localization.Localization.SharedResourceFile) + "</span>";
        //    }
        //}
        //protected override void CreateChildControls()
        //{
        //    tablePageNumbers = new System.Web.UI.WebControls.Table();
        //    cellDisplayStatus = new System.Web.UI.WebControls.TableCell();
        //    cellDisplayLinks = new System.Web.UI.WebControls.TableCell();
        //    cellDisplayStatus.CssClass = "Normal";
        //    cellDisplayLinks.CssClass = "Normal";
        //    if (String.IsNullOrEmpty(this.CssClass)) {
        //        tablePageNumbers.CssClass = "PagingTable";
        //    } else {
        //        tablePageNumbers.CssClass = this.CssClass;
        //    }
        //    int intRowIndex = tablePageNumbers.Rows.Add(new TableRow());
        //    PageNumbers = new Repeater();
        //    PageNumberLinkTemplate I = new PageNumberLinkTemplate(this);
        //    PageNumbers.ItemTemplate = I;
        //    BindPageNumbers(TotalRecords, PageSize);
        //    cellDisplayStatus.HorizontalAlign = HorizontalAlign.Left;
        //    cellDisplayStatus.Width = new Unit("50%");
        //    cellDisplayLinks.HorizontalAlign = HorizontalAlign.Right;
        //    cellDisplayLinks.Width = new Unit("50%");
        //    int intTotalPages = TotalPages;
        //    if (intTotalPages == 0)
        //        intTotalPages = 1;
        //    string str;
        //    str = string.Format(Services.Localization.Localization.GetString("Pages"), CurrentPage.ToString(), intTotalPages.ToString());
        //    LiteralControl lit = new LiteralControl(str);
        //    cellDisplayStatus.Controls.Add(lit);
        //    tablePageNumbers.Rows[intRowIndex].Cells.Add(cellDisplayStatus);
        //    tablePageNumbers.Rows[intRowIndex].Cells.Add(cellDisplayLinks);
        //}
        //protected void OnPageChanged(EventArgs e)
        //{
        //    if (PageChanged != null) {
        //        PageChanged(this, e);
        //    }
        //}
        //protected override void Render(System.Web.UI.HtmlTextWriter output)
        //{
        //    if (PageNumbers == null) {
        //        CreateChildControls();
        //    }
        //    System.Text.StringBuilder str = new System.Text.StringBuilder();
        //    str.Append(GetFirstLink() + "&nbsp;&nbsp;&nbsp;");
        //    str.Append(GetPreviousLink() + "&nbsp;&nbsp;&nbsp;");
        //    System.Text.StringBuilder result = new System.Text.StringBuilder(1024);
        //    PageNumbers.RenderControl(new HtmlTextWriter(new System.IO.StringWriter(result)));
        //    str.Append(result.ToString());
        //    str.Append(GetNextLink() + "&nbsp;&nbsp;&nbsp;");
        //    str.Append(GetLastLink() + "&nbsp;&nbsp;&nbsp;");
        //    cellDisplayLinks.Controls.Add(new LiteralControl(str.ToString()));
        //    tablePageNumbers.RenderControl(output);
        //}
        //public void RaisePostBackEvent(string eventArgument)
        //{
        //    CurrentPage = int.Parse(eventArgument.Replace("Page_", ""));
        //    OnPageChanged(new EventArgs());
        //}
        public class PageNumberLinkTemplate : ITemplate
        {
            static int itemcount = 0;
            private PagingControl _PagingControl;
            public PageNumberLinkTemplate(PagingControl ctlPagingControl)
            {
                _PagingControl = ctlPagingControl;
            }
            void ITemplate.InstantiateIn(Control container)
            {
                Literal l = new Literal();
                l.DataBinding += this.BindData;
                container.Controls.Add(l);
            }
            private void BindData(object sender, System.EventArgs e)
            {
                Literal lc;
                lc = (Literal)sender;
                RepeaterItem container;
                container = (RepeaterItem)lc.NamingContainer;
                lc.Text = _PagingControl.GetLink(Convert.ToInt32(DataBinder.Eval(container.DataItem, "PageNum"))) + "&nbsp;&nbsp;";
            }
        }
    }
}