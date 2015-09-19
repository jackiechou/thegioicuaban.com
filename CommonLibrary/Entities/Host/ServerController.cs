using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Data;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Common;

namespace CommonLibrary.Entities.Host
{
    public class ServerController
    {
        private const string cacheKey = "WebServers";
        private const int cacheTimeout = 20;
        private const System.Web.Caching.CacheItemPriority cachePriority = System.Web.Caching.CacheItemPriority.High;
        private static DataProvider dataProvider = DataProvider.Instance();
        private static object GetServersCallBack(CacheItemArgs cacheItemArgs)
        {
            return CBO.FillCollection<ServerInfo>(dataProvider.GetServers());
        }
        public static bool UseAppName
        {
            get
            {
                Dictionary<string, string> uniqueServers = new Dictionary<string, string>();
                foreach (ServerInfo server in GetEnabledServers())
                {
                    uniqueServers[server.ServerName] = server.IISAppName;
                }
                return uniqueServers.Count < GetEnabledServers().Count;
            }
        }
        public static void ClearCachedServers()
        {
            DataCache.RemoveCache(cacheKey);
        }
        public static void DeleteServer(int serverID)
        {
            DataProvider.Instance().DeleteServer(serverID);
            ClearCachedServers();
        }
        public static List<ServerInfo> GetEnabledServers()
        {
            List<ServerInfo> servers = new List<ServerInfo>();
            foreach (ServerInfo server in GetServers())
            {
                if (server.Enabled)
                {
                    servers.Add(server);
                }
            }
            return servers;
        }
        public static string GetExecutingServerName()
        {
            string executingServerName = Globals.ServerName;
            if (UseAppName)
            {
                executingServerName += "-" + Globals.IISAppName;
            }
            return executingServerName;
        }
        public static string GetServerName(ServerInfo webServer)
        {
            string serverName = webServer.ServerName;
            if (UseAppName)
            {
                serverName += "-" + webServer.IISAppName;
            }
            return serverName;
        }
        public static List<ServerInfo> GetServers()
        {
            List<ServerInfo> servers = CBO.GetCachedObject<List<ServerInfo>>(new CacheItemArgs(cacheKey, cacheTimeout, cachePriority), GetServersCallBack);
            return servers;
        }
        public static void UpdateServer(ServerInfo server)
        {
            DataProvider.Instance().UpdateServer(server.ServerID, server.Url, server.Enabled);
            ClearCachedServers();
        }
        public static void UpdateServerActivity(ServerInfo server)
        {
            DataProvider.Instance().UpdateServerActivity(server.ServerName, server.IISAppName, server.CreatedDate, server.LastActivityDate);
            ClearCachedServers();
        }
    }
}
