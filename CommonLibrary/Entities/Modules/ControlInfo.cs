using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Entities.Modules
{
    [Serializable()]
    public abstract class ControlInfo : BaseEntityInfo
    {
        private string _ControlKey;
        private string _ControlSrc;
        private bool _SupportsPartialRendering = Null.NullBoolean;
        public string ControlKey
        {
            get { return _ControlKey; }
            set { _ControlKey = value; }
        }
        public string ControlSrc
        {
            get { return _ControlSrc; }
            set { _ControlSrc = value; }
        }
        public bool SupportsPartialRendering
        {
            get { return _SupportsPartialRendering; }
            set { _SupportsPartialRendering = value; }
        }
        protected override void FillInternal(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);
            ControlKey = Null.SetNullString(dr["ControlKey"]);
            ControlSrc = Null.SetNullString(dr["ControlSrc"]);
            SupportsPartialRendering = Null.SetNullBoolean(dr["SupportsPartialRendering"]);
        }
        protected void ReadXmlInternal(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "controlKey":
                    ControlKey = reader.ReadElementContentAsString();
                    break;
                case "controlSrc":
                    ControlSrc = reader.ReadElementContentAsString();
                    break;
                case "supportsPartialRendering":
                    string elementvalue = reader.ReadElementContentAsString();
                    if (!string.IsNullOrEmpty(elementvalue))
                    {
                        SupportsPartialRendering = bool.Parse(elementvalue);
                    }
                    break;
            }
        }
        protected void WriteXmlInternal(XmlWriter writer)
        {
            writer.WriteElementString("controlKey", ControlKey);
            writer.WriteElementString("controlSrc", ControlSrc);
            writer.WriteElementString("supportsPartialRendering", SupportsPartialRendering.ToString());
        }
    }
}
