using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Collections;

namespace CommonLibrary.Common.Utilities
{
    public class CacheItemArgs
    {
        private CacheItemRemovedCallback _CacheCallback;
        private CacheDependency _CacheDependency;
        private string _CacheKey;
        private CacheItemPriority _CachePriority;
        private int _CacheTimeOut;
        private ArrayList _ParamList;
        private string _ProcedureName;
        public CacheItemArgs(string key)
        {
            _CacheKey = key;
            _CacheTimeOut = 20;
            _CachePriority = CacheItemPriority.Default;
            //_ParamList = new ArrayList();
        }
        public CacheItemArgs(string key, int timeout)
            : this(key)
        {
            _CacheTimeOut = timeout;
            _CachePriority = CacheItemPriority.Default;
            //_ParamList = new ArrayList();
        }
        public CacheItemArgs(string key, CacheItemPriority priority)
            : this(key)
        {
            _CachePriority = priority;
            //_ParamList = new ArrayList();
        }
        public CacheItemArgs(string key, int timeout, CacheItemPriority priority)
            : this(key)
        {
            _CacheTimeOut = timeout;
            _CachePriority = priority;
        }
        public CacheItemArgs(string key, int timeout, CacheItemPriority priority, params object[] parameters)
            : this(key)
        {
            _CacheTimeOut = timeout;
            _CachePriority = priority;
            _ParamList = new ArrayList();
            foreach (object obj in parameters)
            {
                _ParamList.Add(obj);
            }
        }
        public CacheItemRemovedCallback CacheCallback
        {
            get { return _CacheCallback; }
            set { _CacheCallback = value; }
        }
        public CacheDependency CacheDependency
        {
            get { return _CacheDependency; }
            set { _CacheDependency = value; }
        }
        public string CacheKey
        {
            get { return _CacheKey; }
        }
        public CacheItemPriority CachePriority
        {
            get { return _CachePriority; }
            set { _CachePriority = value; }
        }
        public int CacheTimeOut
        {
            get { return _CacheTimeOut; }
            set { _CacheTimeOut = value; }
        }
        public ArrayList ParamList
        {
            get
            {
                if (_ParamList == null)
                {
                    _ParamList = new ArrayList();
                }
                return _ParamList;
            }
        }
        public object[] Params
        {
            get { return ParamList.ToArray(); }
        }
        public string ProcedureName
        {
            get { return _ProcedureName; }
            set { _ProcedureName = value; }
        }
    }
}
