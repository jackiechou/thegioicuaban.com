using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using System.Collections;
using System.Web;
using CommonLibrary.Common.Utilities;
using System.IO;
using CommonLibrary.Services.Scheduling;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Services.Log.EventLog;
using CommonLibrary.Data;
using CommonLibrary.Entities.Host;
using CommonLibrary.Services.EventQueue;
using CommonLibrary.Services.Upgrade;

namespace CommonLibrary.Common
{
    public class Initialize
    {
        private static bool InitializedAlready;
        private static object InitializeLock = new object();
        private static void CacheMappedDirectory()
        {
            Services.FileSystem.FolderController objFolderController = new Services.FileSystem.FolderController();
            PortalController objPortalController = new PortalController();
            ArrayList arrPortals = objPortalController.GetPortals();
            int i;
            for (i = 0; i <= arrPortals.Count - 1; i++)
            {
                PortalInfo objPortalInfo = (PortalInfo)arrPortals[i];
                objFolderController.SetMappedDirectory(objPortalInfo, HttpContext.Current);
            }
        }
        private static string CheckVersion(HttpApplication app)
        {
            HttpServerUtility Server = app.Server;
            bool AutoUpgrade;
            if (Config.GetSetting("AutoUpgrade") == null)
            {
                AutoUpgrade = true;
            }
            else
            {
                AutoUpgrade = bool.Parse(Config.GetSetting("AutoUpgrade"));
            }
            bool UseWizard;
            if (Config.GetSetting("UseInstallWizard") == null)
            {
                UseWizard = true;
            }
            else
            {
                UseWizard = bool.Parse(Config.GetSetting("UseInstallWizard"));
            }
            string retValue = Null.NullString;
            switch (Globals.Status)
            {
                case Globals.UpgradeStatus.Install:
                    if (AutoUpgrade)
                    {
                        if (UseWizard)
                        {
                            retValue = "~/Install/InstallWizard.aspx";
                        }
                        else
                        {
                            retValue = "~/Install/Install.aspx?mode=install";
                        }
                    }
                    else
                    {
                        CreateUnderConstructionPage(Server);
                        retValue = "~/Install/UnderConstruction.htm";
                    }
                    break;
                case Globals.UpgradeStatus.Upgrade:
                    if (AutoUpgrade)
                    {
                        retValue = "~/Install/Install.aspx?mode=upgrade";
                    }
                    else
                    {
                        CreateUnderConstructionPage(Server);
                        retValue = "~/Install/UnderConstruction.htm";
                    }
                    break;
                case Globals.UpgradeStatus.Error:
                    CreateUnderConstructionPage(Server);
                    retValue = "~/Install/UnderConstruction.htm";
                    break;
            }
            return retValue;
        }
        private static void CreateUnderConstructionPage(HttpServerUtility server)
        {
            if (!File.Exists(server.MapPath("~/Install/UnderConstruction.htm")))
            {
                if (File.Exists(server.MapPath("~/Install/UnderConstruction.template.htm")))
                {
                    File.Copy(server.MapPath("~/Install/UnderConstruction.template.htm"), server.MapPath("~/Install/UnderConstruction.htm"));
                }
            }
        }
        private static string InitializeApp(HttpApplication app)
        {
            HttpServerUtility Server = app.Server;
            HttpRequest Request = app.Request;
            string redirect = Null.NullString;
            if (HttpContext.Current.Request.ApplicationPath == "/")
            {
                if (String.IsNullOrEmpty(Config.GetSetting("InstallationSubfolder")))
                {
                    Globals.ApplicationPath = "";
                }
                else
                {
                    Globals.ApplicationPath = (Config.GetSetting("InstallationSubfolder") + "/").ToLowerInvariant();
                }
            }
            else
            {
                Globals.ApplicationPath = Request.ApplicationPath.ToLowerInvariant();
            }
            Globals.ApplicationMapPath = System.AppDomain.CurrentDomain.BaseDirectory.Substring(0, System.AppDomain.CurrentDomain.BaseDirectory.Length - 1);
            Globals.ApplicationMapPath = Globals.ApplicationMapPath.Replace("/", "\\");
            Globals.HostPath = Globals.ApplicationPath + "/Portals/_default/";
            Globals.HostMapPath = Server.MapPath(Globals.HostPath);
            Globals.InstallPath = Globals.ApplicationPath + "/Install/";
            Globals.InstallMapPath = Server.MapPath(Globals.InstallPath);
            Globals.GetStatus();
            if (!Request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx") && !Request.Url.LocalPath.ToLower().EndsWith("install.aspx"))
            {
                redirect = CheckVersion(app);
                if (string.IsNullOrEmpty(redirect))
                {
                    CacheMappedDirectory();
                    Globals.IISAppName = Request.ServerVariables["APPL_MD_PATH"];
                    Globals.OperatingSystemVersion = Environment.OSVersion.Version;
                    Globals.NETFrameworkVersion = GetNETFrameworkVersion();
                    Globals.DatabaseEngineVersion = GetDatabaseEngineVersion();
                    Upgrade.TryUpgradeNETFramework();
                    StartScheduler();
                    LogStart();
                    EventQueueController.ProcessMessages("Application_Start");

                    //Set Flag so we can determine the first Page Request after Application Start
                    app.Context.Items.Add("FirstRequest", true);

                    ServerController.UpdateServerActivity(new ServerInfo());
                }
            }
            else
            {
                Globals.NETFrameworkVersion = GetNETFrameworkVersion();
            }
            return redirect;
        }
        private static System.Version GetNETFrameworkVersion()
        {
            string version = System.Environment.Version.ToString(2);
            if (version == "2.0")
            {
                System.Reflection.Assembly assembly;
                try
                {
                    assembly = AppDomain.CurrentDomain.Load("System.Runtime.Serialization, Version=3.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089");
                    version = "3.0";
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                try
                {
                    assembly = AppDomain.CurrentDomain.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089");
                    version = "3.5";
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            return new System.Version(version);
        }
        private static System.Version GetDatabaseEngineVersion()
        {
            return DataProvider.Instance().GetDatabaseEngineVersion();
        }
        public static void Init(HttpApplication app)
        {
            HttpResponse Response = app.Response;
            string redirect = Null.NullString;
            if ((InitializedAlready && Globals.Status == Globals.UpgradeStatus.None))
            {
                return;
            }
            lock (InitializeLock)
            {
                if ((InitializedAlready && Globals.Status == Globals.UpgradeStatus.None))
                {
                    return;
                }
                redirect = InitializeApp(app);
                InitializedAlready = true;
            }
            if (!string.IsNullOrEmpty(redirect))
            {
                Response.Redirect(redirect, true);
            }
        }
        public static void LogStart()
        {
            EventLogController objEv = new EventLogController();
            LogInfo objEventLogInfo = new LogInfo();
            objEventLogInfo.BypassBuffering = true;
            objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.APPLICATION_START.ToString();
            objEv.AddLog(objEventLogInfo);
        }
        public static void StartScheduler()
        {
            if (Services.Scheduling.SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD)
            {
                SchedulingProvider scheduler = SchedulingProvider.Instance();
                scheduler.RunEventSchedule(CommonLibrary.Services.Scheduling.EventName.APPLICATION_START);
                System.Threading.Thread newThread = new System.Threading.Thread(CommonLibrary.Services.Scheduling.SchedulingProvider.Instance().Start);
                newThread.IsBackground = true;
                newThread.Start();
            }
        }
        public static void LogEnd()
        {
            try
            {
                System.Web.ApplicationShutdownReason shutdownReason = System.Web.Hosting.HostingEnvironment.ShutdownReason;
                string shutdownDetail = "";
                switch (shutdownReason)
                {
                    case ApplicationShutdownReason.BinDirChangeOrDirectoryRename:
                        shutdownDetail = "The AppDomain shut down because of a change to the Bin folder or files contained in it.";
                        break;
                    case ApplicationShutdownReason.BrowsersDirChangeOrDirectoryRename:
                        shutdownDetail = "The AppDomain shut down because of a change to the App_Browsers folder or files contained in it.";
                        break;
                    case ApplicationShutdownReason.ChangeInGlobalAsax:
                        shutdownDetail = "The AppDomain shut down because of a change to Global.asax.";
                        break;
                    case ApplicationShutdownReason.ChangeInSecurityPolicyFile:
                        shutdownDetail = "The AppDomain shut down because of a change in the code access security policy file.";
                        break;
                    case ApplicationShutdownReason.CodeDirChangeOrDirectoryRename:
                        shutdownDetail = "The AppDomain shut down because of a change to the App_Code folder or files contained in it.";
                        break;
                    case ApplicationShutdownReason.ConfigurationChange:
                        shutdownDetail = "The AppDomain shut down because of a change to the application level configuration.";
                        break;
                    case ApplicationShutdownReason.HostingEnvironment:
                        shutdownDetail = "The AppDomain shut down because of the hosting environment.";
                        break;
                    case ApplicationShutdownReason.HttpRuntimeClose:
                        shutdownDetail = "The AppDomain shut down because of a call to Close.";
                        break;
                    case ApplicationShutdownReason.IdleTimeout:
                        shutdownDetail = "The AppDomain shut down because of the maximum allowed idle time limit.";
                        break;
                    case ApplicationShutdownReason.InitializationError:
                        shutdownDetail = "The AppDomain shut down because of an AppDomain initialization error.";
                        break;
                    case ApplicationShutdownReason.MaxRecompilationsReached:
                        shutdownDetail = "The AppDomain shut down because of the maximum number of dynamic recompiles of resources limit.";
                        break;
                    case ApplicationShutdownReason.PhysicalApplicationPathChanged:
                        shutdownDetail = "The AppDomain shut down because of a change to the physical path for the application.";
                        break;
                    case ApplicationShutdownReason.ResourcesDirChangeOrDirectoryRename:
                        shutdownDetail = "The AppDomain shut down because of a change to the App_GlobalResources folder or files contained in it.";
                        break;
                    case ApplicationShutdownReason.UnloadAppDomainCalled:
                        shutdownDetail = "The AppDomain shut down because of a call to UnloadAppDomain.";
                        break;
                    default:
                        shutdownDetail = "No shutdown reason provided.";
                        break;
                }
                EventLogController objEv = new EventLogController();
                LogInfo objEventLogInfo = new LogInfo();
                objEventLogInfo.BypassBuffering = true;
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.APPLICATION_SHUTTING_DOWN.ToString();
                objEventLogInfo.AddProperty("Shutdown Details", shutdownDetail);
                objEv.AddLog(objEventLogInfo);
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            //if (Globals.Status != Globals.UpgradeStatus.Install)
            //{
            //    LoggingProvider.Instance().PurgeLogBuffer();
            //}
        }
        public static void RunSchedule(HttpRequest request)
        {
            if (request.Url.LocalPath.ToLower().EndsWith("install.aspx") || request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx"))
            {
                return;
            }
            try
            {
                if (Services.Scheduling.SchedulingProvider.SchedulerMode == SchedulerMode.REQUEST_METHOD && Services.Scheduling.SchedulingProvider.ReadyForPoll)
                {
                    SchedulingProvider scheduler = SchedulingProvider.Instance();
                    System.Threading.Thread RequestScheduleThread;
                    RequestScheduleThread = new System.Threading.Thread(scheduler.ExecuteTasks);
                    RequestScheduleThread.IsBackground = true;
                    RequestScheduleThread.Start();
                    Services.Scheduling.SchedulingProvider.ScheduleLastPolled = DateTime.Now;
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
        }
        public static void StopScheduler()
        {
            //SchedulingProvider.Instance().Halt("Stopped by Application_End");
        }
    }
}
