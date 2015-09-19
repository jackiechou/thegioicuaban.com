using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace CommonLibrary.HttpModules.Compression.Filters
{
    public class GZipFilter : CompressingFilter
    {
        private GZipStream m_stream = null;
        public GZipFilter(Stream baseStream)
            : base(baseStream)
        {
            m_stream = new GZipStream(baseStream, CompressionMode.Compress);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!HasWrittenHeaders)
            {
                WriteHeaders();
            }
            m_stream.Write(buffer, offset, count);
        }
        public override string ContentEncoding
        {
            get { return "gzip"; }
        }
        public override void Close()
        {
            m_stream.Close();
        }
        public override void Flush()
        {
            m_stream.Flush();
        }
    }
}
