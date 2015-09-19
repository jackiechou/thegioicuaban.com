using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Data;

namespace CommonLibrary.Modules.SearchInput
{
    public class SearchInputController
    {
        public ArrayList GetSearchResultModules(int PortalID)
        {
            return CBO.FillCollection(DataProvider.Instance().GetSearchResultModules(PortalID), typeof(SearchResultsModuleInfo));
        }
    }
}
