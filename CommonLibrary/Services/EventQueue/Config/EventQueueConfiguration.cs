using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLibrary.Common.Utilities;
using System.Web.Caching;
using CommonLibrary.Common;
using System.Xml;

namespace CommonLibrary.Services.EventQueue.Config
{
    internal class EventQueueConfiguration
    {
        private Dictionary<string, PublishedEvent> _publishedEvents;
        private Dictionary<string, SubscriberInfo> _eventQueueSubscribers;
        internal EventQueueConfiguration()
        {
            _publishedEvents = new Dictionary<string, PublishedEvent>();
            _eventQueueSubscribers = new Dictionary<string, SubscriberInfo>();
        }
        internal Dictionary<string, SubscriberInfo> EventQueueSubscribers
        {
            get { return _eventQueueSubscribers; }
            set { _eventQueueSubscribers = value; }
        }
        internal Dictionary<string, PublishedEvent> PublishedEvents
        {
            get { return _publishedEvents; }
            set { _publishedEvents = value; }
        }
        private void Deserialize(string configXml)
        {
            if (!String.IsNullOrEmpty(configXml))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(configXml);
                foreach (XmlElement xmlItem in xmlDoc.SelectNodes("/EventQueueConfig/PublishedEvents/Event"))
                {
                    PublishedEvent oPublishedEvent = new PublishedEvent();
                    oPublishedEvent.EventName = xmlItem.SelectSingleNode("EventName").InnerText;
                    oPublishedEvent.Subscribers = xmlItem.SelectSingleNode("Subscribers").InnerText;
                    this.PublishedEvents.Add(oPublishedEvent.EventName, oPublishedEvent);
                }
                foreach (XmlElement xmlItem in xmlDoc.SelectNodes("/EventQueueConfig/EventQueueSubscribers/Subscriber"))
                {
                    SubscriberInfo oSubscriberInfo = new SubscriberInfo();
                    oSubscriberInfo.ID = xmlItem.SelectSingleNode("ID").InnerText;
                    oSubscriberInfo.Name = xmlItem.SelectSingleNode("Name").InnerText;
                    oSubscriberInfo.Address = xmlItem.SelectSingleNode("Address").InnerText;
                    oSubscriberInfo.Description = xmlItem.SelectSingleNode("Description").InnerText;
                    oSubscriberInfo.PrivateKey = xmlItem.SelectSingleNode("PrivateKey").InnerText;
                    this.EventQueueSubscribers.Add(oSubscriberInfo.ID, oSubscriberInfo);
                }
            }
        }

        public static void RegisterEventSubscription(EventQueueConfiguration config, string eventname, SubscriberInfo subscriber)
        {
            PublishedEvent e = new PublishedEvent();
            e.EventName = eventname;
            e.Subscribers = subscriber.ID;
            config.PublishedEvents.Add(e.EventName, e);
            if (!config.EventQueueSubscribers.ContainsKey(subscriber.ID))
            {
                config.EventQueueSubscribers.Add(subscriber.ID, subscriber);
            }
        }

        private string Serialize()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = true;
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = false;
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartElement("EventQueueConfig");
            writer.WriteStartElement("PublishedEvents");
            foreach (string key in this.PublishedEvents.Keys)
            {
                writer.WriteStartElement("Event");
                writer.WriteElementString("EventName", this.PublishedEvents[key].EventName);
                writer.WriteElementString("Subscribers", this.PublishedEvents[key].Subscribers);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("EventQueueSubscribers");
            foreach (string key in this.EventQueueSubscribers.Keys)
            {
                writer.WriteStartElement("Subscriber");
                writer.WriteElementString("ID", this.EventQueueSubscribers[key].ID);
                writer.WriteElementString("Name", this.EventQueueSubscribers[key].Name);
                writer.WriteElementString("Address", this.EventQueueSubscribers[key].Address);
                writer.WriteElementString("Description", this.EventQueueSubscribers[key].Description);
                writer.WriteElementString("PrivateKey", this.EventQueueSubscribers[key].PrivateKey);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
            return sb.ToString();
        }
        static internal EventQueueConfiguration GetConfig()
        {
            EventQueueConfiguration config = (EventQueueConfiguration)DataCache.GetCache("EventQueueConfig");
            if ((config == null))
            {
                string filePath = Globals.HostMapPath + "EventQueue\\EventQueue.config";
                if (File.Exists(filePath))
                {
                    config = new EventQueueConfiguration();
                    config.Deserialize(FileSystemUtils.ReadFile(filePath));
                    //Set back into Cache
                   // DataCache.SetCache("EventQueueConfig", config, new CacheDependency(filePath));
                }
                else
                {
                    config = new EventQueueConfiguration();
                    config.PublishedEvents = new Dictionary<string, PublishedEvent>();
                    config.EventQueueSubscribers = new Dictionary<string, SubscriberInfo>();
                    SubscriberInfo subscriber = new SubscriberInfo("DNN Core");
                    RegisterEventSubscription(config, "Application_Start", subscriber);
                    RegisterEventSubscription(config, "Application_Start_FirstRequest", subscriber);
                    SaveConfig(config, filePath);
                }
            }
            return config;
        }

        internal static void SaveConfig(EventQueueConfiguration config, string filePath)
        {
            StreamWriter oStream = File.CreateText(filePath);
            oStream.WriteLine(config.Serialize());
            oStream.Close();
            //Set back into Cache
            //DataCache.SetCache("EventQueueConfig", config, new CacheDependency(filePath));
        }
    }
}
