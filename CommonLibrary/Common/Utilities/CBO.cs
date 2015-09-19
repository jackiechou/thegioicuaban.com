using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.Data;
using CommonLibrary.Services.Exceptions;
using System.Collections;
using System.IO;
using CommonLibrary.Entities.Modules;
using CommonLibrary.Entities;
using System.Text.RegularExpressions;

namespace CommonLibrary.Common.Utilities
{
    public class CBO
    {
        private const string defaultCacheByProperty = "ModuleID";
        private const int defaultCacheTimeOut = 20;
        private const string defaultPrimaryKey = "ItemID";
        private const string objectMapCacheKey = "ObjectMap_";
        private const string schemaCacheKey = "Schema_";
        private static object CreateObjectFromReader(Type objType, IDataReader dr, bool closeReader)
        {
            object objObject = null;
            bool isSuccess = Null.NullBoolean;
            bool canRead = true;
            if (closeReader)
            {
                canRead = false;
                if (dr.Read())
                {
                    canRead = true;
                }
            }
            try
            {
                if (canRead)
                {
                    objObject = CreateObject(objType, false);
                   // FillObjectFromReader(objObject, dr);
                }
                isSuccess = true;
            }
            finally
            {
                if ((!isSuccess))
                    closeReader = true;
                CloseDataReader(dr, closeReader);
            }
            return objObject;
        }
        private static IDictionary<TKey, TValue> FillDictionaryFromReader<TKey, TValue>(string keyField, IDataReader dr, IDictionary<TKey, TValue> objDictionary)
        {
            TValue objObject;
            TKey keyValue = default(TKey);
            try
            {
                while (dr.Read())
                {
                    objObject = (TValue)CreateObjectFromReader(typeof(TValue), dr, false);
                    if (keyField == "KeyID" && objObject is IHydratable)
                    {
                        keyValue = (TKey)Null.SetNull(((IHydratable)objObject).KeyID, keyValue);
                    }
                    else
                    {
                        if (typeof(TKey).Name == "Int32" && dr[keyField].GetType().Name == "Decimal")
                        {
                            keyValue = (TKey)Null.SetNull(dr[keyField], keyValue);
                        }
                        else if (typeof(TKey).Name.ToLower() == "string" && dr[keyField].GetType().Name.ToLower() == "dbnull")
                        {
                            keyValue = (TKey)Null.SetNull(dr[keyField], "");
                        }
                        else
                        {
                            keyValue = (TKey)Null.SetNull(dr[keyField], keyValue);
                        }
                    }
                    if (objObject != null)
                    {
                        objDictionary[keyValue] = objObject;
                    }
                }
            }
            finally
            {
                CloseDataReader(dr, true);
            }
            return objDictionary;
        }
        private static IList FillListFromReader(Type objType, IDataReader dr, IList objList, bool closeReader)
        {
            object objObject;
            bool isSuccess = Null.NullBoolean;
            try
            {
                while (dr.Read())
                {
                    objObject = CreateObjectFromReader(objType, dr, false);
                    objList.Add(objObject);
                }
                isSuccess = true;
            }
            finally
            {
                if ((!isSuccess))
                    closeReader = true;
                CloseDataReader(dr, closeReader);
            }
            return objList;
        }
        private static IList<TItem> FillListFromReader<TItem>(IDataReader dr, IList<TItem> objList, bool closeReader)
        {
            TItem objObject;
            bool isSuccess = Null.NullBoolean;
            try
            {
                while (dr.Read())
                {
                    objObject = (TItem)CreateObjectFromReader(typeof(TItem), dr, false);
                    objList.Add(objObject);
                }
                isSuccess = true;
            }
            finally
            {
                if ((!isSuccess))
                    closeReader = true;
                CloseDataReader(dr, closeReader);
            }
            return objList;
        }
        //private static void FillObjectFromReader(object objObject, IDataReader dr)
        //{
        //    try
        //    {
        //        if (objObject is IHydratable)
        //        {
        //            IHydratable objHydratable = objObject as IHydratable;
        //            if (objHydratable != null)
        //            {
        //                objHydratable.Fill(dr);
        //            }
        //        }
        //        else
        //        {
        //            HydrateObject(objObject, dr);
        //        }
        //    }
        //    catch (IndexOutOfRangeException iex)
        //    {
        //        if (Host.ThrowCBOExceptions)
        //        {
        //            throw new ObjectHydrationException("Error Reading DataReader", iex, objObject.GetType(), dr);
        //        }
        //        else
        //        {
        //            Exceptions.LogException(iex);
        //        }
        //    }
        //}
        //private static void HydrateObject(object objObject, IDataReader dr)
        //{
        //    PropertyInfo objPropertyInfo = null;
        //    Type objPropertyType = null;
        //    object objDataValue;
        //    Type objDataType;
        //    int intIndex;
        //    ObjectMappingInfo objMappingInfo = GetObjectMapping(objObject.GetType());
        //    if (objObject is BaseEntityInfo && !(objObject is DotNetNuke.Services.Scheduling.ScheduleItem))
        //    {
        //        ((BaseEntityInfo)objObject).FillBaseProperties(dr);
        //    }
        //    for (intIndex = 0; intIndex <= dr.FieldCount - 1; intIndex++)
        //    {
        //        if (objMappingInfo.Properties.TryGetValue(dr.GetName(intIndex).ToUpperInvariant(), out objPropertyInfo))
        //        {
        //            objPropertyType = objPropertyInfo.PropertyType;
        //            if (objPropertyInfo.CanWrite)
        //            {
        //                objDataValue = dr.GetValue(intIndex);
        //                objDataType = objDataValue.GetType();
        //                if (objDataValue == null || objDataValue == DBNull.Value)
        //                {
        //                    objPropertyInfo.SetValue(objObject, Null.SetNull(objPropertyInfo), null);
        //                }
        //                else if (objPropertyType.Equals(objDataType))
        //                {
        //                    objPropertyInfo.SetValue(objObject, objDataValue, null);
        //                }
        //                else
        //                {

        //                    if (objPropertyType.BaseType.Equals(typeof(System.Enum)))
        //                    {
        //                        if (Regex.IsMatch(objDataValue.ToString(), "^\\d+$"))
        //                        {
        //                            objPropertyInfo.SetValue(objObject, System.Enum.ToObject(objPropertyType, Convert.ToInt32(objDataValue)), null);
        //                        }
        //                        else
        //                        {
        //                            objPropertyInfo.SetValue(objObject, System.Enum.ToObject(objPropertyType, objDataValue), null);
        //                        }
        //                    }
        //                    else if (objPropertyType == typeof(Guid))
        //                    {
        //                        objPropertyInfo.SetValue(objObject, Convert.ChangeType(new Guid(objDataValue.ToString()), objPropertyType), null);
        //                    }
        //                    else if (objPropertyType == typeof(System.Version))
        //                    {
        //                        objPropertyInfo.SetValue(objObject, new Version(objDataValue.ToString()), null);
        //                    }
        //                    else if (objPropertyType == objDataType)
        //                    {
        //                        objPropertyInfo.SetValue(objObject, objDataValue, null);
        //                    }
        //                    else
        //                    {
        //                        objPropertyInfo.SetValue(objObject, Convert.ChangeType(objDataValue, objPropertyType), null);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        private static string GetCacheByProperty(Type objType)
        {
            string cacheProperty = defaultCacheByProperty;
            return cacheProperty;
        }
        private static int GetCacheTimeOutMultiplier(Type objType)
        {
            int cacheTimeOut = defaultCacheTimeOut;
            return cacheTimeOut;
        }
        private static string GetColumnName(PropertyInfo objProperty)
        {
            string columnName = objProperty.Name;
            return columnName;
        }
        private static ObjectMappingInfo GetObjectMapping(Type objType)
        {
            string cacheKey = objectMapCacheKey + objType.FullName;
            ObjectMappingInfo objMap = (ObjectMappingInfo)DataCache.GetCache(cacheKey);
            if (objMap == null)
            {
                objMap = new ObjectMappingInfo();
                objMap.ObjectType = objType.FullName;
                objMap.PrimaryKey = GetPrimaryKey(objType);
                objMap.TableName = GetTableName(objType);
                foreach (PropertyInfo objProperty in objType.GetProperties())
                {
                    objMap.Properties.Add(objProperty.Name.ToUpperInvariant(), objProperty);
                    objMap.ColumnNames.Add(objProperty.Name.ToUpperInvariant(), GetColumnName(objProperty));
                }
                DataCache.SetCache(cacheKey, objMap);
            }
            return objMap;
        }
        private static string GetPrimaryKey(Type objType)
        {
            string primaryKey = defaultPrimaryKey;
            return primaryKey;
        }
        private static string GetTableName(Type objType)
        {
            string tableName = string.Empty;
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = objType.Name;
                if (tableName.EndsWith("Info"))
                {
                    tableName.Replace("Info", string.Empty);
                }
            }
            if (!string.IsNullOrEmpty(Config.GetSetting("ObjectQualifier")))
            {
                tableName = Config.GetSetting("ObjectQualifier") + tableName;
            }
            return tableName;
        }
        //public static object CloneObject(object objObject)
        //{
        //    try
        //    {
        //        Type objType = objObject.GetType();
        //        object objNewObject = Activator.CreateInstance(objType);
        //        ObjectMappingInfo objMappingInfo = GetObjectMapping(objType);
        //        foreach (KeyValuePair<string, PropertyInfo> kvp in objMappingInfo.Properties)
        //        {
        //            PropertyInfo objProperty = kvp.Value;
        //            if (objProperty.CanWrite)
        //            {
        //                ICloneable objPropertyClone = objProperty.GetValue(objObject, null) as ICloneable;
        //                if (objPropertyClone == null)
        //                {
        //                    objProperty.SetValue(objNewObject, objProperty.GetValue(objObject, null), null);
        //                }
        //                else
        //                {
        //                    objProperty.SetValue(objNewObject, objPropertyClone.Clone(), null);
        //                }
        //                IEnumerable enumerable = objProperty.GetValue(objObject, null) as IEnumerable;
        //                if (enumerable != null)
        //                {
        //                    IList list = objProperty.GetValue(objNewObject, null) as IList;
        //                    if (list != null)
        //                    {
        //                        foreach (object obj in enumerable)
        //                        {
        //                            list.Add(CloneObject(obj));
        //                        }
        //                    }
        //                    IDictionary dic = objProperty.GetValue(objNewObject, null) as IDictionary;
        //                    if (dic != null)
        //                    {
        //                        foreach (DictionaryEntry de in enumerable)
        //                        {
        //                            dic.Add(de.Key, CloneObject(de.Value));
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return objNewObject;
        //    }
        //    catch (Exception exc)
        //    {
        //        //Exceptions.LogException(exc);
        //        return null;
        //    }
        //}
        public static void CloseDataReader(IDataReader dr, bool closeReader)
        {
            if (dr != null && closeReader)
            {
                dr.Close();
            }
        }
        public static TObject CreateObject<TObject>()
        {
            return (TObject)CreateObject(typeof(TObject), false);
        }
        public static TObject CreateObject<TObject>(bool initialise)
        {
            return (TObject)CreateObject(typeof(TObject), initialise);
        }
        public static object CreateObject(Type objType, bool initialise)
        {
            object objObject = Activator.CreateInstance(objType);
            //if (initialise)
            //{
            //    InitializeObject(objObject);
            //}
            return objObject;
        }
        public static TObject DeserializeObject<TObject>(string fileName)
        {
            return DeserializeObject<TObject>(XmlReader.Create(new FileStream(fileName, FileMode.Open, FileAccess.Read)));
        }
        public static TObject DeserializeObject<TObject>(XmlDocument document)
        {
            return DeserializeObject<TObject>(XmlReader.Create(new StringReader(document.OuterXml)));
        }
        public static TObject DeserializeObject<TObject>(Stream stream)
        {
            return DeserializeObject<TObject>(XmlReader.Create(stream));
        }
        public static TObject DeserializeObject<TObject>(TextReader reader)
        {
            return DeserializeObject<TObject>(XmlReader.Create(reader));
        }
        public static TObject DeserializeObject<TObject>(XmlReader reader)
        {
            TObject objObject = CreateObject<TObject>(true);
            IXmlSerializable xmlSerializableObject = objObject as IXmlSerializable;
            if (xmlSerializableObject == null)
            {
                XmlSerializer serializer = new XmlSerializer(objObject.GetType());
                objObject = (TObject)serializer.Deserialize(reader);
            }
            else
            {
                xmlSerializableObject.ReadXml(reader);
            }
            return objObject;
        }
        public static ArrayList FillCollection(IDataReader dr, Type objType)
        {
            return (ArrayList)FillListFromReader(objType, dr, new ArrayList(), true);
        }
        public static ArrayList FillCollection(IDataReader dr, Type objType, bool closeReader)
        {
            return (ArrayList)FillListFromReader(objType, dr, new ArrayList(), closeReader);
        }
        public static IList FillCollection(IDataReader dr, Type objType, ref IList objToFill)
        {
            return FillListFromReader(objType, dr, objToFill, true);
        }
        public static List<TItem> FillCollection<TItem>(IDataReader dr)
        {
            return (List<TItem>)FillListFromReader<TItem>(dr, new List<TItem>(), true);
        }
        public static IList<TItem> FillCollection<TItem>(IDataReader dr, ref IList<TItem> objToFill)
        {
            return FillListFromReader<TItem>(dr, objToFill, true);
        }
        public static IList<TItem> FillCollection<TItem>(IDataReader dr, IList<TItem> objToFill, bool closeReader)
        {
            return FillListFromReader<TItem>(dr, objToFill, closeReader);
        }
        public static ArrayList FillCollection(IDataReader dr, ref Type objType, ref int totalRecords)
        {
            ArrayList objFillCollection = (ArrayList)FillListFromReader(objType, dr, new ArrayList(), false);
            try
            {
                if (dr.NextResult())
                {
                    totalRecords = Globals.GetTotalRecords(ref dr);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CloseDataReader(dr, true);
            }
            return objFillCollection;
        }
        public static List<T> FillCollection<T>(IDataReader dr, ref int totalRecords)
        {
            IList<T> objFillCollection = FillCollection<T>(dr, new List<T>(), false);
            try
            {
                if (dr.NextResult())
                {
                    totalRecords = Globals.GetTotalRecords(ref dr);
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            finally
            {
                CloseDataReader(dr, true);
            }
            return (List<T>)objFillCollection;
        }
        public static IDictionary<int, TItem> FillDictionary<TItem>(IDataReader dr) where TItem : IHydratable
        {
            return FillDictionaryFromReader<int, TItem>("KeyID", dr, new Dictionary<int, TItem>());
        }
        public static IDictionary<int, TItem> FillDictionary<TItem>(IDataReader dr, ref IDictionary<int, TItem> objToFill) where TItem : IHydratable
        {
            return FillDictionaryFromReader<int, TItem>("KeyID", dr, objToFill);
        }
        public static Dictionary<TKey, TValue> FillDictionary<TKey, TValue>(string keyField, IDataReader dr)
        {
            return (Dictionary<TKey, TValue>)FillDictionaryFromReader<TKey, TValue>(keyField, dr, new Dictionary<TKey, TValue>());
        }
        public static Dictionary<TKey, TValue> FillDictionary<TKey, TValue>(string keyField, IDataReader dr, IDictionary<TKey, TValue> objDictionary)
        {
            return (Dictionary<TKey, TValue>)FillDictionaryFromReader<TKey, TValue>(keyField, dr, objDictionary);
        }
        public static TObject FillObject<TObject>(IDataReader dr)
        {
            return (TObject)CreateObjectFromReader(typeof(TObject), dr, true);
        }
        public static TObject FillObject<TObject>(IDataReader dr, bool closeReader)
        {
            return (TObject)CreateObjectFromReader(typeof(TObject), dr, closeReader);
        }
        public static object FillObject(IDataReader dr, Type objType)
        {
            return CreateObjectFromReader(objType, dr, true);
        }
        public static object FillObject(IDataReader dr, Type objType, bool closeReader)
        {
            return CreateObjectFromReader(objType, dr, closeReader);
        }

        public static IQueryable<TItem> FillQueryable<TItem>(IDataReader dr)
        {
            return FillListFromReader<TItem>(dr, new List<TItem>(), true).AsQueryable();
        }

        public static SortedList<TKey, TValue> FillSortedList<TKey, TValue>(string keyField, IDataReader dr)
        {
            return (SortedList<TKey, TValue>)FillDictionaryFromReader<TKey, TValue>(keyField, dr, new SortedList<TKey, TValue>());
        }
        public static TObject GetCachedObject<TObject>(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired)
        {
            return DataCache.GetCachedData<TObject>(cacheItemArgs, cacheItemExpired);
        }
        public static TObject GetCachedObject<TObject>(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired, bool saveInDictionary)
        {
            return DataCache.GetCachedData<TObject>(cacheItemArgs, cacheItemExpired, saveInDictionary);
        }
        public static Dictionary<string, PropertyInfo> GetProperties<TObject>()
        {
            return GetObjectMapping(typeof(TObject)).Properties;
        }
        public static Dictionary<string, PropertyInfo> GetProperties(Type objType)
        {
            return GetObjectMapping(objType).Properties;
        }
        public static void InitializeObject(object objObject)
        {
            foreach (PropertyInfo objPropertyInfo in GetObjectMapping(objObject.GetType()).Properties.Values)
            {
                if (objPropertyInfo.CanWrite)
                {
                    objPropertyInfo.SetValue(objObject, Null.SetNull(objPropertyInfo), null);
                }
            }
        }
        public static object InitializeObject(object objObject, Type objType)
        {
            foreach (PropertyInfo objPropertyInfo in GetObjectMapping(objType).Properties.Values)
            {
                if (objPropertyInfo.CanWrite)
                {
                    objPropertyInfo.SetValue(objObject, Null.SetNull(objPropertyInfo), null);
                }
            }
            return objObject;
        }
        public static void SerializeObject(object objObject, string fileName)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment)))
            {
                SerializeObject(objObject, writer);
                writer.Flush();
            }
        }
        public static void SerializeObject(object objObject, XmlDocument document)
        {
            StringBuilder sb = new StringBuilder();
            SerializeObject(objObject, XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Document)));
            document.LoadXml(sb.ToString());
        }
        public static void SerializeObject(object objObject, Stream stream)
        {
            using (XmlWriter writer = XmlWriter.Create(stream, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment)))
            {
                SerializeObject(objObject, writer);
                writer.Flush();
            }
        }
        public static void SerializeObject(object objObject, TextWriter textWriter)
        {
            using (XmlWriter writer = XmlWriter.Create(textWriter, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment)))
            {
                SerializeObject(objObject, writer);
                writer.Flush();
            }
        }
        public static void SerializeObject(object objObject, XmlWriter writer)
        {
            IXmlSerializable xmlSerializableObject = objObject as IXmlSerializable;
            if (xmlSerializableObject == null)
            {
                XmlSerializer serializer = new XmlSerializer(objObject.GetType());
                serializer.Serialize(writer, objObject);
            }
            else
            {
                xmlSerializableObject.WriteXml(writer);
            }
        }
    }
}
