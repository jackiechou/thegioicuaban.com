using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.Skins.EventListeners
{
    public class SkinEventArgs : EventArgs
    {
        private Skin _Skin;
        public SkinEventArgs(Skin skin)
        {
            _Skin = skin;
        }
        public Skin Skin
        {
            get { return _Skin; }
        }
    }
}
