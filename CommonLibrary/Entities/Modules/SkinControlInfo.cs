using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using CommonLibrary.Common.Utilities;
using System.Xml.Serialization;

namespace CommonLibrary.Entities.Modules
{
    [Serializable()]
    public class SkinControlInfo : ControlInfo, IXmlSerializable, IHydratable
    {
        private int _SkinControlID = Null.NullInteger;
        private int _PackageID = Null.NullInteger;
        public int SkinControlID
        {
            get { return _SkinControlID; }
            set { _SkinControlID = value; }
        }
        public int PackageID
        {
            get { return _PackageID; }
            set { _PackageID = value; }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            SkinControlID = Null.SetNullInteger(dr["SkinControlID"]);
            PackageID = Null.SetNullInteger(dr["PackageID"]);
            FillInternal(dr);
        }
        public int KeyID
        {
            get { return SkinControlID; }
            set { SkinControlID = value; }
        }
        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if (reader.NodeType == XmlNodeType.Whitespace)
                {
                    continue;
                }
                else
                {
                    ReadXmlInternal(reader);
                }
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("moduleControl");
            WriteXmlInternal(writer);
            writer.WriteEndElement();
        }
    }
}
