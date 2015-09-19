using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Users.Users_Online
{
    [Serializable()]
    public class AnonymousUserInfo : BaseUserInfo
    {
        private string _UserID;
        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
    }
}
