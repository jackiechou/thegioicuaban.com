using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Log.EventLog
{
    [Serializable()]
    public class LogTypeConfigInfo : LogTypeInfo
    {
        private string _ID;
        private bool _LoggingIsActive;
        private string _LogFileName;
        private string _LogFileNameWithPath;
        private string _LogTypePortalID;
        private string _KeepMostRecent;
        public enum NotificationThresholdTimeTypes
        {
            None = 0,
            Seconds = 1,
            Minutes = 2,
            Hours = 3,
            Days = 4
        }
        private bool _EmailNotificationIsActive;
        private string _MailFromAddress;
        private string _MailToAddress;
        private int _NotificationThreshold;
        private int _NotificationThresholdTime;
        private NotificationThresholdTimeTypes _NotificationThresholdTimeType;
        public System.DateTime StartDateTime
        {
            get
            {
                switch (this.NotificationThresholdTimeType)
                {
                    case NotificationThresholdTimeTypes.Seconds:
                        return System.DateTime.Now.AddSeconds(NotificationThresholdTime * -1);
                    case NotificationThresholdTimeTypes.Minutes:
                        return System.DateTime.Now.AddMinutes(NotificationThresholdTime * -1);
                    case NotificationThresholdTimeTypes.Hours:
                        return System.DateTime.Now.AddHours(NotificationThresholdTime * -1);
                    case NotificationThresholdTimeTypes.Days:
                        return System.DateTime.Now.AddDays(NotificationThresholdTime * -1);
                    case NotificationThresholdTimeTypes.None:
                        return Null.NullDate;
                    default:
                        return Null.NullDate;
                }
            }
        }
        public bool EmailNotificationIsActive
        {
            get { return _EmailNotificationIsActive; }
            set { _EmailNotificationIsActive = value; }
        }
        public string MailFromAddress
        {
            get { return _MailFromAddress; }
            set { _MailFromAddress = value; }
        }
        public string MailToAddress
        {
            get { return _MailToAddress; }
            set { _MailToAddress = value; }
        }
        public int NotificationThreshold
        {
            get { return _NotificationThreshold; }
            set { _NotificationThreshold = value; }
        }
        public int NotificationThresholdTime
        {
            get { return _NotificationThresholdTime; }
            set { _NotificationThresholdTime = value; }
        }
        public NotificationThresholdTimeTypes NotificationThresholdTimeType
        {
            get { return _NotificationThresholdTimeType; }
            set { _NotificationThresholdTimeType = value; }
        }
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public bool LoggingIsActive
        {
            get { return _LoggingIsActive; }
            set { _LoggingIsActive = value; }
        }
        public string LogFileName
        {
            get { return _LogFileName; }
            set { _LogFileName = value; }
        }
        public string LogFileNameWithPath
        {
            get { return _LogFileNameWithPath; }
            set { _LogFileNameWithPath = value; }
        }
        public string LogTypePortalID
        {
            get { return _LogTypePortalID; }
            set { _LogTypePortalID = value; }
        }
        public string KeepMostRecent
        {
            get { return _KeepMostRecent; }
            set { _KeepMostRecent = value; }
        }
    }
}
