using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Common.Utilities
{
    public class UrlTrackingInfo
    {
        private int _UrlTrackingID;
        private int _PortalID;
        private string _Url;
        private string _UrlType;
        private int _Clicks;
        private System.DateTime _LastClick;
        private System.DateTime _CreatedDate;
        private bool _LogActivity;
        private bool _TrackClicks;
        private int _ModuleID;
        private bool _NewWindow;
        public UrlTrackingInfo()
        {
        }
        public int UrlTrackingID
        {
            get { return _UrlTrackingID; }
            set { _UrlTrackingID = value; }
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
        public string UrlType
        {
            get { return _UrlType; }
            set { _UrlType = value; }
        }
        public int Clicks
        {
            get { return _Clicks; }
            set { _Clicks = value; }
        }
        public System.DateTime LastClick
        {
            get { return _LastClick; }
            set { _LastClick = value; }
        }
        public System.DateTime CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }
        public bool LogActivity
        {
            get { return _LogActivity; }
            set { _LogActivity = value; }
        }
        public bool TrackClicks
        {
            get { return _TrackClicks; }
            set { _TrackClicks = value; }
        }
        public int ModuleID
        {
            get { return _ModuleID; }
            set { _ModuleID = value; }
        }
        public bool NewWindow
        {
            get { return _NewWindow; }
            set { _NewWindow = value; }
        }
    }
}
