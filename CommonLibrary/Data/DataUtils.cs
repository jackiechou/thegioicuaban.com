using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace CommonLibrary.Data
{
    public static class DataUtils
    {
        public const BindingFlags MemberAccess =
           BindingFlags.Public | BindingFlags.NonPublic |
           BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public const BindingFlags MemberPublicInstanceAccess =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;


        /// <summary>
        /// Generates a unique Id as a string of up to 16 characters.
        /// Based on a GUID and the size takes that subset of a the
        /// Guid's 16 bytes to create a string id.
        /// 
        /// String Id contains numbers and lower case alpha chars 36 total.
        /// 
        /// Sizes: 6 gives roughly 99.97% uniqueness. 
        ///        8 gives less than 1 in a million doubles.
        ///        16 will give full GUID strength uniqueness
        /// </summary>
        /// <returns></returns>
        /// <summary>
        public static string GenerateUniqueId(int stringSize = 8)
        {
            string chars = "abcdefghijkmnopqrstuvwxyz1234567890";
            StringBuilder result = new StringBuilder(stringSize);
            int count = 0;



            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                result.Append(chars[b % (chars.Length - 1)]);
                count++;
                if (count >= stringSize)
                    return result.ToString();
            }
            return result.ToString();
        }


        /// Generates a unique numeric ID. Generated off a GUID and
        /// returned as a 64 bit long value
        /// </summary>
        /// <returns></returns>
        public static long GenerateUniqueNumericId()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// Copies the content of a data row to another. Runs through the target's fields
        /// and looks for fields of the same name in the source row. Structure must mathc
        /// or fields are skipped.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CopyDataRow(DataRow source, DataRow target)
        {
            DataColumnCollection columns = target.Table.Columns;

            for (int x = 0; x < columns.Count; x++)
            {
                string fieldname = columns[x].ColumnName;

                try
                {
                    target[x] = source[fieldname];
                }
                catch { ;}  // skip any errors
            }

            return true;
        }

        public static void CopyObjectFromDataRow(DataRow row, object targetObject)
        {
            MemberInfo[] miT = targetObject.GetType().FindMembers(MemberTypes.Field | MemberTypes.Property, MemberAccess, null, null);
            foreach (MemberInfo Field in miT)
            {
                string Name = Field.Name;
                if (!row.Table.Columns.Contains(Name))
                    continue;

                if (Field.MemberType == MemberTypes.Field)
                {
                    ((FieldInfo)Field).SetValue(targetObject, row[Name]);
                }
                else if (Field.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)Field).SetValue(targetObject, row[Name], null);
                }
            }
        }

        /// <summary>
        /// Copies the content of an object to a DataRow with matching field names.
        /// Both properties and fields are copied. If a field copy fails due to a
        /// type mismatch copying continues but the method returns false
        /// </summary>
        /// <param name="row"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CopyObjectToDataRow(DataRow row, object target)
        {
            bool result = true;

            MemberInfo[] miT = target.GetType().FindMembers(MemberTypes.Field | MemberTypes.Property, MemberAccess, null, null);
            foreach (MemberInfo Field in miT)
            {
                string name = Field.Name;
                if (!row.Table.Columns.Contains(name))
                    continue;

                try
                {
                    if (Field.MemberType == MemberTypes.Field)
                    {
                        row[name] = ((FieldInfo)Field).GetValue(target);
                    }
                    else if (Field.MemberType == MemberTypes.Property)
                    {
                        row[name] = ((PropertyInfo)Field).GetValue(target, null);
                    }
                }
                catch { result = false; }
            }

            return result;
        }

        /// <summary>
        /// The default SQL date used by InitializeDataRowWithBlanks. Considered a blank date instead of null.
        /// </summary>
        public static DateTime MinimumSqlDate = DateTime.Parse("01/01/1900");

        /// <summary>
        /// Initializes a Datarow containing NULL values with 'empty' values instead.
        /// Empty values are:
        /// String - ""
        /// all number types - 0 or 0.00
        /// DateTime - Value of MinimumSqlData (1/1/1900 by default);
        /// Boolean - false
        /// Binary values and timestamps are left alone
        /// </summary>
        /// <param name="row">DataRow to be initialized</param>
        public static void InitializeDataRowWithBlanks(DataRow row)
        {
            DataColumnCollection loColumns = row.Table.Columns;

            for (int x = 0; x < loColumns.Count; x++)
            {
                if (!row.IsNull(x))
                    continue;

                string lcRowType = loColumns[x].DataType.Name;

                if (lcRowType == "String")
                    row[x] = string.Empty;
                else if (lcRowType.StartsWith("Int"))
                    row[x] = 0;
                else if (lcRowType == "Byte")
                    row[x] = 0;
                else if (lcRowType == "Decimal")
                    row[x] = 0.00M;
                else if (lcRowType == "Double")
                    row[x] = 0.00;
                else if (lcRowType == "Boolean")
                    row[x] = false;
                else if (lcRowType == "DateTime")
                    row[x] = DataUtils.MinimumSqlDate;

                // Everything else isn't handled explicitly and left alone
                // Byte[] most specifically

            }
        }

        /// <summary>
        /// Maps a SqlDbType to a .NET type
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static Type SqlTypeToDotNetType(SqlDbType sqlType)
        {
            if (sqlType == SqlDbType.NText || sqlType == SqlDbType.Text ||
                sqlType == SqlDbType.VarChar || sqlType == SqlDbType.NVarChar)
                return typeof(string);
            else if (sqlType == SqlDbType.Int)
                return typeof(Int32);
            else if (sqlType == SqlDbType.Decimal || sqlType == SqlDbType.Money)
                return typeof(decimal);
            else if (sqlType == SqlDbType.Bit)
                return typeof(Boolean);
            else if (sqlType == SqlDbType.DateTime || sqlType == SqlDbType.DateTime2)
                return typeof(DateTime);
            else if (sqlType == SqlDbType.Char || sqlType == SqlDbType.NChar)
                return typeof(char);
            else if (sqlType == SqlDbType.Float)
                return typeof(Single);
            else if (sqlType == SqlDbType.Real)
                return typeof(Double);
            else if (sqlType == SqlDbType.BigInt)
                return typeof(Int64);
            else if (sqlType == SqlDbType.Image)
                return typeof(byte[]);
            else if (sqlType == SqlDbType.SmallInt)
                return typeof(byte);

            throw new InvalidCastException("Unable to convert " + sqlType.ToString() + " to .NET type.");
        }

        /// <summary>
        /// Maps a DbType to a .NET native type
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static Type DbTypeToDotNetType(DbType sqlType)
        {
            if (sqlType == DbType.String || sqlType == DbType.StringFixedLength || sqlType == DbType.AnsiString)
                return typeof(string);
            else if (sqlType == DbType.Int16 || sqlType == DbType.Int32)
                return typeof(Int32);
            else if (sqlType == DbType.Int64)
                return typeof(Int64);
            else if (sqlType == DbType.Decimal || sqlType == DbType.Currency)
                return typeof(decimal);
            else if (sqlType == DbType.Boolean)
                return typeof(Boolean);
            else if (sqlType == DbType.DateTime || sqlType == DbType.DateTime2 || sqlType == DbType.Date)
                return typeof(DateTime);
            else if (sqlType == DbType.Single)
                return typeof(Single);
            else if (sqlType == DbType.Double)
                return typeof(Double);
            else if (sqlType == DbType.Binary)
                return typeof(byte[]);
            else if (sqlType == DbType.SByte || sqlType == DbType.Byte)
                return typeof(byte);
            else if (sqlType == DbType.Guid)
                return typeof(Guid);

            throw new InvalidCastException("Unable to convert " + sqlType.ToString() + " to .NET type.");
        }

        /// <summary>
        /// Converts a .NET type into a DbType value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType DotNetTypeToDbType(Type type)
        {
            if (type == typeof(string))
                return DbType.String;
            else if (type == typeof(Int32))
                return DbType.Int32;
            else if (type == typeof(Int16))
                return DbType.Int16;
            else if (type == typeof(Int64))
                return DbType.Int64;
            else if (type == typeof(Guid))
                return DbType.Guid;
            else if (type == typeof(decimal))
                return DbType.Decimal;
            else if (type == typeof(double) || type == typeof(float))
                return DbType.Double;
            else if (type == typeof(Single))
                return DbType.Single;
            else if (type == typeof(bool) || type == typeof(Boolean))
                return DbType.Boolean;
            else if (type == typeof(DateTime))
                return DbType.DateTime;
            else if (type == typeof(DateTimeOffset))
                return DbType.DateTimeOffset;
            else if (type == typeof(byte))
                return DbType.Byte;
            else if (type == typeof(byte[]))
                return DbType.Object;

            throw new InvalidCastException(string.Format("Unable to cast {0} to a DbType", type.Name));
        }
        /// <summary>
        /// Converts a .NET type into a SqlDbType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType DotNetTypeToSqlType(Type type)
        {
            if (type == typeof(string))
                return SqlDbType.NVarChar;
            else if (type == typeof(Int32))
                return SqlDbType.Int;
            else if (type == typeof(Int16))
                return SqlDbType.SmallInt;
            else if (type == typeof(Int64))
                return SqlDbType.BigInt;
            else if (type == typeof(Guid))
                return SqlDbType.UniqueIdentifier;
            else if (type == typeof(decimal))
                return SqlDbType.Decimal;
            else if (type == typeof(double) || type == typeof(float))
                return SqlDbType.Float;
            else if (type == typeof(Single))
                return SqlDbType.Float;
            else if (type == typeof(bool) || type == typeof(Boolean))
                return SqlDbType.Bit;
            else if (type == typeof(DateTime))
                return SqlDbType.DateTime;
            else if (type == typeof(DateTimeOffset))
                return SqlDbType.DateTimeOffset;
            else if (type == typeof(byte))
                return SqlDbType.SmallInt;
            else if (type == typeof(byte[]))
                return SqlDbType.Image;

            throw new InvalidCastException(string.Format("Unable to cast {0} to a DbType", type.Name));
        }

        #region Minimal Sql Data Access Function

        /// <summary>
        /// Creates a Command object and opens a connection
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public static SqlCommand GetSqlCommand(string ConnectionString, string Sql, params SqlParameter[] Parameters)
        {
            SqlCommand Command = new SqlCommand();
            Command.CommandText = Sql;

            try
            {
                if (ConnectionString != string.Empty)
                    ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionString].ConnectionString;

                Command.Connection = new SqlConnection(ConnectionString);
                Command.Connection.Open();
            }
            catch
            {
                return null;
            }


            if (Parameters != null)
            {
                foreach (SqlParameter Parm in Parameters)
                {
                    Command.Parameters.Add(Parm);
                }
            }

            return Command;
        }

        /// <summary>
        /// Returns a SqlDataReader object from a SQL string.
        /// 
        /// Please ensure you close the Reader object
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="Sql"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public static SqlDataReader GetSqlDataReader(string ConnectionString, string Sql, params SqlParameter[] Parameters)
        {
            SqlCommand Command = GetSqlCommand(ConnectionString, Sql, Parameters);
            if (Command == null)
                return null;

            SqlDataReader Reader = null;
            try
            {
                Reader = Command.ExecuteReader();
            }
            catch
            {
                CloseConnection(Command);
                return null;
            }

            return Reader;
        }

        /// <summary>
        /// Returns a DataTable from a Sql Command string passed in.
        /// </summary>
        /// <param name="Tablename"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="Sql"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string Tablename, string ConnectionString, string Sql, params SqlParameter[] Parameters)
        {
            SqlCommand Command = GetSqlCommand(ConnectionString, Sql, Parameters);
            if (Command == null)
                return null;

            SqlDataAdapter Adapter = new SqlDataAdapter(Command);

            DataTable dt = new DataTable(Tablename);

            try
            {
                Adapter.Fill(dt);
            }
            catch
            {
                return null;
            }
            finally
            {
                CloseConnection(Command);
            }

            return dt;
        }


        /// <summary>
        /// Closes a connection
        /// </summary>
        /// <param name="Command"></param>
        public static void CloseConnection(SqlCommand Command)
        {
            if (Command.Connection != null &&
                Command.Connection.State == ConnectionState.Open)
                Command.Connection.Close();
        }
        #endregion


        /// <summary>
        /// Convert an IList&lt;T&gt; into a DataTable schema
        /// </summary>
        public static DataTable CreateDataTable<T>() where T : class
        {
            Type objType = typeof(T);
            DataTable table = new DataTable(objType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
            foreach (PropertyDescriptor property in properties)
            {
                Type propertyType = property.PropertyType;
                if (!CanUseType(propertyType)) continue; //shallow only

                //nullables must use underlying types
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                //enums also need special treatment
                if (propertyType.IsEnum)
                    propertyType = Enum.GetUnderlyingType(propertyType); //probably Int32
                //if you have nested application classes, they just get added. Check if this is valid?
                Debug.WriteLine("table.Columns.Add(\"" + property.Name + "\", typeof(" + propertyType.Name + "));");
                table.Columns.Add(property.Name, propertyType);
            }
            return table;
        }

        /// <summary>
        /// Convert an IList&lt;T&gt; into a DataTable
        /// </summary>
        /// <example><code>
        /// IList&lt;Person&gt; people = new List&lt;Person&gt;
        ///    {
        ///        new Person { Id= 1, DoB = DateTime.Now, Name = "Bob", Sex = Person.Sexes.Male }
        ///    };
        /// DataTable dt = DataUtil.ConvertToDataTable(people);
        /// </code></example>
        public static DataTable ConvertToDataTable<T>(IList<T> list) where T : class
        {
            DataTable table = CreateDataTable<T>();
            Type objType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
            //Debug.WriteLine("foreach (" + objType.Name + " item in list) {");
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor property in properties)
                {
                    if (!CanUseType(property.PropertyType)) continue; //shallow only
                    //Debug.WriteLine("row[\"" + property.Name + "\"] = item." + property.Name + "; //.HasValue ? (object)item." + property.Name + ": DBNull.Value;");
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value; //can't use null
                }
                Debug.WriteLine("//===");
                table.Rows.Add(row);
            }
            return table;
        }

        private static bool CanUseType(Type propertyType)
        {
            //only strings and value types
            if (propertyType.IsArray) return false;
            if (!propertyType.IsValueType && propertyType != typeof(string)) return false;
            return true;
        }

        /// <summary>
        /// Convert DataTable to IList&lt;T&gt;. Some column names should match property names- or you'll have a list of empty T entities.
        /// </summary>
        /// <example><code>
        /// IList&lt;Person&gt; people = new List&lt;Person&gt;
        ///    {
        ///        new Person { Id= 1, DoB = DateTime.Now, Name = "Bob", Sex = Person.Sexes.Male }
        ///    };
        /// DataTable dt = DataUtil.ConvertToDataTable(people);
        /// IList&lt;Person&gt; people2 = DataUtil.ConvertToList&lt;Person&gt;(dt); //round trip
        /// //Note that people2 is a list of cloned Person objects
        /// </code></example>
        public static IList<T> ConvertToList<T>(DataTable dt) where T : class, new()
        {
            if (dt == null || dt.Rows.Count == 0) return null;
            IList<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T obj = ConvertDataRowToEntity<T>(row);
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// Convert a single DataRow into an object of type T.
        /// </summary>
        public static T ConvertDataRowToEntity<T>(DataRow row) where T : class, new()
        {
            Type objType = typeof(T);
            T obj = Activator.CreateInstance<T>(); //hence the new() contsraint
            //Debug.WriteLine(objType.Name + " = new " + objType.Name + "();");
            foreach (DataColumn column in row.Table.Columns)
            {
                //may error if no match
                PropertyInfo property =
                    objType.GetProperty(column.ColumnName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null || !property.CanWrite)
                {
                    //Debug.WriteLine("//Property " + column.ColumnName + " not in object");
                    continue; //or throw
                }
                object value = row[column.ColumnName];
                if (value == DBNull.Value) value = null;
                property.SetValue(obj, value, null);
                Debug.WriteLine("obj." + property.Name + " = row[\"" + column.ColumnName + "\"];");
            }
            return obj;
        }
    }
}
