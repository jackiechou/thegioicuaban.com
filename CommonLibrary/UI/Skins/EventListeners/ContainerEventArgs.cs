using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Containers;

namespace CommonLibrary.UI.Skins.EventListeners
{
    public class ContainerEventArgs : EventArgs
    {
        private Container _Container;
        public ContainerEventArgs(Container container)
        {
            _Container = container;
        }
        public Container Container
        {
            get { return _Container; }
        }
    }
}
