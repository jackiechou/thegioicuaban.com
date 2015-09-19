using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace CommonLibrary.Entities.Portal
{
    [Serializable()]
    public class PortalAliasInfo : BaseEntityInfo, IXmlSerializable
    {
        private int _PortalID;
		private int _PortalAliasID;
		private string _HTTPAlias;
		public int PortalID {
			get { return _PortalID; }
			set { _PortalID= value; }
		}
		public int PortalAliasID {
			get { return _PortalAliasID; }
			set { _PortalAliasID= value; }
		}
		public string HTTPAlias {
			get { return _HTTPAlias; }
			set { _HTTPAlias= value; }
		}
		public XmlSchema GetSchema()
		{
			return null;
		}
		public void ReadXml(XmlReader reader)
		{
			while (reader.Read()) {
				if (reader.NodeType == XmlNodeType.EndElement) {
					break;
				} else if (reader.NodeType == XmlNodeType.Whitespace) {
					continue;
				} else {
					switch (reader.Name) {
						case "portalID":
							PortalID = reader.ReadElementContentAsInt();
							break;
						case "portalAliasID":
							PortalAliasID = reader.ReadElementContentAsInt();
							break;
						case "HTTPAlias":
							HTTPAlias = reader.ReadElementContentAsString();
							break;
					}
				}
			}
		}
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("portalAlias");
			writer.WriteElementString("portalID", PortalID.ToString());
			writer.WriteElementString("portalAliasID", PortalAliasID.ToString());
			writer.WriteElementString("HTTPAlias", HTTPAlias);
			writer.WriteEndElement();
		}	
    }
}
