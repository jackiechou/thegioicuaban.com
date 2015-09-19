using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonLibrary.HttpModules.Compression.Filters
{
    public abstract class HttpOutputFilter : Stream
    {
        private Stream _sink;
        protected HttpOutputFilter(Stream baseStream)
        {
            _sink = baseStream;
        }
        protected Stream BaseStream
        {
            get { return _sink; }
        }
        public override bool CanRead
        {
            get { return false; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }
        public override bool CanWrite
        {
            get { return _sink.CanWrite; }
        }
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public override long Seek(long offset, SeekOrigin direction)
        {
            throw new NotSupportedException();
        }
        public override void SetLength(long length)
        {
            throw new NotSupportedException();
        }
        public override void Close()
        {
            _sink.Close();
        }
        public override void Flush()
        {
            _sink.Flush();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
