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
    public class LanguagePackInfo : BaseEntityInfo, IHydratable
    {
        private int _LanguagePackID = Null.NullInteger;
        private int _LanguageID = Null.NullInteger;
        private int _PackageID = Null.NullInteger;
        private int _DependentPackageID = Null.NullInteger;
        public int LanguagePackID
        {
            get { return _LanguagePackID; }
            set { _LanguagePackID = value; }
        }
        public int LanguageID
        {
            get { return _LanguageID; }
            set { _LanguageID = value; }
        }
        public int PackageID
        {
            get { return _PackageID; }
            set { _PackageID = value; }
        }
        public int DependentPackageID
        {
            get { return _DependentPackageID; }
            set { _DependentPackageID = value; }
        }
        public LanguagePackType PackageType
        {
            get
            {
                if (DependentPackageID == -2)
                {
                    return LanguagePackType.Core;
                }
                else
                {
                    return LanguagePackType.Extension;
                }
            }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            LanguagePackID = Null.SetNullInteger(dr["LanguagePackID"]);
            LanguageID = Null.SetNullInteger(dr["LanguageID"]);
            PackageID = Null.SetNullInteger(dr["PackageID"]);
            DependentPackageID = Null.SetNullInteger(dr["DependentPackageID"]);
            base.FillInternal(dr);
        }
        public int KeyID
        {
            get { return LanguagePackID; }
            set { LanguagePackID = value; }
        }
    }
}
