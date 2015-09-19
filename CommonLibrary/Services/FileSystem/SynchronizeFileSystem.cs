using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using System.Collections;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Services.Scheduling;

namespace CommonLibrary.Services.FileSystem
{
    public class SynchronizeFileSystem : SchedulerClient
    {
        public SynchronizeFileSystem(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }
        public override void DoWork()
        {
            try
            {
                this.Progressing();
                Synchronize();
                this.ScheduleHistoryItem.Succeeded = true;
                this.ScheduleHistoryItem.AddLogNote("File System Synchronized.");
            }
            catch (Exception exc)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("File System Synchronization failed. " + exc.ToString());
                this.Errored(ref exc);
                Exceptions.Exceptions.LogException(exc);
            }
        }
        private void Synchronize()
        {
            PortalController objPortals = new PortalController();
            ArrayList arrPortals = objPortals.GetPortals();
            PortalInfo objPortal;
            FileSystemUtils.Synchronize(Null.NullInteger, Null.NullInteger, Common.Globals.HostMapPath, false);
            int intIndex;
            for (intIndex = 0; intIndex <= arrPortals.Count - 1; intIndex++)
            {
                objPortal = (PortalInfo)arrPortals[intIndex];
                FileSystemUtils.Synchronize(objPortal.PortalID, objPortal.AdministratorRoleId, objPortal.HomeDirectoryMapPath, false);
            }
        }
    }
}
