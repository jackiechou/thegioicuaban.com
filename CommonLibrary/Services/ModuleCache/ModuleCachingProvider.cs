using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.ComponentModel;

namespace CommonLibrary.Services.ModuleCache
{
    public abstract class ModuleCachingProvider
    {
        public static Dictionary<string, ModuleCachingProvider> GetProviderList()
        {
            return ComponentModel.ComponentFactory.GetComponents<ModuleCachingProvider>();
        }
        public static ModuleCachingProvider Instance(string FriendlyName)
        {
            return ComponentFactory.GetComponent<ModuleCachingProvider>(FriendlyName);
        }
        public static void RemoveItemFromAllProviders(int tabModuleId)
        {
            foreach (KeyValuePair<string, ModuleCachingProvider> kvp in GetProviderList())
            {
                kvp.Value.Remove(tabModuleId);
            }
        }
        protected string ByteArrayToString(byte[] arrInput)
        {
            int i;
            System.Text.StringBuilder sOutput = new System.Text.StringBuilder(arrInput.Length);
            for (i = 0; i <= arrInput.Length - 1; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
        public abstract string GenerateCacheKey(int tabModuleId, SortedDictionary<string, string> varyBy);
        public abstract int GetItemCount(int tabModuleId);
        public abstract byte[] GetModule(int tabModuleId, string cacheKey);
        public abstract void PurgeCache(int portalId);
        public abstract void PurgeExpiredItems(int portalId);
        public abstract void Remove(int tabModuleId);
        public abstract void SetModule(int tabModuleId, string cacheKey, TimeSpan duration, byte[] moduleOutput);
    }
}
