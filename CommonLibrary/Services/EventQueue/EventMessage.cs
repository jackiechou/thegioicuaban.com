using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using CommonLibrary.Common.Utilities;
using System.Collections.Specialized;

namespace CommonLibrary.Services.EventQueue
{
    public enum MessagePriority
    {
        High,
        Medium,
        Low
    }
    [Serializable()]
    public class EventMessage
    {
        private int _eventMessageID = Null.NullInteger;
        private string _processorType = Null.NullString;
        private string _processorCommand = Null.NullString;
        private string _body = Null.NullString;
        private MessagePriority _priority = MessagePriority.Low;
        private NameValueCollection _attributes;
        private string _sender = Null.NullString;
        private string _subscribers = Null.NullString;
        private string _authorizedRoles = Null.NullString;
        private DateTime _expirationDate;
        private string _exceptionMessage = Null.NullString;
        private DateTime _sentDate;
        public EventMessage()
        {
            _attributes = new NameValueCollection();
        }
        public int EventMessageID
        {
            get { return _eventMessageID; }
            set { _eventMessageID = value; }
        }
        public string ProcessorType
        {
            get
            {
                if (_processorType == null)
                {
                    return string.Empty;
                }
                else
                {
                    return _processorType;
                }
            }
            set { _processorType = value; }
        }
        public string ProcessorCommand
        {
            get
            {
                if (_processorCommand == null)
                {
                    return string.Empty;
                }
                else
                {
                    return _processorCommand;
                }
            }
            set { _processorCommand = value; }
        }
        public string Body
        {
            get
            {
                if (_body == null)
                {
                    return string.Empty;
                }
                else
                {
                    return _body;
                }
            }
            set { _body = value; }
        }
        public string Sender
        {
            get
            {
                if (_sender == null)
                {
                    return string.Empty;
                }
                else
                {
                    return _sender;
                }
            }
            set { _sender = value; }
        }
        public string Subscribers
        {
            get
            {
                if (_subscribers == null)
                {
                    return string.Empty;
                }
                else
                {
                    return _subscribers;
                }
            }
            set { _subscribers = value; }
        }
        public string AuthorizedRoles
        {
            get
            {
                if (_authorizedRoles == null)
                {
                    return string.Empty;
                }
                else
                {
                    return _authorizedRoles;
                }
            }
            set { _authorizedRoles = value; }
        }
        public MessagePriority Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
        public string ExceptionMessage
        {
            get
            {
                if (_exceptionMessage == null)
                {
                    return string.Empty;
                }
                else
                {
                    return _exceptionMessage;
                }
            }
            set { _exceptionMessage = value; }
        }
        public DateTime SentDate
        {
            get { return _sentDate.ToLocalTime(); }
            set { _sentDate = value.ToUniversalTime(); }
        }
        public DateTime ExpirationDate
        {
            get { return _expirationDate.ToLocalTime(); }
            set { _expirationDate = value.ToUniversalTime(); }
        }
        public NameValueCollection Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
        public void DeserializeAttributes(string configXml)
        {
            string attName = Null.NullString;
            string attValue = Null.NullString;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader reader = XmlReader.Create(new StringReader(configXml));
            reader.ReadStartElement("Attributes");
            if (!reader.IsEmptyElement)
            {
                do
                {
                    reader.ReadStartElement("Attribute");
                    reader.ReadStartElement("Name");
                    attName = reader.ReadString();
                    reader.ReadEndElement();
                    if (!reader.IsEmptyElement)
                    {
                        reader.ReadStartElement("Value");
                        attValue = reader.ReadString();
                        reader.ReadEndElement();
                    }
                    else
                    {
                        reader.Read();
                    }
                    _attributes.Add(attName, attValue);
                } while (reader.ReadToNextSibling("Attribute"));
            }
        }
        public string SerializeAttributes()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.OmitXmlDeclaration = true;
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartElement("Attributes");
            foreach (string key in this.Attributes.Keys)
            {
                writer.WriteStartElement("Attribute");
                writer.WriteElementString("Name", key);
                if (this.Attributes[key].IndexOfAny("<'>\"&".ToCharArray()) > -1)
                {
                    writer.WriteStartElement("Value");
                    writer.WriteCData(this.Attributes[key]);
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteElementString("Value", this.Attributes[key]);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Close();
            return sb.ToString();
        }
    }
}
