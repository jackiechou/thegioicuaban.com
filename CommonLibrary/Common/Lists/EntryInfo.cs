using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;

namespace CommonLibrary.Common.Lists
{
    [Serializable()]
    public class ListEntryInfo
    {
        private int _EntryID;
        private int _PortalID;
        private string _Key = Null.NullString;
        private string _ListName = Null.NullString;
        private string _DisplayName = Null.NullString;
        private string _Value = Null.NullString;
        private string _Text = Null.NullString;
        private string _Description = Null.NullString;
        private string _Parent = Null.NullString;
        private int _ParentID = 0;
        private int _Level = 0;
        private int _SortOrder = 0;
        private int _DefinitionID = 0;
        private bool _HasChildren = false;
        private string _ParentKey = Null.NullString;
        private bool _systemlist = false;
        public ListEntryInfo()
        {
        }
        public int EntryID
        {
            get { return _EntryID; }
            set { _EntryID = value; }
        }
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        public string Key
        {
            get
            {
                string _Key = ParentKey.Replace(":", ".");
                if (!string.IsNullOrEmpty(_Key))
                {
                    _Key += ".";
                }
                return _Key + ListName + ":" + Value;
            }
        }
        public string ListName
        {
            get { return _ListName; }
            set { _ListName = value; }
        }
        public string DisplayName
        {
            get { return ListName + ":" + Text; }
        }
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public int ParentID
        {
            get { return _ParentID; }
            set { _ParentID = value; }
        }
        public string Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }
        public int Level
        {
            get { return _Level; }
            set { _Level = value; }
        }
        public int SortOrder
        {
            get { return _SortOrder; }
            set { _SortOrder = value; }
        }
        public int DefinitionID
        {
            get { return _DefinitionID; }
            set { _DefinitionID = value; }
        }
        public bool HasChildren
        {
            get { return _HasChildren; }
            set { _HasChildren = value; }
        }
        public string ParentKey
        {
            get { return _ParentKey; }
            set { _ParentKey = value; }
        }
        public bool SystemList
        {
            get { return _systemlist; }
            set { _systemlist = value; }
        }
    }
}
