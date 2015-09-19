using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities;

namespace CommonLibrary.Common.Lists
{
    [Serializable()]
    public class ListInfo : BaseEntityInfo
    {
        private string mName = Null.NullString;
        private int mLevel = 0;
        private int mDefinitionID = Null.NullInteger;
        private int mEntryCount = 0;
        private int mParentID = 0;
        private string mParentKey = Null.NullString;
        private string mParent = Null.NullString;
        private string mParentList = Null.NullString;
        private bool mIsPopulated = Null.NullBoolean;
        private int mPortalID = Null.NullInteger;
        private bool mEnableSortOrder = Null.NullBoolean;
        private bool mSystemList = Null.NullBoolean;
        public ListInfo(string Name)
            : base()
        {
            mName = Name;
        }
        public ListInfo()
            : base()
        {
        }
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        public string DisplayName
        {
            get
            {
                string _DisplayName = Parent;
                if (!string.IsNullOrEmpty(_DisplayName))
                {
                    _DisplayName += ":";
                }
                return _DisplayName + Name;
            }
        }
        public int Level
        {
            get { return mLevel; }
            set { mLevel = value; }
        }
        public int DefinitionID
        {
            get { return mDefinitionID; }
            set { mDefinitionID = value; }
        }
        public string Key
        {
            get
            {
                string _Key = ParentKey;
                if (!string.IsNullOrEmpty(_Key))
                {
                    _Key += ":";
                }
                return _Key + Name;
            }
        }
        public int EntryCount
        {
            get { return mEntryCount; }
            set { mEntryCount = value; }
        }
        public int PortalID
        {
            get { return mPortalID; }
            set { mPortalID = value; }
        }
        public int ParentID
        {
            get { return mParentID; }
            set { mParentID = value; }
        }
        public string ParentKey
        {
            get { return mParentKey; }
            set { mParentKey = value; }
        }
        public string Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }
        public string ParentList
        {
            get { return mParentList; }
            set { mParentList = value; }
        }
        public bool IsPopulated
        {
            get { return mIsPopulated; }
            set { mIsPopulated = value; }
        }
        public bool EnableSortOrder
        {
            get { return mEnableSortOrder; }
            set { mEnableSortOrder = value; }
        }
        public bool SystemList
        {
            get { return mSystemList; }
            set { mSystemList = value; }
        }
    }
}
