using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Services.Search
{
    [Serializable()]
    public class SearchItemInfoCollection : CollectionBase
    {
        public SearchItemInfoCollection()
            : base()
        {
        }
        public SearchItemInfoCollection(SearchItemInfoCollection value)
            : base()
        {
            AddRange(value);
        }
        public SearchItemInfoCollection(SearchItemInfo[] value)
            : base()
        {
            AddRange(value);
        }
        public SearchItemInfoCollection(ArrayList value)
            : base()
        {
            AddRange(value);
        }
        public SearchItemInfo this[int index]
        {
            get { return (SearchItemInfo)List[index]; }
            set { List[index] = value; }
        }
        public int Add(SearchItemInfo value)
        {
            return List.Add(value);
        }
        public int IndexOf(SearchItemInfo value)
        {
            return List.IndexOf(value);
        }
        public void Insert(int index, SearchItemInfo value)
        {
            List.Insert(index, value);
        }
        public void Remove(SearchItemInfo value)
        {
            List.Remove(value);
        }
        public bool Contains(SearchItemInfo value)
        {
            return List.Contains(value);
        }
        public void AddRange(SearchItemInfo[] value)
        {
            for (int i = 0; i <= value.Length - 1; i++)
            {
                Add(value[i]);
            }
        }
        public void AddRange(ArrayList value)
        {
            foreach (object obj in value)
            {
                if (obj is SearchItemInfo)
                {
                    Add((SearchItemInfo)obj);
                }
            }
        }
        public void AddRange(SearchItemInfoCollection value)
        {
            for (int i = 0; i <= value.Count - 1; i++)
            {
                Add((SearchItemInfo)value.List[i]);
            }
        }
        public void CopyTo(SearchItemInfo[] array, int index)
        {
            List.CopyTo(array, index);
        }
        public SearchItemInfo[] ToArray()
        {
            SearchItemInfo[] arr = new SearchItemInfo[Count];
            CopyTo(arr, 0);
            return arr;
        }
        public SearchItemInfoCollection ModuleItems(int ModuleId)
        {
            SearchItemInfoCollection retValue = new SearchItemInfoCollection();
            foreach (SearchItemInfo info in this)
            {
                if (info.ModuleId == ModuleId)
                {
                    retValue.Add(info);
                }
            }
            return retValue;
        }
    }
}
