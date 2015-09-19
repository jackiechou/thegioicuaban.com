using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SortOrderAttribute : System.Attribute
    {
        private int _order;
        public SortOrderAttribute(int order)
        {
            _order = order;
        }
        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }
        public static int DefaultOrder
        {
            get { return Int32.MaxValue; }
        }
    }
}
