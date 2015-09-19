using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Data.Linq
{
    public interface ILinqEntity
    {
        void Detach();
        bool IsAttached();
        string ToEntityString(int indentLevel, string indentValue);
    }
}
