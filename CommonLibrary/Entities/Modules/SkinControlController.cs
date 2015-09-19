using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Data;

namespace CommonLibrary.Entities.Modules
{
    public class SkinControlController
    {
        private static DataProvider dataProvider = DataProvider.Instance();
        public static void DeleteSkinControl(SkinControlInfo skinControl)
        {
            dataProvider.DeleteSkinControl(skinControl.SkinControlID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(skinControl, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.SKINCONTROL_DELETED);
        }
        public static SkinControlInfo GetSkinControl(int skinControlID)
        {
            return CBO.FillObject<SkinControlInfo>(dataProvider.GetSkinControl(skinControlID));
        }
        public static SkinControlInfo GetSkinControlByPackageID(int packageID)
        {
            return CBO.FillObject<SkinControlInfo>(dataProvider.GetSkinControlByPackageID(packageID));
        }
        public static SkinControlInfo GetSkinControlByKey(string key)
        {
            return CBO.FillObject<SkinControlInfo>(dataProvider.GetSkinControlByKey(key));
        }
        public static Dictionary<string, SkinControlInfo> GetSkinControls()
        {
            return CBO.FillDictionary<string, SkinControlInfo>("ControlKey", dataProvider.GetSkinControls(), new Dictionary<string, SkinControlInfo>());
        }
        public static int SaveSkinControl(SkinControlInfo skinControl)
        {
            int skinControlID = skinControl.SkinControlID;
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            if (skinControlID == Null.NullInteger)
            {
                skinControlID = dataProvider.AddSkinControl(skinControl.PackageID, skinControl.ControlKey, skinControl.ControlSrc, skinControl.SupportsPartialRendering, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(skinControl, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.SKINCONTROL_CREATED);
            }
            else
            {
                dataProvider.UpdateSkinControl(skinControl.SkinControlID, skinControl.PackageID, skinControl.ControlKey, skinControl.ControlSrc, skinControl.SupportsPartialRendering, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(skinControl, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.SKINCONTROL_UPDATED);
            }
            return skinControlID;
        }
    }
}
