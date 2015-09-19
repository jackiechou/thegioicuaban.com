using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Library.Common;
using System.IO;
using CommonLibrary.Security;
using System.Web.Configuration;
using System.Xml.XPath;
using CommonLibrary.Framework.Providers;


namespace CommonLibrary.Common.Utilities
{
    public class Config
    {
        public static XmlDocument AddAppSetting(XmlDocument xmlDoc, string Key, string Value)
        {
            XmlElement xmlElement;
            XmlNode xmlAppSettings = xmlDoc.SelectSingleNode("//appSettings");
            if (xmlAppSettings != null)
            {
                XmlNode xmlNode = xmlAppSettings.SelectSingleNode(("//add[@key='" + Key + "']"));
                if (xmlNode != null)
                {
                    xmlElement = (XmlElement)xmlNode;
                    xmlElement.SetAttribute("value", Value);
                }
                else
                {
                    xmlElement = xmlDoc.CreateElement("add");
                    xmlElement.SetAttribute("key", Key);
                    xmlElement.SetAttribute("value", Value);
                    xmlAppSettings.AppendChild(xmlElement);
                }
            }
            return xmlDoc;
        }
        public static void AddCodeSubDirectory(string name)
        {
            XmlDocument xmlConfig = Load();
            XmlNode xmlCompilation = xmlConfig.SelectSingleNode("configuration/system.web/compilation");
            if (xmlCompilation == null)
            {
                xmlCompilation = xmlConfig.SelectSingleNode("configuration/location/system.web/compilation");
            }
            XmlNode xmlSubDirectories = xmlCompilation.SelectSingleNode("codeSubDirectories");
            if (xmlSubDirectories == null)
            {
                xmlSubDirectories = xmlConfig.CreateElement("codeSubDirectories");
                xmlCompilation.AppendChild(xmlSubDirectories);
            }
            XmlNode xmlSubDirectory = xmlSubDirectories.SelectSingleNode("add[@directoryName='" + name + "']");
            if (xmlSubDirectory == null)
            {
                xmlSubDirectory = xmlConfig.CreateElement("add");
                XmlUtils.CreateAttribute(xmlConfig, xmlSubDirectory, "directoryName", name);
                xmlSubDirectories.AppendChild(xmlSubDirectory);
            }
            Save(xmlConfig);
        }
        public static void BackupConfig()
        {
            string backupFolder = Globals.glbConfigFolder + "Backup_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "\\";
            try
            {
                if (!Directory.Exists(Globals.ApplicationMapPath + backupFolder))
                {
                    Directory.CreateDirectory(Globals.ApplicationMapPath + backupFolder);
                }
                if (File.Exists(Globals.ApplicationMapPath + "\\web.config"))
                {
                    File.Copy(Globals.ApplicationMapPath + "\\web.config", Globals.ApplicationMapPath + backupFolder + "web_old.config", true);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        public static string GetConnectionString()
        {
            return GetConnectionString(GetDefaultProvider("data").Attributes["connectionStringName"]);
        }
        public static string GetConnectionString(string name)
        {
            string connectionString = "";
            if (!String.IsNullOrEmpty(name))
            {
                connectionString = WebConfigurationManager.ConnectionStrings[name].ConnectionString;                
            }
            if (String.IsNullOrEmpty(connectionString))
            {
                if (!String.IsNullOrEmpty(name))
                {
                    connectionString = Config.GetSetting(name);
                }
            }
            return connectionString;
        }
        public static string GetUpgradeConnectionString()
        {
            string _upgradeConnectionString = GetDefaultProvider("data").Attributes["upgradeConnectionString"];
            return _upgradeConnectionString;
        }
        public static string GetDataBaseOwner()
        {
            string _databaseOwner = GetDefaultProvider("data").Attributes["databaseOwner"];
            if (!String.IsNullOrEmpty(_databaseOwner) && _databaseOwner.EndsWith(".") == false)
            {
                _databaseOwner += ".";
            }
            return _databaseOwner;
        }
        public static Provider GetDefaultProvider(string type)
        {
            ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(type);
            return (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];
        }
        public static string GetObjectQualifer()
        {
            Provider objProvider = GetDefaultProvider("data");
            string _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (!String.IsNullOrEmpty(_objectQualifier) && _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }
            return _objectQualifier;
        }
        public static int GetPersistentCookieTimeout()
        {
            XPathNavigator configNav = Load().CreateNavigator();
            XPathNavigator locationNav = configNav.SelectSingleNode("configuration/location");
            XPathNavigator formsNav;
            if (locationNav == null)
            {
                formsNav = configNav.SelectSingleNode("configuration/system.web/authentication/forms");
            }
            else
            {
                formsNav = configNav.SelectSingleNode("configuration/location/system.web/authentication/forms");
            }
            int PersistentCookieTimeout = 0;
            if (!String.IsNullOrEmpty(Config.GetSetting("PersistentCookieTimeout")))
            {
                PersistentCookieTimeout = int.Parse(Config.GetSetting("PersistentCookieTimeout"));
            }
            if (PersistentCookieTimeout == 0)
            {
                PersistentCookieTimeout = XmlUtils.GetAttributeValueAsInteger(formsNav, "timeout", 30);
            }
            return PersistentCookieTimeout;
        }
        public static Provider GetProvider(string type, string name)
        {
            ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(type);
            return (Provider)_providerConfiguration.Providers[name];
        }
        public static string GetProviderPath(string type)
        {
            Provider objProvider = GetDefaultProvider(type);
            string _providerPath = objProvider.Attributes["providerPath"];
            return _providerPath;
        }
        public static string GetSetting(string setting)
        {
            return WebConfigurationManager.AppSettings[setting];
        }
        public static object GetSection(string section)
        {
            return WebConfigurationManager.GetWebApplicationSection(section);
        }
        public static XmlDocument Load()
        {
            return Load("web.config");
        }
        public static XmlDocument Load(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Globals.ApplicationMapPath + "\\" + filename);
            if (!String.IsNullOrEmpty(xmlDoc.DocumentElement.GetAttribute("xmlns")))
            {
                string strDoc = xmlDoc.InnerXml.Replace("xmlns=\"http://schemas.microsoft.com/.NetConfiguration/v2.0\"", "");
                xmlDoc.LoadXml(strDoc);
            }
            return xmlDoc;
        }
        public static void RemoveCodeSubDirectory(string name)
        {
            XmlDocument xmlConfig = Load();
            XmlNode xmlCompilation = xmlConfig.SelectSingleNode("configuration/system.web/compilation");
            if (xmlCompilation == null)
            {
                xmlCompilation = xmlConfig.SelectSingleNode("configuration/location/system.web/compilation");
            }
            XmlNode xmlSubDirectories = xmlCompilation.SelectSingleNode("codeSubDirectories");
            if (xmlSubDirectories == null)
            {
                return;
            }
            XmlNode xmlSubDirectory = xmlSubDirectories.SelectSingleNode("add[@directoryName='" + name + "']");
            if (xmlSubDirectory != null)
            {
                xmlSubDirectories.RemoveChild(xmlSubDirectory);
            }
            Save(xmlConfig);
        }
        public static string Save(XmlDocument xmlDoc)
        {
            return Save(xmlDoc, "web.config");
        }
        public static string Save(XmlDocument xmlDoc, string filename)
        {
            try
            {
                string strFilePath = Common.Globals.ApplicationMapPath + "\\" + filename;
                FileAttributes objFileAttributes = FileAttributes.Normal;
                if (File.Exists(strFilePath))
                {
                    objFileAttributes = File.GetAttributes(strFilePath);
                    File.SetAttributes(strFilePath, FileAttributes.Normal);
                }
                XmlTextWriter writer = new XmlTextWriter(strFilePath, null);
                writer.Formatting = Formatting.Indented;
                xmlDoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                File.SetAttributes(strFilePath, objFileAttributes);
                return "";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
        public static bool Touch()
        {
            try
            {
                File.SetLastWriteTime(Common.Globals.ApplicationMapPath + "\\web.config", System.DateTime.Now);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }
        public static void UpdateConnectionString(string conn)
        {
            XmlDocument xmlConfig = Load();
            string name = GetDefaultProvider("data").Attributes["connectionStringName"];
            XmlNode xmlConnection = xmlConfig.SelectSingleNode("configuration/connectionStrings/add[@name='" + name + "']");
            XmlUtils.UpdateAttribute(xmlConnection, "connectionString", conn);
            XmlNode xmlAppSetting = xmlConfig.SelectSingleNode("configuration/appSettings/add[@key='" + name + "']");
            XmlUtils.UpdateAttribute(xmlAppSetting, "value", conn);
            Save(xmlConfig);
        }
        public static void UpdateDataProvider(string name, string databaseOwner, string objectQualifier)
        {
            XmlDocument xmlConfig = Load();
            XmlNode xmlProvider = xmlConfig.SelectSingleNode("configuration/dotnetnuke/data/providers/add[@name='" + name + "']");
            XmlUtils.UpdateAttribute(xmlProvider, "databaseOwner", databaseOwner);
            XmlUtils.UpdateAttribute(xmlProvider, "objectQualifier", objectQualifier);
            Save(xmlConfig);
        }
        public static string UpdateMachineKey()
        {
            string backupFolder = Globals.glbConfigFolder + "Backup_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "\\";
            XmlDocument xmlConfig = new XmlDocument();
            string strError = "";
            BackupConfig();
            try
            {
                xmlConfig = Load();
                xmlConfig = UpdateMachineKey(xmlConfig);
            }
            catch (Exception ex)
            {
                strError += ex.Message;
            }
            strError += Save(xmlConfig, backupFolder + "web_.config");
            strError += Save(xmlConfig);
            return strError;
        }
        public static XmlDocument UpdateMachineKey(XmlDocument xmlConfig)
        {
            PortalSecurity objSecurity = new PortalSecurity();
            string validationKey = objSecurity.CreateKey(20);
            string decryptionKey = objSecurity.CreateKey(24);
            XmlNode xmlMachineKey = xmlConfig.SelectSingleNode("configuration/system.web/machineKey");
            XmlUtils.UpdateAttribute(xmlMachineKey, "validationKey", validationKey);
            XmlUtils.UpdateAttribute(xmlMachineKey, "decryptionKey", decryptionKey);
            xmlConfig = AddAppSetting(xmlConfig, "InstallationDate", System.DateTime.Today.ToShortDateString());
            return xmlConfig;
        }
        public static string UpdateValidationKey()
        {
            string backupFolder = Globals.glbConfigFolder + "Backup_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "\\";
            XmlDocument xmlConfig = new XmlDocument();
            string strError = "";
            BackupConfig();
            try
            {
                xmlConfig = Load();
                xmlConfig = UpdateValidationKey(xmlConfig);
            }
            catch (Exception ex)
            {
                strError += ex.Message;
            }
            strError += Save(xmlConfig, backupFolder + "web_.config");
            strError += Save(xmlConfig);
            return strError;
        }
        public static XmlDocument UpdateValidationKey(XmlDocument xmlConfig)
        {
            XmlNode xmlMachineKey;
            string strError = string.Empty;
            xmlMachineKey = xmlConfig.SelectSingleNode("configuration/system.web/machineKey");
            if (xmlMachineKey.Attributes["validationKey"].Value == "F9D1A2D3E1D3E2F7B3D9F90FF3965ABDAC304902")
            {
                PortalSecurity objSecurity = new PortalSecurity();
                string validationKey = objSecurity.CreateKey(20);
                XmlUtils.UpdateAttribute(xmlMachineKey, "validationKey", validationKey);
            }
            return xmlConfig;
        }
    }
}
