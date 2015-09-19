using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Services.Search
{
    [Serializable()]
    public class SearchItemInfo
    {
        private int _SearchItemId;
        private string _Title;
        private string _Description;
        private int _Author;
        private System.DateTime _PubDate;
        private int _ModuleId;
        private string _SearchKey;
        private string _Content;
        private string _GUID;
        private int _ImageFileId;
        private int _HitCount;
        public SearchItemInfo()
        {
        }
        public SearchItemInfo(string Title, string Description, int Author, System.DateTime PubDate, int ModuleID, string SearchKey, string Content)
            : this(Title, Description, Author, PubDate, ModuleID, SearchKey, Content, "", Null.NullInteger)
        {
        }
        public SearchItemInfo(string Title, string Description, int Author, System.DateTime PubDate, int ModuleID, string SearchKey, string Content, string Guid)
            : this(Title, Description, Author, PubDate, ModuleID, SearchKey, Content, Guid, Null.NullInteger)
        {
        }
        public SearchItemInfo(string Title, string Description, int Author, System.DateTime PubDate, int ModuleID, string SearchKey, string Content, int Image)
            : this(Title, Description, Author, PubDate, ModuleID, SearchKey, Content, "", Image)
        {
        }
        public SearchItemInfo(string Title, string Description, int Author, System.DateTime PubDate, int ModuleID, string SearchKey, string Content, string Guid, int Image)
        {
            _Title = Title;
            _Description = Description;
            _Author = Author;
            _PubDate = PubDate;
            _ModuleId = ModuleID;
            _SearchKey = SearchKey;
            _Content = Content;
            _GUID = Guid;
            _ImageFileId = Image;
            _HitCount = 0;
        }
        public int SearchItemId
        {
            get { return _SearchItemId; }
            set { _SearchItemId = value; }
        }
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public int Author
        {
            get { return _Author; }
            set { _Author = value; }
        }
        public System.DateTime PubDate
        {
            get { return _PubDate; }
            set { _PubDate = value; }
        }
        public int ModuleId
        {
            get { return _ModuleId; }
            set { _ModuleId = value; }
        }
        public string SearchKey
        {
            get { return _SearchKey; }
            set { _SearchKey = value; }
        }
        public string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }
        public string GUID
        {
            get { return _GUID; }
            set { _GUID = value; }
        }
        public int ImageFileId
        {
            get { return _ImageFileId; }
            set { _ImageFileId = value; }
        }
        public int HitCount
        {
            get { return _HitCount; }
            set { _HitCount = value; }
        }
    }
}
