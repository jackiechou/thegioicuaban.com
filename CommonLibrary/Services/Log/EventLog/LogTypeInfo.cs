using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Log.EventLog
{
    [Serializable()]
    public class LogTypeInfo
    {
        private string _LogTypeKey;
        private string _LogTypeFriendlyName;
        private string _LogTypeDescription;
        private string _LogTypeOwner;
        private string _LogTypeCSSClass;
        public string LogTypeKey
        {
            get { return _LogTypeKey; }
            set { _LogTypeKey = value; }
        }
        public string LogTypeFriendlyName
        {
            get { return _LogTypeFriendlyName; }
            set { _LogTypeFriendlyName = value; }
        }
        public string LogTypeDescription
        {
            get { return _LogTypeDescription; }
            set { _LogTypeDescription = value; }
        }
        public string LogTypeOwner
        {
            get { return _LogTypeOwner; }
            set { _LogTypeOwner = value; }
        }
        public string LogTypeCSSClass
        {
            get { return _LogTypeCSSClass; }
            set { _LogTypeCSSClass = value; }
        }
    }
}
