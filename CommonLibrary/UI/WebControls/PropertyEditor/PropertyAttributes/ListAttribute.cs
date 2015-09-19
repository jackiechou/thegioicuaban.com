using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.WebControls.PropertyEditor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ListAttribute : System.Attribute
    {
        public ListAttribute(string listName, string parentKey, ListBoundField valueField, ListBoundField textField)
        {
            _ListName = listName;
            _ParentKey = parentKey;
            _TextField = textField;
            _ValueField = valueField;
        }
        private string _ListName;
        private string _ParentKey;
        private ListBoundField _TextField;
        private ListBoundField _ValueField;
        public string ListName
        {
            get { return _ListName; }
        }
        public string ParentKey
        {
            get { return _ParentKey; }
        }
        public ListBoundField TextField
        {
            get { return _TextField; }
        }
        public ListBoundField ValueField
        {
            get { return _ValueField; }
        }
    }
}
