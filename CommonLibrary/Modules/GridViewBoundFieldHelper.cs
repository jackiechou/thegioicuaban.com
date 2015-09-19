using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace CommonLibrary.Modules
{
    public static class GridViewBoundFieldHelper
    {
        public static int GetIndex(GridView grd, string fieldName)
        {
            for (int i = 0; i < grd.Columns.Count; i++)
            {
                DataControlField field = grd.Columns[i];

                BoundField bfield = field as BoundField;

                //Assuming accessing happens at data level, e.g with data field's name
                if (bfield != null && bfield.DataField == fieldName)
                    return i;
            }
            return -1;
        }

        public static BoundField GetField(GridView grd, string fieldName)
        {
            int index = GetIndex(grd, fieldName);
            return (index == -1) ? null : grd.Columns[index] as BoundField;
        }

        public static string GetText(GridViewRow row, string fieldName)
        {
            GridView grd = row.NamingContainer as GridView;
            if (grd != null)
            {
                int index = GetIndex(grd, fieldName);
                if (index != -1)
                    return row.Cells[index].Text;
            }
            return "";
        }

        public static string GetText(GridView grd, int rowIndex, string fieldName)
        {
            GridViewRow row = grd.Rows[rowIndex];
            return GetText(row, fieldName);
        }
    }
}
