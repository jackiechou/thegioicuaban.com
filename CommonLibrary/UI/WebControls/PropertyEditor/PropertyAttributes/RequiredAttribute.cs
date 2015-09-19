using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredAttribute : System.Attribute
    {
        public RequiredAttribute(bool required)
        {
            _Required = required;
        }
        private bool _Required = false;
        public bool Required
        {
            get { return _Required; }
        }
    }
}
