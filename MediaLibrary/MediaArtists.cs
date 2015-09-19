using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary;

namespace MediaLibrary
{
    public class MediaArtists
    {
        private List<Media_Artists> _items;
        private string IP = NetworkUtils.GetAddress(AddressType.IPv4);

        public MediaArtists(){ }
        public MediaArtists(List<Media_Artists> items)
        {
            if (items == null) items = LoadAll();
            _items = items;
        }

        //================================================================================
        public static List<Media_Artists> GetActiveListByFixedNum(int iTotalItemCount)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                var query = from x in dbContext.Media_Artists where x.Status.Equals("1") orderby x.ArtistId descending select x;
                int TotalItemCount = 0;                
                if (iTotalItemCount <= query.Count())
                    TotalItemCount = iTotalItemCount;
                else
                    TotalItemCount = query.Count();                
                return query.Take(TotalItemCount).ToList();
            }
        }
        //================================================================================

        public List<Media_Artists> LoadAll()
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                _items = (from x in dbContext.Media_Artists select x).ToList();
                return _items;
            }
        }

        public List<Media_Artists> LoadDatedItemsByPage(int startRow, int pageSize, string sortBy, bool sortAscending)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                //List<Media_Artists> list = (from p in dbContext.Media_Artists select p).Skip(startRow * pageSize).Take(pageSize).ToList();

                SortData(sortBy, sortAscending);
                //copy to an array
                int count = pageSize;
                _items = LoadAll();
                if ((_items.Count - startRow) < count) count = _items.Count - startRow;
                Media_Artists[] pagearray = new Media_Artists[count];
                
                _items.CopyTo(startRow, pagearray, 0, count);
                //then create a list from array
                List<Media_Artists> page = new List<Media_Artists>(pagearray);
                return page;
            }
        }

        public int LoadDatedItemsCount()
        {
            return _items.Count;
        }

        private void SortData(string sortBy, bool sortAscending)
        {
            if (sortBy.Equals("ArtistName", StringComparison.OrdinalIgnoreCase))
            {
                if (sortAscending)
                    _items.Sort(delegate(Media_Artists p1, Media_Artists p2)
                    { return p1.ArtistName.CompareTo(p2.ArtistName); });
                else
                    _items.Sort(delegate(Media_Artists p1, Media_Artists p2)
                    { return -p1.ArtistName.CompareTo(p2.ArtistName); });
            }
            else if (sortBy.Equals("ArtistId", StringComparison.OrdinalIgnoreCase))
            {
                if (sortAscending)
                    _items.Sort(delegate(Media_Artists p1, Media_Artists p2)
                    { return p1.ArtistId.CompareTo(p2.ArtistId); });
                else
                    _items.Sort(delegate(Media_Artists p1, Media_Artists p2)
                    { return -p1.ArtistId.CompareTo(p2.ArtistId); });
            }            
        }

        public Media_Artists Find(int key)
        {
            return _items.Find(delegate(Media_Artists d) { return (d.ArtistId == key); });
        }

        public Media_Artists GetDetails(int idx)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Artists artists_obj = dbContext.Media_Artists.Where(x => x.ArtistId == idx).FirstOrDefault();
                return artists_obj;
            }   
        }

        public void Delete(int key)
        {
            Media_Artists item = _items.Find(delegate(Media_Artists d) { return (d.ArtistId == key); });
            if (item != null) _items.Remove(item);
        }

        public static List<Media_Artists> GetListByStatus(string status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                IQueryable<Media_Artists> query = from x in dbContext.Media_Artists select x;
                List<Media_Artists> lst = new List<Media_Artists>();
                if (string.IsNullOrEmpty(status))
                    lst = query.ToList();
                else
                    lst = query.Where(x => x.Status == status).ToList();
                return lst;
            }
        }

       

        //INSERT- UPDATE - DELETE 
        public int Insert(string UserId, string ArtistName, string FrontImage, string MainImage, string Description, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                string Alias = StringUtils.GenerateFriendlyString(ArtistName);
                dbContext.CommandTimeout = Settings.CommandTimeout;
                System.Nullable<Int32> ListOrder = (from u in dbContext.Media_Artists select u.ListOrder).DefaultIfEmpty(0).Max() + 1;

                Media_Artists artists_obj = new Media_Artists();
                artists_obj.ArtistName = ArtistName;
                artists_obj.Alias = Alias;
                artists_obj.FrontImage = FrontImage;
                artists_obj.MainImage = MainImage;
                artists_obj.Description = Description;
                artists_obj.ListOrder = (ListOrder == null ? 1 : ListOrder);
                artists_obj.IPLog = IP;    
                artists_obj.CreatedOnDate = System.DateTime.Now;
                artists_obj.CreatedByUserId = new Guid(UserId);            
                artists_obj.Status = Status;
                dbContext.AddToMedia_Artists(artists_obj);
                int i  = dbContext.SaveChanges();
                return i;
            }
        }

        public int Update(int ArtistId, string UserId, string ArtistName, string FrontImage, string MainImage, string Description, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                string Alias = StringUtils.GenerateFriendlyString(ArtistName);
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Artists artists_obj = dbContext.Media_Artists.Single(x => x.ArtistId == ArtistId);
                artists_obj.ArtistName = ArtistName;
                artists_obj.Alias = Alias;
                artists_obj.FrontImage = FrontImage;
                artists_obj.MainImage = MainImage;
                artists_obj.Description = Description;
                artists_obj.IPLastUpdate = IP;
                artists_obj.LastModifiedDate = System.DateTime.Now;
                artists_obj.LastModifiedByUserId = new Guid(UserId);
                artists_obj.Status = Status;
                int i = dbContext.SaveChanges();
                return i;
            }        
        }

        public int UpdateStatus(string UserId, int ArtistId, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Artists artists_obj = dbContext.Media_Artists.Single(x => x.ArtistId == ArtistId);
                artists_obj.IPLastUpdate = IP;
                artists_obj.LastModifiedByUserId = new Guid(UserId);
                artists_obj.LastModifiedDate = System.DateTime.Now;
                artists_obj.Status = Status;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateSortKey(string UserId, int ArtistId, int ListOrder)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Artists artists_obj = dbContext.Media_Artists.Single(x => x.ArtistId == ArtistId);
                artists_obj.IPLastUpdate = IP;
                artists_obj.LastModifiedByUserId = new Guid(UserId);
                artists_obj.LastModifiedDate = System.DateTime.Now;
                artists_obj.ListOrder = ListOrder;
                int i = dbContext.SaveChanges();
                return i;
            }
        }
    }
}
