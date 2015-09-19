using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace CommonLibrary.Data.Serialization
{
    /// <summary>
    /// JavaScriptSerializer extension methods.
    /// </summary>
    public static class JavaScriptSerializerExtensions
    {
        /// <summary>
        /// Registers the byte array converter.
        /// </summary>
        /// <param name="serializer">The <see cref="JavaScriptSerializer"/> instance.</param>
        public static void RegisterByteArrayConverter(this JavaScriptSerializer serializer)
        {
            serializer.RegisterConverters(new[] { new ByteArrayConverter() });
        }
    }
}
