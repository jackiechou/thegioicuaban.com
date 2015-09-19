using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using System.Data.Linq;
using CommonLibrary.Common.Utilities;
using System.Data.Common;
using System.ComponentModel;

namespace GalleryLibrary
{
    public class CustomCollectionFiles
    {
        [DataObjectField(true)]
        public int TopicId { get; set; }
        public string Title { get; set; }
        public string IconFile { get; set; }
        public string Description { get; set; }
        public int? ListOrder { get; set; }
        public string Tags { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public DateTime? LastModifieddDate { get; set; }
        public string UserLog { get; set; }
        public string UserLastUpdate { get; set; }
        public string IPLog { get; set; }
        public string IPLastUpdate { get; set; }
        public char Status { get; set; }
        public int CollectionId { get; set; }
        public int CollectionFileId { get; set; }
    }

    public class GalleryCollection
    {   
        private static string IP = NetworkUtils.GetAddress(AddressType.IPv4);

        //================================================================================
        public static List<Gallery_Collection> GetActiveListByFixedNum(int iTotalItemCount, int topicId)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                var query = from x in context.Gallery_Collections where x.TopicId == topicId && x.Status.Equals("1") orderby x.CollectionId descending select x;
                int TotalItemCount = 0;
                if (query.Count() > 0)
                {
                    if (iTotalItemCount <= query.Count())
                        TotalItemCount = iTotalItemCount;
                    else
                        TotalItemCount = query.Count();
                }
                return query.Take(TotalItemCount).ToList();
            }
        }
        //================================================================================

        public List<Gallery_Collection> GetList(int topicid, char status)
        {
            using(GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString)){
                context.CommandTimeout = Settings.CommandTimeout;
                var query = from g in context.Gallery_Collections
                            where g.Status == status && g.TopicId == topicid
                            select g;
                List<Gallery_Collection> lst = query.ToList();
                return lst;
            }
        }

        public List<CustomCollectionFiles> GetCollectionListForMaster(int TopicId, char status)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                var query = from gtf in context.Gallery_Topics
                            join cl in context.Gallery_Collections
                            on gtf.Gallery_TopicId equals cl.TopicId
                            where gtf.Status == status && gtf.Gallery_TopicId == TopicId
                            select new { gtf, cl };
                List<CustomCollectionFiles> list = new List<CustomCollectionFiles>();
                foreach (var c in query)
                {
                    CustomCollectionFiles file_obj = new CustomCollectionFiles();
                    file_obj.TopicId = c.gtf.Gallery_TopicId;
                    file_obj.CollectionId = c.cl.CollectionId;
                    file_obj.Title = c.cl.Title;
                    file_obj.IconFile = c.cl.IconFile;
                    file_obj.Description = c.cl.Description;
                    file_obj.ListOrder = Convert.ToInt32(c.cl.ListOrder);
                    file_obj.Tags = c.cl.Tags;
                    file_obj.Url = c.cl.Url;
                    file_obj.CreatedOnDate = Convert.ToDateTime(c.cl.CreatedOnDate);
                    file_obj.LastModifieddDate = Convert.ToDateTime(c.cl.LastModifieddDate);
                    file_obj.UserLog = c.cl.UserLog.ToString();
                    file_obj.UserLastUpdate = c.cl.UserLastUpdate.ToString();
                    file_obj.IPLog = c.cl.IPLog;
                    file_obj.IPLastUpdate = c.cl.IPLastUpdate;
                    file_obj.Status = c.cl.Status;
                    list.Add(file_obj);
                }
                return list;
            }
        }

        public Gallery_Collection GetDetails(int Idx)
        {
            using(GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString)){
                context.CommandTimeout = Settings.CommandTimeout;
                Gallery_Collection collection = context.Gallery_Collections.Where(p => p.CollectionId == Idx).SingleOrDefault();
                return collection;
            }
        }

        public bool InsertData(int TopicId, string Title, string IconFile, string Description, string Tags, string Url,
	                           string UserLog, string Status)
        {
            using(GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                System.Nullable<Int32> ListOrder = (int?)context.Gallery_Collections.Max(x => (int?)x.CollectionId) ?? 0;
                Gallery_Collection collection = new Gallery_Collection();
                collection.TopicId = TopicId;
                collection.Title = Title;
                collection.IconFile = IconFile;
                collection.Description = Description;
                collection.ListOrder = ListOrder + 1;
                collection.UserLog = Guid.Parse(UserLog);
                collection.UserLastUpdate = Guid.Parse(UserLog);
                collection.CreatedOnDate = System.DateTime.Now;
                collection.LastModifieddDate = System.DateTime.Now;
                collection.Status = Convert.ToChar(Status);
                collection.Tags = Tags;
                collection.Url = Url;
                collection.IPLog = IP;
                collection.IPLastUpdate = IP;

            
                context.Gallery_Collections.InsertOnSubmit(collection);
                context.SubmitChanges();
                int newCollectionId = collection.CollectionId;
                if (newCollectionId > 0) { return true; }
                else { return false; }
            }
        }

        public bool UpdateData(int Idx, int TopicId, string Title, string IconFile, string Description, int ListOrder, string Tags, string Url,
                               string UserLog, string UserLastUpdate, DateTime CreatedOnDate, string IPLog, string Status)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.Connection.Open();
                Gallery_Collection collection_obj = new Gallery_Collection();
                collection_obj.CollectionId = Idx;
                collection_obj.TopicId = TopicId;
                collection_obj.Title = Title;
                collection_obj.IconFile = IconFile;
                collection_obj.Description = Description;
                collection_obj.ListOrder = ListOrder;
                collection_obj.UserLog = Guid.Parse(UserLog);
                collection_obj.UserLastUpdate = Guid.Parse(UserLastUpdate);
                collection_obj.CreatedOnDate = CreatedOnDate;
                collection_obj.LastModifieddDate = DateTime.Now;
                collection_obj.Status = Convert.ToChar(Status);
                collection_obj.Tags = Tags;
                collection_obj.Url = Url;
                collection_obj.IPLog = IPLog;
                collection_obj.IPLastUpdate = IP;

                ChangeSet changeSet = null;
                int changeCount = 0;
                DbTransaction trans = context.Connection.BeginTransaction();
                try
                {
                    context.Transaction = trans;
                    context.Gallery_Collections.Attach(collection_obj);
                    context.Refresh(RefreshMode.KeepCurrentValues, collection_obj);
                    changeSet = context.GetChangeSet();
                    changeCount = changeSet.Updates.Count;
                    context.SubmitChanges();

                    trans.Commit();
                   
                }
                catch (Exception ex)
                {
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    throw ex;
                }
                finally
                {
                    if (context.Connection != null && context.Connection.State == System.Data.ConnectionState.Open)
                    {
                        context.Connection.Close();
                    }
                }

                if (changeCount > 0) { return true; }
                else { return false; }
            }        
        }

        public void Delete(int Idx)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                context.DeferredLoadingEnabled = false;
                Gallery_Collection collection_obj = context.Gallery_Collections.Single(p => p.CollectionId == Idx);
                context.Gallery_Collections.DeleteOnSubmit(collection_obj);
                context.SubmitChanges();
            }
        }

        public void UpdateStatus(string userid, int topicid, char status)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                var query = from x in context.Gallery_Collections
                            where x.CollectionId == topicid
                            select x;

                foreach (Gallery_Collection cllection in query)
                {
                    cllection.Status = status;
                    cllection.IPLastUpdate = IP;
                    cllection.UserLastUpdate = Guid.Parse(userid);
                    cllection.LastModifieddDate = System.DateTime.Now;
                }
                context.SubmitChanges();
            }
        }
    }
}
