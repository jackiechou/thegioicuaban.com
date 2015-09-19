using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonLibrary.Common.Utilities;
using System.Web.Caching;
using CommonLibrary.Services.Cache;
using CommonLibrary.Services.Exceptions;

namespace CommonLibrary.Providers.CachingProviders.FileBasedCachingProvider
{
    public class FBCachingProvider : CachingProvider
    {
        static internal string CachingDirectory = "Cache\\";
        internal const string CacheFileExtension = ".resources";
        public override void Insert(string Key, object Value, CommonLibrary.Services.Cache.CacheDependency Dependency, System.DateTime AbsoluteExpiration, System.TimeSpan SlidingExpiration, CacheItemPriority Priority, CacheItemRemovedCallback OnRemoveCallback)
        {
            CommonLibrary.Services.Cache.CacheDependency d = Dependency;
            if (IsWebFarm())
            {
                string[] f = new string[1];
                f[0] = GetFileName(Key);
                CreateCacheFile(f[0], Key);
                d = new CommonLibrary.Services.Cache.CacheDependency(f, null, Dependency);
            }
            base.Insert(Key, Value, d, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback);
        }
        public override bool IsWebFarm()
        {
            bool _IsWebFarm = Null.NullBoolean;
            if (!string.IsNullOrEmpty(Config.GetSetting("IsWebFarm")))
            {
                _IsWebFarm = bool.Parse(Config.GetSetting("IsWebFarm"));
            }
            return _IsWebFarm;
        }
        public override string PurgeCache()
        {
            return PurgeCacheFiles(Common.Globals.HostMapPath + CachingDirectory);
        }
        public override void Remove(string Key)
        {
            base.Remove(Key);
            if (IsWebFarm())
            {
                string f = GetFileName(Key);
                DeleteCacheFile(f);
            }
        }
        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i <= arrInput.Length - 1; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
        private static void CreateCacheFile(string FileName, string CacheKey)
        {
            StreamWriter s = null;
            try
            {
                if (!File.Exists(FileName))
                {
                    s = File.CreateText(FileName);
                    s.Write(CacheKey);
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }
        }
        private static void DeleteCacheFile(string FileName)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
        }
        private static string GetFileName(string CacheKey)
        {
            byte[] FileNameBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(CacheKey);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            FileNameBytes = md5.ComputeHash(FileNameBytes);
            string FinalFileName = ByteArrayToString(FileNameBytes);
            return Path.GetFullPath(Common.Globals.HostMapPath + CachingDirectory + FinalFileName + CacheFileExtension);
        }
        private string PurgeCacheFiles(string Folder)
        {
            int PurgedFiles = 0;
            int PurgeErrors = 0;
            int i;
            string[] f;
            f = Directory.GetFiles(Folder);
            for (i = 0; i <= f.Length - 1; i++)
            {
                System.DateTime dtLastWrite;
                dtLastWrite = File.GetLastWriteTime(f[i]);
                if (dtLastWrite < DateTime.Now.Subtract(new TimeSpan(2, 0, 0)))
                {
                    string strCacheKey = Path.GetFileNameWithoutExtension(f[i]);
                    if (DataCache.GetCache(strCacheKey) == null)
                    {
                        try
                        {
                            File.Delete(f[i]);
                            PurgedFiles += 1;
                        }
                        catch (Exception ex)
                        {
                            PurgeErrors += 1;
                            ex.ToString();
                        }
                    }
                }
            }
            return string.Format("Cache Synchronization Files Processed: " + f.Length.ToString() + ", Purged: " + PurgedFiles.ToString() + ", Errors: " + PurgeErrors.ToString());
        }
    }
}
