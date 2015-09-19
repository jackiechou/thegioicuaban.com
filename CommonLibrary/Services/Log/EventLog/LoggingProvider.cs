using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.ComponentModel;
using System.Collections;

namespace CommonLibrary.Services.Log.EventLog
{
    public abstract class LoggingProvider
    {
        public enum ReturnType
        {
            LogInfoObjects,
            XML
        }
        public static LoggingProvider Instance()
        {
            return ComponentFactory.GetComponent<LoggingProvider>();
        }
        public abstract bool SupportsEmailNotification();
        public abstract bool SupportsInternalViewer();
        public abstract bool LoggingIsEnabled(string LogType, int PortalID);
        public abstract bool SupportsSendToCoreTeam();
        public abstract bool SupportsSendViaEmail();
        public abstract void AddLog(LogInfo LogInfo);
        public abstract void DeleteLog(LogInfo LogInfo);
        public abstract void ClearLog();
        public abstract void PurgeLogBuffer();
        public abstract void SendLogNotifications();
        public abstract ArrayList GetLogTypeConfigInfo();
        public abstract LogTypeConfigInfo GetLogTypeConfigInfoByID(string ID);
        public abstract ArrayList GetLogTypeInfo();
        public abstract void AddLogTypeConfigInfo(string ID, bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string Threshold, string NotificationThresholdTime, string NotificationThresholdTimeType,
        string MailFromAddress, string MailToAddress);
        public abstract void UpdateLogTypeConfigInfo(string ID, bool LoggingIsActive, string LogTypeKey, string LogTypePortalID, string KeepMostRecent, string LogFileName, bool EmailNotificationIsActive, string Threshold, string NotificationThresholdTime, string NotificationThresholdTimeType,
        string MailFromAddress, string MailToAddress);
        public abstract void DeleteLogTypeConfigInfo(string ID);
        public abstract void AddLogType(string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner);
        public abstract void UpdateLogType(string LogTypeKey, string LogTypeFriendlyName, string LogTypeDescription, string LogTypeCSSClass, string LogTypeOwner);
        public abstract void DeleteLogType(string LogTypeKey);
        public abstract object GetSingleLog(LogInfo LogInfo, ReturnType objReturnType);
        public abstract LogInfoArray GetLog();
        public abstract LogInfoArray GetLog(string LogType);
        public abstract LogInfoArray GetLog(int PortalID);
        public abstract LogInfoArray GetLog(int PortalID, string LogType);
        public abstract LogInfoArray GetLog(int PageSize, int PageIndex, ref int TotalRecords);
        public abstract LogInfoArray GetLog(string LogType, int PageSize, int PageIndex, ref int TotalRecords);
        public abstract LogInfoArray GetLog(int PortalID, int PageSize, int PageIndex, ref int TotalRecords);
        public abstract LogInfoArray GetLog(int PortalID, string LogType, int PageSize, int PageIndex, ref int TotalRecords);
    }
}
