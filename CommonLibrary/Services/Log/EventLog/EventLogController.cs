using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Users;
using CommonLibrary.Security.Roles;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Entities.Tabs;

namespace CommonLibrary.Services.Log.EventLog
{
    public partial class EventLogController : LogController
    {
        public void AddLog(LogInfo objEventLogInfo)
        {
            LogController objLogController = new LogController();
            objLogController.AddLog(objEventLogInfo);
        }
        public void AddLog(object objCBO, PortalSettings _PortalSettings, int UserID, string UserName, string LogType)
        {
            LogController objLogController = new LogController();
            LogInfo objLogInfo = new LogInfo();
            objLogInfo.LogUserID = UserID;
            objLogInfo.LogTypeKey = LogType.ToString();
            if (_PortalSettings != null)
            {
                objLogInfo.LogPortalID = _PortalSettings.PortalId;
                objLogInfo.LogPortalName = _PortalSettings.PortalName;
            }
            switch (objCBO.GetType().FullName)
            {
                case "CommonLibrary.Entities.Portals.PortalInfo":
                    PortalInfo objPortal = (PortalInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objPortal.PortalID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalName", objPortal.PortalName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Description", objPortal.Description));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("KeyWords", objPortal.KeyWords));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("LogoFile", objPortal.LogoFile));
                    break;
                case "CommonLibrary.Entities.Tabs.TabInfo":
                    TabInfo objTab = (TabInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("TabID", objTab.TabID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objTab.PortalID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("TabName", objTab.TabName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Title", objTab.Title));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Description", objTab.Description));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("KeyWords", objTab.KeyWords));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Url", objTab.Url));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ParentId", objTab.ParentId.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("IconFile", objTab.IconFile));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("IsVisible", objTab.IsVisible.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("SkinSrc", objTab.SkinSrc));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ContainerSrc", objTab.ContainerSrc));
                    break;
                case "CommonLibrary.Entities.Modules.ModuleInfo":
                    ModuleInfo objModule = (ModuleInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleId", objModule.ModuleID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleTitle", objModule.ModuleTitle));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("TabModuleID", objModule.TabModuleID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("TabID", objModule.TabID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objModule.PortalID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ModuleDefId", objModule.ModuleDefID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("FriendlyName", objModule.DesktopModule.FriendlyName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("IconFile", objModule.IconFile));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Visibility", objModule.Visibility.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("ContainerSrc", objModule.ContainerSrc));
                    break;
                case "CommonLibrary.Entities.Users.UserInfo":
                    UserInfo objUser = (UserInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("UserID", objUser.UserID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("FirstName", objUser.Profile.FirstName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("LastName", objUser.Profile.LastName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("UserName", objUser.Username));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Email", objUser.Email));
                    break;
                case "CommonLibrary.Security.Roles.RoleInfo":
                    RoleInfo objRole = (RoleInfo)objCBO;
                    objLogInfo.LogProperties.Add(new LogDetailInfo("RoleID", objRole.RoleID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("RoleName", objRole.RoleName));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("PortalID", objRole.PortalID.ToString()));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("Description", objRole.Description));
                    objLogInfo.LogProperties.Add(new LogDetailInfo("IsPublic", objRole.IsPublic.ToString()));
                    break;
                default:
                    objLogInfo.LogProperties.Add(new LogDetailInfo("logdetail", XmlUtils.Serialize(objCBO)));
                    break;
            }
            objLogController.AddLog(objLogInfo);
        }
        public void AddLog(object objCBO, PortalSettings _PortalSettings, int UserID, string UserName, Services.Log.EventLog.EventLogController.EventLogType objLogType)
        {
            AddLog(objCBO, _PortalSettings, UserID, UserName, objLogType.ToString());
        }
        public void AddLog(PortalSettings _PortalSettings, int UserID, Services.Log.EventLog.EventLogController.EventLogType objLogType)
        {
            LogProperties objProperties = new LogProperties();
            AddLog(objProperties, _PortalSettings, UserID, objLogType.ToString(), false);
        }
        public void AddLog(string PropertyName, string PropertyValue, PortalSettings _PortalSettings, int UserID, Services.Log.EventLog.EventLogController.EventLogType objLogType)
        {
            LogProperties objProperties = new LogProperties();
            LogDetailInfo objLogDetailInfo = new LogDetailInfo();
            objLogDetailInfo.PropertyName = PropertyName;
            objLogDetailInfo.PropertyValue = PropertyValue;
            objProperties.Add(objLogDetailInfo);
            AddLog(objProperties, _PortalSettings, UserID, objLogType.ToString(), false);
        }
        public void AddLog(string PropertyName, string PropertyValue, PortalSettings _PortalSettings, int UserID, string LogType)
        {
            LogProperties objProperties = new LogProperties();
            LogDetailInfo objLogDetailInfo = new LogDetailInfo();
            objLogDetailInfo.PropertyName = PropertyName;
            objLogDetailInfo.PropertyValue = PropertyValue;
            objProperties.Add(objLogDetailInfo);
            AddLog(objProperties, _PortalSettings, UserID, LogType, false);
        }
        public void AddLog(LogProperties objProperties, PortalSettings _PortalSettings, int UserID, string LogTypeKey, bool BypassBuffering)
        {
            LogController objLogController = new LogController();
            LogInfo objLogInfo = new LogInfo();
            objLogInfo.LogUserID = UserID;
            objLogInfo.LogTypeKey = LogTypeKey;
            objLogInfo.LogProperties = objProperties;
            objLogInfo.BypassBuffering = BypassBuffering;
            if (_PortalSettings != null)
            {
                objLogInfo.LogPortalID = _PortalSettings.PortalId;
                objLogInfo.LogPortalName = _PortalSettings.PortalName;
            }
            objLogController.AddLog(objLogInfo);
        }
    }
}
