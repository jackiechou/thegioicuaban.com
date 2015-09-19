using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Common.Lists
{
    [Serializable()]
    public class ListEntryInfoCollection : CollectionBase
    {
        private Hashtable mKeyIndexLookup = new Hashtable();
        public ListEntryInfoCollection()
            : base()
        {
        }
        public ListEntryInfo GetChildren(string ParentName)
        {
            return (ListEntryInfo)this[ParentName];
        }
        internal new void Clear()
        {
            mKeyIndexLookup.Clear();
            base.Clear();
        }
        public void Add(string key, ListEntryInfo value)
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
        public ListEntryInfo this[int index]
        {
            get
            {
                try
                {
                    return (ListEntryInfo)base.List[index];
                }
                catch (System.Exception Exc)
                {
                    Exc.ToString();
                    return null;
                }
            }
        }
        public ListEntryInfo this[string key]
        {
            get
            {
                int index;
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
                return (ListEntryInfo)base.List[index];
            }
        }
    }
}
