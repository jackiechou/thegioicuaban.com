using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MaxLengthAttribute : System.Attribute
    {
        public MaxLengthAttribute(int length)
        {
            _Length = length;
        }
        private int _Length = Null.NullInteger;
        public int Length
        {
            get { return _Length; }
        }
    }
}
