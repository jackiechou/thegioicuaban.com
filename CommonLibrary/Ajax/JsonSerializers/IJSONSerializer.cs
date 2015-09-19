using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Ajax.JsonSerializers
{
    public interface IJSONSerializer
    {
        string Serialize(object value);
        object Deserialize(string jsonString, Type type);
        JsonDateEncodingModes DateSerializationMode { get; set; }
        bool FormatJsonOutput { get; set; }
    }

    public class JSONSerializerBase
    {
        /// <summary>
        /// Master instance of the JSONSerializer that the user interacts with
        /// Used to read option properties
        /// </summary>
        protected JSONSerializer masterSerializer = null;


        /// <summary>
        /// Encodes Dates as a JSON string value that is compatible
        /// with MS AJAX and is safe for JSON validators. If false
        /// serializes dates as new Date() expression instead.
        /// 
        /// The default is true.
        /// </summary>
        public JsonDateEncodingModes DateSerializationMode
        {
            get { return masterSerializer.DateSerializationMode; }
            set { masterSerializer.DateSerializationMode = value; }
        }


        /// <summary>
        /// Determines if there are line breaks inserted into the 
        /// JSON to make it more easily human readable.
        /// </summary>
        public bool FormatJsonOutput
        {
            get { return masterSerializer.FormatJsonOutput; }
            set { masterSerializer.FormatJsonOutput = value; }
        }

        /// <summary>
        ///  Force a master Serializer to be passed for settings
        /// </summary>
        /// <param name="serializer"></param>
        public JSONSerializerBase(JSONSerializer serializer)
        {
            masterSerializer = serializer;
        }

    }
}
