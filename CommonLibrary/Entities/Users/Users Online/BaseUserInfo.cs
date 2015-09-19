using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Users.Users_Online
{
    [Serializable()]
    public abstract class BaseUserInfo
    {
        private int _PortalID;
        private int _TabID;
        private DateTime _CreationDate;
        private DateTime _LastActiveDate;
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        public int TabID
        {
            get { return _TabID; }
            set { _TabID = value; }
        }
        public DateTime CreationDate
        {
            get { return _CreationDate; }
            set { _CreationDate = value; }
        }
        public DateTime LastActiveDate
        {
            get { return _LastActiveDate; }
            set { _LastActiveDate = value; }
        }
    }
}
