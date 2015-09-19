using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Script.Serialization;

namespace CommonLibrary.Modules
{
    public enum makejson
    {
        e_without_square_brackets,
        e_with_square_brackets
    }

    public class JsonMethods
    {
        #region Method 1 ==============================================================================
        /// <summary>
        /// requested this through an ajax call, you can call a => [{name:{name1:"ab",name2:"cd"},id:9}]
        /// var jsondatastructure = eval (yourResponseText);
        /// </summary>
        public string makejsonoftable(DataTable table, makejson e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in table.Rows)
            {
                if (sb.Length != 0)
                    sb.Append(",");
                sb.Append("{");
                StringBuilder sb2 = new StringBuilder();
                foreach (DataColumn col in table.Columns)
                {
                    string fieldname = col.ColumnName;
                    string fieldvalue = dr[fieldname].ToString();
                    if (sb2.Length != 0)
                        sb2.Append(",");
                    sb2.Append(string.Format("\"{0}\":\"{1}\"", fieldname, fieldvalue));


                }
                sb.Append(sb2.ToString());
                sb.Append("}");


            }
            if (e == makejson.e_with_square_brackets)
            {
                sb.Insert(0, "[");
                sb.Append("]");
            }
            return sb.ToString();
        }
        #endregion =====================================================================================

        private static List<Dictionary<string, object>> RowsToDictionary(DataTable table)
        {
            List<Dictionary<string, object>> objs = new List<Dictionary<string, object>>();
            foreach (DataRow dr in table.Rows)
            {
                Dictionary<string, object> drow = new Dictionary<string, object>();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    drow.Add(table.Columns.ToString(), dr);
                }
                objs.Add(drow);
            }
            return objs;
        }

        public static Dictionary<string, object> ToJson(DataTable table)
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            d.Add(table.TableName, RowsToDictionary(table)); return d;
        }

        public static Dictionary<string, object> ToJson(DataSet data)
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            foreach (DataTable table in data.Tables)
            {
                d.Add(table.TableName, RowsToDictionary(table));
            }
            return d;
        }

        //=============================================================================================
        //public string ToJson(this object obj)
        //{
        //    //Dictionary<string, object> dict = new Dictionary<string, object>();
        //    //dict.Add("id", 1);
        //    //dict.Add("title", "The title");

        //    var serializer = new JavaScriptSerializer();
        //    return serializer.Serialize(obj); // serializer.Serialize((object)dict);
        //}

        //public string ToJson(this object obj, int recursionDepth)
        //{
        //    var serializer = new JavaScriptSerializer();
        //    serializer.RecursionLimit = recursionDepth;
        //    return serializer.Serialize(obj);
        //}
    }
}
