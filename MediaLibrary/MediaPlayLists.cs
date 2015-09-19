using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary;

namespace MediaLibrary
{
    public class MediaPlayLists
    {
        private List<Media_PlayLists> _items;
        string IP = NetworkUtils.GetAddress(AddressType.IPv4);
        public MediaPlayLists() { }
        public MediaPlayLists(List<Media_PlayLists> items)
        {
            if (items == null) items = LoadAll();
            _items = items;
        }

        //=================================================================================================
        public static List<Media_PlayLists> GetActiveListByTypeIdFixedNum(int TypeId, int iTotalItemCount)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                var query = from x in dbContext.Media_PlayLists where x.Status=="1" && x.TypeId==TypeId orderby x.PlayListId descending select x;
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
        //=================================================================================================

        public List<Media_PlayLists> LoadAll()
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                _items = (from x in dbContext.Media_PlayLists select x).ToList();
                return _items;
            }
        }
        public List<Media_PlayLists> LoadDatedItemsByPage(string status, int startRow, int pageSize, string sortBy, bool sortAscending)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                SortData(sortBy, sortAscending);
                //copy to an array
                int count = pageSize;
                _items = GetListByStatus(status);
                if ((_items.Count - startRow) < count) count = _items.Count - startRow;
                Media_PlayLists[] pagearray = new Media_PlayLists[count];

                _items.CopyTo(startRow, pagearray, 0, count);
                //then create a list from array
                List<Media_PlayLists> page = new List<Media_PlayLists>(pagearray);
                return page;

                //_items = (from p in dbContext.Media_PlayLists select p).OrderBy(x => x.SortOrder).Skip(startRow * pageSize).Take(pageSize).ToList();
                //return _items;
            }
        }
        private void SortData(string sortBy, bool sortAscending)
        {
            if (sortBy.Equals("PlayListName", StringComparison.OrdinalIgnoreCase))
            {
                if (sortAscending)
                    _items.Sort(delegate(Media_PlayLists p1, Media_PlayLists p2)
                    { return p1.PlayListName.CompareTo(p2.PlayListName); });
                else
                    _items.Sort(delegate(Media_PlayLists p1, Media_PlayLists p2)
                    { return -p1.PlayListName.CompareTo(p2.PlayListName); });
            }
            else if (sortBy.Equals("ComposerId", StringComparison.OrdinalIgnoreCase))
            {
                if (sortAscending)
                    _items.Sort(delegate(Media_PlayLists p1, Media_PlayLists p2)
                    { return p1.PlayListId.CompareTo(p2.PlayListId); });
                else
                    _items.Sort(delegate(Media_PlayLists p1, Media_PlayLists p2)
                    { return -p1.PlayListId.CompareTo(p2.PlayListId); });
            }
        }

        public int LoadDatedItemsCount()
        {
            return _items.Count;
        }
        public List<Media_PlayLists> GetListByStatus(string status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                IQueryable<Media_PlayLists> query = from x in dbContext.Media_PlayLists select x;
                List<Media_PlayLists> lst= new List<Media_PlayLists>();
                if (string.IsNullOrEmpty(status))
                    lst = query.ToList();
                else
                    lst = query.Where(x => x.Status == status).ToList();
                return lst;
            }        
        }
        public Media_PlayLists Find(int key)
        {
            return _items.Find(delegate(Media_PlayLists d) { return (d.PlayListId == key); });
        }
      
        public Media_PlayLists GetDetails(int idx)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_PlayLists playLists_obj = dbContext.Media_PlayLists.Where(x => x.PlayListId == idx).FirstOrDefault();
                return playLists_obj;
            }   
        }

        //INSERT- UPDATE - DELETE 
        public int Insert(string UserId, int TypeId, string PlayListName, string FrontImage, string MainImage, string Description, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                string Alias = StringUtils.GenerateFriendlyString(PlayListName);
                dbContext.CommandTimeout = Settings.CommandTimeout;
                System.Nullable<Int32> SortOrder = (from u in dbContext.Media_PlayLists select u.SortOrder).DefaultIfEmpty(0).Max() + 1;

                Media_PlayLists entity = new Media_PlayLists();
                entity.TypeId = TypeId;
                entity.PlayListName = PlayListName;
                entity.Alias = Alias;
                entity.FrontImage = FrontImage;
                entity.MainImage = MainImage;
                entity.Description = Description;
                entity.SortOrder = (SortOrder == null ? 1 : SortOrder);
                entity.IPLog = IP;
                entity.CreatedOnDate = System.DateTime.Now;
                entity.CreatedByUserId = new Guid(UserId);
                entity.Status = Status;
                dbContext.AddToMedia_PlayLists(entity);
                int i  = dbContext.SaveChanges();
                return i;
            }
        }

        public int Update(int PlayListId,int TypeId, string UserId, string PlayListName, string FrontImage, string MainImage, string Description, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                string Alias = StringUtils.GenerateFriendlyString(PlayListName);
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_PlayLists entity = dbContext.Media_PlayLists.Single(x => x.PlayListId == PlayListId);
                entity.TypeId = TypeId;
                entity.PlayListName = PlayListName;
                entity.Alias = Alias;
                entity.FrontImage = FrontImage;
                entity.MainImage = MainImage;
                entity.Description = Description;
                entity.IPModifiedLog = IP;
                entity.LastModifiedOnDate = System.DateTime.Now;
                entity.LastModifiedByUserId = new Guid(UserId);
                entity.Status = Status;
                int i = dbContext.SaveChanges();
                return i;
            }        
        }

        public int UpdateStatus(string UserId, int PlayListId, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_PlayLists playLists_obj = dbContext.Media_PlayLists.Single(x => x.PlayListId == PlayListId);
                playLists_obj.IPModifiedLog = IP;
                playLists_obj.LastModifiedOnDate = System.DateTime.Now;
                playLists_obj.LastModifiedByUserId = new Guid(UserId);
                playLists_obj.Status = Status;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateSortKey(string UserId, int PlayListId, int SortOrder)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_PlayLists playLists_obj = dbContext.Media_PlayLists.Single(x => x.PlayListId == PlayListId);
                playLists_obj.IPModifiedLog = IP;
                playLists_obj.LastModifiedOnDate = System.DateTime.Now;
                playLists_obj.LastModifiedByUserId = new Guid(UserId);
                playLists_obj.SortOrder = SortOrder;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public void Delete(int key)
        {
            Media_PlayLists item = _items.Find(delegate(Media_PlayLists d) { return (d.PlayListId == key); });
            if (item != null) _items.Remove(item);
        }
    }
}
