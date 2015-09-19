using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Common
{
    public class FileItem
    {
        private string _Value;
        private string _Text;
        public FileItem(string value, string text)
        {
            _Value = value;
            _Text = text;
        }
        public string Value
        {
            get { return _Value; }
        }
        public string Text
        {
            get { return _Text; }
        }
    }
}
