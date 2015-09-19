using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.ComponentModel
{
    public interface IComponentBuilder
    {
        string Name { get; }
        object BuildComponent();
    }
}
