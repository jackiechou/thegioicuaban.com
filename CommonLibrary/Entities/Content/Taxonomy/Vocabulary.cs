using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    [Serializable()]
    public class Vocabulary : BaseEntityInfo, IHydratable
    {

        #region "Private Members"

        private string _Description;
        private bool _IsSystem;
        private string _Name;
        private int _ScopeId;
        private ScopeType _ScopeType;
        private int _ScopeTypeId;
        private List<Term> _Terms;
        private VocabularyType _Type;
        private int _VocabularyId;

        private int _Weight;
        #endregion

        #region "Constructors"

        public Vocabulary()
            : this(Null.NullString, Null.NullString, VocabularyType.Simple)
        {
        }

        public Vocabulary(string name)
            : this(name, Null.NullString, VocabularyType.Simple)
        {
        }

        public Vocabulary(string name, string description)
            : this(name, description, VocabularyType.Simple)
        {
        }

        public Vocabulary(VocabularyType type)
            : this(Null.NullString, Null.NullString, type)
        {
        }

        public Vocabulary(string name, string description, VocabularyType type)
        {
            _Description = description;
            _Name = name;
            _Type = type;

            _ScopeId = Null.NullInteger;
            _ScopeTypeId = Null.NullInteger;
            _VocabularyId = Null.NullInteger;
            _Weight = 0;
        }

        #endregion

        #region "Public Properties"

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public bool IsHeirarchical
        {
            get { return (Type == VocabularyType.Hierarchy); }
        }

        public bool IsSystem
        {
            get
            {
                return _IsSystem;
            }
            set
            {
                _IsSystem = value;
            }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int ScopeId
        {
            get { return _ScopeId; }
            set { _ScopeId = value; }
        }

        //public ScopeType ScopeType
        //{
        //    get
        //    {
        //        if (_ScopeType == null)
        //        {
        //            _ScopeType = this.GetScopeType(_ScopeTypeId);
        //        }

        //        return _ScopeType;
        //    }
        //}

        public int ScopeTypeId
        {
            get { return _ScopeTypeId; }
            set { _ScopeTypeId = value; }
        }

        //public List<Term> Terms
        //{
        //    get
        //    {
        //        if (_Terms == null)
        //        {
        //            _Terms = this.GetTerms(_VocabularyId);
        //        }
        //        return _Terms;
        //    }
        //}

        public VocabularyType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public int VocabularyId
        {
            get { return _VocabularyId; }
            set { _VocabularyId = value; }
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
            VocabularyId = Null.SetNullInteger(dr["VocabularyID"]);
            switch (Convert.ToInt16(dr["VocabularyTypeID"]))
            {
                case 1:
                    Type = VocabularyType.Simple;
                    break;
                case 2:
                    Type = VocabularyType.Hierarchy;
                    break;
            }
            IsSystem = Null.SetNullBoolean(dr["IsSystem"]);
            Name = Null.SetNullString(dr["Name"]);
            Description = Null.SetNullString(dr["Description"]);
            ScopeId = Null.SetNullInteger(dr["ScopeID"]);
            ScopeTypeId = Null.SetNullInteger(dr["ScopeTypeID"]);
            Weight = Null.SetNullInteger(dr["Weight"]);

            //Fill base class properties
            FillInternal(dr);
        }

        public virtual int KeyID
        {
            get { return VocabularyId; }
            set { VocabularyId = value; }
        }

        #endregion

    }
}
