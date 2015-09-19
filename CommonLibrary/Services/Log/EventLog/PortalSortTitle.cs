using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Entities.Portal;


namespace CommonLibrary.Services.Log.EventLog
{
    public class PortalSortTitle : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            return ((PortalInfo)x).PortalName.CompareTo(((PortalInfo)y).PortalName);
        }
    }
}
