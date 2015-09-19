using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Banner
{
    [Serializable()]
    public class BannerInfo
    {
        private int _BannerId;
        private int _VendorId;
        private string _ImageFile;
        private string _BannerName;
        private string _URL;
        private int _Impressions;
        private double _CPM;
        private int _Views;
        private int _ClickThroughs;
        private System.DateTime _StartDate;
        private System.DateTime _EndDate;
        private string _CreatedByUser;
        private System.DateTime _CreatedDate;
        private int _BannerTypeId;
        private string _Description;
        private string _GroupName;
        private int _Criteria;
        private int _Width;
        private int _Height;
        public BannerInfo()
        {
        }
        public int BannerId
        {
            get { return _BannerId; }
            set { _BannerId = value; }
        }
        public int VendorId
        {
            get { return _VendorId; }
            set { _VendorId = value; }
        }
        public string ImageFile
        {
            get { return _ImageFile; }
            set { _ImageFile = value; }
        }
        public string BannerName
        {
            get { return _BannerName; }
            set { _BannerName = value; }
        }
        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }
        public int Impressions
        {
            get { return _Impressions; }
            set { _Impressions = value; }
        }
        public double CPM
        {
            get { return _CPM; }
            set { _CPM = value; }
        }
        public int Views
        {
            get { return _Views; }
            set { _Views = value; }
        }
        public int ClickThroughs
        {
            get { return _ClickThroughs; }
            set { _ClickThroughs = value; }
        }
        public System.DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        public System.DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        public string CreatedByUser
        {
            get { return _CreatedByUser; }
            set { _CreatedByUser = value; }
        }
        public System.DateTime CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }
        public int BannerTypeId
        {
            get { return _BannerTypeId; }
            set { _BannerTypeId = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public string GroupName
        {
            get { return _GroupName; }
            set { _GroupName = value; }
        }
        public int Criteria
        {
            get { return _Criteria; }
            set { _Criteria = value; }
        }
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
    }
}
