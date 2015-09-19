using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.Framework;

namespace CommonLibrary.ComponentModel
{
    public class TransientComponentBuilder : IComponentBuilder
    {
        private string _Name;
        private Type _Type;
        public TransientComponentBuilder(string name, Type type)
        {
            _Name = name;
            _Type = type;
        }
        public object BuildComponent()
        {
            return Reflection.CreateInstance(_Type);
        }
        public string Name
        {
            get { return _Name; }
        }
    }
}
