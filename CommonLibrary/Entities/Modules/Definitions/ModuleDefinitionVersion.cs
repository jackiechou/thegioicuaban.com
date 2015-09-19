using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using CommonLibrary.Entities.Portal;
using System.Web;
using CommonLibrary.Services.Localization;
using CommonLibrary.Common;

namespace CommonLibrary.Entities.Modules.Definitions
{
    public enum ModuleDefinitionVersion
    {
        VUnknown = 0,
        V1 = 1,
        V2 = 2,
        V2_Skin = 3,
        V2_Provider = 4,
        V3 = 5
    }
    public class ModuleDefinitionValidator : XmlValidatorBase
    {
        private string GetDnnSchemaPath(Stream xmlStream)
        {
            ModuleDefinitionVersion Version = GetModuleDefinitionVersion(xmlStream);
            string schemaPath = "";
            switch (Version)
            {
                case ModuleDefinitionVersion.V2:
                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2.xsd";
                    break;
                case ModuleDefinitionVersion.V3:
                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V3.xsd";
                    break;
                case ModuleDefinitionVersion.V2_Skin:
                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2Skin.xsd";
                    break;
                case ModuleDefinitionVersion.V2_Provider:
                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2Provider.xsd";
                    break;
                case ModuleDefinitionVersion.VUnknown:
                    throw new Exception(GetLocalizedString("EXCEPTION_LoadFailed"));
            }
            return Path.Combine(Common.Globals.ApplicationMapPath, schemaPath);
        }
        private string GetLocalizedString(string key)
        {
            PortalSettings objPortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            if (objPortalSettings == null)
            {
                return key;
            }
            else
            {
                return Localization.GetString(key, objPortalSettings);
            }
        }
        public ModuleDefinitionVersion GetModuleDefinitionVersion(Stream xmlStream)
        {
            ModuleDefinitionVersion retValue = ModuleDefinitionVersion.VUnknown;
            xmlStream.Seek(0, SeekOrigin.Begin);
            XmlTextReader xmlReader = new XmlTextReader(xmlStream);
            xmlReader.MoveToContent();
            switch (xmlReader.LocalName.ToLower())
            {
                case "module":
                    retValue = ModuleDefinitionVersion.V1;
                    break;
                case "dotnetnuke":
                    switch (xmlReader.GetAttribute("type"))
                    {
                        case "Module":
                            switch (xmlReader.GetAttribute("version"))
                            {
                                case "2.0":
                                    retValue = ModuleDefinitionVersion.V2;
                                    break;
                                case "3.0":
                                    retValue = ModuleDefinitionVersion.V3;
                                    break;
                            }
                            break;
                        case "SkinObject":
                            retValue = ModuleDefinitionVersion.V2_Skin;
                            break;
                        case "Provider":
                            retValue = ModuleDefinitionVersion.V2_Provider;
                            break;
                        default:
                            retValue = ModuleDefinitionVersion.VUnknown;
                            break;
                    }
                    break;
                default:
                    retValue = ModuleDefinitionVersion.VUnknown;
                    break;
            }
            return retValue;
        }
        public override bool Validate(Stream XmlStream)
        {
            SchemaSet.Add("", GetDnnSchemaPath(XmlStream));
            return base.Validate(XmlStream);
        }
    }
}
