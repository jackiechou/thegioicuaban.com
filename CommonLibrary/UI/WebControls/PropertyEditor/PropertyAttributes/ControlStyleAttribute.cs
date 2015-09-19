using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ControlStyleAttribute : System.Attribute
    {
        public ControlStyleAttribute(string cssClass)
        {
            _CssClass = cssClass;
        }
        public ControlStyleAttribute(string cssClass, string width)
        {
            _CssClass = cssClass;
            _Width = Unit.Parse(width);
        }
        public ControlStyleAttribute(string cssClass, string width, string height)
        {
            _CssClass = cssClass;
            _Height = Unit.Parse(height);
            _Width = Unit.Parse(width);
        }
        private string _CssClass;
        private Unit _Height;
        private Unit _Width;
        public string CssClass
        {
            get { return _CssClass; }
        }
        public Unit Height
        {
            get { return _Height; }
        }
        public Unit Width
        {
            get { return _Width; }
        }
    }
}
