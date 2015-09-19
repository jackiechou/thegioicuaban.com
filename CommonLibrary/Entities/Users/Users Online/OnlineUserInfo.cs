using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Users.Users_Online
{
    [Serializable()]
    public class OnlineUserInfo : BaseUserInfo
    {
        private int _UserID;
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
    }
}
