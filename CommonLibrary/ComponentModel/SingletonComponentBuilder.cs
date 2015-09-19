using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Framework;

namespace CommonLibrary.ComponentModel
{
    public class SingletonComponentBuilder : IComponentBuilder
    {
        private string _Name;
        private Type _Type;
        private object _Instance;
        public SingletonComponentBuilder(string name, Type type)
        {
            _Name = name;
            _Type = type;
        }
        private void CreateInstance()
        {
            _Instance = Reflection.CreateInstance(_Type);
        }
        public object BuildComponent()
        {
            if (_Instance == null)
            {
                CreateInstance();
            }
            return _Instance;
        }
        public string Name
        {
            get { return _Name; }
        }
    }
}
