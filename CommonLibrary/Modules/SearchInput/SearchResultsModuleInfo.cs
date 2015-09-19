using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules.SearchInput
{
    public class SearchResultsModuleInfo
    {
        private int _tabID;
        private string _searchTabName;
        public int TabID
        {
            get { return _tabID; }
            set { _tabID = value; }
        }
        public string SearchTabName
        {
            get { return _searchTabName; }
            set { _searchTabName = value; }
        }
    }
}
