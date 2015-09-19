using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Scheduling
{
    public class PurgeScheduleHistory : SchedulerClient
    {
        public PurgeScheduleHistory(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }
        public override void DoWork()
        {
            try
            {
                this.Progressing();
                SchedulingProvider.Instance().PurgeScheduleHistory();
                this.ScheduleHistoryItem.Succeeded = true;
                this.ScheduleHistoryItem.AddLogNote("Schedule history purged.");
            }
            catch (Exception exc)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("Schedule history purge failed." + exc.ToString());
                this.ScheduleHistoryItem.Succeeded = false;
                this.Errored(ref exc);
                Exceptions.Exceptions.LogException(exc);
            }
        }
    }
}
