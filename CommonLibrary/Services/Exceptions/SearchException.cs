using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Search;

namespace CommonLibrary.Services.Exceptions
{
    public class SearchException : BasePortalException
    {
        private SearchItemInfo m_SearchItem;
        public SearchException()
            : base()
        {
        }
        public SearchException(string message, Exception inner, SearchItemInfo searchItem)
            : base(message, inner)
        {

            m_SearchItem = searchItem;
        }
        public SearchItemInfo SearchItem
        {
            get { return m_SearchItem; }
        }
    }
}
