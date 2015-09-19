using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Scheduling;

namespace CommonLibrary.Entities.Users.Users_Online
{
    public class PurgeUsersOnline : SchedulerClient
    {
        public PurgeUsersOnline(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }
        private void UpdateUsersOnline()
        {
            UserOnlineController objUserOnlineController = new UserOnlineController();
            if ((objUserOnlineController.IsEnabled()))
            {
                this.Status = "Updating Users Online";
                objUserOnlineController.UpdateUsersOnline();
                this.Status = "Update Users Online Successfully";
                this.ScheduleHistoryItem.Succeeded = true;
            }
        }
        public override void DoWork()
        {
            try
            {
                this.Progressing();
                UpdateUsersOnline();
                this.ScheduleHistoryItem.Succeeded = true;
                this.ScheduleHistoryItem.AddLogNote("UsersOnline purge completed.");
            }
            catch (Exception exc)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("UsersOnline purge failed." + exc.ToString());
                this.Errored(ref exc);
                CommonLibrary.Services.Exceptions.Exceptions.LogException(exc);
            }
        }
    }
}
