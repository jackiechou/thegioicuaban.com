using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;

namespace CommonLibrary.Services.Log.EventLog
{
    public class LogProperties : ArrayList
    {
        public string Summary
        {
            get
            {
                string summary = this.ToString();
                if (summary.Length > 75)
                    summary = summary.Substring(0, 75);
                return summary;
            }
        }
        public void Deserialize(string content)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                reader.ReadStartElement("LogProperties");
                if (reader.ReadState != ReadState.EndOfFile && reader.NodeType != XmlNodeType.None && !String.IsNullOrEmpty(reader.LocalName))
                {
                    ReadXml(reader);
                }
                reader.Close();
            }
        }
        public void ReadXml(XmlReader reader)
        {
            do
            {
                reader.ReadStartElement("LogProperty");
                LogDetailInfo logDetail = new LogDetailInfo();
                logDetail.ReadXml(reader);
                this.Add(logDetail);
            } while (reader.ReadToNextSibling("LogProperty"));
        }
        public string Serialize()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.OmitXmlDeclaration = true;
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            WriteXml(writer);
            writer.Close();
            return sb.ToString();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogDetailInfo logDetail in this)
            {
                sb.Append(logDetail.ToString());
            }
            return sb.ToString();
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("LogProperties");
            foreach (LogDetailInfo logDetail in this)
            {
                logDetail.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}
