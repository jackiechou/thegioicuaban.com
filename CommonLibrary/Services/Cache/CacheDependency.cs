using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Cache
{
    public class CacheDependency : IDisposable
    {
        private string[] _fileNames = null;
        private string[] _cacheKeys = null;
        private DateTime _utcStart = DateTime.MaxValue;
        private Cache.CacheDependency _cacheDependency = null;
        private System.Web.Caching.CacheDependency _systemCacheDependency = null;
        public CacheDependency(System.Web.Caching.CacheDependency systemCacheDependency)
        {
            _systemCacheDependency = systemCacheDependency;
        }
        public CacheDependency(string filename)
        {
            _fileNames = new string[] { filename };
        }
        public CacheDependency(string[] filenames)
        {
            _fileNames = filenames;
        }
        public CacheDependency(string[] filenames, DateTime start)
        {
            _utcStart = start.ToUniversalTime();
            _fileNames = filenames;
        }
        public CacheDependency(string[] filenames, string[] cachekeys)
        {
            _fileNames = filenames;
            _cacheKeys = cachekeys;
        }
        public CacheDependency(string filename, DateTime start)
        {
            _utcStart = start.ToUniversalTime();
            if (filename != null)
            {
                _fileNames = new string[] { filename };
            }
        }
        public CacheDependency(string[] filenames, string[] cachekeys, DateTime start)
        {
            _utcStart = start.ToUniversalTime();
            _fileNames = filenames;
            _cacheKeys = cachekeys;
        }
        public CacheDependency(string[] filenames, string[] cachekeys, CacheDependency dependency)
        {
            _fileNames = filenames;
            _cacheKeys = cachekeys;
            _cacheDependency = dependency;
        }
        public CacheDependency(string[] filenames, string[] cachekeys, CacheDependency dependency, DateTime start)
        {
            _utcStart = start.ToUniversalTime();
            _fileNames = filenames;
            _cacheKeys = cachekeys;
            _cacheDependency = dependency;
        }
        public string[] CacheKeys
        {
            get { return _cacheKeys; }
        }
        public string[] FileNames
        {
            get { return _fileNames; }
        }
        public bool HasChanged
        {
            get { return SystemCacheDependency.HasChanged; }
        }
        //public CacheDependency CacheDependency
        //{
        //    get { return _cacheDependency; }
        //}
        public DateTime StartTime
        {
            get { return _utcStart; }
        }
        public System.Web.Caching.CacheDependency SystemCacheDependency
        {
            get
            {
                try
                {
                    if (_systemCacheDependency == null)
                    {
                        if (_cacheDependency == null)
                        {
                            _systemCacheDependency = new System.Web.Caching.CacheDependency(_fileNames, _cacheKeys, _utcStart);
                        }
                        else
                        {
                            _systemCacheDependency = new System.Web.Caching.CacheDependency(_fileNames, _cacheKeys, _cacheDependency.SystemCacheDependency, _utcStart);
                        }
                    }
                    return _systemCacheDependency;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public DateTime UtcLastModified
        {
            get { return SystemCacheDependency.UtcLastModified; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ((disposing))
            {
                if (_cacheDependency != null)
                    _cacheDependency.Dispose(disposing);
                if (_systemCacheDependency != null)
                    _systemCacheDependency.Dispose();
                _fileNames = null;
                _cacheKeys = null;
                _cacheDependency = null;
                _systemCacheDependency = null;
            }
        }
    }
}
