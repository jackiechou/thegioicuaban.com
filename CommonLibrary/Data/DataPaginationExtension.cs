using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Data
{
    public static class DataPaginationExtension
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize)
        {
            return new PagedList<T>(source, index, pageSize);
        }
    }

    public class PagedList<T> : List<T>
    {
        private readonly int itemsRemaining;
        public int ItemsRemaining{
            get { return itemsRemaining; }
        }
        private readonly int totalCount;

        public int TotalCount{get { return totalCount; }}

        public PagedList(IQueryable<T> source, int index, int pageSize)
        {
            index = index > 1 ? index - 1 : 0;
            totalCount = source.Count();
            itemsRemaining = totalCount - ((index * pageSize) + pageSize);
            if (itemsRemaining < 0)
           {
               index = totalCount/pageSize;
             itemsRemaining = 0;
            }
            AddRange(source.Skip(index * pageSize).Take(pageSize));  
        }
    }
}
