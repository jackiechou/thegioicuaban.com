using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Entities.Portal
{
    [Serializable()]
    public class PortalAliasCollection : DictionaryBase
    {
        public PortalAliasCollection()
            : base()
        {
        }
        public PortalAliasInfo this[string key]
        {
            get { return (PortalAliasInfo)this.Dictionary[key]; }
            set { this.Dictionary[key] = value; }
        }
        public bool Contains(String key)
        {
            return Dictionary.Contains(key);
        }
        public Boolean HasKeys
        {
            get { return this.Dictionary.Keys.Count > 0; }
        }
        public ICollection Keys
        {
            get { return this.Dictionary.Keys; }
        }
        public void Add(String key, PortalAliasInfo value)
        {
            this.Dictionary.Add(key, value);
        }
    }
}
