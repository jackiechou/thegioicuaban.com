using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsReadOnlyAttribute : System.Attribute
    {
        public IsReadOnlyAttribute(bool read)
        {
            _IsReadOnly = read;
        }
        private bool _IsReadOnly = false;
        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
        }
    }
}
