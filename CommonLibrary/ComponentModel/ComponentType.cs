using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.ComponentModel
{
    public class ComponentType
    {
        private Type _BaseType;
        private ComponentBuilderCollection _ComponentBuilders = new ComponentBuilderCollection();
        public Type BaseType
        {
            get { return _BaseType; }
        }
        public ComponentBuilderCollection ComponentBuilders
        {
            get { return _ComponentBuilders; }
        }
        public ComponentType(Type baseType)
        {
            _BaseType = baseType;
        }
    }
}
