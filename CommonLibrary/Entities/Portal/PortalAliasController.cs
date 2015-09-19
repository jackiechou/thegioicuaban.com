using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonLibrary.Entities.Tabs;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Data;
using System.Collections;
using CommonLibrary.Entities.Users;

namespace CommonLibrary.Entities.Portal
{
    public class PortalAliasController
    {
        private static object GetPortalAliasLookupCallBack(CacheItemArgs cacheItemArgs)
        {
            return new PortalAliasController().GetPortalAliases();
        }
        public static string GetPortalAliasByPortal(int PortalId, string PortalAlias)
        {
            string retValue = "";
            PortalAliasCollection objPortalAliasCollection = PortalAliasController.GetPortalAliasLookup();
            string strHTTPAlias;
            bool bFound = false;
            PortalAliasInfo objPortalAliasInfo = objPortalAliasCollection[PortalAlias.ToLower()];
            if (objPortalAliasInfo != null)
            {
                if (objPortalAliasInfo.PortalID == PortalId)
                {
                    retValue = objPortalAliasInfo.HTTPAlias;
                    bFound = true;
                }
            }
            if (!bFound)
            {
                foreach (string key in objPortalAliasCollection.Keys)
                {
                    objPortalAliasInfo = objPortalAliasCollection[key];
                    strHTTPAlias = objPortalAliasInfo.HTTPAlias.ToLower();
                    if (strHTTPAlias.StartsWith(PortalAlias.ToLower()) == true && objPortalAliasInfo.PortalID == PortalId)
                    {
                        retValue = objPortalAliasInfo.HTTPAlias;
                        break;
                    }
                    if (strHTTPAlias.StartsWith("www."))
                    {
                        strHTTPAlias = strHTTPAlias.Replace("www.", "");
                    }
                    else
                    {
                        strHTTPAlias = string.Concat("www.", strHTTPAlias);
                    }
                    if (strHTTPAlias.StartsWith(PortalAlias.ToLower()) == true && objPortalAliasInfo.PortalID == PortalId)
                    {
                        retValue = objPortalAliasInfo.HTTPAlias;
                        break;
                    }
                }
            }
            return retValue;
        }
        public static string GetPortalAliasByTab(int TabID, string PortalAlias)
        {
            string retValue = Null.NullString;
            int intPortalId = -2;
            TabController objTabs = new TabController();
            TabInfo objTab = objTabs.GetTab(TabID, Null.NullInteger, false);
            if (objTab != null)
            {
                if (!objTab.IsDeleted)
                {
                    intPortalId = objTab.PortalID;
                }
            }
            switch (intPortalId)
            {
                case -2:
                    break;
                case -1:
                    retValue = PortalAlias;
                    break;
                default:
                    retValue = GetPortalAliasByPortal(intPortalId, PortalAlias);
                    break;
            }
            return retValue;
        }
        public static PortalAliasInfo GetPortalAliasInfo(string PortalAlias)
        {
            string strPortalAlias;
            PortalAliasInfo objPortalAliasInfo = GetPortalAliasLookup(PortalAlias.ToLower());
            if (objPortalAliasInfo == null)
            {
                if (PortalAlias.ToLower().StartsWith("www."))
                {
                    strPortalAlias = PortalAlias.Replace("www.", "");
                }
                else
                {
                    strPortalAlias = string.Concat("www.", PortalAlias);
                }
                objPortalAliasInfo = GetPortalAliasLookup(strPortalAlias.ToLower());
            }
            if (objPortalAliasInfo == null)
            {
                if (PortalAlias.IndexOf(".") != -1)
                {
                    strPortalAlias = PortalAlias.Substring(PortalAlias.IndexOf(".") + 1);
                }
                else
                {
                    strPortalAlias = PortalAlias;
                }
                if (objPortalAliasInfo == null)
                {
                    objPortalAliasInfo = GetPortalAliasLookup("*." + strPortalAlias.ToLower());
                }
                if (objPortalAliasInfo == null)
                {
                    objPortalAliasInfo = GetPortalAliasLookup(strPortalAlias.ToLower());
                }
                if (objPortalAliasInfo == null)
                {
                    objPortalAliasInfo = GetPortalAliasLookup("www." + strPortalAlias.ToLower());
                }
            }
            if (objPortalAliasInfo == null)
            {
                PortalAliasCollection objPortalAliasCollection = GetPortalAliasLookup();
                if (!objPortalAliasCollection.HasKeys || (objPortalAliasCollection.Count == 1 && objPortalAliasCollection.Contains("_default")))
                {
                    DataProvider.Instance().UpdatePortalAlias(PortalAlias.ToLower(), UserController.GetCurrentUserInfo().UserID);
                    Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog("PortalAlias", PortalAlias.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALALIAS_UPDATED);
                    DataCache.RemoveCache("GetPortalByAlias");
                    objPortalAliasInfo = GetPortalAliasLookup(PortalAlias.ToLower());
                }
            }
            return objPortalAliasInfo;
        }
        public static PortalAliasCollection GetPortalAliasLookup()
        {
            return CBO.GetCachedObject<PortalAliasCollection>(new CacheItemArgs(DataCache.PortalAliasCacheKey, DataCache.PortalAliasCacheTimeOut, DataCache.PortalAliasCachePriority), GetPortalAliasLookupCallBack, true);
        }

        public static PortalAliasInfo GetPortalAliasLookup(string aliasInfo)
        {
            return GetPortalAliasLookup()[aliasInfo];
        }
        public int AddPortalAlias(PortalAliasInfo objPortalAliasInfo)
        {
            int Id = DataProvider.Instance().AddPortalAlias(objPortalAliasInfo.PortalID, objPortalAliasInfo.HTTPAlias.ToLower(), UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objPortalAliasInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALALIAS_CREATED);
            DataCache.RemoveCache(DataCache.PortalAliasCacheKey);
            return Id;
        }
        public void DeletePortalAlias(int PortalAliasID)
        {
            DataCache.RemoveCache(DataCache.PortalAliasCacheKey);
            DataProvider.Instance().DeletePortalAlias(PortalAliasID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("PortalAliasID", PortalAliasID.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALALIAS_DELETED);
        }
        public PortalAliasInfo GetPortalAlias(string PortalAlias, int PortalID)
        {
            return (PortalAliasInfo)CBO.FillObject(DataProvider.Instance().GetPortalAlias(PortalAlias, PortalID), typeof(PortalAliasInfo));
        }
        public ArrayList GetPortalAliasArrayByPortalID(int PortalID)
        {
            IDataReader dr = DataProvider.Instance().GetPortalAliasByPortalID(PortalID);
            try
            {
                ArrayList arr = new ArrayList();
                while (dr.Read())
                {
                    PortalAliasInfo objPortalAliasInfo = new PortalAliasInfo();
                    objPortalAliasInfo.PortalAliasID = Convert.ToInt32(dr["PortalAliasID"]);
                    objPortalAliasInfo.PortalID = Convert.ToInt32(dr["PortalID"]);
                    objPortalAliasInfo.HTTPAlias = Convert.ToString(dr["HTTPAlias"]).ToLower();
                    arr.Add(objPortalAliasInfo);
                }
                return arr;
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
        }
        public PortalAliasInfo GetPortalAliasByPortalAliasID(int PortalAliasID)
        {
            return (PortalAliasInfo)CBO.FillObject((DataProvider.Instance().GetPortalAliasByPortalAliasID(PortalAliasID)), typeof(PortalAliasInfo));
        }
        public PortalAliasCollection GetPortalAliasByPortalID(int PortalID)
        {
            IDataReader dr = DataProvider.Instance().GetPortalAliasByPortalID(PortalID);
            try
            {
                PortalAliasCollection objPortalAliasCollection = new PortalAliasCollection();
                while (dr.Read())
                {
                    PortalAliasInfo objPortalAliasInfo = new PortalAliasInfo();
                    objPortalAliasInfo.PortalAliasID = Convert.ToInt32(dr["PortalAliasID"]);
                    objPortalAliasInfo.PortalID = Convert.ToInt32(dr["PortalID"]);
                    objPortalAliasInfo.HTTPAlias = Convert.ToString(dr["HTTPAlias"]);
                    objPortalAliasCollection.Add(Convert.ToString(dr["HTTPAlias"]).ToLower(), objPortalAliasInfo);
                }
                return objPortalAliasCollection;
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
        }
        public PortalAliasCollection GetPortalAliases()
        {
            return GetPortalAliasByPortalID(-1);
        }
        public PortalInfo GetPortalByPortalAliasID(int PortalAliasId)
        {
            return (PortalInfo)CBO.FillObject(DataProvider.Instance().GetPortalByPortalAliasID(PortalAliasId), typeof(PortalInfo));
        }
        public void UpdatePortalAliasInfo(PortalAliasInfo objPortalAliasInfo)
        {
            DataCache.RemoveCache(DataCache.PortalAliasCacheKey);
            DataProvider.Instance().UpdatePortalAliasInfo(objPortalAliasInfo.PortalAliasID, objPortalAliasInfo.PortalID, objPortalAliasInfo.HTTPAlias.ToLower(), UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(objPortalAliasInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.PORTALALIAS_UPDATED);
        }
    }
}
