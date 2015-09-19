using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using CommonLibrary.ComponentModel;

namespace CommonLibrary.Services.OutputCache
{
    public abstract class OutputCachingProvider
    {
        public static Dictionary<string, OutputCachingProvider> GetProviderList()
        {
            return ComponentModel.ComponentFactory.GetComponents<OutputCachingProvider>();
        }
        public static OutputCachingProvider Instance(string FriendlyName)
        {
            return ComponentFactory.GetComponent<OutputCachingProvider>(FriendlyName);
        }
        public static void RemoveItemFromAllProviders(int tabId)
        {
            foreach (KeyValuePair<string, OutputCachingProvider> kvp in GetProviderList())
            {
                kvp.Value.Remove(tabId);
            }
        }
        public abstract string GenerateCacheKey(int tabId, System.Collections.Specialized.StringCollection includeVaryByKeys, System.Collections.Specialized.StringCollection excludeVaryByKeys, SortedDictionary<string, string> varyBy);
        public abstract int GetItemCount(int tabId);
        public abstract byte[] GetOutput(int tabId, string cacheKey);
        public abstract OutputCacheResponseFilter GetResponseFilter(int tabId, int maxVaryByCount, Stream responseFilter, string cacheKey, TimeSpan cacheDuration);
        public abstract void PurgeCache(int portalId);
        public abstract void PurgeExpiredItems(int portalId);
        public abstract void Remove(int tabId);
        public abstract void SetOutput(int tabId, string cacheKey, TimeSpan duration, byte[] output);
        public abstract bool StreamOutput(int tabId, string cacheKey, HttpContext context);
    }
}
