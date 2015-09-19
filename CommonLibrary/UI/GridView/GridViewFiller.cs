using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace CommonLibrary.UI.GridView
{
    /// <summary>
    /// Fills a GridView with paged data
    /// </summary>
    public static class GridViewFiller
    {
        /// <summary>
        /// Fills the specified gridview with a page of data.
        /// </summary>
        /// <param name="gv">The gridview.</param>
        /// <param name="list">The single page of data.</param>
        /// <param name="count">The total count (to work out number of pages).</param>
        /// <param name="pageSize">Size of the page.</param>
        public static void Fill(System.Web.UI.WebControls.GridView gv, IList<object> list, int count, int pageSize)
        {
            //create an ObjectDateSource object programmatically
            ObjectDataSource ods = new ObjectDataSource();
            ods.ID = "ods" + gv.ID;           

            ods.EnablePaging = gv.AllowPaging;
            ods.TypeName = "ObjectAdaptor"; //can be a common base class
            ods.SelectMethod = "Select";
            ods.SelectCountMethod = "Count";
            ods.StartRowIndexParameterName = "startRowIndex";
            ods.MaximumRowsParameterName = "maximumRows";
            ods.EnableViewState = false;
            //when creating, inject the data into the table adaptor
            ods.ObjectCreating += delegate(object sender, ObjectDataSourceEventArgs e)
            { e.ObjectInstance = new ObjectAdaptor(list, count); };
            ods.DataBind();

            gv.PageSize = pageSize;
            gv.DataSource = ods;
            gv.DataBind();
        }
    }
}
