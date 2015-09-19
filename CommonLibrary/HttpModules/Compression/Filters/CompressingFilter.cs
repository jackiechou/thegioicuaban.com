using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace CommonLibrary.HttpModules.Compression.Filters
{
    public abstract class CompressingFilter : HttpOutputFilter
    {
        private bool _HasWrittenHeaders = false;
        protected CompressingFilter(Stream baseStream)
            : base(baseStream)
        {

        }
        public abstract string ContentEncoding { get; }
        protected bool HasWrittenHeaders
        {
            get { return _HasWrittenHeaders; }
        }
        protected void WriteHeaders()
        {
            HttpContext.Current.Response.AppendHeader("Content-Encoding", ContentEncoding);
            HttpContext.Current.Response.AppendHeader("X-Compressed-By", "DotNetNuke-Compression");
            _HasWrittenHeaders = true;
        }
    }
}
