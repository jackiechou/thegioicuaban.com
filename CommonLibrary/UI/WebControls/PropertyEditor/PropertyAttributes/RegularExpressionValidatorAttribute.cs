using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RegularExpressionValidatorAttribute : System.Attribute
    {
        public RegularExpressionValidatorAttribute(string expression)
        {
            _Expression = expression;
        }
        private string _Expression;
        public string Expression
        {
            get { return _Expression; }
        }
    }
}
