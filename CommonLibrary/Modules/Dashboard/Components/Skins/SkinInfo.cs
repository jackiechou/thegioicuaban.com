using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules.Dashboard.Components.Skins
{
    public class SkinInfo
    {
        private bool _InUse;
        public bool InUse
        {
            get { return _InUse; }
            set { _InUse = value; }
        }
        private string _SkinFile;
        public string SkinFile
        {
            get { return _SkinFile; }
            set { _SkinFile = value; }
        }
        private string _SkinName;
        public string SkinName
        {
            get { return _SkinName; }
            set { _SkinName = value; }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("skin");
            writer.WriteElementString("skinName", SkinName);
            writer.WriteElementString("inUse", InUse.ToString());
            writer.WriteEndElement();
        }
    }
}
