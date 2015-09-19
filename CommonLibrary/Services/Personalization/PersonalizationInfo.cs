using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Services.Personalization
{
    [Serializable()]
    public class PersonalizationInfo
    {
        private int _UserId;
        private int _PortalId;
        private bool _IsModified;
        private Hashtable _Profile;
        public PersonalizationInfo()
        {
        }
        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        public int PortalId
        {
            get { return _PortalId; }
            set { _PortalId = value; }
        }
        public bool IsModified
        {
            get { return _IsModified; }
            set { _IsModified = value; }
        }
        public Hashtable Profile
        {
            get { return _Profile; }
            set { _Profile = value; }
        }
    }
}
