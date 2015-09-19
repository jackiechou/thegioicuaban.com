using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.Skins.EventListeners
{
    public class SkinEventListener
    {
        private SkinEventType _Type;
        private SkinEventHandler _skinEvent;
        public SkinEventListener(SkinEventType type, SkinEventHandler e)
        {
            _Type = type;
            _skinEvent = e;
        }
        public SkinEventType EventType
        {
            get { return _Type; }
        }
        public SkinEventHandler SkinEvent
        {
            get { return _skinEvent; }
        }
    }
}
