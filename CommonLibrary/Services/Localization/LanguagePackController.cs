using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Data;
using CommonLibrary.Entities.Portal;
using CommonLibrary.Entities.Users;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Localization
{
    public class LanguagePackController
    {
        public static void DeleteLanguagePack(LanguagePackInfo languagePack)
        {
            if (languagePack.PackageType == LanguagePackType.Core)
            {
                Locale language = Localization.GetLocaleByID(languagePack.LanguageID);
                if (language != null)
                {
                    Localization.DeleteLanguage(language);
                }
            }
            DataProvider.Instance().DeleteLanguagePack(languagePack.LanguagePackID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(languagePack, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGEPACK_DELETED);
        }
        public static LanguagePackInfo GetLanguagePackByPackage(int packageID)
        {
            return CBO.FillObject<LanguagePackInfo>(DataProvider.Instance().GetLanguagePackByPackage(packageID));
        }
        public static void SaveLanguagePack(LanguagePackInfo languagePack)
        {
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            if (languagePack.LanguagePackID == Null.NullInteger)
            {
                languagePack.LanguagePackID = DataProvider.Instance().AddLanguagePack(languagePack.PackageID, languagePack.LanguageID, languagePack.DependentPackageID, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(languagePack, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGEPACK_CREATED);
            }
            else
            {
                DataProvider.Instance().UpdateLanguagePack(languagePack.LanguagePackID, languagePack.PackageID, languagePack.LanguageID, languagePack.DependentPackageID, UserController.GetCurrentUserInfo().UserID);
                objEventLog.AddLog(languagePack, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", Log.EventLog.EventLogController.EventLogType.LANGUAGEPACK_UPDATED);
            }
        }
    }
}
