using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Common.Utilities
{
    public class UrlLogInfo
    {
        private int _UrlLogID;
        private int _UrlTrackingID;
        private System.DateTime _ClickDate;
        private int _UserID;
        private string _FullName;
        public UrlLogInfo()
        {
        }
        public int UrlLogID
        {
            get { return _UrlLogID; }
            set { _UrlLogID = value; }
        }
        public int UrlTrackingID
        {
            get { return _UrlTrackingID; }
            set { _UrlTrackingID = value; }
        }
        public System.DateTime ClickDate
        {
            get { return _ClickDate; }
            set { _ClickDate = value; }
        }
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }
    }
}
