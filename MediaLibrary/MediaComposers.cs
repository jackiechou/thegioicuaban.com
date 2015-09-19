using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary;

namespace MediaLibrary
{
    public class MediaComposers
    {
        private static List<Media_Composers> _items;
        string IP = NetworkUtils.GetAddress(AddressType.IPv4);
        public MediaComposers() { }
        public MediaComposers(List<Media_Composers> items)
        {
            if (items == null) items = LoadAll();
            _items = items;
        }

        public static List<Media_Composers> LoadAll()
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                _items = (from x in dbContext.Media_Composers select x).ToList();
                return _items;
            }
        }


        public static List<Media_Composers> GetListByStatus(string status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                IQueryable<Media_Composers> query = from x in dbContext.Media_Composers select x;
                if (string.IsNullOrEmpty(status))
                    _items = query.ToList();
                else
                    _items = query.Where(x => x.Status == status).ToList();
                return _items;
            }        
        }

        public static List<Media_Composers> LoadDatedItemsByPage(string status, int startRow, int pageSize, string sortBy, bool sortAscending)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                //List<Media_Artists> list = (from p in dbContext.Media_Artists select p).Skip(startRow * pageSize).Take(pageSize).ToList();

                SortData(sortBy, sortAscending);
                //copy to an array
                int count = pageSize;
                _items = GetListByStatus(status);
                if ((_items.Count - startRow) < count) count = _items.Count - startRow;
                Media_Composers[] pagearray = new Media_Composers[count];

                _items.CopyTo(startRow, pagearray, 0, count);
                //then create a list from array
                List<Media_Composers> page = new List<Media_Composers>(pagearray);
                return page;
            }
        }

        public static int LoadDatedItemsCount()
        {
            return _items.Count;
        }

        private static void SortData(string sortBy, bool sortAscending)
        {
            if (sortBy.Equals("ComposerName", StringComparison.OrdinalIgnoreCase))
            {
                if (sortAscending)
                    _items.Sort(delegate(Media_Composers p1, Media_Composers p2)
                    { return p1.ComposerName.CompareTo(p2.ComposerName); });
                else
                    _items.Sort(delegate(Media_Composers p1, Media_Composers p2)
                    { return -p1.ComposerName.CompareTo(p2.ComposerName); });
            }
            else if (sortBy.Equals("ComposerId", StringComparison.OrdinalIgnoreCase))
            {
                if (sortAscending)
                    _items.Sort(delegate(Media_Composers p1, Media_Composers p2)
                    { return p1.ComposerId.CompareTo(p2.ComposerId); });
                else
                    _items.Sort(delegate(Media_Composers p1, Media_Composers p2)
                    { return -p1.ComposerId.CompareTo(p2.ComposerId); });
            }            
        }

        public Media_Composers Find(int key)
        {
            return _items.Find(delegate(Media_Composers d) { return (d.ComposerId == key); });
        }

        public Media_Composers GetDetails(int idx)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Composers composers_obj = dbContext.Media_Composers.Where(x => x.ComposerId == idx).FirstOrDefault();
                return composers_obj;
            }   
        }

        //INSERT- UPDATE - DELETE 
        public int Insert(string UserId, string ComposerName, string FrontImage, string MainImage, string Description, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                string Alias = StringUtils.GenerateFriendlyString(ComposerName);
                dbContext.CommandTimeout = Settings.CommandTimeout;
                System.Nullable<Int32> ListOrder = (from u in dbContext.Media_Composers select u.ListOrder).DefaultIfEmpty(0).Max() + 1;

                Media_Composers composers_obj = new Media_Composers();
                composers_obj.ComposerName = ComposerName;
                composers_obj.Alias = Alias;
                composers_obj.FrontImage = FrontImage;
                composers_obj.MainImage = MainImage;
                composers_obj.Description = Description;
                composers_obj.ListOrder = (ListOrder == null ? 1 : ListOrder);
                composers_obj.IPLog = IP;    
                composers_obj.CreatedOnDate = System.DateTime.Now;
                composers_obj.CreatedByUserId = new Guid(UserId);            
                composers_obj.Status = Status;
                dbContext.AddToMedia_Composers(composers_obj);
                int i  = dbContext.SaveChanges();
                return i;
            }
        }

        public int Update(int ComposerId, string UserId, string ComposerName, string FrontImage, string MainImage, string Description, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                string Alias = StringUtils.GenerateFriendlyString(ComposerName);
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Composers composers_obj = dbContext.Media_Composers.Single(x => x.ComposerId == ComposerId);
                composers_obj.ComposerName = ComposerName;
                composers_obj.Alias = Alias;
                composers_obj.FrontImage = FrontImage;
                composers_obj.MainImage = MainImage;
                composers_obj.Description = Description;
                composers_obj.IPModifiedLog = IP;
                composers_obj.LastModifiedOnDate = System.DateTime.Now;
                composers_obj.LastModifiedByUserId = new Guid(UserId);
                composers_obj.Status = Status;
                int i = dbContext.SaveChanges();
                return i;
            }        
        }

        public int UpdateStatus(string UserId, int ComposerId, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Composers composers_obj = dbContext.Media_Composers.Single(x => x.ComposerId == ComposerId);
                composers_obj.IPModifiedLog = IP;
                composers_obj.LastModifiedByUserId = new Guid(UserId);
                composers_obj.LastModifiedOnDate = System.DateTime.Now;
                composers_obj.Status = Status;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateSortKey(string UserId, int ComposerId, int ListOrder)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Composers composers_obj = dbContext.Media_Composers.Single(x => x.ComposerId == ComposerId);
                composers_obj.IPModifiedLog = IP;
                composers_obj.LastModifiedByUserId = new Guid(UserId);
                composers_obj.LastModifiedOnDate = System.DateTime.Now;
                composers_obj.ListOrder = ListOrder;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public void Delete(int key)
        {
            Media_Composers item = _items.Find(delegate(Media_Composers d) { return (d.ComposerId == key); });
            if (item != null) _items.Remove(item);
        }
    }
}
