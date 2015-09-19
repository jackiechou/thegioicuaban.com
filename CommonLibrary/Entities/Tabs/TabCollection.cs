using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Entities.Tabs
{
    [Serializable()]
    public class TabCollection : Dictionary<int, TabInfo>
    {
        private List<TabInfo> list;
        private Dictionary<int, List<TabInfo>> children;
        public TabCollection()
        {
            list = new List<TabInfo>();
            children = new Dictionary<int, List<TabInfo>>();
        }
        public TabCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public TabCollection(List<TabInfo> tabs)
            : this()
        {
            AddRange(tabs);
        }
        private int AddToChildren(TabInfo tab)
        {
            List<TabInfo> childList = null;
            if (!children.TryGetValue(tab.ParentId, out childList))
            {
                childList = new List<TabInfo>();
                children.Add(tab.ParentId, childList);
            }
            childList.Add(tab);
            return childList.Count;
        }
        public void Add(TabInfo tab)
        {
            Add(tab.TabID, tab);
            if (tab.ParentId == Null.NullInteger)
            {
                AddToChildren(tab);
                list.Add(tab);
            }
            else
            {
                for (int index = 0; index <= list.Count - 1; index++)
                {
                    TabInfo parentTab = list[index];
                    if (parentTab.TabID == tab.ParentId)
                    {
                        int childCount = AddToChildren(tab);
                        list.Insert(index + childCount, tab);
                    }
                }
            }
        }
        public void AddRange(List<TabInfo> tabs)
        {
            foreach (TabInfo tab in tabs)
            {
                Add(tab);
            }
        }
        public List<TabInfo> AsList()
        {
            return list;
        }
        public List<TabInfo> DescendentsOf(int tabId)
        {
            List<TabInfo> descendantTabs = new List<TabInfo>();
            for (int index = 0; index <= list.Count - 1; index++)
            {
                TabInfo parentTab = list[index];
                if (parentTab.TabID == tabId)
                {
                    for (int descendantIndex = index + 1; descendantIndex <= list.Count - 1; descendantIndex++)
                    {
                        TabInfo descendantTab = list[descendantIndex];
                        if (descendantTab.Level > parentTab.Level)
                        {
                            descendantTabs.Add(descendantTab);
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                }
            }
            return descendantTabs;
        }
        public ArrayList ToArrayList()
        {
            ArrayList tabs = new ArrayList();
            foreach (TabInfo tab in list)
            {
                tabs.Add(tab);
            }
            return tabs;
        }
        public List<TabInfo> WithParentId(int parentId)
        {
            List<TabInfo> tabs = null;
            if (!children.TryGetValue(parentId, out tabs))
            {
                tabs = new List<TabInfo>();
            }
            return tabs;
        }
    }
}
