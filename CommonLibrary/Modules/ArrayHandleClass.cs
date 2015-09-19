using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using System.Data;

namespace CommonLibrary.Modules
{
    class ArrayHandleClass
    {
        public List<string> ConvertString2GenericClass(string stringSet)
        {
            //string stringSet = "a,b,c,d";
            string[] stringArray = stringSet.Split(new char[] { ',' });
            System.Collections.Generic.List<string> genericList = new System.Collections.Generic.List<string>(stringArray);
            return genericList;

            //List<string> list = new List<string>();
            //Array.ForEach(values, value => list.Add(value.ToString()));
        }

        public List<object> ConvertArray2IEnumerable(object[] input_obj)
        {
            //Many built-in collections implement IEnumerable such as Dictionary<>, HashSet<>, Hashtable, Queue, Stack
            List<object> genericList = new List<object>();
            if (input_obj is IEnumerable)
            {

                var enumerator = ((IEnumerable)input_obj).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    genericList.Add(enumerator.Current);
                }
            }
            return genericList;
        }

        public string[] ConvertArray2GenericClass(object[] inputArray)
        {            
            string[] result = Array.ConvertAll(inputArray, p => p.ToString());
            return result;
        }
        
        public IList ConvertObject2GernericClass(object input_obj)
        {
            IList collection = (IList)input_obj;
            //if (input_obj is IList)
            //    collection = ((IEnumerable)input_obj).Cast<object>().ToList();
            return collection;
        }

        public List<string> ConvertStringArray2List(string[] arrStr)
        {
            //string[] arrStr = { "Add", "Array", "Elements", "To", "List", "C#" };
            List<string> lstString = new List<string>();
            foreach (string stritem in arrStr)
            {
                lstString.Add(stritem);
            }
            return lstString;
        }

        public string[,] ConvertDataTableToArray(DataTable dt)
        {
            //Declare 2D-String array
            string[,] arrString =
                new string[dt.Rows.Count + 1, dt.Columns.Count];
            int index = 0;
            //Save ColumnName in 1st row of the array
            foreach (DataColumn dc in dt.Columns)
            {
                arrString[0, index] =
                    Convert.ToString(dc.ColumnName);
                index++;
            }
            //Reset Index
            index = 0;
            //Now save DataTable values in array,
            //here we start from second row as ColumnNames are stored in first row
            for (int row = 1; row < dt.Rows.Count + 1; row++)
            {
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    arrString[row, col] =
                        Convert.ToString(dt.Rows[row - 1][col]);
                }
            }
            //Return 2D-String Array
            return arrString;
        }

        private Hashtable ToHashTable(DataSet ds)
        {
            Hashtable hashtable = new Hashtable();
            string value;
            int rowVal;

            for (int row = 0; row < ds.Tables[0].Rows.Count; ++row)
            {
                rowVal = row + 1;
                value = "Value" + rowVal.ToString();
                hashtable.Add(value, ds.Tables[0].Rows[row][0].ToString());
            }
            return hashtable;
        }

        //Generate a string array from a string with , (Comma) as the separator
        public String[] GetArray(String str)
        {
            // Split() is a readyment function it ll split string based on char     // seprator
            String[] Keyword = str.Split(',');
            return Keyword;
        }
    }
}
