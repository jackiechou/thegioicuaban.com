using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using CommonLibrary.Common.Utilities;
using System.Data.Linq;
using System.ComponentModel;

namespace GalleryLibrary
{
    public class CustomGalleryFiles
    {
        [DataObjectField(true)]
        public int Gallery_TopicId { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string Description { get; set; }
        public int? ListOrder { get; set; }
        public string Tags { get; set; }
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

    public class GalleryFile
    {
        private static string IP = NetworkUtils.GetAddress(AddressType.IPv4);

        public static List<Gallery_File> GetActiveList(int CollectionId)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                //context.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, context.Gallery_Files);
                context.CommandTimeout = Settings.CommandTimeout;
                var list = (from f in context.Gallery_Files
                            join gcf in context.Gallery_Collection_Files
                             on f.FileId equals gcf.FileId
                            where f.Status == '1' && gcf.CollectionId == CollectionId
                            select f).ToList();
                return list;
            }
        }

        public static List<CustomGalleryFiles> GetList(int CollectionId, char status)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                var query = from f in context.Gallery_Files
                            join gcf in context.Gallery_Collection_Files
                            on f.FileId equals gcf.FileId
                            join cl in context.Gallery_Collections
                            on gcf.CollectionId equals cl.CollectionId
                            where f.Status == status && gcf.CollectionId == CollectionId
                            select new { f, gcf, cl };
                List<CustomGalleryFiles> list = new List<CustomGalleryFiles>();
                foreach (var c in query)
                {
                    CustomGalleryFiles file_obj = new CustomGalleryFiles();
                    file_obj.Gallery_TopicId = c.cl.TopicId;
                    file_obj.FileId = c.f.FileId;
                    file_obj.FileName = c.f.FileName;
                    file_obj.FileUrl = c.f.FileUrl;
                    file_obj.Description = c.f.Description;
                    file_obj.ListOrder = Convert.ToInt32(c.f.ListOrder);
                    file_obj.Tags = c.f.Tags;
                    file_obj.CreatedOnDate = Convert.ToDateTime(c.f.CreatedOnDate);
                    file_obj.LastModifieddDate = Convert.ToDateTime(c.f.LastModifieddDate);
                    file_obj.UserLog = c.f.UserLog.ToString();
                    file_obj.UserLastUpdate = c.f.UserLastUpdate.ToString();
                    file_obj.IPLog = c.f.IPLog;
                    file_obj.IPLastUpdate = c.f.IPLastUpdate;
                    file_obj.Status = c.f.Status;

                    file_obj.CollectionId = Convert.ToInt32(c.gcf.CollectionId);
                    list.Add(file_obj);
                }
                return list;
            }
        }

        public static CustomGalleryFiles GetDetails(int Idx)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                context.DeferredLoadingEnabled = false;
                var query = (from f in context.Gallery_Files
                             join gcf in context.Gallery_Collection_Files
                             on f.FileId equals gcf.FileId
                             join cl in context.Gallery_Collections
                             on gcf.CollectionId equals cl.CollectionId
                             join gt in context.Gallery_Topics
                             on cl.TopicId equals gt.Gallery_TopicId
                             where f.FileId == Idx
                             select new CustomGalleryFiles
                             {
                                 FileId = f.FileId,
                                 FileName = f.FileName,
                                 FileUrl = f.FileUrl,
                                 Description = f.Description,
                                 ListOrder = Convert.ToInt32(f.ListOrder),
                                 Tags = f.Tags,
                                 CreatedOnDate = Convert.ToDateTime(f.CreatedOnDate),
                                 LastModifieddDate = Convert.ToDateTime(f.LastModifieddDate),
                                 UserLog = f.UserLog.ToString(),
                                 UserLastUpdate = f.UserLastUpdate.ToString(),
                                 IPLog = f.IPLog,
                                 IPLastUpdate = f.IPLastUpdate,
                                 Status = f.Status,
                                 CollectionId = Convert.ToInt32(gcf.CollectionId),
                                 CollectionFileId = Convert.ToInt32(gcf.CollectionFileId),
                                 Gallery_TopicId = Convert.ToInt32(cl.TopicId)
                             }).SingleOrDefault();
                return query;
            }
        }

        public void Delete(int Idx)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.CommandTimeout = Settings.CommandTimeout;
                context.DeferredLoadingEnabled = false;
                Gallery_File gallery_obj = context.Gallery_Files.Single(p => p.FileId == Idx);
                context.Gallery_Files.DeleteOnSubmit(gallery_obj);

                Gallery_Collection_File collection_file_obj = context.Gallery_Collection_Files.Single(p => p.FileId == Idx);
                context.Gallery_Collection_Files.DeleteOnSubmit(collection_file_obj);
                context.SubmitChanges();
            }
        }

        public bool InsertData(int CollectionId, string FileName,string FileUrl, 
            string Description, string UserLog, string Status, string Tags)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                int? ListOrder = (int?)context.Gallery_Collections.Max(x => (int?)x.CollectionId) ?? 0;

                Gallery_File file = new Gallery_File();
                file.FileName = FileName;
                file.FileUrl = FileUrl;
                file.ListOrder = ListOrder + 1;
                file.Description = Description;
                file.UserLog = Guid.Parse(UserLog);
                file.CreatedOnDate = System.DateTime.Now;
                file.Status = Convert.ToChar(Status);
                file.Tags = Tags;
                file.IPLog = IP;

                bool result = false;
                context.DeferredLoadingEnabled = false;
                context.Gallery_Files.InsertOnSubmit(file);
                context.SubmitChanges();
                int FileId = file.FileId;
                if (FileId > 0)
                {
                    Gallery_Collection_File collection_file = new Gallery_Collection_File();
                    collection_file.CollectionId = CollectionId;
                    collection_file.FileId = FileId;

                    context.Gallery_Collection_Files.InsertOnSubmit(collection_file);
                    context.SubmitChanges();
                    result = true;
                }
                return result;
            }
        }

          public bool UpdateData(int Idx, int CollectionFileId, int CollectionId, string FileName, string FileUrl,
            int ListOrder, string Description, string Tags, string Status,  DateTime CreatedOnDate, string IPLog,string UserLog, string UserLastUpdate)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                context.DeferredLoadingEnabled = false;
                Gallery_File file = new Gallery_File();
                file.FileId = Idx;
                file.FileName = FileName;
                file.FileUrl = FileUrl;
                file.Description = Description;
                file.ListOrder = ListOrder;
                file.UserLog = Guid.Parse(UserLog);
                file.UserLastUpdate = Guid.Parse(UserLastUpdate);
                file.CreatedOnDate = CreatedOnDate;
                file.LastModifieddDate = System.DateTime.Now;
                file.Status = Convert.ToChar(Status);
                file.Tags = Tags;
                file.IPLog = IPLog;
                file.IPLastUpdate = IP;

                ChangeSet changeSet = null;
                int changeCount = 0;
                context.Gallery_Files.Attach(file);
                context.Refresh(RefreshMode.KeepCurrentValues, file);
                changeSet = context.GetChangeSet();
                changeCount = changeSet.Updates.Count;
                context.SubmitChanges();

                bool result = false;
                if (changeCount > 0)
                {
                    Gallery_Collection_File collection_file = new Gallery_Collection_File();
                    collection_file.CollectionFileId = CollectionFileId;
                    collection_file.CollectionId = CollectionId;
                    collection_file.FileId = Idx;

                    context.Gallery_Collection_Files.Attach(collection_file);
                    context.Refresh(RefreshMode.KeepCurrentValues, collection_file);
                    context.SubmitChanges();
                    result = true;
                }
                return result;
            }
        }

        public void UpdateStatus(string userid, int fileid, char status)
        {
            using (GalleryDataContext context = new GalleryDataContext(Settings.ConnectionString))
            {
                var query = from x in context.Gallery_Files
                            where x.FileId == fileid
                            select x;

                foreach (Gallery_File file in query)
                {
                    file.Status = status;
                    file.UserLastUpdate = Guid.Parse(userid);
                    file.IPLastUpdate = IP;
                    file.LastModifieddDate = System.DateTime.Now;
                }
                context.SubmitChanges();
            }
        }
    }
}
