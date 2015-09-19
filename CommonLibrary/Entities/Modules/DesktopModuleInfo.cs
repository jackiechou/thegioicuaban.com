using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommonLibrary.Entities.Modules.Definitions;
using System.Xml.Schema;
using CommonLibrary.Common.Utilities;
using System.Xml.Serialization;

namespace CommonLibrary.Entities.Modules
{
    [Serializable()]
    public class DesktopModuleInfo : BaseEntityInfo, IXmlSerializable, IHydratable
    {
        private int _DesktopModuleID = Null.NullInteger;
        private int _PackageID = Null.NullInteger;
        private string _CodeSubDirectory = Null.NullString;
        private string _ModuleName;
        private string _FolderName;
        private string _FriendlyName;
        private string _Description;
        private string _Version;
        private bool _IsPremium = Null.NullBoolean;
        private bool _IsAdmin = Null.NullBoolean;
        private int _SupportedFeatures = Null.NullInteger;
        private string _BusinessControllerClass;
        private string _CompatibleVersions;
        private string _Dependencies;
        private string _Permissions;
        private Dictionary<string, ModuleDefinitionInfo> _ModuleDefinitions;
        public int DesktopModuleID
        {
            get { return _DesktopModuleID; }
            set { _DesktopModuleID = value; }
        }
        public int PackageID
        {
            get { return _PackageID; }
            set { _PackageID = value; }
        }
        public string CodeSubDirectory
        {
            get { return _CodeSubDirectory; }
            set { _CodeSubDirectory = value; }
        }
        public string BusinessControllerClass
        {
            get { return _BusinessControllerClass; }
            set { _BusinessControllerClass = value; }
        }
        public string CompatibleVersions
        {
            get { return _CompatibleVersions; }
            set { _CompatibleVersions = value; }
        }
        public string Dependencies
        {
            get { return _Dependencies; }
            set { _Dependencies = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public string FolderName
        {
            get { return _FolderName; }
            set { _FolderName = value; }
        }
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set { _FriendlyName = value; }
        }
        public bool IsAdmin
        {
            get { return _IsAdmin; }
            set { _IsAdmin = value; }
        }
        public bool IsPortable
        {
            get { return GetFeature(DesktopModuleSupportedFeature.IsPortable); }
            set { UpdateFeature(DesktopModuleSupportedFeature.IsPortable, value); }
        }
        public bool IsPremium
        {
            get { return _IsPremium; }
            set { _IsPremium = value; }
        }
        public bool IsSearchable
        {
            get { return GetFeature(DesktopModuleSupportedFeature.IsSearchable); }
            set { UpdateFeature(DesktopModuleSupportedFeature.IsSearchable, value); }
        }
        public bool IsUpgradeable
        {
            get { return GetFeature(DesktopModuleSupportedFeature.IsUpgradeable); }
            set { UpdateFeature(DesktopModuleSupportedFeature.IsUpgradeable, value); }
        }
        public Dictionary<string, ModuleDefinitionInfo> ModuleDefinitions
        {
            get
            {
                if (_ModuleDefinitions == null)
                {
                    if (DesktopModuleID > Null.NullInteger)
                    {
                        _ModuleDefinitions = ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(DesktopModuleID);
                    }
                    else
                    {
                        _ModuleDefinitions = new Dictionary<string, ModuleDefinitionInfo>();
                    }
                }
                return _ModuleDefinitions;
            }
        }
        public string ModuleName
        {
            get { return _ModuleName; }
            set { _ModuleName = value; }
        }
        public string Permissions
        {
            get { return _Permissions; }
            set { _Permissions = value; }
        }
        public int SupportedFeatures
        {
            get { return (_SupportedFeatures); }
            set { _SupportedFeatures = value; }
        }
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        private void ClearFeature(DesktopModuleSupportedFeature Feature)
        {
            SupportedFeatures = SupportedFeatures & (int)Feature;
        }
        private bool GetFeature(DesktopModuleSupportedFeature Feature)
        {
            bool isSet = false;
            if (SupportedFeatures > Null.NullInteger && (SupportedFeatures & (int)Feature) == (int)Feature)
            {
                isSet = true;
            }
            return isSet;
        }
        private void SetFeature(DesktopModuleSupportedFeature Feature)
        {
            SupportedFeatures = SupportedFeatures ^ (int)Feature;
        }
        private void UpdateFeature(DesktopModuleSupportedFeature Feature, bool IsSet)
        {
            if (IsSet)
            {
                SetFeature(Feature);
            }
            else
            {
                ClearFeature(Feature);
            }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            DesktopModuleID = Null.SetNullInteger(dr["DesktopModuleID"]);
            PackageID = Null.SetNullInteger(dr["PackageID"]);
            ModuleName = Null.SetNullString(dr["ModuleName"]);
            FriendlyName = Null.SetNullString(dr["FriendlyName"]);
            Description = Null.SetNullString(dr["Description"]);
            FolderName = Null.SetNullString(dr["FolderName"]);
            Version = Null.SetNullString(dr["Version"]);
            Description = Null.SetNullString(dr["Description"]);
            IsPremium = Null.SetNullBoolean(dr["IsPremium"]);
            IsAdmin = Null.SetNullBoolean(dr["IsAdmin"]);
            BusinessControllerClass = Null.SetNullString(dr["BusinessControllerClass"]);
            SupportedFeatures = Null.SetNullInteger(dr["SupportedFeatures"]);
            CompatibleVersions = Null.SetNullString(dr["CompatibleVersions"]);
            Dependencies = Null.SetNullString(dr["Dependencies"]);
            Permissions = Null.SetNullString(dr["Permissions"]);
            base.FillInternal(dr);
        }
        public int KeyID
        {
            get { return DesktopModuleID; }
            set { DesktopModuleID = value; }
        }
        public XmlSchema GetSchema()
        {
            return null;
        }
        private void ReadSupportedFeatures(XmlReader reader)
        {
            SupportedFeatures = 0;
            reader.ReadStartElement("supportedFeatures");
            do
            {
                if (reader.HasAttributes)
                {
                    reader.MoveToFirstAttribute();
                    switch (reader.ReadContentAsString())
                    {
                        case "Portable":
                            IsPortable = true;
                            break;
                        case "Searchable":
                            IsSearchable = true;
                            break;
                        case "Upgradeable":
                            IsUpgradeable = true;
                            break;
                    }
                }
            } while (reader.ReadToNextSibling("supportedFeature"));
        }
        private void ReadModuleDefinitions(XmlReader reader)
        {
            reader.ReadStartElement("moduleDefinitions");
            do
            {
                reader.ReadStartElement("moduleDefinition");
                ModuleDefinitionInfo moduleDefinition = new ModuleDefinitionInfo();
                moduleDefinition.ReadXml(reader);
                ModuleDefinitions.Add(moduleDefinition.FriendlyName, moduleDefinition);
            } while (reader.ReadToNextSibling("moduleDefinition"));
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
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "moduleDefinitions" && !reader.IsEmptyElement)
                {
                    ReadModuleDefinitions(reader);
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "supportedFeatures" && !reader.IsEmptyElement)
                {
                    ReadSupportedFeatures(reader);
                }
                else
                {
                    switch (reader.Name)
                    {
                        case "moduleName":
                            ModuleName = reader.ReadElementContentAsString();
                            break;
                        case "foldername":
                            FolderName = reader.ReadElementContentAsString();
                            break;
                        case "businessControllerClass":
                            BusinessControllerClass = reader.ReadElementContentAsString();
                            break;
                        case "codeSubDirectory":
                            CodeSubDirectory = reader.ReadElementContentAsString();
                            break;
                        case "isAdmin":
                            bool isAdmin;
                            Boolean.TryParse(reader.ReadElementContentAsString(), out isAdmin);
                            IsAdmin = isAdmin;
                            break;
                        case "isPremium":
                            bool isPremium;
                            Boolean.TryParse(reader.ReadElementContentAsString(), out isPremium);
                            IsPremium = isPremium;
                            break;

                    }
                }
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("desktopModule");
            writer.WriteElementString("moduleName", ModuleName);
            writer.WriteElementString("foldername", FolderName);
            writer.WriteElementString("businessControllerClass", BusinessControllerClass);
            if (!string.IsNullOrEmpty(CodeSubDirectory))
            {
                writer.WriteElementString("codeSubDirectory", CodeSubDirectory);
            }
            writer.WriteStartElement("supportedFeatures");
            if (IsPortable)
            {
                writer.WriteStartElement("supportedFeature");
                writer.WriteAttributeString("type", "Portable");
                writer.WriteEndElement();
            }
            if (IsSearchable)
            {
                writer.WriteStartElement("supportedFeature");
                writer.WriteAttributeString("type", "Searchable");
                writer.WriteEndElement();
            }
            if (IsUpgradeable)
            {
                writer.WriteStartElement("supportedFeature");
                writer.WriteAttributeString("type", "Upgradeable");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("moduleDefinitions");
            foreach (ModuleDefinitionInfo definition in ModuleDefinitions.Values)
            {
                definition.WriteXml(writer);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
