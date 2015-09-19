using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Data;
using CommonLibrary.Entities.Users;
using CommonLibrary.Entities.Portal;
using System.Collections;
using CommonLibrary.Common.Utilities;
using System.Data;

namespace CommonLibrary.Common.Lists
{
    public class ListController
    {
        private void ClearCache(int PortalId)
        {
            DataCache.ClearListsCache(PortalId);
        }
        private ListInfo FillListInfo(IDataReader dr, bool CheckForOpenDataReader)
        {
            ListInfo objListInfo = null;
            bool canContinue = true;
            if (CheckForOpenDataReader)
            {
                canContinue = false;
                if (dr.Read())
                {
                    canContinue = true;
                }
            }
            if (canContinue)
            {
                objListInfo = new ListInfo(Convert.ToString(dr["ListName"]));
                {
                    objListInfo.Level = Convert.ToInt32(dr["Level"]);
                    objListInfo.PortalID = Convert.ToInt32(dr["PortalID"]);
                    objListInfo.DefinitionID = Convert.ToInt32(dr["DefinitionID"]);
                    objListInfo.EntryCount = Convert.ToInt32(dr["EntryCount"]);
                    objListInfo.ParentID = Convert.ToInt32(dr["ParentID"]);
                    objListInfo.ParentKey = Convert.ToString(dr["ParentKey"]);
                    objListInfo.Parent = Convert.ToString(dr["Parent"]);
                    objListInfo.ParentList = Convert.ToString(dr["ParentList"]);
                    objListInfo.EnableSortOrder = (Convert.ToInt32(dr["MaxSortOrder"]) > 0);
                    objListInfo.SystemList = Convert.ToInt32(dr["SystemList"]) > 0;
                }
            }
            return objListInfo;
        }
        private Dictionary<string, ListInfo> FillListInfoDictionary(IDataReader dr)
        {
            Dictionary<string, ListInfo> dic = new Dictionary<string, ListInfo>();
            try
            {
                ListInfo obj;
                while (dr.Read())
                {
                    obj = FillListInfo(dr, false);
                    if (!dic.ContainsKey(obj.Key))
                    {
                        dic.Add(obj.Key, obj);
                    }
                }
            }
            catch (Exception exc)
            {
                CommonLibrary.Services.Exceptions.Exceptions.LogException(exc);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            return dic;
        }
        private object GetListInfoDictionaryCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalId = (int)cacheItemArgs.ParamList[0];
            return FillListInfoDictionary(DataProvider.Instance().GetLists(portalId));
        }
        private Dictionary<string, ListInfo> GetListInfoDictionary(int PortalId)
        {
            string cacheKey = string.Format(DataCache.ListsCacheKey, PortalId.ToString());
            return CBO.GetCachedObject<Dictionary<string, ListInfo>>(new CacheItemArgs(cacheKey, DataCache.ListsCacheTimeOut, DataCache.ListsCachePriority, PortalId), GetListInfoDictionaryCallBack);
        }
        public int AddListEntry(ListEntryInfo ListEntry)
        {
            bool EnableSortOrder = (ListEntry.SortOrder > 0);
            ClearCache(ListEntry.PortalID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(ListEntry, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.LISTENTRY_CREATED);
            return DataProvider.Instance().AddListEntry(ListEntry.ListName, ListEntry.Value, ListEntry.Text, ListEntry.ParentID, ListEntry.Level, EnableSortOrder, ListEntry.DefinitionID, ListEntry.Description, ListEntry.PortalID, ListEntry.SystemList, UserController.GetCurrentUserInfo().UserID);
        }
        public void DeleteList(string ListName, string ParentKey)
        {
            ListInfo list = GetListInfo(ListName, ParentKey);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog("ListName", ListName.ToString(), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.LISTENTRY_DELETED);
            DataProvider.Instance().DeleteList(ListName, ParentKey);
            ClearCache(list.PortalID);
        }
        public void DeleteList(ListInfo list, bool includeChildren)
        {
            SortedList<string, ListInfo> lists = new SortedList<string, ListInfo>();
            lists.Add(list.Key, list);
            if (includeChildren)
            {
                foreach (KeyValuePair<string, ListInfo> listPair in GetListInfoDictionary(list.PortalID))
                {
                    if ((listPair.Value.ParentList.StartsWith(list.Key)))
                    {
                        lists.Add(listPair.Value.Key.Replace(":", "."), listPair.Value);
                    }
                }
            }
            for (int i = lists.Count - 1; i >= 0; i += -1)
            {
                DeleteList(lists.Values[i].Name, lists.Values[i].ParentKey);
            }
        }
        public void DeleteListEntryByID(int EntryID, bool DeleteChild)
        {
            ListEntryInfo entry = GetListEntryInfo(EntryID);
            DataProvider.Instance().DeleteListEntryByID(EntryID, DeleteChild);
            ClearCache(entry.PortalID);
        }
        public void DeleteListEntryByListName(string ListName, string Value, bool DeleteChild)
        {
            ListEntryInfo entry = GetListEntryInfo(ListName, Value);
            DataProvider.Instance().DeleteListEntryByListName(ListName, Value, DeleteChild);
            ClearCache(entry.PortalID);
        }
        public ListEntryInfo GetListEntryInfo(int EntryID)
        {
            return (ListEntryInfo)CBO.FillObject(DataProvider.Instance().GetListEntry(EntryID), typeof(ListEntryInfo));
        }
        public ListEntryInfo GetListEntryInfo(string ListName, string Value)
        {
            return (ListEntryInfo)CBO.FillObject(DataProvider.Instance().GetListEntry(ListName, Value), typeof(ListEntryInfo));
        }
        public ListEntryInfoCollection GetListEntryInfoCollection(string ListName)
        {
            return GetListEntryInfoCollection(ListName, "", Null.NullInteger);
        }
        public ListEntryInfoCollection GetListEntryInfoCollection(string ListName, string ParentKey)
        {
            return GetListEntryInfoCollection(ListName, ParentKey, Null.NullInteger);
        }
        public ListEntryInfoCollection GetListEntryInfoCollection(string ListName, string ParentKey, int PortalId)
        {
            ListEntryInfoCollection objListEntryInfoCollection = new ListEntryInfoCollection();
            ArrayList arrListEntries = CBO.FillCollection(DataProvider.Instance().GetListEntriesByListName(ListName, ParentKey, PortalId), typeof(ListEntryInfo));
            foreach (ListEntryInfo entry in arrListEntries)
            {
                objListEntryInfoCollection.Add(entry.Key, entry);
            }
            return objListEntryInfoCollection;
        }
        public ListInfo GetListInfo(string ListName)
        {
            return GetListInfo(ListName, "");
        }
        public ListInfo GetListInfo(string ListName, string ParentKey)
        {
            return GetListInfo(ListName, ParentKey, -1);
        }
        public ListInfo GetListInfo(string ListName, string ParentKey, int PortalID)
        {
            ListInfo list = null;
            string key = Null.NullString;
            if (!string.IsNullOrEmpty(ParentKey))
            {
                key = ParentKey + ":";
            }
            key += ListName;
            Dictionary<string, ListInfo> dicLists = GetListInfoDictionary(PortalID);
            if (!dicLists.TryGetValue(key, out list))
            {
                IDataReader dr = DataProvider.Instance().GetList(ListName, ParentKey, PortalID);
                try
                {
                    list = FillListInfo(dr, true);
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
            }
            return list;
        }
        public ListInfoCollection GetListInfoCollection()
        {
            return GetListInfoCollection("");
        }
        public ListInfoCollection GetListInfoCollection(string ListName)
        {
            return GetListInfoCollection(ListName, "");
        }
        public ListInfoCollection GetListInfoCollection(string ListName, string ParentKey)
        {
            return GetListInfoCollection(ListName, ParentKey, -1);
        }
        public ListInfoCollection GetListInfoCollection(string ListName, string ParentKey, int PortalID)
        {
            IList lists = new ListInfoCollection();
            foreach (KeyValuePair<string, ListInfo> listPair in GetListInfoDictionary(PortalID))
            {
                ListInfo list = listPair.Value;
                if ((list.Name == ListName || string.IsNullOrEmpty(ListName)) && (list.ParentKey == ParentKey || string.IsNullOrEmpty(ParentKey)) && (list.PortalID == PortalID || PortalID == Null.NullInteger))
                {
                    lists.Add(list);
                }
            }
            return (ListInfoCollection)lists;
        }
        public void UpdateListEntry(ListEntryInfo ListEntry)
        {
            DataProvider.Instance().UpdateListEntry(ListEntry.EntryID, ListEntry.Value, ListEntry.Text, ListEntry.Description, UserController.GetCurrentUserInfo().UserID);
            Services.Log.EventLog.EventLogController objEventLog = new Services.Log.EventLog.EventLogController();
            objEventLog.AddLog(ListEntry, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", CommonLibrary.Services.Log.EventLog.EventLogController.EventLogType.LISTENTRY_UPDATED);
            ClearCache(ListEntry.PortalID);
        }
        public void UpdateListSortOrder(int EntryID, bool MoveUp)
        {
            DataProvider.Instance().UpdateListSortOrder(EntryID, MoveUp);
            ListEntryInfo entry = GetListEntryInfo(EntryID);
            ClearCache(entry.PortalID);
        }
    }
}
