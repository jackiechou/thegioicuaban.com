using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Collections;

namespace CommonLibrary.Common.Utilities
{
    public class CollectionsUtil
    {
        //public static DataTable ConvertToDataTable(IList list)
        //{
        //    try
        //    {
        //        DataTable table = CreateTable((Object[])list[0], list);

        //        foreach (Object[] objs in list)
        //        {
        //            int i = 0;
        //            DataRow row = table.NewRow();

        //            foreach (Object obj in objs)
        //            {
        //                row[i++] = obj;
        //            }

        //            table.Rows.Add(row);
        //        }

        //        return table;
        //    }
        //    catch { }
        //    return null;
        //}

        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);


            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);


            foreach (PropertyDescriptor prop in properties)
            {
                // HERE IS WHERE THE ERROR IS THROWN FOR NULLABLE TYPES
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            return table;
        }

        public static DataTable CreateTable(Object[] objs, IList list)
        {
            DataTable table = new DataTable();
            int i = 0;

            foreach (Object obj in objs)
            {
                Object[] objTemplate = ((Object[])list.Cast<Object>().FirstOrDefault(x => ((Object[])x)[i] != null));
                table.Columns.Add(null, obj == null ?
                    objTemplate == null || objTemplate[i] == null ? typeof(string)
                    : objTemplate.GetType() : obj.GetType());
            }

            return table;
        }
    }

}
