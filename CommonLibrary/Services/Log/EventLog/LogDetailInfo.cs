using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CommonLibrary.Services.Log.EventLog
{
    [Serializable()]
    public class LogDetailInfo
    {
        private string _PropertyName;
        private string _PropertyValue;
        public LogDetailInfo()
            : this("", "")
        {
        }
        public LogDetailInfo(string name, string value)
        {
            _PropertyName = name;
            _PropertyValue = value;
        }
        public string PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }
        public string PropertyValue
        {
            get { return _PropertyValue; }
            set { _PropertyValue = value; }
        }
        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("PropertyName");
            PropertyName = reader.ReadString();
            reader.ReadEndElement();
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement("PropertyValue");
                PropertyValue = reader.ReadString();
                reader.ReadEndElement();
            }
            else
            {
                reader.Read();
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<b>");
            sb.Append(PropertyName);
            sb.Append("</b>: ");
            sb.Append(PropertyValue);
            sb.Append(";&nbsp;");
            return sb.ToString();
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("LogProperty");
            writer.WriteElementString("PropertyName", PropertyName);
            writer.WriteElementString("PropertyValue", PropertyValue);
            writer.WriteEndElement();
        }
    }
}
