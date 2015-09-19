using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.UI.Skins.EventListeners
{
    public class ContainerEventListener
    {
        private ContainerEventType _Type;
        private ContainerEventHandler _ContainerEvent;
        public ContainerEventListener(ContainerEventType type, ContainerEventHandler e)
        {
            _Type = type;
            _ContainerEvent = e;
        }
        public ContainerEventType EventType
        {
            get { return _Type; }
        }
        public ContainerEventHandler ContainerEvent
        {
            get { return _ContainerEvent; }
        }
    }
}
