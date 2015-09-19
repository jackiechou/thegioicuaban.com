using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.Data;
using CommonLibrary.Data;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Services.Exceptions;

namespace CommonLibrary.Entities.Host
{
    public class HostSettingsController
    {
        public IDataReader GetHostSetting(string SettingName)
        {
            return DataProvider.Instance().GetHostSetting(SettingName);
        }
        public IDataReader GetHostSettings()
        {
            return DataProvider.Instance().GetHostSettings();
        }
        public void UpdateHostSetting(string SettingName, string SettingValue)
        {
            UpdateHostSetting(SettingName, SettingValue, false, true);
        }
        public void UpdateHostSetting(string SettingName, string SettingValue, bool SettingIsSecure)
        {
            UpdateHostSetting(SettingName, SettingValue, SettingIsSecure, true);
        }
        public void UpdateHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, bool clearCache)
        {
            IDataReader dr = null;
            try
            {
                dr = DataProvider.Instance().GetHostSetting(SettingName);
                Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                if (dr.Read())
                {
                    DataProvider.Instance().UpdateHostSetting(SettingName, SettingValue, SettingIsSecure, UserController.GetCurrentUserInfo().UserID);
                    objEventLog.AddLog(SettingName.ToString(), SettingValue.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.HOST_SETTING_UPDATED);
                }
                else
                {
                    DataProvider.Instance().AddHostSetting(SettingName, SettingValue, SettingIsSecure, UserController.GetCurrentUserInfo().UserID);
                    objEventLog.AddLog(SettingName.ToString(), SettingValue.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.HOST_SETTING_CREATED);
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            if (clearCache)
            {
                DataCache.ClearHostCache(false);
            }
        }
    }
}
