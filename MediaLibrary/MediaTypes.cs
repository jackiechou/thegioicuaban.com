using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CommonLibrary;
using CommonLibrary.Common;
using CommonLibrary.Modules;
using CommonLibrary.Common.Utilities;
using System.Net;
using CommonLibrary.Common.Lists;

namespace MediaLibrary
{
    public class MediaTypes
    {
        string IP = NetworkUtils.GetAddress(AddressType.IPv4);
        public MediaTypes(){}

        public string GetTypePathByTypeId(int idx)
        {            
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                string TypePath = (from x in dbContext.Media_Types where x.TypeId == idx select x.TypePath).SingleOrDefault();
                return TypePath;
            }        
        }

        public List<Media_Types> GetListByStatus(string status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                IQueryable<Media_Types> query = from x in dbContext.Media_Types select x;
                List<Media_Types> lst= new List<Media_Types>();
                if (string.IsNullOrEmpty(status))
                    lst = query.ToList();
                else
                    lst = query.Where(x => x.Status == status).ToList();
                return lst;
            }        
        }

        public List<Media_Types> GetSortedListByStatus(string status, string sortBy)
        {            
            List<Media_Types> objList = GetListByStatus(status);
            if (sortBy != "")
            {
                GenericComparer<Media_Types> cmp = new GenericComparer<Media_Types>(sortBy);
                objList.Sort(cmp);
            }
            return objList;
        }


        public Media_Types GetDetails(int idx)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Types type_obj = dbContext.Media_Types.Where(x => x.TypeId == idx).FirstOrDefault();
                return type_obj;
            }   
        }

        //INSERT- UPDATE - DELETE 
        public int Insert(string UserId, string TypeName, string TypeExt, string TypePath, string Description, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                System.Nullable<Int32> ListOrder = (from u in dbContext.Media_Types select u.TypeId).DefaultIfEmpty(0).Max() + 1;

                Media_Types type_obj = new Media_Types();
                type_obj.TypeName = TypeName;
                type_obj.TypeExt = TypeExt;
                type_obj.TypePath = TypePath;
                type_obj.Description = Description;
                type_obj.CreatedByUserId = new Guid(UserId);
                type_obj.CreatedOnDate = System.DateTime.Now;                
                type_obj.IPLog = IP;
                type_obj.ListOrder = (ListOrder == null ? 1 : ListOrder);
                type_obj.Status = Status;
                dbContext.AddToMedia_Types(type_obj);
                int i  = dbContext.SaveChanges();
                return i;
            }
        }

        public int Update(int TypeId, string UserId, string TypeName, string TypeExt, string TypePath, string Description, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                //Media_Types type_obj = (from x in dbContext.Media_Types where x.TypeId == TypeId select x).First();
                Media_Types type_obj = dbContext.Media_Types.Single(x => x.TypeId == TypeId);
                type_obj.TypeName = TypeName;
                type_obj.TypeExt = TypeExt;
                type_obj.TypePath = TypePath;
                type_obj.Description = Description;
                type_obj.IPModifiedLog = IP;
                type_obj.ModifiedDate = System.DateTime.Now;
                type_obj.LastModifiedByUserId = new Guid(UserId);
                type_obj.Status = Status;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateStatus(string UserId, int TypeId, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Types type_obj = dbContext.Media_Types.Single(x => x.TypeId == TypeId);
                type_obj.TypeId = TypeId;
                type_obj.Status = Status;
                type_obj.LastModifiedByUserId = new Guid(UserId);
                type_obj.ModifiedDate = System.DateTime.Now;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateSortKey(string UserId, int TypeId, int ListOrder)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;

                Media_Types type_obj = dbContext.Media_Types.Single(x => x.TypeId == TypeId);
                type_obj.TypeId = TypeId;
                type_obj.ListOrder = ListOrder;
                type_obj.LastModifiedByUserId = new Guid(UserId);
                type_obj.ModifiedDate = System.DateTime.Now;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int Delte(int TypeId)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                //Media_Types type_obj = (from x in dbContext.Media_Types where x.TypeId == TypeId select x).First();
                Media_Types type_obj = dbContext.Media_Types.Single(x => x.TypeId == TypeId);
                dbContext.DeleteObject(type_obj);
                int i = dbContext.SaveChanges();
                return i;
            }
        }
    }
}
