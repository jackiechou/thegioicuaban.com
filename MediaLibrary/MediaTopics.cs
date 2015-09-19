using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Library.Common;
using System.Data;
using CommonLibrary;
using System.Data.SqlClient;
using CommonLibrary.Common;
using CommonLibrary.Modules;
using CommonLibrary.Common.Utilities;

namespace MediaLibrary
{
    public class MediaTopics
    {
        
        ModuleClass module_obj = new ModuleClass();
        string IP = NetworkUtils.GetAddress(AddressType.IPv4);
       
        public MediaTopics(){}

        public List<Media_Topics> GetListByStatus(string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                IQueryable<Media_Topics> query = from x in dbContext.Media_Topics select x;
                if (!string.IsNullOrEmpty(Status))
                    query = query.Where(x => x.Status == Status);
                return query.ToList();
            }  
        }

        public List<Media_Topics> GetListByTypeStatus(int TypeId, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                IQueryable<Media_Topics> query = from x in dbContext.Media_Topics select x;
                if (TypeId > 0)
                    query = query.Where(x => x.TypeId == TypeId);
                if (!string.IsNullOrEmpty(Status))
                    query = query.Where(x => x.Status == Status);
                return query.ToList();
            }  
        }

        public Media_Topics GetDetailById(int idx)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Topics topic_obj = dbContext.Media_Topics.Where(x => x.TopicId == idx).FirstOrDefault();
                return topic_obj;
            }   
        }        
       
        //INSERT- UPDATE - DELETE 
        public int Insert(string UserId, int TypeId, int ParentId, string Name, string Photo, string Description, string Status)
        {
            string Alias = module_obj.createTags(Name);
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                System.Nullable<Int32> ListOrder = (from u in dbContext.Media_Types select u.TypeId).DefaultIfEmpty(0).Max() + 1;

                Media_Topics topic_obj = new Media_Topics();
                topic_obj.TypeId = TypeId;
                topic_obj.Name = Name;
                topic_obj.Alias = Alias;
                topic_obj.ParentId = ParentId;
                topic_obj.ListOrder = (ListOrder == null ? 1 : ListOrder);
                topic_obj.Photo = Photo;
                topic_obj.Description = Description;
                topic_obj.IPLog = IP;
                topic_obj.CreatedByUserId = new Guid(UserId);
                topic_obj.CreatedOnDate = System.DateTime.Now;
                topic_obj.Status = Status;
                dbContext.AddToMedia_Topics(topic_obj);
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int Update(string UserId, int TypeId, int TopicId, int ParentId, string Name, string Photo, string Description, string Status)
        {
            string Alias = module_obj.createTags(Name);
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Topics topic_obj = dbContext.Media_Topics.Single(x => x.TopicId == TopicId);
                topic_obj.TypeId = TypeId;
                topic_obj.TopicId = TopicId;
                topic_obj.Name = Name;
                topic_obj.Alias = Alias;
                topic_obj.ParentId = ParentId;
                topic_obj.Photo = Photo;
                topic_obj.Description = Description;
                topic_obj.IPLog = IP;
                topic_obj.CreatedByUserId = new Guid(UserId);
                topic_obj.CreatedOnDate = System.DateTime.Now;
                topic_obj.Status = Status;              
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateStatus(string UserId, int TopicId, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Topics topic_obj = dbContext.Media_Topics.Single(x => x.TopicId == TopicId);
                topic_obj.TopicId = TopicId;
                topic_obj.Status = Status;
                topic_obj.LastModifiedByUserId = new Guid(UserId);
                topic_obj.LastModifiedDate = System.DateTime.Now;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateSortKey(string UserId, int TopicId, int ListOrder)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;

                Media_Topics topic_obj = dbContext.Media_Topics.Single(x => x.TopicId == TopicId);
                topic_obj.TopicId = TopicId;
                topic_obj.ListOrder = ListOrder;
                topic_obj.LastModifiedByUserId = new Guid(UserId);
                topic_obj.LastModifiedDate = System.DateTime.Now;
                int i = dbContext.SaveChanges();
                return i;
             }
        }

        public int Delete(string UserId, int Idx, string Dir_Path)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                string filename = (from x in dbContext.Media_Topics where x.TopicId == Idx select x.Photo).SingleOrDefault();
                if (filename != null && filename != string.Empty)
                    module_obj.deleteFile(filename, Dir_Path);

                //Media_Topics topic_obj = (from x in dbContext.Media_Topics where x.TopicId == Idx select x).First();
                Media_Topics topic_obj = dbContext.Media_Topics.Single(x => x.TopicId == Idx);
                dbContext.DeleteObject(topic_obj);
                int i = dbContext.SaveChanges();
                return i;
            }
        } 
    }
}
