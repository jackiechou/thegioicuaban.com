using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Specialized;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;
using System.Data;
using CommonLibrary.Entities.Content.Taxonomy;
using CommonLibrary.Entities.Content.Common;

namespace CommonLibrary.Entities.Content
{
    public class ContentItem : BaseEntityInfo, IHydratable
    {
        #region "Private Members"

        private int _ContentItemId = Null.NullInteger;
        private string _Content;
        private int _ContentTypeId = Null.NullInteger;
        private int _TabID = Null.NullInteger;
        private int _ModuleID = Null.NullInteger;
        private string _ContentKey;
        private bool _Indexed;
        private NameValueCollection _Metadata;

        private List<Term> _Terms;
        #endregion

        #region "Public Properties"

        [XmlIgnore()]
        public int ContentItemId
        {
            get { return _ContentItemId; }
            set { _ContentItemId = value; }
        }

        [XmlIgnore()]
        public string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }

        [XmlIgnore()]
        public string ContentKey
        {
            get { return _ContentKey; }
            set { _ContentKey = value; }
        }

        [XmlIgnore()]
        public int ContentTypeId
        {
            get { return _ContentTypeId; }
            set { _ContentTypeId = value; }
        }

        [XmlIgnore()]
        public bool Indexed
        {
            get { return _Indexed; }
            set { _Indexed = value; }
        }

        [XmlIgnore()]
        public NameValueCollection Metadata
        {
            get
            {
                if (_Metadata == null)
                {
                    _Metadata = this.GetMetaData(ContentItemId);
                }
                return _Metadata;
            }
        }

        [XmlElement("moduleID")]
        public int ModuleID
        {
            get { return _ModuleID; }
            set { _ModuleID = value; }
        }

        [XmlElement("tabid")]
        public int TabID
        {
            get { return _TabID; }
            set { _TabID = value; }
        }

        public List<Term> Terms
        {
            get
            {
                if (_Terms == null)
                {
                    _Terms = this.GetTerms(ContentItemId);
                }
                return _Terms;
            }
        }

        #endregion

        #region "Protected Methods"

        protected override void FillInternal(IDataReader dr)
        {
            base.FillInternal(dr);

            ContentItemId = Null.SetNullInteger(dr["ContentItemID"]);
            Content = Null.SetNullString(dr["Content"]);
            ContentTypeId = Null.SetNullInteger(dr["ContentTypeID"]);
            TabID = Null.SetNullInteger(dr["TabID"]);
            ModuleID = Null.SetNullInteger(dr["ModuleID"]);
            ContentKey = Null.SetNullString(dr["ContentKey"]);
            Indexed = Null.SetNullBoolean(dr["Indexed"]);
        }

        #endregion

        #region "IHydratable Implementation"

        public virtual void Fill(System.Data.IDataReader dr)
        {
            FillInternal(dr);
        }

        [XmlIgnore]
        public virtual int KeyID
        {
            get { return ContentItemId; }
            set { ContentItemId = value; }
        }

        #endregion

    }
}
