using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Banner
{
    public enum BannerType : int
    {
        Banner = 1,
        MicroButton = 2,
        Button = 3,
        Block = 4,
        Skyscraper = 5,
        Text = 6,
        Script = 7
    }
    [Serializable()]
    public class BannerTypeInfo
    {
        private int _BannerTypeId;
        private string _BannerTypeName;
        public BannerTypeInfo()
        {
        }
        public BannerTypeInfo(int BannerTypeId, string BannerTypeName)
        {
            _BannerTypeId = BannerTypeId;
            _BannerTypeName = BannerTypeName;
        }
        public int BannerTypeId
        {
            get { return _BannerTypeId; }
            set { _BannerTypeId = value; }
        }
        public string BannerTypeName
        {
            get { return _BannerTypeName; }
            set { _BannerTypeName = value; }
        }
    }
}
