using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.HttpModules.Compression.Config
{
    public enum Algorithms
    {
        Deflate = 2,
        GZip = 1,
        None = 0,
        Default = -1
    }
}
