using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace CommonLibrary.Common.Utilities
{    
    /// <summary>
    /// Json Extensions based on the JavaScript Serializer in System.web
    /// </summary>
    public static class JsonExtensions
    {

        /// <summary> 
        /// Serializes a type to Json. Note the type must be marked Serializable 
        /// or include a DataContract attribute. 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns>
        public static string ToJsonString(object value)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            string json = ser.Serialize(value);
            return json;
        }

        /// <summary> 
        /// Extension method on object that serializes the value to Json. 
        /// Note the type must be marked Serializable or include a DataContract attribute. 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static string ToJson(this object value)
        {
            return ToJsonString(value);
        }

        /// <summary> 
        /// Deserializes a json string into a specific type. 
        /// Note that the type specified must be serializable. 
        /// </summary> 
        /// <param name="json"></param> 
        /// <param name="type"></param> 
        /// <returns></returns> 
        public static object FromJsonString(string json, Type type)
        {
            // *** Have to use Reflection with a 'dynamic' non constant type instance 
            JavaScriptSerializer ser = new JavaScriptSerializer();

            object result = ser.GetType().GetMethod("Deserialize").MakeGenericMethod(type).Invoke(ser, new object[1] { json });
            return result;
        }

        /// <summary> 
        /// Extension method to string that deserializes a json string 
        /// into a specific type. 
        /// Note that the type specified must be serializable. 
        /// </summary> 
        /// <param name="json"></param> 
        /// <param name="type"></param> 
        /// <returns></returns> 
        public static object FromJson(this string json, Type type)
        {
            return FromJsonString(json, type);
        }

        public static TType FromJson<TType>(this string json)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();

            TType result = ser.Deserialize<TType>(json);
            return result;
        }
    }
}
