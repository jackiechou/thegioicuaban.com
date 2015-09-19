using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LabelModeAttribute : System.Attribute
    {
        public LabelModeAttribute(LabelMode mode)
        {
            _Mode = mode;
        }
        private LabelMode _Mode;
        public LabelMode Mode
        {
            get { return _Mode; }
        }
    }
}
