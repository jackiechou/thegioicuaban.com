using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Data;
using System.IO;
using CommonLibrary.Modules;

namespace CommonLibrary.Services.Url.FriendlyUrl
{
    public class RouteController
    {
        public static List<aspnet_Routes> GetListByPortalIdCultureCodeStatus(int PortalId, int ContentItemId, string CultureCode, bool Discontinued)
        {
            using (CommonEntities dbContext = new CommonEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                var query = from x in dbContext.aspnet_Routes
                            where x.PortalId == PortalId
                                && x.CultureCode == CultureCode
                                && x.Discontinued == Discontinued
                            select x;
                if (!string.IsNullOrEmpty(ContentItemId.ToString()) && ContentItemId > 0)
                    query = query.Where(x => x.ContentItemId == ContentItemId);
                return query.ToList();
            }
        }

        public aspnet_Routes GetDetails(int RouteId)
        {
            using (CommonEntities dbContext = new CommonEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                aspnet_Routes entity = dbContext.aspnet_Routes.Where(x => x.RouteId == RouteId).FirstOrDefault();
                return entity;
            }
        }

        ////INSERT- UPDATE - DELETE 
        public int Insert(int PortalId, int ContentItemId, string CultureCode, string RouteName, string RouteUrl, string PhysicalFile,
        bool CheckPhysicalUrlAccess, string RouteValueDictionary, string Description, bool Discontinued)
        {
            int returnValue = 0;
            using (CommonEntities dbContext = new CommonEntities())
            {
                using (System.Transactions.TransactionScope transcope = new System.Transactions.TransactionScope())
                {
                    int ListOrder = (from u in dbContext.aspnet_Routes select u.ListOrder).DefaultIfEmpty(0).Max() + 1;
                    aspnet_Routes entity = new aspnet_Routes();
                    entity.PortalId = PortalId;
                    entity.ContentItemId = ContentItemId;
                    entity.CultureCode = CultureCode;
                    entity.RouteName = RouteName;
                    entity.RouteUrl = RouteUrl;
                    entity.PhysicalFile = PhysicalFile;
                    entity.CheckPhysicalUrlAccess = CheckPhysicalUrlAccess;
                    entity.RouteValueDictionary = RouteValueDictionary;
                    entity.Description = Description;
                    entity.ListOrder = ListOrder;
                    entity.Discontinued = Discontinued;
                    dbContext.AddToaspnet_Routes(entity);
                    returnValue = dbContext.SaveChanges();
                    dbContext.Connection.Close();
                    transcope.Complete();
                }
            }
            return returnValue;
        }

        public int Update(int RouteId, int PortalId, int ContentItemId, string CultureCode, string RouteName, string RouteUrl, string PhysicalFile,
        bool CheckPhysicalUrlAccess, string RouteValueDictionary, string Description, bool Discontinued)
        {
            int i = 0;
            using (CommonEntities dbContext = new CommonEntities())
            {
                using (System.Transactions.TransactionScope transcope = new System.Transactions.TransactionScope())
                {
                    dbContext.Connection.Open();
                    aspnet_Routes entity = dbContext.aspnet_Routes.Single(x => x.RouteId == RouteId);
                    entity.PortalId = PortalId;
                    entity.ContentItemId = ContentItemId;
                    entity.CultureCode = CultureCode;
                    entity.RouteName = RouteName;
                    entity.RouteUrl = RouteUrl;
                    entity.PhysicalFile = PhysicalFile;
                    entity.CheckPhysicalUrlAccess = CheckPhysicalUrlAccess;
                    entity.RouteValueDictionary = RouteValueDictionary;
                    entity.Description = Description;
                    entity.Discontinued = Discontinued;
                    i = dbContext.SaveChanges();
                    dbContext.Connection.Close();
                    transcope.Complete();
                }
            }
            return i;
        }

        public static void Delete(int RouteId)
        {
            using (CommonEntities dbContext = new CommonEntities())
            {
                using (System.Transactions.TransactionScope transcope = new System.Transactions.TransactionScope())
                {
                    dbContext.Connection.Open();
                    aspnet_Routes entity = dbContext.aspnet_Routes.Single(x => x.RouteId == RouteId);
                    dbContext.aspnet_Routes.DeleteObject(entity);
                    dbContext.SaveChanges();
                    dbContext.Connection.Close();
                    transcope.Complete();
                }
            }
        }

        public int UpdateStatus(int RouteId, bool Discontinued)
        {
            using (CommonEntities dbContext = new CommonEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                aspnet_Routes entity = dbContext.aspnet_Routes.Single(x => x.RouteId == RouteId);
                entity.Discontinued = Discontinued;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateSortKey(int RouteId, int ListOrder)
        {
            using (CommonEntities dbContext = new CommonEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                aspnet_Routes album_obj = dbContext.aspnet_Routes.Single(x => x.RouteId == RouteId);
                album_obj.ListOrder = ListOrder;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public static int WriteFileRouter(int PortalId, int ContentItemId, string CultureCode, bool Discontinued, string HomeDirectory, string RouterPath, string RouterFile)
        {
            List<aspnet_Routes> lst = RouteController.GetListByPortalIdCultureCodeStatus(PortalId, ContentItemId, CultureCode, Discontinued);

            if (!Directory.Exists(HomeDirectory))
                Directory.CreateDirectory(HomeDirectory);

            if (!Directory.Exists(RouterPath))
                Directory.CreateDirectory(RouterPath);

            string FileRouterPath = Path.Combine(RouterPath, RouterFile);
            if (!System.IO.File.Exists(FileRouterPath))
                System.IO.File.Create(FileRouterPath);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            FileHandleClass file_obj = new FileHandleClass();
            FileInfo fileInfo = new FileInfo(FileRouterPath);
            string n = file_obj.ReadFile(fileInfo.FullName);
            int start = 0; int end = 0;

            if (n.IndexOf("//start") > -1)
                start = n.IndexOf("//start");

            if (n.IndexOf("//end") > -1)
                end = n.IndexOf("//end");

            string content = n.Replace(n.Substring(start + 7, end - start - 7), "\r\n            ");
            file_obj.UpDateFile(fileInfo.FullName, content);
            n = file_obj.ReadFile(fileInfo.FullName);
            string optionparams = string.Empty;
            for (int i = 0; i < lst.Count; i++)
            {
                string RouteName = lst[i].RouteName;
                string RouteUrl = lst[i].RouteUrl;
                string PhysicalFile = lst[i].PhysicalFile;
                bool CheckPhysicalUrlAccess = lst[i].CheckPhysicalUrlAccess;
                string RouteValueDictionary = StringDirectionProcess(lst[i].RouteValueDictionary);
                if (RouteValueDictionary != "")
                    RouteValueDictionary = ", new System.Web.Routing.RouteValueDictionary { " + RouteValueDictionary + " })";
                else
                    RouteValueDictionary = ")";
                string newrow = "routes.MapPageRoute(\"" + RouteName + "\", \"" + RouteUrl + "\", \"" + PhysicalFile + "\"," + CheckPhysicalUrlAccess.ToString().ToLower() + RouteValueDictionary + ";";
                n = n.Replace("//end", newrow + System.Environment.NewLine + "            //end");
                file_obj.UpDateFile(fileInfo.FullName, n);
            }

            return 1;
        }

        private static string StringDirectionProcess(string RouteValueDictionary)
        {
            string optionparams = string.Empty;

            if (RouteValueDictionary != "")
            {
                string[] items = RouteValueDictionary.Split(',');

                int numitem = 0;
                foreach (var item in items)
                {
                    string value = item.Split('=')[1];
                    string name = item.Split('=')[0];
                    optionparams += "{ \"" + name + "\"," + value + "}";
                    numitem++;
                    if (numitem < items.Length)
                    {
                        optionparams += ",";
                    }
                }
            }

            return optionparams;

        }
    }
}
