using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.Sitemap
{
    public class SitemapUrl
    {

        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }


        private DateTime _lastModified;
        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = value; }
        }


        private SitemapChangeFrequency _changeFrequency;
        public SitemapChangeFrequency ChangeFrequency
        {
            get { return _changeFrequency; }
            set { _changeFrequency = value; }
        }


        private float _priority;
        public float Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

    }
}
