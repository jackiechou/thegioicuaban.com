using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommonLibrary.Common.Utilities;
using System.Xml.Schema;
using CommonLibrary.Security.Permissions;
using System.Xml.Serialization;

namespace CommonLibrary.Entities.Modules.Definitions
{
    [Serializable()]
    public class ModuleDefinitionInfo : IXmlSerializable, IHydratable
    {
        private int _DefaultCacheTime = 0;
        private int _DesktopModuleID = Null.NullInteger;
        private string _FriendlyName;
        private Dictionary<string, ModuleControlInfo> _ModuleControls;
        private int _ModuleDefID = Null.NullInteger;
        private Dictionary<string, PermissionInfo> _Permissions = new Dictionary<string, PermissionInfo>();
        private int _TempModuleID;
        public int ModuleDefID
        {
            get { return _ModuleDefID; }
            set { _ModuleDefID = value; }
        }
        public int DefaultCacheTime
        {
            get { return _DefaultCacheTime; }
            set { _DefaultCacheTime = value; }
        }
        public int DesktopModuleID
        {
            get { return _DesktopModuleID; }
            set { _DesktopModuleID = value; }
        }
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set { _FriendlyName = value; }
        }
        public Dictionary<string, ModuleControlInfo> ModuleControls
        {
            get
            {
                if (_ModuleControls == null)
                {
                    LoadControls();
                }
                return _ModuleControls;
            }
        }
        public Dictionary<string, PermissionInfo> Permissions
        {
            get { return _Permissions; }
        }
        public void LoadControls()
        {
            if (ModuleDefID > Null.NullInteger)
            {
                _ModuleControls = ModuleControlController.GetModuleControlsByModuleDefinitionID(ModuleDefID);
            }
            else
            {
                _ModuleControls = new Dictionary<string, ModuleControlInfo>();
            }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            ModuleDefID = Null.SetNullInteger(dr["ModuleDefID"]);
            DesktopModuleID = Null.SetNullInteger(dr["DesktopModuleID"]);
            DefaultCacheTime = Null.SetNullInteger(dr["DefaultCacheTime"]);
            FriendlyName = Null.SetNullString(dr["FriendlyName"]);
        }
        public int KeyID
        {
            get { return ModuleDefID; }
            set { ModuleDefID = value; }
        }
        public XmlSchema GetSchema()
        {
            return null;
        }
        private void ReadModuleControls(XmlReader reader)
        {
            reader.ReadStartElement("moduleControls");
            do
            {
                reader.ReadStartElement("moduleControl");
                ModuleControlInfo moduleControl = new ModuleControlInfo();
                moduleControl.ReadXml(reader);
                ModuleControls.Add(moduleControl.ControlKey, moduleControl);
            } while (reader.ReadToNextSibling("moduleControl"));
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
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "moduleControls")
                {
                    ReadModuleControls(reader);
                }
                else
                {
                    switch (reader.Name)
                    {
                        case "friendlyName":
                            FriendlyName = reader.ReadElementContentAsString();
                            break;
                        case "defaultCacheTime":
                            string elementvalue = reader.ReadElementContentAsString();
                            if (!string.IsNullOrEmpty(elementvalue))
                            {
                                DefaultCacheTime = int.Parse(elementvalue);
                            }
                            break;
                    }
                }
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("moduleDefinition");
            writer.WriteElementString("friendlyName", FriendlyName);
            writer.WriteElementString("defaultCacheTime", DefaultCacheTime.ToString());
            writer.WriteStartElement("moduleControls");
            foreach (ModuleControlInfo control in ModuleControls.Values)
            {
                control.WriteXml(writer);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
 
    }
}
