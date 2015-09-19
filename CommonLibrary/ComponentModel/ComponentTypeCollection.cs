using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonLibrary.ComponentModel
{
    public class ComponentTypeCollection : KeyedCollection<Type, ComponentType>
    {
        protected override Type GetKeyForItem(ComponentType item)
        {
            return item.BaseType;
        }
    }
}
