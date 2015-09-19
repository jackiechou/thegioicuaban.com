using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    [Serializable()]
    public class Term : BaseEntityInfo, IHydratable
    {

        #region "Private Members"

        private List<Term> _ChildTerms;
        private string _Description;
        private int _Left;
        private string _Name;
        private Nullable<int> _ParentTermId;
        private int _Right;
        private List<string> _Synonyms;
        private int _TermId;
        private Vocabulary _Vocabulary;
        private int _VocabularyId;

        private int _Weight;
        #endregion

        #region "Constructors"

        public Term()
            : this(Null.NullString, Null.NullString, Null.NullInteger)
        {
        }

        public Term(int vocabularyId)
            : this(Null.NullString, Null.NullString, vocabularyId)
        {
        }

        public Term(string name)
            : this(name, Null.NullString, Null.NullInteger)
        {
        }

        public Term(string name, string description)
            : this(name, description, Null.NullInteger)
        {
        }

        public Term(string name, string description, int vocabularyId)
        {
            _Description = description;
            _Name = name;
            _VocabularyId = vocabularyId;

            _ParentTermId = null;
            _TermId = Null.NullInteger;
            _Left = 0;
            _Right = 0;
            _Weight = 0;
        }

        #endregion

        #region "Public Properties"

        //public List<Term> ChildTerms
        //{
        //    get
        //    {
        //        if (_ChildTerms == null)
        //        {
        //            _ChildTerms = this.GetChildTerms(_TermId, _VocabularyId);
        //        }
        //        return _ChildTerms;
        //    }
        //}

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        //public bool IsHeirarchical
        //{
        //    get { return (Vocabulary.Type == VocabularyType.Hierarchy); }
        //}

        public int Left
        {
            get { return _Left; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public Nullable<int> ParentTermId
        {
            get { return _ParentTermId; }
            set { _ParentTermId = value; }
        }

        public int Right
        {
            get { return _Right; }
        }

        public List<string> Synonyms
        {
            get { return _Synonyms; }
        }

        public int TermId
        {
            get { return _TermId; }
            set { _TermId = value; }
        }

        //public Vocabulary Vocabulary
        //{
        //    get
        //    {
        //        if (_Vocabulary == null && _VocabularyId > Null.NullInteger)
        //        {
        //            _Vocabulary = this.GetVocabulary(_VocabularyId);
        //        }
        //        return _Vocabulary;
        //    }
        //}

        public int VocabularyId
        {
            get { return _VocabularyId; }
        }

        public int Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }

        #endregion

        #region "IHydratable Implementation"

        public virtual void Fill(System.Data.IDataReader dr)
        {
            TermId = Null.SetNullInteger(dr["TermID"]);
            Name = Null.SetNullString(dr["Name"]);
            Description = Null.SetNullString(dr["Description"]);
            Weight = Null.SetNullInteger(dr["Weight"]);
            _VocabularyId = Null.SetNullInteger(dr["VocabularyID"]);

            if (dr["TermLeft"] != DBNull.Value)
            {
                _Left = Convert.ToInt32(dr["TermLeft"]);
            }
            if (dr["TermRight"] != DBNull.Value)
            {
                _Right = Convert.ToInt32(dr["TermRight"]);
            }
            if (dr["ParentTermID"] != DBNull.Value)
            {
                ParentTermId = Convert.ToInt32(dr["ParentTermID"]);
            }

            //Fill base class properties
            FillInternal(dr);
        }

        public int KeyID
        {
            get { return TermId; }
            set { TermId = value; }
        }

        #endregion

    }
}
