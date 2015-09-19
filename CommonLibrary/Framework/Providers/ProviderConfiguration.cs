using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Framework.Providers
{
    public class ProviderConfiguration
    {
        private Hashtable _Providers = new Hashtable();
        private string _DefaultProvider;
        public static ProviderConfiguration GetProviderConfiguration(string strProvider)
        {
            return (ProviderConfiguration)Config.GetSection("dotnetnuke/" + strProvider);
        }
        internal void LoadValuesFromConfigurationXml(XmlNode node)
        {
            XmlAttributeCollection attributeCollection = node.Attributes;
            _DefaultProvider = attributeCollection["defaultProvider"].Value;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "providers")
                {
                    GetProviders(child);
                }
            }
        }
        internal void GetProviders(XmlNode node)
        {
            foreach (XmlNode Provider in node.ChildNodes)
            {
                switch (Provider.Name)
                {
                    case "add":
                        Providers.Add(Provider.Attributes["name"].Value, new Provider(Provider.Attributes));
                        break;
                    case "remove":
                        Providers.Remove(Provider.Attributes["name"].Value);
                        break;
                    case "clear":
                        Providers.Clear();
                        break;
                }
            }
        }
        public string DefaultProvider
        {
            get { return _DefaultProvider; }
        }
        public Hashtable Providers
        {
            get { return _Providers; }
        }
    }
}
