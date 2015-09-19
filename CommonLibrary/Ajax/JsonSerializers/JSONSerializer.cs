using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Ajax.JsonSerializers
{
    /// <summary>
    /// The high level JSON Serializer wrapper that provides 
    /// serialization and deserialization services to the application. 
    /// 
    /// This class merely defers operation to the specified JSON parsing implementation
    /// to allow for pluggable JSON serializers to be used.
    /// 
    /// Supported parsers include:
    /// 
    /// * West Wind Native that's built-in (no dependencies)   (This is the default)
    /// * JavaScriptSerializer (ASP.NET JavaScript Serializer)
    /// * JSON.NET   (requires JSON.NET assembly to be included and JSONNET_REFERENCE global Define    
    /// </summary>
    public class JSONSerializer
    {
        /// <summary>
        /// This property determines the default parser that is created when
        /// using the default constructor. This is also the default serializer
        /// used when using the AjaxMethodCallback control.
        /// 
        /// This property should be set only once at application startup typically
        /// in Application_Start of a Web app.
        /// </summary>
        public static SupportedJsonParserTypes DefaultJsonParserType = SupportedJsonParserTypes.CustomJsonSerializer;

        /// <summary>
        /// Determines whether fields are serialized. Supported only for the West Wind JSON Serializer        
        /// </summary>
        public static bool SerializeFields = false;

        private IJSONSerializer _serializer = null;


        /// <summary>
        /// Determines the date serialization mode supported 
        /// for the Westwind and Json.NET parsers (not the JavaScript JSON Serializer)
        /// </summary>
        public JsonDateEncodingModes DateSerializationMode
        {
            get { return _SerializeDateAsFormatString; }
            set { _SerializeDateAsFormatString = value; }
        }
        private JsonDateEncodingModes _SerializeDateAsFormatString = JsonDateEncodingModes.ISO;

        /// <summary>
        /// Determines if there are line breaks inserted into the 
        /// JSON to make it more easily human readable.
        /// 
        /// By default if running in DebugMode this flag will be set to true
        /// </summary>
        public bool FormatJsonOutput
        {
            get { return _FormatJsonOutput; }
            set { _FormatJsonOutput = value; }
        }
        private bool _FormatJsonOutput = false;



        /// <summary>
        /// Default Constructor - assigns default 
        /// </summary>
        public JSONSerializer()
            : this(DefaultJsonParserType)
        { }

        public JSONSerializer(IJSONSerializer serializer)
        {
            _serializer = serializer;
        }

        public JSONSerializer(SupportedJsonParserTypes parserType)
        {
            // The West Wind Parser is native
            if (parserType == SupportedJsonParserTypes.CustomJsonSerializer)
                _serializer = new CustomJsonSerializer(this);

#if (true) //JSONNET_REFERENCE)
            else if (parserType == SupportedJsonParserTypes.JsonNet)
                _serializer = new JsonNetJsonSerializer(this);
#endif
            else if (parserType == SupportedJsonParserTypes.JavaScriptSerializer)
                _serializer = new WebExtensionsJavaScriptSerializer(this);

            else
                throw new InvalidOperationException();
        }

        public string Serialize(object value)
        {
            return _serializer.Serialize(value);
        }

        public object Deserialize(string jsonString, Type type)
        {
            return _serializer.Deserialize(jsonString, type);
        }

        public TType Deserialize<TType>(string jsonString)
        {
            return (TType)Deserialize(jsonString, typeof(TType));
        }
    }

    public enum SupportedJsonParserTypes
    {
        /// <summary>
        /// Default - Custom JSON parser.
        /// </summary>
        CustomJsonSerializer,
        /// <summary>
        /// NewtonSoft JSON.NET JSON Parser
        /// </summary>
        JsonNet,
        /// <summary>
        /// The ASP.NET JavaScript Serializer
        /// </summary>
        JavaScriptSerializer
    }


    /// <summary>
    /// Enumeration that determines how JavaScript dates are
    /// generated in JSON output
    /// </summary>
    public enum JsonDateEncodingModes
    {
        NewDateExpression,
        MsAjax,
        ISO
    }
}
