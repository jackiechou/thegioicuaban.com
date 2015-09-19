using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using CommonLibrary.Entities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.Services.Localization
{
    [Serializable()]
    public class Locale : BaseEntityInfo, IHydratable
    {
        private string _Code;
        private int _LanguageID = Null.NullInteger;
        private string _Text;
        private string _Fallback;
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }
        public int LanguageID
        {
            get { return _LanguageID; }
            set { _LanguageID = value; }
        }
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
        public string Fallback
        {
            get { return _Fallback; }
            set { _Fallback = value; }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            LanguageID = Null.SetNullInteger(dr["LanguageID"]);
            Code = Null.SetNullString(dr["CultureCode"]);
            Text = Null.SetNullString(dr["CultureName"]);
            Fallback = Null.SetNullString(dr["FallbackCulture"]);
            base.FillInternal(dr);
        }
        public int KeyID
        {
            get { return LanguageID; }
            set { LanguageID = value; }
        }
    }
}
