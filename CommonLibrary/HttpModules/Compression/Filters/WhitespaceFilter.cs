using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CommonLibrary.HttpModules.Compression.Filters
{
    public class WhitespaceFilter : HttpOutputFilter
    {
        private static Regex _reg;
        public WhitespaceFilter(Stream baseStream, Regex reg)
            : base(baseStream)
        {
            _reg = reg;
        }
        public override void Write(byte[] buf, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buf, offset, data, 0, count);
            string html = System.Text.Encoding.Default.GetString(buf);
            html = _reg.Replace(html, string.Empty);
            byte[] outdata = System.Text.Encoding.Default.GetBytes(html);
            BaseStream.Write(outdata, 0, outdata.GetLength(0));
        }
    }
}
