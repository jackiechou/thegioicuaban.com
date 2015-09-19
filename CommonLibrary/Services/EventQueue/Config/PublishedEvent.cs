using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CommonLibrary.Services.EventQueue.Config
{
    [Serializable(), XmlRoot("PublishedEvent")]
    public class PublishedEvent
    {
        private string _subscribers;
        private string _eventName;
        public string EventName
        {
            get { return _eventName; }
            set { _eventName = value; }
        }
        public string Subscribers
        {
            get { return _subscribers; }
            set { _subscribers = value; }
        }
    }
}
