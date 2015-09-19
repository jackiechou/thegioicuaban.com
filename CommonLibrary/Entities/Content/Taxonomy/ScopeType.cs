using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Entities.Content.Taxonomy
{
    [Serializable()]
    public class ScopeType : IHydratable
    {

        #region "Private Members"

        private int _ScopeTypeId;

        private string _ScopeType;
        #endregion

        #region "Constructors"

        public ScopeType()
            : this(Null.NullString)
        {
        }

        public ScopeType(string scopeType)
        {
            _ScopeTypeId = Null.NullInteger;
            _ScopeType = scopeType;
        }

        #endregion

        #region "Public Properties"

        public int ScopeTypeId
        {
            get { return _ScopeTypeId; }
            set { _ScopeTypeId = value; }
        }

        public string Type
        {
            get { return _ScopeType; }
            set { _ScopeType = value; }
        }

        #endregion

        #region "IHydratable Implementation"

        public void Fill(System.Data.IDataReader dr)
        {
            ScopeTypeId = Null.SetNullInteger(dr["ScopeTypeID"]);
            Type = Null.SetNullString(dr["ScopeType"]);
        }

        public int KeyID
        {
            get { return ScopeTypeId; }
            set { ScopeTypeId = value; }
        }

        #endregion

        public override string ToString()
        {
            return Type;
        }

    }
}
