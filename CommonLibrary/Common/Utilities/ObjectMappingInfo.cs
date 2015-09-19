using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CommonLibrary.Common.Utilities
{
    [Serializable()]
    public class ObjectMappingInfo
    {
        private string _CacheByProperty;
        private int _CacheTimeOutMultiplier;
        private Dictionary<string, string> _ColumnNames;
        private Dictionary<string, PropertyInfo> _Properties;
        private string _ObjectType;
        private string _TableName;
        private string _PrimaryKey;
        private const string RootCacheKey = "ObjectCache_";
        public ObjectMappingInfo()
        {
            _Properties = new Dictionary<string, PropertyInfo>();
            _ColumnNames = new Dictionary<string, string>();
        }
        public string CacheKey
        {
            get
            {
                string _CacheKey = RootCacheKey + TableName + "_";
                if (!string.IsNullOrEmpty(CacheByProperty))
                {
                    _CacheKey += CacheByProperty + "_";
                }
                return _CacheKey;
            }
        }
        public string CacheByProperty
        {
            get { return _CacheByProperty; }
            set { _CacheByProperty = value; }
        }
        public int CacheTimeOutMultiplier
        {
            get { return _CacheTimeOutMultiplier; }
            set { _CacheTimeOutMultiplier = value; }
        }
        public Dictionary<string, string> ColumnNames
        {
            get { return _ColumnNames; }
        }
        public string ObjectType
        {
            get { return _ObjectType; }
            set { _ObjectType = value; }
        }
        public string PrimaryKey
        {
            get { return _PrimaryKey; }
            set { _PrimaryKey = value; }
        }
        public Dictionary<string, PropertyInfo> Properties
        {
            get { return _Properties; }
        }
        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }
    }
}
