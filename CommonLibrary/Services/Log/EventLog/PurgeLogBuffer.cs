using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Scheduling;

namespace CommonLibrary.Services.Log.EventLog
{
    public class PurgeLogBuffer : SchedulerClient
    {
        public PurgeLogBuffer(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }
        public override void DoWork()
        {
            try
            {
                this.Progressing();
                LoggingProvider.Instance().PurgeLogBuffer();
                this.ScheduleHistoryItem.Succeeded = true;
                this.ScheduleHistoryItem.AddLogNote("Purged log entries successfully");
            }
            catch (Exception exc)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc.ToString());
                this.Errored(ref exc);
                Exceptions.Exceptions.LogException(exc);
            }
        }
    }
}
