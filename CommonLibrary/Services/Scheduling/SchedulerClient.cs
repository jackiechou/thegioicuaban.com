using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.Threading;

namespace CommonLibrary.Services.Scheduling
{
    public abstract class SchedulerClient
    {
        public event WorkStarted ProcessStarted;
        public event WorkProgressing ProcessProgressing;
        public event WorkCompleted ProcessCompleted;
        public event WorkErrored ProcessErrored;
        public void Started()
        {
            if (ProcessStarted != null)
            {
                ProcessStarted(this);
            }
        }
        public void Progressing()
        {
            if (ProcessProgressing != null)
            {
                ProcessProgressing(this);
            }
        }
        public void Completed()
        {
            if (ProcessCompleted != null)
            {
                ProcessCompleted(this);
            }
        }
        public void Errored(ref Exception objException)
        {
            if (ProcessErrored != null)
            {
                ProcessErrored(this, objException);
            }
        }
        public abstract void DoWork();
        private string _SchedulerEventGUID;
        private string _ProcessMethod;
        private string _Status;
        private ScheduleHistoryItem _ScheduleHistoryItem;
        public SchedulerClient()
        {
            _SchedulerEventGUID = Null.NullString;
            _ProcessMethod = Null.NullString;
            _Status = Null.NullString;
            _ScheduleHistoryItem = new ScheduleHistoryItem();
        }
        public ScheduleHistoryItem ScheduleHistoryItem
        {
            get { return _ScheduleHistoryItem; }
            set { _ScheduleHistoryItem = value; }
        }
        public string SchedulerEventGUID
        {
            get { return _SchedulerEventGUID; }
            set { _SchedulerEventGUID = value; }
        }
        public string aProcessMethod
        {
            get { return _ProcessMethod; }
            set { _ProcessMethod = value; }
        }
        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        public int ThreadID
        {
            get { return Thread.CurrentThread.ManagedThreadId; }
        }
    }
}
