using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FormatAttribute : System.Attribute
    {
        public FormatAttribute(string format)
        {
            _Format = format;
        }
        private string _Format;
        public string Format
        {
            get { return _Format; }
        }
    }
}
