using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.ComponentModel;
using System.Collections;
using CommonLibrary.Entities.Host;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Scheduling
{
    public enum EventName
    {
        APPLICATION_START
    }
    public enum ScheduleSource
    {
        NOT_SET,
        STARTED_FROM_SCHEDULE_CHANGE,
        STARTED_FROM_EVENT,
        STARTED_FROM_TIMER,
        STARTED_FROM_BEGIN_REQUEST
    }
    public enum ScheduleStatus
    {
        NOT_SET,
        WAITING_FOR_OPEN_THREAD,
        RUNNING_EVENT_SCHEDULE,
        RUNNING_TIMER_SCHEDULE,
        RUNNING_REQUEST_SCHEDULE,
        WAITING_FOR_REQUEST,
        SHUTTING_DOWN,
        STOPPED
    }
    public enum SchedulerMode
    {
        DISABLED = 0,
        TIMER_METHOD = 1,
        REQUEST_METHOD = 2
    }
    public delegate void WorkStarted(SchedulerClient objSchedulerClient);
    public delegate void WorkProgressing(SchedulerClient objSchedulerClient);
    public delegate void WorkCompleted(SchedulerClient objSchedulerClient);
    public delegate void WorkErrored(SchedulerClient objSchedulerClient, Exception objException);
    public abstract class SchedulingProvider
    {
        private string _providerPath;
        private static bool _Debug;
        private static int _MaxThreads;
        public static bool Debug
        {
            get { return _Debug; }
        }
        public static bool Enabled
        {
            get
            {
                if (SchedulerMode != SchedulerMode.DISABLED)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public Scheduling.EventName EventName;
        public static int MaxThreads
        {
            get { return _MaxThreads; }
        }
        public string ProviderPath
        {
            get { return _providerPath; }
        }
        public static bool ReadyForPoll
        {
            get
            {
                if (DataCache.GetCache("ScheduleLastPolled") == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static System.DateTime ScheduleLastPolled
        {
            get
            {
                if (DataCache.GetCache("ScheduleLastPolled") != null)
                {
                    return Convert.ToDateTime(DataCache.GetCache("ScheduleLastPolled"));
                }
                else
                {
                    return System.DateTime.MinValue;
                }
            }
            set
            {
                System.DateTime ns;
                Scheduling.ScheduleItem s;
                s = Instance().GetNextScheduledTask(ServerController.GetExecutingServerName());
                if (s != null)
                {
                    System.DateTime NextStart = s.NextStart;
                    if (NextStart >= DateTime.Now)
                    {
                        ns = NextStart;
                    }
                    else
                    {
                        ns = DateTime.Now.AddMinutes(1);
                    }
                }
                else
                {
                    ns = DateTime.Now.AddMinutes(1);
                }
                DataCache.SetCache("ScheduleLastPolled", value, ns);
            }
        }
        public static Services.Scheduling.SchedulerMode SchedulerMode
        {
            get { return Host.SchedulerMode; }
        }
        public virtual Dictionary<string, string> Settings
        {
            get { return new Dictionary<string, string>(); }
        }
        public SchedulingProvider()
        {
            _providerPath = Settings["providerPath"];
            if (!string.IsNullOrEmpty(Settings["debug"]))
            {
                _Debug = Convert.ToBoolean(Settings["debug"]);
            }
            if (!string.IsNullOrEmpty(Settings["maxThreads"]))
            {
                _MaxThreads = Convert.ToInt32(Settings["maxThreads"]);
            }
            else
            {
                _MaxThreads = 1;
            }
        }
        public static new SchedulingProvider Instance()
        {
            return ComponentFactory.GetComponent<SchedulingProvider>();
        }
        public abstract void Start();
        public abstract void ExecuteTasks();
        public abstract void ReStart(string SourceOfRestart);
        public abstract void StartAndWaitForResponse();
        public abstract void Halt(string SourceOfHalt);
        public abstract void PurgeScheduleHistory();
        public abstract void RunEventSchedule(Scheduling.EventName objEventName);
        public abstract ArrayList GetSchedule();
        public abstract ArrayList GetSchedule(string Server);
        public abstract ScheduleItem GetSchedule(int ScheduleID);
        public abstract ScheduleItem GetSchedule(string TypeFullName, string Server);
        public abstract ScheduleItem GetNextScheduledTask(string Server);
        public abstract ArrayList GetScheduleHistory(int ScheduleID);
        public abstract Hashtable GetScheduleItemSettings(int ScheduleID);
        public abstract void AddScheduleItemSetting(int ScheduleID, string Name, string Value);
        public abstract IList<ScheduleItem> GetScheduleQueue();
        public abstract IList<ScheduleItem> GetScheduleProcessing();
        public abstract int GetFreeThreadCount();
        public abstract int GetActiveThreadCount();
        public abstract int GetMaxThreadCount();
        public abstract ScheduleStatus GetScheduleStatus();
        public abstract int AddSchedule(ScheduleItem objScheduleItem);
        public abstract void UpdateSchedule(ScheduleItem objScheduleItem);
        public abstract void DeleteSchedule(ScheduleItem objScheduleItem);
        public virtual void RunScheduleItemNow(ScheduleItem objScheduleItem)
        {
        }
    }
}
