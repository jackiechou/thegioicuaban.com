using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Entities.Tabs
{
    public class TabExistsException : System.Exception
    {
        private int _tabId = Null.NullInteger;
        public TabExistsException(int tabId, string message)
            : base(message)
        {
            _tabId = tabId;
        }
        public int TabId
        {
            get { return _tabId; }
        }
    }
}
