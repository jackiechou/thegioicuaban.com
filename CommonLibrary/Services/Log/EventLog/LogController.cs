using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CommonLibrary.Common.Utilities;
using System.IO;
using System.Threading;
using System.Collections;
using System.Xml;
using CommonLibrary.Common;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Services.Log.EventLog
{
    public class LogController
    {
        private static ReaderWriterLock lockLog = new ReaderWriterLock();
        private const int ReaderLockTimeout = 10000;
        private const int WriterLockTimeout = 10000;
        public void AddLog(LogInfo objLogInfo)
        {
            if (Globals.Status == Globals.UpgradeStatus.Install)
            {
                AddLogToFile(objLogInfo);
            }
            else
            {
                try
                {
                    objLogInfo.LogCreateDate = DateTime.Now;
                    objLogInfo.LogServerName = Common.Globals.ServerName;
                    if (string.IsNullOrEmpty(objLogInfo.LogServerName))
                    {
                        objLogInfo.LogServerName = "NA";
                    }
                    if (String.IsNullOrEmpty(objLogInfo.LogUserName))
                    {
                        if (HttpContext.Current != null && HttpContext.Current.Request != null)
                        {
                            if (HttpContext.Current.Request.IsAuthenticated)
                            {
                                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                                objLogInfo.LogUserName = objUserInfo.Username;
                            }
                        }
                    }
                    LoggingProvider.Instance().AddLog(objLogInfo);
                }
                catch (Exception exc)
                {
                    AddLogToFile(objLogInfo);
                }
            }
        }

        private void AddLogToFile(LogInfo objLogInfo)
        {
            try
            {
                string str = objLogInfo.Serialize();
                string f;
                f = Common.Globals.HostMapPath + "\\Logs\\LogFailures.xml.resources";
                WriteLog(f, str);
            }
            catch (Exception exc)
            {
                exc.ToString();
            }
        }
        public virtual void AddLogType(string configFile, string fallbackConfigFile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(configFile);
            }
            catch (FileNotFoundException exc)
            {
                xmlDoc.Load(fallbackConfigFile);
                exc.ToString();
            }
            XmlNodeList LogType = xmlDoc.SelectNodes("/LogConfig/LogTypes/LogType");
            foreach (XmlNode LogTypeInfo in LogType)
            {
                Log.EventLog.LogTypeInfo objLogTypeInfo = new Log.EventLog.LogTypeInfo();
                objLogTypeInfo.LogTypeKey = LogTypeInfo.Attributes["LogTypeKey"].Value;
                objLogTypeInfo.LogTypeFriendlyName = LogTypeInfo.Attributes["LogTypeFriendlyName"].Value;
                objLogTypeInfo.LogTypeDescription = LogTypeInfo.Attributes["LogTypeDescription"].Value;
                objLogTypeInfo.LogTypeCSSClass = LogTypeInfo.Attributes["LogTypeCSSClass"].Value;
                objLogTypeInfo.LogTypeOwner = LogTypeInfo.Attributes["LogTypeOwner"].Value;
                AddLogType(objLogTypeInfo);
            }
            XmlNodeList LogTypeConfig = xmlDoc.SelectNodes("/LogConfig/LogTypeConfig");
            foreach (XmlNode LogTypeConfigInfo in LogTypeConfig)
            {
                Log.EventLog.LogTypeConfigInfo objLogTypeConfig = new Log.EventLog.LogTypeConfigInfo();
                objLogTypeConfig.EmailNotificationIsActive = LogTypeConfigInfo.Attributes["EmailNotificationStatus"].Value == "On";
                objLogTypeConfig.KeepMostRecent = LogTypeConfigInfo.Attributes["KeepMostRecent"].Value;
                objLogTypeConfig.LoggingIsActive = LogTypeConfigInfo.Attributes["LoggingStatus"].Value == "On";
                objLogTypeConfig.LogTypeKey = LogTypeConfigInfo.Attributes["LogTypeKey"].Value;
                objLogTypeConfig.LogTypePortalID = LogTypeConfigInfo.Attributes["LogTypePortalID"].Value;
                objLogTypeConfig.MailFromAddress = LogTypeConfigInfo.Attributes["MailFromAddress"].Value;
                objLogTypeConfig.MailToAddress = LogTypeConfigInfo.Attributes["MailToAddress"].Value;
                objLogTypeConfig.NotificationThreshold = Convert.ToInt32(LogTypeConfigInfo.Attributes["NotificationThreshold"].Value);
                objLogTypeConfig.NotificationThresholdTime = Convert.ToInt32(LogTypeConfigInfo.Attributes["NotificationThresholdTime"].Value);
                objLogTypeConfig.NotificationThresholdTimeType = (Services.Log.EventLog.LogTypeConfigInfo.NotificationThresholdTimeTypes)Enum.Parse(typeof(Services.Log.EventLog.LogTypeConfigInfo.NotificationThresholdTimeTypes), LogTypeConfigInfo.Attributes["NotificationThresholdTimeType"].Value);
                AddLogTypeConfigInfo(objLogTypeConfig);
            }
        }
        public virtual void AddLogType(LogTypeInfo objLogTypeInfo)
        {
            LoggingProvider.Instance().AddLogType(objLogTypeInfo.LogTypeKey, objLogTypeInfo.LogTypeFriendlyName, objLogTypeInfo.LogTypeDescription, objLogTypeInfo.LogTypeCSSClass, objLogTypeInfo.LogTypeOwner);
        }
        public virtual void AddLogTypeConfigInfo(LogTypeConfigInfo objLogTypeConfigInfo)
        {
            LoggingProvider.Instance().AddLogTypeConfigInfo(objLogTypeConfigInfo.ID, objLogTypeConfigInfo.LoggingIsActive, objLogTypeConfigInfo.LogTypeKey, objLogTypeConfigInfo.LogTypePortalID, objLogTypeConfigInfo.KeepMostRecent, objLogTypeConfigInfo.LogFileName, objLogTypeConfigInfo.EmailNotificationIsActive, Convert.ToString(objLogTypeConfigInfo.NotificationThreshold), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTime), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTimeType),
            objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress);
        }
        public void ClearLog()
        {
            LoggingProvider.Instance().ClearLog();
        }
        public void DeleteLog(LogInfo objLogInfo)
        {
            LoggingProvider.Instance().DeleteLog(objLogInfo);
        }
        public virtual void DeleteLogType(LogTypeInfo objLogTypeInfo)
        {
            LoggingProvider.Instance().DeleteLogType(objLogTypeInfo.LogTypeKey);
        }
        public virtual void DeleteLogTypeConfigInfo(LogTypeConfigInfo objLogTypeConfigInfo)
        {
            LoggingProvider.Instance().DeleteLogTypeConfigInfo(objLogTypeConfigInfo.ID);
        }
        public virtual LogInfoArray GetLog(int PageSize, int PageIndex, ref int TotalRecords)
        {
            return LoggingProvider.Instance().GetLog(PageSize, PageIndex, ref TotalRecords);
        }
        public virtual LogInfoArray GetLog(int PortalID, int PageSize, int PageIndex, ref int TotalRecords)
        {
            return LoggingProvider.Instance().GetLog(PortalID, PageSize, PageIndex, ref TotalRecords);
        }
        public virtual LogInfoArray GetLog(int PortalID, string LogType, int PageSize, int PageIndex, ref int TotalRecords)
        {
            return LoggingProvider.Instance().GetLog(PortalID, LogType, PageSize, PageIndex, ref TotalRecords);
        }
        public virtual LogInfoArray GetLog(string LogType, int PageSize, int PageIndex, ref int TotalRecords)
        {
            return LoggingProvider.Instance().GetLog(LogType, PageSize, PageIndex, ref TotalRecords);
        }
        public virtual ArrayList GetLogTypeConfigInfo()
        {
            return (ArrayList)LoggingProvider.Instance().GetLogTypeConfigInfo();
        }
        public virtual LogTypeConfigInfo GetLogTypeConfigInfoByID(string ID)
        {
            return LoggingProvider.Instance().GetLogTypeConfigInfoByID(ID);
        }
        public virtual ArrayList GetLogTypeInfo()
        {
            return (ArrayList)LoggingProvider.Instance().GetLogTypeInfo();
        }
        public virtual object GetSingleLog(LogInfo objLogInfo, LoggingProvider.ReturnType objReturnType)
        {
            return LoggingProvider.Instance().GetSingleLog(objLogInfo, objReturnType);
        }
        public bool LoggingIsEnabled(string LogType, int PortalID)
        {
            return LoggingProvider.Instance().LoggingIsEnabled(LogType, PortalID);
        }
        public void PurgeLogBuffer()
        {
            LoggingProvider.Instance().PurgeLogBuffer();
        }
        public virtual bool SupportsEmailNotification()
        {
            return LoggingProvider.Instance().SupportsEmailNotification();
        }
        public virtual bool SupportsInternalViewer()
        {
            return LoggingProvider.Instance().SupportsInternalViewer();
        }
        public virtual void UpdateLogTypeConfigInfo(LogTypeConfigInfo objLogTypeConfigInfo)
        {
            LoggingProvider.Instance().UpdateLogTypeConfigInfo(objLogTypeConfigInfo.ID, objLogTypeConfigInfo.LoggingIsActive, objLogTypeConfigInfo.LogTypeKey, objLogTypeConfigInfo.LogTypePortalID, objLogTypeConfigInfo.KeepMostRecent, objLogTypeConfigInfo.LogFileName, objLogTypeConfigInfo.EmailNotificationIsActive, Convert.ToString(objLogTypeConfigInfo.NotificationThreshold), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTime), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTimeType),
            objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress);
        }
        public virtual void UpdateLogType(LogTypeInfo objLogTypeInfo)
        {
            LoggingProvider.Instance().UpdateLogType(objLogTypeInfo.LogTypeKey, objLogTypeInfo.LogTypeFriendlyName, objLogTypeInfo.LogTypeDescription, objLogTypeInfo.LogTypeCSSClass, objLogTypeInfo.LogTypeOwner);
        }
        private void WriteLog(string FilePath, string Message)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                lockLog.AcquireWriterLock(WriterLockTimeout);
                int intAttempts = 0;
                while (fs == null && intAttempts < 100)
                {
                    intAttempts += 1;
                    try
                    {
                        fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                    }
                    catch (IOException exc)
                    {
                        Thread.Sleep(1);
                        exc.ToString();
                    }
                }
                if (fs == null)
                {
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Response.Write("An error has occurred writing to the exception log.");
                        HttpContext.Current.Response.End();
                    }
                }
                else
                {
                    sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    long FileLength;
                    FileLength = fs.Length;
                    if (FileLength > 0)
                    {
                        fs.Position = FileLength - 9;
                    }
                    else
                    {
                        Message = "<logs>" + Message;
                    }
                    sw.WriteLine(Message + "</logs>");
                    sw.Flush();
                }
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
            catch (UnauthorizedAccessException exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "Unauthorized Access Error");
                    string strMessage = exc.Message + " The Windows User Account listed below must have Read/Write Privileges to this path.";
                    HtmlUtils.WriteError(response, FilePath, strMessage);
                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "Directory Not Found Error");
                    string strMessage = exc.Message;
                    HtmlUtils.WriteError(response, FilePath, strMessage);
                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            catch (PathTooLongException exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "Path Too Long Error");
                    string strMessage = exc.Message;
                    HtmlUtils.WriteError(response, FilePath, strMessage);
                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            catch (IOException exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "IO Error");
                    string strMessage = exc.Message;
                    HtmlUtils.WriteError(response, FilePath, strMessage);
                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            catch (Exception exc)
            {
                if (HttpContext.Current != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    HtmlUtils.WriteHeader(response, "Unhandled Error");
                    string strMessage = exc.Message;
                    HtmlUtils.WriteError(response, FilePath, strMessage);
                    HtmlUtils.WriteFooter(response);
                    response.End();
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
                lockLog.ReleaseWriterLock();
            }
        }
       
    }
}
