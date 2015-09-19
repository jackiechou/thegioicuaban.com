using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.Skins
{
    [Serializable()]
    public class SkinInfo
    {
        private int _SkinId;
        private int _PortalId;
        private string _SkinRoot;
        private UI.Skins.SkinType _SkinType;
        private string _SkinSrc;
        public int SkinId
        {
            get { return _SkinId; }
            set { _SkinId = value; }
        }
        public int PortalId
        {
            get { return _PortalId; }
            set { _PortalId = value; }
        }
        public string SkinRoot
        {
            get { return _SkinRoot; }
            set { _SkinRoot = value; }
        }
        public UI.Skins.SkinType SkinType
        {
            get { return _SkinType; }
            set { _SkinType = value; }
        }
        public string SkinSrc
        {
            get { return _SkinSrc; }
            set { _SkinSrc = value; }
        }       
    }
}
