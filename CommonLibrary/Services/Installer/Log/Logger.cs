using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Installer.Log
{
    public class Logger
    {
        private string _ErrorClass;
        private bool _HasWarnings;
        private string _HighlightClass;
        private List<LogEntry> _Logs;
        private string _NormalClass;
        private bool _Valid;
        public Logger()
        {
            _Logs = new List<LogEntry>();
            _Valid = true;
            _HasWarnings = Null.NullBoolean;
        }
        public string ErrorClass
        {
            get
            {
                if (String.IsNullOrEmpty(_ErrorClass))
                {
                    _ErrorClass = "NormalRed";
                }
                return _ErrorClass;
            }
            set { _ErrorClass = value; }
        }
        public bool HasWarnings
        {
            get { return _HasWarnings; }
        }
        public string HighlightClass
        {
            get
            {
                if (String.IsNullOrEmpty(_HighlightClass))
                {
                    _HighlightClass = "NormalBold";
                }
                return _HighlightClass;
            }
            set { _HighlightClass = value; }
        }
        public List<LogEntry> Logs
        {
            get { return _Logs; }
        }
        public string NormalClass
        {
            get
            {
                if (String.IsNullOrEmpty(_NormalClass))
                {
                    _NormalClass = "Normal";
                }
                return _NormalClass;
            }
            set { _NormalClass = value; }
        }
        public bool Valid
        {
            get { return _Valid; }
        }
        public void AddFailure(string failure)
        {
            _Logs.Add(new LogEntry(LogType.Failure, failure));
            _Valid = false;
        }
        public void AddFailure(Exception ex)
        {
            AddFailure((Util.EXCEPTION + ex.ToString()));
        }
        public void AddInfo(string info)
        {
            _Logs.Add(new LogEntry(LogType.Info, info));
        }
        public void AddWarning(string warning)
        {
            _Logs.Add(new LogEntry(LogType.Warning, warning));
            _HasWarnings = true;
        }
        public void EndJob(string job)
        {
            _Logs.Add(new LogEntry(LogType.EndJob, job));
        }
        public HtmlTable GetLogsTable()
        {
            HtmlTable arrayTable = new HtmlTable();
            foreach (LogEntry entry in Logs)
            {
                HtmlTableRow tr = new HtmlTableRow();
                HtmlTableCell tdType = new HtmlTableCell();
                tdType.InnerText = Util.GetLocalizedString("LOG.PALogger." + entry.Type.ToString());
                HtmlTableCell tdDescription = new HtmlTableCell();
                tdDescription.InnerText = entry.Description;
                tr.Cells.Add(tdType);
                tr.Cells.Add(tdDescription);
                switch (entry.Type)
                {
                    case LogType.Failure:
                    case LogType.Warning:
                        tdType.Attributes.Add("class", ErrorClass);
                        tdDescription.Attributes.Add("class", ErrorClass);
                        break;
                    case LogType.StartJob:
                    case LogType.EndJob:
                        tdType.Attributes.Add("class", HighlightClass);
                        tdDescription.Attributes.Add("class", HighlightClass);
                        break;
                    case LogType.Info:
                        tdType.Attributes.Add("class", NormalClass);
                        tdDescription.Attributes.Add("class", NormalClass);
                        break;
                }
                arrayTable.Rows.Add(tr);
                if (entry.Type == LogType.EndJob)
                {
                    HtmlTableRow SpaceTR = new HtmlTableRow();
                    HtmlTableCell SpaceTD = new HtmlTableCell();
                    SpaceTD.ColSpan = 2;
                    SpaceTD.InnerHtml = "&nbsp;";
                    SpaceTR.Cells.Add(SpaceTD);
                    arrayTable.Rows.Add(SpaceTR);
                }
            }
            return arrayTable;
        }
        public void ResetFlags()
        {
            _Valid = true;
        }
        public void StartJob(string job)
        {
            _Logs.Add(new LogEntry(LogType.StartJob, job));
        }
    }
}
