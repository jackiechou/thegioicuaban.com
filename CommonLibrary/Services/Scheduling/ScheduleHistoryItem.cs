using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Scheduling
{
    [Serializable()]
    public class ScheduleHistoryItem : ScheduleItem
    {
        private int _ScheduleHistoryID;
        private System.DateTime _StartDate;
        private System.DateTime _EndDate;
        private bool _Succeeded;
        private System.Text.StringBuilder _LogNotes;
        private string _Server;
        public ScheduleHistoryItem()
        {
            _ScheduleHistoryID = Null.NullInteger;
            _StartDate = Null.NullDate;
            _EndDate = Null.NullDate;
            _Succeeded = Null.NullBoolean;
            _LogNotes = new System.Text.StringBuilder();
            _Server = Null.NullString;
        }
        public ScheduleHistoryItem(ScheduleItem objScheduleItem)
        {
            this.AttachToEvent = objScheduleItem.AttachToEvent;
            this.CatchUpEnabled = objScheduleItem.CatchUpEnabled;
            this.Enabled = objScheduleItem.Enabled;
            this.NextStart = objScheduleItem.NextStart;
            this.ObjectDependencies = objScheduleItem.ObjectDependencies;
            this.ProcessGroup = objScheduleItem.ProcessGroup;
            this.RetainHistoryNum = objScheduleItem.RetainHistoryNum;
            this.RetryTimeLapse = objScheduleItem.RetryTimeLapse;
            this.RetryTimeLapseMeasurement = objScheduleItem.RetryTimeLapseMeasurement;
            this.ScheduleID = objScheduleItem.ScheduleID;
            this.ScheduleSource = objScheduleItem.ScheduleSource;
            this.ThreadID = objScheduleItem.ThreadID;
            this.TimeLapse = objScheduleItem.TimeLapse;
            this.TimeLapseMeasurement = objScheduleItem.TimeLapseMeasurement;
            this.TypeFullName = objScheduleItem.TypeFullName;
            this.Servers = objScheduleItem.Servers;
            this.FriendlyName = objScheduleItem.FriendlyName;
            _ScheduleHistoryID = Null.NullInteger;
            _StartDate = Null.NullDate;
            _EndDate = Null.NullDate;
            _Succeeded = Null.NullBoolean;
            _LogNotes = new System.Text.StringBuilder();
            _Server = Null.NullString;
        }
        public double ElapsedTime
        {
            get
            {
                try
                {
                    if (_EndDate == Null.NullDate && _StartDate != Null.NullDate)
                    {
                        return DateTime.Now.Subtract(_StartDate).TotalSeconds;
                    }
                    else if (_StartDate != Null.NullDate)
                    {
                        return _EndDate.Subtract(_StartDate).TotalSeconds;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        public System.DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        public string LogNotes
        {
            get { return _LogNotes.ToString(); }
            set { _LogNotes = new System.Text.StringBuilder(value); }
        }
        public bool Overdue
        {
            get
            {
                if (NextStart < DateTime.Now && EndDate == Null.NullDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public double OverdueBy
        {
            get
            {
                try
                {
                    if (NextStart <= DateTime.Now && EndDate == Null.NullDate)
                    {
                        return DateTime.Now.Subtract(NextStart).TotalSeconds;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        public double RemainingTime
        {
            get
            {
                try
                {
                    if (NextStart > DateTime.Now && EndDate == Null.NullDate)
                    {
                        return NextStart.Subtract(DateTime.Now).TotalSeconds;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int ScheduleHistoryID
        {
            get { return _ScheduleHistoryID; }
            set { _ScheduleHistoryID = value; }
        }
        public string Server
        {
            get { return _Server; }
            set { _Server = value; }
        }
        public System.DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        public bool Succeeded
        {
            get { return _Succeeded; }
            set { _Succeeded = value; }
        }
        public void AddLogNote(string Notes)
        {
            _LogNotes.Append(Notes + Environment.NewLine);
        }
        public override void Fill(System.Data.IDataReader dr)
        {
            ScheduleHistoryID = Null.SetNullInteger(dr["ScheduleHistoryID"]);
            StartDate = Null.SetNullDateTime(dr["StartDate"]);
            EndDate = Null.SetNullDateTime(dr["EndDate"]);
            Succeeded = Null.SetNullBoolean(dr["Succeeded"]);
            LogNotes = Null.SetNullString(dr["LogNotes"]);
            Server = Null.SetNullString(dr["Server"]);
            base.FillInternal(dr);
        }
    }
}
