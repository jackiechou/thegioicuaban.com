using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.UI.Skins.EventListeners;

namespace CommonLibrary.Application
{
    public class AppContext
    {
        private Application _Application;
        private List<SkinEventListener> _SkinEventListeners;
        private List<ContainerEventListener> _ContainerEventListeners;
        private static AppContext _Current;
        protected AppContext()
            : this(new Application())
        {
        }
        protected AppContext(Application application)
        {
            _Application = application;
            _ContainerEventListeners = new List<ContainerEventListener>();
            _SkinEventListeners = new List<SkinEventListener>();
        }
        public Application Application
        {
            get { return _Application; }
        }
        public List<ContainerEventListener> ContainerEventListeners
        {
            get { return _ContainerEventListeners; }
        }
        public List<SkinEventListener> SkinEventListeners
        {
            get { return _SkinEventListeners; }
        }
        public static AppContext Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new AppContext();
                }
                return _Current;
            }
            set { _Current = value; }
        }
    }
}
