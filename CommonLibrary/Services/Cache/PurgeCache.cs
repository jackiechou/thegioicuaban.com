using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Scheduling;

namespace CommonLibrary.Services.Cache
{
    public class PurgeCache : SchedulerClient
    {
        public PurgeCache(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }
        public override void DoWork()
        {
            try
            {
                string str = CachingProvider.Instance().PurgeCache();
                this.ScheduleHistoryItem.Succeeded = true;
                this.ScheduleHistoryItem.AddLogNote(str);
            }
            catch (Exception exc)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote(string.Format("Purging cache task failed.", exc.ToString()));
                this.Errored(ref exc);
                Exceptions.Exceptions.LogException(exc);
            }
        }
    }
}
