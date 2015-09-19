using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonLibrary.ComponentModel
{
    public class ComponentBuilderCollection : KeyedCollection<string, IComponentBuilder>
    {
        protected override string GetKeyForItem(IComponentBuilder item)
        {
            return item.Name;
        }
    }
}
