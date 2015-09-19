using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace CommonLibrary.HttpModules.Compression.Filters
{
    public class DeflateFilter : CompressingFilter
    {
        private DeflateStream m_stream = null;
        public DeflateFilter(Stream baseStream)
            : base(baseStream)
        {
            m_stream = new DeflateStream(baseStream, CompressionMode.Compress);
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
            get { return "deflate"; }
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
