using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonLibrary.Services.OutputCache
{
    public abstract class OutputCacheResponseFilter : Stream
    {
        private Stream _chainedStream;
        private string _cacheKey;
        public string CacheKey
        {
            get { return _cacheKey; }
            set { _cacheKey = value; }
        }
        public Stream ChainedStream
        {
            get { return _chainedStream; }
            set { _chainedStream = value; }
        }
        public OutputCacheResponseFilter(Stream filterChain, string cacheKey)
            : base()
        {
            _chainedStream = filterChain;
            _cacheKey = cacheKey;
        }
        public override abstract bool CanRead { get; }
        public override abstract bool CanSeek { get; }
        public override abstract bool CanWrite { get; }
        public override abstract long Length { get; }
        public override abstract long Position { get; set; }
        public abstract byte[] StopFiltering(int itemId, bool deleteData);
        public override abstract void Flush();
        public override abstract void Write(byte[] buffer, int offset, int count);
        public override abstract int Read(byte[] buffer, int offset, int count);
        public override abstract long Seek(long offset, SeekOrigin origin);
        public override abstract void SetLength(long value);
    }
}
