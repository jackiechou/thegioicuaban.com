using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Exceptions
{
    [Serializable()]
    public class ExceptionInfo
    {
        private string _Method;
        private int _FileColumnNumber;
        private string _FileName;
        private int _FileLineNumber;
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        public int FileColumnNumber
        {
            get { return _FileColumnNumber; }
            set { _FileColumnNumber = value; }
        }
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
        public int FileLineNumber
        {
            get { return _FileLineNumber; }
            set { _FileLineNumber = value; }
        }
    }
}
