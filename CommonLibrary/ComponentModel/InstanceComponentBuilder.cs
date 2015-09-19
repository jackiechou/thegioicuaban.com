using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.ComponentModel
{
    public class InstanceComponentBuilder : IComponentBuilder
    {
        private string _Name;
        private object _Instance;
        public InstanceComponentBuilder(string name, object instance)
        {
            _Name = Name;
            _Instance = instance;
        }
        public object BuildComponent()
        {
            return _Instance;
        }
        public string Name
        {
            get { return _Name; }
        }
    }
}
