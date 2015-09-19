using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CommonLibrary.Common.Lists
{
    // <summary>
    /// This class is used to compare any 
    /// type(property) of a class for sorting.
    /// This class automatically fetches the 
    /// type of the property and compares.
    /// </summary>
    public sealed class GenericComparer<T> : IComparer<T>
    {
        public enum SortOrder { Ascending, Descending };

        #region Constructors
        public GenericComparer(string sortColumn, SortOrder sortingOrder)
        {
            this._sortColumn = sortColumn;
            this._sortingOrder = sortingOrder;
        }

        /// <summary>
        /// Constructor when passing in a sort expression containing both the Sort Column and the Sort Order
        /// e.g., "BPCode ASC".
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <remarks>
        /// This constructor is useful when using this with the ASP.NET ObjectDataSource, 
        /// which passes the SortParameterName in this format
        /// </remarks>
        public GenericComparer(string sortExpression)
        {
            string[] sortExprArray = sortExpression.Split(" ".ToCharArray());
            string sortColumn = sortExprArray[0];
            SortOrder sortingOrder;
            sortingOrder = SortOrder.Ascending;
            if (sortExprArray.Length > 1)
            {
                if (sortExprArray[1].ToUpper() == "DESC")
                {
                    sortingOrder = SortOrder.Descending;
                }
            }

            this._sortColumn = sortColumn;
            this._sortingOrder = sortingOrder;
        }

        #endregion



        #region public properties
        /// <summary>
        /// Column Name(public property of the class) to be sorted.
        /// </summary>
        private string _sortColumn;
        public string SortColumn
        {
            get { return _sortColumn; }
        }

        /// <summary>
        /// Sorting order (ASC OR DESC)
        /// </summary>
        private SortOrder _sortingOrder;
        public SortOrder SortingOrder
        {
            get { return _sortingOrder; }
        }

        #endregion

        /// <summary>
        /// Compare two objects of the same class, 
        /// based on the value of a given property
        /// </summary>
        /// <param name="x">First Object</param>
        /// <param name="y">Second Object</param>
        /// <returns>int</returns>
        public int Compare(T x, T y)
        {

            // User reflection to get the property
            PropertyInfo propertyInfo = typeof(T).GetProperty(_sortColumn);

            // Cast the property to IComparable, so we can use the built-in compare.
            IComparable obj1 = (IComparable)propertyInfo.GetValue(x, null);
            IComparable obj2 = (IComparable)propertyInfo.GetValue(y, null);

            // Order depends on Asc vs Desc.
            if (_sortingOrder == SortOrder.Ascending)
            {
                return (obj1.CompareTo(obj2));
            }
            else
            {
                return (obj2.CompareTo(obj1));
            }
        }
    }
}
