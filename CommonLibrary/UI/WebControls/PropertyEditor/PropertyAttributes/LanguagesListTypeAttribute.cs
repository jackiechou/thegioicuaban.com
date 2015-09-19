using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Services.Localization;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LanguagesListTypeAttribute : System.Attribute
    {
        public LanguagesListTypeAttribute(LanguagesListType type)
        {
            _ListType = type;
        }
        private LanguagesListType _ListType;
        public LanguagesListType ListType
        {
            get { return _ListType; }
        }
    }
}
