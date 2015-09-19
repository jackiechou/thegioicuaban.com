using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Xml;

namespace CommonLibrary.Framework.Providers
{
    public class Provider
    {
        private string _ProviderName;
        private string _ProviderType;
        private NameValueCollection _ProviderAttributes = new NameValueCollection();
        public Provider(XmlAttributeCollection Attributes)
        {
            _ProviderName = Attributes["name"].Value;
            _ProviderType = Attributes["type"].Value;
            foreach (XmlAttribute Attribute in Attributes)
            {
                if (Attribute.Name != "name" && Attribute.Name != "type")
                {
                    _ProviderAttributes.Add(Attribute.Name, Attribute.Value);
                }
            }
        }
        public string Name
        {
            get { return _ProviderName; }
        }
        public string Type
        {
            get { return _ProviderType; }
        }
        public NameValueCollection Attributes
        {
            get { return _ProviderAttributes; }
        }
    }
}
