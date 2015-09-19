using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Scheduling;

namespace CommonLibrary.Services.Log.EventLog
{
    public class SendLogNotifications : SchedulerClient
    {
        public SendLogNotifications(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }
        public override void DoWork()
        {
            try
            {
                this.Progressing();
                LoggingProvider.Instance().SendLogNotifications();
                this.ScheduleHistoryItem.Succeeded = true;
                this.ScheduleHistoryItem.AddLogNote("Sent log notifications successfully");
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
