using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary;

namespace MediaLibrary
{
    public class MediaAlbums
    {
        string IP = NetworkUtils.GetAddress(AddressType.IPv4);
        public MediaAlbums() { }

        public static List<Media_Albums> GetListByStatus(string status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                IQueryable<Media_Albums> query = from x in dbContext.Media_Albums select x;
                List<Media_Albums> lst = new List<Media_Albums>();
                if (string.IsNullOrEmpty(status))
                    lst = query.ToList();
                else
                    lst = query.Where(x => x.Status == status).ToList();
                return lst;
            }
        }

        public static List<Media_Albums> GetActiveListByTypeIdFixedNum(int TypeId, int iTotalItemCount)
        {
            using (System.Transactions.TransactionScope transScope = new System.Transactions.TransactionScope())
            {
                List<Media_Albums> lst = new List<Media_Albums>();
                using (MediaEntities dbContext = new MediaEntities())
                {
                    try
                    {
                        dbContext.CommandTimeout = 216000;
                        var query = from x in dbContext.Media_Albums where x.Status =="1" && x.TypeId ==TypeId orderby x.AlbumId descending select x;
                        int TotalItemCount = 0;
                        if (query.Count() > 0)
                        {
                            if (iTotalItemCount <= query.Count())
                                TotalItemCount = iTotalItemCount;
                            else
                                TotalItemCount = query.Count();
                        }                        
                        lst = query.Take(TotalItemCount).ToList();
                        transScope.Complete();
                    }catch(Exception  ex){ex.InnerException.Message.ToString();}
                    finally
                    {
                        if (dbContext.Connection.State != System.Data.ConnectionState.Closed)                       
                            dbContext.Connection.Close();                       
                    }
                    return lst;
                }                
            }
        }

        public static List<Media_Albums> GetActiveList(int? page, int? pageSize)
        {
            List<Media_Albums> lst = GetListByStatus("1");

            //if no page size specified, set page size to total # of rows in data table
            int _pagesize = (pageSize.HasValue) ? pageSize.Value : lst.Count;
            
            //paging logic <-> default to page 1 if no page supplied
            int _page = (page.HasValue) ? page.Value : 1;
            return lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();
        }

        //public static List<Media_Albums> GetTotalItemCountActiveList(int? pageIndex, int? pageSize, int iTotalItemCount)
        //{
        //    using (MediaEntities dbContext = new MediaEntities())
        //    {
        //        var lst = GetActiveListByFixedNum(iTotalItemCount);
        //        //if no page size specified, set page size to total # of rows in data table
        //        int _pagesize = (pageSize.HasValue) ? pageSize.Value : lst.Count;

        //        //paging logic <-> default to page 1 if no page supplied
        //        int _page = (pageIndex.HasValue) ? pageIndex.Value : 1;
        //        return lst.Skip((_page - 1) * _pagesize).Take(_pagesize).ToList();
        //    }
        //}

        //public static string LoadPaginationLinks(int? pageSize, int iTotalItemCount)
        //{
        //    string pagination_link = string.Empty;
        //    using (MediaEntities dbContext = new MediaEntities())
        //    {
        //        var lst = GetActiveListByFixedNum(iTotalItemCount);
        //        int TotalItemCount = 0;
        //        if (iTotalItemCount > lst.Count)
        //            TotalItemCount = lst.Count;
        //        else
        //            TotalItemCount = iTotalItemCount;

        //        int PageCount = 0;
        //        if (TotalItemCount > 0)
        //            PageCount = (int)Math.Ceiling(TotalItemCount / (double)pageSize);

        //        for (int i = 1; i <= PageCount; i++)
        //        {
        //            pagination_link += "<a href=\"javascript:void(0);\" onclick=\"ajaxAlbum(" + i + "," + pageSize + "," + iTotalItemCount + ")\">" + i + "</a>";
        //        }                
        //    }
        //    return pagination_link;
        //}
        //public static string LoadAlbumListWithPagination(int? pageIndex, int? pageSize, int iTotalItemCount)
        //{
        //    string strHTML = string.Empty;
        //    List<Media_Albums> lst = MediaAlbums.GetTotalItemCountActiveList(pageIndex, pageSize, iTotalItemCount);
        //    if (lst.Count > 0)
        //    {
        //        string result = string.Empty;
        //        foreach (var x in lst)
        //        {
        //            result += "<a href=\"#\"><h4><span>" + x.AlbumName + "</span></h4></a>";
        //        }
        //        strHTML = "<div class='album_info'>" + result + "</div><br/>"
        //              + "<div class='album_pagination'>"
        //                    + LoadPaginationLinks(pageSize, iTotalItemCount)
        //              + "</div>";
        //    }
        //    return strHTML;
        //}
   

        public Media_Albums GetDetails(int idx)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Albums albums_obj = dbContext.Media_Albums.Where(x => x.AlbumId == idx).FirstOrDefault();
                return albums_obj;
            }
        }

        //INSERT- UPDATE - DELETE 
        public int Insert(string UserId, int TypeId, string AlbumName, string FrontImage, string MainImage, string Description, string Status)
        {
            int returnValue = 0;
            using (MediaEntities dbContext = new MediaEntities())
            {
                using (System.Transactions.TransactionScope transcope = new System.Transactions.TransactionScope())
                {
                    string Alias = StringUtils.GenerateFriendlyString(AlbumName);
                    dbContext.CommandTimeout = Settings.CommandTimeout;
                    System.Nullable<Int32> SortKey = (from u in dbContext.Media_Albums select u.SortKey).DefaultIfEmpty(0).Max() + 1;

                    Media_Albums entity = new Media_Albums();
                    entity.TypeId = TypeId;
                    entity.AlbumName = AlbumName;
                    entity.Alias = Alias;
                    entity.FrontImage = FrontImage;
                    entity.MainImage = MainImage;
                    entity.Description = Description;
                    entity.SortKey = (SortKey == null ? 1 : SortKey);
                    entity.IPLog = IP;
                    entity.CreatedByUserId = new Guid(UserId);
                    entity.CreatedOnDate = System.DateTime.Now;
                    entity.Status = Status;

                    dbContext.AddToMedia_Albums(entity);
                    returnValue = dbContext.SaveChanges();
                    transcope.Complete();
                }                
            }
            return returnValue;
        }

        public int Update(int AlbumId, string UserId, int TypeId, string AlbumName, string FrontImage, string MainImage, string Description, string Status)
        {
            int i =0;
            using (MediaEntities dbContext = new MediaEntities())
            {
                using (System.Transactions.TransactionScope transcope = new System.Transactions.TransactionScope())
                {
                    //dbContext.CommandTimeout = Settings.CommandTimeout;
                    dbContext.Connection.Open();                    
                    string Alias = StringUtils.GenerateFriendlyString(AlbumName);
                    Media_Albums entity = dbContext.Media_Albums.Single(x => x.AlbumId == AlbumId);
                    entity.TypeId = TypeId;
                    entity.AlbumName = AlbumName;
                    entity.Alias = Alias;
                    entity.FrontImage = FrontImage;
                    entity.MainImage = MainImage;
                    entity.Description = Description;
                    entity.IPLastUpdate = IP;
                    entity.LastModifiedByUserId = new Guid(UserId);
                    entity.LastModifiedOnDate = System.DateTime.Now;
                    entity.Status = Status;
                    i = dbContext.SaveChanges();
                    dbContext.Connection.Close();
                    transcope.Complete();
                }   
            }
            return i;
        }

        public int UpdateStatus(string UserId, int AlbumId, string Status)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Albums album_obj = dbContext.Media_Albums.Single(x => x.AlbumId == AlbumId);               
                album_obj.IPLastUpdate = IP;
                album_obj.LastModifiedByUserId = new Guid(UserId);
                album_obj.LastModifiedOnDate = System.DateTime.Now;
                album_obj.Status = Status;
                int i = dbContext.SaveChanges();
                return i;
            }
        }

        public int UpdateSortKey(string UserId, int AlbumId, int SortKey)
        {
            using (MediaEntities dbContext = new MediaEntities())
            {
                dbContext.CommandTimeout = Settings.CommandTimeout;
                Media_Albums album_obj = dbContext.Media_Albums.Single(x => x.AlbumId == AlbumId);
                album_obj.IPLastUpdate = IP;
                album_obj.LastModifiedByUserId = new Guid(UserId);
                album_obj.LastModifiedOnDate = System.DateTime.Now;
                album_obj.SortKey = SortKey;
                int i = dbContext.SaveChanges();
                return i;
            }
        }
    }
}
