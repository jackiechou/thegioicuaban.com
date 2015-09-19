using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Entities.Modules
{
    public interface IActionable
    {
        Actions.ModuleActionCollection ModuleActions { get; }
    }
}
