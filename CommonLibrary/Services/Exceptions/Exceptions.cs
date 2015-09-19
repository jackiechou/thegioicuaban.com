using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Web.UI;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common.Utilities;
using System.Web;
using CommonLibrary.Common;
using System.Web.UI.WebControls;
using CommonLibrary.Entities.Modules;
using CommonLibrary.UI.Modules;
using CommonLibrary.Entities.Host;
using CommonLibrary.Services.Log.EventLog;

namespace CommonLibrary.Services.Exceptions
{
    public static class Exceptions
    {
        public static ExceptionInfo GetExceptionInfo(Exception e)
        {
            ExceptionInfo objExceptionInfo = new ExceptionInfo();
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }
            StackTrace st = new StackTrace(e, true);
            StackFrame sf = st.GetFrame(0);
            try
            {
                MemberInfo mi = sf.GetMethod();
                string res = mi.DeclaringType.Namespace + ".";
                res += mi.DeclaringType.Name + ".";
                res += mi.Name;
                objExceptionInfo.Method = res;
            }
            catch (Exception ex)
            {
                ex.ToString();
                objExceptionInfo.Method = "N/A - Reflection Permission required";
            }
            if (!String.IsNullOrEmpty(sf.GetFileName()))
            {
                objExceptionInfo.FileName = sf.GetFileName();
                objExceptionInfo.FileColumnNumber = sf.GetFileColumnNumber();
                objExceptionInfo.FileLineNumber = sf.GetFileLineNumber();
            }
            return objExceptionInfo;
        }
        private static bool ThreadAbortCheck(Exception exc)
        {
            if (exc is System.Threading.ThreadAbortException)
            {
                System.Threading.Thread.ResetAbort();
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void ProcessModuleLoadException(PortalModuleBase objPortalModuleBase, Exception exc)
        {
            ProcessModuleLoadException((Control)objPortalModuleBase, exc);
        }
        public static void ProcessModuleLoadException(PortalModuleBase objPortalModuleBase, Exception exc, bool DisplayErrorMessage)
        {
            ProcessModuleLoadException((Control)objPortalModuleBase, exc, DisplayErrorMessage);
        }
        public static void ProcessModuleLoadException(string FriendlyMessage, PortalModuleBase objPortalModuleBase, Exception exc, bool DisplayErrorMessage)
        {
            ProcessModuleLoadException(FriendlyMessage, (Control)objPortalModuleBase, exc, DisplayErrorMessage);
        }
        public static void ProcessModuleLoadException(Control ctrl, Exception exc)
        {
            if (ThreadAbortCheck(exc))
                return;
            ProcessModuleLoadException(ctrl, exc, true);
        }
        public static void ProcessModuleLoadException(Control ctrl, Exception exc, bool DisplayErrorMessage)
        {
            if (ThreadAbortCheck(exc))
                return;
            string friendlyMessage = Services.Localization.Localization.GetString("ErrorOccurred");
            IModuleControl ctrlModule = ctrl as IModuleControl;
            if (ctrlModule == null)
            {
                friendlyMessage = Services.Localization.Localization.GetString("ErrorOccurred");
            }
            else
            {
                string moduleTitle = Null.NullString;
                if (ctrlModule != null && ctrlModule.ModuleContext.Configuration != null)
                {
                    moduleTitle = ctrlModule.ModuleContext.Configuration.ModuleTitle;
                }
                friendlyMessage = string.Format(Localization.Localization.GetString("ModuleUnavailable"), moduleTitle);
            }
            ProcessModuleLoadException(friendlyMessage, ctrl, exc, DisplayErrorMessage);
        }
        public static void ProcessModuleLoadException(string FriendlyMessage, Control ctrl, Exception exc)
        {
            if (ThreadAbortCheck(exc))
                return;
            ProcessModuleLoadException(FriendlyMessage, ctrl, exc, true);
        }
        public static void ProcessModuleLoadException(string FriendlyMessage, Control ctrl, Exception exc, bool DisplayErrorMessage)
        {
            if (ThreadAbortCheck(exc))
                return;
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            try
            {
                if (!Host.UseCustomErrorMessages)
                {
                    throw new ModuleLoadException(FriendlyMessage, exc);
                }
                else
                {
                    IModuleControl ctrlModule = ctrl as IModuleControl;
                    ModuleLoadException lex = null;
                    if (ctrlModule == null)
                    {
                        lex = new ModuleLoadException(exc.Message.ToString(), exc);
                    }
                    else
                    {
                        lex = new ModuleLoadException(exc.Message.ToString(), exc, ctrlModule.ModuleContext.Configuration);
                    }
                    CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
                    objExceptionLog.AddLog(lex);
                    if (DisplayErrorMessage)
                    {
                        PlaceHolder ErrorPlaceholder = null;
                        if (ctrl.Parent != null)
                        {
                            ErrorPlaceholder = (PlaceHolder)ctrl.Parent.FindControl("MessagePlaceHolder");
                        }
                        if (ErrorPlaceholder != null)
                        {
                            ctrl.Visible = false;
                            ErrorPlaceholder.Visible = true;
                            ErrorPlaceholder.Controls.Add(new ErrorContainer(_portalSettings, FriendlyMessage, lex).Container);
                        }
                        else
                        {
                            ctrl.Controls.Add(new ErrorContainer(_portalSettings, FriendlyMessage, lex).Container);
                        }
                    }
                }
            }
            catch (Exception exc2)
            {
                ProcessPageLoadException(exc2);
            }
        }
        public static void ProcessPageLoadException(Exception exc)
        {
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            string appURL = Globals.ApplicationURL();
            if (appURL.IndexOf("?") == Null.NullInteger)
            {
                appURL += "?def=ErrorMessage";
            }
            else
            {
                appURL += "&def=ErrorMessage";
            }
            ProcessPageLoadException(exc, appURL);
        }
        public static void ProcessPageLoadException(Exception exc, string URL)
        {
            if (ThreadAbortCheck(exc))
                return;
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            if (!Host.UseCustomErrorMessages)
            {
                throw new PageLoadException(exc.Message, exc);
            }
            else
            {
                PageLoadException lex = new PageLoadException(exc.Message.ToString(), exc);
                CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
                objExceptionLog.AddLog(lex);
                if (!String.IsNullOrEmpty(URL))
                {
                    if (URL.IndexOf("error=terminate") != -1)
                    {
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Server.Transfer("~/ErrorPage.aspx");
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect(URL, true);
                    }
                }
            }
        }
        public static void LogException(ModuleLoadException exc)
        {
            CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
            objExceptionLog.AddLog(exc, CommonLibrary.Services.Log.EventLog.EventLogController.ExceptionLogType.MODULE_LOAD_EXCEPTION);
        }
        public static void LogException(PageLoadException exc)
        {
            CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
            objExceptionLog.AddLog(exc, CommonLibrary.Services.Log.EventLog.EventLogController.ExceptionLogType.PAGE_LOAD_EXCEPTION);
        }
        public static void LogException(SchedulerException exc)
        {
            CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
            objExceptionLog.AddLog(exc, CommonLibrary.Services.Log.EventLog.EventLogController.ExceptionLogType.SCHEDULER_EXCEPTION);
        }
        public static void LogException(SecurityException exc)
        {
            CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
            objExceptionLog.AddLog(exc, CommonLibrary.Services.Log.EventLog.EventLogController.ExceptionLogType.SECURITY_EXCEPTION);
        }
        public static void LogException(Exception exc)
        {
            CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
            objExceptionLog.AddLog(exc, CommonLibrary.Services.Log.EventLog.EventLogController.ExceptionLogType.GENERAL_EXCEPTION);
        }
        public static void ProcessSchedulerException(Exception exc)
        {
            CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
            objExceptionLog.AddLog(exc, CommonLibrary.Services.Log.EventLog.EventLogController.ExceptionLogType.SCHEDULER_EXCEPTION);
        }
        public static void LogSearchException(SearchException exc)
        {
            CommonLibrary.Services.Log.EventLog.EventLogController objExceptionLog = new CommonLibrary.Services.Log.EventLog.EventLogController();
            objExceptionLog.AddLog(exc, CommonLibrary.Services.Log.EventLog.EventLogController.ExceptionLogType.SEARCH_INDEXER_EXCEPTION);
        }
    }
}
