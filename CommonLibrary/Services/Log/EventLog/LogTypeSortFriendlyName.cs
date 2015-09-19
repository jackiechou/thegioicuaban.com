using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Log.EventLog
{
    public class LogTypeSortFriendlyName : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return ((LogTypeInfo)x).LogTypeFriendlyName.CompareTo(((LogTypeInfo)y).LogTypeFriendlyName);
        }
    }
}
