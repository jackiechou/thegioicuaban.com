using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules.Dashboard.Components.Modules
{
    public class ModuleInfo
    {
        private int _DesktopModuleId;
        public int DesktopModuleId
        {
            get { return _DesktopModuleId; }
            set { _DesktopModuleId = value; }
        }
        private int _Instances;
        public int Instances
        {
            get { return _Instances; }
            set { _Instances = value; }
        }
        private string _FriendlyName;
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set { _FriendlyName = value; }
        }
        private string _ModuleName;
        public string ModuleName
        {
            get { return _ModuleName; }
            set { _ModuleName = value; }
        }
        private string _Version;
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("module");
            writer.WriteElementString("moduleName", ModuleName);
            writer.WriteElementString("friendlyName", FriendlyName);
            writer.WriteElementString("version", Version);
            writer.WriteElementString("instances", Instances.ToString());
            writer.WriteEndElement();
        }
    }
}
