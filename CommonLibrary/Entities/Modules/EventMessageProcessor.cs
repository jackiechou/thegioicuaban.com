using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.EventQueue;
using System.Web;
using CommonLibrary.Services.Exceptions;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Entities.Modules
{
    public class EventMessageProcessor : EventMessageProcessorBase
    {
        private void ImportModule(EventMessage message)
        {
            try
            {
                string BusinessControllerClass = message.Attributes["BusinessControllerClass"];
                object objController = Framework.Reflection.CreateObject(BusinessControllerClass, "");
                if (objController is IPortable)
                {
                    int ModuleId = Convert.ToInt32(message.Attributes["ModuleId"]);
                    string Content = HttpContext.Current.Server.HtmlDecode(message.Attributes["Content"]);
                    string Version = message.Attributes["Version"];
                    int UserID = Convert.ToInt32(message.Attributes["UserId"]);
                    ((IPortable)objController).ImportModule(ModuleId, Content, Version, UserID);
                    ModuleController.SynchronizeModule(ModuleId);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
        }
        private void UpgradeModule(EventMessage message)
        {
            try
            {
                string BusinessControllerClass = message.Attributes["BusinessControllerClass"];
                object objController = Framework.Reflection.CreateObject(BusinessControllerClass, "");
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                Services.Log.EventLog.LogInfo objEventLogInfo;
                if (objController is IUpgradeable)
                {
                    string[] UpgradeVersions = message.Attributes["UpgradeVersionsList"].ToString().Split(',');
                    foreach (string Version in UpgradeVersions)
                    {
                        string Results = ((IUpgradeable)objController).UpgradeModule(Version);
                        objEventLogInfo = new Services.Log.EventLog.LogInfo();
                        objEventLogInfo.AddProperty("Module Upgraded", BusinessControllerClass);
                        objEventLogInfo.AddProperty("Version", Version);
                        if (!string.IsNullOrEmpty(Results))
                        {
                            objEventLogInfo.AddProperty("Results", Results);
                        }
                        objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.MODULE_UPDATED.ToString();
                        objEventLog.AddLog(objEventLogInfo);
                    }
                }
                UpdateSupportedFeatures(objController, Convert.ToInt32(message.Attributes["DesktopModuleId"]));
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
        }
        private void UpdateSupportedFeatures(EventMessage message)
        {
            string BusinessControllerClass = message.Attributes["BusinessControllerClass"];
            object objController = Framework.Reflection.CreateObject(BusinessControllerClass, "");
            UpdateSupportedFeatures(objController, Convert.ToInt32(message.Attributes["DesktopModuleId"]));
        }
        private void UpdateSupportedFeatures(object objController, int desktopModuleId)
        {
            try
            {
                DesktopModuleInfo oDesktopModule = DesktopModuleController.GetDesktopModule(desktopModuleId, Null.NullInteger);
                if ((oDesktopModule != null))
                {
                    oDesktopModule.SupportedFeatures = 0;
                    oDesktopModule.IsPortable = (objController is IPortable);
                    oDesktopModule.IsSearchable = (objController is ISearchable);
                    oDesktopModule.IsUpgradeable = (objController is IUpgradeable);
                    DesktopModuleController.SaveDesktopModule(oDesktopModule, false, true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
        }
        public override bool ProcessMessage(EventMessage message)
        {
            try
            {
                switch (message.ProcessorCommand)
                {
                    case "UpdateSupportedFeatures":
                        UpdateSupportedFeatures(message);
                        break;
                    case "UpgradeModule":
                        UpgradeModule(message);
                        break;
                    case "ImportModule":
                        ImportModule(message);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                message.ExceptionMessage = ex.Message;
                return false;
            }
            return true;
        }
    }
}
