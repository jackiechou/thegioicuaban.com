using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Common.Lists
{
    [Serializable()]
    public class ListInfoCollection : CollectionBase
    {
        private Hashtable mKeyIndexLookup = new Hashtable();
        public ListInfoCollection()
            : base()
        {

        }
        public ListInfo GetChildren(string ParentName)
        {
            return (ListInfo)Item(ParentName);
        }
        internal new void Clear()
        {
            mKeyIndexLookup.Clear();
            base.Clear();
        }
        public void Add(string key, object value)
        {
            int index;
            try
            {
                index = base.List.Add(value);
                mKeyIndexLookup.Add(key.ToLower(), index);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public object Item(int index)
        {
            try
            {
                object obj;
                obj = base.List[index];
                return obj;
            }
            catch (System.Exception exc)
            {
                exc.ToString();
                return null;
            }
        }
        public object Item(string key)
        {
            int index;
            object obj;
            try
            {
                if (mKeyIndexLookup[key.ToLower()] == null)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
            index = Convert.ToInt32(mKeyIndexLookup[key.ToLower()]);
            obj = base.List[index];
            return obj;
        }
        public object Item(string key, bool Cache)
        {
            int index;
            object obj = null;
            bool itemExists = false;
            try
            {
                if (mKeyIndexLookup[key.ToLower()] != null)
                {
                    itemExists = true;
                }
            }
            catch (Exception ex)
            {
            }
            if (!itemExists)
            {
                ListController ctlLists = new ListController();
                string listName = key.Substring(key.IndexOf(":") + 1);
                string parentKey = key.Replace(listName, "").TrimEnd(':');
                ListInfo listInfo = ctlLists.GetListInfo(listName, parentKey);
                if (Cache)
                {
                    this.Add(listInfo.Key, listInfo);
                    return listInfo;
                }
            }
            else
            {
                index = Convert.ToInt32(mKeyIndexLookup[key.ToLower()]);
                obj = base.List[index];
            }
            return obj;
        }
        public ArrayList GetChild(string ParentKey)
        {
            ArrayList childList = new ArrayList();
            foreach (object child in List)
            {
                if (((ListInfo)child).Key.IndexOf(ParentKey.ToLower()) > -1)
                {
                    childList.Add(child);
                }
            }
            return childList;
        }
    }
}
