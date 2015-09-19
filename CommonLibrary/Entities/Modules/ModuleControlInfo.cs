using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Security;
using System.Xml.Serialization;

namespace CommonLibrary.Entities.Modules
{
    [Serializable()]
    public class ModuleControlInfo : ControlInfo, IXmlSerializable, IHydratable
    {
        private int _ModuleControlID = Null.NullInteger;
        private int _ModuleDefID = Null.NullInteger;
        private string _ControlTitle;
        private SecurityAccessLevel _ControlType = SecurityAccessLevel.Anonymous;
        private string _IconFile;
        private string _HelpURL;
        private int _ViewOrder;
        public int ModuleControlID
        {
            get { return _ModuleControlID; }
            set { _ModuleControlID = value; }
        }
        public int ModuleDefID
        {
            get { return _ModuleDefID; }
            set { _ModuleDefID = value; }
        }
        public string ControlTitle
        {
            get { return _ControlTitle; }
            set { _ControlTitle = value; }
        }
        public SecurityAccessLevel ControlType
        {
            get { return _ControlType; }
            set { _ControlType = value; }
        }
        public string IconFile
        {
            get { return _IconFile; }
            set { _IconFile = value; }
        }
        public string HelpURL
        {
            get { return _HelpURL; }
            set { _HelpURL = value; }
        }
        public int ViewOrder
        {
            get { return _ViewOrder; }
            set { _ViewOrder = value; }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            ModuleControlID = Null.SetNullInteger(dr["ModuleControlID"]);
            FillInternal(dr);
            ModuleDefID = Null.SetNullInteger(dr["ModuleDefID"]);
            ControlTitle = Null.SetNullString(dr["ControlTitle"]);
            IconFile = Null.SetNullString(dr["IconFile"]);
            HelpURL = Null.SetNullString(dr["HelpUrl"]);
            ControlType = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), Null.SetNullString(dr["ControlType"]));
            ViewOrder = Null.SetNullInteger(dr["ViewOrder"]);
            base.FillInternal(dr);
        }
        public int KeyID
        {
            get { return ModuleControlID; }
            set { ModuleControlID = value; }
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
                    switch (reader.Name)
                    {
                        case "controlTitle":
                            ControlTitle = reader.ReadElementContentAsString();
                            break;
                        case "controlType":
                            ControlType = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), reader.ReadElementContentAsString());
                            break;
                        case "iconFile":
                            IconFile = reader.ReadElementContentAsString();
                            break;
                        case "helpUrl":
                            HelpURL = reader.ReadElementContentAsString();
                            break;
                        case "viewOrder":
                            string elementvalue = reader.ReadElementContentAsString();
                            if (!string.IsNullOrEmpty(elementvalue))
                            {
                                ViewOrder = int.Parse(elementvalue);
                            }
                            break;
                    }
                }
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("moduleControl");
            WriteXmlInternal(writer);
            writer.WriteElementString("controlTitle", ControlTitle);
            writer.WriteElementString("controlType", ControlType.ToString());
            writer.WriteElementString("iconFile", IconFile);
            writer.WriteElementString("helpUrl", HelpURL);
            if (ViewOrder > Null.NullInteger)
            {
                writer.WriteElementString("viewOrder", ViewOrder.ToString());
            }
            writer.WriteEndElement();
        }
    }
}
