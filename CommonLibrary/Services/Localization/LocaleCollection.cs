using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace CommonLibrary.Services.Localization
{
    public class LocaleCollection : NameObjectCollectionBase
    {
        private DictionaryEntry _de = new DictionaryEntry();
        public LocaleCollection()
        {
        }
        public DictionaryEntry this[int index]
        {
            get
            {
                _de.Key = this.BaseGetKey(index);
                _de.Value = this.BaseGet(index);
                return _de;
            }
        }
        public Locale this[string key]
        {
            get { return (Locale)this.BaseGet(key); }
            set { this.BaseSet(key, value); }
        }
        public string[] AllKeys
        {
            get { return this.BaseGetAllKeys(); }
        }
        public Array AllValues
        {
            get { return this.BaseGetAllValues(); }
        }
        public Boolean HasKeys
        {
            get { return this.BaseHasKeys(); }
        }
        public void Add(String key, Object value)
        {
            this.BaseAdd(key, value);
        }
        public void Remove(String key)
        {
            this.BaseRemove(key);
        }
    }
}
