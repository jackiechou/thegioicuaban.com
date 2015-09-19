using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Common.Utilities
{
    public class UrlInfo
    {
        private int _UrlID;
        private int _PortalID;
        private string _Url;
        public UrlInfo()
        {
        }
        public int UrlID
        {
            get { return _UrlID; }
            set { _UrlID = value; }
        }
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }
    }
}
