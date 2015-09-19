using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Common.Utilities;
using System.Xml.Serialization;
using CommonLibrary.Entities;
using CommonLibrary.Entities.Modules;

namespace CommonLibrary.UI.Skins
{
    [Serializable()]
    public class SkinPackageInfo : BaseEntityInfo, IHydratable
    {
        private int _SkinPackageID = Null.NullInteger;
        private int _PackageID = Null.NullInteger;
        private int _PortalID = Null.NullInteger;
        private string _SkinName;
        private Dictionary<int, string> _Skins = new Dictionary<int, string>();
        private string _SkinType;
        public int PackageID
        {
            get { return _PackageID; }
            set { _PackageID = value; }
        }
        public int SkinPackageID
        {
            get { return _SkinPackageID; }
            set { _SkinPackageID = value; }
        }
        public int PortalID
        {
            get { return _PortalID; }
            set { _PortalID = value; }
        }
        public string SkinName
        {
            get { return _SkinName; }
            set { _SkinName = value; }
        }
        [XmlIgnore()]
        public Dictionary<int, string> Skins
        {
            get { return _Skins; }
            set { _Skins = value; }
        }
        public string SkinType
        {
            get { return _SkinType; }
            set { _SkinType = value; }
        }
        public void Fill(System.Data.IDataReader dr)
        {
            SkinPackageID = Null.SetNullInteger(dr["SkinPackageID"]);
            PackageID = Null.SetNullInteger(dr["PackageID"]);
            SkinName = Null.SetNullString(dr["SkinName"]);
            SkinType = Null.SetNullString(dr["SkinType"]);
            base.FillInternal(dr);
            if (dr.NextResult())
            {
                while (dr.Read())
                {
                    int skinID = Null.SetNullInteger(dr["SkinID"]);
                    if (skinID > Null.NullInteger)
                    {
                        _Skins[skinID] = Null.SetNullString(dr["SkinSrc"]);
                    }
                }
            }
        }
        public int KeyID
        {
            get { return SkinPackageID; }
            set { SkinPackageID = value; }
        }
    }
}
