using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Security;

namespace CommonLibrary.Services.EventQueue.Config
{
    public class SubscriberInfo
    {
        private string _id;
        private string _name;
        private string _description;
        private string _address;
        private string _privateKey;
        public SubscriberInfo()
        {
            _id = System.Guid.NewGuid().ToString();
            _name = "";
            _description = "";
            _address = "";
            PortalSecurity oPortalSecurity = new PortalSecurity();
            _privateKey = oPortalSecurity.CreateKey(16);
        }
        public SubscriberInfo(string subscriberName)
            : this()
        {
            _name = subscriberName;
        }
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        public string PrivateKey
        {
            get { return _privateKey; }
            set { _privateKey = value; }
        }
    }
}
